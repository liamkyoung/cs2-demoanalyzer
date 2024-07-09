using CSProsLibrary.Models;
using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Services.Interfaces.Scraping;
using DemoFile;
using DemoFile.Sdk;
using Microsoft.Extensions.Logging;

namespace CSProsLibrary.Services;

public class DemoAnalyzerService : IDemoAnalyzerService
{
        private readonly IPlayerService _playerService;
        private readonly ISkinService _skinService;
        private readonly IWeaponService _weaponService;
        private readonly IGameService _gameService;
        private readonly IScrapingService _scrapingService;
        private readonly IFileManagerService _fileManager;
        private readonly ILogger<DemoAnalyzerService> _logger;

        public DemoAnalyzerService(IPlayerService playerService, ISkinService skinService, IWeaponService weaponService,
                IGameService gameService, IScrapingService scrapingService, IFileManagerService fileManager, ILogger<DemoAnalyzerService> logger)
        {
                _playerService = playerService;
                _skinService = skinService;
                _weaponService = weaponService;
                _gameService = gameService;
                _scrapingService = scrapingService;
                _fileManager = fileManager;
                _logger = logger;
        }
        
        private static readonly string DEMO_EXTRACT_PATH = Environment.GetEnvironmentVariable("DEMO_ANALYZE_DIR")!;
        
        public async Task AnalyzeAllDemos()
        {
                if (!Directory.Exists(DEMO_EXTRACT_PATH))
                {
                        _logger.LogInformation($"Input path does not exist: {DEMO_EXTRACT_PATH}");
                        return;
                }
        
                try
                {
                        var demoFilePaths = _fileManager.GetAllExtractedDemoFilePaths();
                        _logger.LogInformation($"Found {demoFilePaths.Count()} demofiles to parse.");
                        foreach (var filePath in demoFilePaths)
                        {
                                await AnalyzeDemo(filePath);
                        }
                }
                catch (Exception ex)
                {
                        _logger.LogError($"Could not analyze demo: {ex.StackTrace}");
                }
        
                _logger.LogInformation("Analyzed all demo files.");
        }

        public async Task AnalyzeDemo(string filePath)
        {
                var playerToWeaponIdUsage = new Dictionary<string, Dictionary<long, int>>();
                var weaponIdToDemoName = new Dictionary<long, string>();

                // Step 1: Initialize DemoParser
                var demo = new DemoParser();


                // Step 2: Attach Events
                demo.Source1GameEvents.PlayerDeath += e =>
                {
                        long.TryParse(e.WeaponItemid, out var weaponItemId);
                        var playerName = e.Attacker?.PlayerName;

                        weaponIdToDemoName.TryAdd(weaponItemId, e.Weapon);

                        if (playerName == null)
                        {
                                Console.WriteLine("Player name was null at time of kill..");
                        }
                        else if (playerToWeaponIdUsage.TryGetValue(playerName, out var weaponIdUsage))
                        {
                                // New weaponId
                                if (weaponIdUsage.TryGetValue(weaponItemId, out var _))
                                {
                                        playerToWeaponIdUsage[playerName][weaponItemId]++; // Increment number of times skin was used.
                                }
                                else
                                {
                                        playerToWeaponIdUsage[playerName].TryAdd(weaponItemId, 1);
                                }
                        }
                        else
                        {
                                playerToWeaponIdUsage.TryAdd(playerName,
                                        new Dictionary<long, int>() { [weaponItemId] = 1 });
                        }
                };

                demo.Source1GameEvents.RoundEnd += e => { Console.WriteLine($"ROUND END: {e.Winner} | {e.Message}"); };

                // Step 3: Run Parser and Analyze Demo
                try
                {
                        _logger.LogInformation($"Processing file: {filePath}");

                        await using (FileStream file = File.OpenRead(filePath))
                        { 
                                await demo.ReadAllAsync(file);
                        }

                        var gameData = await ValidateEssentialGameData(demo, playerToWeaponIdUsage); // Validation

                        if (gameData == null)
                        {
                                // Delete file because do not want to re-parse demo if cannot be validated
                                // The most likely cause for a validation error is that the game has already been parsed.
                                // Second most likely is that too many matches on player names
                                _fileManager.TryDeleteFile(filePath);

                                return;
                        }

                        foreach (var (gamerTag, weaponItems) in playerToWeaponIdUsage)
                        {
                                var player = await _playerService.GetPlayerByGamerTag(gamerTag);

                                if (player == null)
                                {
                                        _logger.LogError($"Player not found: {gamerTag}");
                                        player = await _playerService.GetPlayerMostSimilarToInGameName(gamerTag);
                                        
                                        if (player == null)
                                        {
                                                _logger.LogError($"Player still not found: {gamerTag}");
                                                continue;
                                        }
                                        
                                        _logger.LogInformation($"[FOUND] Player most similar to {gamerTag} IS {player.GamerTag}");
                                }
                                

                                await ProcessPlayerSkins(player, weaponItems, gameData);
                        }


                        _logger.LogInformation($"Logging Game {gameData.Game.Id} - ({gameData.Map.DemoName})");
                        await _gameService.SetGameAsParsed(gameData.Game.Id);
                        _logger.LogInformation($"[SUCCESSFUL PARSE] Game (id: {gameData.Game.Id}) - ({gameData.Map.DemoName}) has been parsed");

                        _fileManager.TryDeleteFile(filePath);
                        
                }
                catch (Exception e)
                {
                        _logger.LogError($"An exception occured running DemoAnalyzerService: {e.Message}");
                }
                
                
                
                
                
                
                
                // HELPER FUNCTION
                async Task ProcessPlayerSkins(Player player, Dictionary<long, int> weaponItems,
                        GameEssentialData gameData)
                {
                        _logger.LogInformation(
                                $"Processing player's used skins: {player.GamerTag} | {weaponItems.Count} weapons");

                        foreach (var (weaponItemId, numberOfKills) in weaponItems)
                        {
                                var weaponDemoName = weaponIdToDemoName[weaponItemId];

                                if (weaponItemId == 0) // No skin -- Vanilla Item -- Edge case.
                                {
                                        _logger.LogInformation(
                                                $"[{weaponDemoName}] | WeaponItemId = 0, Skipping...");
                                        continue;
                                }

                                _logger.LogInformation(
                                        $"[{player.GamerTag}] | {weaponDemoName} | weaponItemId: {weaponItemId} | Kills: {numberOfKills}");

                                if (await _skinService.HasWeaponItem(weaponItemId))
                                {
                                        await AddCachedSkinAppearance(gameData.Game, weaponItemId,
                                                numberOfKills);
                                }
                                else
                                {
                                        await AddNewSkinAppearanceToDatabase(gameData.Game, weaponDemoName,
                                                weaponItemId, numberOfKills);
                                }
                        }

                        async Task AddCachedSkinAppearance(Game game, long weaponItemId,
                                int numberOfKills)
                        {
                                // Check if weaponItem has associated skin
                                var cachedWeaponItemSkin =
                                        await _skinService.GetSkinByWeaponItemIdAsync(weaponItemId);

                                // Skip adding skin to DB and just log use of skin
                                if (cachedWeaponItemSkin != null)
                                {
                                        _logger.LogInformation(
                                                $"Cache hit: [{cachedWeaponItemSkin.Weapon.Name} | {cachedWeaponItemSkin.Name}] ({weaponItemId})");

                                        await _skinService.AddOrUpdateSkinUsageAsync(
                                                cachedWeaponItemSkin, player, game, numberOfKills);
                                }
                        }

                        async Task<bool> AddNewSkinAppearanceToDatabase(Game game, string weaponDemoName,
                                long weaponItemId, int numberOfKills)
                        {
                                var weaponOfSkin = await _weaponService.GetWeaponFromDemoName(weaponDemoName);

                                if (weaponOfSkin == null)
                                {
                                        _logger.LogError(
                                                $"Invalid weapon, skipping... {weaponDemoName}");
                                        return false;
                                }

                                // Look up skin, store it, log usage
                                var skinInfo = _scrapingService.GetSkinInfoFromWeaponItemId(weaponItemId);

                                if (skinInfo == null)
                                {
                                        await _skinService.AddWeaponItemAsync(weaponItemId,
                                                null); // Add caching for skins that cannot be parsed.
                                        return false; // Not found on csgo.exchange page...
                                }

                                // Skin can exist already (from different player's inventory (different itemId))
                                var skin = await _skinService.GetSkinByNameAndWeaponId(
                                        skinInfo.SkinName, weaponOfSkin);

                                if (skin == null)
                                {
                                        skin = _skinService.GenerateSkinFromData(skinInfo,
                                                weaponOfSkin);
                                        _logger.LogInformation(
                                                $"Created Skin: {skin.Weapon.Name} | {skin.Name}");
                                        await _skinService.AddSkinAsync(skin);
                                }
                                else
                                {
                                        _logger.LogInformation(
                                                $"Found Existing Skin in DB: {skin.Weapon.Name} | {skin.Name}");
                                }

                                await _skinService.AddWeaponItemAsync(weaponItemId, skin);
                                await _skinService.AddOrUpdateSkinUsageAsync(skin, player, game, numberOfKills);

                                return true;
                        }
                }
        }

        

        // Complex method to get teamIds due to difficulty getting accurate IGN names from players
        // Also, stand-ins can make a team hard to categorize.
        // Algorithm here attempts to find the highest # of matches of players on a single team. Takes most common 2 teams.
        private async Task<int[]> GetTeamIds(IEnumerable<string> gamerTags)
        {
                // <TeamId, Frequency>. Take top 2
                var teamIds = new Dictionary<int, int>();
                foreach (var gamerTag in gamerTags)
                {
                        var player = await _playerService.GetPlayerByGamerTag(gamerTag);

                        if (player == null)
                        {
                                _logger.LogError($"Could not find player {gamerTag}");
                                
                                player = await _playerService.GetPlayerMostSimilarToInGameName(gamerTag);
                                        
                                if (player == null)
                                {
                                        _logger.LogError($"Player still not found: {gamerTag}");
                                }
                                else
                                {
                                        _logger.LogInformation($"[FOUND] Player most similar to {gamerTag} IS {player.GamerTag}");
                                        if (teamIds.TryGetValue(player.TeamId!.Value, out var _))
                                        {
                                                teamIds[player.TeamId!.Value]++;
                                                _logger.LogInformation($"Added team {player.TeamId.Value}");
                                        }
                                        else
                                        {
                                                teamIds.TryAdd(player.TeamId!.Value, 1); // First player on team added
                                        }
                                }
                        }
                        else if (player.TeamId != null)
                        {
                                if (teamIds.TryGetValue(player.TeamId!.Value, out var _))
                                {
                                        teamIds[player.TeamId!.Value]++;
                                        _logger.LogInformation($"Added team {player.TeamId.Value}");
                                }
                                else
                                {
                                        teamIds.TryAdd(player.TeamId!.Value, 1); // First player on team added
                                }
                        }
                        else
                        {
                                _logger.LogError($"Player {player.GamerTag}'s team is null...");
                        }
                }

                var top2Teams = teamIds.OrderByDescending(pair => pair.Value).Take(2);

                if (top2Teams.Count() != 2)
                {
                        _logger.LogError($"ERROR: Found number of teams while parsing: {teamIds.Count}");
                        return new int[] { };
                }


                return top2Teams.Select(t => t.Key).ToArray(); // Take only the team IDs
        }

        private async Task<GameEssentialData?> ValidateEssentialGameData(DemoParser demo, Dictionary<string, Dictionary<long, int>> playerToWeaponIdUsage)
        {
                var playerGamerTags = playerToWeaponIdUsage.Keys.ToList();

                playerGamerTags.ForEach(p => _logger.LogInformation($"[FOUND] Player {p}"));

                var teamIds = await GetTeamIds(playerGamerTags);
                var map = await GetDemoMap(demo.FileHeader.MapName);

                if (teamIds.Length == 0 || map == null)
                {
                        return null;
                }

                var game = await _gameService.GetUnparsedGameByTeamsAndMap(teamIds[0], teamIds[1], map.Id);

                if (game == null)
                {
                        _logger.LogError($"ERROR: Game not found... {map.DemoName} | {map.Id}");
                        return null;
                }

                return new GameEssentialData()
                {
                        Game = game,
                        PlayerGamerTags = playerGamerTags,
                        TeamIds = teamIds,
                        Map = map
                };
        }

        private class GameEssentialData
        {
                public required List<string> PlayerGamerTags { get; init; }
                public required int[] TeamIds { get; init; }
                public required Game Game { get; init; }
                public required Map Map { get; init; }
        }

        private async Task<Map?> GetDemoMap(string mapName)
        {
                var map = await _gameService.GetMapByDemoName(mapName);

                if (map == null)
                {
                        _logger.LogError($"ERROR: Map not found: {mapName}"); // Format: de_mirage
                        return null;
                }

                return map;
        }
}