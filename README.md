# TaskHub Flow - Task Management System

A professional-grade full-stack task management 

### Technical spec 'Tasks web application - Tech design - SuperCom'
A full elaborated technical spec is attached to the repository as well. It includes :
1. Architectural strategy decisions and trade-offs
2. detailed API endpoints 
3. Data models
4. The requested SQL query
5. Optional future extensions 

### Deployment 
The easiest way to review this project is using Docker. This will automatically set up the database and launch all services.

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

### Instructions
1. Open a terminal in this root directory.
2. Run the following command:
   ```bash
   docker compose up --build -d
   ```
3. Wait **30-60 seconds** for SQL Server to initialize and migrations to apply.
4. Access the application at: **[http://localhost:8080](http://localhost:8080)**
   - *Note: API Swagger is available at http://localhost:5050/swagger*

## ✨ Features

- **Real-Time Dashboards**: Automatic 30-second polling keeps task statuses (like "Reminded" badges) synced with background processes.
- **Background Workers**: Includes a `.NET HostedService` that monitors due dates and publishes reminder events to RabbitMQ.
- **Smart Sorting**: Tasks are prioritised by level (High > Medium > Low) and then by the nearest due date.
- **Robust Validation**: Combined client-side (Zod) and server-side (FluentValidation) ensures data integrity.
- **Seeded Data**: Alice Smith and Bob Jones and some tags are pre-populated so you can test assignment and filtering immediately.

## 🛠️ Tech Stack

- **Backend**: .NET 8 Web API, Entity Framework Core (SQL Server), RabbitMQ, Masstransit.
- **Frontend**: React 18, Redux Toolkit (RTK Query), Vite, Vanilla CSS.
- **Infrastructure**: Docker & Docker Compose.

## 👨‍💻 Local Development (Alternative)

If you prefer to run the components manually:
### 1. Database
Update `appsettings.json` connection string and run:
`dotnet ef database update`
### 2. Backend
`cd TaskManagementSystem.Api`
`dotnet run`
### 3. Frontend
`cd TaskManagementSystem.Client`
`npm install`
`npm run dev`