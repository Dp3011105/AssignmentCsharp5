using ASMHT_PH53971.Context;
using ASMHT_PH53971.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASMHT_PH53971.Controllers
{
    public class GioHangController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GioHangController(MyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddMonAn(int maMonAn, int soLuong)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaNguoiDung == user.Id);

            if (gioHang == null)
            {
                gioHang = new GioHang { MaNguoiDung = user.Id };
                _context.GioHangs.Add(gioHang);
                await _context.SaveChangesAsync();
            }

            var chiTietGioHang = await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.MaGioHang == gioHang.MaGioHang && c.MaMonAn == maMonAn);

            if (chiTietGioHang == null)
            {
                var monAn = await _context.MonAns.FirstOrDefaultAsync(m => m.MaMonAn == maMonAn);
                if (monAn == null) return NotFound();

                chiTietGioHang = new ChiTietGioHang
                {
                    MaGioHang = gioHang.MaGioHang,
                    MaMonAn = maMonAn,
                    SoLuong = soLuong,
                    Gia = monAn.Gia,
                    Loai = "MonAn"
                };
                _context.ChiTietGioHangs.Add(chiTietGioHang);
            }
            else
            {
                chiTietGioHang.SoLuong += soLuong;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> AddCombo(int maCombo, int soLuong)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaNguoiDung == user.Id);

            if (gioHang == null)
            {
                gioHang = new GioHang { MaNguoiDung = user.Id };
                _context.GioHangs.Add(gioHang);
                await _context.SaveChangesAsync();
            }

            var chiTietGioHang = await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.MaGioHang == gioHang.MaGioHang && c.MaCombo == maCombo);

            if (chiTietGioHang == null)
            {
                var combo = await _context.Combos.FirstOrDefaultAsync(c => c.MaCombo == maCombo);
                if (combo == null) return NotFound();

                chiTietGioHang = new ChiTietGioHang
                {
                    MaGioHang = gioHang.MaGioHang,
                    MaCombo = maCombo,
                    SoLuong = soLuong,
                    Gia = combo.Gia,
                    Loai = "Combo"
                };
                _context.ChiTietGioHangs.Add(chiTietGioHang);
            }
            else
            {
                chiTietGioHang.SoLuong += soLuong;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(ct => ct.MaMonAnNavigation)
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(ct => ct.MaComboNavigation)
                .FirstOrDefaultAsync(g => g.MaNguoiDung == user.Id);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    MaNguoiDung = user.Id,
                    ChiTietGioHangs = new List<ChiTietGioHang>()
                };
            }

            return View(gioHang);
        }





        [HttpPost]
        public IActionResult RemoveFromCart(int chiTietGioHangId)
        {
            var chiTietGioHang = _context.ChiTietGioHangs.FirstOrDefault(ct => ct.MaChiTietGioHang == chiTietGioHangId);

            if (chiTietGioHang != null)
            {
                _context.ChiTietGioHangs.Remove(chiTietGioHang);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int chiTietGioHangId, int newQuantity)
        {
            var chiTietGioHang = _context.ChiTietGioHangs.FirstOrDefault(ct => ct.MaChiTietGioHang == chiTietGioHangId);

            if (chiTietGioHang != null)
            {
                chiTietGioHang.SoLuong = newQuantity;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để thanh toán.";
                return RedirectToAction("Login", "Account");
            }

            var gioHang = await _context.GioHangs
                .Include(gh => gh.ChiTietGioHangs)
                    .ThenInclude(ct => ct.MaMonAnNavigation)
                .Include(gh => gh.ChiTietGioHangs)
                    .ThenInclude(ct => ct.MaComboNavigation)
                .FirstOrDefaultAsync(gh => gh.MaNguoiDung == user.Id);

            if (gioHang == null || !gioHang.ChiTietGioHangs.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index");
            }

            foreach (var chiTietGioHang in gioHang.ChiTietGioHangs)
            {
                if (chiTietGioHang.MaMonAn != null) 
                {
                    var monAn = await _context.MonAns.FindAsync(chiTietGioHang.MaMonAn);
                    if (monAn == null || monAn.Soluong < chiTietGioHang.SoLuong)
                    {
                        TempData["Message"] = $"Món ăn '{monAn?.TenMonAn ?? "Không xác định"}' không đủ hàng.";
                        return RedirectToAction("Index");
                    }
                }
                else if (chiTietGioHang.MaCombo != null)
                {
                    var combo = await _context.Combos.FindAsync(chiTietGioHang.MaCombo);
                    if (combo == null)
                    {
                        TempData["Message"] = "Combo không hợp lệ.";
                        return RedirectToAction("Index");
                    }
                }
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var donHang = new DonHang
                    {
                        MaNguoiDung = user.Id,
                        ThoiGianDat = DateTime.Now,
                        TrangThai = "Chua_Van_Chuyen",
                        TongTien = gioHang.ChiTietGioHangs.Sum(ct =>
                            ct.MaCombo != null ? ct.SoLuong * (ct.MaComboNavigation?.Gia ?? 0) :
                            ct.MaMonAn != null ? ct.SoLuong * (ct.MaMonAnNavigation?.Gia ?? 0) : 0
                        )

                    };
                    await _context.DonHangs.AddAsync(donHang);
                    await _context.SaveChangesAsync();

                    foreach (var chiTietGioHang in gioHang.ChiTietGioHangs)
                    {
                        var chiTietDonHang = new ChiTietDonHang
                        {
                            MaDonHang = donHang.MaDonHang,
                            MaMonAn = chiTietGioHang.MaMonAn,
                            MaCombo = chiTietGioHang.MaCombo, 
                            SoLuong = chiTietGioHang.SoLuong,
                            Gia = chiTietGioHang.Gia
                        };
                        await _context.ChiTietDonHangs.AddAsync(chiTietDonHang);

                        if (chiTietGioHang.MaMonAn != null)
                        {
                            var monAn = await _context.MonAns.FindAsync(chiTietGioHang.MaMonAn);
                            if (monAn != null)
                            {
                                monAn.Soluong -= chiTietGioHang.SoLuong;
                            }
                        }

                        if (chiTietGioHang.MaCombo != null)
                        {
                            var combo = await _context.Combos.FindAsync(chiTietGioHang.MaCombo);
                            if (combo != null)
                            {
                                if (chiTietGioHang.MaCombo != null)
                                {
                                    var combos = await _context.Combos.FindAsync(chiTietGioHang.MaCombo);
                                    if (combo != null && combo.SoLuong >= chiTietGioHang.SoLuong)
                                    {
                                        combo.SoLuong -= chiTietGioHang.SoLuong; 
                                    }
                                    else
                                    {
                                        TempData["Message"] = $"Combo '{combo?.TenCombo ?? "Không xác định"}' không đủ hàng.";
                                        return RedirectToAction("Index");
                                    }
                                }

                            }
                        }
                    }

                    _context.ChiTietGioHangs.RemoveRange(gioHang.ChiTietGioHangs);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["Message"] = "Thanh toán thành công!";
                    return RedirectToAction("DonHangCuaToi", "DonHang");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Message"] = "Có lỗi xảy ra khi thanh toán. Vui lòng thử lại.";
                    Console.WriteLine("Lỗi: " + ex.Message);
                    return RedirectToAction("Index");
                }
            }
        }











    }
}
