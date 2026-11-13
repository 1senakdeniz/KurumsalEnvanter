using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KurumsalEnvanterMVC.Data;
using KurumsalEnvanterMVC.Data.Repositories;
using KurumsalEnvanterMVC.Models.Entities;

namespace KurumsalEnvanterMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly AppDbContext _context;

        // Bağımlılık enjeksiyonu ile Hem Repository hem DbContext çağrılıyor
        public EmployeeController(IRepository<Employee> employeeRepository, AppDbContext context)
        {
            _employeeRepository = employeeRepository;
            _context = context;
        }

        // 22. GÜN GÜNCELLEMESİ: Arama Destekli Personel Listesi
        public async Task<IActionResult> Index(string searchString)
        {
            var employeesQuery = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                employeesQuery = employeesQuery.Where(e => e.FullName.Contains(searchString)
                                                        || e.Email.Contains(searchString)
                                                        || e.Department.Contains(searchString));
            }

            ViewBag.CurrentSearch = searchString;
            return View(await employeesQuery.ToListAsync());
        }

        // 22. GÜN: Personelin Üzerindeki Zimmetli Cihazları Gösteren Detay Metodu
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Assets!)
                .ThenInclude(a => a.Category)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // 1. Yeni Personel Ekleme Sayfasını Açan Metot (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 2. Formdan Gelen Personel Verisini SQL'e Kaydeden Metot (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _employeeRepository.AddAsync(employee); // Jenerik Repository ile ekleme
                await _employeeRepository.SaveChangesAsync(); // Değişiklikleri SQL'e kaydetme

                // 24. GÜN: Global Bildirim Mesajı
                TempData["SuccessMessage"] = "Yeni personel kaydı başarıyla oluşturuldu.";

                return RedirectToAction(nameof(Index));       // Başarılıysa listeye geri dön
            }
            return View(employee);
        }

        // 1. Düzenleme Sayfasını Açan Metot (GET) - Seçilen Personeli ID ile Bulur
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // 2. Formdan Gelen Güncel Personel Verisini Kaydeden Metot (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _employeeRepository.Update(employee); // Jenerik Repository ile güncelleme
                await _employeeRepository.SaveChangesAsync();

                // 24. GÜN: Global Bildirim Mesajı
                TempData["SuccessMessage"] = "Personel bilgileri başarıyla güncellendi.";

                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // 3. Personel Silme İşlemini Gerçekleştiren Aksiyon (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee != null)
            {
                _employeeRepository.Delete(employee); // Jenerik Repository ile silme
                await _employeeRepository.SaveChangesAsync();

                // 24. GÜN: Global Bildirim Mesajı
                TempData["SuccessMessage"] = "Personel kaydı başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}