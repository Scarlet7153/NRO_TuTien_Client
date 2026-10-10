# KIỂM KÊ & QUY HOẠCH LUỒNG (THREAD INVENTORY & AUDIT)

> **Mã định danh:** `P3.1` / Sprint 1 Task T5.  
> **Cập nhật:** 10/10/2026.  
> **Nguồn đối chiếu:** Toàn bộ thư mục `Assets/Scripts/` (Đặc biệt: `Net/GameConnection.cs`, `Functions/`, `Image.cs`, `Util/MainThread.cs`).  
> **Độ tin cậy:** `[A]` (Kiểm tra trực tiếp mã nguồn C#).

---

## 1. Tổng Quan Thực Trạng Đa Luồng

Trong các phiên bản Dragon Boy / NRO truyền thống (gốc Java J2ME chuyển đổi sang Unity C#), kiến trúc đa luồng thường bị lạm dụng:
- Tạo thread tùy tiện bằng `new Thread(...)` không đặt tên, không có vòng đời quản lý, không bắt ngoại lệ.
- Sử dụng `Thread.Sleep(...)` rải rác để đồng bộ hóa hoặc tạo độ trễ logic, gây block luồng ngầm hoặc tiêu hao tài nguyên CPU vô ích.
- Gọi trực tiếp API Unity (`Image.createImage`, nạp resource) từ luồng phụ, gây nguy cơ Crash ứng dụng trên các bản build IL2CPP / Mobile.

**Các bước đã refactor thành công trong giai đoạn P1–P3:**
- Đã quy hoạch kết nối mạng vào `GameConnection.cs` (`NRO.Net`) với 3 luồng rõ ràng: `_connectThread`, `_readThread`, `_writeThread` (có cờ `IsBackground = true`, token thế hệ kết nối `_generation`).
- Đã loại bỏ luồng ngầm phi pháp `AutoLogin` (commit `6912ead`).
- Đã chuyển đổi `Rms`, `SMS`, `Sound` sang sử dụng `MainThread.IsMain` và `AutoResetEvent` thay vì `Thread.Sleep` polling (commit `fe58dce`).

---

## 2. Bảng Danh Mục Kiểm Kê Luồng Hiện Hữu (Thread Inventory)

| Vị Trí File & Dòng | Mục Đích Tạo Luồng | Cơ Chế Triển Khai | Rủi Ro Nhận Diện | Phân Loại Xử Lý |
|:---|:---|:---|:---|:---:|
| `Net/GameConnection.cs:61`<br>`Net/GameConnection.cs:93`<br>`Net/GameConnection.cs:94` | Kết nối TCP, Đọc gói tin mạng (`ReadLoop`), Ghi gói tin mạng (`WriteLoop`) | `new Thread(ConnectLoop)`<br>`new Thread(ReadLoop)`<br>`new Thread(WriteLoop)`<br>(có `IsBackground = true`) | **Đã kiểm soát tốt:** Quản lý bằng `_generation`, tự hủy khi đóng socket, hàng đợi an toàn | **Giữ nguyên chuẩn hóa** `[A]` |
| `Functions/HandlerFunctions/`<br>`ItemHandler.cs:30, 38, 127, 147` | Thao tác tự động gom vật phẩm, lọc đồ rác trong túi | `new Thread(delegate () { ... })` vô danh | **Rủi ro cao:** Không có cơ chế hủy khi đổi map/chết nhân vật, gọi trực tiếp `Service.gI()` rải rác | **Cần refactor sang Coroutine / Update Tick** `[A]` |
| `Functions/AutoFunctions/`<br>`AutoSpecialSkillFunctions.cs:138, 149, 240, 350` | Tự động nâng nội tại / quay kỹ năng đặc biệt | `new Thread(new ThreadStart(open)).Start()` | **Rủi ro cao:** Spam thread liên tục mỗi lần người chơi bấm nút mở bảng | **Cần gộp vào State Machine** `[A]` |
| `Functions/AutoFunctions/`<br>`AutoCrackBallFunctions.cs:168, 189, 206, 343` | Tự động quay số ngọc rồng / nhận thưởng | `new Thread(new ThreadStart(runnhando)).Start()` | **Rủi ro cao:** Thread chạy ngầm vô hạn, không đồng bộ với trạng thái mạng của server | **Cần chuyển sang Async/Await hoặc Task** `[A]` |
| `Functions/AutoFunctions/`<br>`AutoBuyFunctions.cs:100, 121` | Tự động mua vật phẩm shop | `new Thread(delegate () ...)` | **Rủi ro trung bình:** Dễ spam request gây nghẽn kết nối hoặc bị server kick | **Cần giới hạn tần suất (Rate Limit)** `[A]` |
| `Functions/MainFunctions.cs:103, 124` | Khởi động luồng đập đồ tự động, vòng quay | `new Thread(AutoDapDo).Start()` | **Rủi ro cao:** Tạo thread nền không thể ngắt khi người chơi đóng game | **Cần Coroutine quản lý vòng đời** `[A]` |

---

## 3. Bảng Kiểm Kê `Thread.Sleep` (Chờ Đợi Khối Luồng)

| Vị Trí File & Dòng | Thời Gian Sleep | Nguyên Nhân Sử Dụng | Giải Pháp Thay Thế |
|:---|:---:|:---|:---|
| `Image.cs:183, 210, 237, 269, 297` | `5 ms` | Chờ đợi luồng tải ảnh Texture nền hoàn thành (`while (isWaiting) Thread.Sleep(5)`) | Thay thế bằng `TaskCompletionSource` hoặc tải ảnh bất đồng bộ qua Unity `UnityWebRequest` |
| `AutoUpgradeFunctions.cs:85, 89, 162, 174` | `100 - 500 ms` | Tạo delay giữa các lần đập đồ để chờ Server trả kết quả | Dùng event listener phản hồi từ gói tin Server thay vì sleep mù |
| `ItemHandler.cs:218, 233, 235, 258, 338, 361` | `100 - 1000 ms` | Chờ nhặt từng món đồ trên mặt đất | Dùng bộ đếm game tick (`GameCanvas.gameTick`) trong hàm `update()` |
| `AutoCrackBallFunctions.cs:54, 63, 72, 78, 106, 214...` | `200 - 2000 ms` | Giả lập thao tác người chơi quay số | Chuyển sang Coroutine `yield return new WaitForSeconds(...)` |

---

## 4. Quy Tắc & Tiêu Chuẩn Cho Giai Đoạn Tiếp Theo

1. **Quy tắc Vàng Unity MainThread:**
   - Tuyệt đối KHÔNG gọi bất kỳ API nào của Unity (`GameObject`, `Transform`, `Texture2D`, `Resources`, `PlayerPrefs`) từ các luồng do `System.Threading.Thread` tạo ra.
   - Khi luồng mạng hoặc luồng worker cần cập nhật giao diện, bắt buộc đẩy dữ liệu vào hàng đợi an toàn hoặc gọi qua `MainThread.QueueAction(...)`.
2. **Quy hoạch dọn dẹp các tính năng Auto:**
   - Không tạo `new Thread` trong các class mod `Functions/`. Thay vào đó, tích hợp vào vòng lặp `update()` chính của game với cờ trạng thái rõ ràng, đảm bảo khi người chơi chuyển map hoặc ngắt kết nối thì toàn bộ auto dừng ngay lập tức.
