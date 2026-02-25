using CustomerFluent.Models;
using CustomerFluent.Repositories;

namespace CustomerFluent.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            try
            {
                _logger.LogInformation("Getting all customers");
                return await _unitOfWork.Customers.GetCustomersWithFullDetailsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting customer with ID: {CustomerId}", id);
                return await _unitOfWork.Customers.GetCustomerWithFullDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with ID: {CustomerId}", id);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerWithFullDetailsAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting customer with full details for ID: {CustomerId}", id);
                return await _unitOfWork.Customers.GetCustomerWithFullDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with full details for ID: {CustomerId}", id);
                throw;
            }
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            try
            {
                _logger.LogInformation("Creating new customer: {CustomerName}", customer.Name);
                
                customer.CreatedDate = DateTime.Now;
                customer.ModifiedDate = DateTime.Now;

                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully created customer: {CustomerName} with ID: {CustomerId}", 
                    customer.Name, customer.Id);
                
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer: {CustomerName}", customer.Name);
                throw;
            }
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                _logger.LogInformation("Updating customer with ID: {CustomerId}", customer.Id);
                
                customer.ModifiedDate = DateTime.Now;
                
                await _unitOfWork.Customers.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully updated customer: {CustomerName} with ID: {CustomerId}", 
                    customer.Name, customer.Id);
                
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer with ID: {CustomerId}", customer.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);
                
                var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for deletion", id);
                    return false;
                }

                await _unitOfWork.Customers.DeleteAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted customer: {CustomerName} with ID: {CustomerId}", 
                    customer.Name, customer.Id);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name)
        {
            try
            {
                _logger.LogInformation("Searching customers by name: {Name}", name);
                return await _unitOfWork.Customers.SearchCustomersByNameAsync(name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers by name: {Name}", name);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber)
        {
            try
            {
                _logger.LogInformation("Getting customer by phone: {PhoneNumber}", phoneNumber);
                return await _unitOfWork.Customers.GetCustomerByPhoneAsync(phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by phone: {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city)
        {
            try
            {
                _logger.LogInformation("Getting customers by city: {City}", city);
                return await _unitOfWork.Customers.GetCustomersByCityAsync(city);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customers by city: {City}", city);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByAgeRangeAsync(int minAge, int maxAge)
        {
            try
            {
                _logger.LogInformation("Getting customers by age range: {MinAge}-{MaxAge}", minAge, maxAge);
                return await _unitOfWork.Customers.GetCustomersByAgeRangeAsync(minAge, maxAge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customers by age range: {MinAge}-{MaxAge}", minAge, maxAge);
                throw;
            }
        }

        public async Task<decimal> GetCustomerTotalPaidAsync(int customerId)
        {
            try
            {
                _logger.LogInformation("Getting total paid amount for customer: {CustomerId}", customerId);
                return await _unitOfWork.Customers.GetTotalAmountPaidByCustomerAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total paid amount for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<decimal> GetCustomerTotalRemainingAsync(int customerId)
        {
            try
            {
                _logger.LogInformation("Getting total remaining amount for customer: {CustomerId}", customerId);
                return await _unitOfWork.Customers.GetTotalAmountRemainingByCustomerAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total remaining amount for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> CustomerHasOutstandingPaymentsAsync(int customerId)
        {
            try
            {
                _logger.LogInformation("Checking outstanding payments for customer: {CustomerId}", customerId);
                return await _unitOfWork.Customers.HasOutstandingPaymentsAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking outstanding payments for customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> CustomerExistsAsync(int id)
        {
            try
            {
                _logger.LogDebug("Checking if customer exists with ID: {CustomerId}", id);
                return await _unitOfWork.Customers.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking customer existence with ID: {CustomerId}", id);
                throw;
            }
        }
    }
}
