# KIỂM KÊ TOÀN DIỆN PANEL & MÀN HÌNH GIAO DIỆN (UI INVENTORY)

> **Mã định danh:** `D0.1` / Sprint 1 Task T3.  
> **Cập nhật:** 10/10/2026.  
> **Nguồn đối chiếu:**  
> - `Assets/Scripts/Panel.cs` (11,703 dòng, 315 KB)  
> - `Assets/Scripts/NewPanel.cs` (23 dòng stub)  
> - Các màn hình: `GameScr.cs`, `LoginScr.cs`, `CreateCharScr.cs`, `ServerListScreen.cs`, `Menu.cs`, `InputDlg.cs`, `InfoDlg.cs`, `MsgDlg.cs`, `CrackBallScr.cs`, `RadarScr.cs`  
> **Độ tin cậy:** `[A]` (Kiểm tra trực tiếp mã nguồn C#).

---

## 1. Tổng Quan Kiến Trúc Giao Diện Hiện Tại

Hiện tại, toàn bộ giao diện bảng (dialogs, tabs, menus, shop, inventory, v.v.) của client NRO được nhồi nhét tập trung trong file khổng lồ `Panel.cs` với 26 loại bảng (`TYPE_` 0 đến 25).
- Cơ chế vẽ: Sử dụng `mGraphics` trên toạ độ tuyệt đối, clip view thủ công, quản lý thanh cuộn bằng `cmy`, `cmtoY`, `cmyLim`.
- Phân luồng dữ liệu: Đan xen chặt chẽ giữa đọc dữ liệu từ `Char.myCharz()` và gửi gói tin qua `Service.gI()`.
- Lớp `NewPanel.cs`: Là class khung rỗng (stub 23 dòng), chưa có logic thực thi.

---

## 2. Bảng Danh Mục Kiểm Kê 26 Panel Types Trong `Panel.cs`

| Type ID | Tên Hằng Số | Mục Đích & Chức Năng | Hàm Vẽ Chính | Nguồn Dữ Liệu | Lệnh Mạng Gửi Đi | Nhóm Tu Tiên Đề Xuất (Hub) | Độ Khó Refactor |
|:---:|:---|:---|:---|:---|:---|:---|:---:|
| **0** | `TYPE_MAIN`<br>(`Panel.cs:433`) | Bảng thông tin chính của nhân vật: Hành trang túi, Kỹ năng, Nhiệm vụ, Bang, Chỉ số | `paintTask`<br>`paintInventory`<br>`paintSkill`<br>`paintClans` | `Char.myCharz().arrItemBag`<br>`Char.myCharz().vSkillFight`<br>`Char.myCharz().taskMaint` | `Service.gI().useItem`<br>`Service.gI().upSkill`<br>`Service.gI().getTask` | **Hub 1: Nhân Vật** & **Hub 2: Tu Luyện** | Cao |
| **1** | `TYPE_SHOP`<br>(`Panel.cs:435`) | Cửa hàng NPC bán đồ, kỳ ngọc, vật phẩm thường và cao cấp | `paintShop`<br>(`Panel.cs:4168`) | `ItemOption[]`<br>`GameCanvas.panel.shopItem` | `Service.gI().buyItem`<br>`Service.gI().getItem` | **Hub 5: Phường Thị** | Trung bình |
| **2** | `TYPE_BOX`<br>(`Panel.cs:437`) | Rương chứa đồ cá nhân (tại nhà hoặc qua dịch vụ) | `paintBox`<br>`paintInventory` | `Char.myCharz().arrItemBox`<br>`arrItemBag` | `Service.gI().getItem(0, id)`<br>`Service.gI().getItem(1, id)` | **Hub 1: Nhân Vật (Tàng Trữ)** | Trung bình |
| **3** | `TYPE_ZONE`<br>(`Panel.cs:439`) | Chọn khu vực / chuyển phân thân map hiện tại | `paintZone` | `GameScr.gI().zones` | `Service.gI().requestChangeZone` | **Hub 6: Cửu Châu (Giới Vực)** | Thấp |
| **4** | `TYPE_MAP`<br>(`Panel.cs:441`) | Bản đồ thế giới, bản đồ vệ tinh các hành tinh/vùng | `paintMap`<br>(`Panel.cs:6921`) | `TileMap.mapNames`<br>`TileMap.vGo` | Không (chỉ hiển thị) | **Hub 6: Cửu Châu (Địa Đồ)** | Thấp |
| **5** | `TYPE_CLANS`<br>(`Panel.cs:443`) | Quản lý bang hội: danh sách thành viên, xin vào, đóng góp, nhắn tin bang | `paintClans` | `Clan.gI()`<br>`ClanMember[]` | `Service.gI().clanMessage`<br>`Service.gI().clanDonate` | **Hub 4: Tông Môn** | Cao |
| **6** | `TYPE_INFOMATION`<br>(`Panel.cs:445`) | Bảng cộng điểm tiềm năng, xem kinh nghiệm, máu, mana, giáp, chí mạng | `paintChiSo` | `Char.myCharz().cHPGoc`<br>`cMPGoc`<br>`cDamGoc`<br>`cDefGoc` | `Service.gI().upPotential` | **Hub 1: Nhân Vật (Tu Vi / Kinh Mạch)** | Trung bình |
| **7** | `TYPE_BODY`<br>(`Panel.cs:447`) | Xem trang bị đang mặc trên người (Áo, Quần, Găng, Giày, Rada) | `paintInventory` | `Char.myCharz().arrItemBody` | `Service.gI().useItem` (tháo đồ) | **Hub 1: Nhân Vật (Pháp Bảo Bản Mệnh)** | Trung bình |
| **8** | `TYPE_MESS`<br>(`Panel.cs:449`) | Hộp thư riêng tư, xem tin nhắn từ hảo hữu hoặc hệ thống | `paintLogChat` | `InfoDlg.vInfo` | Không (đọc local cache) | **Hub 4: Tông Môn & Hảo Hữu (Truyền Âm)** | Thấp |
| **9** | `TYPE_ARCHIVEMENT`<br>(`Panel.cs:451`) | Bảng thành tựu nhiệm vụ phụ, nhận ngọc thưởng danh hiệu | `paintArchivement` | `Archivement[]` | `Service.gI().getArchivement` | **Hub 2: Tu Luyện (Chiến Tích / Đạo Tâm)** | Thấp |
| **10** | `PLAYER_MENU`<br>(`Panel.cs:453`) | Menu ngữ cảnh tương tác khi bấm chọn người chơi khác | `paintPlayerMenu`<br>(`Panel.cs:5332`) | `Char charFocus` | `Service.gI().addFriend`<br>`Service.gI().giaoDich`<br>`Service.gI().thachDau` | **Popup Context Menu (Tương Tác Đạo Hữu)** | Trung bình |
| **11** | `TYPE_FRIEND`<br>(`Panel.cs:455`) | Danh sách hảo hữu, trạng thái online/offline, chat nhanh | `paintFriend` | `Char.vFriend` | `Service.gI().chatFriend`<br>`Service.gI().deleteFriend` | **Hub 4: Tông Môn & Hảo Hữu (Đạo Hữu)** | Thấp |
| **12** | `TYPE_COMBINE`<br>(`Panel.cs:457`) | Lò luyện trang bị: ép sao, nâng cấp, pha lê hóa trang bị | `paintCombine`<br>`paintCombineNPC` | `Combine.itemCombine`<br>`Combine.gold` | `Service.gI().combine(type, ...)` | **Hub 2: Tu Luyện (Lò Luyện Khí / Luyện Đan)** | Cao |
| **13** | `TYPE_GIAODICH`<br>(`Panel.cs:459`) | Bảng giao dịch 2 người chơi: đặt vật phẩm, khóa, xác nhận trao đổi | `paintGiaoDich`<br>(2 bên) | `Item[] myGD`<br>`Item[] friendGD` | `Service.gI().giaoDichItem`<br>`Service.gI().khoaGiaoDich` | **Hub 5: Phường Thị (Giao Dịch Đạo Lữ)** | Cao |
| **14** | `TYPE_MAPTRANS`<br>(`Panel.cs:461`) | Danh sách điểm truyền tống Capsule / tàu bay qua các map | `paintMapTrans` | `MapTrans.vMap` | `Service.gI().requestMapSelect` | **Hub 6: Cửu Châu (Truyền Tống Trận)** | Thấp |
| **15** | `TYPE_TOP`<br>(`Panel.cs:463`) | Bảng xếp hạng: Sức mạnh, Nạp ngọc, Đại hội võ thuật, Thần thú | `paintTop`<br>(`Panel.cs:3921`) | `TopInfo[]` (Server trả về) | `Service.gI().requestTop(type)` | **Hub 4: Tông Môn (Thiên Kiêu Bảng)** | Thấp |
| **16** | `TYPE_ENEMY`<br>(`Panel.cs:465`) | Danh sách cừu nhân / kẻ thù từng tiêu diệt hoặc cừu sát | `paintEnemy` | `Char.vEnemy` | `Service.gI().deleteEnemy` | **Hub 4: Tông Môn (Sổ Cừu Thù)** | Thấp |
| **17** | `TYPE_KIGUI`<br>(`Panel.cs:467`) | Chợ giao dịch ký gửi: người chơi đăng bán và mua bằng vàng/ngọc | `paintShop` | `Item[] kiguiList` | `Service.gI().kiguiAction` | **Hub 5: Phường Thị (Đấu Giá Các)** | Cao |
| **18** | `TYPE_FLAG`<br>(`Panel.cs:469`) | Bảng chọn màu cờ PK (tự do, bang hội, liên minh) | `paintFlagChange` | `Flag.flags` | `Service.gI().changeFlag(id)` | **Hub 6: Cửu Châu (Chiến Kỳ)** | Thấp |
| **19** | `TYPE_OPTION`<br>(`Panel.cs:471`) | Cấu hình cài đặt game: Nhạc, Hiệu ứng, Đồ họa cao/thấp, Phím ảo | `paintOption` | `Rms` / `GameCanvas` settings | Không (lưu cục bộ RMS) | **Hub 6: Cửu Châu (Thiết Lập Đạo Quả)** | Thấp |
| **20** | `TYPE_ACCOUNT`<br>(`Panel.cs:473`) | Thông tin tài khoản, đổi mật khẩu, kích hoạt số điện thoại/email | `paintAccount` | `Session_ME` username | `Service.gI().changePass` | **Hub 6: Cửu Châu (Đạo Lộ Danh Phẩm)** | Thấp |
| **21** | `TYPE_PET_MAIN`<br>(`Panel.cs:475`) | Quản lý Đệ tử / Linh thú: Chỉ số, trang bị riêng của Pet, trạng thái bảo vệ | `paintPetInventory`<br>`paintPetStatus` | `Char.myCharz().arrItemPet`<br>`Char.myCharz().cHPGoc_Pet` | `Service.gI().petStatus`<br>`Service.gI().useItemPet` | **Hub 3: Linh Thú (Linh Sủng / Đạo Đồng)** | Trung bình |
| **22** | `TYPE_AUTO`<br>(`Panel.cs:477`) | Cấu hình tự động luyện tập: Tự đánh, Tự hồi phục đậu, Gom vật phẩm | `paintAuto` | `AutoTrain` settings | `Service.gI().autoTrain` | **Hub 2: Tu Luyện (Bế Quan Tu Vi)** | Thấp |
| **23** | `TYPE_GAMEINFO`<br>(`Panel.cs:479`) | Bảng thông báo hệ thống / Menu chức năng Mod | `MenuHandler.paintMenuMod` | Mod options / Event data | Tùy chọn Mod | **Hub 6: Cửu Châu (Bảng Tin Thiên Giới)** | Thấp |
| **24** | `TYPE_GAMEINFOSUB`<br>(`Panel.cs:481`) | Chi tiết bài viết tin tức, hướng dẫn sự kiện game | `paintGameSubInfo` | Sub-articles text | Không (chỉ hiển thị) | **Hub 6: Cửu Châu (Chi Tiết Điển Tích)** | Thấp |
| **25** | `TYPE_SPEACIALSKILL`<br>(`Panel.cs:483`) | Bảng Nội tại / Bí kíp đặc biệt của hệ phái nhân vật | `paintSpeacialSkill` | `SpeacialSkill` data | `Service.gI().speacialSkillAction` | **Hub 2: Tu Luyện (Tâm Pháp Thần Thông)** | Trung bình |

---

## 3. Danh Mục Các Màn Hình Khác (Screen Inventory)

| Class | Vai Trò | Tình Trạng Mã Nguồn | Đánh Giá Tương Thích Theme Tu Tiên |
|:---|:---|:---|:---|
| `LoginScr.cs` | Đăng nhập tài khoản, chọn máy chủ | 24 KB, vẽ trực tiếp qua Graphics | Cần đại tu giao diện: Cổng Thiên Môn, mây khói, phông chữ thư pháp |
| `ServerListScreen.cs` | Danh sách máy chủ (Khu vực, Cụm server) | 28 KB, cuộn danh sách server | Cần bố trí theo kiểu Cửu Trọng Thiên / Vị Diện |
| `CreateCharScr.cs` | Tạo nhân vật (Chọn môn phái/hành tinh, tóc, mắt) | 18 KB, preview 3D/sprite | Biến đổi thành chọn Đạo Thể (Thần Tộc, Ma Tộc, Tiên Tộc) |
| `ChooseCharScr.cs` | Chọn nhân vật đã tạo | 4.3 KB | Khung bài vị / Thẻ bài nhân vật phong cách Tiên hiệp |
| `CrackBallScr.cs` | Vòng quay may mắn / Mở rương báu | 15.8 KB | Biến đổi thành Bát Quái Trận Đồ / Vòng Quay Tầm Bảo |
| `RadarScr.cs` / `Info_RadaScr.cs` | Radar sưu tầm quái vật & danh hiệu | 15.5 KB + 5.5 KB | Biến đổi thành "Vạn Thú Đồ Lục" / "Sơn Hải Kinh" |
| `Menu.cs` | Menu tương tác NPC (hộp thoại lựa chọn) | 13.1 KB | Hộp thoại phong cách thẻ ngọc (Ngọc giản truyền tin) |
| `InputDlg.cs` / `InfoDlg.cs` / `MsgDlg.cs` | Hộp thoại thông báo, xác nhận, nhập liệu | 1.3 KB - 2.5 KB | Khung viền mạ đồng cổ kính, nút thư pháp |
| `NewPanel.cs` | Khung Panel mở rộng | 23 dòng (Stub rỗng) | Sẵn sàng làm vỏ bọc kế thừa cho hệ thống UI mới |

---

## 4. Đề Xuất Kiến Trúc Tái Cấu Trúc Thành 6 Hub Tu Tiên

Để phá vỡ tảng đá nguyên khối 11,703 dòng của `Panel.cs`, toàn bộ 26 loại panel sẽ được gom nhóm thành 6 Hub màn hình độc lập (theo Phụ lục B của FINAL_PLAN):

```text
[GIAO DIỆN TU TIÊN MỚI]
├── Hub 1: BẢN NGUYÊN (Nhân Vật)
│   ├── Khí Hải & Kinh Mạch (Chỉ số / TYPE_INFOMATION)
│   ├── Tàng Trữ Đạo Đại (Túi đồ / TYPE_MAIN - Tab 1)
│   ├── Bản Mệnh Pháp Bảo (Trang bị / TYPE_BODY)
│   └── Càn Khôn Đạo Trữ (Rương đồ / TYPE_BOX)
│
├── Hub 2: ĐẠO ĐẠO (Tu Luyện)
│   ├── Thần Thông & Công Pháp (Kỹ năng / TYPE_MAIN - Tab 2 & TYPE_SPEACIALSKILL)
│   ├── Lò Luyện Khí & Luyện Đan (Ghép đồ / TYPE_COMBINE)
│   ├── Bế Quan Tham Thiền (Auto train / TYPE_AUTO)
│   └── Đạo Tâm Chiến Tích (Thành tựu / TYPE_ARCHIVEMENT)
│
├── Hub 3: LINH THÚ (Đồng Hành)
│   ├── Linh Sủng / Đạo Đồng (Pet info / TYPE_PET_MAIN)
│   └── Thú Trang (Trang bị riêng linh thú)
│
├── Hub 4: TÔNG MÔN (Xã Hội)
│   ├── Tông Môn Điện (Bang hội / TYPE_CLANS)
│   ├── Đạo Hữu Lục (Bạn bè / TYPE_FRIEND)
│   ├── Cừu Địch Bảng (Kẻ thù / TYPE_ENEMY)
│   └── Thiên Kiêu Bảng (Xếp hạng / TYPE_TOP)
│
├── Hub 5: THƯƠNG TRƯỜNG (Kinh Tế)
│   ├── Tiên Gia Phường Thị (Shop NPC / TYPE_SHOP)
│   ├── Vạn Bảo Đấu Giá Các (Ký gửi / TYPE_KIGUI)
│   └── Đạo Lữ Trao Đổi (Giao dịch 2 người / TYPE_GIAODICH)
│
└── Hub 6: THIÊN ĐỊA (Thế Giới & Cài Đặt)
    ├── Cửu Châu Đại Địa Đồ (Bản đồ / TYPE_MAP)
    ├── Không Gian Truyền Tống (Capsule / TYPE_MAPTRANS)
    ├── Phân Thân Hư Giới (Đổi khu / TYPE_ZONE)
    └── Đạo Pháp Thiết Lập (Cài đặt / TYPE_OPTION & TYPE_ACCOUNT)
```

---

## 5. Rủi Ro & Chiến Lược Di Trú (Migration Risk Matrix)

1. **Rủi ro cao nhất (High Risk):**
   - `TYPE_COMBINE` (Ghép đồ): Logic animation phức tạp (`paintCombineEff`, `combineSuccess`), nếu tách lớp sai sẽ gây mất hiệu ứng nâng cấp đồ hoặc kẹt UI.
   - `TYPE_GIAODICH` (Giao dịch): Logic đồng bộ thời gian thực hai chiều giữa 2 người chơi. Bất kỳ lỗi hiển thị nào cũng có thể dẫn đến hiểu lầm hack/mất đồ.
   - `TYPE_CLANS` (Bang hội): Chứa nhiều trạng thái phụ (xem thành viên, xin vào bang, đóng góp đậu).
2. **Chiến lược bảo toàn:**
   - **Giai đoạn đầu (Track U):** Giữ nguyên `Panel.cs` làm backend xử lý logic dữ liệu. Lớp UI Tu Tiên mới sẽ đóng vai trò Renderer/View hiện đại hơn hoặc bọc ngoài thông qua Adapter pattern, không sửa đổi thuật toán xử lý item trong `Panel.cs`.
