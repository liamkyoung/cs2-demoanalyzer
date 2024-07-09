namespace CSProsLibrary.Services.Interfaces;

public interface IFileManagerService
{
    void ExtractAllDemoArchives();
    bool TryDeleteFile(string filePath);
    IEnumerable<string> GetAllExtractedDemoFilePaths();
}