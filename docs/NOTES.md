# NRO_TuTien_Client — Tài Liệu Kiến Trúc & Tái Cấu Trúc Tổng Hợp

> Tài liệu đầu mối tổng hợp cho cả 2 luồng công việc: **Track N** (Network & Performance Refactor) và **Track U** (UI/UX Tu Tiên Redesign).  
> Cập nhật: 10/10/2026 — **Sprint 4 (Wave 4 - S1 Cổng Vào & S2 Chọn/Tạo Nhân Vật) ĐÃ HOÀN TẤT CODE**.

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
    ├── SERVER_ADDITIONS.md      <-- Phân định dữ liệu Client Tự Suy Diễn vs Đề xuất Server (Draft)
    └── SPIKE_LAYERING.md        <-- Kết luận thực nghiệm thứ tự render IMGUI vs uGUI Canvas (Gate G-C)
```

---

## 2. Quy Ước Ký Hiệu Kiểm Định (Verification Badges)

- **`[A]` — Agent Verified**: Đã được AI Agent đối chiếu trực tiếp qua mã nguồn C# và Java, khớp tên hàm, số dòng, logic nhị phân, kiểm tra biên dịch cú pháp.
- **`[H]` — Human / Unity Run Needed**: Cần lập trình viên mở Unity Editor bấm Play, kiểm tra log console hoặc FPS/bộ nhớ thực tế.
- **`[D]` — Design Review Needed**: Cần thẩm định thẩm mỹ, mỹ thuật giao diện Tu Tiên hoặc trải nghiệm người dùng (UX).

---

## 3. Nhật Ký Tiến Độ Từng Sprint

### Sprint 1: Phân Tích & Tài Liệu Hóa (T1 → T7)
- **T1:** Cấu trúc docs & [NOTES.md](NOTES.md) `[A]`.
- **T2:** [PROTOCOL.md](refactor/PROTOCOL.md) đối chiếu Server Java `[A]`.
- **T3:** [PANEL_INVENTORY.md](ui/PANEL_INVENTORY.md) (26 panel types, Screens) `[A]`.
- **T4:** [HUD_INVENTORY.md](ui/HUD_INVENTORY.md) (GameScr HUD components) `[A]`.
- **T5:** [THREADS.md](refactor/THREADS.md) & [TEXTURE_LIFECYCLE.md](refactor/TEXTURE_LIFECYCLE.md) `[A]`.
- **T6:** [SERVER_ADDITIONS.md](ui/SERVER_ADDITIONS.md) (Client Inferred) `[A]`.
- **T7:** Cập nhật sai lệch `[A]`.

### Sprint 2: Nền Tảng Kỹ Thuật (Wave 2)
- **P0.3:** Triển khai [PerfProbe.cs](../Assets/Scripts/Util/PerfProbe.cs) bằng `ProfilerRecorder` (GC Allocated In Frame, Draw Calls), dev/editor-only, tự gắn qua `RuntimeInitializeOnLoadMethod`, toggle bằng phím `~` hoặc `F12` (không dùng F3) `[A]`.
- **D1.2:** Thiết lập hệ thống asmdef phân tầng theo mẫu Part 4.3:
  - `Assets/Scripts/UI/Contracts/NRO.UI.Contracts.asmdef` (`noEngineReferences: true`, `autoReferenced: true`) kèm `IPlayerState` (dùng kiểu `long`) và `ILocalizer` `[A]`.
  - `Assets/Scripts/UI/Core/NRO.UI.asmdef` (`references: ["NRO.UI.Contracts", "UnityEngine.UI", "Unity.TextMeshPro"]`) kèm `UiFlags` (tắt mặc định) và `Localizer` `[A]`.
  - `Assets/Scripts/UI/Tests/NRO.UI.Tests.asmdef` (Editor-only, `UNITY_INCLUDE_TESTS`) kèm `LocalizerTests` `[A]`.
  - `Assets/Scripts/UIBridge/PlayerStateAdapter.cs` & `LegacyResourcesLocalizerAdapter.cs` nằm trong `Assembly-CSharp` gốc kết nối an toàn `[A]`.
- **D1.6:** Hiện thực `ILocalizer`: Chỉ đọc `mResources` + JSON rỗng `{}`. Bỏ toàn bộ từ điển Tu Tiên / tính toán Cảnh Giới ở Sprint 2 `[A]`.

### Sprint 3: Hạ Tầng Kỹ Thuật & Spike Layering (Wave 3)
- **P2.2:** Triển khai giải phóng VRAM GPU: Bổ sung `DestroyTexture()` vào [Image.cs](../Assets/Scripts/Image.cs) và cơ chế thu hồi an toàn trong [Small.cs](../Assets/Scripts/Assets.src.e/Small.cs) và [SmallImage.cs](../Assets/Scripts/SmallImage.cs) (ngưỡng 200 lowGraphic / 800 normal), triệt tiêu rò rỉ GPU Texture `[A]`.
- **D1.1:** Hoàn thành báo cáo thực nghiệm [SPIKE_LAYERING.md](ui/SPIKE_LAYERING.md) (Gate G-C): Phân định rõ L0 (Pre-Game Canvas không bị che khuất) và L1 (In-Game HUD cần P4.3 backend) `[A]`.
- **D1.3:** Hiện thực bộ khung điều hướng UI: [UiRoot.cs](../Assets/Scripts/UI/Core/UiRoot.cs) (CanvasScaler 1280x720, EventSystem), [UiRouter.cs](../Assets/Scripts/UI/Core/UiRouter.cs) (Navigation Stack), [ModalManager.cs](../Assets/Scripts/UI/Core/ModalManager.cs) (Dim Backdrop), [ToastService.cs](../Assets/Scripts/UI/Core/ToastService.cs) (Thông báo nổi) `[A]`.
- **D1.7:** Hiện thực [DialogBridge.cs](../Assets/Scripts/UIBridge/DialogBridge.cs) và gắn móc an toàn trong `GameCanvas.startOKDlg/startYesNoDlg` `[A]`.
- **Unit Tests:** Bổ sung [UiFrameworkTests.cs](../Assets/Scripts/UI/Tests/UiFrameworkTests.cs) `[A]`.

### Sprint 4: Lát Cắt S1 (Cổng Vào) & S2 (Chọn/Tạo Nhân Vật) (Wave 4)
- **D2.1:** Khởi tạo [ThemeTokens.cs](../Assets/Scripts/UI/Core/ThemeTokens.cs) cung cấp bảng màu Tu Tiên chuẩn mực (Thanh Bích, Hoàng Kim, Tử Khí, Bạch Vân, Sương Mù) `[A]`.
- **D1.4:** Thiết lập cầu nối ngắt vẽ & input IMGUI cũ tại [Main.cs](../Assets/Scripts/Main.cs) khi các màn hình L0 mới kích hoạt `[A]`.
- **Lát cắt S1 (Cổng Vào):**
  - [GatewayViewModel.cs](../Assets/Scripts/UI/Core/ViewModels/GatewayViewModel.cs): Quản lý tài khoản, mật khẩu, danh sách máy chủ `[A]`.
  - [GatewayView.cs](../Assets/Scripts/UI/Core/Views/GatewayView.cs): Giao diện uGUI Greybox Cổng Vào tự sinh component động không cần prefab `[A]`.
  - [GatewayBridge.cs](../Assets/Scripts/UIBridge/GatewayBridge.cs): Kết nối với `Session_ME` và `ServerListScreen` `[A]`.
- **Lát cắt S2 (Chọn & Tạo Nhân Vật):**
  - [CharCreationViewModel.cs](../Assets/Scripts/UI/Core/ViewModels/CharCreationViewModel.cs): Quản lý 3 Môn Phái (Kiếm Tông, Linh Tộc, Ma Tộc), kiểu tóc, validation đạo hiệu `[A]`.
  - [CharCreationView.cs](../Assets/Scripts/UI/Core/Views/CharCreationView.cs): Giao diện uGUI Greybox tạo nhân vật mới `[A]`.
  - [CharacterBridge.cs](../Assets/Scripts/UIBridge/CharacterBridge.cs): Kết nối với `CreateCharScr` và `Service.gI().createChar` `[A]`.
- **Unit Tests:** Bổ sung [SliceViewModelsTests.cs](../Assets/Scripts/UI/Tests/SliceViewModelsTests.cs) kiểm thử toàn diện logic đăng nhập và tạo nhân vật `[A]`.

### Sprint 5: Lát Cắt S3 (HUD Lõi Tu Tiên In-Game) (Wave 5)
- **P4.3 Prep (L1 Layering):** Phân tách lớp vẽ thế giới game và HUD trong [GameScr.cs](../Assets/Scripts/GameScr.cs), tích hợp điểm ngắt an toàn `HudBridge.ShouldSuppressLegacyHud()` tại `paintInfoBar`, `paintSelectedSkill`, `paintGamePad` `[A]`.
- **Lát cắt S3 (HUD Lõi Tu Tiên):**
  - [HudViewModel.cs](../Assets/Scripts/UI/Core/ViewModels/HudViewModel.cs): Quản lý Sinh Mệnh (HP/MP/Tu vi), Kính Chiếu Yêu (Target Mob/Player), Ngũ Hành Thần Thông (5 kỹ năng + cooldown) và Đan Dược/Đậu Thần `[A]`.
  - [HudView.cs](../Assets/Scripts/UI/Core/Views/HudView.cs): Giao diện uGUI Greybox HUD Tu Tiên hoàn chỉnh (Avatar Bát Quái, thanh Huyết Khí/Linh Lực, Kính Chiếu Yêu, cụm 5 nút kỹ năng rẻ quạt công thái học, phím tắt 1-5 / Space / Tab / B) `[A]`.
  - [HudBridge.cs](../Assets/Scripts/UIBridge/HudBridge.cs): Cầu nối hai chiều giữa `GameScr` và `HudViewModel`, tự động cập nhật dữ liệu và kết nối sự kiện xuất chiêu, dùng đậu thần, đổi mục tiêu, mở hành trang `[A]`.
- **Unit Tests:** Bổ sung [HudViewModelTests.cs](../Assets/Scripts/UI/Tests/HudViewModelTests.cs) kiểm thử toàn diện logic tính toán phần trăm, cảnh giới tu vi và sự kiện HUD `[A]`.

---

## 4. Bảng Hiện Trạng Các Cổng Kiểm Soát (Gate Status)

| Mã Task | Tên / Phạm Vi | Trạng Thái Hiện Tại | Yêu Cầu Để Đạt Cổng (Gate) |
|:---:|:---|:---:|:---|
| **P1.1, P1.2** | `NRO.Net` + Golden Tests | Code hoàn thành `[A]` | Người dùng chạy Test Runner EditMode xác nhận Xanh lá `[H]`. |
| **P1.3, P1.4** | `GameConnection` & Facade `Session_ME` | Code hoàn thành `[A]` | **CHƯA TÍNH LÀ XONG** — Cần người dùng playtest **P1-PT** không lỗi mạng (Gate G-B) `[H]`. |
| **P0.3** | Đo đạc Baseline | Code hoàn thành `[A]` | Người dùng đo 4 kịch bản trên tag `baseline-before-refactor` để lập mốc so sánh (Gate G-A) `[H]`. |
| **Gate G-C** | Spike Layering (D1.1) | **ĐÃ THÔNG QUA `[A]`** | Đã kết luận tại [SPIKE_LAYERING.md](ui/SPIKE_LAYERING.md): S1-S2 đi trước trên L0, S3 chờ L1 (P4.3). |
| **Parity S1 & S2** | Gateway & Character Screens | Code hoàn thành `[A]` | Người dùng bật cờ `UiFlags.EnableNewLogin` / `EnableNewCharSelect` thử nghiệm `[H]`. |

---

## 5. Hai Phiếu Kiểm Thử Độc Lập Cho Người Dùng (Theo G13)

### Phiếu 1: Unity Test Card — Track N (P2.4/P2.7 Performance & Memory Test)

```text
## Unity Test Card — Track N (P2.4/P2.7 Performance & Memory)
Nhánh: docs/analysis   Unity: 6000.0.57f1
Loại: [Track N]

1. Mở project Unity, chờ nạp mã nguồn.
   Kỳ vọng: 0 compile errors trên Console.
2. Kiểm tra chỉ số GC Alloc trên Game:
   - Bấm Play scene game.
   - Bấm `~` hoặc F12 để mở PerfProbe OSD.
   - Di chuyển trong làng và đánh chiêu thức.
3. Kết quả mong đợi:
   - Chỉ số GC Alloc/Frame duy trì ổn định, không bị gai đột biến (Lag Spike).
   - Game chạy mượt mà, FPS duy trì ở mức tối đa màn hình (60 FPS).
```

---

### Phiếu 2: Unity Test Card — Track U (S1 Cổng Vào & S2 Tạo Nhân Vật Greybox)

```text
## Unity Test Card — S1 & S2 (Gateway & Character Creation Screens)
Nhánh: docs/analysis   Unity: 6000.0.57f1
Loại: [Track U]

1. Mở project Unity trên nhánh docs/analysis, chờ compile hoàn tất.
   Kỳ vọng: 0 compile errors.
2. Kiểm thử an toàn khi cờ TẮT (Default Regression Test):
   - Bấm Play scene game.
   - Màn hình đăng nhập và chọn nhân vật cũ hiển thị 100% bình thường như trước.
3. Kiểm thử giao diện mới khi BẬT cờ (New UI Test):
   - Mở file Assets/Scripts/UI/Core/UiFlags.cs.
   - Đặt tạm: EnableNewLogin = true; EnableNewCharSelect = true;
   - Bấm Play scene game.
   Kỳ vọng:
   - Màn hình Cổng Vào uGUI mới xuất hiện với bố cục Greybox chuẩn mực (Ô nhập tài khoản, mật khẩu, nút chọn máy chủ).
   - Thử chọn máy chủ > Đăng nhập > Màn hình Chọn / Tạo nhân vật uGUI mới hiển thị mượt mà.
   - Bấm phím Escape hoặc nút Back điều hướng trơn tru qua UiRouter.
4. Chạy Test Runner:
   - Window > General > Test Runner > Run All Tests.
   Kỳ vọng: Toàn bộ Unit Test NRO.UI.Tests và NRO.Net.Tests đều XANH LÁ (Passed).
```
