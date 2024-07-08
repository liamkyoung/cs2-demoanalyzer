using CSProsLibrary.Models;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSProsLibrary.Services;

public class CountryService : ICountryService
{
    private readonly ILogger<ICountryService> _logger;
    private readonly ICountryRepository _countryRepository;
    
    public CountryService(ILogger<ICountryService> logger, ICountryRepository countryRepository)
    {
        _logger = logger;
        _countryRepository = countryRepository;
    }

    public async Task<Country?> GetCountryByName(string countryName)
    {
        if (string.IsNullOrEmpty(countryName))
        {
            _logger.LogError("Could not find country: name not provided.");
        }
        
        return await _countryRepository.GetCountryByNameAsync(countryName.ToLower());
    }

    public async Task<IEnumerable<Country>> ListAllCountries()
    {
        return await _countryRepository.ListAllCountries();
    }
}