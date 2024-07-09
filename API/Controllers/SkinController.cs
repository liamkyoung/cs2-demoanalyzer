using CSProsLibrary.Models.Dtos.Viewmodels;
using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Services.Interfaces.Scraping;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SkinController : ControllerBase
{
    private readonly ISkinService _skinService;
    private readonly IWeaponService _weaponService;
    private readonly IPlayerService _playerService;
    private readonly ICountryService _countryService;
    
    public SkinController(ISkinService skinService, IWeaponService weaponService, IPlayerService playerService, ICountryService countryService)
    {
        _playerService = playerService;
        _skinService = skinService;
        _weaponService = weaponService;
        _countryService = countryService;
    }
    
    
    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        var countries = await _countryService.ListAllCountries();

        return Ok(countries);
    }
    
    
    [HttpGet("GetAllSkinsForWeapon")]
    public async Task<IActionResult> GetAllSkinsForWeapon(string weaponName)
    {
        var weapon = await _weaponService.GetWeaponFromNormalizedName(weaponName);

        if (weapon == null)
        {
            return NotFound("Weapon not found...");
        }
        
        var skins = await _skinService.GetAllSkinsForWeapon(weapon);
        return Ok(skins.Select(skin => SkinProfile.Convert(skin)));
    }
    
    [HttpGet("GetPopularSkinsForAllWeapons")]
    public async Task<IActionResult> GetPopularSkinsForAllWeapons()
    {
        var popularSkins = await _skinService.GetPopularSkinsForAllWeapons();
        return Ok(popularSkins);
    }
    
    [HttpGet("GetPopularSkinsForWeapon")]
    public async Task<IActionResult> GetPopularSkinsForWeapon(string weaponName)
    {
        var weapon = await _weaponService.GetWeaponFromNormalizedName(weaponName);

        if (weapon == null)
        {
            return NotFound("Weapon not found...");
        }
        
        var popularSkins = await _skinService.GetPopularSkinsForWeapon(weapon);
        return Ok(popularSkins.Select(skin => SkinProfile.Convert(skin)));
    }
    
    [HttpGet("GetPopularSkinsForPlayerAndWeapon")]
    public async Task<IActionResult> GetPopularSkinsForPlayerAndWeapon(string playerName, string weaponName)
    {
        var weapon = await _weaponService.GetWeaponFromNormalizedName(weaponName);
        var player = await _playerService.GetPlayerByGamerTag(playerName);

        if (weapon == null || player == null)
        {
            return NotFound();
        }
        
        var popularSkins = await _skinService.GetMostUsedSkins(weapon, player);
        return Ok(popularSkins);
    }
    
    [HttpGet("GetAllSkinsByPlayerName")]
    public async Task<IActionResult> GetSkinsByPlayerName(string playerName)
    {
        var player = await _playerService.GetPlayerByGamerTag(playerName);

        if (player == null)
        {
            return NotFound("Player not found");
        }

        var skins = await _skinService.GetAllSkinsUsedByPlayer(player);
        
        return Ok(skins.Select(skin => SkinProfile.Convert(skin)));
    }
    
    [HttpGet("PlayersUsingSkin")]
    public async Task<IActionResult> MostPopularPlayersWithSkin(string skinName, string weaponName)
    {
        if (string.IsNullOrEmpty(skinName) || string.IsNullOrEmpty(weaponName))
        {
            return BadRequest("Invalid inputs of skinName and weaponDemoName");
        }
        
        var weapon = await _weaponService.GetWeaponFromNormalizedName(weaponName);

        if (weapon == null)
        {
            return NotFound("Weapon not found...");
        }
        
        var skin = await _skinService.GetSkinByNameAndWeaponId(skinName, weapon);

        if (skin == null)
        {
            return NotFound("Skin not found...");
        }
        
        var skinInfo = await _skinService.GetMostPopularPlayersUsingSkin(skin);
        return Ok(skinInfo);
    }
    
    [HttpGet("TrendingSkins")]
    public async Task<IActionResult> GetTrendingSkins()
    {
        var skins = await _skinService.GetTrendingSkins(TimeSpan.FromDays(30));
        return Ok(skins.Select(s => SkinProfile.Convert(s)));
    }
}