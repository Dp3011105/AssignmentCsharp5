using System.ComponentModel.DataAnnotations;

namespace AsmApi_PH53971.Models
{
    public class Combo
    {
        [Key]
        public int MaCombo { get; set; }
        public string TenCombo { get; set; }
        public string MoTa { get; set; }
        public double Gia { get; set; }
        public bool TrangThai { get; set; }
        public string DuongDanHinh { get; set; }


        public virtual ICollection<ChiTietCombo> ChiTietCombos { get; set; } = new List<ChiTietCombo>();

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
    }
}
