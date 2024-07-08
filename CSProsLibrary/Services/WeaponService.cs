using CSProsLibrary.Models;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSProsLibrary.Services;

public class WeaponService : IWeaponService
{
        private readonly ILogger<IWeaponService> _logger;
        private readonly IWeaponRepository _weaponRepository;
    
        public WeaponService(ILogger<IWeaponService> logger, IWeaponRepository weaponRepository)
        {
            _logger = logger;
            _weaponRepository = weaponRepository;
        }

        public async Task<Weapon?> GetWeaponFromDemoName(string demoName)
        {
            return await _weaponRepository.GetWeaponByDemoName(demoName);
        }
        
        public async Task<Weapon?> GetWeaponFromNormalizedName(string normalizedName)
        {
            return await _weaponRepository.GetWeaponByNormalizedName(normalizedName);
        }
}