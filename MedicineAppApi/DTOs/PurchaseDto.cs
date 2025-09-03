using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.DTOs
{
    public class PurchaseDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<PurchaseItemDto> Items { get; set; } = new List<PurchaseItemDto>();
    }

    public class CreatePurchaseDto
    {
        [Required]
        public int SupplierId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string InvoiceNo { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [Required]
        public List<CreatePurchaseItemDto> Items { get; set; } = new List<CreatePurchaseItemDto>();
    }

    public class UpdatePurchaseDto
    {
        [StringLength(50)]
        public string? InvoiceNo { get; set; }
        
        public DateTime? Date { get; set; }
    }
}
