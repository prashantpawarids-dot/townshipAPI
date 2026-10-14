# Township API

Backend REST API for the Township management system. It serves the web application and handles residents, vehicles, visitors, cameras and reports.

## Features

- Resident / tenant and guest management
- Vehicle registration and ANPR camera integration
- Live camera view and camera status monitoring
- Service provider management
- Reports
- Real-time updates using SignalR hubs
- JWT login and role-based access

## Tech Stack

- C# / ASP.NET Core Web API (.NET 8)
- Entity Framework Core with SQL Server
- SignalR for real-time data
- FFmpeg for live camera streaming

## Project Structure

| Folder | Purpose |
|---|---|
| `Controllers/` | API endpoints |
| `Models/` | Data models |
| `Data/` | Database context |
| `Migrations/` | Database migrations |
| `Services/` | Business logic |
| `Hubs/` | Real-time SignalR hubs |

## Getting Started

1. Clone the repo and open `Township-API.sln` in Visual Studio 2022.
2. Set your SQL Server connection string in `appsettings.json`.
3. Apply the database migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the project:
   ```bash
   dotnet run
   ```

## Note

Do not commit passwords, API keys or connection strings with real credentials to GitHub.

## Author

**Prashant Pawar** - [@prashantpawarids-dot](https://github.com/prashantpawarids-dot)
