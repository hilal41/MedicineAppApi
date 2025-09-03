using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.Models
{
    public class StockMovement
    {
        public int Id { get; set; }
        
        [Required]
        public int MedicineId { get; set; }
        
        [Required]
        public int ChangeQty { get; set; } // Positive for additions, negative for reductions
        
        [Required]
        [StringLength(100)]
        public string Reason { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string ReferenceType { get; set; } = string.Empty; // "INVOICE", "PURCHASE", "ADJUSTMENT", "TRANSFER", etc.
        
        public int? ReferenceId { get; set; } // ID of the related entity (invoice, purchase order, etc.)
        
        [Required]
        public int CreatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Medicine Medicine { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
    }
}
