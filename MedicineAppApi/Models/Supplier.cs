using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Contact { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<SupplierMedicine> SupplierMedicines { get; set; } = new List<SupplierMedicine>();
    }
}
