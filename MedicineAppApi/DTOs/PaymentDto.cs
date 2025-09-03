using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int ReceivedBy { get; set; }
        public string ReceivedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePaymentDto
    {
        [Required]
        public int InvoiceId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Method { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

    public class UpdatePaymentDto
    {
        [Range(0.01, double.MaxValue)]
        public decimal? Amount { get; set; }
        
        [StringLength(20)]
        public string? Method { get; set; }
        
        public DateTime? Date { get; set; }
    }

    public class PaymentSummaryDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal InvoiceTotal { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public bool IsFullyPaid { get; set; }
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }
}
