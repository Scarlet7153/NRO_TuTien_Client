namespace NRO.UI.Contracts
{
    /// <summary>
    /// Hop dong giao dien ban dia hoa & truy xuat chuoi van ban (D1.6).
    /// Ho tro tra cuu chuoi theo khoa va dinh dang co tham so.
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        /// Lay chuoi van ban tuong ung voi khoa truyen vao.
        /// Neu khong tim thay trong tu dien, fallback ve gia tri mac dinh hoac chinh key.
        /// </summary>
        string Get(string key);

        /// <summary>
        /// Dinh dang chuoi co tham so mot cach an toan (string.Format).
        /// </summary>
        string Format(string key, params object[] args);
    }
}
