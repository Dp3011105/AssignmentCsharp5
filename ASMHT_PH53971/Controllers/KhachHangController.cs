using ASMHT_PH53971.Context;
using ASMHT_PH53971.Models.ViewModels;
using ASMHT_PH53971.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASMHT_PH53971.Controllers
{
    public class KhachHangController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _context;

        public KhachHangController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, MyDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public IActionResult Index(string searchName, string selectedTag)
        {
            var query = _context.MonAns.AsQueryable();


            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(m => m.TenMonAn.Contains(searchName));
            }


            if (!string.IsNullOrEmpty(selectedTag))
            {
                query = query.Where(m => m.MaTag.ToString() == selectedTag);
            }

            query = query.Where(m => m.TrangThai == true);

            var result = query.ToList();

            ViewBag.Tags = _context.Tags.ToList();

            return View(result);
        }


        public IActionResult Details(int id)
        {
            var product = _context.MonAns.FirstOrDefault(m => m.MaMonAn == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }





        public IActionResult IndexComBo(string searchName, string selectedTag)
        {
            var query = _context.Combos.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(m => m.TenCombo.Contains(searchName));
            }


            query = query.Where(m => m.TrangThai == true);

            var result = query.ToList();

            return View(result);
        }




        public IActionResult DetailsComBo(int id)
        {

            var combo = _context.Combos
                .Include(c => c.ChiTietCombos)
                .ThenInclude(ct => ct.MaMonAnNavigation)
                .FirstOrDefault(c => c.MaCombo == id);

            if (combo == null)
            {
                return NotFound();
            }


            var viewModel = new ComboDetailsViewModel
            {
                Combo = combo,
                MonAnsInCombo = combo.ChiTietCombos.ToList()
            };

            return View(viewModel);
        }




        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditNguoiDung()
        {
            // Lấy thông tin người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            // Kiểm tra xem người dùng có mật khẩu hay không
            var hasPassword = await _userManager.HasPasswordAsync(user);

            var model = new UserEditViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Gender = user.Gender,
                Nationality = user.Nationality,
                Email = user.Email,
                HasPassword = hasPassword

            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNguoiDung(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.DateOfBirth = model.DateOfBirth;
            user.Address = model.Address;
            user.Gender = model.Gender;
            user.Nationality = model.Nationality;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            if (model.HasPassword &&
                !string.IsNullOrEmpty(model.OldPassword) &&
                !string.IsNullOrEmpty(model.NewPassword) &&
                !string.IsNullOrEmpty(model.ConfirmPassword))
            {
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                    return View(model);
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError("OldPassword", "Mật khẩu cũ không chính xác.");
                    return View(model);
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            return RedirectToAction("EditNguoiDung");
        }


    }
}
