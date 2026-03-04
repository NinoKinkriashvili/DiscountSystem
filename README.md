# Discounts System (Independent Backend Project)

Role-based discounts marketplace backend with moderation workflows, time-limited reservations, purchases and automated cleanup via background workers.

## Overview
This project implements a multi-role backend platform where:
- **Merchants** create and manage discount offers
- **Administrators** moderate offers and manage system settings
- **Customers** browse offers, reserve coupons for a limited time, and purchase them

The system includes automated background processing to clean up expired reservations and keep data consistent.

## Key Features
- **Role-based access**: Administrator / Merchant / Customer
- **Offer lifecycle & moderation**: create → pending → approve/reject (with reason), visibility control
- **Reservations & purchases**: time-limited reservations, availability handling, purchase flow
- **Background workers**: automated cleanup of expired reservations (and other scheduled tasks)
- **Validation & error handling**: FluentValidation + centralized exception middleware (consistent API errors)
- **API tooling**: Swagger for testing and documentation, API versioning, health checks
- **Async-first**: async workflows with CancellationToken support

## Tech Stack
- **C# / .NET (ASP.NET Core)**
- **ASP.NET Core Web API** (MVC layer included in the solution)
- **Entity Framework Core (Code First)** + **SQL Server**
- **JWT Authentication** + role-based authorization
- **FluentValidation**
- **Swagger (OpenAPI)**
- **Background Worker Services**
- Clean Architecture / Layered approach, Repository Pattern

## Architecture
The solution follows a layered structure:
- `DiscountsSystem.Domain` — domain entities and core rules
- `DiscountsSystem.Application` — use-cases, services, DTOs, validation contracts
- `DiscountsSystem.Infrastructure` — EF Core persistence, repositories, external integrations
- `DiscountsSystem.Api` — Web API endpoints, middleware, auth, versioning, Swagger
- `DiscountsSystem.Mvc` — MVC layer (UI) in the same solution
- `DiscountsSystem.Worker` — background hosted services for scheduled cleanup tasks

## Getting Started

### Prerequisites
- .NET SDK (compatible with the solution)
- SQL Server (local or Docker)

### Configuration (Important)
This repo does **not** include development secrets.
Create your own `appsettings.Development.json` in:
- `DiscountsSystem.Api`
- `DiscountsSystem.Mvc`
- `DiscountsSystem.Worker`

Add your local configuration such as:
- SQL Server connection string
- JWT settings (issuer/audience/key)
- any email/SMTP settings (if used)

> Tip: Keep secrets out of source control. Use User Secrets or environment variables for local development.

### Database Setup
From the solution root (or the project that contains migrations), run:
```bash
dotnet ef database update
