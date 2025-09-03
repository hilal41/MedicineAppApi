using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public bool IsFullyPaid { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }

    public class CreateInvoiceDto
    {
        [Required]
        [StringLength(50)]
        public string InvoiceNo { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int CustomerId { get; set; }
        
        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public List<CreateInvoiceItemDto> Items { get; set; } = new List<CreateInvoiceItemDto>();
    }

    public class UpdateInvoiceDto
    {
        public int? CustomerId { get; set; }
        
        [Range(0, 100)]
        public decimal? DiscountPercent { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? DiscountAmount { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
