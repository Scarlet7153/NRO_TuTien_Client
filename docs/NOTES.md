# NRO_TuTien_Client — Tài Liệu Kiến Trúc & Tái Cấu Trúc Tổng Hợp

> Tài liệu đầu mối tổng hợp cho cả 2 luồng công việc: **Track N** (Network & Performance Refactor) và **Track U** (UI/UX Tu Tiên Redesign).  
> Cập nhật: 10/10/2026 — **Sprint 1 (Analysis & Architecture Phase) ĐÃ HOÀN TẤT 100%**.

---

## 1. Bản Đồ Thư Mục Tài Liệu (`docs/`)

```text
docs/
├── NOTES.md                     <-- File này (Đầu mối tổng hợp, hiện trạng, sai lệch mã nguồn)
├── refactor/                    <-- Track N: Network & Performance
│   ├── PROTOCOL.md              <-- Đặc tả giao thức mạng Client ↔ Server (đối chiếu mã nguồn 2 chiều)
│   ├── THREADS.md               <-- Kiểm kê toàn bộ Thread / Thread.Sleep / MainThread rủi ro
│   └── TEXTURE_LIFECYCLE.md     <-- Vòng đời bộ nhớ Texture / VRAM leak / Đề xuất LRU cache
└── ui/                          <-- Track U: Tu Tiên UI/UX Overhaul
    ├── PANEL_INVENTORY.md       <-- Kiểm kê toàn diện 26 Panel Types + NewPanel + Các Screen
    ├── HUD_INVENTORY.md         <-- Kiểm kê HUD chiến đấu GameScr (HP/MP, Joystick, Kỹ năng, Commands)
    └── SERVER_ADDITIONS.md      <-- Phân định dữ liệu Client Tự Suy Diễn vs Đề xuất Server (Draft)
```

---

## 2. Quy Ước Ký Hiệu Kiểm Định (Verification Badges)

- **`[A]` — Agent Verified**: Đã được AI Agent đối chiếu trực tiếp qua mã nguồn C# và Java, khớp tên hàm, số dòng, logic nhị phân.
- **`[H]` — Human / Unity Run Needed**: Cần lập trình viên mở Unity Editor bấm Play, kiểm tra log console hoặc FPS/bộ nhớ thực tế.
- **`[D]` — Design Review Needed**: Cần thẩm định thẩm mỹ, mỹ thuật giao diện Tu Tiên hoặc trải nghiệm người dùng (UX).

---

## 3. Nhật Ký Tiến Độ Phân Tích (Sprint 1: T1 → T7)

| Task | Mã Định Danh | Nội Dung Công Việc | File Sản Phẩm | Trạng Thái |
|:---:|:---:|:---|:---|:---:|
| **T1** | `P0.1` | Khởi tạo cấu trúc thư mục `docs/ui/`, `docs/NOTES.md`, nhánh phân tích | `docs/NOTES.md` | **Hoàn thành `[A]`** |
| **T2** | `P0.2` | Đối chiếu giao thức mạng 2 chiều Client ↔ Server Java (`MessageSendCollect.java`) | `docs/refactor/PROTOCOL.md` | **Hoàn thành `[A]`** |
| **T3** | `D0.1` | Kiểm kê toàn bộ 26 Panel Types, `NewPanel.cs` và phân nhóm 6 Hub Tu Tiên | `docs/ui/PANEL_INVENTORY.md` | **Hoàn thành `[A]`** |
| **T4** | `D0.1` | Kiểm kê giao diện HUD GameScr, các thanh chỉ số, điều khiển, Commands | `docs/ui/HUD_INVENTORY.md` | **Hoàn thành `[A]`** |
| **T5** | `P3.1` / `P2.2` | Kiểm kê đa luồng (`THREADS.md`) & Vòng đời bộ nhớ hình ảnh (`TEXTURE_LIFECYCLE.md`) | `docs/refactor/THREADS.md`<br>`docs/refactor/TEXTURE_LIFECYCLE.md` | **Hoàn thành `[A]`** |
| **T6** | `D0.3` | Dự thảo gói tin mở rộng Server & phân định Client Inferred | `docs/ui/SERVER_ADDITIONS.md` | **Hoàn thành `[A]`** |
| **T7** | Tổng kết | Cập nhật báo cáo tổng kết, danh mục sai lệch và kế hoạch Sprint 2 | `docs/NOTES.md` | **Hoàn thành `[A]`** |

---

## 4. Tổng Hợp Các Phát Hiện Quan Trọng Giữa FINAL_PLAN.md Và Thực Tế

| # | Khẳng Định Ban Đầu | Hiện Trạng Thực Tế Trong Mã Nguồn | Ý Nghĩa Kiến Trúc & Hướng Xử Lý |
|:---:|:---|:---|:---|
| **1** | `Session_ME.cs` có ~603 dòng monolith | Đã được refactor thành Facade (`Session_ME.cs`: 180 dòng, `Session_ME2.cs`: 177 dòng) dùng `GameConnection` | **P1.4 đã hoàn thành**. Kiến trúc mạng đã sạch sẽ, an toàn luồng `[A]`. |
| **2** | Client và Server đều xử lý BIG_SET hai chiều | **Chỉ Server gửi BIG_SET cho Client** (`MessageSendCollect.java:42`). Khi Client gửi lên, Server chỉ đọc 2-byte short (`MessageSendCollect.java:89`). | **Tuyệt đối không gửi gói tin 3-byte size từ Client lên Server**. Giới hạn gói Client $\rightarrow$ Server là 64KB `[A]`. |
| **3** | `NewPanel.cs` là hệ thống panel mới | `NewPanel.cs` chỉ là class stub rỗng 23 dòng (`Assets/Scripts/NewPanel.cs`) | Sẵn sàng làm vỏ bọc kế thừa / adapter cho UI Tu Tiên mới `[A]`. |
| **4** | Quản lý bộ nhớ Texture | Trong `Image.cs` và `SmallImage.cs` **hoàn toàn không có bất kỳ lệnh `Destroy(texture)`** nào | Giải thích hiện tượng phình RAM khi chơi lâu. Cần triển khai LRU Cache ở Sprint 2 `[A]`. |
| **5** | Đa luồng trong client | Đã dọn dẹp AutoLogin, Rms, SMS, Sound; nhưng thư mục mod `Assets/Scripts/Functions/` còn rất nhiều `new Thread` và `Thread.Sleep` | Cần quy hoạch dọn dẹp các luồng mod này sang Coroutine / Update Tick ở Sprint 2 `[A]`. |
| **6** | Dữ liệu giao diện Tu Tiên | Cần sửa Server để cung cấp dữ liệu tu vi, cảnh giới, phẩm cấp | **Khẳng định: MVP Track U KHÔNG CẦN SỬA SERVER**. 100% cảnh giới, linh căn, phẩm cấp có thể tự suy diễn từ `cPower`, `cgender`, `item` `[A]`. |

---

## 5. Lộ Trình Sẵn Sàng Cho Sprint 2

Sau khi Sprint 1 kết thúc với bộ tài liệu phân tích hoàn chỉnh:
- **Track N (Hiệu năng):**
  - `P2.2`: Triển khai LRU Texture Cache cho `SmallImage` có gọi `Destroy(texture)`.
  - `P3.1`: Dọn dẹp các `new Thread` và `Thread.Sleep` trong thư mục `Functions/`.
- **Track U (Giao diện Tu Tiên):**
  - Bắt đầu thiết kế Design System & Theme Tokens (Màu ngọc bích, vàng kim, mây khói, phông chữ thư pháp).
  - Triển khai Hub 1 (Bản Nguyên - Nhân Vật & Hành Trang) song song với HUD Thái Cực Âm Dương.
