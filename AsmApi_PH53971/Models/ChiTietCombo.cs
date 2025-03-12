using System.ComponentModel.DataAnnotations;

namespace AsmApi_PH53971.Models
{
    public class ChiTietCombo
    {
        [Key]
        public int MaCombo { get; set; }

        public int MaMonAn { get; set; }

        public int SoLuong { get; set; }

        public virtual Combo MaComboNavigation { get; set; } = null!;

        public virtual MonAn MaMonAnNavigation { get; set; } = null!;
    }
}
