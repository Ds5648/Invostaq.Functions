# InvoStaq – Azure Functions Invoice API

A simple REST API built with Azure Functions v4, C# .NET 8, and EF Core + SQLite.

---

## Local Setup

### Prerequisites
- .NET 8 SDK
- Azure Functions Core Tools v4
- Visual Studio 2022

### Install Azure Functions Core Tools
Download v4 MSI from:
https://github.com/Azure/azure-functions-core-tools/releases

---

## How to Run Locally

Open `InvoStaq.Functions.csproj` in Visual Studio 2022 and press F5


SQLite database `invostaq.db` is auto-created on first run.

---

## API Endpoints

Base URL: `http://localhost:7074/api`

### POST /invoice
Save a new invoice.

POST /api/invoice
Content-Type: application/json
{
  "Amount": 500,
  "Date": "2026-04-02"
}

**Responses**

| Status | Meaning |
|--------|---------|
| 201 Created | Invoice saved successfully |
| 400 Bad Request | Missing or invalid fields |
| 409 Conflict | Duplicate invoice ID |

---

### GET /invoice/{id}
Retrieve an invoice by ID.
Get/api/invoice/{id}
**Responses**

| Status | Meaning |
|--------|---------|
| 200 OK | Invoice returned |
| 404 Not Found | Invoice not found |

---

---

## IaC Structure (Terraform)

Terraform files in `infra/terraform/` provision:

| Resource | Purpose |
|----------|---------|
| Resource Group | Container for all resources |
| Storage Account | Required by Functions runtime |
| Application Insights | Monitoring and telemetry |
| Azure SQL Server + Database | Production database |
| App Service Plan | Serverless hosting |
| Function App | Hosts the invoice API |

---

## CI/CD Pipeline

`.github/workflows/deploy.yml` has 3 jobs:

| Job | What it does |
|-----|-------------|
| **Build** | Builds and publishes the .NET project |
| **Terraform Plan** | Validates and dry-runs infrastructure |
| **Deploy** | Deploys Function App to Azure |

## Folder Structure

InvoStaq.Functions/
|- Data/
   -- InvoiceDbContext.cs     - EF Core DbContext with SQLite
| Functions/
   -- InvoiceFunctions.cs     - POST and GET HTTP triggers
| Models/
   -- Invoice.cs              - Entity model
   -- CreateInvoiceRequest.cs - Request DTO
|- infra/
  |- terraform/
       |-- main.tf             - Azure resources
       |-- variables.tf        - Input variables
       |-- outputs.tf          - Output values
|-  Program.cs                  - Host startup + DI
|-  host.json
|-  local.settings.json

.github/
   |-- workflows/
       |-- deploy.yml          - CI/CD pipeline

----Completed-----
