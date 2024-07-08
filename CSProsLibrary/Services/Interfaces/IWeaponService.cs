using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;

namespace CSProsLibrary.Services.Interfaces;

public interface IWeaponService
{
    Task<Weapon?> GetWeaponFromDemoName(string weaponName);
    Task<Weapon?> GetWeaponFromNormalizedName(string normalizedName);
}