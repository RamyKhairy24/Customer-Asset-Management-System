using CustomerFluent.Models;

namespace CustomerFluent.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerWithFullDetailsAsync(int id);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name);
        Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber);
        Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city);
        Task<IEnumerable<Customer>> GetCustomersByAgeRangeAsync(int minAge, int maxAge);
        Task<decimal> GetCustomerTotalPaidAsync(int customerId);
        Task<decimal> GetCustomerTotalRemainingAsync(int customerId);
        Task<bool> CustomerHasOutstandingPaymentsAsync(int customerId);
        Task<bool> CustomerExistsAsync(int id);
    }
}
