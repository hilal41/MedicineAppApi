using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Common.Exceptions;
using AutoMapper;

namespace MedicineAppApi.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto, int receivedByUserId);
        Task<PaymentDto?> GetPaymentByIdAsync(int id);
        Task<IEnumerable<PaymentDto>> GetPaymentsByInvoiceAsync(int invoiceId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByMethodAsync(string method);
        Task<IEnumerable<PaymentDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentDto>> GetPaymentsByReceivedByAsync(int receivedBy);
        Task<PaymentDto> UpdatePaymentAsync(int id, UpdatePaymentDto updatePaymentDto);
        Task<bool> DeletePaymentAsync(int id);
        Task<PaymentSummaryDto> GetPaymentSummaryByInvoiceAsync(int invoiceId);
        Task<decimal> GetTotalPaymentsByInvoiceAsync(int invoiceId);
        Task<decimal> GetTotalPaymentsByMethodAsync(string method, DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetPaymentCountByDateRangeAsync(DateTime startDate, DateTime endDate);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto, int receivedByUserId)
        {
                    // Validate invoice exists
        var invoice = await _invoiceRepository.GetByIdAsync(createPaymentDto.InvoiceId);
        if (invoice == null)
        {
            throw new NotFoundException("Invoice", createPaymentDto.InvoiceId, "INVOICE_NOT_FOUND");
        }

            // Get total payments for this invoice
            var totalPaid = await _paymentRepository.GetTotalPaymentsByInvoiceAsync(createPaymentDto.InvoiceId);
            var remainingBalance = invoice.Total - totalPaid;

                    // Check if payment amount exceeds remaining balance
        if (createPaymentDto.Amount > remainingBalance)
        {
            throw new BusinessRuleException("Payment amount exceeds remaining balance", "PAYMENT_AMOUNT_EXCEEDS_BALANCE", new { InvoiceNo = invoice.InvoiceNo, PaymentAmount = createPaymentDto.Amount, RemainingBalance = remainingBalance });
        }

                    // Validate payment method
        var validMethods = new[] { "cash", "card", "credit", "bank_transfer", "check", "mobile_money" };
        if (!validMethods.Contains(createPaymentDto.Method.ToLower()))
        {
            throw new ValidationException($"Invalid payment method. Valid methods are: {string.Join(", ", validMethods)}", "INVALID_PAYMENT_METHOD");
        }

            var payment = _mapper.Map<Payment>(createPaymentDto);
            payment.ReceivedBy = receivedByUserId;
            payment.CreatedAt = DateTime.UtcNow;

            await _paymentRepository.AddAsync(payment);

            var result = await _paymentRepository.GetByIdAsync(payment.Id);
            return _mapper.Map<PaymentDto>(result);
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByInvoiceAsync(int invoiceId)
        {
            var payments = await _paymentRepository.GetByInvoiceAsync(invoiceId);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByMethodAsync(string method)
        {
            var payments = await _paymentRepository.GetByMethodAsync(method);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByReceivedByAsync(int receivedBy)
        {
            var payments = await _paymentRepository.GetByReceivedByAsync(receivedBy);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto> UpdatePaymentAsync(int id, UpdatePaymentDto updatePaymentDto)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                throw new NotFoundException("Payment", id, "PAYMENT_NOT_FOUND");
            }

            // If amount is being updated, validate it doesn't exceed remaining balance
            if (updatePaymentDto.Amount.HasValue && updatePaymentDto.Amount.Value != payment.Amount)
            {
                var invoice = await _invoiceRepository.GetByIdAsync(payment.InvoiceId);
                if (invoice != null)
                {
                    var totalPaid = await _paymentRepository.GetTotalPaymentsByInvoiceAsync(payment.InvoiceId);
                    var currentPaymentAmount = payment.Amount;
                    var remainingBalance = invoice.Total - (totalPaid - currentPaymentAmount);

                    if (updatePaymentDto.Amount.Value > remainingBalance + currentPaymentAmount)
                    {
                        throw new BusinessRuleException("Updated payment amount exceeds remaining balance", "PAYMENT_AMOUNT_EXCEEDS_BALANCE", new { InvoiceNo = invoice.InvoiceNo, PaymentAmount = updatePaymentDto.Amount.Value, RemainingBalance = remainingBalance });
                    }
                }
            }

            // Validate payment method if being updated
            if (!string.IsNullOrEmpty(updatePaymentDto.Method))
            {
                var validMethods = new[] { "cash", "card", "credit", "bank_transfer", "check", "mobile_money" };
                if (!validMethods.Contains(updatePaymentDto.Method.ToLower()))
                {
                    throw new ValidationException($"Invalid payment method. Valid methods are: {string.Join(", ", validMethods)}", "INVALID_PAYMENT_METHOD");
                }
            }

            // Update payment properties
            if (updatePaymentDto.Amount.HasValue)
                payment.Amount = updatePaymentDto.Amount.Value;

            if (!string.IsNullOrEmpty(updatePaymentDto.Method))
                payment.Method = updatePaymentDto.Method;

            if (updatePaymentDto.Date.HasValue)
                payment.Date = updatePaymentDto.Date.Value;

            await _paymentRepository.UpdateAsync(payment);

            var result = await _paymentRepository.GetByIdAsync(id);
            return _mapper.Map<PaymentDto>(result);
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return false;
            }

            await _paymentRepository.DeleteAsync(payment);
            return true;
        }

        public async Task<PaymentSummaryDto> GetPaymentSummaryByInvoiceAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new InvalidOperationException($"Invoice with ID {invoiceId} not found");
            }

            var payments = await _paymentRepository.GetByInvoiceAsync(invoiceId);
            var totalPaid = await _paymentRepository.GetTotalPaymentsByInvoiceAsync(invoiceId);
            var remainingBalance = invoice.Total - totalPaid;
            var isFullyPaid = remainingBalance <= 0;

            var paymentDtos = _mapper.Map<List<PaymentDto>>(payments);

            return new PaymentSummaryDto
            {
                InvoiceId = invoiceId,
                InvoiceNo = invoice.InvoiceNo,
                InvoiceTotal = invoice.Total,
                TotalPaid = totalPaid,
                RemainingBalance = remainingBalance,
                IsFullyPaid = isFullyPaid,
                Payments = paymentDtos
            };
        }

        public async Task<decimal> GetTotalPaymentsByInvoiceAsync(int invoiceId)
        {
            return await _paymentRepository.GetTotalPaymentsByInvoiceAsync(invoiceId);
        }

        public async Task<decimal> GetTotalPaymentsByMethodAsync(string method, DateTime startDate, DateTime endDate)
        {
            return await _paymentRepository.GetTotalPaymentsByMethodAsync(method, startDate, endDate);
        }

        public async Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _paymentRepository.GetTotalPaymentsByDateRangeAsync(startDate, endDate);
        }

        public async Task<int> GetPaymentCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _paymentRepository.GetPaymentCountByDateRangeAsync(startDate, endDate);
        }
    }
}
