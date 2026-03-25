# 💼 Financial Portfolio Management System

A full-stack application for tracking and analysing investment portfolios in real time. Built with ASP.NET Core 8 and React TypeScript, featuring interactive dashboards, transaction management, and a MongoDB caching layer for fast analytics.

---

## ✨ Features

- 📊 Real-time portfolio dashboard with key performance metrics
- 💹 Interactive charts — holdings allocation, trends, and performance over time
- 📋 Transaction history with filtering by type, date, and symbol
- 🔍 Deep analytics — top gainers/losers, ROI, asset allocation breakdown
- ⚡ MongoDB caching layer to avoid repeated SQL computation on analytics
- 📱 Fully responsive design across desktop and mobile

---

## 🛠️ Tech Stack

### Backend
| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core |
| Primary database | SQL Server |
| Cache / analytics store | MongoDB |
| Object mapping | AutoMapper |
| Testing | xUnit, Moq |

### Frontend
| Layer | Technology |
|---|---|
| Framework | React 18 + TypeScript |
| Styling | Tailwind CSS |
| Charts | Recharts |
| Routing | React Router v6 |
| Testing | Vitest, React Testing Library |

### DevOps
| Layer | Technology |
|---|---|
| CI/CD | GitHub Actions |
| Database | Azure SQL Serverless |

---

## 🏗️ Architecture

```
Client (React SPA)
        │
        ▼
ASP.NET Core 8 REST API
        │
        ├── SQL Server       ← Portfolios, holdings, transactions, accounts
        └── MongoDB          ← Cached analytics responses
```

**Key design decisions:**
- Clean architecture: Controllers → Services → Repository pattern
- MongoDB acts as a read cache — analytics are computed once from SQL, then served from Mongo on repeat requests
- React SPA with lazy-loaded routes and memoised components for performance

---



## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/en-gb/sql-server/) (or SQL Server Express)
- [MongoDB](https://www.mongodb.com/try/download/community)

### 1. Clone the repository

```bash
git clone https://github.com/your-username/financial-portfolio.git
cd financial-portfolio
```

### 2. Configure the backend

Update `FinancialPortfolio.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FinancialPortfolioDB;Trusted_Connection=True;",
    "MongoConnection": "mongodb://localhost:27017"
  }
}
```

### 3. Run database migrations

```bash
cd FinancialPortfolio.Api
dotnet ef database update
```

### 4. Start the API

```bash
dotnet run
# API runs at https://localhost:5000
# Swagger UI at https://localhost:5000/swagger
```

### 5. Start the frontend

```bash
cd financial-portfolio-ui
npm install
npm run dev
# App runs at http://localhost:5173
```

---

## 📡 API Endpoints

### Portfolios

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/portfolios/user/{userId}` | Get all portfolios for a user |
| `GET` | `/api/portfolios/{id}` | Get portfolio by ID |
| `POST` | `/api/portfolios` | Create a new portfolio |
| `PUT` | `/api/portfolios/{id}` | Update a portfolio |
| `DELETE` | `/api/portfolios/{id}` | Delete a portfolio |
| `GET` | `/api/portfolios/{id}/summary` | Get portfolio value summary |

### Holdings

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/portfolios/{id}/holdings` | Get all holdings in a portfolio |
| `POST` | `/api/portfolios/{id}/holdings` | Add a holding |
| `PUT` | `/api/holdings/{id}` | Update a holding |
| `DELETE` | `/api/holdings/{id}` | Remove a holding |

### Transactions

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/transactions/{portfolioId}` | Get transaction history |
| `POST` | `/api/transactions` | Record a new transaction |

### Analytics

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/analytics/{portfolioId}` | Get full portfolio analytics |
| `GET` | `/api/analytics/{portfolioId}/history` | Get transaction history with filters |

---


## 🧪 Running Tests

```bash
# Backend tests
cd FinancialPortfolio.Tests
dotnet test --verbosity normal

# Frontend tests
cd financial-portfolio-ui
npm test
```

---


---

## 📄 Licence

This project is open source under the [MIT Licence](LICENSE).
