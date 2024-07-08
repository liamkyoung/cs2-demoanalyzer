using System.Linq;
using CSProsLibrary.Models;
using CSProsLibrary.Models.Enums;
using CSProsLibrary.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSProsLibrary.Repositories;

public class WeaponRepository : IWeaponRepository
{
    private readonly IApplicationDbContext _context;

    public WeaponRepository(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Weapon>> GetAllWeapons()
    {
        return await _context.Weapons.ToListAsync();
    }

    public async Task<IEnumerable<Weapon>?> GetAllWeaponsForSide(TeamExclusivity side)
    {
        return await _context.Weapons.Where(weapon => weapon.TeamAssigned == side).ToListAsync();
    }

    public async Task<IEnumerable<Weapon>?> GetAllWeaponsByType(WeaponType weaponType)
    {
        return await _context.Weapons.Where(weapon => weapon.Type == weaponType).ToListAsync();
    }

    public async Task<Weapon?> GetWeaponByName(string weaponName)
    {
        return await _context.Weapons.Where(weapon => weapon.Name == weaponName).FirstOrDefaultAsync();
    }
    
    public async Task<Weapon?> GetWeaponByNormalizedName(string normalizedName)
    {
        return await _context.Weapons.Where(weapon => weapon.NormalizedName == normalizedName).FirstOrDefaultAsync();
    }
    
    public async Task<Weapon?> GetWeaponByDemoName(string demoName)
    {
        return await _context.Weapons.Where(weapon => weapon.DemoName == demoName).FirstOrDefaultAsync();
    }

    public async Task<Weapon?> GetByIdAsync(int id)
    {
        return await _context.Weapons.FindAsync(id);
    }

    public async Task AddAsync(Weapon weapon)
    {
        _context.Weapons.Add(weapon);
        await _context.SaveChangesAsync();
    }
}