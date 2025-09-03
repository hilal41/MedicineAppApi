using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public MedicineController(IMedicineRepository medicineRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _medicineRepository = medicineRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // GET: api/medicine
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineDto>>> GetMedicines()
        {
            var medicines = await _medicineRepository.GetAllAsync();
            var medicineDtos = _mapper.Map<IEnumerable<MedicineDto>>(medicines);
            return Ok(medicineDtos);
        }

        // GET: api/medicine/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicineDto>> GetMedicine(int id)
        {
            var medicine = await _medicineRepository.GetByIdAsync(id);

            if (medicine == null)
            {
                return NotFound();
            }

            var medicineDto = _mapper.Map<MedicineDto>(medicine);
            return Ok(medicineDto);
        }

        // GET: api/medicine/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<MedicineDto>>> GetMedicinesByCategory(int categoryId)
        {
            var medicines = await _medicineRepository.GetByCategoryAsync(categoryId);
            var medicineDtos = _mapper.Map<IEnumerable<MedicineDto>>(medicines);
            return Ok(medicineDtos);
        }

        // GET: api/medicine/expiring/30
        [HttpGet("expiring/{daysThreshold}")]
        public async Task<ActionResult<IEnumerable<MedicineDto>>> GetExpiringMedicines(int daysThreshold)
        {
            var medicines = await _medicineRepository.GetExpiringSoonAsync(daysThreshold);
            var medicineDtos = _mapper.Map<IEnumerable<MedicineDto>>(medicines);
            return Ok(medicineDtos);
        }

        // GET: api/medicine/barcode/123456789
        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<MedicineDto>> GetMedicineByBarcode(string barcode)
        {
            var medicine = await _medicineRepository.GetByBarcodeAsync(barcode);

            if (medicine == null)
            {
                return NotFound();
            }

            var medicineDto = _mapper.Map<MedicineDto>(medicine);
            return Ok(medicineDto);
        }

        // POST: api/medicine
        [HttpPost]
        public async Task<ActionResult<MedicineDto>> CreateMedicine(CreateMedicineDto createMedicineDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if category exists
            var category = await _categoryRepository.GetByIdAsync(createMedicineDto.CategoryId);
            if (category == null)
            {
                return BadRequest("Category not found");
            }

            // Check if medicine name already exists
            if (await _medicineRepository.NameExistsAsync(createMedicineDto.Name))
            {
                return BadRequest("Medicine with this name already exists");
            }

            // Check if barcode already exists (if provided)
            if (!string.IsNullOrEmpty(createMedicineDto.Barcode) && 
                await _medicineRepository.BarcodeExistsAsync(createMedicineDto.Barcode))
            {
                return BadRequest("Medicine with this barcode already exists");
            }

            var medicine = _mapper.Map<Medicine>(createMedicineDto);
            await _medicineRepository.AddAsync(medicine);

            var medicineDto = _mapper.Map<MedicineDto>(medicine);
            return CreatedAtAction(nameof(GetMedicine), new { id = medicine.Id }, medicineDto);
        }

        // PUT: api/medicine/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, UpdateMedicineDto updateMedicineDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var medicine = await _medicineRepository.GetByIdAsync(id);

            if (medicine == null)
            {
                return NotFound();
            }

            // Check if category exists (if being updated)
            if (updateMedicineDto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(updateMedicineDto.CategoryId.Value);
                if (category == null)
                {
                    return BadRequest("Category not found");
                }
            }

            // Check if medicine name already exists (if being updated)
            if (!string.IsNullOrEmpty(updateMedicineDto.Name) && 
                updateMedicineDto.Name != medicine.Name &&
                await _medicineRepository.NameExistsAsync(updateMedicineDto.Name))
            {
                return BadRequest("Medicine with this name already exists");
            }

            // Check if barcode already exists (if being updated)
            if (!string.IsNullOrEmpty(updateMedicineDto.Barcode) && 
                updateMedicineDto.Barcode != medicine.Barcode &&
                await _medicineRepository.BarcodeExistsAsync(updateMedicineDto.Barcode))
            {
                return BadRequest("Medicine with this barcode already exists");
            }

            _mapper.Map(updateMedicineDto, medicine);
            await _medicineRepository.UpdateAsync(medicine);

            return NoContent();
        }

        // DELETE: api/medicine/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var medicine = await _medicineRepository.GetByIdAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }

            await _medicineRepository.DeleteAsync(medicine);
            return NoContent();
        }
    }
}
