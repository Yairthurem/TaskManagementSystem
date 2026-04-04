# TaskHub Flow - Task Management System

A professional-grade full-stack task management system 

### Technical spec 
A full elaborated technical spec is attached to the repository as well 'Tech design - SuperCom.pdf'
It includes:
1. Architectural strategy decisions and trade-offs
2. Detailed API endpoints 
3. Data models
4. The requested SQL query
5. Optional future extensions 

### 🐳 Deployment (Recommended)
The easiest way to review this project is using Docker. This will automatically set up the database, RabbitMQ, and launch all services.

### Instructions
1. Open a terminal in this root directory.
2. Run the following command:
   ```bash
   docker compose down -v
   docker compose up --build -d
   ```
3. Access the application at:
   - **Frontend UI**: [http://localhost:8080](http://localhost:8080) 🟢
   - **API Swagger**: [http://localhost:5001/swagger](http://localhost:5001/swagger) 🟢
   - **RabbitMQ Dashboard**: [http://localhost:15673](http://localhost:15673) (guest/guest) 🟢

### 1. Automated Tests (Full Proof)
To verify the core business logic and service mapping in isolation:
```bash
dotnet test
```

### 2. Manual Integration (Swagger)
You can verify the **full data flow** (API -> SQL Server -> RabbitMQ Background Reminders) by using the Swagger UI at [http://localhost:5001/swagger](http://localhost:5001/swagger).

---

## ✨ Features
- **Real-Time Dashboards**: Automatic 30-second polling keeps task statuses (like "Reminded" badges) synced with background processes.
- **Background Workers**: Reminders are triggered by a `.NET HostedService` and processed via **RabbitMQ** for high reliability.
- **Smart Sorting**: Tasks are prioritised by level (High > Medium > Low) and then by the nearest due date.
- **Robust Validation**: Combined client-side (Zod) and server-side (FluentValidation) ensures data integrity.
- **Professional Seeding**: Pre-loaded with full user details (Alice Smith, Bob Jones) and tags.

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