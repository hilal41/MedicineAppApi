using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto, int createdByUserId);
        Task<InvoiceDto?> GetInvoiceByIdAsync(int id);
        Task<InvoiceDto?> GetInvoiceByInvoiceNoAsync(string invoiceNo);
        Task<IEnumerable<InvoiceDto>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerAsync(int customerId);
        Task<InvoiceDto> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateInvoiceDto);
        Task<bool> DeleteInvoiceAsync(int id);
        Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate);
        Task<int> GetInvoiceCountAsync(DateTime startDate, DateTime endDate);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceItemRepository _invoiceItemRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly IMapper _mapper;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IInvoiceItemRepository invoiceItemRepository,
            IMedicineRepository medicineRepository,
            IStockMovementRepository stockMovementRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceItemRepository = invoiceItemRepository;
            _medicineRepository = medicineRepository;
            _stockMovementRepository = stockMovementRepository;
            _mapper = mapper;
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createInvoiceDto, int createdByUserId)
        {
            // Check if invoice number already exists
            if (await _invoiceRepository.InvoiceNoExistsAsync(createInvoiceDto.InvoiceNo))
            {
                throw new InvalidOperationException("Invoice number already exists");
            }

            // Validate items and calculate totals
            decimal subtotal = 0;
            foreach (var item in createInvoiceDto.Items)
            {
                var medicine = await _medicineRepository.GetByIdAsync(item.MedicineId);
                if (medicine == null)
                {
                    throw new InvalidOperationException($"Medicine with ID {item.MedicineId} not found");
                }

                if (medicine.Quantity < item.Qty)
                {
                    throw new InvalidOperationException($"Insufficient stock for medicine {medicine.Name}. Available: {medicine.Quantity}, Requested: {item.Qty}");
                }

                item.Price = medicine.Price; // Use medicine's current price
                subtotal += item.Price * item.Qty;
            }

            // Calculate discount and total
            decimal discountAmount = 0;
            if (createInvoiceDto.DiscountPercent > 0)
            {
                discountAmount = subtotal * (createInvoiceDto.DiscountPercent / 100);
            }
            else if (createInvoiceDto.DiscountAmount > 0)
            {
                discountAmount = createInvoiceDto.DiscountAmount;
            }

            decimal total = subtotal - discountAmount;

            // Create invoice
            var invoice = new Invoice
            {
                InvoiceNo = createInvoiceDto.InvoiceNo,
                Date = createInvoiceDto.Date,
                CustomerId = createInvoiceDto.CustomerId,
                Subtotal = subtotal,
                DiscountPercent = createInvoiceDto.DiscountPercent,
                DiscountAmount = discountAmount,
                Total = total,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow,
                Notes = createInvoiceDto.Notes
            };

            await _invoiceRepository.AddAsync(invoice);

            // Create invoice items and update medicine quantities
            foreach (var itemDto in createInvoiceDto.Items)
            {
                var medicine = await _medicineRepository.GetByIdAsync(itemDto.MedicineId);
                
                var invoiceItem = new InvoiceItem
                {
                    InvoiceId = invoice.Id,
                    MedicineId = itemDto.MedicineId,
                    Qty = itemDto.Qty,
                    Price = itemDto.Price,
                    Subtotal = itemDto.Price * itemDto.Qty
                };

                await _invoiceItemRepository.AddAsync(invoiceItem);

                // Update medicine quantity
                medicine.Quantity -= itemDto.Qty;
                medicine.UpdatedAt = DateTime.UtcNow;
                await _medicineRepository.UpdateAsync(medicine);

                // Create stock movement record
                var stockMovement = new StockMovement
                {
                    MedicineId = itemDto.MedicineId,
                    ChangeQty = -itemDto.Qty, // Negative for sales
                    Reason = $"Sold via invoice {invoice.InvoiceNo}",
                    ReferenceType = "INVOICE",
                    ReferenceId = invoice.Id,
                    CreatedBy = createdByUserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _stockMovementRepository.AddAsync(stockMovement);
            }

            // Return the created invoice with items
            var result = await _invoiceRepository.GetByIdAsync(invoice.Id);
            return _mapper.Map<InvoiceDto>(result);
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            return invoice != null ? _mapper.Map<InvoiceDto>(invoice) : null;
        }

        public async Task<InvoiceDto?> GetInvoiceByInvoiceNoAsync(string invoiceNo)
        {
            var invoice = await _invoiceRepository.GetByInvoiceNoAsync(invoiceNo);
            return invoice != null ? _mapper.Map<InvoiceDto>(invoice) : null;
        }

        public async Task<IEnumerable<InvoiceDto>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _invoiceRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        }

        public async Task<IEnumerable<InvoiceDto>> GetInvoicesByCustomerAsync(int customerId)
        {
            var invoices = await _invoiceRepository.GetByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        }

        public async Task<InvoiceDto> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateInvoiceDto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
            {
                throw new InvalidOperationException("Invoice not found");
            }

            // Update invoice properties
            if (updateInvoiceDto.CustomerId.HasValue)
                invoice.CustomerId = updateInvoiceDto.CustomerId.Value;

            if (updateInvoiceDto.DiscountPercent.HasValue)
                invoice.DiscountPercent = updateInvoiceDto.DiscountPercent.Value;

            if (updateInvoiceDto.DiscountAmount.HasValue)
                invoice.DiscountAmount = updateInvoiceDto.DiscountAmount.Value;

            if (updateInvoiceDto.Notes != null)
                invoice.Notes = updateInvoiceDto.Notes;

            // Recalculate total if discount changed
            if (updateInvoiceDto.DiscountPercent.HasValue || updateInvoiceDto.DiscountAmount.HasValue)
            {
                decimal discountAmount = 0;
                if (invoice.DiscountPercent > 0)
                {
                    discountAmount = invoice.Subtotal * (invoice.DiscountPercent / 100);
                }
                else if (invoice.DiscountAmount > 0)
                {
                    discountAmount = invoice.DiscountAmount;
                }

                invoice.Total = invoice.Subtotal - discountAmount;
            }

            await _invoiceRepository.UpdateAsync(invoice);

            var result = await _invoiceRepository.GetByIdAsync(id);
            return _mapper.Map<InvoiceDto>(result);
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
            {
                return false;
            }

            // Restore medicine quantities and create stock movements
            var invoiceItems = await _invoiceItemRepository.GetByInvoiceAsync(id);
            foreach (var item in invoiceItems)
            {
                var medicine = await _medicineRepository.GetByIdAsync(item.MedicineId);
                if (medicine != null)
                {
                    medicine.Quantity += item.Qty;
                    medicine.UpdatedAt = DateTime.UtcNow;
                    await _medicineRepository.UpdateAsync(medicine);

                    // Create stock movement record for restoration
                    var stockMovement = new StockMovement
                    {
                        MedicineId = item.MedicineId,
                        ChangeQty = item.Qty, // Positive for restoration
                        Reason = $"Invoice {invoice.InvoiceNo} deleted - stock restored",
                        ReferenceType = "INVOICE_DELETE",
                        ReferenceId = invoice.Id,
                        CreatedBy = 1, // TODO: Get actual user ID
                        CreatedAt = DateTime.UtcNow
                    };
                    await _stockMovementRepository.AddAsync(stockMovement);
                }
            }

            await _invoiceRepository.DeleteAsync(invoice);
            return true;
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate)
        {
            return await _invoiceRepository.GetTotalSalesAsync(startDate, endDate);
        }

        public async Task<int> GetInvoiceCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _invoiceRepository.GetInvoiceCountAsync(startDate, endDate);
        }
    }
}
