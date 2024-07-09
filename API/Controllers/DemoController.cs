using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Services.Interfaces.Scraping;
using CSProsLibrary.Services.Interfaces;
using CSProsLibrary.Services.Interfaces.Scraping;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class DemoController : ControllerBase
{
    private readonly IDemoAnalyzerService _demoAnalyzer;
    private readonly IDemoDownloaderService _demoDownloader;
    private readonly IScrapingService _scrapingService;
    private readonly ISkinService _skinService;
    private readonly IWeaponService _weaponService;
    
    public DemoController(IDemoAnalyzerService demoAnalyzer, IDemoDownloaderService demoDownloader, ISkinService skinService, IWeaponService weaponService, IScrapingService scrapingService)
    {
        _demoAnalyzer = demoAnalyzer;
        _demoDownloader = demoDownloader;
        _skinService = skinService;
        _weaponService = weaponService;
        _scrapingService = scrapingService;
    }
    
    [HttpGet("AnalyzeExampleDemo")]
    public async Task<IActionResult> AnalyzeDemo()
    {
        await _demoAnalyzer.AnalyzeDemo("Data/ExampleDemos/faze-navi-m1.dem");
        return Ok();
    }
    
    [HttpGet("AnalyzeDemo")]
    public async Task<IActionResult> AnalyzeSpecificDemo(string demoName)
    {
        await _demoAnalyzer.AnalyzeDemo($"Data/Demos/Extracted/{demoName}");
        return Ok();
    }
    
    [HttpGet("TryAnalyzeAllDemos")]
    public async Task<IActionResult> TryAnalyzeAllDemos()
    {
        await _demoAnalyzer.AnalyzeAllDemos();
        return Ok();
    }
    
    [HttpGet("DownloadDemos")]
    public async Task<IActionResult> DownloadDemos()
    {
        await _demoDownloader.Start();
        return Ok();
    }
    
    [HttpGet("Extract")]
    public async Task<IActionResult> ManuallyExtractDemo()
    {
        _demoDownloader.ManuallyExtractDemos();
        return Ok();
    }

}