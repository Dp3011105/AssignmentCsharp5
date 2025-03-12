using ASMHT_PH53971.Context;
using ASMHT_PH53971.Authorization;
using ASMHT_PH53971.Models.ViewModels;
using ASMHT_PH53971.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;


namespace ASMHT_PH53971.Controllers
{

    [AuthorizeAdmin]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _context;

        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7225/api/MonAns";


        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, MyDbContext context , IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }


        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

       
        public async Task<IActionResult> ListNguoiDung()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }








        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EditNguoiDung(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Gender = user.Gender,
                Nationality = user.Nationality,
                Email = user.Email,
                AvailableRoles = roles.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name,
                    Selected = userRoles.Contains(role.Name) 
                }).ToList(),
                SelectedRoles = userRoles.ToList() 
            };

            return View(model);
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNguoiDung(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
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

            user.Role = model.SelectedRoles?.FirstOrDefault();

            await _userManager.UpdateAsync(user); 

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (model.SelectedRoles != null && model.SelectedRoles.Any())
            {
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);
            }

            return RedirectToAction("ListNguoiDung", "Admin");
        }


        public IActionResult ListComBo()
        {
           
            var comboList = _context.Combos
                                    .Include(c => c.ChiTietCombos)  
                                    .ThenInclude(ct => ct.MaMonAnNavigation) 
                                    .ToList();

            return View(comboList);
        }




        public IActionResult CreateCombo()
        {
            ViewBag.MonAnList = _context.MonAns.ToList();
            return View(new Combo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCombo(Combo combo, List<int> selectedMonAnIds, List<string> selectedSoLuongs)
        {
            if (ModelState.IsValid)
            {
                _context.Combos.Add(combo);
                _context.SaveChanges();

                for (int i = 0; i < selectedMonAnIds.Count; i++)
                {
                    int soLuong = int.Parse(selectedSoLuongs[i]);

                    if (soLuong > 0)
                    {
                        var chiTiet = new ChiTietCombo
                        {
                            MaCombo = combo.MaCombo,
                            MaMonAn = selectedMonAnIds[i],
                            SoLuong = soLuong
                        };
                        _context.ChiTietCombos.Add(chiTiet);
                    }
                }

                _context.SaveChanges();
                return RedirectToAction("ListComBo", "Admin");
            }

            ViewBag.MonAnList = _context.MonAns.Where(m => m.TrangThai == true).ToList();
            return View(combo);
        }



        public IActionResult EditCombo(int id)
        {
            var combo = _context.Combos.Include(c => c.ChiTietCombos)
                                       .ThenInclude(ct => ct.MaMonAnNavigation)
                                       .FirstOrDefault(c => c.MaCombo == id);
            if (combo == null) return NotFound();

            ViewBag.MonAnList = _context.MonAns.ToList();
            return View(combo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCombo(int id, Combo updatedCombo, List<int> selectedMonAnIds, List<string> selectedSoLuongs)
        {
            if (id != updatedCombo.MaCombo) return NotFound();

            if (ModelState.IsValid)
            {
                var combo = _context.Combos.Include(c => c.ChiTietCombos).FirstOrDefault(c => c.MaCombo == id);
                if (combo == null) return NotFound();

                combo.TenCombo = updatedCombo.TenCombo;
                combo.MoTa = updatedCombo.MoTa;
                combo.Gia = updatedCombo.Gia;
                combo.TrangThai = updatedCombo.TrangThai;
                combo.DuongDanHinh = updatedCombo.DuongDanHinh;

                _context.ChiTietCombos.RemoveRange(combo.ChiTietCombos);

                for (int i = 0; i < selectedMonAnIds.Count; i++)
                {
                    int soLuong = int.Parse(selectedSoLuongs[i]);

                    if (soLuong > 0)
                    {
                        var chiTiet = new ChiTietCombo
                        {
                            MaCombo = id,
                            MaMonAn = selectedMonAnIds[i],
                            SoLuong = soLuong
                        };
                        _context.ChiTietCombos.Add(chiTiet);
                    }
                }

                _context.SaveChanges();
                return RedirectToAction("ListComBo", "Admin");
            }

            ViewBag.MonAnList = _context.MonAns.ToList();
            return View(updatedCombo);
        }




        public IActionResult ListDonHang()
        {
            var donHangs = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaMonAnNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaComboNavigation)
                .ToList();

            return View(donHangs);
        }









        public IActionResult IndexTag()
        {
            var tags = _context.Tags.ToList();
            return View(tags);
        }

        [HttpGet]
        public IActionResult CreateTag()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTag(Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Tags.Add(tag);
                _context.SaveChanges();
                return RedirectToAction(nameof(IndexTag)); 
            }
            return View(tag);
        }


        [HttpGet]
        public IActionResult EditTag(int id)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.MaTag == id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTag(Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Update(tag);
                _context.SaveChanges();
                return RedirectToAction(nameof(IndexTag));
            }
            return View(tag);
        }






        public async Task<IActionResult> ListMonAn()
        {
            var response = await _httpClient.GetAsync("https://localhost:7225/api/MonAns");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var monAnList = await response.Content.ReadFromJsonAsync<List<MonAn>>();
            return View(monAnList);
        }

        public IActionResult CreateMonAn()
        {
            ViewBag.Tags = _context.Tags.ToList();
            return View(new MonAn());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonAn monAn)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7225/api/MonAns", monAn);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListMonAn", "Admin");
                }
            }
            ViewBag.Tags = _context.Tags.ToList();
            return View(monAn);
        }

        public async Task<IActionResult> EditMonAn(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7225/api/MonAns/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var monAn = await response.Content.ReadFromJsonAsync<MonAn>();
            ViewBag.Tags = _context.Tags.ToList();
            return View(monAn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMonAn(int id, MonAn monAn)
        {
            if (id != monAn.MaMonAn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7225/api/MonAns/{id}", monAn);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(ListMonAn));
                }
            }
            ViewBag.Tags = _context.Tags.ToList();
            return View(monAn);
        }


    }

}
