using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KurumsalEnvanterMVC.Data;

namespace KurumsalEnvanterMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        // Ýstatistikleri SQL Server'dan LINQ sorgularýyla çekmek için veri tabaný enjeksiyonu yapýyoruz
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // LINQ Count metotlarý ile anlýk sayýsal verileri alýyoruz
            ViewBag.TotalAssets = await _context.Assets.CountAsync();
            ViewBag.InStockAssets = await _context.Assets.CountAsync(a => a.Status == "Stokta");
            ViewBag.AssignedAssets = await _context.Assets.CountAsync(a => a.Status == "Zimmetlendi");
            ViewBag.TotalEmployees = await _context.Employees.CountAsync();

            return View();
        }

        // 26. GÜN GÜNCELLEMESÝ: Status Code Destekli Hata Yakalama Metodu
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404)
                {
                    ViewBag.ErrorMessage = "Aradýðýnýz sayfa veya kayýt bulunamadý (404).";
                }
                else if (statusCode.Value == 500)
                {
                    ViewBag.ErrorMessage = "Sunucu tarafýnda beklenmeyen bir hata oluþtu (500).";
                }
                else
                {
                    ViewBag.ErrorMessage = $"Bir hata oluþtu. Hata Kodu: {statusCode.Value}";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Sistemde beklenmeyen bir hata meydana geldi.";
            }

            return View(); // ASP.NET Core otomatik olarak Views/Shared/Error.cshtml dosyasýný tetikler
        }
    }
}