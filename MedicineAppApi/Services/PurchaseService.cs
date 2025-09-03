using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Services
{
    public interface IPurchaseService
    {
        Task<PurchaseDto> CreatePurchaseAsync(CreatePurchaseDto createPurchaseDto, int createdByUserId);
        Task<PurchaseDto?> GetPurchaseByIdAsync(int id);
        Task<PurchaseDto?> GetPurchaseByInvoiceNoAsync(string invoiceNo);
        Task<IEnumerable<PurchaseDto>> GetPurchasesBySupplierAsync(int supplierId);
        Task<IEnumerable<PurchaseDto>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PurchaseDto>> GetPurchasesByCreatedByAsync(int createdBy);
        Task<PurchaseDto> UpdatePurchaseAsync(int id, UpdatePurchaseDto updatePurchaseDto);
        Task<bool> DeletePurchaseAsync(int id);
        Task<decimal> GetTotalPurchasesAsync(DateTime startDate, DateTime endDate);
        Task<int> GetPurchaseCountAsync(DateTime startDate, DateTime endDate);
    }

    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IPurchaseItemRepository _purchaseItemRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly IMapper _mapper;

        public PurchaseService(
            IPurchaseRepository purchaseRepository,
            IPurchaseItemRepository purchaseItemRepository,
            IMedicineRepository medicineRepository,
            IStockMovementRepository stockMovementRepository,
            IMapper mapper)
        {
            _purchaseRepository = purchaseRepository;
            _purchaseItemRepository = purchaseItemRepository;
            _medicineRepository = medicineRepository;
            _stockMovementRepository = stockMovementRepository;
            _mapper = mapper;
        }

        public async Task<PurchaseDto> CreatePurchaseAsync(CreatePurchaseDto createPurchaseDto, int createdByUserId)
        {
            // Check if purchase invoice number already exists
            if (await _purchaseRepository.InvoiceNoExistsAsync(createPurchaseDto.InvoiceNo))
            {
                throw new InvalidOperationException("Purchase invoice number already exists");
            }

            // Validate items and calculate totals
            decimal total = 0;
            foreach (var item in createPurchaseDto.Items)
            {
                var medicine = await _medicineRepository.GetByIdAsync(item.MedicineId);
                if (medicine == null)
                {
                    throw new InvalidOperationException($"Medicine with ID {item.MedicineId} not found");
                }

                item.Price = item.Price; // Use provided price
                total += item.Price * item.Qty;
            }

            // Create purchase
            var purchase = new Purchase
            {
                SupplierId = createPurchaseDto.SupplierId,
                InvoiceNo = createPurchaseDto.InvoiceNo,
                Date = createPurchaseDto.Date,
                Total = total,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _purchaseRepository.AddAsync(purchase);

            // Create purchase items and update medicine quantities
            foreach (var itemDto in createPurchaseDto.Items)
            {
                var medicine = await _medicineRepository.GetByIdAsync(itemDto.MedicineId);
                
                var purchaseItem = new PurchaseItem
                {
                    PurchaseId = purchase.Id,
                    MedicineId = itemDto.MedicineId,
                    Qty = itemDto.Qty,
                    Price = itemDto.Price,
                    Subtotal = itemDto.Price * itemDto.Qty
                };

                await _purchaseItemRepository.AddAsync(purchaseItem);

                // Update medicine quantity
                medicine.Quantity += itemDto.Qty;
                medicine.UpdatedAt = DateTime.UtcNow;
                await _medicineRepository.UpdateAsync(medicine);

                // Create stock movement record
                var stockMovement = new StockMovement
                {
                    MedicineId = itemDto.MedicineId,
                    ChangeQty = itemDto.Qty, // Positive for purchases
                    Reason = $"Purchased via purchase {purchase.InvoiceNo}",
                    ReferenceType = "PURCHASE",
                    ReferenceId = purchase.Id,
                    CreatedBy = createdByUserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _stockMovementRepository.AddAsync(stockMovement);
            }

            // Return the created purchase with items
            var result = await _purchaseRepository.GetByIdAsync(purchase.Id);
            return _mapper.Map<PurchaseDto>(result);
        }

        public async Task<PurchaseDto?> GetPurchaseByIdAsync(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            return purchase != null ? _mapper.Map<PurchaseDto>(purchase) : null;
        }

        public async Task<PurchaseDto?> GetPurchaseByInvoiceNoAsync(string invoiceNo)
        {
            var purchase = await _purchaseRepository.GetByInvoiceNoAsync(invoiceNo);
            return purchase != null ? _mapper.Map<PurchaseDto>(purchase) : null;
        }

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesBySupplierAsync(int supplierId)
        {
            var purchases = await _purchaseRepository.GetBySupplierAsync(supplierId);
            return _mapper.Map<IEnumerable<PurchaseDto>>(purchases);
        }

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var purchases = await _purchaseRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<PurchaseDto>>(purchases);
        }

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesByCreatedByAsync(int createdBy)
        {
            var purchases = await _purchaseRepository.GetByCreatedByAsync(createdBy);
            return _mapper.Map<IEnumerable<PurchaseDto>>(purchases);
        }

        public async Task<PurchaseDto> UpdatePurchaseAsync(int id, UpdatePurchaseDto updatePurchaseDto)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
            {
                throw new InvalidOperationException("Purchase not found");
            }

            // Update purchase properties
            if (!string.IsNullOrEmpty(updatePurchaseDto.InvoiceNo))
            {
                // Check if new invoice number already exists
                if (await _purchaseRepository.InvoiceNoExistsAsync(updatePurchaseDto.InvoiceNo) && 
                    updatePurchaseDto.InvoiceNo != purchase.InvoiceNo)
                {
                    throw new InvalidOperationException("Purchase invoice number already exists");
                }
                purchase.InvoiceNo = updatePurchaseDto.InvoiceNo;
            }

            if (updatePurchaseDto.Date.HasValue)
                purchase.Date = updatePurchaseDto.Date.Value;

            await _purchaseRepository.UpdateAsync(purchase);

            var result = await _purchaseRepository.GetByIdAsync(id);
            return _mapper.Map<PurchaseDto>(result);
        }

        public async Task<bool> DeletePurchaseAsync(int id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
            {
                return false;
            }

            // Restore medicine quantities and create stock movements
            var purchaseItems = await _purchaseItemRepository.GetByPurchaseAsync(id);
            foreach (var item in purchaseItems)
            {
                var medicine = await _medicineRepository.GetByIdAsync(item.MedicineId);
                if (medicine != null)
                {
                    medicine.Quantity -= item.Qty;
                    medicine.UpdatedAt = DateTime.UtcNow;
                    await _medicineRepository.UpdateAsync(medicine);

                    // Create stock movement record for restoration
                    var stockMovement = new StockMovement
                    {
                        MedicineId = item.MedicineId,
                        ChangeQty = -item.Qty, // Negative for restoration
                        Reason = $"Purchase {purchase.InvoiceNo} deleted - stock restored",
                        ReferenceType = "PURCHASE_DELETE",
                        ReferenceId = purchase.Id,
                        CreatedBy = 1, // TODO: Get actual user ID
                        CreatedAt = DateTime.UtcNow
                    };
                    await _stockMovementRepository.AddAsync(stockMovement);
                }
            }

            await _purchaseRepository.DeleteAsync(purchase);
            return true;
        }

        public async Task<decimal> GetTotalPurchasesAsync(DateTime startDate, DateTime endDate)
        {
            return await _purchaseRepository.GetTotalPurchasesAsync(startDate, endDate);
        }

        public async Task<int> GetPurchaseCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _purchaseRepository.GetPurchaseCountAsync(startDate, endDate);
        }
    }
}
