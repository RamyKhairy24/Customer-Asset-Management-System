using Microsoft.EntityFrameworkCore;
using CustomerFluent.Data;
using CustomerFluent.Models;

namespace CustomerFluent.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context, ILogger<CustomerRepository> logger) 
            : base(context, logger)
        {
        }

        public async Task<Customer?> GetCustomerWithLocationAsync(int id)
        {
            try
            {
                _logger.LogDebug("Getting customer with location for ID: {CustomerId}", id);
                return await _dbSet
                    .Include(c => c.Location)
                        .ThenInclude(l => l!.House)
                    .Include(c => c.Location)
                        .ThenInclude(l => l!.Building)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with location for ID: {CustomerId}", id);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerWithPaymentsAsync(int id)
        {
            try
            {
                _logger.LogDebug("Getting customer with payments for ID: {CustomerId}", id);
                return await _dbSet
                    .Include(c => c.PaymentStatuses)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with payments for ID: {CustomerId}", id);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerWithFullDetailsAsync(int id)
        {
            try
            {
                _logger.LogDebug("Getting customer with full details for ID: {CustomerId}", id);
                return await _dbSet
                    .Include(c => c.Location)
                        .ThenInclude(l => l!.House)
                    .Include(c => c.Location)
                        .ThenInclude(l => l!.Building)
                    .Include(c => c.PaymentStatuses)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with full details for ID: {CustomerId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithLocationsAsync()
        {
            try
            {
                _logger.LogDebug("Getting all customers with locations");
                return await _dbSet
                    .Include(c => c.Location)
                        .ThenInclude(l => l!.House)
                    .Include(c => c.Location)
                        .ThenInclude(l => l!.Building)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers with locations");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithPaymentsAsync()
        {
            try
            {
                _logger.LogDebug("Getting all customers with payments");
                return await _dbSet
                    .Include(c => c.PaymentStatuses)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers with payments");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithFullDetailsAsync()
        {
            try
            {
                _logger.LogDebug("Getting all customers with full details");
                return await _dbSet
                    .Include(c => c.Location)
                        .ThenInclude(l => l!.House)
                    .Include(c => c.Location)
                        .ThenInclude(l => l!.Building)
                    .Include(c => c.PaymentStatuses)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers with full details");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByAgeRangeAsync(int minAge, int maxAge)
        {
            try
            {
                _logger.LogDebug("Getting customers by age range: {MinAge}-{MaxAge}", minAge, maxAge);
                return await _dbSet
                    .Where(c => c.Age >= minAge && c.Age <= maxAge)
                    .Include(c => c.Location)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customers by age range: {MinAge}-{MaxAge}", minAge, maxAge);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city)
        {
            try
            {
                _logger.LogDebug("Getting customers by city: {City}", city);
                return await _dbSet
                    .Include(c => c.Location)
                    .Where(c => c.Location != null && c.Location.City == city)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customers by city: {City}", city);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber)
        {
            try
            {
                _logger.LogDebug("Getting customer by phone: {PhoneNumber}", phoneNumber);
                return await _dbSet
                    .Include(c => c.Location)
                    .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by phone: {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name)
        {
            try
            {
                _logger.LogDebug("Searching customers by name: {Name}", name);
                return await _dbSet
                    .Where(c => c.Name.Contains(name))
                    .Include(c => c.Location)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers by name: {Name}", name);
                throw;
            }
        }

        public async Task<decimal> GetTotalAmountPaidByCustomerAsync(int customerId)
        {
            try
            {
                _logger.LogDebug("Getting total amount paid by customer: {CustomerId}", customerId);
                return await _context.PaymentStatuses
                    .Where(p => p.CustomerId == customerId)
                    .SumAsync(p => p.AmountPaid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total amount paid by customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<decimal> GetTotalAmountRemainingByCustomerAsync(int customerId)
        {
            try
            {
                _logger.LogDebug("Getting total amount remaining by customer: {CustomerId}", customerId);
                return await _context.PaymentStatuses
                    .Where(p => p.CustomerId == customerId)
                    .SumAsync(p => p.AmountRemaining);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total amount remaining by customer: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> HasOutstandingPaymentsAsync(int customerId)
        {
            try
            {
                _logger.LogDebug("Checking outstanding payments for customer: {CustomerId}", customerId);
                return await _context.PaymentStatuses
                    .AnyAsync(p => p.CustomerId == customerId && p.AmountRemaining > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking outstanding payments for customer: {CustomerId}", customerId);
                throw;
            }
        }
    }
}