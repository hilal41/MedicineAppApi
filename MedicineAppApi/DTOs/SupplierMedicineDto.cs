using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.DTOs
{
    public class SupplierMedicineDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public decimal DefaultPurchasePrice { get; set; }
        public int LeadTimeDays { get; set; }
    }

    public class CreateSupplierMedicineDto
    {
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
    }

    public class UpdateSupplierMedicineDto
    {
        [Range(0, double.MaxValue)]
        public decimal? DefaultPurchasePrice { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? LeadTimeDays { get; set; }
    }
}
