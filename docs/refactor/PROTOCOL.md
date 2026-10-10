# TÀI LIỆU GIAO THỨC MẠNG NRO

Tài liệu này mô tả chi tiết giao thức mạng giữa Client và Server của trò chơi, được dịch ngược từ mã nguồn `Session_ME.cs` và `Session_ME2.cs`. Bất kỳ thay đổi nào ở Client cũng phải tuân thủ nghiêm ngặt các quy tắc này để đảm bảo giao tiếp không bị lỗi.

## 1. Cơ chế mã hoá (Stream Cipher)
Giao thức sử dụng một dạng mã hoá XOR stream đơn giản, với hai con trỏ đọc/ghi độc lập.

*   **Nhận Khóa (Key):**
    Khóa là một mảng `sbyte` do server gửi về (thông qua lệnh `-27`). Ngay sau khi nhận, mảng này được biến đổi một lần duy nhất trước khi sử dụng:
    ```csharp
    // Session_ME.cs:129
    for (int j = 0; j < key.Length - 1; j++) {
        key[j + 1] = (sbyte)(key[j + 1] ^ key[j]);
    }
    ```
*   **Trạng thái Cipher:**
    Có 2 con trỏ `curR` (dành cho việc đọc) và `curW` (dành cho việc ghi). Cả hai đều khởi tạo bằng 0. Khi gọi `cleanNetwork()`, cả hai đều bị reset về 0 (Session_ME.cs:509).

*   **Hàm Giải Mã / Mã Hoá:**
    Mỗi byte đi qua cipher đều tiêu thụ 1 vị trí key theo tuần tự và xoay vòng theo độ dài của key.
    ```csharp
    // Session_ME.cs:443 (readKey)
    // Session_ME.cs:456 (writeKey)
    sbyte result = (sbyte)((key[cur] & 0xFF) ^ (b & 0xFF));
    cur = (sbyte)((cur + 1) % key.Length);
    return result;
    ```
    **Chú ý quan trọng:** Mỗi byte của gói tin (bao gồm `cmd`, các byte `length`, và toàn bộ `payload`) đều phải đi qua cipher nếu cờ `getKeyComplete == true`.

## 2. Quá trình Nhận (Server → Client)

1.  **Đọc Lệnh (Command):**
    Đọc 1 byte `cmd` đầu tiên. Nếu `getKeyComplete` là `true`, byte này được giải mã bằng `readKey(b)`.

2.  **Đọc Gói Lớn (BIG_SET):**
    Nếu `cmd` thuộc vào tập `BIG_SET` của Session (Session_ME: `{-32, -66, 11, -67, -74, -87, 66}`, Session_ME2: `{-32, -66, 11, -67, -74, -87}`):
    *   Đọc tiếp 3 byte liên tiếp.
    *   Giải mã từng byte và cộng thêm 128: `num = readKey(b) + 128` (Session_ME.cs:150)
    *   Độ dài (24-bit Little-Endian): `len = (num3 * 256 + num2) * 256 + num`
    *   Đọc `len` byte payload, sau đó nếu `getKeyComplete == true`, giải mã từng byte payload bằng `readKey`.

3.  **Đọc Gói Thường:**
    Nếu `cmd` không thuộc `BIG_SET`:
    *   Nếu `getKeyComplete == true`: Đọc 2 byte, giải mã từng byte, ráp lại thành độ dài 16-bit Big-Endian:
        `len = ((readKey(b2) & 0xFF) << 8) | (readKey(b3) & 0xFF)` (Session_ME.cs:189)
    *   Nếu `getKeyComplete == false` (*Quirk*): Đọc 2 byte gốc, ráp lại theo công thức:
        `len = (b4 & 0xFF00) | (b5 & 0xFF)` (Session_ME.cs:195). Do `b4` là `sbyte`, các bit cao có thể mang dấu âm. Client giữ nguyên quirk này.
    *   Đọc `len` byte payload, sau đó nếu `getKeyComplete == true`, giải mã từng byte payload bằng `readKey`.

4.  **Gói Handshake (Lệnh -27):**
    *   Khi nhận gói `cmd == -27` (Session_ME.cs:119), Client đọc phần payload (chưa bị mã hóa):
        *   1 byte `b`: độ dài key.
        *   Đọc `b` bytes để nạp mảng `key`.
        *   Kích hoạt biến đổi mảng key `key[j+1] ^= key[j]`.
        *   Gán `getKeyComplete = true`.
        *   Sau đó tiếp tục đọc (sử dụng con trỏ DataInputStream cũ): UTF `IP2`, Int `PORT2`, Byte `isConnect2`.

## 3. Quá trình Gửi (Client → Server)

Gửi tin nhắn diễn ra tại hàm `doSendMessage(Message)` (Session_ME.cs:381).

*   **Chưa có Key (`getKeyComplete == false`):**
    *   Ghi `cmd` (không mã hoá).
    *   Nếu có payload (`data != null`), ghi 2 byte độ dài (Little-Endian thông qua `BinaryWriter.Write(ushort)`). **Đặc biệt:** Không hề ghi phần `data` vào luồng mạng! (Quirk của phiên bản cũ, chỉ gói `-27` được gửi lúc này và nó thường có data null).
    *   Nếu `data == null`, ghi `ushort 0` (2 byte Little-Endian `0x00 0x00`).
*   **Đã có Key (`getKeyComplete == true`):**
    *   Mã hoá và ghi `cmd`: `writeKey(cmd)`.
    *   Mã hoá và ghi 2 byte độ dài (Big-Endian): `writeKey(len >> 8)` và `writeKey(len & 0xFF)`.
    *   Nếu có payload, lặp qua từng byte, mã hoá `writeKey(data[i])` và ghi.

Sau mỗi gói tin, Client luôn gọi `dos.Flush()`.

## 4. Quá trình Kết Nối & Vòng Đời

*   **Socket:** Sử dụng `TcpClient` với cấu hình `NoDelay = true`.
*   **Host mặc định:** Nếu host truyền vào rỗng hoặc `"127.0.0.0"`, sẽ dùng IP `192.168.2.4` và port `14445` (chỉ áp dụng ở Session_ME, dòng 310).
*   **Chống Spam Connect:** Phải cách nhau tối thiểu 50ms giữa 2 lần kết nối `timeWaitConnect = currentTimeMillis() + 50` (Session_ME.cs:318).
*   **Sự Kiện Disconnect:** Tại luồng nhận (MessageCollector), nếu kết thúc luồng mà `currentTimeMillis() - timeConnected > 500`, Client sẽ gọi `onDisconnected`. Nếu dưới 500ms, sẽ gọi `onConnectionFail` (Session_ME.cs:104).
*   **Gửi Handshake đầu tiên:** Ngay sau khi socket mở thành công, Client reset `key = null` và gửi gói `-27` rỗng (Session_ME.cs:370).
