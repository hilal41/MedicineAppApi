using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.DTOs
{
    public class StockMovementDto
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public int ChangeQty { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ReferenceType { get; set; } = string.Empty;
        public int? ReferenceId { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int BalanceAfterMovement { get; set; } // Calculated field
    }

    public class CreateStockMovementDto
    {
        [Required]
        public int MedicineId { get; set; }
        
        [Required]
        public int ChangeQty { get; set; } // Positive for additions, negative for reductions
        
        [Required]
        [StringLength(100)]
        public string Reason { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string ReferenceType { get; set; } = string.Empty;
        
        public int? ReferenceId { get; set; }
    }

    public class UpdateStockMovementDto
    {
        [StringLength(100)]
        public string? Reason { get; set; }
    }

    public class StockMovementSummaryDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public DateTime LastMovementDate { get; set; }
    }
}
