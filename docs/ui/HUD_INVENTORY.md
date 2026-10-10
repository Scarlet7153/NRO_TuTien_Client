# KIỂM KÊ GIAO DIỆN CHIẾN ĐẤU & ĐIỀU KHIỂN (HUD INVENTORY)

> **Mã định danh:** `D0.1` / Sprint 1 Task T4.  
> **Cập nhật:** 10/10/2026.  
> **Nguồn đối chiếu:**  
> - `Assets/Scripts/GameScr.cs` (7,574 dòng, 196 KB)  
> - `Assets/Scripts/ChatTextField.cs`, `Command.cs`, `Skill.cs`  
> **Độ tin cậy:** `[A]` (Kiểm tra trực tiếp mã nguồn C#).

---

## 1. Tổng Quan Kiến Trúc HUD GameScr

Giao diện HUD (Heads-Up Display) hiển thị trực tiếp đè lên khung cảnh game trong quá trình thám hiểm và chiến đấu. Toàn bộ logic vẽ tập trung trong hàm `GameScr.paint(mGraphics g)` (`GameScr.cs:4961-5460`).

HUD phân tầng thành 4 nhóm chính:
1. **Khung Chỉ Số Sinh Mệnh (Vitality Bar):** Góc trên trái (HP, KI/MP, Thể lực, Thông tin mục tiêu).
2. **Khung Kỹ Năng & Vật Phẩm Nhanh (Action Bar):** Góc dưới phải (Ô Đậu Thần hồi phục, 5 ô Kỹ Năng chủ động).
3. **Điều Khiển Di Chuyển (Movement Control):** Góc dưới trái (Cần Analog / Joystick ảo trên mobile).
4. **Chỉ Báo Môi Trường & Hệ Thống (World & Status Overlays):** Mũi tên chỉ đường, Thanh phụ bản, Chat VIP, Chatbox.

---

## 2. Bảng Danh Mục Kiểm Kê Các Thành Phần HUD

| Tên Thành Phần | Hàm Vẽ Chính (`GameScr.cs`) | Vị Trí Màn Hình | Nguồn Dữ Liệu | Cơ Chế Tương Tác | Đề Xuất Thay Thế Tu Tiên | Độ Khó |
|:---|:---|:---|:---|:---|:---|:---:|
| **Thanh Sinh Lực (HP Bar)** | `paintImageBar`<br>(`line 5844`) | Góc trên bên trái (`X=0..139`, `Y=5..15`) | `Char.myCharz().cHP`<br>`cHPFull`<br>`dHP` (hp mượt) | Tự động cập nhật theo sát thương | **Thanh Huyết Khí (Chu Tước Xích Huyết):** Ống đồng khắc họa tiết vân mây | Trung bình |
| **Thanh Pháp Lực (KI/MP Bar)** | `paintImageBar`<br>(`line 5844`) | Dưới thanh HP (`X=139`, `Y=20..26`) | `Char.myCharz().cMP`<br>`cMPFull`<br>`dMP` | Tiêu hao khi xuất chiêu | **Thanh Linh Lực (Thanh Long Chân Khí):** Màu lam ngọc tỏa sương mù | Trung bình |
| **Thanh Thể Lực Thứ 2 (SP Bar)** | `paintImageBar`<br>(`line 5866`) | Dưới thanh MP (`X=139`, `Y=31..41`) | `Char.myCharz().powerPoint`<br>`maxPowerPoint` | Tích lũy nộ khí | **Thanh Đạo Hạnh / Nộ Khí (Huyền Vũ Chấn Khí)** | Thấp |
| **Avatar & Đạo Hiệu** | `paintImageBar`<br>(`line 5879`) | Góc trên bên trái (`0, 0`) | `paintHeadWithXY`<br>`Char.myCharz().cName` | Bấm để mở menu nhân vật | **Khung Thái Cực Âm Dương:** Chân dung tu sĩ viền linh quang | Thấp |
| **Mục Tiêu Chỉ Định (Target Focus)** | `paintInfoBar`<br>(`line 5993-6034`) | Giữa mép trên màn hình (`imgScrW/2`) | `mobFocus` (quái)<br>`npcFocus` (NPC)<br>`charFocus` (người) | Bấm chọn mục tiêu trên map / phím Tab | **Kính Chiếu Yêu / La Bàn Nhận Diện:** Thẻ bài hiển thị tu vi đối phương | Trung bình |
| **Ô Hồi Máu Nhanh (Đậu Thần / Đan Dược)** | `paintSelectedSkill`<br>(`line 6088-6141`) | Kế bên thanh kỹ năng (`xHP, yHP`) | `hpPotion`<br>`lastUsePotion`<br>(cooldown 10s) | Bấm phím `*` hoặc chạm màn hình | **Hồ Lô Cửu Chuyển Linh Đan:** Hiệu ứng hào quang khi hồi phục | Thấp |
| **5 Ô Kỹ Năng Chiến Đấu (Skill Bar)** | `paintSelectedSkill`<br>(`line 6155-6194`) | Góc dưới phải (Mobile) hoặc đáy giữa (PC) | `keySkill[]`<br>`onScreenSkill[]`<br>`Char.myCharz().myskill` | Phím PC: 1-5 hoặc Q-T. Mobile: chạm cảm ứng | **Ngũ Hành Pháp Trận (Kim-Mộc-Thủy-Hỏa-Thổ):** Khung ngũ hành cổ kính | Cao |
| **Cần Điều Khiển Ảo (Joystick / Analog)** | `paintGamePad`<br>(`line 7146`) | Góc dưới bên trái | `GamePad`<br>`isAnalog` | Kéo thả cảm ứng điều hướng | **Bát Quái Du Thân Bộ:** Vòng tròn thái cực xoay nhẹ khi di chuyển | Trung bình |
| **Mũi Tên Chỉ Hướng NPC & Chuyển Map** | `paintWaypointArrow`<br>`paintArrowPointToNPC`<br>(`line 5551, 5665`) | Mép ngoài màn hình | Tọa độ `Waypoint`, `Npc` so với `cmx, cmy` | Chỉ báo thị giác dẫn đường | **Phi Kiếm Dẫn Lộ:** Kiếm khí bay lượn chỉ hướng động phủ | Thấp |
| **Thanh Trạng Thái Phụ Bản / Boss** | `paintPhuBanBar`<br>`paintChienTruong_Life` | Giữa trên màn hình | Máu Boss / Thời gian phụ bản / Điểm phe | Hiển thị tiến độ phụ bản | **Thiên Kiếp Đồng Hồ / Ma Thần Giáng Thế** | Thấp |
| **Bảng Tin Chat VIP** | `paintChatVip`<br>(`line 7213`) | Chạy ngang phía trên | Tin tức broadcast máy chủ | Tự cuộn chữ | **Thiên Đạo Chiếu Thư:** Cuộn thư pháp bay ngang đỉnh trời | Thấp |
| **Hộp Thoại Chat Nhanh (ChatTextField)** | `ChatTextField.paint`<br>(`line 5419`) | Đáy màn hình | `ChatTextField.gI().tfChat` | Gõ văn bản / Emoji | **Truyền Âm Phù:** Ô ngọc giản thanh thoát, gọn gàng | Trung bình |

---

## 3. Danh Mục Các Nút Lệnh Tắt (Command Objects Trong `GameScr.cs`)

| Biến Command | Mã Hành Động (Action ID) | Tọa Độ Mặc Định | Chức Năng |
|:---|:---:|:---|:---|
| `cmdMenu` | `11000` (`line 947`) | `(0, 50)` hoặc `(0, gH - 30)` | Mở bảng Menu chức năng chính của game |
| `cmdFocus` | `11001` (`line 948`) | `(gW - 32, gH - 32)` | Đổi mục tiêu mục tiêu lân cận gần nhất |
| `cmdBag` | `1100011` (`line 1791`) | Thanh menu dưới | Mở nhanh Túi đồ (Hành trang) |
| `cmdSkill` | `1100012` (`line 1792`) | Thanh menu dưới | Mở nhanh Bảng kỹ năng nhân vật |
| `cmdTiemnang` | `1100013` (`line 1793`) | Thanh menu dưới | Mở nhanh Bảng cộng điểm tiềm năng |
| `cmdInfo` | `1100014` (`line 1794`) | Thanh menu dưới | Mở nhanh Thông tin nhân vật |
| `cmdDead` | `11038` (`line 2615`) | Trung tâm màn hình khi tử vong | Về nơi hồi sinh / Dùng ngọc hồi sinh tại chỗ |

---

## 4. Thiết Kế Đột Phá Cho HUD Tu Tiên (Track U UI Concept)

### 4.1. Cụm Chỉ Số Sinh Mệnh (Thái Cực Đạo Trận)
Thay vì các thanh chữ nhật thô sơ của Dragon Ball nguyên bản, cụm HP/MP sẽ được tái thiết kế thành **Thái Cực Linh Lung**:
- **Khí Hải (HP):** Vòng cung huyết quang bên trái, uốn lượn mềm mại.
- **Linh Căn (MP):** Vòng cung thanh quang đối xứng bên phải.
- **Tâm Trận (Avatar):** Trung tâm là vòng Âm Dương Bát Quái xoay chậm khi nhân vật đứng yên, phát sáng linh khí khi vào trạng thái chiến đấu (`isAttack`).

### 4.2. Cụm Kỹ Năng (Ngũ Hành Thần Thông)
- 5 ô kỹ năng chủ động xếp theo hình vòng cung chuẩn công thái học ngón tay cái bên phải (Mobile) hoặc thanh ngang thư pháp tinh tế (PC).
- Viền ô kỹ năng thể hiện thuộc tính tương sinh ngũ hành (Kim: Ánh Kim, Mộc: Xanh Lục, Thủy: Lam Bích, Hỏa: Viêm Đỏ, Thổ: Hoàng Nâu).
- Cooldown hiển thị dưới dạng sương khói bay dần từ đáy lên đỉnh ô.

---

## 5. Đánh Giá Độ Khó & Rủi Ro Khi Refactor HUD

1. **Rủi ro tính toán toạ độ cảm ứng (Touch Hitbox):**
   - Các biến tọa độ `xHP`, `yHP`, `xSkill`, `xS[]`, `yS[]` được tính toán phụ thuộc vào kích thước màn hình `GameCanvas.w, GameCanvas.h`. Khi thay thế sprite hoặc bố cục, phải giữ nguyên vùng hitbox nhận diện chạm hoặc bao bọc trong một component Controller riêng.
2. **Hiệu năng vẽ trên thiết bị yếu:**
   - HUD vẽ liên tục 60 lần/giây trong `paint()`. Không được cấp phát `new` đối tượng bộ nhớ (GC alloc) trong các hàm vẽ HUD này. Tất cả sprite viền, font chữ, hiệu ứng phải được cache tĩnh từ trước.
