using CustomerFluent.Models;

namespace CustomerFluent.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerWithLocationAsync(int id);
        Task<Customer?> GetCustomerWithPaymentsAsync(int id);
        Task<Customer?> GetCustomerWithFullDetailsAsync(int id);
        Task<IEnumerable<Customer>> GetCustomersWithLocationsAsync();
        Task<IEnumerable<Customer>> GetCustomersWithPaymentsAsync();
        Task<IEnumerable<Customer>> GetCustomersWithFullDetailsAsync();
        Task<IEnumerable<Customer>> GetCustomersByAgeRangeAsync(int minAge, int maxAge);
        Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city);
        Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber);
        Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name);
        Task<decimal> GetTotalAmountPaidByCustomerAsync(int customerId);
        Task<decimal> GetTotalAmountRemainingByCustomerAsync(int customerId);
        Task<bool> HasOutstandingPaymentsAsync(int customerId);
    }
}