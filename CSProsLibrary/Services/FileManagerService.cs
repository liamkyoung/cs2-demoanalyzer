using CSProsLibrary.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace CSProsLibrary.Services;

public class FileManagerService : IFileManagerService
{
    private readonly ILogger<FileManagerService> _logger;
    private static readonly string DEMO_EXTRACT_PATH = Environment.GetEnvironmentVariable("DEMO_ANALYZE_DIR")!;
    private static readonly string DEMO_DOWNLOAD_PATH = Environment.GetEnvironmentVariable("DEMO_DOWNLOAD_DIR")!;
    

    public FileManagerService(ILogger<FileManagerService> logger)
    {
        _logger = logger;
    }
    
    
    public void ExtractAllDemoArchives()
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

    public bool TryDeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation($"{filePath} deleted successfully.");
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Error deleting file: {filePath}");
        }

        return false;
    }

    public IEnumerable<string> GetAllExtractedDemoFilePaths()
    {
        var files = Directory.GetFiles(DEMO_EXTRACT_PATH, "*.dem");

        return files.Where(f => f.EndsWith(".dem"));
    }
}