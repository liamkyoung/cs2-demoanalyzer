## Introduction:

**What is this project?**

- This project tracks Counter Strike players and the skins that they use in-game. This project scrapes a few different sites to create an up-to-date DB of players, teams, and games.
  In addition, this project analyzes demo (.dem) files of professional CS2 matches using the DemoFile library [found here.](https://github.com/saul/demofile-net)

**Does this data exist anywhere else?**

- Limited information about professional player skins is out there because of frequent changes of weapon skins from the players. This project helps to track and record the skins used by each player.

**Is this data up-to-date?**

- This project includes _almost_ every game recorded played in Tier-1 from CS2. As new games are played every day, updated data will be scraped and logged every few hours on the site.

**What are the limitations of this project?**

- Skin data parsed from .DEM files are limited. Skins can only be logged from users killing another player with the weapon with the given weapon Id, and is cross referenced with the public skin database at csgo.exchange.
- Knife kills are not recorded.
- Rarely, a .dem file cannot be parsed if the _stream_ does not end properly on the file, which occurs about 2% of the time.
- Rarely, kills are attributed to the wrong player if their in-game-name more similarly resembles another player's name more than their own. Proper procedures are in place to properly identify players.
- Games and Skin Usage is updated as new games come in, and Team / Player data is updated weekly

## Project architecture:

<img width="1040" alt="Screen Shot 2024-07-09 at 4 54 46 PM" src="https://github.com/liamkyoung/cs2-demoanalyzer/assets/52087920/63553649-7b91-4f20-bd03-a1a8cd77e538">

**Front-end**: Next.js 14 / TailwindCSS / Typescript (Hosted on Vercel)
**API**: .NET / (Hosted on EC2 Instance)
**Database**: PostgreSQL (Hosted on RDS) (Schema Here)
**Web Scraper / Demo Downloader**: (Hosted on Local Raspberry Pi, running on cron job)
**Demo Analyzer**: (Hosted on Local Raspberry Pi, running on cron job)
Shared Library: Shared Services, Repositories, etc.

## Setup

There is more than one way to set up this project. I will walk through how I have the project running with a split between cloud hosting and local data processing.

Firstly, clone the repo to your machine.

### Backend

Starting with the backend, there are 3 projects and 1 database.

On all machines, you must install .NET 8. There are many ways to do this on all different OSs, but [this guide](<(https://learn.microsoft.com/en-us/dotnet/core/install/linux-debian)>) helped me for Debian

#### Database

Install Postgres on your machine or set up an RDS AWS instance with Postgres 15.6

If local:
Use PG Admin to create a database with name
`cspros`

Make sure that

1. Dotnet is installed (try `dotnet --version`)
2. You are in the CSProsLibrary directory
3. You have connectionstring vars setup
   (CSPROS_DEV and CSPROS_PROD)

Then, after you can access CSProsLibrary, then try to update your databse using the command at the root dir of the project

```
dotnet ef database update --project ./CSProsLibrary --startup-project ./CS2DemoAnalyzer
```

#### API

I have my API running on an AWS EC2 container reverse proxied with nginx.

1. Install .NET 8

- Depending on your OS, there are different ways to install .NET 8.
- My EC2 server is running Debian, so I [followed these steps.](https://learn.microsoft.com/en-us/dotnet/core/install/linux-debian)

2. Add Proper Environment Variables
   For the app to run, you must set an environment variables to point to your dev DB and prod DB. These should be set with an appsettings.json file (or could be replaced with env variables)

What the appsettings.json file looks like:

```
{
	"ConnectionStrings": {
		"CSPROS_PROD": "Host=server_address;Database=cspros;Username=cspros;Password=YOURPASSWORD",
                "CSPROS_DEV": "Host=localhost;Database=cspros;Username=cspros;Password=YOURPASSWORD"
	}
}
```

3. Run on Local.
   **OR**
   If running on prod, create nginx reverse proxy to forward traffic, and make sure to run the profile `https-prod` on prod.

4. Setup API as a service on linux

#### CS2DemoAnalyzer

- Make sure to set environment variables:

```
# Path where archived files are saved
DEMO_DOWNLOAD_DIR=/path

# Path where extracted files are saved
DEMO_ANALYZE_DIR=/path
```

1. Install Firefox web browser
2. Install geckodriver.

Linux users: Make sure to place the Geckodriver executable in your `/usr/local/bin`

You can now run the app with your IDE or with the start.sh command (NOTE: You will need to update the path of dotnet and where you installed your project to execute the app)

#### WebScraper

Follow the same steps as CS2DemoAnalyzer to set up the webscraper

You can run the app with your IDE or with the start.sh command (NOTE: You will need to update the path of dotnet and where you installed your project to execute the app)

### Frontend

For the frontend, make sure to have [Next.js installed](https://nextjs.org/docs/getting-started/installation)

To run on local, create a `.local.env` file and paste in the env variable `NEXT_PUBLIC_BACKEND_HOST` pointing to the backend api.

**NEXT_PUBLIC_BACKEND_HOST=https://localhost:7280**

To run, go into the frontend directory and execute `npm run dev`

