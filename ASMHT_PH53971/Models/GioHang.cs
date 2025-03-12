using System.ComponentModel.DataAnnotations;

namespace ASMHT_PH53971.Models
{
    public class GioHang
    {
        [Key]
        public int MaGioHang { get; set; }

        public string? MaNguoiDung { get; set; }

        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

        public virtual ApplicationUser? MaNguoiDungNavigation { get; set; }

    }
}
