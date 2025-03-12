using System.ComponentModel.DataAnnotations;

namespace ASMHT_PH53971.Models
{
    public class ChiTietGioHang
    {



        [Key]
        public int MaChiTietGioHang { get; set; }

        public int? MaGioHang { get; set; }

        public int? MaMonAn { get; set; }

        public int? MaCombo { get; set; }

        public int SoLuong { get; set; }

        public double Gia { get; set; }

        public string Loai { get; set; } = null!;

        public virtual Combo? MaComboNavigation { get; set; }

        public virtual GioHang? MaGioHangNavigation { get; set; }

        public virtual MonAn? MaMonAnNavigation { get; set; }
    }

}
