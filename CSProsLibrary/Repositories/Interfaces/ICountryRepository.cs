using CSProsLibrary.Models;

namespace CSProsLibrary.Repositories.Interfaces;

public interface ICountryRepository
{
    Task<IEnumerable<Country>> ListAllCountries();
    Task<Country?> GetCountryByNameAsync(string countryName);
}