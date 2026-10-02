using System;

namespace Assets.src.g
{
    public class RegisterScreen : mScreen, IActionListener
    {
        public TField tfUser;   // Họ và tên
        public TField tfSodt;   // Tài khoản (số ĐT)
        public TField tfPass2;  // Nhập lại mật khẩu

        public static bool isContinueToLogin = false;

        private int focus; // 0 = tfUser, 1 = tfSodt
        private Command cmdOK;
        private Command cmdFogetPass;
        private Command cmdMenu;
        private Command cmdBackFromRegister;
        private Command cmdRes;

        public static string serverName;
        public static Image imgTitle;

        private int xLog, yLog;

        public RegisterScreen(sbyte haveName)
        {
            TileMap.bgID = (sbyte)(mSystem.currentTimeMillis() % 9);
            if (TileMap.bgID == 5 || TileMap.bgID == 6) TileMap.bgID = 4;

            GameScr.loadCamera(true, -1, -1);
            GameScr.cmx = 100;
            GameScr.cmy = 200;

            // --- Tạo 2 ô input: tfSodt (Tài khoản), tfUser (Họ và tên) ---
            tfSodt = new TField();
            tfSodt.setIputType(TField.INPUT_TYPE_NUMERIC);
            tfSodt.width = 220;
            tfSodt.height = mScreen.ITEM_HEIGHT + 2;
            tfSodt.name = "Tài khoản";
            if (haveName == 1) tfSodt.setText("01234567890");

            tfUser = new TField();
            tfUser.setIputType(TField.INPUT_TYPE_PASSWORD);
            tfUser.width = 220;
            tfUser.height = mScreen.ITEM_HEIGHT + 2;
            tfUser.isFocus = true;
            tfUser.name = "Mật khẩu";

            tfPass2 = new TField();
            tfPass2.setIputType(TField.INPUT_TYPE_PASSWORD);
            tfPass2.width = 220;
            tfPass2.height = mScreen.ITEM_HEIGHT + 2;
            tfPass2.isFocus = false;
            tfPass2.name = "Nhập lại mật khẩu";
            if (haveName == 1) tfUser.setText("Nguyễn Văn A");

            // --- Nút ---
            cmdOK = new Command(mResources.OK, this, 2008, null);
            cmdFogetPass = new Command("Thoát", this, 1003, null);
            cmdMenu = new Command(mResources.MENU, this, 2003, null);
            cmdRes = new Command(mResources.register, this, 2002, null);
            cmdBackFromRegister = new Command(mResources.CANCEL, this, 10021, null);

            // Vị trí mặc định cho soft key
            center = cmdOK;
            left = cmdFogetPass;
        }

        public new void switchToMe()
        {
            Res.outz("Res switch");
            SoundMn.gI().stopAll();
            focus = 0;
            tfUser.isFocus = true;
            tfSodt.isFocus = false;
            if (tfPass2 != null) tfPass2.isFocus = false;

            if (GameCanvas.isTouch)
            {
                // Với touch, không auto focus để nhấn vào field
                tfUser.isFocus = false;
                focus = -1;
            }
            base.switchToMe();
        }

        protected void doMenu()
        {
            MyVector v = new MyVector("vMenu Login");
            v.addElement(new Command(mResources.registerNewAcc, this, 2004, null));
            v.addElement(new Command(mResources.selectServer, this, 1004, null));
            v.addElement(new Command(mResources.forgetPass, this, 1003, null));
            v.addElement(new Command(mResources.website, this, 1005, null));

            int low = Rms.loadRMSInt("lowGraphic");
            if (low == 1) v.addElement(new Command(mResources.increase_vga, this, 10041, null));
            else v.addElement(new Command(mResources.decrease_vga, this, 10042, null));

            v.addElement(new Command(mResources.EXIT, GameCanvas.instance, 8885, null));
            GameCanvas.menu.startAt(v, 0);
        }

        // Đăng ký (nếu bạn vẫn dùng flow này)
        protected void doRegister() => GameCanvas.startOKDlg("Vui lòng nhấn OK để xác nhận.");

        protected void doRegister(string user) { }

        public override void update()
        {
            tfUser.update();
            tfSodt.update();
            if (tfPass2 != null) tfPass2.update();

            // Camera nền như cũ
            GameScr.cmx++;
            if (GameScr.cmx > GameCanvas.w * 3 + 100) GameScr.cmx = 100;

            // Soft key theo ngữ cảnh
            if (GameCanvas.isTouch)
            {
                center = cmdOK;
                left = cmdFogetPass;
            }
            else
            {
                center = cmdOK;
                left = cmdFogetPass;
            }
        }

        public override void keyPress(int keyCode)
        {
            if (tfUser.isFocus) tfUser.keyPressed(keyCode);
            else if (tfSodt.isFocus) tfSodt.keyPressed(keyCode);
            else if (tfPass2 != null && tfPass2.isFocus) tfPass2.keyPressed(keyCode);
            base.keyPress(keyCode);
        }

        public override void updateKey()
        {
            if (isContinueToLogin) return;

            // Gán phím xóa tương ứng
            if (!GameCanvas.isTouch)
            {
                if (tfUser.isFocus) right = tfUser.cmdClear;
                else if (tfSodt.isFocus) right = tfSodt.cmdClear;
                else if (tfPass2 != null && tfPass2.isFocus) right = tfPass2.cmdClear;
            }

            // Điều hướng trái/phải giữa 2 field
            if (GameCanvas.keyPressed[21] || GameCanvas.keyPressed[22])
            {
                focus++;
                if (focus > 2) focus = 0;
                
                tfUser.isFocus = (focus == 0);
                tfSodt.isFocus = (focus == 1);
                if (tfPass2 != null) tfPass2.isFocus = (focus == 2);
                GameCanvas.clearKeyPressed();
            }

            // Chạm chọn field
            if (GameCanvas.isPointerJustRelease)
            {
                if (GameCanvas.isPointerHoldIn(tfUser.x, tfUser.y, tfUser.width, tfUser.height))
                {
                    focus = 0;
                    tfUser.isFocus = true;
                    tfSodt.isFocus = false;
                    if (tfPass2 != null) tfPass2.isFocus = false;
                }
                else if (GameCanvas.isPointerHoldIn(tfSodt.x, tfSodt.y, tfSodt.width, tfSodt.height))
                {
                    focus = 1;
                    tfUser.isFocus = false;
                    tfSodt.isFocus = true;
                    if (tfPass2 != null) tfPass2.isFocus = false;
                }
                else if (tfPass2 != null && GameCanvas.isPointerHoldIn(tfPass2.x, tfPass2.y, tfPass2.width, tfPass2.height))
                {
                    focus = 2;
                    tfUser.isFocus = false;
                    tfSodt.isFocus = false;
                    tfPass2.isFocus = true;
                }
            }

            base.updateKey();
            GameCanvas.clearKeyPressed();
        }

        public override void paint(mGraphics g)
        {
            GameCanvas.paintBGGameScr(g);
            if (ChatPopup.currChatPopup != null || ChatPopup.serverChatPopUp != null) return;

            if (GameCanvas.currentDialog == null)
            {
                // ====== Tham số layout gọn ======
                int popupW = 240;
                int padTop = 35;
                int padBottom = 20;
                int gap = 26;                                // khoảng cách giữa 2 ô
                int fieldH = mScreen.ITEM_HEIGHT + 2;

                // tạm đặt x,y để tính chiều cao
                int x = (GameCanvas.w - popupW) / 2;
                int y = GameCanvas.h / 2 - 60;               // điểm neo tạm, sẽ chuẩn sau

                // ====== Tính vị trí 2 ô ======
                tfSodt.x = x + 10;
                tfSodt.y = y + padTop;

                tfUser.x = tfSodt.x;
                tfUser.y = tfSodt.y + gap;
                
                if (tfPass2 != null) {
                    tfPass2.x = tfSodt.x;
                    tfPass2.y = tfUser.y + gap;
                }

                int contentBottom = (tfPass2 != null) ? (tfPass2.y + fieldH) : (tfUser.y + fieldH);
                int popupH = (contentBottom - y) + padBottom;

                PopUp.paintPopUp(g, x, y, popupW, popupH, -1, true);

                mFont.tahoma_7b_dark.drawString(g, "Đăng ký", x + popupW / 2, y + 10, mFont.CENTER);

                // vẽ 2 ô
                tfSodt.paint(g);
                tfUser.paint(g);
                if (tfPass2 != null) tfPass2.paint(g);

                // (tùy chọn) đưa version ra ngoài khung để không phải tăng chiều cao
                string ver = GameMidlet.VERSION;
                g.setColor(GameCanvas.skyColor);
                g.fillRect(GameCanvas.w - 40, 4, 36, 11);
                mFont.tahoma_7_grey.drawString(g, ver, GameCanvas.w - 22, 4, mFont.CENTER);
            }

            base.paint(g);
        }


        public void perform(int idAction, object p)
        {
            switch (idAction)
            {
                case 1000:
                    try { GameMidlet.instance.platformRequest((string)p); } catch { }
                    GameCanvas.endDlg();
                    break;

                case 1003: // Thoát
                    //Session_ME.gI().close();
                    GameCanvas.serverScreen.switchToMe();
                    break;

                case 1004: // Chọn server
                    ServerListScreen.doUpdateServer();
                    GameCanvas.serverScreen.switchToMe();
                    break;

                case 10021: // Back từ register
                    actRegisterLeft();
                    break;

                case 1005: // Website
                    try { GameMidlet.instance.platformRequest("http://abc.com"); } catch { }
                    break;

                case 2002: // Register note
                    doRegister();
                    break;

                case 2003: // Menu
                    doMenu();
                    break;

                case 2004: // Act register (nếu vẫn cần)
                    actRegister();
                    break;

                case 2008: // OK
                    if (tfSodt.getText().Equals(string.Empty) || tfUser.getText().Equals(string.Empty) || (tfPass2 != null && tfPass2.getText().Equals(string.Empty)))
                    {
                        GameCanvas.startOKDlg("Vui lòng điền đầy đủ thông tin");
                        break;
                    }
                    if (tfPass2 != null && !tfUser.getText().Equals(tfPass2.getText())) 
                    {
                        GameCanvas.startOKDlg("Mật khẩu nhập lại không khớp!");
                        break;
                    }

                    GameCanvas.startOKDlg(mResources.PLEASEWAIT);

                    // Gọi charInfo với các trường còn lại rỗng để giữ nguyên chữ ký hàm
                    // Nếu server bạn có API khác gọn hơn, thay thế đoạn này.
                    Service.gI().charInfo(
                        "",   // Ngày
                        "",   // Tháng
                        "",   // Năm
                        "",   // Địa chỉ
                        "",   // CMND/Passport
                        "",   // Ngày cấp
                        "",   // Nơi cấp
                        tfSodt.getText(), // Số ĐT / tài khoản
                        tfUser.getText()  // Họ và tên
                    );
                    break;
            }
        }

        public void actRegisterLeft()
        {
            // Quay lại login hoặc menu đơn giản
            tfUser.isFocus = true;
            tfSodt.isFocus = false;
            if (tfPass2 != null) tfPass2.isFocus = false;
            left = cmdMenu;
        }

        public void actRegister()
        {
            GameCanvas.endDlg();
            GameCanvas.startOKDlg(mResources.regNote);
            tfUser.isFocus = true;
        }
    }
}
