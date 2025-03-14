﻿using Azure;
using System.ComponentModel.DataAnnotations;

namespace ASMHT_PH53971.Models
{
    public class MonAn
    {
        [Key]
        public int MaMonAn { get; set; }

        public string TenMonAn { get; set; } = null!;

        public string? MoTa { get; set; }

        public double Gia { get; set; }

        public int? MaTag { get; set; }

        public string? DuongDanHinh { get; set; }

        public int? Soluong { get; set; }

        public bool? TrangThai { get; set; }

        public virtual ICollection<ChiTietCombo> ChiTietCombos { get; set; } = new List<ChiTietCombo>();

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

        public virtual Tag? MaTagNavigation { get; set; }
    }
}
