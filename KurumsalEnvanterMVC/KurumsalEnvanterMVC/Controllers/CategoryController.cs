using Microsoft.AspNetCore.Mvc;
using KurumsalEnvanterMVC.Data.Repositories;
using KurumsalEnvanterMVC.Models.Entities;

namespace KurumsalEnvanterMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        // Doğru Yapıcı Metot (Constructor) Tanımlaması
        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Kategorileri listeleyeceğimiz ana sayfa aksiyonu
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // 1. Yeni Kategori Ekleme Sayfasını Açan Metot (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 2. Formdan Gelen Veriyi SQL'e Kaydeden Metot (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category); // Jenerik Repository ile ekleme
                await _categoryRepository.SaveChangesAsync(); // SQL'e kaydetme

                // 24. GÜN: Global Bildirim Mesajı
                TempData["SuccessMessage"] = "Yeni kategori başarıyla eklendi.";

                return RedirectToAction(nameof(Index));      // Başarılıysa listeye geri dön
            }
            return View(category);
        }

        // 1. Düzenleme Sayfasını Açan Metot (GET) - Seçilen Kategoriyi ID ile Bulur
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // 2. Formdan Gelen Güncel Veriyi Kaydeden Metot (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(category); // Jenerik Repository ile güncelleme
                await _categoryRepository.SaveChangesAsync();

                // 24. GÜN: Global Bildirim Mesajı
                TempData["SuccessMessage"] = "Kategori bilgileri başarıyla güncellendi.";

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 3. Silme İşlemini Gerçekleştiren Aksiyon (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                _categoryRepository.Delete(category); // Jenerik Repository ile silme
                await _categoryRepository.SaveChangesAsync();

                // 24. GÜN: Global Bildirim Mesajı
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}