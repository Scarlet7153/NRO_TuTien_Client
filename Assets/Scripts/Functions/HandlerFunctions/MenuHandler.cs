using Functions.AutoFunctions;
using MapObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Functions.HandlerFunctions
{
    internal class MenuHandler
{
		public static string[][] MenuOption = new string[][]
	{
			new string[]
			{
				"D.sách",
				"Item"
			},
			new string[]
			{
				"Chức",
				"Năng"
			},
			new string[]
            {
				"H.Dẫn","Sử Dụng"
            }
	};
		public static int PANEL_TYPE { get; set; }
		/// <summary>
		public static int[] LIST_ITEM_ICONID = new int[]
		{
			// 1. Tăng chỉ số cơ bản
			2755,  // Bổ huyết (id 382)
			2756,  // Bổ khí (id 383)
			2754,  // Cuồng nộ (id 381)
			2757,  // Giáp Xên bọ hung (id 384)
			2760,  // Ẩn danh (id 385)
			2758,  // Máy dò Capsule kì bí (id 379)

			// 2. Tăng chỉ số siêu cấp
			14424, // Bổ Huyết Siêu Cấp (id 1100)
			14425, // Bổ Khí Siêu Cấp (id 1101)
			14426, // Cuồng Nộ Siêu Cấp (id 1099)
			10712, // Giáp Xên Siêu Cấp (id 1102)
			10717, // Ẩn Danh Đặc Biệt (id 1103)

			// 3. Đan dược tu tiên (TNSM)
			22066, // Tụ khí đan (id 2033)
			22067, // Ngưng khí đan (id 2034)
			22068, // Trúc cơ đan (id 2035)
			22069, // Ngộ đạo đan (id 2036)
			22942, // Định thần đan (id 1265)

			// 4. Thức ăn tăng chỉ số
			6324,  // Bánh Pudding (id 663)
			6325,  // Xúc xích (id 664)
			6326,  // Kem dâu (id 665)
			6327,  // Mì ly (id 666)
			6328,  // Sushi (id 667)

			// 5. Thức ăn sự kiện & Đặc biệt
			7149,  // Khẩu trang (id 764)
			8060,  // Cua rang me (id 880)
			8061,  // Bạch tuộc nướng (id 881)
			8062,  // Tôm tẩm bột chiên xù (id 882)
			7079,  // Bánh tét (id 752)
			7080,  // Bánh chưng (id 753)
			8243,  // Kẹo một mắt (id 899)
			22727, // Khóa ác quỷ (id 1354)

			// 6. Huyết Long (Ngọc Rồng Đen)
			15543, // Huyết Long 1 Sao (id 1822)
			15544, // Huyết Long 2 Sao (id 1823)
			15545, // Huyết Long 3 Sao (id 1824)
			15546, // Huyết Long 4 Sao (id 1825)
			15547, // Huyết Long 5 Sao (id 1826)
			15548, // Huyết Long 6 Sao (id 1827)
			15549, // Huyết Long 7 Sao (id 1828)

			// 7. Ngọc buff Tu Tiên
			20716, // Ngọc Siêu Thần (id 1829)
			20717, // Ngọc Vương Giả (id 1830)
			20719, // Ngọc Thiên Tử (id 1831)
			20720, // Ngọc Kỳ Thiên (id 1832)

			// 8. Máy dò đặc biệt
			22071, // Máy dò bóng tối (id 1201 / id 1353)
			22070, // Máy dò đá bóng tối THƯỜNG (id 1352)
			14397  // Máy dò Boss (id 1422)
		};

		// Token: 0x04001395 RID: 5013
		public static string[] LIST_ITEM_NAME = new string[]
		{
			// 1. Tăng chỉ số cơ bản
			"Bổ huyết",
			"Bổ khí",
			"Cuồng nộ",
			"Giáp Xên bọ hung",
			"Ẩn danh",
			"Máy dò Capsule kì bí",

			// 2. Tăng chỉ số siêu cấp
			"Bổ Huyết Siêu Cấp",
			"Bổ Khí Siêu Cấp",
			"Cuồng Nộ Siêu Cấp",
			"Giáp Xên Siêu Cấp",
			"Ẩn Danh Đặc Biệt",

			// 3. Đan dược tu tiên (TNSM)
			"Tụ khí đan",
			"Ngưng khí đan",
			"Trúc cơ đan",
			"Ngộ đạo đan",
			"Định thần đan",

			// 4. Thức ăn tăng chỉ số
			"Bánh Pudding",
			"Xúc xích",
			"Kem dâu",
			"Mì ly",
			"Sushi",

			// 5. Thức ăn sự kiện & Đặc biệt
			"Khẩu trang",
			"Cua rang me",
			"Bạch tuộc nướng",
			"Tôm tẩm bột chiên xù",
			"Bánh tét",
			"Bánh chưng",
			"Kẹo một mắt",
			"Khóa ác quỷ",

			// 6. Huyết Long (Ngọc Rồng Đen)
			"Huyết Long 1 Sao",
			"Huyết Long 2 Sao",
			"Huyết Long 3 Sao",
			"Huyết Long 4 Sao",
			"Huyết Long 5 Sao",
			"Huyết Long 6 Sao",
			"Huyết Long 7 Sao",

			// 7. Ngọc buff Tu Tiên
			"Ngọc Siêu Thần",
			"Ngọc Vương Giả",
			"Ngọc Thiên Tử",
			"Ngọc Kỳ Thiên",

			// 8. Máy dò đặc biệt
			"Máy dò bóng tối",
			"Máy dò đá bóng tối THƯỜNG",
			"Máy dò Boss"
		};

		// Token: 0x04001396 RID: 5014
		public static string[] TUTORIAL_SETTINGS = new string[]
		{
			"=== 1. PHÍM TẮT (PC) ===",
			"A: Bật / Tắt Tự Đánh",
			"B: Dùng Bông Tai Nhanh",
			"C: Dùng Capsule Nhanh",
			"E: Bật / Tắt Auto Hồi Sinh",
			"M / F: Mở Bảng Đổi Khu",
			"X: Mở Menu Mod Tiện Ích",
			"J: Load Qua Map Trái",
			"K: Load Qua Map Giữa",
			"L: Load Qua Map Phải",
			"Q: Dừng Auto / Đập Đồ",
			"Space: Bơm Đậu Thần (HP/KI)",
			"R: Mở Khung Chat",
			"Y: Đồng Ý (Yes) / Tin Nhắn",
			"1 - 5: Phím Tắt Kỹ Năng",
			"=== 2. LỆNH CHAT ===",
			"ak: Bật / Tắt Tự Đánh",
			"dapdo: Bật / Tắt Auto Đập Đồ",
			"tdlt: Bật / Tắt Tàn Sát",
			"k [X]: Đổi Khu X (vd: k 5)",
			"vq: Bật / Tắt Quay Thượng Đế",
			"alogin: Bật / Tắt Auto Login",
			"ahs: Bật / Tắt Auto Hồi Sinh",
			"anhat: Bật / Tắt Auto Nhặt",
			"huy / stop: Dừng Mua Nhiều",
			"/atc|[nội dung]: Auto Chat (5s)"
		};
		
		public static string[] GRAPHIC_SETTING_LIST_NAME = new string[]
		{
			"Xóa Map",
			"Bật Nền Màu RGB",
			"Danh Sách Người Chơi",
			"Thông Báo Boss",
			"Đường Kẻ Tới Boss",
			"Tàn Sát",
			"Auto Nhặt",
			"Auto Login",
			"Auto Hồi Sinh",
			"Auto Up Đệ"
			
		};
		public static string Setting_SubNames(int index)
		{
			string result;
			switch (index)
			{
				case 0:
					result = "Trạng thái: " + InfoHandler.StatusMenu(GraphicsHandler.xoamap);
					break;
				case 1:
					result = "Trạng thái: " + InfoHandler.StatusMenu(GraphicsHandler.enablePaintColor_Wallpaper);
					break;
				case 2:
					result = "Trạng thái: " + InfoHandler.StatusMenu(GraphicsHandler.listchar);
					break;
				case 3:
					result = "Trạng thái: " + InfoHandler.StatusMenu(MainFunctions.isSanBoss);
					break;
				case 4:
					result = "Trạng thái: " + InfoHandler.StatusMenu(BossFunctions.LineBoss);
					break;
				case 5:
					result = "Trạng thái: " + InfoHandler.StatusMenu(AutoPickMobHandler.IsTanSat);
					break;
				case 6:
					result = "Trạng thái: " + InfoHandler.StatusMenu(AutoPickFunctions.isAutoPick);
					break;
				case 7:
					result = "Trạng thái: " + InfoHandler.StatusMenu(CharFunctions.isAutoLogin);
					break;
				case 8:
					result = "Trạng thái: " + InfoHandler.StatusMenu(CharFunctions.getInstance().AutoRivive());
					break;
				case 9:
					result = "Trạng thái: " + InfoHandler.StatusMenu(Char.isPetHandler);
					break;
				default:
					result = string.Empty;
					break;
			}
			return result;
		}
		public static void doFireGraphicSetting(int selected)
		{
			bool flag = selected == -1;
			if (!flag)
			{
				switch (selected)
				{
					case 0:
						GraphicsHandler.xoamap = !GraphicsHandler.xoamap;
						GameScr.info1.addInfo("Xóa Map: " + InfoHandler.Status(GraphicsHandler.xoamap), 0);
						break;
					case 1:
						{
							bool flag2 = !GraphicsHandler.enablePaintColor_Wallpaper;
							if (flag2)
							{
								ChatableHandler.gI().OpenChat("Nhập mã màu nền");
								GraphicsHandler.enablePaintColor_Wallpaper = true;
							}
							else
							{
								GraphicsHandler.enablePaintColor_Wallpaper = false;
							}
							GameScr.info1.addInfo("Bật hình nền Color RGB: " + InfoHandler.Status(GraphicsHandler.enablePaintColor_Wallpaper), 0);
							break;
						}
					case 2:
						GraphicsHandler.listchar = !GraphicsHandler.listchar;
						GameScr.info1.addInfo("D.sách Nhân Vật: " + InfoHandler.Status(GraphicsHandler.listchar), 0);
						break;
					case 3:
						MainFunctions.isSanBoss = !MainFunctions.isSanBoss;
						GameScr.info1.addInfo("Thông Báo Boss: " + InfoHandler.Status(MainFunctions.isSanBoss), 0);
						break;
					case 4:
						BossFunctions.LineBoss = !BossFunctions.LineBoss;
						GameScr.info1.addInfo("Đường Kẻ Tới Boss: " + InfoHandler.Status(BossFunctions.LineBoss), 0);
						break;
					case 5:
						AutoPickMobHandler.IsTanSat = !AutoPickMobHandler.IsTanSat;
						GameScr.info1.addInfo("Tàn Sát: " + InfoHandler.Status(AutoPickMobHandler.IsTanSat), 0);
						break;
					case 6:
						AutoPickFunctions.isAutoPick = !AutoPickFunctions.isAutoPick;
						GameScr.info1.addInfo("Auto Nhặt: " + InfoHandler.Status(AutoPickFunctions.isAutoPick), 0);
						break;
					case 7:
						CharFunctions.isAutoLogin = !CharFunctions.isAutoLogin;
						GameScr.info1.addInfo("Auto Login: " + InfoHandler.Status(CharFunctions.isAutoLogin), 0);
						break;
					case 8:
						CharFunctions.getInstance().Handler();
						GameScr.info1.addInfo("Auto Hồi Sinh: " + InfoHandler.Status(CharFunctions.getInstance().AutoRivive()), 0);
						break;
					case 9:
						Char.isPetHandler = !Char.isPetHandler;
						GameScr.info1.addInfo("Auto Up Đệ: " + InfoHandler.Status(Char.isPetHandler), 0);
						break;
					
				}
			}
		}
		/// </summary>
		/// <param name="panelType"></var>

		public class ActiveItem
		{
			public int iconID;
			public string name;
			public int quantity;
			public bool isAuto;
		}

		public static List<ActiveItem> activeItems = new List<ActiveItem>();

		public static bool isBuffItem(Item item)
		{
			if (item == null || item.template == null)
			{
				return false;
			}
			int icon = (int)item.template.iconID;
			for (int i = 0; i < LIST_ITEM_ICONID.Length; i++)
			{
				if (LIST_ITEM_ICONID[i] == icon)
				{
					return true;
				}
			}
			return item.template.type == 29;
		}

		public static void updateActiveItems()
		{
			activeItems.Clear();
			if (global::Char.myCharz() == null || global::Char.myCharz().arrItemBag == null)
			{
				return;
			}
			List<int> addedIcons = new List<int>();

			// 1. Quét các item trong hành trang của nhân vật
			for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
			{
				Item item = global::Char.myCharz().arrItemBag[i];
				if (item != null && item.template != null)
				{
					int icon = (int)item.template.iconID;
					if (isBuffItem(item) && !addedIcons.Contains(icon))
					{
						addedIcons.Add(icon);
						bool isAuto = false;
						for (int k = 0; k < ItemHandler.ListItemAuto.Count; k++)
						{
							if (ItemHandler.ListItemAuto[k].iconID == icon)
							{
								isAuto = true;
								break;
							}
						}
						activeItems.Add(new ActiveItem
						{
							iconID = icon,
							name = item.template.name,
							quantity = ItemHandler.ItemQuantity(icon, "iconID"),
							isAuto = isAuto
						});
					}
				}
			}

			// 2. Thêm các item đang có trong danh sách ListItemAuto nhưng tạm hết trong túi (để người chơi có thể xóa)
			for (int k = 0; k < ItemHandler.ListItemAuto.Count; k++)
			{
				int icon = ItemHandler.ListItemAuto[k].iconID;
				if (!addedIcons.Contains(icon))
				{
					addedIcons.Add(icon);
					activeItems.Add(new ActiveItem
					{
						iconID = icon,
						name = ItemHandler.ListItemAuto[k].name,
						quantity = ItemHandler.ItemQuantity(icon, "iconID"),
						isAuto = true
					});
				}
			}
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x000AE8E1 File Offset: 0x000ACAE1
		public static void paintItemList(mGraphics g)
		{
			updateActiveItems();
			GameCanvas.panel.currentListLength = activeItems.Count;
			g.setClip(GameCanvas.panel.xScroll, GameCanvas.panel.yScroll, GameCanvas.panel.wScroll, GameCanvas.panel.hScroll);
			g.translate(0, -GameCanvas.panel.cmy);

			if (activeItems.Count == 0)
			{
				mFont.tahoma_7_white.drawString(g, "Không có vật phẩm buff nào trong người", GameCanvas.panel.xScroll + 10, GameCanvas.panel.yScroll + 15, 0);
				GameCanvas.panel.paintScrollArrow(g);
				return;
			}

			for (int i = 0; i < activeItems.Count; i++)
			{
				int xScroll = GameCanvas.panel.xScroll;
				int num = GameCanvas.panel.yScroll + i * GameCanvas.panel.ITEM_HEIGHT;
				int w = GameCanvas.panel.wScroll - 1;
				int h = GameCanvas.panel.ITEM_HEIGHT - 1;
				bool flag = num - GameCanvas.panel.cmy <= GameCanvas.panel.yScroll + GameCanvas.panel.hScroll && num - GameCanvas.panel.cmy >= GameCanvas.panel.yScroll - GameCanvas.panel.ITEM_HEIGHT;
				if (flag)
				{
					ActiveItem item = activeItems[i];
					if (i == GameCanvas.panel.selected)
					{
						g.setColor(16383818, 0.5f);
						g.fillRect(xScroll, num, w, h);
					}
					else
					{
						g.setColor(0, 0.35f);
						g.fillRect(xScroll, num, w, h);
					}
					if (mGraphics.zoomLevel == 1)
					{
						mFont.tahoma_7b_green.drawString(g, item.name, xScroll + 30, num + 2, 0);
					}
					else
					{
						mFont.tahoma_7_white.drawStringBd(g, item.name, xScroll + 30, num + 2, 0, mFont.tahoma_7b_dark);
					}
					SmallImage.drawSmallImage(g, item.iconID, xScroll + 2, num + 3, 0, 0);
					string st = (item.quantity > 0) ? ("Số lượng: x" + item.quantity) : "Số lượng: x0";
					mFont font = (item.quantity > 0) ? mFont.tahoma_7_yellow : mFont.tahoma_7b_dark;
					if (item.isAuto)
					{
						font = mFont.tahoma_7b_red;
						st = "ẤN ĐỂ XÓA KHỎI DANH SÁCH ! ! !";
					}
					font.drawString(g, st, xScroll + 30, num + 12, 0);
				}
			}
			GameCanvas.panel.paintScrollArrow(g);
		}
		

		// Token: 0x06000A88 RID: 2696 RVA: 0x000AE15C File Offset: 0x000AC35C
		public static void paintGraphicSetting(mGraphics g)
		{
			g.setClip(GameCanvas.panel.xScroll, GameCanvas.panel.yScroll, GameCanvas.panel.wScroll, GameCanvas.panel.hScroll);
			g.translate(0, -GameCanvas.panel.cmy);
			for (int i = 0; i < MenuHandler.GRAPHIC_SETTING_LIST_NAME.Length; i++)
			{
				int xScroll = GameCanvas.panel.xScroll;
				int num = GameCanvas.panel.yScroll + i * GameCanvas.panel.ITEM_HEIGHT;
				int w = GameCanvas.panel.wScroll - 1;
				int h = GameCanvas.panel.ITEM_HEIGHT - 1;
				bool flag = num - GameCanvas.panel.cmy <= GameCanvas.panel.yScroll + GameCanvas.panel.hScroll && num - GameCanvas.panel.cmy >= GameCanvas.panel.yScroll - GameCanvas.panel.ITEM_HEIGHT;
				if (flag)
				{
					g.setColor((i != GameCanvas.panel.selected) ? 0 : 0, 0.5f);
					g.fillRect(xScroll, num, w, h);
					mFont.tahoma_7_white.drawString(g, (i + 1).ToString() + ". " + MenuHandler.GRAPHIC_SETTING_LIST_NAME[i], xScroll + 5, num, 0);
					bool flag2 = MenuHandler.Setting_SubNames(i).Contains("Trạng thái: Đang Bật");
					if (flag2)
					{
						g.setColor((i != GameCanvas.panel.selected) ? 0 : 0, 0.5f);
						g.fillRect(xScroll, num, w, h);
					}
					((MenuHandler.Setting_SubNames(i) == "Trạng thái: Đang Bật") ? mFont.tahoma_7_yellow : mFont.tahoma_7).drawString(g, MenuHandler.Setting_SubNames(i), xScroll + 5, num + 11, 0);
				}
			}
		}
		public static void paintTutorialSettings(mGraphics g)
		{
			g.setClip(GameCanvas.panel.xScroll, GameCanvas.panel.yScroll, GameCanvas.panel.wScroll, GameCanvas.panel.hScroll);
			g.translate(0, -GameCanvas.panel.cmy);
			for (int i = 0; i < TUTORIAL_SETTINGS.Length; i++)
			{
				int xScroll = GameCanvas.panel.xScroll;
				int num = GameCanvas.panel.yScroll + i * GameCanvas.panel.ITEM_HEIGHT;
				int w = GameCanvas.panel.wScroll - 1;
				int h = GameCanvas.panel.ITEM_HEIGHT - 1;
				bool flag = num - GameCanvas.panel.cmy <= GameCanvas.panel.yScroll + GameCanvas.panel.hScroll && num - GameCanvas.panel.cmy >= GameCanvas.panel.yScroll - GameCanvas.panel.ITEM_HEIGHT;
				if (flag)
				{
					bool isHeader = TUTORIAL_SETTINGS[i].StartsWith("=");
					g.setColor((i != GameCanvas.panel.selected) ? (isHeader ? 14145495 : 15196114) : 16383818);
					g.fillRect(xScroll, num, w, h);
					mFont font = isHeader ? mFont.tahoma_7b_red : mFont.tahoma_7b_blue;
					font.drawString(g, MenuHandler.TUTORIAL_SETTINGS[i], xScroll + 8, num + 5, 0);
				}
			}

			GameCanvas.panel.paintScrollArrow(g);
		}
		// Token: 0x06000A89 RID: 2697 RVA: 0x000AE32C File Offset: 0x000AC52C
		
		public static void doFireMenu()
		{
			switch (GameCanvas.panel.currentTabIndex)
			{
				case 0:
					MenuHandler.doFireItem(GameCanvas.panel.selected);
					break;
				case 1:
					MenuHandler.doFireGraphicSetting(GameCanvas.panel.selected);
					break;
				
			}
		}
		public static void paintMenuMod(mGraphics g)
		{
			int panel_TYPE = MenuHandler.PANEL_TYPE;
			int num = panel_TYPE;
			if (num == 0)
			{
				MenuHandler.paintModMenu(g);
			}
		}
		public static void paintModMenu(mGraphics g)
		{
			switch (GameCanvas.panel.currentTabIndex)
			{
				case 0:
					MenuHandler.paintItemList(g);
					break;
				case 1:
					MenuHandler.paintGraphicSetting(g);
					break;
				case 2:
					MenuHandler.paintTutorialSettings(g);
					break;
			
			}
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x000ADE94 File Offset: 0x000AC094
		public static void doFireItem(int selected)
		{
			updateActiveItems();
			if (selected < 0 || selected >= activeItems.Count)
			{
				return;
			}
			ActiveItem target = activeItems[selected];
			if (target.isAuto)
			{
				for (int k = 0; k < ItemHandler.ListItemAuto.Count; k++)
				{
					if (ItemHandler.ListItemAuto[k].iconID == target.iconID)
					{
						ItemHandler.ListItemAuto.RemoveAt(k);
						GameScr.info1.addInfo("Đã xóa " + target.name + " khỏi d/s item", 0);
						break;
					}
				}
			}
			else
			{
				if (target.quantity <= 0)
				{
					GameScr.info1.addInfo("Bạn Không Có Item!", 0);
					return;
				}
				ItemHandler.ListItemAuto.Add(new ItemHandler.Items(target.iconID, target.name));
				GameScr.info1.addInfo("Đã thêm " + target.name + " vào d/s item", 0);
			}
			updateActiveItems();
			GameCanvas.panel.currentListLength = activeItems.Count;
			GameCanvas.panel.cmyLim = GameCanvas.panel.currentListLength * GameCanvas.panel.ITEM_HEIGHT - GameCanvas.panel.hScroll;
			if (GameCanvas.panel.cmyLim < 0)
			{
				GameCanvas.panel.cmyLim = 0;
			}
		}
		public static void setTypeMenuMod(int panelType)
		{
			GameCanvas.panel.type = 23;
			MenuHandler.PANEL_TYPE = panelType;
			MenuHandler.setTypeModMenu();
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x000AE900 File Offset: 0x000ACB00
		public static void setTypeModMenu()
		{
			SoundMn.gI().getSoundOption();
			GameCanvas.panel.setType(0);
			bool flag = MenuHandler.PANEL_TYPE == 0;
			if (flag)
			{
				GameCanvas.panel.tabName[23] = MenuHandler.MenuOption;
				GameCanvas.panel.setType(0);
				MenuHandler.setTabMenuMod();
			}
		}
		public static void setTabMenuMod()
		{
			int panel_TYPE = MenuHandler.PANEL_TYPE;
			int num = panel_TYPE;
			if (num == 0)
			{
				MenuHandler.setTabModMenu();
			}
		}
		public static void setTabModMenu()
		{
			switch (GameCanvas.panel.currentTabIndex)
			{
				case 0:
					updateActiveItems();
					GameCanvas.panel.currentListLength = activeItems.Count;
					break;
				case 1:
					GameCanvas.panel.currentListLength = MenuHandler.GRAPHIC_SETTING_LIST_NAME.Length;
					break;
				case 2:
					GameCanvas.panel.currentListLength = MenuHandler.TUTORIAL_SETTINGS.Length;
					break;
			
			}
			GameCanvas.panel.ITEM_HEIGHT = 24;
			GameCanvas.panel.selected = (GameCanvas.isTouch ? -1 : 0);
			GameCanvas.panel.cmyLim = GameCanvas.panel.currentListLength * GameCanvas.panel.ITEM_HEIGHT - GameCanvas.panel.hScroll;
			bool flag = GameCanvas.panel.cmyLim < 0;
			if (flag)
			{
				GameCanvas.panel.cmyLim = 0;
			}
			GameCanvas.panel.cmy = (GameCanvas.panel.cmtoY = GameCanvas.panel.cmyLast[GameCanvas.panel.currentTabIndex]);
			bool flag2 = GameCanvas.panel.cmy < 0;
			if (flag2)
			{
				GameCanvas.panel.cmy = (GameCanvas.panel.cmtoY = 0);
			}
			bool flag3 = GameCanvas.panel.cmy > GameCanvas.panel.cmyLim;
			if (flag3)
			{
				GameCanvas.panel.cmy = (GameCanvas.panel.cmtoY = GameCanvas.panel.cmyLim);
			}
		}
	}
}
