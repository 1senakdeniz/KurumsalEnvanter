using System.ComponentModel.DataAnnotations;

namespace KurumsalEnvanterMVC.Models.Entities
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
        [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
        [Display(Name = "Kategori Adı")]
        public string CategoryName { get; set; } = string.Empty;

        // İlişkisel özellikler
        public ICollection<Asset>? Assets { get; set; }
    }
}