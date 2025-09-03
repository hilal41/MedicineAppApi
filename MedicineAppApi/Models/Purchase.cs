using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        
        [Required]
        public int SupplierId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string InvoiceNo { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Supplier Supplier { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
