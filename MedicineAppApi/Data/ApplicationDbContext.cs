using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Models;

namespace MedicineAppApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<SupplierMedicine> SupplierMedicines { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                
                // Make email unique
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            // Configure Supplier entity
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Contact).HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Configure Medicine entity
            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Batch).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Barcode).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
                
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Medicines)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure SupplierMedicine entity
            modelBuilder.Entity<SupplierMedicine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DefaultPurchasePrice).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.SupplierMedicines)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Medicine)
                    .WithMany(m => m.SupplierMedicines)
                    .HasForeignKey(e => e.MedicineId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Ensure unique combination of supplier and medicine
                entity.HasIndex(e => new { e.SupplierId, e.MedicineId }).IsUnique();
            });

            // Configure Invoice entity
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Make invoice number unique
                entity.HasIndex(e => e.InvoiceNo).IsUnique();
                
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.Invoices)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Invoices)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure InvoiceItem entity
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Qty).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Invoice)
                    .WithMany(i => i.InvoiceItems)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Medicine)
                    .WithMany(m => m.InvoiceItems)
                    .HasForeignKey(e => e.MedicineId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Method).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                entity.HasOne(e => e.Invoice)
                    .WithMany(i => i.Payments)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.ReceivedByUser)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(e => e.ReceivedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Index for better performance on queries
                entity.HasIndex(e => new { e.InvoiceId, e.Date });
                entity.HasIndex(e => new { e.Method, e.Date });
            });

            // Configure StockMovement entity
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChangeQty).IsRequired();
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ReferenceType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                
                entity.HasOne(e => e.Medicine)
                    .WithMany(m => m.StockMovements)
                    .HasForeignKey(e => e.MedicineId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.StockMovements)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Index for better performance on queries
                entity.HasIndex(e => new { e.MedicineId, e.CreatedAt });
                entity.HasIndex(e => new { e.ReferenceType, e.ReferenceId });
            });

            // Configure Purchase entity
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Make purchase invoice number unique
                entity.HasIndex(e => e.InvoiceNo).IsUnique();
                
                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.Purchases)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.Purchases)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PurchaseItem entity
            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Qty).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Purchase)
                    .WithMany(p => p.PurchaseItems)
                    .HasForeignKey(e => e.PurchaseId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Medicine)
                    .WithMany(m => m.PurchaseItems)
                    .HasForeignKey(e => e.MedicineId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
