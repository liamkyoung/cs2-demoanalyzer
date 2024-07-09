using CSProsLibrary.Models;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Models;
using CSProsLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace CSProsLibrary.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly ApplicationDbContext _context;
    
    public CountryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Country?> GetCountryByNameAsync(string countryName)
    {
        return await _context.Countries.Where(p => p.Name.ToLower() == countryName).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Country>> ListAllCountries()
    {
        return await _context.Countries.ToListAsync();
    }
}