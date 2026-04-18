# TaskHub Flow - Task Management System

A professional-grade full-stack task management system 

### Technical spec 
A full elaborated technical spec is attached to the repository see 'Tech design - SuperCom.pdf'
It includes:
1. Architectural strategy decisions and trade-offs
2. Detailed API endpoints 
3. Data models
4. The requested SQL query
5. Optional future extensions 

## 🛠️ Tech Stack
- **Backend**: .NET 8 Web API, Entity Framework Core (SQL Server), RabbitMQ, Masstransit.
- **Frontend**: React 18, Redux Toolkit (RTK Query), Vite, Vanilla CSS.
- **Infrastructure**: Docker & Docker Compose.

### 🐳 Deployment
The easiest way to review this project is using Docker. This will automatically set up the database, RabbitMQ, and launch all services.
### Prerequisites
Docker installed and running
### Instructions
1. Open a terminal in root directory.
2. Run the following command:
   ```bash
   docker compose down -v
   docker compose up --build -d
   ```
3. Access the application at:
   - **API Swagger**: [http://localhost:5001/swagger](http://localhost:5001/swagger) 🟢
   - **Frontend UI**: [http://localhost:8080](http://localhost:8080) 🟢
   - **RabbitMQ Dashboard**: [http://localhost:15673](http://localhost:15673) (guest/guest) 🟢
   - To watch logs 
   ```bash
   docker compose logs api -f
   ```

## ✨ SQL query (also resides in the tech spec)
SELECT t.Id, t.Title, COUNT(tt.TagId) AS TagCount, STRING_AGG(tg.Name, ', ') AS TagNames 
FROM Tasks t 
JOIN TaskTags tt ON t.Id = tt.TaskId 
JOIN Tags tg ON tg.Id = tt.TagId 
GROUP BY t.Id, t.Title 
HAVING COUNT(tt.TagId) >= 2 
ORDER BY TagCount DESC
