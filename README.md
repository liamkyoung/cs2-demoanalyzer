# CS2 Professional Skin Analyzer

## Introduction:
*What is this project?*
- This project tracks Counter Strike players and the skins that they use in-game. This project scrapes a few different sites to create an up-to-date DB of players, teams, and games.
In addition, this project analyzes demo (.dem) files of professional CS2 matches using the DemoFile library (https://github.com/saul/demofile-net)[found here.]


## Project architecture:
<img width="1040" alt="Screen Shot 2024-07-09 at 4 54 46 PM" src="https://github.com/liamkyoung/cs2-demoanalyzer/assets/52087920/63553649-7b91-4f20-bd03-a1a8cd77e538">

**Front-end**: Next.js 14 / TailwindCSS / Typescript (Hosted on Vercel)
**API**: .NET / (Hosted on EC2 Instance)
**Database**: PostgreSQL (Hosted on RDS) (Schema Here)
**Web Scraper / Demo Downloader**: (Hosted on Local)
**Demo Analyzer**: (Hosted on Local)
Shared Library: Shared Services, Repositories, etc.

