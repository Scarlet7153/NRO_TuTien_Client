# NRO_TuTien_Client — Tài Liệu Kiến Trúc & Tái Cấu Trúc Tổng Hợp

> Tài liệu đầu mối tổng hợp cho cả 2 luồng công việc: **Track N** (Network & Performance Refactor) và **Track U** (UI/UX Tu Tiên Redesign).  
> Cập nhật: 10/10/2026 — **Sprint 2 (Wave 2 - Technical Foundations) ĐÃ HOÀN TẤT CODE**.

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

---

## 4. Bảng Hiện Trạng Các Cổng Kiểm Soát (Gate Status)

| Mã Task | Tên / Phạm Vi | Trạng Thái Hiện Tại | Yêu Cầu Để Đạt Cổng (Gate) |
|:---:|:---|:---:|:---|
| **P1.1, P1.2** | `NRO.Net` + Golden Tests | Code hoàn thành `[A]` | Người dùng chạy Test Runner EditMode xác nhận Xanh lá `[H]`. |
| **P1.3, P1.4** | `GameConnection` & Facade `Session_ME` | Code hoàn thành `[A]` | **CHƯA TÍNH LÀ XONG** — Cần người dùng playtest **P1-PT** không lỗi mạng (Gate G-B) `[H]`. |
| **P0.3** | Đo đạc Baseline | Code hoàn thành `[A]` | Người dùng đo 4 kịch bản trên tag `baseline-before-refactor` để lập mốc so sánh (Gate G-A) `[H]`. |
| **D1.2, D1.6** | UI Asmdef & ILocalizer | Code hoàn thành `[A]` | Unity compile 0 error, Test Runner EditMode `NRO.UI.Tests` pass `[H]`. |

---

## 5. Hai Phiếu Kiểm Thử Độc Lập Cho Người Dùng (Theo G13)

### Phiếu 1: Unity Test Card — Track N (P0.3 PerfProbe Baseline)

```text
## Unity Test Card — P0.3 (PerfProbe Baseline Measurement)
Nhánh: docs/analysis (hoặc checkout baseline-before-refactor)   Unity: 6000.0.57f1
Loại: [Track N]

1. Mở project Unity, chờ nạp mã nguồn.
   Kỳ vọng: 0 compile errors trên Console.
2. Đo Baseline trên tag 'baseline-before-refactor':
   - git checkout baseline-before-refactor
   - Đưa file PerfProbe.cs vào Assets/Scripts/Util/PerfProbe.cs
   - Bấm Play scene game.
   - Bấm phím `~` (BackQuote) hoặc F12 để hiển thị OSD PerfProbe góc trên phải.
3. Chạy 4 kịch bản chuẩn trong 60 giây mỗi kịch bản:
   a) Màn hình đăng nhập / Đứng yên tại Làng Aru.
   b) Di chuyển liên tục qua 3 map.
   c) Đánh quái / dùng chiêu thức liên tục.
   d) Mở / đóng Panel hành trang 20 lần.
4. Kết quả mong đợi:
   - Ghi lại các chỉ số: FPS trung bình, 1% Low, FrameTime (ms), GC Alloc/Frame, Draw Calls.
   - Lưu kết quả làm mốc so sánh cho Gate G-A.
```

---

### Phiếu 2: Unity Test Card — Track U (D1.2 asmdef & D1.6 ILocalizer)

```text
## Unity Test Card — D1.2 & D1.6 (UI Architecture & Localizer)
Nhánh: docs/analysis   Unity: 6000.0.57f1
Loại: [Track U]

1. Mở project Unity trên nhánh docs/analysis, chờ compile hoàn tất.
   Kỳ vọng: 0 compile errors.
   Unity tự động nhận diện 3 asmdef: NRO.UI.Contracts, NRO.UI, NRO.UI.Tests.
2. Kiểm tra Test Runner:
   - Menu: Window > General > Test Runner (hoặc Window > Analysis > Test Runner).
   - Chọn tab: EditMode.
   - Bấm nút: 'Run All'.
   Kỳ vọng:
   - NRO.Net.Tests tiếp tục XANH LÁ (Passed).
   - NRO.UI.Tests (LocalizerTests) XANH LÁ (Passed 100%).
3. Thao tác thử Game:
   - Bấm Play scene game.
   - Đăng nhập và vào game bình thường.
4. Kết quả mong đợi:
   - Game chạy nguyên vẹn như cũ, 0 ngoại lệ trên Console.
   - UiFlags mặc định tắt toàn bộ nên không ảnh hưởng đến giao diện hiện hành.
```
