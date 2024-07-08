namespace CSProsLibrary.Services.Interfaces;

public interface IDemoAnalyzerService
{
    Task AnalyzeDemo(string filePath);
    Task AnalyzeAllDemos();
}