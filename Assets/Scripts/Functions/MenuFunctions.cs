using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Functions.AutoFunctions;
using Functions.HandlerFunctions;

namespace Functions
{
    internal class MenuFunctions : IActionListener, IChatable
    {
        public static bool isPetSPInfo;
        public static long lastTimeGetPetSPInfo;
        public static bool isShowPanelPet;
        public static MenuFunctions _Instance;
        //<summary>Variable<>
        public static MenuFunctions getInstance()
        {
            bool flag = MenuFunctions._Instance == null;
            if (flag)
            {
                MenuFunctions._Instance = new MenuFunctions();
            }
            return MenuFunctions._Instance;
        }
        private static void UseItem(int templateId)
        {
            for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
            {
                Item item = global::Char.myCharz().arrItemBag[i];
                bool flag = item != null && (int)item.template.id == templateId;
                if (flag)
                {
                    Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
                    break;
                }
            }
        }
        public static void UseBongTai()
        {
            UseItem(921);
            UseItem(454);
            UseItem(1129);
        }
        public static void UseCapsule()
        {
            UseItem(194);
            UseItem(193);
        }
        public void perform(int idAction, object p)
        {
            switch (idAction)
            {
                case 1:
                    Service.gI().openUIZone();
                    break;
                case 2:
                    UseItem(454);
                    UseItem(921);
                    ShowMenu();
                    break;
                case 3:
                    SkillFunctions.ShowMenu();
                    break;
                case 4:
                    PeanFunctions.ShowMenu();
                    break;
                case 5:
                    ChatTextField.gI().strChat = MainFunctions.inputSpeed[0];
                    ChatTextField.gI().tfChat.name = MainFunctions.inputSpeed[1];
                    ChatTextField.gI().startChat2(getInstance(), string.Empty);
                    break;
                case 6:
                    ShowMenuTanSat();
                    break;
                case 7:
                    AutoPickMobHandler.IsTanSat = !AutoPickMobHandler.IsTanSat;
                    GameScr.info1.addInfo("|5|Tự Động Luyện Tập:\n" + (AutoPickMobHandler.IsTanSat ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 8:
                    AutoPickMobHandler.IsNeSieuQuai = !AutoPickMobHandler.IsNeSieuQuai;
                    GameScr.info1.addInfo("|5|Né Siêu Quái: " + (AutoPickMobHandler.IsNeSieuQuai ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 9:
                    SkillFunctions.dichChuyenPem = !SkillFunctions.dichChuyenPem;
                    GameScr.info1.addInfo("|5|Dịch Chuyển Đến Quái:\n" + (SkillFunctions.dichChuyenPem ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 10:
                    AutoPickMobHandler.IsVuotDiaHinh = !AutoPickMobHandler.IsVuotDiaHinh;
                    GameScr.info1.addInfo("|5|Vượt Địa Hình: " + (AutoPickMobHandler.IsVuotDiaHinh ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 11:
                    ShowMenuBoss();
                    break;
                case 12:
                    MainFunctions.isSanBoss = !MainFunctions.isSanBoss;
                    GameScr.info1.addInfo("|5|Thông Báo Boss: " + (MainFunctions.isSanBoss ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 13:
                    BossFunctions.LineBoss = !BossFunctions.LineBoss;
                    GameScr.info1.addInfo("|5|Đường Kẻ Boss: " + (BossFunctions.LineBoss ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 14:
                    ShowMenuMore();
                    break;
                case 15:
                    ChatTextField.gI().strChat = MainFunctions.inputFPS[0];
                    ChatTextField.gI().tfChat.name = MainFunctions.inputFPS[1];
                    ChatTextField.gI().startChat2(getInstance(), string.Empty);
                    break;
                case 16:
                    CharFunctions.getInstance().Handler();
                    GameScr.info1.addInfo("|5|Auto Hồi Sinh: " + (CharFunctions.getInstance().AutoRivive() ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 17:
                    //MapObject.MainObject.ShowXmapMenu();
                    AutoChatFunctions.ShowMenu();
                    break;
                case 18:
                    isPetSPInfo = !isPetSPInfo;
                    GameScr.info1.addInfo("|5|Thông Tin Sư Phụ, Đệ Tử: " + (isPetSPInfo ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 25:
                    CharFunctions.isAutoLogin = !CharFunctions.isAutoLogin;
                    GameScr.info1.addInfo("|5|Auto Login: " + (CharFunctions.isAutoLogin ? "ON" : "OFF"), 0);
                    ShowMenu();
                    break;
                case 26:
                    AutoPickFunctions.isAutoPick = !AutoPickFunctions.isAutoPick;
                    GameScr.info1.addInfo("|5|Auto Nhặt: " + (AutoPickFunctions.isAutoPick ? "OFF" : "ON"), 0);
                    ShowMenu();
                    break;
                case 27:
                    ChatTextField.gI().strChat = ScreenFunctions.StringArr[0];
                    ChatTextField.gI().tfChat.name = ScreenFunctions.StringArr[1];
                    ChatTextField.gI().startChat2(getInstance(), string.Empty);
                    break;
                case 28:
                    BossFunctions.focusBoss = !BossFunctions.focusBoss;
                    GameScr.info1.addInfo("Focus Boss:\n"+(BossFunctions.focusBoss ? "ON":"OFF"),0);
                    ShowMenu();
                    break;
                default:
                    break;
            }
        }


        private static void myVectorMenu4()
        {
            MyVector myVector = new MyVector();
            myVector.addElement(new Command("FPS\n[" + Application.targetFrameRate + "]", getInstance(), 15, null));
            //myVector.addElement(new Command("Chữ Nháy", getInstance(), 27, null));
            myVector.addElement(new Command("Thông tin\nSư phụ\nĐệ Tử\n" + (isPetSPInfo ? "[ON]" : "[OFF]"), getInstance(), 18, null));
        
            myVector.addElement(new Command("Auto\nHồi Sinh\n" + (CharFunctions.getInstance().AutoRivive() ? "[ON]" : "[OFF]"), getInstance(), 16, null));
            myVector.addElement(new Command("Auto Login\n" + (CharFunctions.isAutoLogin ? "[ON]" : "[OFF]"), getInstance(), 25, null));
            myVector.addElement(new Command("Auto Nhặt\n" + ((!AutoPickFunctions.isAutoPick || AutoPickFunctions.pickByList != 0) ? "[OFF]" : "[ON]"), MenuFunctions.getInstance(), 26, null));
            GameCanvas.menu.startAt(myVector, 3);
        }
        private static void myVectorMenu()
        {
            MyVector myVector = new MyVector();
            //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            //myVector.addElement(new Command("Đổi Khu", getInstance(), 1, null));
            //myVector.addElement(new Command("Bông Tai", getInstance(), 2, null));
            //}
            myVector.addElement(new Command("Cheat\n["+Time.timeScale+"]", getInstance(), 5, null));
            //myVector.addElement(new Command("Xmap", getInstance(), 17, null));
            myVector.addElement(new Command("Auto Chat", getInstance(), 17, null));
            myVector.addElement(new Command("Auto Skill", getInstance(), 3, null));
            myVector.addElement(new Command("Auto Đậu", getInstance(), 4, null));
            myVector.addElement(new Command("Tàn Sát", getInstance(), 6, null));
            myVector.addElement(new Command("Boss", getInstance(), 11, null));
            myVector.addElement(new Command("Khác", getInstance(), 14, null));
            GameCanvas.menu.startAt(myVector, 3);
        }
        private static void myVectorMenu3()
        {
            MyVector myVector = new MyVector();
            myVector.addElement(new Command("Thông Báo Boss\n"+(MainFunctions.isSanBoss ? "[ON]":"[OFF]"), getInstance(), 12, null));
            myVector.addElement(new Command("Đường Kẻ Boss\n" + (BossFunctions.LineBoss ? "[ON]" : "[OFF]"), getInstance(), 13, null));
            myVector.addElement(new Command("Focus Boss\n" + (BossFunctions.focusBoss ? "[ON]" : "[OFF]"), getInstance(), 28, null));
            GameCanvas.menu.startAt(myVector, 3);
        }
        private static void myVectorMenu2()
        {
            MyVector myVector = new MyVector();
            myVector.addElement(new Command("TDLT", getInstance(), 7, null));
            myVector.addElement(new Command("Né Siêu Quái\n" + (AutoPickMobHandler.IsNeSieuQuai ? "[ON]" : "[OFF]"), getInstance(), 8, null));
            myVector.addElement(new Command("Dịch Chuyển Đến Quái\n"+(SkillFunctions.dichChuyenPem ? "[ON]":"[OFF]"), getInstance(), 9, null));
            myVector.addElement(new Command("Vượt Địa Hình\n" + (AutoPickMobHandler.IsVuotDiaHinh ? "[ON]" : "[OFF]"), getInstance(), 10, null));
            GameCanvas.menu.startAt(myVector, 3);
        }
        public static void ShowMenuTanSat() => myVectorMenu2();
        public static void ShowMenu() => myVectorMenu();
        public static void ShowMenuBoss() => myVectorMenu3();
        public static void ShowMenuMore() => myVectorMenu4();
        public void onChatFromMe(string text, string to)
        {

            if (ChatTextField.gI().strChat.Equals(ScreenFunctions.StringArr[0]))
            {

                try
                {
                    string str = ChatTextField.gI().tfChat.getText();
                    ScreenFunctions.caption = str;
                    GameScr.info1.addInfo("|5|In Lên Màn\n: " + str, 0);
                    MainFunctions.ResetTF();
                }
                catch (Exception e)
                {
                    //GameScr.info1.addInfo(e.ToString(), 0);
                    GameScr.info1.addInfo("Nội dung nhập không chính xác", 0);
                    MainFunctions.ResetTF();
                }
           
                return;
            }
            if (ChatTextField.gI().strChat.Equals(MainFunctions.inputSpeed[0]))
            {
              
                try
                {
                    int num4 = int.Parse(ChatTextField.gI().tfChat.getText());
                    num4 = Math.min(20, num4);
                    num4 = Math.max(1, num4);
                    Time.timeScale = num4;
                    GameScr.info1.addInfo("|5|Cheat: " + Time.timeScale, 0);
                    MainFunctions.ResetTF();
                }
                catch (Exception e)
                {
                    GameScr.info1.addInfo("Chỉ được phép nhập số", 0);
                    MainFunctions.ResetTF();

                }

                return;
            }
            if (ChatTextField.gI().strChat.Equals(MainFunctions.inputFPS[0]))
            {

                try
                {
                    int num4 = int.Parse(ChatTextField.gI().tfChat.getText());
                    num4 = Math.max(35, num4);
                    num4 = Math.min(120, num4);
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = num4;
                    GameScr.info1.addInfo("|7|FPS Hiện Tại: " + Application.targetFrameRate, 0);
                    MainFunctions.ResetTF();
                }
                catch (Exception e)
                {
                    GameScr.info1.addInfo("Chỉ được phép nhập số", 0);
                    MainFunctions.ResetTF();

                }
            }
            MainFunctions.ResetTF();
        }
        ///
        static MenuFunctions()
        {
         
        }
        ///
        /// <summary>
   
        /// </summary>
        public void onCancelChat()
        {
          
        }
        

    }
}
