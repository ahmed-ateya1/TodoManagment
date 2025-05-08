# TodoManagement Application

A simple and clean Todo Management API built with ASP.NET Core, following Domain-Driven Design (DDD) principles. The application includes a frontend built with Bootstrap for managing todos.

---

## 📋 Project Description

The **Todo Management Application** is a task management tool that allows users to create, read, update, and delete (CRUD) todos, manage their status, and apply filters for improved productivity.

---

## ✅ Features Implemented

### 🧾 Core Features

- [x] **CRUD Operations** for Todos (Create, Read, Update, Delete)
- [x] **Todo Status Management** (Pending, InProgress, Completed)
- [x] **Validation**:
  - Title is required (max 100 characters)
  - Valid date entries
- [x] **List Todos** with:
  - Filter by status
  - Additional filters: Priority, Due Date range
  - Sorting options (by CreatedDate, DueDate, etc.)
- [x] **Mark Todo as Complete**
- [x] **Docker Compose For Application**

### ⚙️ Technical Implementation

- [x] ASP.NET Core 8.0 Web API
- [x] Entity Framework Core
- [x] Domain-Driven Design (DDD) structure
- [x] Domain Events (e.g., `TodoCompletedEvent`)
- [x] Clean separation of Core, API, and Infrastructure layers
- [x] API documentation via Swagger (XML documentation included)

### 🎨 Frontend

- [x] Basic UI built with **Bootstrap**
- [x] Todo list with:
  - Filter by status
  - Create/Edit Todo forms
  - Delete confirmation dialogs
  - Status labels for visual feedback

---

## 📂 Project Structure

```
├── TodoManagement.API
│   ├── Controllers
│   ├── Middlewares
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
├── TodoManagement.Core
│   ├── Domain
│   ├── Dtos
│   ├── Services
│   ├── Validators
│   ├── Exceptions
│   └── DependencyInjection.cs
├── TodoManagement.Infrastructure
│   ├── Data
│   ├── Migrations
│   ├── Repositories
│   └── Configuration
├── docker-compose.yml
├── frontend
│   ├── index.html
│   ├── CSS
│   ├── js
│   └── Dockerfile
└── README.md
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) (for running without Docker)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (for running without Docker)
- [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/) (for containerized setup)
- [Git](https://git-scm.com/) (for cloning the repository)

### Option 1: Running the API Locally (Without Docker)

1. **Clone the repository**:

   ```bash
   git clone https://github.com/ahmed-ateya1/Todo-Management.git
   cd TodoManagement
   ```

2. **Set up the database**:
   - Ensure SQL Server is running.
   - Update the connection string in `TodoManagement.API/appsettings.json` to point to your SQL Server instance.
   - Run migrations to create the database:
     ```bash
     cd TodoManagement.Infrastructure
     dotnet ef database update
     ```

3. **Run the API**:
   - Navigate to the API project:
     ```bash
     cd ../TodoManagement.API
     dotnet run
     ```
   - The API will be available at `http://localhost:8080`. Access Swagger at `http://localhost:8080/swagger`.

4. **Run the frontend**:
   - Navigate to the `frontend` directory and serve the static files using a local server (e.g., Live Server in VS Code or `http-server`):
     ```bash
     cd ../frontend
     npx http-server -p 3000
     ```
   - Open `http://localhost:3000` in your browser.

### Option 2: Running with Docker Compose

1. **Clone the repository** (if not already done):

   ```bash
   git clone https://github.com/ahmed-ateya1/TodoManagement.git
   cd TodoManagement
   ```

2. **Set up environment variables** (optional):
   - Create a `.env` file in the root directory to store sensitive data (e.g., database password):
     ```env
     MSSQL_SA_PASSWORD=AhmedAteya12348@#
     DOCKER_REGISTRY=
     ```
   - Ensure the password meets SQL Server complexity requirements (at least 8 characters, including uppercase, lowercase, numbers, and special characters).

3. **Build and run the services**:
   - Run the following command to build the Docker images and start the containers:
     ```bash
     docker-compose up --build
     ```
   - This will:
     - Build the `todomanagment` (API) and `frontend` images.
     - Pull the `mcr.microsoft.com/mssql/server:2022-latest` image for the database.
     - Start the services: API (`todomanagment`), database (`todo-db`), and frontend.
   - The services will be available at:
     - **Frontend**: `http://localhost:3000`
     - **API**: `http://localhost:8080` (Swagger at `http://localhost:8080/swagger`)
     - **SQL Server**: `localhost:1433` (for external tools, if needed)

4. **Access the application**:
   - Open `http://localhost:3000/index.html` in your browser to use the frontend.
   - The frontend communicates with the API at `http://todomanagment:8080/api/Todo` (configured via the `API_BASE_URL` environment variable).

5. **Stop the services**:
   - To stop and remove the containers:
     ```bash
     docker-compose down
     ```
   - To preserve the database data, the `todo_data` volume will persist unless explicitly removed.

6. **Troubleshooting**:
   - View logs for debugging:
     ```bash
     docker-compose logs <service-name>  # e.g., todomanagment, todo-db, frontend
     ```
   - If the API or frontend fails to connect, ensure:
     - The database is fully initialized (health checks are included to handle this).
     - The `API_BASE_URL` in the `frontend` service is set to `http://todomanagment:8080/api/Todo`.
   - To remove volumes (e.g., to reset the database):
     ```bash
     docker-compose down -v
     ```

---
### Challenges Faced
- Integrating frontend with the API required handling CORS issues.
- Ensuring smooth database migrations with Docker.

## 📝 Notes

- The Docker Compose setup includes a health check for the `todo-db` service to ensure the API starts only after the database is ready.
- The frontend uses Nginx to serve static files and communicates with the API over the `todo-network`.
- For production, secure the `MSSQL_SA_PASSWORD` using Docker secrets or a secrets management tool.
- Ensure CORS is configured correctly in `TodoManagement.API` to allow requests from `http://frontend:80` (Docker) and `http://localhost:3000` (local development).
