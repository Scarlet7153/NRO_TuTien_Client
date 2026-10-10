# DỰ THẢO GÓI TIN & MỞ RỘNG MÁY CHỦ (SERVER ADDITIONS PROPOSAL - DRAFT)

> **Mã định danh:** `D0.3` / Sprint 1 Task T6.  
> **Cập nhật:** 10/10/2026.  
> **Mục tiêu:** Phân định rõ ràng giữa dữ liệu Client tự suy diễn (Client Inferred) và dữ liệu cần Server bổ sung (Server Additions) cho theme Tu Tiên.  
> **Độ tin cậy:** `[A]` (Dựa trên cấu trúc mạng hiện hữu của `NRO_Tu_Tien` Server).

---

## 1. Nguyên Tắc Thiết Kế Cốt Lõi

> [!IMPORTANT]
> **TIÊU CHÍ ƯU TIÊN SỐ 1: KHÔNG SỬA SERVER NẾU CLIENT CÓ THỂ TỰ SUY DIỄN.**  
> Mục tiêu đại tu giao diện Tu Tiên (Track U) là khoác lên một diện mạo Tiên Hiệp đỉnh cao mà không làm phá vỡ logic máy chủ hiện hành. Mọi chỉ số của NRO gốc đều có thể ánh xạ mỹ học sang khái niệm Tu Chân Giới.

---

## 2. Bảng Phân Định Dữ Liệu Giao Diện Tu Tiên (Data Gap Analysis)

| Khái Niệm Tu Tiên | Tương Ứng Trong NRO Gốc | Phương Án Triển Khai | Nguồn Dữ Liệu & Công Thức Suy Diễn |
|:---|:---|:---:|:---|
| **Cảnh Giới Tu Vi** (Luyện Khí, Trúc Cơ, Kim Đan, Nguyên Anh...) | Điểm Sức Mạnh (`Char.cPower`) | **Client Tự Suy Diễn `[A]`** | Ánh xạ theo bảng mốc `cPower` (Ví dụ: <100k: Phàm Nhân, 1M-10M: Trúc Cơ, 100M-1B: Nguyên Anh, >10B: Độ Kiếp). |
| **Linh Căn / Thuộc Tính Môn Phái** (Kiếm Tông, Ma Tộc, Thần Tộc) | Hành tinh / Giới tính (`Char.cgender`) | **Client Tự Suy Diễn `[A]`** | - `gender = 0` (Trái Đất) $\rightarrow$ Thái Hư Kiếm Tông (Phong/Lôi).<br>- `gender = 1` (Namếc) $\rightarrow$ Trường Sinh Linh Tộc (Mộc/Thủy - Y Đạo).<br>- `gender = 2` (Xayda) $\rightarrow$ Cửu U Ma Tộc (Hỏa/Thể Tu). |
| **Phẩm Cấp Pháp Bảo** (Hoàng, Huyền, Địa, Thiên, Thần Cấp) | Cấp sao pha lê / Cấp trang bị (`Item.level`, `ItemOption`) | **Client Tự Suy Diễn `[A]`** | - 0-1 sao: Phàm Khí / Hoàng Cấp.<br>- 2-3 sao: Huyền Cấp.<br>- 4-5 sao: Địa Cấp.<br>- 6-7 sao: Thiên Cấp.<br>- Đồ Kích Hoạt / Thần Linh: Tiên Khí / Thần Cấp. |
| **Linh Đan Dược Tễ** | Đậu Thần (`magicTree.currPeas`, `hpPotion`) | **Client Tự Suy Diễn `[A]`** | Đậu 1-10 tương ứng Cửu Chuyển Linh Đan (Cấp 1 đến Cấp 10). |
| **Linh Thú / Đạo Đồng** | Đệ tử / Thú cưỡi (`TYPE_PET_MAIN`) | **Client Tự Suy Diễn `[A]`** | Trạng thái bảo vệ/đi theo của Đệ tử tương ứng Linh Sủng hộ đạo. |
| **Bế Quan Tham Thiền** | Auto train (`TYPE_AUTO`) | **Client Tự Suy Diễn `[A]`** | Giao diện ngồi xếp bằng nhập định, tỏa linh quang. |
| **Tông Môn Chức Vị** | Trưởng bang, Phó bang, Thành viên (`ClanMember.role`) | **Client Tự Suy Diễn `[A]`** | - Role 0: Tông Chủ (Chưởng Môn).<br>- Role 1: Đại Trưởng Lão.<br>- Role 2: Chấp Sự / Nội Môn Đệ Tử. |

---

## 3. Các Tính Năng Đề Xuất Bổ Sung Từ Server (Dành Cho Giai Đoạn Nâng Cấp Sâu)

Nếu trong tương lai đội ngũ muốn phát triển gameplay Tu Tiên chuyên sâu vượt ra ngoài phạm vi giao diện, các gói tin sau đây được dự thảo để mở rộng:

### 3.1. Gói Tin Nhận Thông Tin Tu Tiên Mở Rộng (`CMD_TUTIEN_INFO = -115`)
- **Chiều:** Server $\rightarrow$ Client
- **Mục đích:** Cung cấp thông tin danh hiệu tu chân động, cảnh giới đột phá có tỷ lệ thất bại (Thiên Kiếp).
- **Cấu trúc nhị phân:**
  ```text
  [byte cmd = -115]
  [short length]
  [int realmId]           // ID Cảnh giới cụ thể (0-15)
  [byte realmSubLevel]    // Tầng tiểu cảnh giới (1-9)
  [long expCultivation]   // Điểm tu vi linh khí tích lũy
  [long expRequired]      // Điểm cần để đột phá
  [byte successRate]      // Tỷ lệ độ kiếp thành công (%)
  ```

### 3.2. Gói Tin Yêu Cầu Đột Phá Cảnh Giới (`CMD_BREAKTHROUGH = -116`)
- **Chiều:** Client $\rightarrow$ Server
- **Mục đích:** Người chơi bấm nút "Độ Kiếp" trong giao diện Cảnh Giới.
- **Cấu trúc nhị phân:**
  ```text
  [byte cmd = -116]
  [short length = 2]
  [short pillItemId]      // ID đan dược phụ trợ hộ mạch (hoặc -1 nếu không dùng)
  ```

---

## 4. Kết Luận & Khuyến Nghị Cho Track U

1. **Sprint 1 - 4 (MVP Track U):**
   - **Tuyệt đối KHÔNG cần sửa đổi Server.**
   - Toàn bộ giao diện 6 Hub Tu Tiên và HUD Thái Cực sẽ vận hành hoàn hảo 100% dựa trên cơ chế **Client Tự Suy Diễn** từ dữ liệu NRO gốc.
2. **Kiểm tra tương thích ngược:**
   - Client vẫn kết nối và tương tác trơn tru với bất kỳ Server NRO tiêu chuẩn nào mà không gây lỗi phân tích gói tin (`MessageSendCollect`).
