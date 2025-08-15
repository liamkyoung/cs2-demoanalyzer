# 🎯 CS Pros - Professional Counter-Strike Player & Skin Tracker

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-14-000000?style=flat-square&logo=nextdotjs)](https://nextjs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15.6-336791?style=flat-square&logo=postgresql)](https://postgresql.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6?style=flat-square&logo=typescript)](https://typescriptlang.org/)

> A comprehensive data platform that tracks professional Counter-Strike 2 players and their in-game weapon skins through automated demo file analysis and web scraping.

## 🌟 Project Overview

CS Pros fills a unique gap in the esports ecosystem by providing real-time tracking of professional CS2 players' weapon skin preferences. Unlike existing platforms that offer limited or outdated information, this system automatically analyzes professional match demo files to maintain an up-to-date database of player statistics and skin usage patterns.

### ✨ Key Features

- **Real-time Demo Analysis**: Automated parsing of professional CS2 match demos using advanced .NET libraries
- **Comprehensive Player Database**: Tracks players, teams, and match statistics across Tier-1 professional games
- **Skin Usage Analytics**: Monitors weapon skin preferences with frequency tracking and trend analysis
- **Automated Data Pipeline**: Continuous scraping and updating with minimal manual intervention
- **Modern Web Interface**: Clean, responsive frontend for data visualization and player lookup

### 📊 Technical Achievements

- **99%+ Match Coverage**: Processes nearly every Tier-1 CS2 professional game
- **98% Parse Success Rate**: Robust demo file parsing with comprehensive error handling
- **Real-time Updates**: Data refreshed every few hours as new matches are played
- **Scalable Architecture**: Designed to handle growing data volumes and user traffic

## 🏗️ System Architecture

![Architecture Diagram](https://github.com/liamkyoung/cs2-demoanalyzer/assets/52087920/63553649-7b91-4f20-bd03-a1a8cd77e538)


### 🛠️ Technology Stack

**Frontend**
[Frontend Code Here](https://github.com/liamkyoung/cspros-frontend)
- Next.js 14 with TypeScript
- Tailwind CSS for modern, responsive design
- Deployed on Vercel for easy maintenance and optimal performance

**Backend**
- .NET 8 Web API with Entity Framework Core
- RESTful architecture with comprehensive error handling
- Hosted on AWS EC2 with Nginx reverse proxy

**Database**
- PostgreSQL 15.6 for robust data persistence
- Optimized schema for complex player/match relationships
- Hosted on AWS RDS for reliability and scalability

**Data Processing**
- Custom demo file parser using DemoFile-Net library
- Selenium-based web scraping for match data
- Automated pipeline running on Raspberry Pi cluster

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm
- PostgreSQL 15.6+
- Firefox browser (for web scraping)
- GeckoDriver

### Quick Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/liamkyoung/cs2-demoanalyzer.git
   cd cs2-demoanalyzer
   ```

2. **Database Setup**
   ```bash
   # Create PostgreSQL database named 'cspros'
   # Update connection strings in appsettings.json
   dotnet ef database update --project ./CSProsLibrary --startup-project ./CS2DemoAnalyzer
   ```

3. **Environment Configuration**
   ```json
   {
     "ConnectionStrings": {
       "CSPROS_PROD": "Host=server_address;Database=cspros;Username=cspros;Password=YOUR_PASSWORD",
       "CSPROS_DEV": "Host=localhost;Database=cspros;Username=cspros;Password=YOUR_PASSWORD"
     }
   }
   ```

4. **Set Environment Variables**
   ```bash
   export DEMO_DOWNLOAD_DIR=/path/to/downloads
   export DEMO_ANALYZE_DIR=/path/to/analysis
   export NEXT_PUBLIC_BACKEND_HOST=https://localhost:7280
   ```

5. **Run the Application**
   ```bash
   # Start the API
   dotnet run --project ./CSProsAPI
   
   # Start the frontend (in separate terminal)
   cd frontend && npm run dev
   ```

## 📈 Data Pipeline

### Automated Workflow

1. **Match Discovery**: Web scraper identifies new professional matches
2. **Demo Download**: Automated download of match demo files
3. **Data Extraction**: Parse demos for player actions, kills, and skin information
4. **Database Update**: Store processed data with proper player attribution
5. **API Serving**: Expose data through RESTful endpoints
6. **Frontend Display**: Present information in user-friendly interface

### Data Quality Assurance

- **Player Identification**: Advanced algorithms to correctly attribute actions to players
- **Skin Validation**: Cross-reference with CSGOExchange database for accuracy
- **Error Recovery**: Comprehensive logging and retry mechanisms for failed parses

## 🎯 Technical Challenges Solved

- **Demo File Parsing**: Developed robust parsing logic handling various demo file formats and edge cases
- **Player Attribution**: Created sophisticated matching algorithms to correctly identify players across different naming conventions
- **Scalable Data Processing**: Designed efficient batch processing system handling large volumes of match data
- **Real-time Updates**: Implemented automated pipeline maintaining data freshness without manual intervention

## 🔗 Related Projects

**Frontend Repository**: [CS Pros Frontend](https://github.com/liamkyoung/cspros-frontend)

## 📝 Technical Notes

### Current Limitations

- Knife kills are not recorded due to demo file format limitations
- Skin data only available from kills (weapon must be used to eliminate another player)
- ~2% of demo files cannot be parsed due to stream corruption
- Player name matching occasionally requires manual verification

### Future Enhancements

- Machine learning models for improved player identification
- Extended weapon tracking beyond kills
- Integration with additional data sources
- Performance optimizations for larger datasets

## 🤝 Contributing

This project demonstrates full-stack development capabilities, automated data processing, and scalable system design. The codebase showcases modern development practices including:

- Clean architecture patterns
- Automated testing strategies
- CI/CD pipeline implementation
- Performance optimization techniques
- Error handling and logging best practices

---

**Built with ❤️ for the Counter-Strike community**

*This project represents a comprehensive solution to a real-world data problem, demonstrating skills in web scraping, data processing, API development, database design, and modern frontend development.*