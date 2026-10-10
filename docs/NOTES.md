# NRO_TuTien_Client — Tài Liệu Kiến Trúc & Tái Cấu Trúc

> Tài liệu đầu mối tổng hợp cho cả 2 luồng công việc: **Track N** (Network & Performance Refactor) và **Track U** (UI/UX Tu Tiên Redesign).  
> Cập nhật lần đầu: 10/10/2026 (Sprint 1 - Analysis Phase).

---

## 1. Bản Đồ Thư Mục Tài Liệu (`docs/`)

```text
docs/
├── NOTES.md                     <-- File này (Đầu mối tổng hợp, trạng thái, sai lệch phát hiện)
├── refactor/                    <-- Track N: Network & Performance
│   ├── PROTOCOL.md              <-- Đặc tả giao thức mạng Client ↔ Server (đối chiếu mã nguồn)
│   ├── THREADS.md               <-- Kiểm kê toàn bộ Thread / async / MainThread
│   └── TEXTURE_LIFECYCLE.md     <-- Vòng đời bộ nhớ Texture / Image / Asset
└── ui/                          <-- Track U: Tu Tiên UI/UX Overhaul
    ├── PANEL_INVENTORY.md       <-- Kiểm kê toàn bộ Panel & Screen (26 types + Screens)
    ├── HUD_INVENTORY.md         <-- Kiểm kê HUD chiến đấu & nút điều khiển (GameScr)
    └── SERVER_ADDITIONS.md      <-- Đề xuất gói tin server bổ sung (Draft)
```

---

## 2. Quy Ước Ký Hiệu Kiểm Định (Verification Badges)

Vì AI Agent không sở hữu Unity Editor và không tự chạy runtime, mọi hạng mục kiểm định được gắn thẻ minh bạch:

- **`[A]` — Agent Verified**: Đã được AI Agent đối chiếu trực tiếp qua mã nguồn C# và Java, khớp tên hàm, số dòng, logic nhị phân.
- **`[H]` — Human / Unity Run Needed**: Cần lập trình viên mở Unity Editor bấm Play, kiểm tra log console hoặc FPS/bộ nhớ thực tế.
- **`[D]` — Design Review Needed**: Cần thẩm định thẩm mỹ, mỹ thuật giao diện Tu Tiên hoặc trải nghiệm người dùng (UX).
- **`[Cần xác nhận]`**: Vấn đề chưa đủ dữ kiện mã nguồn hoặc có nghi vấn nghiệp vụ cần quyết định.

---

## 3. Nhật Ký Tiến Độ Phân Tích (Sprint 1)

| Task | Mục Tiêu | Trạng Thái | File Sản Phẩm |
|:---|:---|:---:|:---|
| **T1** | Cấu trúc thư mục docs & branching | Hoàn thành | `docs/NOTES.md` |
| **T2** | Bổ sung PROTOCOL.md đối chiếu Server Java | Đang thực hiện | `docs/refactor/PROTOCOL.md` |
| **T3** | Phân tích toàn diện 26 Panel Types + Screens | Chờ xử lý | `docs/ui/PANEL_INVENTORY.md` |
| **T4** | Phân tích HUD GameScr (HP/MP, Chat, Joystick, Radar) | Chờ xử lý | `docs/ui/HUD_INVENTORY.md` |
| **T5** | Kiểm kê Thread & Vòng đời bộ nhớ Texture | Chờ xử lý | `docs/refactor/THREADS.md`, `TEXTURE_LIFECYCLE.md` |
| **T6** | Dự thảo gói tin mở rộng Server (Tu Tiên data) | Chờ xử lý | `docs/ui/SERVER_ADDITIONS.md` |
| **T7** | Cập nhật tổng kết sai lệch và danh sách kiểm tra | Chờ xử lý | `docs/NOTES.md` |

---

## 4. Bảng Sai Lệch Phát Hiện Giữa FINAL_PLAN.md Và Code Thực Tế

| # | Khẳng Định Trong FINAL_PLAN.md | Hiện Trạng Thực Tế Trong Code | Đánh Giá & Hướng Xử Lý |
|:---|:---|:---|:---|
| 1 | `Session_ME.cs` có ~603 dòng monolith | Đã được refactor thành Facade (`Session_ME.cs`: 180 dòng, `Session_ME2.cs`: 177 dòng) dùng `GameConnection` | **Đã hoàn thành P1.4**. Không cần viết lại từ đầu, chỉ cần bảo trì và bổ sung test. |
| 2 | Chưa có `NRO.Net.asmdef` | Đã có assembly `NRO.Net` và `NRO.Net.Tests` | **Đã hoàn thành P1.1**. Các test đã compile và pass độc lập. |
| 3 | `Session_ME.update()` bị gọi ở 4 nơi | Chỉ còn 1 cuộc gọi duy nhất tại `Main.cs:338` trong hàm `Update()` | **Đã hoàn thành P1.4 / P3.2**. Đã sạch polling rải rác. |
| 4 | Client và Server đều dùng BIG_SET hai chiều | **Chỉ Server gửi BIG_SET cho Client** (`MessageSendCollect.java:42`). Khi Client gửi lên, Server chỉ đọc 2-byte short (`MessageSendCollect.java:89`). | Client gửi lên Server tối đa 65,535 bytes. Client nhận tối đa 16MB (3-byte length). Giới hạn `MaxFrameBytes = 8MB` là an toàn. |
| 5 | Các luồng AutoLogin, Rms, SMS, Sound tạo Thread liên tục | Đã loại bỏ luồng AutoLogin trái phép, Rms/SMS/Sound dùng `MainThread.IsMain` | **Đã hoàn thành P3.2, P3.3**. |
