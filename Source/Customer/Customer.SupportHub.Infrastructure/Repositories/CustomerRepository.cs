using Customer.SupportHub.Domain.Repositories;
using Customer.SupportHub.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Customer.SupportHub.Infrastructure.Repositories;

public class CustomerRepository(ApplicationDbContext context) : ICustomerRepository
{
	public async Task<Domain.Entities.Customer?> FindCustomerByIdAsync(Guid customerid)
	{
		return await context.Customers!.AsNoTracking().SingleOrDefaultAsync(u => u.CustomerId == customerid);
	}

	public async Task<Domain.Entities.Customer?> FindCustomerByEmailAsync(string email)
	{
		return await context.Customers!.AsNoTracking().SingleOrDefaultAsync(u => u.Email == email);
	}

	public async Task UpdateCustomerAsync(Domain.Entities.Customer customer)
	{
		context.Customers!.Update(customer);

		await SaveChangesAsync();
	}

	private async Task SaveChangesAsync()
	{
		await context.SaveChangesAsync();
	}
}