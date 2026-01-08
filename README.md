# TaskManagementSystemAPI

## Tech Stack
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- PostgreSQL
- JWT Authentication

## Setup
1. Update PostgreSQL connection string in `appsettings.json`
2. Run migrations:
   dotnet ef migrations add InitialCreate
   dotnet ef database update
3. Run the API:
   dotnet run

Swagger UI available at `/swagger`