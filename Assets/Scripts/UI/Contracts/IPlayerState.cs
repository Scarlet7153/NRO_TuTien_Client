namespace NRO.UI.Contracts
{
    /// <summary>
    /// Hop dong giao dien trang thai nhan vat (D1.2).
    /// Su dung kieu long cho HP, MP, Power tuong thich 100% voi Char.cs (Char.cHP, cMP, cPower).
    /// Chi anh xa cac truong thuc te co san trong Char.cs, khong phat sinh field tu tien moi o Sprint 2.
    /// </summary>
    public interface IPlayerState
    {
        int CharId { get; }
        string Name { get; }
        sbyte Gender { get; }
        long Hp { get; }
        long MaxHp { get; }
        long Mp { get; }
        long MaxMp { get; }
        long Power { get; }
    }
}
