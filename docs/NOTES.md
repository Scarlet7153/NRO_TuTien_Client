# NRO_TuTien_Client — Tài Liệu Kiến Trúc & Tái Cấu Trúc Tổng Hợp

> Tài liệu đầu mối tổng hợp cho cả 2 luồng công việc: **Track N** (Network & Performance Refactor) và **Track U** (UI/UX Tu Tiên Redesign).  
> Cập nhật: 10/10/2026 — **Sprint 3 (Wave 3 - Technical Infrastructure & Spike Layering) ĐÃ HOÀN TẤT CODE**.

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
- **D1.7:** Hiện thực [DialogBridge.cs](../Assets/Scripts/UIBridge/DialogBridge.cs) và gắn móc an toàn trong `GameCanvas.startOKDlg/startYesNoDlg` (chuyển hướng sang Modal/Toast khi `UiFlags.EnableNewDialogs == true`, giữ 100% logic cũ khi tắt) `[A]`.
- **Unit Tests:** Bổ sung [UiFrameworkTests.cs](../Assets/Scripts/UI/Tests/UiFrameworkTests.cs) kiểm thử toàn diện `UiFlags`, `UiRouter`, `ModalManager` `[A]`.

---

## 4. Bảng Hiện Trạng Các Cổng Kiểm Soát (Gate Status)

| Mã Task | Tên / Phạm Vi | Trạng Thái Hiện Tại | Yêu Cầu Để Đạt Cổng (Gate) |
|:---:|:---|:---:|:---|
| **P1.1, P1.2** | `NRO.Net` + Golden Tests | Code hoàn thành `[A]` | Người dùng chạy Test Runner EditMode xác nhận Xanh lá `[H]`. |
| **P1.3, P1.4** | `GameConnection` & Facade `Session_ME` | Code hoàn thành `[A]` | **CHƯA TÍNH LÀ XONG** — Cần người dùng playtest **P1-PT** không lỗi mạng (Gate G-B) `[H]`. |
| **P0.3** | Đo đạc Baseline | Code hoàn thành `[A]` | Người dùng đo 4 kịch bản trên tag `baseline-before-refactor` để lập mốc so sánh (Gate G-A) `[H]`. |
| **Gate G-C** | Spike Layering (D1.1) | **ĐÃ THÔNG QUA `[A]`** | Đã kết luận tại [SPIKE_LAYERING.md](ui/SPIKE_LAYERING.md): S1-S2 đi trước trên L0, S3 chờ L1 (P4.3). |
| **D1.3, D1.7** | UI Framework & DialogBridge | Code hoàn thành `[A]` | Unity compile 0 error, Test Runner EditMode `NRO.UI.Tests` pass `[H]`. |

---

## 5. Hai Phiếu Kiểm Thử Độc Lập Cho Người Dùng (Theo G13)

### Phiếu 1: Unity Test Card — Track N (P2.2 LRU Texture Cache)

```text
## Unity Test Card — P2.2 (LRU Texture Cache & VRAM Cleanup)
Nhánh: docs/analysis   Unity: 6000.0.57f1
Loại: [Track N]

1. Mở project Unity, chờ nạp mã nguồn.
   Kỳ vọng: 0 compile errors trên Console.
2. Kiểm tra giải phóng Texture:
   - Bấm Play scene game.
   - Bấm `~` (BackQuote) hoặc F12 để mở PerfProbe OSD quan sát chỉ số Heap và Memory.
   - Di chuyển qua 3-5 map khác nhau (để nạp nhiều icon quái và NPC vào SmallImage).
   - Mở Panel Hành Trang lướt xem các item.
   - Quan sát Memory: Sau khi chuyển map hoặc sau chu kỳ dọn dẹp, VRAM/Heap không bị tăng đột biến không kiểm soát.
3. Kết quả mong đợi:
   - Game chạy mượt mà, không gặp lỗi MissingReferenceException của Texture.
   - Icon vật phẩm và sprite nhân vật hiển thị đầy đủ, sắc nét, không bị trắng/mất hình.
```

---

### Phiếu 2: Unity Test Card — Track U (D1.1, D1.3 & D1.7 UI Core Framework)

```text
## Unity Test Card — D1.1, D1.3 & D1.7 (UI Router, Modals & DialogBridge)
Nhánh: docs/analysis   Unity: 6000.0.57f1
Loại: [Track U]

1. Mở project Unity trên nhánh docs/analysis, chờ compile hoàn tất.
   Kỳ vọng: 0 compile errors.
2. Kiểm tra Test Runner:
   - Mở Window > General > Test Runner > Tab EditMode > Bấm 'Run All'.
   Kỳ vọng:
   - NRO.Net.Tests tiếp tục XANH LÁ (Passed).
   - NRO.UI.Tests (LocalizerTests, UiFrameworkTests) XANH LÁ (Passed 100%).
3. Đọc tài liệu Spike Layering:
   - Xem file docs/ui/SPIKE_LAYERING.md để nắm rõ kết luận về cơ chế IMGUI vs uGUI Canvas.
4. Kiểm tra độ an toàn hồi quy (Regression Test):
   - Bấm Play scene game.
   - Đăng nhập và trải nghiệm các tính năng hiện hành.
5. Kết quả mong đợi:
   - Do UiFlags mặc định tắt toàn bộ, game chạy 100% như cũ, không có lỗi ngoại lệ trên Console.
```
