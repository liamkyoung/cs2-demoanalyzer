using API;
using CSProsLibrary;
using CSProsLibrary.Repositories;
using CSProsLibrary.Repositories.Interfaces;
using CSProsLibrary.Services;
using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Services.Interfaces.Scraping;
using CSProsLibrary.Services.Scraping;
using CSProsLibrary.Services.Scraping.Pages;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

// Must create static class to set env variables necessary
// (Could also move env variables into zsh/bash)
// EnvironmentHelper.SetEnvironmentVariables();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddTransient<IPlayerService, PlayerService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<ISkinService, SkinService>();
builder.Services.AddTransient<ITeamService, TeamService>();
builder.Services.AddTransient<ICountryService, CountryService>();
builder.Services.AddTransient<IWeaponService, WeaponService>();

builder.Services.AddHostedService<FileWatcherBackgroundService>();
builder.Services.AddTransient<IDemoAnalyzerService, DemoAnalyzerService>();
builder.Services.AddTransient<IDemoDownloaderService, DemoDownloaderService>();

// Pages
builder.Services.AddTransient<IScrapingService, ScrapingService>();
builder.Services.AddTransient<MatchPage>();
builder.Services.AddTransient<PlayerPage>();
builder.Services.AddTransient<ResultsListPage>();
builder.Services.AddTransient<TeamPage>();

// Web Scraping Services
builder.Services.AddSingleton<IWebDriver, FirefoxDriver>(provider =>
{
    FirefoxOptions options = new FirefoxOptions();
    options.EnableDownloads = true;
    options.ScriptTimeout = TimeSpan.FromSeconds(30);
    options.SetPreference("browser.download.folderList", 2);
    options.SetPreference("browser.download.manager.showWhenStarting", false);
    options.SetPreference("browser.download.dir", Environment.GetEnvironmentVariable("DEMO_DOWNLOAD_DIR"));
    options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/vnd.rar");
    // Configure your Firefox options here, if necessary
    
    return new FirefoxDriver(options);
});

// Repositories
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IWeaponRepository, WeaponRepository>();
builder.Services.AddScoped<ISkinRepository, SkinRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IMapRepository, MapRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ISkinUsageRepository, SkinUsageRepository>();
builder.Services.AddScoped<IWeaponRepository, WeaponRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.UseCors(b =>
    b.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

app.Run();