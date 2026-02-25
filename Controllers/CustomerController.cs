using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting; // Add this using directive
using CustomerFluent.Models;
using CustomerFluent.Services.CustomerService;
using CustomerFluent.DTOs;
using AutoMapper;

namespace CustomerFluent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    [EnableRateLimiting("UserPolicy")] // Add this for regular users
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;
        private readonly IMapper _mapper;

        public CustomerController(
            ICustomerService customerService,
            ILogger<CustomerController> logger,
            IMapper mapper)
        {
            _customerService = customerService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "UserOrAdmin")] // Both Users and Admins can view
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetCustomers()
        {
            try
            {
                _logger.LogInformation("Retrieving all customers");
                var customers = await _customerService.GetAllCustomersAsync();
                var customerDtos = _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);

                _logger.LogInformation("Successfully retrieved {CustomerCount} customers", customerDtos.Count());
                return Ok(customerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving customers");
                return StatusCode(500, "An error occurred while retrieving customers");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomer(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving customer with ID: {CustomerId}", id);
                var customer = await _customerService.GetCustomerByIdAsync(id);

                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", id);
                    return NotFound($"Customer with ID {id} not found");
                }

                var customerDto = _mapper.Map<CustomerResponseDto>(customer);
                _logger.LogInformation("Successfully retrieved customer {CustomerName} (ID: {CustomerId})",
                    customer.Name, customer.Id);
                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving customer with ID: {CustomerId}", id);
                return StatusCode(500, "An error occurred while retrieving the customer");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")] // Only Admins and Managers can create
        public async Task<ActionResult<CustomerResponseDto>> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for customer creation");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating new customer: {CustomerName}", createCustomerDto.Name);

                var customer = _mapper.Map<Customer>(createCustomerDto);

                // Map location if provided
                if (createCustomerDto.Location != null)
                {
                    customer.Location = _mapper.Map<Location>(createCustomerDto.Location);

                    if (createCustomerDto.Location.IsHouse && createCustomerDto.Location.House != null)
                    {
                        customer.Location.House = _mapper.Map<House>(createCustomerDto.Location.House);
                    }
                    else if (!createCustomerDto.Location.IsHouse && createCustomerDto.Location.Building != null)
                    {
                        customer.Location.Building = _mapper.Map<Building>(createCustomerDto.Location.Building);
                    }
                }

                var createdCustomer = await _customerService.CreateCustomerAsync(customer);
                var customerDto = _mapper.Map<CustomerResponseDto>(createdCustomer);

                _logger.LogInformation("Successfully created customer {CustomerName} with ID: {CustomerId}",
                    createdCustomer.Name, createdCustomer.Id);

                return CreatedAtAction(nameof(GetCustomer), new { id = createdCustomer.Id }, customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating customer: {CustomerName}", createCustomerDto.Name);
                return StatusCode(500, "An error occurred while creating the customer");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Only Admins and Managers can update
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto updateCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for customer update");
                return BadRequest(ModelState);
            }

            try
            {
                if (id != updateCustomerDto.Id)
                {
                    _logger.LogWarning("Customer ID mismatch: URL ID {UrlId} vs Body ID {BodyId}", id, updateCustomerDto.Id);
                    return BadRequest("Customer ID mismatch");
                }

                if (!await _customerService.CustomerExistsAsync(id))
                {
                    _logger.LogWarning("Attempted to update non-existent customer with ID: {CustomerId}", id);
                    return NotFound($"Customer with ID {id} not found");
                }

                _logger.LogInformation("Updating customer with ID: {CustomerId}", id);

                var customer = _mapper.Map<Customer>(updateCustomerDto);
                await _customerService.UpdateCustomerAsync(customer);

                _logger.LogInformation("Successfully updated customer {CustomerName} (ID: {CustomerId})",
                    customer.Name, customer.Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating customer with ID: {CustomerId}", id);
                return StatusCode(500, "An error occurred while updating the customer");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can delete
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete customer with ID: {CustomerId}", id);
                var deleted = await _customerService.DeleteCustomerAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Attempted to delete non-existent customer with ID: {CustomerId}", id);
                    return NotFound($"Customer with ID {id} not found");
                }

                _logger.LogInformation("Successfully deleted customer with ID: {CustomerId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting customer with ID: {CustomerId}", id);
                return StatusCode(500, "An error occurred while deleting the customer");
            }
        }

        [HttpGet("search")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> SearchCustomers([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest("Name parameter is required");
                }

                _logger.LogInformation("Searching customers by name: {Name}", name);
                var customers = await _customerService.SearchCustomersByNameAsync(name);
                var customerDtos = _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);

                _logger.LogInformation("Found {Count} customers matching name: {Name}", customerDtos.Count(), name);
                return Ok(customerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching customers by name: {Name}", name);
                return StatusCode(500, "An error occurred while searching customers");
            }
        }

        [HttpGet("by-phone/{phoneNumber}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomerByPhone(string phoneNumber)
        {
            try
            {
                _logger.LogInformation("Getting customer by phone: {PhoneNumber}", phoneNumber);
                var customer = await _customerService.GetCustomerByPhoneAsync(phoneNumber);

                if (customer == null)
                {
                    _logger.LogWarning("Customer with phone {PhoneNumber} not found", phoneNumber);
                    return NotFound($"Customer with phone {phoneNumber} not found");
                }

                var customerDto = _mapper.Map<CustomerResponseDto>(customer);
                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting customer by phone: {PhoneNumber}", phoneNumber);
                return StatusCode(500, "An error occurred while retrieving the customer");
            }
        }
    }
}