using System;
using UnityEngine;
using System.Threading;
namespace Functions.AutoFunctions
{
    public class AutoChatFunctions : IActionListener, IChatable
    {
        private static bool isChat;
        private static bool isGlobalChat;
        private static string textChat, textChatGlobal;
        private static long currChatGlobal;
        private static long currChatDefault;
        private static string[] ChatArray = new string[]
        {
        "Auto Chat Thường:",
        "Nhập Nội Dung"
        };
        private static string[] ChatGlobalArray = new string[] {
    "Auto Chat Thế Giới:",
    "Nhập Nội Dung"
    };
        private static AutoChatFunctions _Instance = new AutoChatFunctions();
        public static AutoChatFunctions gI()
        {
            if (_Instance == null)
            {
                _Instance = new AutoChatFunctions();
            }
            return _Instance;
        }
        public void perform(int idAction, object p)
        {
            switch (idAction)
            {
                case 1:
                    isChat = !isChat;
                    GameScr.info1.addInfo( isChat ? "|5|Auto Chat Với Nội Dung:\n" + textChat:string.Empty, 0);
                    break;
                case 2:
                    isGlobalChat = !isGlobalChat;
                    GameScr.info1.addInfo( isGlobalChat ? "|5|Auto Chat Thế Giới Với Nội Dung:\n"  + textChatGlobal : string.Empty, 0);
                    break;
                case 3:
                    ChatTextField.gI().strChat = ChatArray[0];
                    ChatTextField.gI().tfChat.name = ChatArray[1];
                    ChatTextField.gI().startChat2(gI(), string.Empty);
                    break;
                case 4:
                    ChatTextField.gI().strChat = ChatGlobalArray[0];
                    ChatTextField.gI().tfChat.name = ChatGlobalArray[1];
                    ChatTextField.gI().startChat2(gI(), string.Empty);
                    break;
            }
        }
        public static void ResetTF()
        {
            ChatTextField.gI().strChat = "Chat";
            ChatTextField.gI().tfChat.name = "chat";
            ChatTextField.gI().isShow = false;
        }
        public static void Update()
        { 
        //    if (mSystem.currentTimeMillis() - currChatGlobal >= 2000L && isGlobalChat)
        //    {
        //        AutoChatGlobal();
        //        currChatGlobal = mSystem.currentTimeMillis();
        //    }
        //    if (mSystem.currentTimeMillis() - currChatDefault >= 2000L && isChat)
        //    {
        //        AutoChatDefault();
        //        currChatDefault = mSystem.currentTimeMillis();
        //    }
          
        }
        public static void AutoChatDefault()
        {
            if (isChat && mSystem.currentTimeMillis() - currChatDefault >= 3000L)
            {
                currChatDefault = mSystem.currentTimeMillis();
                if (!string.IsNullOrEmpty(textChat))
                {
                    Service.gI().chat(textChat);
                    return;
                }
                GameScr.info1.addInfo("Chưa cài nội dung auto", 0);
            }
        }
        public static void AutoChatGlobal()
        {
            if (isGlobalChat && mSystem.currentTimeMillis() - currChatGlobal >= 5000L)
            {
                currChatGlobal = mSystem.currentTimeMillis();
                if (!string.IsNullOrEmpty(textChatGlobal))
                {
                    Service.gI().chatGlobal(textChatGlobal);
                    return;
                }
                GameScr.info1.addInfo("Chưa cài nội dung auto chat thế giới", 0);
            }
        }
        public static void ShowMenu()
        {
            MyVector myVector = new MyVector();
            myVector.addElement(new Command("Auto Chat:\n" + (isChat ? "[ON]" : "[OFF]"), gI(), 1, null));
            myVector.addElement(new Command("Auto Chat Thế Giới:\n" + (isGlobalChat ? "[ON]" : "[OFF]"), gI(), 2, null));
            myVector.addElement(new Command("Nội Dung Chat", gI(), 3, null));
            myVector.addElement(new Command("Nội Dung\nCTG", gI(), 4, null));
            GameCanvas.menu.startAt(myVector, 3);
        }
        public void onCancelChat() { }
        public void onChatFromMe(string text, string to)
        {

            bool flag2 = ChatTextField.gI().strChat.Equals(ChatArray[0]);
            if (flag2)
            {
                textChat = ChatTextField.gI().tfChat.getText();
                ResetTF();
            }
            bool flag3 = ChatTextField.gI().strChat.Equals(ChatGlobalArray[0]);
            if (flag3)
            {
                textChatGlobal = ChatTextField.gI().tfChat.getText();
                ResetTF();
            }

            ResetTF();
        }
    }
}