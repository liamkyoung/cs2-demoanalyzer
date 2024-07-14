namespace CS2DemoAnalyzer;

public static class EnvironmentHelper
{
    public static void SetEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("DEMO_DOWNLOAD_DIR", "~/Desktop/Demos_Download");
        Environment.SetEnvironmentVariable("DEMO_ANALYZE_DIR", "~/Desktop/Demos_Extracted");
    }
}
