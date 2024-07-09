# CS2 Professional Skin Analyzer

## Introduction:
*What is this project?*
- This project tracks Counter Strike players and the skins that they use in-game. This project scrapes a few different sites to create an up-to-date DB of players, teams, and games.
In addition, this project analyzes demo (.dem) files of professional CS2 matches using the DemoFile library (https://github.com/saul/demofile-net)[found here.]


## Project architecture:
**Front-end**: Next.js 14 / TailwindCSS / Typescript (Hosted on Vercel)
**API**: .NET / (Hosted on EC2 Instance)
**Database**: PostgreSQL (Hosted on RDS) (Schema Here)
**Web Scraper / Demo Downloader**: (Hosted on Local)
**Demo Analyzer**: (Hosted on Local)
Shared Library: Shared Services, Repositories, etc.

