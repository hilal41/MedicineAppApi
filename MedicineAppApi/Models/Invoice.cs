using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string InvoiceNo { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Subtotal { get; set; }
        
        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
