using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        [Required]
        public int InvoiceId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Method { get; set; } = string.Empty; // cash, card, credit, etc.
        
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int ReceivedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Invoice Invoice { get; set; } = null!;
        public virtual User ReceivedByUser { get; set; } = null!;
    }
}
