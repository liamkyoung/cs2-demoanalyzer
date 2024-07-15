#!/bin/bash
echo "[CSPROS] Launching Demo Analyzer..."
cd ~/Desktop/Projects/cs2-demoanalyzer/CS2DemoAnalyzer
export DEMO_DOWNLOAD_DIR=~/Desktop/Demos_Download
export DEMO_ANALYZE_DIR=~/Desktop/Demos_Extracted
~/.dotnet/dotnet run
echo "[CSPROS] Finished Analyzer Job."
