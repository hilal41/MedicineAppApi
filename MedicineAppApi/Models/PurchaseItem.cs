using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        
        [Required]
        public int PurchaseId { get; set; }
        
        [Required]
        public int MedicineId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Qty { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Subtotal { get; set; }
        
        // Navigation properties
        public virtual Purchase Purchase { get; set; } = null!;
        public virtual Medicine Medicine { get; set; } = null!;
    }
}
