using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.DTOs
{
    public class MedicineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Barcode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateMedicineDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Batch { get; set; } = string.Empty;
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public DateTime ExpiryDate { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(50)]
        public string? Barcode { get; set; }
    }

    public class UpdateMedicineDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        
        [StringLength(50)]
        public string? Batch { get; set; }
        
        public int? CategoryId { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }
        
        [StringLength(50)]
        public string? Barcode { get; set; }
    }
}
