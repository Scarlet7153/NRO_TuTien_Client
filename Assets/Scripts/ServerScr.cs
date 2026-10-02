public class ServerScr : mScreen, IActionListener
{
    public ServerScr()
    {
        TileMap.bgID = (byte)(mSystem.currentTimeMillis() % 9);
        if (TileMap.bgID == 5 || TileMap.bgID == 6) TileMap.bgID = 4;
        GameScr.loadCamera(true, -1, -1);
        GameScr.cmx = 100;
        GameScr.cmy = 200;
    }

    public override void switchToMe()
    {
        // KHÔNG hiển thị gì, tự vào server luôn
        SoundMn.gI().stopAll();
        base.switchToMe();

        // Chọn index mặc định (0) hoặc dùng giá trị đã lưu ở ipSelect
        int idx = ServerListScreen.ipSelect;
        if (idx < 0 || idx >= ServerListScreen.nameServer.Length) idx = 0;

        Session_ME.gI().clearSendingMessage();
        ServerListScreen.ipSelect = idx;

        // Áp dụng server và nhảy tới màn kế tiếp
        GameCanvas.serverScreen.selectServer();
        GameCanvas.serverScreen.switchToMe();
        // nếu selectServer() đã tự chuyển qua Login thì dòng switchToMe() có thể bỏ đi
    }

    // Không cần các hàm còn lại
    public override void update() { }
    public override void paint(mGraphics g) { }
    public override void updateKey() { }
    public void perform(int idAction, object p) { }
}
