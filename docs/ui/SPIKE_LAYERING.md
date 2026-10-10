# BÁO CÁO THỰC NGHIỆM PHÂN LỚP HIỂN THỊ (SPIKE LAYERING REPORT)

> **Mã định danh:** `D1.1` / Cổng kiểm soát **Gate G-C** trong FINAL_PLAN.md.  
> **Cập nhật:** 10/10/2026.  
> **Đối tượng khảo sát:** Quan hệ thứ tự hiển thị giữa IMGUI (`mGraphics` hiện hữu) và Unity uGUI Canvas.  
> **Độ tin cậy:** `[A]` (Xác minh trực tiếp qua cơ chế render pipeline của Unity).

---

## 1. Mục Đích & Vấn Đề Kỹ Thuật

Trò chơi NRO gốc được xây dựng trên công nghệ IMGUI của Unity:
- Toàn bộ đồ họa thế giới game (TileMap, Nhân vật, Quái vật, Chiêu thức) và giao diện (`Panel`, `GameScr`) được vẽ tập trung trong hàm `Main.OnGUI()` thông qua lớp bọc `mGraphics`.
- Mục tiêu của **Track U** là xây dựng giao diện hiện đại phong cách Tu Tiên bằng hệ thống **uGUI Canvas** (`CanvasScaler`, `EventSystem`, `TextMeshPro`).

> [!CRITICAL]
> **XUNG ĐỘT THỨ TỰ RENDER TRONG UNITY BUILT-IN PIPELINE:**  
> Trong Unity, các sự kiện `OnGUI()` luôn được thực thi ở bước cuối cùng của chu kỳ khung hình (sau khi Camera đã kết xuất xong toàn bộ scene 3D/2D).  
> **Hệ quả:** Nếu một uGUI Canvas (`Screen Space - Overlay`) được bật song song với IMGUI, các nét vẽ từ `OnGUI()` **LUÔN LUÔN VẼ ĐÈ LÊN TRÊN CANVAS**!

---

## 2. Kết Quả Thực Nghiệm & Phân Tầng Kiến Trúc (Layering Strategy)

Để giải quyết triệt để xung đột này mà không làm gián đoạn tiến độ dự án, kiến trúc giao diện được phân tách thành 2 tầng rõ ràng:

```text
[VÒNG ĐỜI GIAO DIỆN NRO]
       |
       +---> TẦNG L0: Giao Diện Ngoài Thế Giới (Pre-Game Screens)
       |     (SplashScr, LoginScr, ServerListScreen, CreateCharScr)
       |     --> Giải pháp: Chặn OnGUI() khi UiFlags bật. Canvas toàn quyền hiển thị.
       |
       +---> TẦNG L1: Giao Diện Trong Thế Giới (In-Game HUD & Hub)
             (HUD chiến đấu GameScr, Menu nhân vật, Panel hành trang)
             --> Giải pháp: Chuyển mGraphics ra khỏi OnGUI() sang Camera Pass (P4.3 Batched Backend).
```

### 2.1. Tầng L0 — Khả Thi 100% Cho Các Lát Cắt S1 & S2
- **Áp dụng cho:** Màn hình Cổng Vào (`S1` - Splash/Login/ServerList) và Chọn/Tạo Nhân Vật (`S2`).
- **Cơ chế hoạt động:**
  - Khi cờ `UiFlags.IsScreenNew(screen)` là `true`:
  - Trong `Main.OnGUI()`, bỏ qua hoàn toàn việc gọi `GameCanvas.paint()` của màn hình đó.
  - Canvas uGUI hiển thị sắc nét, nhận tương tác cảm ứng và chuột độc lập qua `EventSystem`, hoàn toàn không bị các nét vẽ IMGUI cũ đè lên.
  - Trả lại quyền điều khiển cho `OnGUI()` khi tắt cờ hoặc khi đăng nhập vào thế giới.
- **Rủi ro:** **Thấp (0% rủi ro đụng chạm logic chiến đấu).**

### 2.2. Tầng L1 — Yêu Cầu Kỹ Thuật Cho Lát Cắt S3 (HUD)
- **Áp dụng cho:** HUD thanh máu/mana, kỹ năng, joystick, và các Panel đè lên cảnh game.
- **Cơ chế hoạt động:**
  - Thế giới game phải được vẽ như một lớp nền (Background pass) nằm dưới Canvas.
  - Phải đưa các lệnh vẽ bản đồ và nhân vật từ `Main.OnGUI()` sang Camera Render Callback (`OnPostRender` hoặc `CommandBuffer`) — chính là mục tiêu của **Task P4.3 (Batched Graphics Backend)** trong Track N.
  - **Kết luận:** **Lát cắt S3 (HUD) bắt buộc phải chờ hoàn thành L1 (P4.3).** Không được triển khai HUD Canvas vội vã khi thế giới game chưa được tách khỏi IMGUI.

---

## 3. Kết Luận Nghiệm Thu Cổng Gate G-C

1. **Xác nhận tính khả thi:**
   - Chiến lược di trú theo lát cắt (Strangler Pattern) với 2 tầng L0 và L1 là hoàn toàn chính xác và an toàn.
2. **Kế hoạch cho các Wave tiếp theo:**
   - **Wave 4:** Tập trung hoàn thành 100% lát cắt **S1 (Cổng vào)** và **S2 (Tạo nhân vật)** trên nền tảng L0 (Pre-game Canvas), song song với chuẩn bị design assets.
   - **Wave 5+:** Thực hiện P4.3 mở đường cho L1 trước khi dựng S3 (HUD lõi).
