using CSProsLibrary.Services.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpCompress.Readers;

namespace CSProsLibrary.Services;

// Service used to monitor for new demos added from DemoDownloader
public class FileWatcherBackgroundService : BackgroundService
{
    private IServiceProvider _serviceProvider;
    private readonly ILogger<FileWatcherBackgroundService> _logger;
    private FileSystemWatcher _demoWatcher;
    private FileSystemWatcher _compressedDemoWatcher;
    private static readonly string DEMO_EXTRACT_PATH = Environment.GetEnvironmentVariable("DEMO_ANALYZE_DIR")!;
    private static readonly string DEMO_DOWNLOAD_PATH = Environment.GetEnvironmentVariable("DEMO_DOWNLOAD_DIR")!;
    
    public FileWatcherBackgroundService(ILogger<FileWatcherBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _demoWatcher = new FileSystemWatcher(DEMO_EXTRACT_PATH)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
        };

        _compressedDemoWatcher = new FileSystemWatcher(DEMO_DOWNLOAD_PATH)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
        };

        // Both will be attached to the OnCreated function
        _compressedDemoWatcher.Created += HandleExtractingDemos;
        _compressedDemoWatcher.EnableRaisingEvents = true;
        
        _demoWatcher.Created += HandleAnalyzeDemo;
        _demoWatcher.EnableRaisingEvents = true;

        return Task.CompletedTask;
    }

    private async void HandleAnalyzeDemo(object sender, FileSystemEventArgs e)
    {
        WaitUntilFileIsReady(e.FullPath); // Waits up to 200 seconds until demo is ready to be parsed
        // Logic to handle the file added event
        _logger.LogInformation($"Analyzing Demo: {e.FullPath}");

        var scope = _serviceProvider.CreateScope();
        var demoService = scope.ServiceProvider.GetRequiredService<IDemoAnalyzerService>();
        
        await Task.Run(() => demoService.AnalyzeDemo(e.FullPath)); // Fire and forget
    }

    private void HandleExtractingDemos(object sender, FileSystemEventArgs e)
    {
        if (!Directory.Exists(DEMO_DOWNLOAD_PATH))
        {
            _logger.LogInformation($"Input path does not exist: {DEMO_DOWNLOAD_PATH}");
            return;
        }

        try
        {
            foreach (var fileName in Directory.GetFiles(DEMO_DOWNLOAD_PATH, "*.rar"))
            {
                if (!fileName.EndsWith(".rar"))
                {
                    continue;
                }
                
                using (var archive = ArchiveFactory.Open(fileName))
                {
                    _logger.LogInformation($"Extracting {fileName}...");
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            entry.WriteToDirectory(DEMO_EXTRACT_PATH, GenerateExtractionOptions());
                        }
                    }
                }
                
                
                File.Delete(fileName);
                _logger.LogInformation($"[Deleted] Archive {fileName}...");
            }
            
            _logger.LogInformation("Finished extracting...");
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Tried to open the compressed demo folder, ran into an exception: {e.InnerException} | {e.Message}");
            _logger.LogError(ex.StackTrace);
        }

        ExtractionOptions GenerateExtractionOptions()
        {
            return new ExtractionOptions()
            {
                Overwrite = true,
                ExtractFullPath = true,
                PreserveFileTime = true,
                PreserveAttributes = true
            };
        }
    }

    private bool WaitUntilFileIsReady(string fileName)
    {
        var attempts = 0;
        while (true)
        {
            try
            {
                // Checks if file can be opened. Will throw exception if fails.
                using (Stream stream = new FileStream(fileName, FileMode.Open)) { } 
                break;
            } catch
            {
                _logger.LogInformation($"[Attempt: {attempts}] Waiting for demo to be ready...");
                if (attempts >= 200) return false;
                attempts++;
            }

            Thread.Sleep(1000);
        }

        return true;
    }
    
    public override void Dispose()
    {
        _demoWatcher.Dispose();
        _compressedDemoWatcher.Dispose();
        base.Dispose();
    }
}
