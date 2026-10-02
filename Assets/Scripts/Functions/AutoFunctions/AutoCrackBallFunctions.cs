using System;
using System.Threading;

namespace Functions.AutoFunctions
{
	// Token: 0x020000C9 RID: 201
	public class AutoCrackBallFunctions : IActionListener
	{
		// Token: 0x06000A97 RID: 2711 RVA: 0x000AAF0C File Offset: 0x000A910C
		public static AutoCrackBallFunctions gI()
		{
			bool flag = AutoCrackBallFunctions.instance == null;
			AutoCrackBallFunctions result;
			if (flag)
			{
				result = (AutoCrackBallFunctions.instance = new AutoCrackBallFunctions());
			}
			else
			{
				result = AutoCrackBallFunctions.instance;
			}
			return result;
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x000AAF40 File Offset: 0x000A9140
		public static void startMenu()
		{
			bool flag = AutoCrackBallFunctions.startmenu;
			if (flag)
			{
				MyVector myVector = new MyVector();
				myVector.addElement(new Command("Mở thường", AutoCrackBallFunctions.gI(), 1, 1));
				myVector.addElement(new Command("Mở đặc\nbiệt", AutoCrackBallFunctions.gI(), 1, 2));
				myVector.addElement(new Command("Nhận đồ.", AutoCrackBallFunctions.gI(), 2, null));
				GameCanvas.menu.startAt(myVector, 3);
			}
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x000068B3 File Offset: 0x00004AB3
		public void StartVongQuay(byte type, int price)
		{
			this.typePrice = type;
			this.Price = price;
			this.isCrackBall = true;
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x000AAFC0 File Offset: 0x000A91C0
		public void update()
		{
			bool flag = AutoCrackBallFunctions.isauto;
			if (flag)
			{
				Service.gI().openMenu(19);
				Thread.Sleep(500);
				for (int i = 0; i < GameCanvas.menu.menuItems.size(); i++)
				{
					bool flag2 = ((Command)GameCanvas.menu.menuItems.elementAt(i)).caption.ToLower().Contains("quay");
					if (flag2)
					{
						Service.gI().confirmMenu(19, (sbyte)i);
					}
				}
				Thread.Sleep(500);
				bool flag3 = !this.isCrackBall;
				if (flag3)
				{
					for (int j = 0; j < GameCanvas.menu.menuItems.size(); j++)
					{
						Service.gI().confirmMenu(19, 0);
					}
				}
				Thread.Sleep(500);
				bool flag4 = !this.isCrackBall;
				if (flag4)
				{
					Service.gI().confirmMenu(19, (sbyte)AutoCrackBallFunctions.type);
				}
				Thread.Sleep(1000);
			}
			while (AutoCrackBallFunctions.isauto)
			{
				try
				{
					this.checkNumTicket();
					int num = this.checkTien();
					bool flag5 = num < this.Price;
					if (flag5)
					{
						AutoCrackBallFunctions.isauto = false;
						GameScr.info1.addInfo("Không đủ tiền.", 0);
						GameScr.gI().switchToMe();
						break;
					}
					bool flag6 = this.Price > 0;
					if (flag6)
					{
						int num2 = num / this.Price;
						num2 += this.numTicket;
						bool flag7 = num2 > 7;
						if (flag7)
						{
							num2 = 7;
						}
						Service.gI().SendCrackBall(2, (byte)num2);
					}
					Thread.Sleep(2000);
					bool flag8 = AutoCrackBallFunctions.khongdu;
					if (flag8)
					{
						AutoCrackBallFunctions.isauto = false;
						AutoCrackBallFunctions.khongdu = false;
						GameScr.gI().switchToMe();
						GameScr.info1.addInfo("Xong.", 0);
						break;
					}
				}
				catch (Exception)
				{
				}
			}
			this.isCrackBall = false;
			GameScr.info1.addInfo("Xong.", 0);
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x000AB1F0 File Offset: 0x000A93F0
		public int checkTien()
		{
			return (int)((this.typePrice == 0) ? global::Char.myCharz().xu : ((long)global::Char.myCharz().checkLuong()));
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x000AB224 File Offset: 0x000A9424
		public static void infoMe(string s)
		{
			bool flag = (s.Contains(mResources.not_enough_money_1) || Char.myCharz().xu < 100000000 || s.ToLower().Contains("còn thiếu")) && !AutoCrackBallFunctions.gI().nhandodangquay;
			if (flag)
			{
				bool flag2 = AutoCrackBallFunctions.isauto;
				if (flag2)
				{
					AutoCrackBallFunctions.khongdu = true;
				}
			}
			else
			{
				bool flag3 = AutoCrackBallFunctions.gI().nhandodangquay;
				if (flag3)
				{
					bool flag4 = GameScr.gI().isBagFull();
					if (flag4)
					{
						AutoCrackBallFunctions.isauto = false;
						GameScr.gI().switchToMe();
						GameScr.info1.addInfo("Rương bạn đã đầy, kết thúc.", 0);
					}
					bool flag5 = GameCanvas.keyAsciiPress == 113;
					if (flag5)
					{
						AutoCrackBallFunctions.isauto = false;
						GameScr.gI().switchToMe();
						GameScr.info1.addInfo("Tắt Auto Quay", 0);
					}
					else
					{
						GameScr.gI().switchToMe();
						AutoCrackBallFunctions.isauto = false;
						AutoCrackBallFunctions.gI().typenhando = -2;
						AutoCrackBallFunctions.gI().ispause = true;
						new Thread(new ThreadStart(AutoCrackBallFunctions.gI().runnhando)).Start();
					}
				}
			}
			bool flag6 = s.ToLower().Contains("rương phụ");
			if (flag6)
			{
				AutoCrackBallFunctions.isauto = false;
				GameScr.gI().switchToMe();
				GameScr.info1.addInfo("Xong.", 0);
			}
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x000AB378 File Offset: 0x000A9578
		public void perform(int idAction, object p)
		{
			bool flag = idAction == 1;
			if (flag)
			{
				AutoCrackBallFunctions.type = (int)p;
				AutoCrackBallFunctions.isauto = true;
				new Thread(new ThreadStart(AutoCrackBallFunctions.gI().update)).Start();
			}
			bool flag2 = idAction == 2;
			if (flag2)
			{
				MyVector myVector = new MyVector();
				myVector.addElement(new Command("Nhận vàng", AutoCrackBallFunctions.gI(), 3, 9));
				myVector.addElement(new Command("Nhận bùa", AutoCrackBallFunctions.gI(), 3, 13));
				myVector.addElement(new Command("Nhận đồ", AutoCrackBallFunctions.gI(), 3, -1));
				myVector.addElement(new Command("Nhận cải trang", AutoCrackBallFunctions.gI(), 3, 5));
				myVector.addElement(new Command("Nhận tất", AutoCrackBallFunctions.gI(), 3, -2));
				GameCanvas.menu.startAt(myVector, 3);
			}
			bool flag3 = idAction == 3;
			if (flag3)
			{
				this.typenhando = (int)p;
				new Thread(new ThreadStart(this.runnhando)).Start();
			}
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x000AB4A0 File Offset: 0x000A96A0
		public void runnhando()
		{
			Service.gI().openMenu(19);
			Thread.Sleep(700);
			bool flag = false;
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < GameCanvas.menu.menuItems.size(); i++)
			{
				Command command = (Command)GameCanvas.menu.menuItems.elementAt(i);
				bool flag2 = command.caption.ToLower().Contains("quay");
				if (flag2)
				{
					num2 = i;
				}
				bool flag3 = command.caption.ToLower().Contains("rương");
				if (flag3)
				{
					num = i;
					break;
				}
			}
			bool flag4 = num != -1;
			if (flag4)
			{
				Service.gI().confirmMenu(19, (sbyte)num);
				flag = true;
			}
			else
			{
				bool flag5 = num2 != -1;
				if (flag5)
				{
					Service.gI().confirmMenu(19, (sbyte)num2);
					this.CloseMenu();
				}
				Thread.Sleep(200);
				for (int j = 0; j < GameCanvas.menu.menuItems.size(); j++)
				{
					bool flag6 = ((Command)GameCanvas.menu.menuItems.elementAt(j)).caption.ToLower().Contains("rương");
					if (flag6)
					{
						Service.gI().confirmMenu(19, (sbyte)j);
						flag = true;
						break;
					}
				}
			}
			this.CloseMenu();
			Thread.Sleep(700);
			bool flag7 = !flag;
			if (flag7)
			{
				GameScr.info1.addInfo("Không có đồ", 0);
			}
			else
			{
				Thread.Sleep(500);
				this.CloseMenu();
				bool flag8 = this.typenhando == -1;
				if (flag8)
				{
					for (int k = 0; k < global::Char.myCharz().arrItemShop[0].Length; k++)
					{
						Item item = global::Char.myCharz().arrItemShop[0][k];
						bool flag9 = GameScr.gI().isBagFull();
						if (flag9)
						{
							break;
						}
						bool flag10 = item != null && (item.template.type == 0 || item.template.type == 1 || item.template.type == 2 || item.template.type == 3 || item.template.type == 4);
						if (flag10)
						{
							Service.gI().buyItem(0, k, 0);
							Thread.Sleep(500);
						}
					}
				}
				else
				{
					bool flag11 = this.typenhando != -2;
					if (flag11)
					{
						int l = 0;
						while (l < global::Char.myCharz().arrItemShop[0].Length)
						{
							Item item2 = global::Char.myCharz().arrItemShop[0][l];
							bool flag12 = GameScr.gI().isBagFull();
							if (flag12)
							{
								break;
							}
							bool flag13 = item2 != null && (int)item2.template.type == this.typenhando;
							if (flag13)
							{
								Service.gI().buyItem(0, l, 0);
								Thread.Sleep(500);
							}
							else
							{
								l++;
							}
						}
					}
					else
					{
						int m = 0;
						while (m < global::Char.myCharz().arrItemShop[0].Length)
						{
							Item item3 = global::Char.myCharz().arrItemShop[0][m];
							bool flag14 = GameScr.gI().isBagFull();
							if (flag14)
							{
								break;
							}
							bool flag15 = item3 != null;
							if (flag15)
							{
								Service.gI().buyItem(0, m, 0);
								Thread.Sleep(500);
							}
							else
							{
								m++;
							}
						}
					}
				}
				bool flag16 = this.nhandodangquay && AutoCrackBallFunctions.isauto;
				if (flag16)
				{
					AutoCrackBallFunctions.isauto = true;
					new Thread(new ThreadStart(AutoCrackBallFunctions.gI().update)).Start();
				}
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x000068CB File Offset: 0x00004ACB
		public void CloseMenu()
		{
			ChatPopup.currChatPopup = null;
			Effect2.vEffect2.removeAllElements();
			Effect2.vEffect2Outside.removeAllElements();
			InfoDlg.hide();
			GameCanvas.menu.doCloseMenu();
			GameCanvas.panel.cp = null;
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x000AB884 File Offset: 0x000A9A84
		private void checkNumTicket()
		{
			for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
			{
				Item item = global::Char.myCharz().arrItemBag[i];
				bool flag = item != null && (item.template.id == 820 || item.template.id == 821);
				if (flag)
				{
					this.numTicket = item.quantity;
					break;
				}
			}
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x000AB8FC File Offset: 0x000A9AFC
		private int checkTickket()
		{
			this.checkNumTicket();
			int num = 8;
			bool flag = num > this.numTicket;
			if (flag)
			{
				num = this.numTicket;
			}
			return num;
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x000AB930 File Offset: 0x000A9B30
		private int checkNum()
		{
			int num = 8;
			num -= this.checkTickket();
			bool flag = num <= 0;
			if (flag)
			{
				num = 0;
			}
			return num;
		}

		// Token: 0x04001371 RID: 4977
		public static bool isauto = false;

		// Token: 0x04001372 RID: 4978
		public static bool khongdu = false;

		// Token: 0x04001373 RID: 4979
		private static int type = 1;

		// Token: 0x04001374 RID: 4980
		public int typenhando;

		// Token: 0x04001375 RID: 4981
		private static AutoCrackBallFunctions instance;

		// Token: 0x04001376 RID: 4982
		private int numTicket;

		// Token: 0x04001377 RID: 4983
		private byte typePrice;

		// Token: 0x04001378 RID: 4984
		private int Price;

		// Token: 0x04001379 RID: 4985
		private bool isCrackBall;

		// Token: 0x0400137A RID: 4986
		public bool nhandodangquay;

		// Token: 0x0400137B RID: 4987
		public bool ispause;

		// Token: 0x0400137C RID: 4988
		public static bool startmenu;
	}
}
