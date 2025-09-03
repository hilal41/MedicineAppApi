using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.DTOs
{
    public class InvoiceItemDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CreateInvoiceItemDto
    {
        [Required]
        public int MedicineId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Qty { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }

    public class UpdateInvoiceItemDto
    {
        [Range(1, int.MaxValue)]
        public int? Qty { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }
    }
}
