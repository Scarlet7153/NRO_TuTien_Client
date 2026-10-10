# TÀI LIỆU ĐẶC TẢ GIAO THỨC MẠNG NRO (CLIENT ↔ SERVER)

> **Mã định danh:** `P0.2` (Bổ sung đối chiếu Server Java).  
> **Cập nhật:** 10/10/2026.  
> **Nguồn đối chiếu:**  
> - Client: `Session_ME.cs`, `Session_ME2.cs`, `Assets/Scripts/Net/StreamCipher.cs`, `FrameCodec.cs`, `GameConnection.cs`  
> - Server: `src/scarlet/network/example/MessageSendCollect.java`, `KeyHandler.java`, `CommandMessage.java`, `Message.java`  
> **Độ tin cậy:** `[A]` (Đã đối chiếu trực tiếp mã nguồn C# và Java).

---

## 1. Tổng Quan & Kiến Trúc Kết Nối

- **Giao thức truyền vận:** TCP (`TcpClient` với `NoDelay = true`).
- **Cấu hình mạng:**
  - IP mặc định: `192.168.2.4` (hoặc từ `data/config/server.properties:ip=192.168.2.4`)
  - Port mặc định: `14445` (`data/config/server.properties:port=14445`)
- **Hai phiên kết nối độc lập trong Client:**
  - `Session_ME`: Kết nối máy chủ logic chính (Game Server).
  - `Session_ME2`: Kết nối máy chủ tài nguyên phụ (Resource/Sub Server).
- **Hạn chế tần suất kết nối:** Khoảng cách tối thiểu 50ms giữa 2 lần gọi connect (`timeWaitConnect = currentTimeMillis() + 50`, `Session_ME.cs:318`).

---

## 2. Bắt Tay & Cơ Chế Mã Hóa Dòng (Stream Cipher Handshake)

Hệ thống sử dụng mã hóa dòng dạng XOR tuần tự với con trỏ xoay vòng (`curR`, `curW`).

```text
[Client]                                           [Server]
   | --- Connect TCP (port 14445) ------------------> |
   | <--- Socket Accepted --------------------------- |
   | --- Gửi Handshake request: cmd = -27 (len=0) ----> |
   | <--- Gửi Handshake response: cmd = -27 --------- |
   |      (Body: len_key + key_bytes_xor + extra_info) |
   | [Khôi phục key & kích hoạt Cipher]               |
   | ================== GIAO TIẾP ĐÃ MÃ HÓA ================== |
```

### 2.1. Quy trình Bắt Tay (Handshake - Command `-27`)

1. **Client gửi yêu cầu bắt tay:**
   - Ngay sau khi kết nối TCP mở thành công (`GameConnection.cs:97`, `Session_ME.cs:370`), Client gửi gói tin command `-27` (`CommandMessage.REQUEST_KEY = -27` tại `CommandMessage.java:23`) với payload rỗng (`data = null`, 2-byte short length = 0).
   - Lúc này `session.sentKey() == false` và Client `getKeyComplete == false`. Gói tin đi dạng RAW (không mã hóa).

2. **Server phát sinh và phản hồi khóa:**
   - Server gọi `KeyHandler.sendKey(ISession)` (`KeyHandler.java:14-29`).
   - Server sinh mảng `byte[] key = session.getKey()`.
   - Ghi vào Message:
     - 1 byte `key.length` (`KeyHandler.java:19`).
     - 1 byte `key[0]` (`KeyHandler.java:20`).
     - Các byte tiếp theo: `key[i] ^ key[i - 1]` với `i` từ 1 đến `key.length - 1` (`KeyHandler.java:21-23`).
   - Gửi Message `-27` về Client dạng RAW vì `session.sentKey()` chưa bật (`MessageSendCollect.java:42, 57, 67`).
   - Sau khi gửi xong, Server đánh dấu `session.setSentKey(true)` (`KeyHandler.java:26`).

3. **Client khôi phục khóa và bật mã hóa:**
   - Khi nhận gói `cmd == -27` (`Session_ME.cs:119-140` hoặc `GameConnection.cs:220`):
     - Đọc 1 byte `len` độ dài khóa.
     - Đọc `len` bytes vào mảng `key`.
     - Thực hiện giải mã mảng khóa:
       ```csharp
       // Session_ME.cs:129 / StreamCipher.TransformKey()
       for (int j = 0; j < key.Length - 1; j++) {
           key[j + 1] = (sbyte)(key[j + 1] ^ key[j]);
       }
       ```
       *(Thuật toán này đảo ngược chính xác bước `key[i] ^ key[i-1]` của Server)*.
     - Đặt `getKeyComplete = true`. Con trỏ `curR = 0`, `curW = 0`.
     - Đọc tiếp thông tin mở rộng (chưa mã hóa): `IP2` (UTF string), `PORT2` (Int32), `isConnect2` (Byte).

### 2.2. Thuật toán Cipher Byte (XOR Stream Cipher)

Mỗi byte đi qua mạng tiêu thụ tuần tự một byte trong mảng khóa `key` và xoay vòng chỉ số:

| Thao tác | Client (`StreamCipher.cs:30-55`) | Server (`MessageSendCollect.java:17-33`) | Khớp mã nguồn |
|:---|:---|:---|:---:|
| **Giải mã (readKey)** | `(sbyte)((key[curR++] & 0xFF) ^ (b & 0xFF));`<br>`curR %= key.Length;` | `(byte)((key[curR++] & 0xFF) ^ (b & 0xFF));`<br>`if (curR >= key.length) curR %= key.length;` | **100% Khớp `[A]`** |
| **Mã hóa (writeKey)** | `(sbyte)((key[curW++] & 0xFF) ^ (b & 0xFF));`<br>`curW %= key.Length;` | `(byte)((key[curW++] & 0xFF) ^ (b & 0xFF));`<br>`if (curW >= key.length) curW %= key.length;` | **100% Khớp `[A]`** |

---

## 3. Cấu Trúc Khung Tin Nhắn (Frame Format)

Giao thức phân biệt 2 loại gói tin: **Gói Tiêu Chuẩn (Standard Frame)** và **Gói Kích Thước Lớn (Big Frame / BIG_SET)**.

### 3.1. Tập Lệnh BIG_SET

- **Server quy định:** `{-32, -66, -74, 11, -67, -87, 66}` (`MessageSendCollect.java:47-49`).
- **Client Session_ME quy định:** `{-32, -66, -74, 11, -67, -87, 66}`.
- **Client Session_ME2 quy định:** `{-32, -66, -74, 11, -67, -87}` *(thiếu cmd 66)*.

Ý nghĩa các lệnh trong BIG_SET:
- `11`: Dữ liệu TileMap / Map Template.
- `-32`: Dữ liệu hình nền (Background image).
- `-66`: Dữ liệu Icon / Item image source.
- `-74`: Dữ liệu tài nguyên game nén (Big resource cache).
- `-67`: Dữ liệu hiệu ứng hình ảnh (Effect data).
- `-87`: Dữ liệu cập nhật phiên bản / Version asset data.
- `66`: Dữ liệu hình ảnh động vật thể (Mob / Character sprite image data).

---

### 3.2. Chiều SERVER → CLIENT (Server gửi, Client đọc)

#### A. Khi gửi gói thuộc BIG_SET:
- **Header:**
  - 1 byte `cmd`: `writeKey(cmd)`
  - 3 bytes độ dài (24-bit Little-Endian, mỗi byte trừ 128):
    - `b0 = writeKey((byte) (size & 0xFF)) - 128` (`MessageSendCollect.java:50`)
    - `b1 = writeKey((byte) ((size >> 8) & 0xFF)) - 128` (`MessageSendCollect.java:51`)
    - `b2 = writeKey((byte) ((size >> 16) & 0xFF)) - 128` (`MessageSendCollect.java:52`)
- **Client giải mã độ dài (`FrameCodec.cs:85-95` / `Session_ME.cs:145-155`):**
  - `num0 = (readKey(b0) + 128) & 0xFF`
  - `num1 = (readKey(b1) + 128) & 0xFF`
  - `num2 = (readKey(b2) + 128) & 0xFF`
  - `size = (num2 << 16) | (num1 << 8) | num0` (kích thước tối đa $2^{24}-1 \approx 16.7$ MB).
- **Payload:** `size` bytes, mỗi byte được mã hóa bằng `writeKey` ở Server và giải mã bằng `readKey` ở Client.

#### B. Khi gửi gói Tiêu Chuẩn (Non-BIG_SET):
- **Header:**
  - 1 byte `cmd`: `writeKey(cmd)` (nếu đã sentKey) hoặc raw `cmd`
  - 2 bytes độ dài (16-bit Big-Endian):
    - Nếu đã sentKey: `writeKey((size >> 8) & 0xFF)` rồi `writeKey(size & 0xFF)` (`MessageSendCollect.java:54-55`).
    - Nếu chưa sentKey: `dos.writeShort(size)` (`MessageSendCollect.java:57`).
- **Client giải mã độ dài:**
  - Nếu `getKeyComplete`: `size = ((readKey(b1) & 0xFF) << 8) | (readKey(b2) & 0xFF)` (`Session_ME.cs:189`).
  - Nếu chưa có key (Quirk cũ): `size = (b1 & 0xFF00) | (b2 & 0xFF)` (`Session_ME.cs:195`).
- **Payload:** `size` bytes.

---

### 3.3. Chiều CLIENT → SERVER (Client gửi, Server đọc)

> [!CRITICAL]
> **PHÁT HIỆN KIẾN TRÚC QUAN TRỌNG:**  
> Tại Server (`MessageSendCollect.java:79-114`), hàm `readMessage` **hoàn toàn KHÔNG xử lý BIG_SET**!  
> Server luôn luôn đọc 2 byte độ dài:  
> `size = ((readKey(session, b1) & 0xFF) << 8) | (readKey(session, b2) & 0xFF);` (`MessageSendCollect.java:88`)  
> Kèm điều kiện bảo vệ: `if (size < 0 || size > 65535) throw new IOException(...)` (`MessageSendCollect.java:93-95`).

**Hệ quả & Quy tắc bắt buộc đối với Client:**
1. **Client gửi lên Server tối đa 65,535 bytes (64 KB)** cho mỗi gói tin.
2. **Client KHÔNG BAO GIỜ gửi gói tin dạng 3-byte size lên Server**. Các lệnh trong BIG_SET chỉ là các lệnh tài nguyên do Server PUSH về Client. Khi Client yêu cầu tài nguyên (ví dụ gửi cmd `66` để xin ảnh), Client gửi qua `Service.cs` dưới dạng gói tin thông thường có tham số (ví dụ `short id`), độ dài payload chỉ vài bytes và header độ dài vẫn là 2 byte short tiêu chuẩn.

---

## 4. Bảng So Sánh Hai Chiều Chi Tiết (Client ↔ Server Cross-Reference)

| Đặc tính / Hành vi | Phía Client (C#) | Phía Server (Java) | Tình trạng khớp |
|:---|:---|:---|:---:|
| **Cổng kết nối TCP** | Mặc định 14445 (`Session_ME.cs:310`) | Mặc định 14445 (`server.properties:port`) | Khớp 100% `[A]` |
| **Lệnh Handshake** | Gửi `-27` rỗng (`Session_ME.cs:370`) | Nhận và xử lý `-27` (`KeyHandler.java:16`) | Khớp 100% `[A]` |
| **Biến đổi Key** | `key[j+1] ^= key[j]` (`Session_ME.cs:129`) | `dos.writeByte(key[i] ^ key[i-1])` (`KeyHandler.java:22`) | Khớp đối ngẫu 100% `[A]` |
| **Thứ tự xoay Key** | `curR` tăng sau khi XOR (`StreamCipher.cs:37`) | `curR++` tăng sau khi XOR (`MessageSendCollect.java:19`) | Khớp 100% `[A]` |
| **BIG_SET Server → Client** | Giải mã 3-byte LE + 128 (`Session_ME.cs:150`) | Ghi 3-byte LE - 128 (`MessageSendCollect.java:50-52`) | Khớp 100% `[A]` |
| **BIG_SET Client → Server** | Client `Session_ME` cũ có hàm gửi 3 byte | **Server KHÔNG hỗ trợ 3-byte nhận** (`MessageSendCollect.java:88`) | **Đã bảo vệ**: Client không gửi payload BIG_SET `[A]` |
| **Giới hạn kích thước gói** | `MaxFrameBytes = 8 * 1024 * 1024` (8MB) | Client→Server: max 65,535 B; Server→Client: max 16.7 MB | An toàn cho Game `[A]` |
| **Xử lý ngắt kết nối** | Timeout 500ms phân biệt `Fail` vs `Disconnected` | Bắt `EOFException` / `SocketException` đóng session | Khớp nghiệp vụ `[A]` |

---

## 5. Danh Sách Gói Lệnh Tiêu Biểu & Phân Luồng

| Cmd | Tên / Chức Năng | Chiều | Xử Lý Phía Client | Xử Lý Phía Server |
|:---:|:---|:---:|:---|:---|
| `-27` | Handshake Key | C ↔ S | `Session_ME.cs:119`, nạp cipher key | `KeyHandler.java:16`, phát sinh key |
| `-32` | Background Resource | S → C | `Controller.cs`, nạp ảnh nền map | `MessageSendCollect.java:47` (BIG_SET) |
| `-66` | Icon / Item Image | S → C | `Controller.cs`, nạp icon vào cache | `MessageSendCollect.java:47` (BIG_SET) |
| `-74` | Big Resource Bundle | S → C | `Controller.cs`, giải nén tài nguyên lớn | `MessageSendCollect.java:47` (BIG_SET) |
| `11` | TileMap Template | S → C | `GameScr.cs`, vẽ ma trận bản đồ | `MessageSendCollect.java:48` (BIG_SET) |
| `66` | Character/Mob Image | S → C | `Controller.cs`, nạp sprite quái/nhân vật | `MessageSendCollect.java:49` (BIG_SET) |
| `-105` | Tu Tiên / Special Action | C → S | `Service.cs`, gửi yêu cầu tính năng | Phân luồng Controller server |
| `1` | Login Request | C → S | `Service.cs:login()`, gửi user/pass | `LoginHandler.java`, xác thực DB |

---

## 6. Khuyến Nghị Kiểm Thử Tự Động (P1.2 Golden Tests)

1. **Test Round-Trip Handshake:**
   - Dữ liệu giả lập key byte từ Server đi qua biến đổi `KeyHandler`, đưa vào `StreamCipher.TransformKey`, xác nhận khôi phục 100% vector ban đầu.
2. **Test BIG_SET Framing:**
   - Frame kích thước 70,000 bytes (vượt quá 65535) với cmd `11`: mã hóa bằng công thức Server `MessageSendCollect`, giải mã bằng `FrameCodec`, xác nhận nguyên vẹn dữ liệu.
3. **Test Boundary Exception:**
   - Gói tin có length âm hoặc vượt quá `MaxFrameBytes` (8MB) phải ngắt kết nối an toàn, ném ngoại lệ rõ ràng, không làm crash luồng chính Unity.
