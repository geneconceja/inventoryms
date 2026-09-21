# InventoryMS — Inventory Management System

A multi-warehouse Inventory Management System built with **ASP.NET Core 9 MVC**, **Entity Framework Core**, **PostgreSQL**, and **ASP.NET Core Identity**.

---

## Overview & Architecture

**InventoryMS** tracks inventory across multiple physical warehouses, manages procurement via purchase orders, enforces strict stock integrity through a centralized business logic service, and provides role-based access control with real-time low-stock alerting.

### Key Architecture Highlights
- **Centralized Stock Engine:** All quantity changes route strictly through `InventoryService` — direct controller writes to stock levels are prohibited.
- **Stock Integrity:** Stock-out transactions can never drop inventory below zero.
- **Automated Receiving:** Purchase Orders only affect inventory once marked as **Received**, which auto-generates Stock-In transactions and updates the audit log.
- **Notifications:** Low-stock threshold detection triggers both in-app dashboard alerts and email notifications via the Brevo API.

---

## Core Entities & Modules

### 8 Functional Modules
1. **Dashboard:** Key metrics, low-stock warnings, recent stock movements, and inventory valuation.
2. **Products:** Catalog management, SKU tracking, pricing, and reorder thresholds.
3. **Categories:** Product classification and organization.
4. **Suppliers:** Vendor contact details and purchasing history.
5. **Warehouses:** Multi-location warehouse management.
6. **Stock Transactions:** Complete audit trail for Stock-In, Stock-Out, and Inter-Warehouse Transfers with user attribution.
7. **Purchase Orders:** Procurement lifecycle (`Draft` &rarr; `Submitted` &rarr; `Approved` &rarr; `Received` &rarr; `Cancelled`).
8. **User Management:** Account administration and role assignments.

### Roles & Permissions

| Role | Permissions |
|---|---|
| **Admin** | Full system access, user account management, role assignment, and system configuration. |
| **Manager** | Manage products, categories, suppliers, warehouses; create and approve purchase orders; access reports and dashboard. |
| **Staff** | Log stock in/out/transfers; view-only access to catalog, suppliers, and general records. |

---

## Tech Stack

- **Framework:** ASP.NET Core 9.0 (MVC + Razor Views)
- **Database:** PostgreSQL 14+ via EF Core 9 (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Authentication & Authorization:** ASP.NET Core Identity with role-based policies
- **Front-End:** Bootstrap 5, Modern CSS, jQuery
- **Notifications:** Brevo API (Transactional Email) + In-App Notifications
- **Tooling:** Entity Framework Core CLI Tools (`dotnet-ef`)

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (version 9.0.200 or later)
- [PostgreSQL](https://www.postgresql.org/download/) installed and running locally or via Docker

### 1. Clone the Repository
```bash
git clone <repository-url>
cd inventoryms
```

### 2. Configure the Database
Create your PostgreSQL database and user (or use your default PostgreSQL superuser):

```sql
CREATE DATABASE inventoryms_db;
CREATE USER inventoryms_app WITH ENCRYPTED PASSWORD 'ims-postgres';
GRANT ALL PRIVILEGES ON DATABASE inventoryms_db TO inventoryms_app;
```

Update your connection string in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=inventoryms_db;Username=inventoryms_app;Password=ims-postgres"
  }
}
```

### 3. Restore Dependencies & Local Tools
```bash
dotnet restore
dotnet tool restore
```

### 4. Apply Database Migrations
*(Once Phase 3 migrations are generated)*
```bash
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```
Navigate to `http://localhost:5056` in your browser.

---

## Configuration Settings

### Brevo Email Configuration
To enable automated low-stock email alerts, add your Brevo API key to `appsettings.Development.json` (or via .NET User Secrets / Environment Variables):

```json
{
  "Brevo": {
    "ApiKey": "YOUR_BREVO_API_KEY",
    "SenderEmail": "notifications@yourdomain.com",
    "SenderName": "InventoryMS Alerts"
  }
}
```

---

## Project Structure

```
inventoryms/
├── Controllers/              # MVC Controllers (8 modules)
├── Data/                     # ApplicationDbContext & Migrations
├── Models/                   # Domain entities, ViewModels, and Enums
│   ├── Category.cs
│   ├── InventoryStock.cs
│   ├── Product.cs
│   ├── PurchaseOrder.cs
│   ├── PurchaseOrderItem.cs
│   ├── StockTransaction.cs
│   ├── Supplier.cs
│   └── Warehouse.cs
├── Services/                 # Business logic (InventoryService, BrevoService)
├── Views/                    # Razor views and shared layouts
├── wwwroot/                  # Static assets (CSS, JavaScript, libraries)
├── requirements-and-scope.md # Functional requirements specification
├── nuget.config              # NuGet package configuration
├── appsettings.json          # Production configuration template
└── appsettings.Development.json # Development configuration (local DB)
```

---

## Development Roadmap

- [x] **Phase 1:** Define requirements & scope (`requirements-and-scope.md`)
- [x] **Phase 2:** Environment setup, `.gitignore`, NuGet packages, and config
- [ ] **Phase 3:** Database schema, EF Core models, Identity, and migrations
- [ ] **Phase 4:** Domain & authentication layer (seed roles/admin, login/register UI)
- [ ] **Phase 5:** Business logic layer (`InventoryService`, low-stock checks, Brevo client)
- [ ] **Phase 6:** Controllers & Views (Dashboard, Products, Warehouses, Orders, etc.)
- [ ] **Phase 7:** Testing across roles, stock integrity rules, and edge cases
- [ ] **Phase 8:** Deployment & monitoring

---

## License
Proprietary / Internal project.
