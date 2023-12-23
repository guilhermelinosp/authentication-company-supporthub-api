using Company.SupportHub.Domain.Entities;

namespace Company.SupportHub.Domain.Repositories;

public interface ICompanyRepository
{
	Task<Entities.Company?> FindCompanyByIdAsync(Guid companyid);
	Task<Entities.Company?> FindCompanyByEmailAsync(string email);
	Task<Entities.Company?> FindCompanyByPhoneAsync(string phone);
	Task<Entities.Company?> FindCompanyByCnpjAsync(string cnpj);
	Task CreateCompanyAsync(Entities.Company company);
	Task UpdateCompanyAsync(Entities.Company company);
	Task DeleteCompanyAsync(Entities.Company company);
}