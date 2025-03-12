using System.ComponentModel.DataAnnotations;

namespace AsmApi_PH53971.Models
{
    public class ChiTietDonHang
    {

        [Key]
        public int MaChiTietDonHang { get; set; }

        public int SoLuong { get; set; }

        public double Gia { get; set; }

        public int? MaDonHang { get; set; }

        public int? MaMonAn { get; set; }

        public int? MaCombo { get; set; }

        public virtual Combo? MaComboNavigation { get; set; }

        public virtual DonHang? MaDonHangNavigation { get; set; }

        public virtual MonAn? MaMonAnNavigation { get; set; }

    }
}
