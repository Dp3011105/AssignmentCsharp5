using ASMHT_PH53971.Context;
using ASMHT_PH53971.Models;
using ASMHT_PH53971.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ASMHT_PH53971.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;

        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
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




        public IActionResult ComboList(string searchName)
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





        public IActionResult ComboDetails(int id)
        {
            var combo = _context.Combos
                .Where(c => c.MaCombo == id)
                .FirstOrDefault();

            if (combo == null)
            {
                return NotFound();
            }

            var monAnsInCombo = _context.ChiTietCombos
                .Where(c => c.MaCombo == id)
                .Include(c => c.MaMonAnNavigation)
                .ToList();

            var model = new ComboDetailsViewModel
            {
                Combo = combo,
                MonAnsInCombo = monAnsInCombo
            };

            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
