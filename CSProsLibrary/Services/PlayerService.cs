using CSProsLibrary.Models;
using CSProsLibrary.Models.Dtos;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Services.Scraping.Pages;
using CSProsLibrary.Utils;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSProsLibrary.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ISkinUsageRepository _skinUsageRepository;
    private readonly ISkinRepository _skinRepository;
    private readonly IWeaponRepository _weaponRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly ITeamService _teamService;
    private readonly PlayerPage _playerPage;
    private readonly IWebDriver _webDriver;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(IWebDriver webDriver,
        ICountryRepository countryRepository,
        PlayerPage playerPage,
        IPlayerRepository playerRepository,
        ISkinRepository skinRepository,
        ISkinUsageRepository skinUsageRepository,
        IWeaponRepository weaponRepository,
        ITeamService teamService,
        ILogger<PlayerService> logger)
    {
        _webDriver = webDriver;
        _playerPage = playerPage;
        _countryRepository = countryRepository;
        _playerRepository = playerRepository;
        _skinUsageRepository = skinUsageRepository;
        _skinRepository = skinRepository;
        _weaponRepository = weaponRepository;
        _teamService = teamService;
        _logger = logger;
    }

    public async Task<IEnumerable<Player>> GetTrendingPlayers(TimeSpan timePeriod, int limit)
    {
        return await _playerRepository.GetTrendingPlayers(timePeriod, limit);
    }
    
    public async Task<Player?> GetPlayerByGamerTag(string gamerTag)
    {
        return await _playerRepository.GetByGamerTagAsync(gamerTag);
    }
    
    public async Task<Player?> GetPlayerByHltvLink(string hltvLink)
    {
        try
        {
            var player = await _playerRepository.GetByHltvLinkAsync(hltvLink);
            return player;
        }
        catch (Exception e)
        {
            _logger.LogInformation($"Could not find player by hltv link: {hltvLink}");
        }

        return null;
    }

    // TODO: Clean
    private PlayerSocials GeneratePlayerSocials(PlayerProfileDto playerProfileDto)
    {
        return new PlayerSocials()
        {
            Instagram = playerProfileDto.InstagramProfile,
            Twitch = playerProfileDto.TwitchProfile,
            Twitter = playerProfileDto.TwitterProfile
        };
    }
    
    public async Task<Player> GeneratePlayer(PlayerProfileDto playerProfileDto)
    {
        Country? country = null;
        Team? team = null;
        
        try
        {
            country = await _countryRepository.GetCountryByNameAsync(playerProfileDto.CountryName);

            if (!string.IsNullOrEmpty(playerProfileDto.TeamName))
            {
                team = await _teamService.GetTeamByName(playerProfileDto.TeamName);
            }
            
        }
        catch (Exception e)
        {
            _logger.LogError($"Error generating player: {playerProfileDto.GamerTag}\n" +
                             $"Country: {playerProfileDto.CountryName}" +
                             $"Team: {playerProfileDto.TeamName}");
            
        }

        return new Player()
        {
            Name = playerProfileDto.RealName,
            GamerTag = playerProfileDto.GamerTag,
            HltvLink = playerProfileDto.HltvLink,
            Age = playerProfileDto.Age,
            Country = country,
            ProfileImage = playerProfileDto.ProfileImage,
            Team = team,
            TimeSinceLastUpdated = DateTimeOffset.UtcNow
        };
    }
    
    public async Task<IEnumerable<Player>> GetPlayersByHltvLinks(IEnumerable<string> playerHltvLinks)
    {
        var players = new List<Player>();
        foreach (var hltvLink in playerHltvLinks)
        {
            var player = await GetPlayerByHltvLink(hltvLink);

            if (player != null && (player.TimeSinceLastUpdated + TimeSpan.FromDays(7)) > DateTimeOffset.Now)
            {
                players.Add(player);
            }
            else
            {
                var playerDto = await ParsePlayerProfile(hltvLink);

                if (playerDto == null)
                {
                    _logger.LogError($"Could not parse player info: {hltvLink}");
                    continue;
                }
                    
                players.Add(await GeneratePlayer(playerDto));
            }
        }

        return players;
    }

    public async Task<Dictionary<Weapon, Skin?>> GetPlayerSkinsForGame(Player player, Game game)
    {
        var skinIds = await _skinUsageRepository.GetAllSkinIdsUsedInGameByPlayer(player, game);
        var skinMap = new Dictionary<Weapon, Skin?>();
        
        if (skinIds == null)
        {
            return skinMap;
        }
        
        var allWeapons = (await _weaponRepository.GetAllWeapons()).ToList();
        foreach (var id in skinIds)
        {
            var skin = await _skinRepository.GetSkinByIdAsync(id);

            if (skin == null)
            {
                throw new Exception($"Skin Not Found... ID: {id}");
            }

            skinMap.TryAdd(skin.Weapon, skin);
            allWeapons.Remove(skin.Weapon); // Removing so can add empty values into dict after
        }

        // Will have the same list of weapons returned, but with empty skin values for skins not used.
        foreach (var weapon in allWeapons)
        {
            skinMap.TryAdd(weapon, null);
        }

        return skinMap;
    }

    public async Task<IEnumerable<Player>> GetTeammates(Player player)
    {
        var teamId = player.TeamId;
        
        if (teamId == null)
        {
            return Enumerable.Empty<Player>();
        }
        
        var players = await _teamService.GetPlayersOnTeamById(teamId.Value);

        if (!players.Any())
        {
            return Enumerable.Empty<Player>();
        }

        var p = players.ToList();
        p.Remove(player);
        return p;
    }
    
    // Retrieves the most used skins for a player
    public async Task<Dictionary<Weapon, Skin?>> GetMostUsedSkinsForPlayer(Player player)
    {
        var skinFrequencies = (await _skinUsageRepository.GetSkinFrequenciesForPlayer(player)).ToList();
        var skinMap = new Dictionary<Weapon, Skin?>();

        if (!skinFrequencies.Any())
        {
            return skinMap;
        }
        
        var weapons = (await _weaponRepository.GetAllWeapons()).ToList();

        foreach (var dto in skinFrequencies)
        {
            if (weapons.Count == 0)
            {
                break; // Has added the most popular skins for every weapon since sorted list.
            }

            var skinId = dto.SkinId;

            var skin = await _skinRepository.GetSkinByIdAsync(skinId);

            if (skin == null)
            {
                continue;
            }

            var skinWeapon = skin.Weapon;
            
            // If has already been added
            if (!skinMap.ContainsKey(skinWeapon))
            {
                skinMap.TryAdd(skinWeapon, skin);
                weapons.Remove(skinWeapon);
            }
        }
        
        // Will have the same list of weapons returned, but with empty skin values for skins not used.
        foreach (var weapon in weapons)
        {
            skinMap.TryAdd(weapon, null);
        }
        
        return skinMap;
    }
    
    public async Task<IEnumerable<SkinFrequencyDto>?> GetPlayersMostUsedSkinsForWeapon(Player player, Weapon weapon)
    {
        var skinFrequencies = await _skinUsageRepository.GetSkinFrequenciesForPlayer(player);
        
        return skinFrequencies.ToList().Where(skinGroup => skinGroup.WeaponId == weapon.Id);
    }

    public async Task<PlayerProfileDto?> ParsePlayerProfile(string playerProfileHref)
    {
        _playerPage.GoToPage(playerProfileHref);
        return _playerPage.GetPlayerData();
    }

    public async Task<bool> AddPlayer(Player player)
    {
        try
        {
            await _playerRepository.AddAsync(player);
            _logger.LogInformation($"Added player to DB: {player}");
        }
        catch (Exception e)
        {
            _logger.LogError($"Could not add player to DB: {player}");
            return false;
        }

        return true;
    }
    
    public async Task<bool> AddPlayer(PlayerProfileDto playerProfile)
    {
        var player = await GeneratePlayer(playerProfile);

        return await AddPlayer(player);
    }

    public async Task<bool> AddPlayersByHltvLinks(IEnumerable<string> playerLinks)
    {
        try
        {
            var players = await GetPlayersByHltvLinks(playerLinks);

            if (!players.Any())
            {
                return false;
            }
            
            // issue with adding -- some being duplicates and some not
            await _playerRepository.AddRangeAsync(players); 
            _logger.LogInformation($"Added {players.Count()} players to DB.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Could not add players to DB... | {e.StackTrace}");
            
            return false;
        }

        return true;
    }

    public async Task<bool> UpdatePlayer(PlayerProfileDto playerProfile)
    {
        var player = await _playerRepository.GetByHltvLinkAsync(playerProfile.HltvLink);

        if (player == null)
        {
            _logger.LogError($"Could not find player {playerProfile.GamerTag} in DB");
            return false;
        }

        try
        {
            await _playerRepository.UpdateAsync(player);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to update player {playerProfile.GamerTag}");
        }

        return false;
    }
    
    public async Task<bool> UpdatePlayer(Player player)
    {
        try
        {
            await _playerRepository.UpdateAsync(player);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to update player {player.GamerTag}");
        }

        return false;
    }

    public async Task<Player?> GetPlayerMostSimilarToInGameName(string inGameName)
    {
        var allGamerTags = await _playerRepository.GetAllPlayerGamerTags();
        var minDifference = int.MaxValue;
        var foundGamerTag = "";

        foreach (var gamerTag in allGamerTags)
        {
            var calculatedDiff = StringSimilarityCalculator.Compute(inGameName, gamerTag);
            
            if (calculatedDiff == 0) // Same string -- same player
            {
                foundGamerTag = gamerTag;
                break;
            }
            else if (calculatedDiff < minDifference)
            {
                minDifference = calculatedDiff;
                foundGamerTag = gamerTag;
            }
        }

        return await _playerRepository.GetByGamerTagAsync(foundGamerTag);
    }
    
}