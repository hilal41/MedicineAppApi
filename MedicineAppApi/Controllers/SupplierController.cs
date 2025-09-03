using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierController(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        // GET: api/supplier
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetSuppliers()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            var supplierDtos = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
            return Ok(supplierDtos);
        }

        // GET: api/supplier/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetSupplier(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            var supplierDto = _mapper.Map<SupplierDto>(supplier);
            return Ok(supplierDto);
        }

        // POST: api/supplier
        [HttpPost]
        public async Task<ActionResult<SupplierDto>> CreateSupplier(CreateSupplierDto createSupplierDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = _mapper.Map<Supplier>(createSupplierDto);
            await _supplierRepository.AddAsync(supplier);

            var supplierDto = _mapper.Map<SupplierDto>(supplier);
            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplierDto);
        }

        // PUT: api/supplier/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, UpdateSupplierDto updateSupplierDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            _mapper.Map(updateSupplierDto, supplier);
            await _supplierRepository.UpdateAsync(supplier);

            return NoContent();
        }

        // DELETE: api/supplier/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            await _supplierRepository.DeleteAsync(supplier);
            return NoContent();
        }
    }
}
