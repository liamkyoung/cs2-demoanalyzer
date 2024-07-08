using CSProsLibrary.Models;

namespace CSProsLibrary.Services.Interfaces;

public interface ICountryService
{
    Task<Country?> GetCountryByName(string countryName);
    Task<IEnumerable<Country>> ListAllCountries();
}