using System.Linq.Expressions;

namespace KurumsalEnvanterMVC.Data.Repositories
{
    // Hangi tablo gelirse gelsin (T) ortak CRUD metotlarını tanımlıyoruz
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(); // Tüm listeyi getirir
        Task<T?> GetByIdAsync(int id);      // ID'ye göre tek bir kayıt getirir
        Task AddAsync(T entity);            // Yeni kayıt ekler
        void Update(T entity);              // Kayıt günceller
        void Delete(T entity);              // Kayıt siler
        Task<bool> SaveChangesAsync();      // Değişiklikleri SQL'e kaydeder
    }
}