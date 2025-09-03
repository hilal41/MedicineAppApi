using System.ComponentModel.DataAnnotations;

namespace MedicineAppApi.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
    }

    public class UpdateCustomerDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
    }
}
