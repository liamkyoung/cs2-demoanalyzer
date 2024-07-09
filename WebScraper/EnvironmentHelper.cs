namespace CS2DemoAnalyzer;

public static class EnvironmentHelper
{
    public static void SetEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("DEMO_DOWNLOAD_DIR", "/Users/lky/Desktop/WebProjects/cspros-main/CSPros-Backend/CSPros-Library/CS2DemoAnalyzer/Data/Demos/Compressed");
        Environment.SetEnvironmentVariable("DEMO_ANALYZE_DIR", "/Users/lky/Desktop/WebProjects/cspros-main/CSPros-Backend/CSPros-Library/CS2DemoAnalyzer/Data/Demos/Extracted");
    }
}