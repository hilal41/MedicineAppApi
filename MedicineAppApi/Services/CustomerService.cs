using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<CustomerDto?> GetCustomerByNameAsync(string name);
        Task<CustomerDto?> GetCustomerByPhoneAsync(string phone);
        Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm);
        Task<IEnumerable<CustomerDto>> GetTopCustomersAsync(int count = 10);
        Task<CustomerDto> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto);
        Task<bool> DeleteCustomerAsync(int id);
        Task<decimal> GetTotalSpentByCustomerAsync(int customerId);
        Task<int> GetInvoiceCountByCustomerAsync(int customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
        {
            // Check if customer with same name already exists
            var existingCustomer = await _customerRepository.GetByNameAsync(createCustomerDto.Name);
            if (existingCustomer != null)
            {
                throw new InvalidOperationException($"Customer with name '{createCustomerDto.Name}' already exists");
            }

            // Check if customer with same phone already exists (if phone is provided)
            if (!string.IsNullOrEmpty(createCustomerDto.Phone))
            {
                var existingCustomerByPhone = await _customerRepository.GetByPhoneAsync(createCustomerDto.Phone);
                if (existingCustomerByPhone != null)
                {
                    throw new InvalidOperationException($"Customer with phone '{createCustomerDto.Phone}' already exists");
                }
            }

            var customer = _mapper.Map<Customer>(createCustomerDto);
            await _customerRepository.AddAsync(customer);

            var result = await _customerRepository.GetByIdAsync(customer.Id);
            var customerDto = _mapper.Map<CustomerDto>(result);
            
            // Set initial values
            customerDto.TotalInvoices = 0;
            customerDto.TotalSpent = 0;

            return customerDto;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return null;

            var customerDto = _mapper.Map<CustomerDto>(customer);
            customerDto.TotalSpent = await _customerRepository.GetTotalSpentByCustomerAsync(id);
            customerDto.TotalInvoices = await _customerRepository.GetInvoiceCountByCustomerAsync(id);

            return customerDto;
        }

        public async Task<CustomerDto?> GetCustomerByNameAsync(string name)
        {
            var customer = await _customerRepository.GetByNameAsync(name);
            if (customer == null)
                return null;

            var customerDto = _mapper.Map<CustomerDto>(customer);
            customerDto.TotalSpent = await _customerRepository.GetTotalSpentByCustomerAsync(customer.Id);
            customerDto.TotalInvoices = await _customerRepository.GetInvoiceCountByCustomerAsync(customer.Id);

            return customerDto;
        }

        public async Task<CustomerDto?> GetCustomerByPhoneAsync(string phone)
        {
            var customer = await _customerRepository.GetByPhoneAsync(phone);
            if (customer == null)
                return null;

            var customerDto = _mapper.Map<CustomerDto>(customer);
            customerDto.TotalSpent = await _customerRepository.GetTotalSpentByCustomerAsync(customer.Id);
            customerDto.TotalInvoices = await _customerRepository.GetInvoiceCountByCustomerAsync(customer.Id);

            return customerDto;
        }

        public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm)
        {
            var customers = await _customerRepository.SearchByNameAsync(searchTerm);
            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);

            // Calculate totals for each customer
            foreach (var customerDto in customerDtos)
            {
                customerDto.TotalSpent = await _customerRepository.GetTotalSpentByCustomerAsync(customerDto.Id);
                customerDto.TotalInvoices = await _customerRepository.GetInvoiceCountByCustomerAsync(customerDto.Id);
            }

            return customerDtos;
        }

        public async Task<IEnumerable<CustomerDto>> GetTopCustomersAsync(int count = 10)
        {
            var customers = await _customerRepository.GetTopCustomersAsync(count);
            var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);

            // Calculate totals for each customer
            foreach (var customerDto in customerDtos)
            {
                customerDto.TotalSpent = await _customerRepository.GetTotalSpentByCustomerAsync(customerDto.Id);
                customerDto.TotalInvoices = await _customerRepository.GetInvoiceCountByCustomerAsync(customerDto.Id);
            }

            return customerDtos;
        }

        public async Task<CustomerDto> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found");
            }

            // Check if new name conflicts with existing customer
            if (!string.IsNullOrEmpty(updateCustomerDto.Name) && updateCustomerDto.Name != customer.Name)
            {
                var existingCustomer = await _customerRepository.GetByNameAsync(updateCustomerDto.Name);
                if (existingCustomer != null)
                {
                    throw new InvalidOperationException($"Customer with name '{updateCustomerDto.Name}' already exists");
                }
            }

            // Check if new phone conflicts with existing customer
            if (!string.IsNullOrEmpty(updateCustomerDto.Phone) && updateCustomerDto.Phone != customer.Phone)
            {
                var existingCustomerByPhone = await _customerRepository.GetByPhoneAsync(updateCustomerDto.Phone);
                if (existingCustomerByPhone != null)
                {
                    throw new InvalidOperationException($"Customer with phone '{updateCustomerDto.Phone}' already exists");
                }
            }

            // Update customer properties
            if (!string.IsNullOrEmpty(updateCustomerDto.Name))
                customer.Name = updateCustomerDto.Name;

            if (updateCustomerDto.Phone != null)
                customer.Phone = updateCustomerDto.Phone;

            if (updateCustomerDto.Address != null)
                customer.Address = updateCustomerDto.Address;

            await _customerRepository.UpdateAsync(customer);

            var result = await _customerRepository.GetByIdAsync(id);
            var customerDto = _mapper.Map<CustomerDto>(result);
            customerDto.TotalSpent = await _customerRepository.GetTotalSpentByCustomerAsync(id);
            customerDto.TotalInvoices = await _customerRepository.GetInvoiceCountByCustomerAsync(id);

            return customerDto;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return false;
            }

            // Check if customer has any invoices
            var invoiceCount = await _customerRepository.GetInvoiceCountByCustomerAsync(id);
            if (invoiceCount > 0)
            {
                throw new InvalidOperationException($"Cannot delete customer '{customer.Name}' because they have {invoiceCount} invoice(s). Please delete the invoices first.");
            }

            await _customerRepository.DeleteAsync(customer);
            return true;
        }

        public async Task<decimal> GetTotalSpentByCustomerAsync(int customerId)
        {
            return await _customerRepository.GetTotalSpentByCustomerAsync(customerId);
        }

        public async Task<int> GetInvoiceCountByCustomerAsync(int customerId)
        {
            return await _customerRepository.GetInvoiceCountByCustomerAsync(customerId);
        }
    }
}
