
### Performing Migrations
##### Creating Migration:
`dotnet ef migrations add MigrationName`
##### Updating DB
`dotnet ef database update`

##### Deleting / Restoring DB
- drop database
- `dotnet ef migrations add InitialMigration`
- `dotnet ef database update`
- Run SQL scripts in `/DBSetup` to get initial data for weapons, maps, countries, etc.