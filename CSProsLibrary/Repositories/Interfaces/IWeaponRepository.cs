using CSProsLibrary.Models;
using CSProsLibrary.Models.Enums;

namespace CSProsLibrary.Repositories.Interfaces;

public interface IWeaponRepository
{
    Task<IEnumerable<Weapon>> GetAllWeapons();
    Task<IEnumerable<Weapon>?> GetAllWeaponsForSide(TeamExclusivity side);
    Task<IEnumerable<Weapon>?> GetAllWeaponsByType(WeaponType type);
    Task<Weapon?> GetWeaponByName(string weaponName); // TODO: Change to enum?
    Task<Weapon?> GetWeaponByDemoName(string demoName);
    Task<Weapon?> GetWeaponByNormalizedName(string normalizedName);
}