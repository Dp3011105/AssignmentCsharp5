namespace ASMHT_PH53971.Models
{
    public class Tag
    {
        public int MaTag { get; set; }

        public string TenTag { get; set; } = null!;

        public bool TrangThai { get; set; }

        public virtual ICollection<MonAn> MonAns { get; set; } = new List<MonAn>();
    }
}
