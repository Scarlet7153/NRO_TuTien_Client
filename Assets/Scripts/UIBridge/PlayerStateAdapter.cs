using NRO.UI.Contracts;

namespace NRO.UIBridge
{
    /// <summary>
    /// Cau noi (Adapter) tu Char.myCharz() sang hop dong IPlayerState (D1.2).
    /// Nam trong Assembly-CSharp goc, tham chieu hop le vao NRO.UI.Contracts ma khong tao circular dependency.
    /// Su dung kieu long cho HP, MP, Power tuong thich 100% voi Char.cHP, cMP, cPower.
    /// </summary>
    public class PlayerStateAdapter : IPlayerState
    {
        private static PlayerStateAdapter _instance;
        public static PlayerStateAdapter Instance => _instance ?? (_instance = new PlayerStateAdapter());

        public int CharId
        {
            get
            {
                var c = Char.myCharz();
                return c != null ? c.charID : 0;
            }
        }

        public string Name
        {
            get
            {
                var c = Char.myCharz();
                return c != null ? (c.cName ?? string.Empty) : string.Empty;
            }
        }

        public sbyte Gender
        {
            get
            {
                var c = Char.myCharz();
                return c != null ? (sbyte)c.cgender : (sbyte)0;
            }
        }

        public long Hp
        {
            get
            {
                var c = Char.myCharz();
                return c != null ? c.cHP : 0L;
            }
        }

        public long MaxHp
        {
            get
            {
                var c = Char.myCharz();
                return c != null ? c.cHPFull : 0L;
            }
        }

        public long Mp
        {
            get
            {
                var c = Char.myCharz();
                return c != null ? c.cMP : 0L;
            }
        }

        public long MaxMp
        {
            get
            {
                var c = Char.myCharz();
                return c != null ? c.cMPFull : 0L;
            }
        }

        public long Power
        {
            get
            {
                var c = Char.myCharz();
                return c != null ? c.cPower : 0L;
            }
        }
    }
}
