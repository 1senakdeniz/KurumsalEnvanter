using System.ComponentModel.DataAnnotations;

namespace KurumsalEnvanterMVC.Models.Entities
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-Posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departman alanı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Departman adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Departman")]
        public string Department { get; set; } = string.Empty;

        // İlişkisel özellikler
        public ICollection<Asset>? Assets { get; set; }
    }
}