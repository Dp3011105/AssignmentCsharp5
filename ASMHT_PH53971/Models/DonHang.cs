using System.ComponentModel.DataAnnotations;

namespace ASMHT_PH53971.Models
{
    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }

        public DateTime ThoiGianDat { get; set; }

        public string? TrangThai { get; set; }

        public double TongTien { get; set; }

        public string? MaNguoiDung { get; set; }

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

        public virtual ApplicationUser? MaNguoiDungNavigation { get; set; }


    }
}
