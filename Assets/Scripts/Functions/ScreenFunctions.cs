using Functions.AutoFunctions;
using Functions.HandlerFunctions;
using System;
using UnityEngine;

namespace Functions
{
  internal class ScreenFunctions : IChatable
{
		/// <summary>
		private static Image imgSettings = mSystem.loadImage("/pc/myTexture2dSettings.png");
		private static Image imgLogo = mSystem.loadImage("/mainImage/logo1E.png");
		/// </summary>
		/// <param ></param>
		public static void Paint(mGraphics g) {
			paintListBosses(g);
			paintLineBoss(g);
			paintListChar(g);
			paintUpgrade(g);
			drawString(g);
			paintModInfo(g);
            if (MenuFunctions.isPetSPInfo)
            {
                paintInfo(g);
            }
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
				g.drawImage(imgSettings, 216, 3); 
			}
            g.drawImage(imgLogo, GameCanvas.w / 2, 29, 3);

            if (global::Char.myCharz().mobFocus != null && global::Char.myCharz().charFocus == null)
			{
				paintMobFocusInfo(g, Char.myCharz().mobFocus);
			}
		}
		private static void paintInfo(mGraphics g) {
			GUIStyle guistyle = new GUIStyle(GUI.skin.label);
			guistyle.normal.textColor = Color.magenta;
			guistyle.fontSize = 17;
			int num = 125;
            //mFont.tahoma_7_whiteSmall.drawString(g, "Sư Phụ [" + Char.myCharz().cName + "]", 10, num, mFont.LEFT);
            mFont.tahoma_7_white.drawStringBd(g, "Sư Phụ [" + Char.myCharz().cName + "]", 10, num, mFont.LEFT, mFont.tahoma_7_grey);
            //g.drawString("Sư Phụ [" + Char.myCharz().cName + "]", 10, num, guistyle);
            num += 10;
			//mFont.tahoma_7_blue1Small.drawString(g, "SM: " + NinjaUtil.getMoneys((long)global::Char.myCharz().cPower), 10, num, mFont.LEFT);
            g.drawString("SM: " + NinjaUtil.getMoneys((long)global::Char.myCharz().cPower), 10, num, guistyle);
            num += 10;
            //mFont.tahoma_7_blue1Small.drawString(g, "TN: " + NinjaUtil.getMoneys((long)global::Char.myCharz().cTiemNang), 10, num, mFont.LEFT);
            g.drawString("TN: " + NinjaUtil.getMoneys((long)global::Char.myCharz().cTiemNang), 10, num, guistyle);
			num += 10;
            g.drawString("HP: " + (global::Char.myCharz().cHP >= 100000000L ? NinjaUtil.formatShortNumber(global::Char.myCharz().cHP) : NinjaUtil.getMoneys((long)global::Char.myCharz().cHP)), 10, num, guistyle);
			num += 10;
            //mFont.tahoma_7_blue1Small.drawString(g, "KI: " + NinjaUtil.getMoneys((long)global::Char.myCharz().cMP), 10, num, mFont.LEFT);
            g.drawString("KI: " + (global::Char.myCharz().cMP >= 100000000L ? NinjaUtil.formatShortNumber(global::Char.myCharz().cMP) : NinjaUtil.getMoneys((long)global::Char.myCharz().cMP)), 10, num, guistyle);
			num += 10;
            //mFont.tahoma_7_blue1Small.drawString(g, "SD: " + NinjaUtil.getMoneys((long)global::Char.myCharz().cDamFull), 10, num, mFont.LEFT);
            g.drawString("SD: " + (global::Char.myCharz().cDamFull >= 100000000L ? NinjaUtil.formatShortNumber(global::Char.myCharz().cDamFull) : NinjaUtil.getMoneys((long)global::Char.myCharz().cDamFull)), 10, num, guistyle);
			if (Char.myCharz().havePet)
			{
				if (mSystem.currentTimeMillis() - MenuFunctions.lastTimeGetPetSPInfo >= 1000)
				{
                    MenuFunctions.lastTimeGetPetSPInfo = mSystem.currentTimeMillis();
                    Service.gI().petInfo();
				}
				GUIStyle guistyle2 = new GUIStyle(GUI.skin.label);
				guistyle2.normal.textColor = Color.red;
				guistyle2.fontSize = 17;
				int num2 = 125;
				//g.drawString("Đệ Tử [" + Char.myPetz().cName + "]", 80, num2, guistyle2);
                mFont.tahoma_7_white.drawStringBd(g, "Đệ Tử [" + Char.myPetz().cName + "]", 90, num2, mFont.LEFT, mFont.tahoma_7_grey);
                num2 += 10;
				g.drawString("SM: " + NinjaUtil.getMoneys((long)global::Char.myPetz().cPower), 90, num2, guistyle2);
				num2 += 10;
				g.drawString("TN: " + NinjaUtil.getMoneys((long)global::Char.myPetz().cTiemNang), 90, num2, guistyle2);
				num2 += 10;
				g.drawString("HP: " + (global::Char.myPetz().cHP >= 100000000L ? NinjaUtil.formatShortNumber(global::Char.myPetz().cHP) : NinjaUtil.getMoneys((long)global::Char.myPetz().cHP)), 90, num2, guistyle2);
				num2 += 10;
				g.drawString("KI: " + (global::Char.myPetz().cMP >= 100000000L ? NinjaUtil.formatShortNumber(global::Char.myPetz().cMP) : NinjaUtil.getMoneys((long)global::Char.myPetz().cMP)), 90, num2, guistyle2);
				num2 += 10;
				g.drawString("SD: " + (global::Char.myPetz().cDamFull >= 100000000L ? NinjaUtil.formatShortNumber(global::Char.myPetz().cDamFull) : NinjaUtil.getMoneys((long)global::Char.myPetz().cDamFull)), 90, num2, guistyle2);
			}
		}
		private static void paintModInfo(mGraphics g)
        {
			int num = 100;
			mFont mfont = mFont.tahoma_7b_yellowSmall2;
			int anchor = mFont.LEFT;
            if (SkillFunctions.isAutoSendAttack)
            {
				mfont.drawString(g, "Tự Đánh: Bật", 7 , num, anchor);
				num += 10;
            }
            if (Char.isPetHandler)
            {
				mfont.drawString(g, "Auto Up Đệ: Bật" ,7 , num, anchor);
				num += 10;
            }
            if (AutoPickMobHandler.IsTanSat)
            {
				mfont.drawString(g, "Tàn Sát: Bật", 7, num, anchor);
				num += 10;
            }
            if (MainFunctions.isSanBoss)
            {
				mfont.drawString(g, "Thông Báo Boss: Bật", 7, num, anchor);
				num += 10;
			}
            if (CharFunctions.getInstance().AutoRivive())
            {
				mfont.drawString(g, "Auto Hồi Sinh: Bật", 7, num, anchor);
				num += 10;
			}
            if (CharFunctions.isAutoLogin)
            {
				mfont.drawString(g, "Auto Login: Bật", 7, num, anchor);
				num += 10;
			}
            if (AutoPickFunctions.isAutoPick)
            {
				mfont.drawString(g, "Auto Nhặt: Bật", 7, num, anchor);
				num += 10;
			}
            if (GraphicsHandler.xoamap)
            {
				mfont.drawString(g, "Xóa Map: Bật", 7, num, anchor);
				num += 10;
			}
            if (GraphicsHandler.listchar)
            {
				mfont.drawString(g, "D.Sách Nhân Vật: Bật", 7, num, anchor);
				num += 10;
			}
            if (GraphicsHandler.enablePaintColor_Wallpaper)
            {
				mfont.drawString(g, "Mã Màu Nền: " + GraphicsHandler.ColorRGB, 7, num, anchor);
				num += 10;
			}
        }
		private static void paintMobFocusInfo(mGraphics g, Mob mobTemplate)
		{
			mFont.tahoma_7b_red.drawString(g, string.Concat(new string[]
			{
		mobTemplate.getTemplate().name,
		" [",
		(mobTemplate.hp >= 1000000000L ? NinjaUtil.formatShortNumber(mobTemplate.hp) : NinjaUtil.getMoneys(mobTemplate.hp)),
		"/",
		(mobTemplate.maxHp >= 1000000000L ? NinjaUtil.formatShortNumber(mobTemplate.maxHp) : NinjaUtil.getMoneys(mobTemplate.maxHp)),
		"]"
			}), GameCanvas.w / 2, 62, 2);
			int num = 72;
			int num2 = 10;
			if (mobTemplate.sleepEff)
			{
				mFont.tahoma_7b_red.drawString(g, "Bị Thôi Miên", GameCanvas.w / 2, num, 2);
				num += num2;
			}
			if (mobTemplate.blindEff)
			{
				mFont.tahoma_7b_red.drawString(g, "Bị Choáng", GameCanvas.w / 2, num, 2);
				num += num2;
			}
			if (mobTemplate.isFreez)
			{
				mFont.tahoma_7b_red.drawString(g, "Bị TDHS", GameCanvas.w / 2, num, 2);
				num += num2;
			}
			if (mobTemplate.holdEffID != 0)
			{
				mFont.tahoma_7b_red.drawString(g, "Bị Trói", GameCanvas.w / 2, num, 2);
				num += num2;
			}
		}
		private static int index;
		public static string caption = "";
		public static string[] StringArr = new string[]
		{
			"Chữ Nháy",
			"Nhập Nội Dung Xuất Hiện Trên Màn Hình:"
		};
		private static void drawString(mGraphics g)
        {
            if (GameCanvas.panel.isShow || (GameCanvas.panel2 != null && GameCanvas.panel2.isShow))
            {
                return;
            }
            if(GameCanvas.gameTick % 20 == 0)
            {
				index++;
				index = (byte)(index % (caption.Length + 1));
				if(index > caption.Length)
                {
					index = 0;
                }
			}
            mFont.tahoma_7b_white.drawStringBd(g, caption.Substring(0, index), 141, 30, mFont.LEFT, mFont.tahoma_7b_dark);
            //mFont.bigNumber_red.drawString(g, "Cheat: " + Time.timeScale.ToString(), 85, 30, mFont.LEFT);
            //mFont.bigNumber_yellow.drawString(g, "Chữ Nháy", 85, 30, mFont.LEFT);
            mFont.tahoma_7_white.drawStringBd(g, string.Concat(new object[] {
			"| ",
			TileMap.mapName,
			" |",
			" - ",
			"Khu ",
			TileMap.zoneID
			}), 7, 80, mFont.LEFT, mFont.tahoma_7_grey);
			mFont.tahoma_7_white.drawStringBd(g, DateTime.Now.ToString(), 7, 90, mFont.LEFT, mFont.tahoma_7_grey);
		}
		
		private static void paintUpgrade(mGraphics g)
        {
			if (AutoUpgradeFunctions.isDapDo)
	{
		mFont.tahoma_7b_red.drawString(g, (AutoUpgradeFunctions.doDeDap != null) ? AutoUpgradeFunctions.doDeDap.template.name : "Chưa Có", GameCanvas.w / 2, 72, mFont.CENTER);
		mFont.tahoma_7b_red.drawString(g, (AutoUpgradeFunctions.doDeDap != null) ? ("Số Sao : " + AutoUpgradeFunctions.saoHienTai.ToString()) : "Số Sao : -1", GameCanvas.w / 2, 82, mFont.CENTER);
		mFont.tahoma_7b_red.drawString(g, "Số Sao Cần Đập : " + AutoUpgradeFunctions.soSaoCanDap + " Sao", GameCanvas.w / 2, 92, mFont.CENTER);
	}
	if (AutoUpgradeFunctions.isDapDo /*|| AutoUpgradeFunctions.isThuongDeThuong || AutoUpgradeFunctions.isThuongDeVip*/)
	{
		mFont.tahoma_7b_red.drawString(g, "Ngọc Xanh : " + NinjaUtil.getMoneys((long)global::Char.myCharz().luong) + " Ngọc Hồng : " + NinjaUtil.getMoneys((long)global::Char.myCharz().luongKhoa), GameCanvas.w / 2, 102, mFont.CENTER);
		mFont.tahoma_7b_red.drawString(g, string.Concat(new object[]
		{
			"Linh Thạch : ",
			NinjaUtil.getMoneys(global::Char.myCharz().xu),
			" Thỏi Vàng : ",
			AutoUpgradeFunctions.thoiVang()
		}), GameCanvas.w / 2, 112, mFont.CENTER);
	}
        }
		public void onCancelChat()
        {

        }
		public void onChatFromMe(string text, string to)
        {

        }
		private static void paintLineBoss(mGraphics g) {
			if (BossFunctions.LineBoss)
			{
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
					if (@char != null && @char != null && @char.cTypePk == 5)
					{
						if (global::Char.myCharz().charFocus == @char)
						{
							g.setColor(Color.green);
							g.drawLine(global::Char.myCharz().cx - GameScr.cmx, global::Char.myCharz().cy - GameScr.cmy, @char.cx - GameScr.cmx, @char.cy - GameScr.cmy);
						}
						else
						{
							g.setColor(Color.red);
							g.drawLine(global::Char.myCharz().cx - GameScr.cmx, global::Char.myCharz().cy - GameScr.cmy, @char.cx - GameScr.cmx, @char.cy - GameScr.cmy);
						}
					}
				}
			}
		}
		private static void paintListChar(mGraphics g)
        {
			bool flag5 = GraphicsHandler.listchar;
			if (flag5)
			{
				ListCharFunctions.Paint(g);
			}
		}
		private static void paintListBosses(mGraphics a)
		{
			if (MainFunctions.isSanBoss)
			{
				int num = 37;
				for (int i = 0; i < MainFunctions.listBosses.Count; i++)
				{
					
					MainFunctions.listBosses[i].paint(a, GameCanvas.w - 2, num, mFont.RIGHT);
					num += 10;
				}
			}
		}
	}
}
