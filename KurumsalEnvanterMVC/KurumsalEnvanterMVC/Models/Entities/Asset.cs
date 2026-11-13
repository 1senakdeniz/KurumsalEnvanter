using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KurumsalEnvanterMVC.Models.Entities
{
    public class Asset
    {
        [Key]
        public int AssetID { get; set; }

        [Required]
        public string AssetName { get; set; } = string.Empty;

        [Required]
        public string SerialNumber { get; set; } = string.Empty;

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Stokta";

        [Required]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        public int? AssignedEmployeeID { get; set; }

        [ForeignKey("AssignedEmployeeID")]
        public Employee? Employee { get; set; }
    }
}