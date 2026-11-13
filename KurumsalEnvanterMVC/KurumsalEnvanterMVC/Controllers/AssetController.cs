using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KurumsalEnvanterMVC.Data;
using KurumsalEnvanterMVC.Models.Entities;

namespace KurumsalEnvanterMVC.Controllers
{
    public class AssetController : Controller
    {
        private readonly AppDbContext _context;

        public AssetController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Liste Sayfası (Index)
        public async Task<IActionResult> Index(string searchString, int? categoryId)
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            ViewBag.Categories = categories;

            var assetsQuery = _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Employee)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                assetsQuery = assetsQuery.Where(a => a.AssetName.Contains(searchString)
                                                  || a.SerialNumber.Contains(searchString));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                assetsQuery = assetsQuery.Where(a => a.CategoryID == categoryId.Value);
            }

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryId;

            var filteredAssets = await assetsQuery.ToListAsync();
            return View(filteredAssets);
        }

        // 2. Yeni Ekleme Sayfası (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            var employees = await _context.Employees.AsNoTracking().ToListAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName");
            ViewBag.Employees = new SelectList(employees, "EmployeeID", "FullName");

            return View();
        }

        // 3. Yeni Ekleme Kayıt (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asset asset)
        {
            if (ModelState.IsValid)
            {
                if (asset.AssignedEmployeeID.HasValue && asset.AssignedEmployeeID.Value > 0)
                {
                    asset.Status = "Zimmetlendi";
                }
                else
                {
                    asset.Status = "Stokta";
                    asset.AssignedEmployeeID = null;
                }

                await _context.Assets.AddAsync(asset);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Yeni demirbaş kaydı başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            var employees = await _context.Employees.AsNoTracking().ToListAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName");
            ViewBag.Employees = new SelectList(employees, "EmployeeID", "FullName");

            return View(asset);
        }

        // 4. Düzenleme Sayfası (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();

            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            var employees = await _context.Employees.AsNoTracking().ToListAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName", asset.CategoryID);
            ViewBag.Employees = new SelectList(employees, "EmployeeID", "FullName", asset.AssignedEmployeeID);

            return View(asset);
        }

        // 5. Düzenleme Kayıt (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Asset asset)
        {
            if (ModelState.IsValid)
            {
                if (asset.AssignedEmployeeID.HasValue && asset.AssignedEmployeeID.Value > 0)
                {
                    asset.Status = "Zimmetlendi";
                }
                else
                {
                    asset.Status = "Stokta";
                    asset.AssignedEmployeeID = null;
                }

                _context.Assets.Update(asset);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Demirbaş ve zimmet bilgileri başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            var employees = await _context.Employees.AsNoTracking().ToListAsync();

            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName", asset.CategoryID);
            ViewBag.Employees = new SelectList(employees, "EmployeeID", "FullName", asset.AssignedEmployeeID);

            return View(asset);
        }

        // 6. Silme (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Demirbaş kaydı başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // 7. CSV / Excel Raporu İndirme
        [HttpGet]
        public async Task<IActionResult> ExportToCsv()
        {
            var assets = await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Employee)
                .AsNoTracking()
                .ToListAsync();

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Seri No;Demirbaş Adı;Kategori;Satın Alma Tarihi;Durum;Zimmetli Personel");

            foreach (var item in assets)
            {
                var categoryName = item.Category != null ? item.Category.CategoryName : "Belirtilmemiş";
                var employeeName = item.Employee != null ? item.Employee.FullName : "Stokta";

                builder.AppendLine($"{item.SerialNumber};{item.AssetName};{categoryName};{item.PurchaseDate:dd.MM.yyyy};{item.Status};{employeeName}");
            }

            var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(builder.ToString())).ToArray();

            return this.File(bytes, "text/csv", $"Demirbas_Raporu_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}