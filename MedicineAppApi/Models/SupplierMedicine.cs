using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicineAppApi.Models
{
    public class SupplierMedicine
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int SupplierId { get; set; }
        
        [Required]
        public int MedicineId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal DefaultPurchasePrice { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int LeadTimeDays { get; set; }
        
        // Navigation properties
        public virtual Supplier Supplier { get; set; } = null!;
        public virtual Medicine Medicine { get; set; } = null!;
    }
}
