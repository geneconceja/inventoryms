# InventoryMS — Requirements & Scope

## Platform
- **Framework:** ASP.NET Core MVC (.NET 8), Razor views
- **Database:** PostgreSQL via EF Core / Npgsql
- **Auth:** ASP.NET Core Identity with role-based authorization
- **Feature tier:** Full — multi-warehouse, roles/auth, reporting & dashboard

## Roles & Permissions

| Role | Can do |
|---|---|
| **Admin** | Everything Manager can, plus manage user accounts and role assignments, system configuration |
| **Manager** | Manage products, categories, suppliers, warehouses; create & approve purchase orders; view dashboard/reports |
| **Staff** | Log stock in/out/transfers; read-only on catalog, suppliers, and everything else |

## Core Entities
Category, Supplier, Warehouse, Product, InventoryStock, StockTransaction, PurchaseOrder, PurchaseOrderItem, ApplicationUser, Role — 10 tables total (see `schema.sql`).

## Modules (8, one per controller)
Dashboard · Products · Categories · Suppliers · Warehouses · Stock Transactions · Purchase Orders · User Management

## Key Business Rules
- All stock quantity changes route through a single `InventoryService` — no direct writes to `InventoryStock` from controllers
- Stock-out transactions can never drop a product's quantity below zero
- A Purchase Order only affects stock once its status changes to **Received** — receiving auto-generates a Stock-In transaction
- A product is "low stock" when its warehouse quantity falls below its `ReorderLevel`
- Every stock transaction is attributed to the user who performed it (audit trail)

## Notifications
- **Email:** Brevo integration sends an alert when a product drops below its `ReorderLevel`
- **In-app:** a notification also surfaces in the app itself (e.g. bell icon / dashboard panel), triggered by the same low-stock condition
- Requires a Brevo API key in configuration; a lightweight `Notification` concern sits alongside the low-stock check

## Units of Measure
- Single unit per product, no conversions. `Quantity` stays a plain integer — no `unit_of_measure` field, no conversion factors.

## Currency
- Single currency for now. `unit_price` and `unit_cost` stay plain `numeric` columns, no currency code.
- **Future-proofing note:** if multi-currency is added later, this means a `currency_code` column on `Product` and `PurchaseOrder`, plus a decision on whether historical order amounts stay in their original currency or get converted at read time. Not a breaking schema change, but worth not architecting against.

## Explicitly Out of Scope
- Barcode / SKU scanning (no scan UI)
- Multi-currency pricing (see note above)
- Unit-of-measure conversions

## Open / Deferred Decisions
- **Deployment target:** not yet chosen (Azure App Service, VM, containers, etc.) — revisit at the deployment phase of the build.

## Development Process (8 phases)
1. Define requirements & scope *(this document)*
2. Set up environment & project
3. Design the database & write migrations
4. Build the domain & auth layer
5. Implement the business logic layer
6. Build controllers & views
7. Test across roles & edge cases
8. Deploy & monitor
