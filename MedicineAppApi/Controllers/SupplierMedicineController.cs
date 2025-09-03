using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierMedicineController : ControllerBase
    {
        private readonly ISupplierMedicineRepository _supplierMedicineRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IMapper _mapper;

        public SupplierMedicineController(
            ISupplierMedicineRepository supplierMedicineRepository,
            ISupplierRepository supplierRepository,
            IMedicineRepository medicineRepository,
            IMapper mapper)
        {
            _supplierMedicineRepository = supplierMedicineRepository;
            _supplierRepository = supplierRepository;
            _medicineRepository = medicineRepository;
            _mapper = mapper;
        }

        // GET: api/suppliermedicine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierMedicineDto>>> GetSupplierMedicines()
        {
            var supplierMedicines = await _supplierMedicineRepository.GetAllAsync();
            var supplierMedicineDtos = _mapper.Map<IEnumerable<SupplierMedicineDto>>(supplierMedicines);
            return Ok(supplierMedicineDtos);
        }

        // GET: api/suppliermedicine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierMedicineDto>> GetSupplierMedicine(int id)
        {
            var supplierMedicine = await _supplierMedicineRepository.GetByIdAsync(id);

            if (supplierMedicine == null)
            {
                return NotFound();
            }

            var supplierMedicineDto = _mapper.Map<SupplierMedicineDto>(supplierMedicine);
            return Ok(supplierMedicineDto);
        }

        // GET: api/suppliermedicine/supplier/5
        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<SupplierMedicineDto>>> GetBySupplier(int supplierId)
        {
            var supplierMedicines = await _supplierMedicineRepository.GetBySupplierAsync(supplierId);
            var supplierMedicineDtos = _mapper.Map<IEnumerable<SupplierMedicineDto>>(supplierMedicines);
            return Ok(supplierMedicineDtos);
        }

        // GET: api/suppliermedicine/medicine/5
        [HttpGet("medicine/{medicineId}")]
        public async Task<ActionResult<IEnumerable<SupplierMedicineDto>>> GetByMedicine(int medicineId)
        {
            var supplierMedicines = await _supplierMedicineRepository.GetByMedicineAsync(medicineId);
            var supplierMedicineDtos = _mapper.Map<IEnumerable<SupplierMedicineDto>>(supplierMedicines);
            return Ok(supplierMedicineDtos);
        }

        // POST: api/suppliermedicine
        [HttpPost]
        public async Task<ActionResult<SupplierMedicineDto>> CreateSupplierMedicine(CreateSupplierMedicineDto createSupplierMedicineDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(createSupplierMedicineDto.SupplierId);
            if (supplier == null)
            {
                return BadRequest("Supplier not found");
            }

            // Check if medicine exists
            var medicine = await _medicineRepository.GetByIdAsync(createSupplierMedicineDto.MedicineId);
            if (medicine == null)
            {
                return BadRequest("Medicine not found");
            }

            // Check if relationship already exists
            if (await _supplierMedicineRepository.ExistsAsync(createSupplierMedicineDto.SupplierId, createSupplierMedicineDto.MedicineId))
            {
                return BadRequest("This supplier-medicine relationship already exists");
            }

            var supplierMedicine = _mapper.Map<SupplierMedicine>(createSupplierMedicineDto);
            await _supplierMedicineRepository.AddAsync(supplierMedicine);

            var supplierMedicineDto = _mapper.Map<SupplierMedicineDto>(supplierMedicine);
            return CreatedAtAction(nameof(GetSupplierMedicine), new { id = supplierMedicine.Id }, supplierMedicineDto);
        }

        // PUT: api/suppliermedicine/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplierMedicine(int id, UpdateSupplierMedicineDto updateSupplierMedicineDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplierMedicine = await _supplierMedicineRepository.GetByIdAsync(id);

            if (supplierMedicine == null)
            {
                return NotFound();
            }

            _mapper.Map(updateSupplierMedicineDto, supplierMedicine);
            await _supplierMedicineRepository.UpdateAsync(supplierMedicine);

            return NoContent();
        }

        // DELETE: api/suppliermedicine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplierMedicine(int id)
        {
            var supplierMedicine = await _supplierMedicineRepository.GetByIdAsync(id);
            if (supplierMedicine == null)
            {
                return NotFound();
            }

            await _supplierMedicineRepository.DeleteAsync(supplierMedicine);
            return NoContent();
        }

        // DELETE: api/suppliermedicine/supplier/5/medicine/10
        [HttpDelete("supplier/{supplierId}/medicine/{medicineId}")]
        public async Task<IActionResult> DeleteSupplierMedicineBySupplierAndMedicine(int supplierId, int medicineId)
        {
            var supplierMedicine = await _supplierMedicineRepository.GetBySupplierAndMedicineAsync(supplierId, medicineId);
            if (supplierMedicine == null)
            {
                return NotFound();
            }

            await _supplierMedicineRepository.DeleteAsync(supplierMedicine);
            return NoContent();
        }
    }
}
