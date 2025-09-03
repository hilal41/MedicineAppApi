using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Services
{
    public interface IStockMovementService
    {
        Task<StockMovementDto> CreateStockMovementAsync(CreateStockMovementDto createStockMovementDto, int createdByUserId);
        Task<StockMovementDto?> GetStockMovementByIdAsync(int id);
        Task<IEnumerable<StockMovementDto>> GetStockMovementsByMedicineAsync(int medicineId);
        Task<IEnumerable<StockMovementDto>> GetStockMovementsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<StockMovementDto>> GetStockMovementsByReferenceAsync(string referenceType, int referenceId);
        Task<IEnumerable<StockMovementDto>> GetStockMovementsByCreatedByAsync(int createdBy);
        Task<StockMovementSummaryDto> GetStockMovementSummaryAsync(int medicineId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<StockMovementSummaryDto>> GetAllStockMovementSummariesAsync();
        Task<decimal> GetTotalInventoryValueAsync();
        Task<IEnumerable<StockMovementDto>> GetLowStockMovementsAsync(int threshold = 10);
        Task<bool> DeleteStockMovementAsync(int id);
    }

    public class StockMovementService : IStockMovementService
    {
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IMapper _mapper;

        public StockMovementService(
            IStockMovementRepository stockMovementRepository,
            IMedicineRepository medicineRepository,
            IMapper mapper)
        {
            _stockMovementRepository = stockMovementRepository;
            _medicineRepository = medicineRepository;
            _mapper = mapper;
        }

        public async Task<StockMovementDto> CreateStockMovementAsync(CreateStockMovementDto createStockMovementDto, int createdByUserId)
        {
            // Validate medicine exists
            var medicine = await _medicineRepository.GetByIdAsync(createStockMovementDto.MedicineId);
            if (medicine == null)
            {
                throw new InvalidOperationException($"Medicine with ID {createStockMovementDto.MedicineId} not found");
            }

            // Check if stock reduction would result in negative stock
            if (createStockMovementDto.ChangeQty < 0 && 
                Math.Abs(createStockMovementDto.ChangeQty) > medicine.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for medicine {medicine.Name}. Available: {medicine.Quantity}, Requested reduction: {Math.Abs(createStockMovementDto.ChangeQty)}");
            }

            // Create stock movement
            var stockMovement = new StockMovement
            {
                MedicineId = createStockMovementDto.MedicineId,
                ChangeQty = createStockMovementDto.ChangeQty,
                Reason = createStockMovementDto.Reason,
                ReferenceType = createStockMovementDto.ReferenceType,
                ReferenceId = createStockMovementDto.ReferenceId,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _stockMovementRepository.AddAsync(stockMovement);

            // Update medicine quantity
            medicine.Quantity += createStockMovementDto.ChangeQty;
            medicine.UpdatedAt = DateTime.UtcNow;
            await _medicineRepository.UpdateAsync(medicine);

            // Return the created stock movement with calculated balance
            var result = await _stockMovementRepository.GetByIdAsync(stockMovement.Id);
            var stockMovementDto = _mapper.Map<StockMovementDto>(result);
            stockMovementDto.BalanceAfterMovement = medicine.Quantity;

            return stockMovementDto;
        }

        public async Task<StockMovementDto?> GetStockMovementByIdAsync(int id)
        {
            var stockMovement = await _stockMovementRepository.GetByIdAsync(id);
            if (stockMovement == null)
                return null;

            var stockMovementDto = _mapper.Map<StockMovementDto>(stockMovement);
            
            // Calculate balance after this movement
            var medicine = await _medicineRepository.GetByIdAsync(stockMovement.MedicineId);
            stockMovementDto.BalanceAfterMovement = medicine?.Quantity ?? 0;

            return stockMovementDto;
        }

        public async Task<IEnumerable<StockMovementDto>> GetStockMovementsByMedicineAsync(int medicineId)
        {
            var stockMovements = await _stockMovementRepository.GetByMedicineAsync(medicineId);
            var stockMovementDtos = _mapper.Map<IEnumerable<StockMovementDto>>(stockMovements);

            // Calculate balance after each movement
            var medicine = await _medicineRepository.GetByIdAsync(medicineId);
            var currentBalance = medicine?.Quantity ?? 0;

            foreach (var dto in stockMovementDtos.Reverse())
            {
                dto.BalanceAfterMovement = currentBalance;
                currentBalance -= dto.ChangeQty; // Go backwards in time
            }

            return stockMovementDtos.Reverse(); // Return in chronological order
        }

        public async Task<IEnumerable<StockMovementDto>> GetStockMovementsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var stockMovements = await _stockMovementRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<StockMovementDto>>(stockMovements);
        }

        public async Task<IEnumerable<StockMovementDto>> GetStockMovementsByReferenceAsync(string referenceType, int referenceId)
        {
            var stockMovements = await _stockMovementRepository.GetByReferenceAsync(referenceType, referenceId);
            return _mapper.Map<IEnumerable<StockMovementDto>>(stockMovements);
        }

        public async Task<IEnumerable<StockMovementDto>> GetStockMovementsByCreatedByAsync(int createdBy)
        {
            var stockMovements = await _stockMovementRepository.GetByCreatedByAsync(createdBy);
            return _mapper.Map<IEnumerable<StockMovementDto>>(stockMovements);
        }

        public async Task<StockMovementSummaryDto> GetStockMovementSummaryAsync(int medicineId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var medicine = await _medicineRepository.GetByIdAsync(medicineId);
            if (medicine == null)
            {
                throw new InvalidOperationException($"Medicine with ID {medicineId} not found");
            }

            var totalIn = await _stockMovementRepository.GetTotalInByMedicineAsync(medicineId, startDate, endDate);
            var totalOut = await _stockMovementRepository.GetTotalOutByMedicineAsync(medicineId, startDate, endDate);
            var lastMovementDate = await _stockMovementRepository.GetLastMovementDateAsync(medicineId);

            return new StockMovementSummaryDto
            {
                MedicineId = medicineId,
                MedicineName = medicine.Name,
                CurrentStock = medicine.Quantity,
                TotalIn = totalIn,
                TotalOut = totalOut,
                LastMovementDate = lastMovementDate ?? DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<StockMovementSummaryDto>> GetAllStockMovementSummariesAsync()
        {
            var medicines = await _medicineRepository.GetAllAsync();
            var summaries = new List<StockMovementSummaryDto>();

            foreach (var medicine in medicines)
            {
                var totalIn = await _stockMovementRepository.GetTotalInByMedicineAsync(medicine.Id);
                var totalOut = await _stockMovementRepository.GetTotalOutByMedicineAsync(medicine.Id);
                var lastMovementDate = await _stockMovementRepository.GetLastMovementDateAsync(medicine.Id);

                summaries.Add(new StockMovementSummaryDto
                {
                    MedicineId = medicine.Id,
                    MedicineName = medicine.Name,
                    CurrentStock = medicine.Quantity,
                    TotalIn = totalIn,
                    TotalOut = totalOut,
                    LastMovementDate = lastMovementDate ?? DateTime.UtcNow
                });
            }

            return summaries;
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            var medicines = await _medicineRepository.GetAllAsync();
            return medicines.Sum(m => m.Quantity * m.Price);
        }

        public async Task<IEnumerable<StockMovementDto>> GetLowStockMovementsAsync(int threshold = 10)
        {
            var stockMovements = await _stockMovementRepository.GetLowStockMovementsAsync(threshold);
            return _mapper.Map<IEnumerable<StockMovementDto>>(stockMovements);
        }

        public async Task<bool> DeleteStockMovementAsync(int id)
        {
            var stockMovement = await _stockMovementRepository.GetByIdAsync(id);
            if (stockMovement == null)
            {
                return false;
            }

            // Reverse the stock movement
            var medicine = await _medicineRepository.GetByIdAsync(stockMovement.MedicineId);
            if (medicine != null)
            {
                medicine.Quantity -= stockMovement.ChangeQty; // Reverse the change
                medicine.UpdatedAt = DateTime.UtcNow;
                await _medicineRepository.UpdateAsync(medicine);
            }

            await _stockMovementRepository.DeleteAsync(stockMovement);
            return true;
        }
    }
}
