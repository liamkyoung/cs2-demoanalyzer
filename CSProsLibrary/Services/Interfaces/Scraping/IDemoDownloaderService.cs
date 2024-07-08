namespace CSProsLibrary.Services.Interfaces.Scraping;

public interface IDemoDownloaderService
{
    Task Start();
    void ManuallyExtractDemos();
}