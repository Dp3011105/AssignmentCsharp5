using ASMHT_PH53971.Context;
using ASMHT_PH53971.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ASMHT_PH53971.Controllers
{
    public class DonHangController : Controller
    {


        private readonly MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DonHangController(MyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }




       




        [HttpPost]
        public IActionResult CapNhatTrangThai(int MaDonHang, string TrangThai)
        {
            try
            {

                var donHang = _context.DonHangs
                    .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaMonAnNavigation)
                    .FirstOrDefault(dh => dh.MaDonHang == MaDonHang);

                if (donHang == null)
                {

                    return Json(new { success = false, message = "Đơn hàng không tồn tại." });
                }


                if (donHang.TrangThai == "Huy_Don" || donHang.TrangThai == "Hoan_Thanh")
                {
                    return Json(new { success = false, message = "Đơn hàng đã bị hủy hoặc đã hoàn thành, không thể thay đổi." });
                }


                if (TrangThai == "Huy_Don")
                {
                    donHang.TrangThai = "Huy_Don";

                    foreach (var chiTiet in donHang.ChiTietDonHangs)
                    {
                        if (chiTiet.MaMonAnNavigation != null)
                        {
                            chiTiet.MaMonAnNavigation.Soluong += chiTiet.SoLuong;
                        }
                    }

                    _context.SaveChanges();
                    return Json(new { success = true, message = "Đơn hàng đã được hủy thành công." });
                }
                if (TrangThai == "Da_Van_Chuyen" || TrangThai == "Hoan_Thanh")
                {
                    donHang.TrangThai = TrangThai;
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Trạng thái đã được cập nhật." });
                }
                else
                {
                    return Json(new { success = false, message = "Trạng thái không hợp lệ." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra: " + ex.Message);
                return Json(new { success = false, message = "Đã có lỗi xảy ra: " + ex.Message });
            }

        }







        public async Task<IActionResult> DonHangCuaToi()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để xem đơn hàng của bạn.";
                return RedirectToAction("Login", "Account");
            }

            var donHangs = _context.DonHangs
                .Where(dh => dh.MaNguoiDung == user.Id) 
                .OrderByDescending(dh => dh.ThoiGianDat)
                .ToList();

            return View(donHangs);
        }




        public IActionResult ChiTietDonHang(int id)
        {
            var donHang = _context.DonHangs
     .Include(dh => dh.ChiTietDonHangs)
         .ThenInclude(ct => ct.MaMonAnNavigation)
     .Include(dh => dh.ChiTietDonHangs)
         .ThenInclude(ct => ct.MaComboNavigation) // 🔥 
     .FirstOrDefault(dh => dh.MaDonHang == id);


            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
    }
}
