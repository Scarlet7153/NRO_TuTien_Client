# VÒNG ĐỜI BỘ NHỚ TEXTURE & HÌNH ẢNH (TEXTURE LIFECYCLE & VRAM AUDIT)

> **Mã định danh:** `P2.2` / Sprint 1 Task T5.  
> **Cập nhật:** 10/10/2026.  
> **Nguồn đối chiếu:** `Assets/Scripts/SmallImage.cs`, `Image.cs`, `mGraphics.cs`, `mSystem.cs`, `Panel.cs`, `Controller.cs`.  
> **Độ tin cậy:** `[A]` (Kiểm tra trực tiếp mã nguồn C#).

---

## 1. Cơ Chế Quản Lý Bộ Nhớ Đồ Họa Hiện Tại

Trong kiến trúc Unity C#, `Texture2D` là một đối tượng hỗn hợp (Hybrid Object):
- Phần vỏ quản lý (Managed Wrapper) nằm trong bộ nhớ Heap của C# (.NET CLR).
- Dữ liệu điểm ảnh gốc (Pixel Buffer & GPU Texture) nằm trong Native C++ Memory của Unity Engine và VRAM card màn hình.

> [!WARNING]
> **Bộ thu gom rác (Garbage Collector - GC) của .NET chỉ giải phóng vỏ C#, KHÔNG TỰ ĐỘNG GIẢI PHÓNG VRAM NATIVE!**  
> Dữ liệu GPU chỉ được giải phóng khi gọi `UnityEngine.Object.Destroy(texture)` tường minh hoặc khi hệ thống kích hoạt `Resources.UnloadUnusedAssets()`.

---

## 2. Các Lỗ Hổng & Rò Rỉ Bộ Nhớ (Memory Leaks / Bloat) Đã Xác Minh

### 2.1. Lỗ hổng trong `SmallImage.cs:284-287`
```csharp
// SmallImage.cs:268-288
public static void update()
{
    ...
    if (num > 200 && GameCanvas.lowGraphic)
    {
        imgNew = new Small[maxSmall]; // <-- RỦI RO LỚN
    }
}
```
1. **Chế độ thường (`lowGraphic == false`):** Mảng `imgNew` không bao giờ được dọn dẹp. Mọi icon item, quái, nhân vật từng xuất hiện trên màn hình sẽ tích tụ vĩnh viễn trong bộ nhớ.
2. **Chế độ cấu hình thấp (`lowGraphic == true`):** Khi số lượng ảnh vượt 200, code đơn giản khởi tạo mảng mới `imgNew = new Small[maxSmall]`. Hơn 200 đối tượng `Texture2D` cũ bị bỏ rơi mà không hề gọi `UnityEngine.Object.Destroy()`, gây tràn VRAM trên các thiết bị di động cấu hình yếu.

### 2.2. Hoàn toàn vắng bóng hàm `Destroy()` trong lớp `Image`
Khảo sát toàn bộ dự án (`Assets/Scripts/`):
- **Không có bất kỳ cuộc gọi `UnityEngine.Object.Destroy(image.texture)`** nào trong toàn bộ `Image.cs` và `SmallImage.cs`.
- Game chỉ dựa vào 5 điểm gọi `Resources.UnloadUnusedAssets()` rải rác:
  - `Panel.cs:3731`
  - `mSystem.cs:282, 342`
  - `Controller.cs:4699, 4988`
- Việc gọi `Resources.UnloadUnusedAssets()` là một thao tác cực kỳ đắt đỏ (Blocking / Stop-The-World trên luồng chính Unity), gây ra hiện tượng sụt giảm khung hình đột ngột (Lag Spike / Giật hình).

### 2.3. Tạo mới Texture 1x1 tạm thời liên tục
- `Image.cs:11`: Mỗi khi `new Image()` được tạo, mặc định khởi tạo `public Texture2D texture = new Texture2D(1, 1);`.
- `mGraphics.cs:227`: `texture2D = new Texture2D(1, 1);` để vẽ các điểm ảnh đơn.
- Các texture tạm này tạo áp lực rác GC liên tục mỗi khung hình.

---

## 3. Kiến Trúc Bộ Đệm Hình Ảnh Đề Xuất (LRU Texture Pool Architecture)

Để giải quyết triệt để rò rỉ VRAM và hỗ trợ đồ họa độ nét cao (HD/4K) cho Theme Tu Tiên mới, hệ thống cần chuyển sang mô hình **LRU (Least Recently Used) Texture Cache**:

```text
[Yêu Cầu Vẽ Icon / Sprite (id)]
               |
        +------+------+
        | Cache Hit?  |
        +------+------+
       Yes /        \ No
          /          \
[Cập nhật Tick]   [Đọc từ File/RMS hoặc Server]
[Trả về Texture]       |
                  [Bộ đệm đầy (> Giới hạn MB)?]
                       |
               [Tìm Texture lâu nhất không dùng]
                       |
               [UnityEngine.Object.Destroy(oldTex)]
                       |
               [Nạp Texture mới vào Pool]
```

### 3.1. Các Thông Số Quy Chuẩn Cho LRU Cache
- **Kích thước bộ đệm tối đa (VRAM Budget):**
  - Mobile (RAM < 3GB): Tối đa `64 MB` hoặc `300 textures`.
  - PC / Mobile cấu hình cao: Tối đa `256 MB` hoặc `1,200 textures`.
- **Hủy Texture an toàn:** Bắt buộc gọi `UnityEngine.Object.Destroy(texture)` ngay khi một slot bị trục xuất (Eviction) khỏi cache.
- **Tái sử dụng Texture 1x1:** Khởi tạo duy nhất 1 static `Texture2D whitePixel = new Texture2D(1, 1)` dùng chung cho toàn bộ `mGraphics`, loại bỏ việc cấp phát `new Texture2D(1, 1)`.

---

## 4. Kế Hoạch Triển Khai (Dự Kiến Sprint 2 - P2.2)

1. **Bước 1:** Bổ sung phương thức hủy tường minh `public void Dispose()` / `public void Destroy()` trong `Image.cs`.
2. **Bước 2:** Cải tạo `SmallImage.cs` sử dụng cấu trúc `Dictionary<int, CacheEntry>` với danh sách liên kết kép (Double-linked list) cho giải thuật LRU.
3. **Bước 3:** Viết unit test đo đạc: đảm bảo số lượng Texture2D trong memory không vượt ngưỡng sau khi duyệt qua 1,000 icon liên tục.
