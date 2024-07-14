using CS2DemoAnalyzer;
using CSProsLibrary;
using CSProsLibrary.Repositories;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services;
using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Services.Interfaces.Scraping;
using CSProsLibrary.Services.Scraping;
using CSProsLibrary.Services.Scraping.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();

        var demoDownloader = host.Services.GetService<IDemoDownloaderService>();

        await demoDownloader.Start();
    }
    
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                EnvironmentHelper.SetEnvironmentVariables();
                
                // Configuration
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                // Register ApplicationDbContext
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("CSPROS_PROD")));
                
                // Web Scraping Services
                services.AddTransient<IScrapingService, ScrapingService>();
                
                services.AddSingleton<IWebDriver, FirefoxDriver>(provider =>
                {
                    var service = FirefoxDriverService.CreateDefaultService("/usr/local/bin");
                    FirefoxOptions options = new FirefoxOptions();
                    options.AddArgument("--headless");
                    options.EnableDownloads = true;
                    options.ScriptTimeout = TimeSpan.FromSeconds(30);
                    options.SetPreference("browser.download.folderList", 2);
                    options.SetPreference("browser.download.manager.showWhenStarting", false);
                    options.SetPreference("browser.download.dir", Environment.GetEnvironmentVariable("DEMO_DOWNLOAD_DIR"));
                    options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/vnd.rar");
                    // Configure your Firefox options here, if necessary
    
                    return new FirefoxDriver(service, options);
                });
                
                services.AddTransient<IPlayerService, PlayerService>();
                services.AddTransient<IGameService, GameService>();
                services.AddTransient<ISkinService, SkinService>();
                services.AddTransient<ITeamService, TeamService>();
                services.AddTransient<ICountryService, CountryService>();
                services.AddTransient<IWeaponService, WeaponService>();
                services.AddHostedService<FileWatcherBackgroundService>();
                services.AddTransient<IDemoAnalyzerService, DemoAnalyzerService>();
                services.AddTransient<IDemoDownloaderService, DemoDownloaderService>();

                // Pages
                services.AddTransient<MatchPage>();
                services.AddTransient<PlayerPage>();
                services.AddTransient<ResultsListPage>();
                services.AddTransient<TeamPage>();
                
                // Repositories
                services.AddScoped<IPlayerRepository, PlayerRepository>();
                services.AddScoped<IWeaponRepository, WeaponRepository>();
                services.AddScoped<ISkinRepository, SkinRepository>();
                services.AddScoped<ITeamRepository, TeamRepository>();
                services.AddScoped<IGameRepository, GameRepository>();
                services.AddScoped<IMapRepository, MapRepository>();
                services.AddScoped<ICountryRepository, CountryRepository>();
                services.AddScoped<ISkinUsageRepository, SkinUsageRepository>();
                services.AddScoped<IWeaponRepository, WeaponRepository>();
                

                // Add logging
                services.AddLogging(configure => configure.AddConsole());
            });
}
