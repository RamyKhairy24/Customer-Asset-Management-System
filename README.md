# Customer-Asset-Management-System
A professional-grade backend API developed during my internship at Commercial International Bank (CIB). This system focuses on the relationship between banking customers and physical assets (properties/units), implementing secure data validation and automated payment tracking.




# CustomerFluent - Property & Asset Management System
### 🏦 CIB Internship Project (Commercial International Bank)

A professional-grade **ASP.NET Core Web API** developed during my internship at **CIB**. This project demonstrates a secure, scalable approach to managing bank-related physical assets (Real Estate), customer relationships, and automated payment tracking.

---

## 🚀 Project Overview
The **CustomerFluent** system was designed to bridge the gap between customer data and property management. It allows administrators to track buildings, individual housing units, and the tenants (customers) occupying them, while maintaining a real-time ledger of payment statuses.

### Key Features
* **Asset Hierarchy:** Manage complex data relationships: `Building` -> `House` -> `Customer`.
* **Payment Lifecycle Tracking:** Automated status updates for monthly payments and outstanding balances.
* **Enterprise Security:** Secure Identity management using **JWT (JSON Web Tokens)** and Role-Based Access Control (RBAC).
* **Data Integrity:** Implementation of **FluentValidation** to ensure bank-grade data quality for all incoming requests.

---

## 🛠️ Tech Stack & Architecture
This project follows **Clean Architecture** principles to ensure the code is maintainable and testable.

* **Framework:** .NET 8.0 (ASP.NET Core Web API)
* **Database:** SQL Server via **Entity Framework Core**
* **Design Patterns:** Repository Pattern, Unit of Work, and DTO (Data Transfer Object) mapping.
* **Libraries:** * **AutoMapper:** For decoupling internal entities from API responses.
    * **FluentValidation:** For robust, server-side business logic validation.
    * **ASP.NET Core Identity:** For managing user authentication and security.

---

## 📂 Repository Structure
* **`Controllers/`**: RESTful endpoints for Auth, Customers, and User Management.
* **`Models/`**: Domain entities representing the core business logic (`Building`, `House`, `PaymentStatus`).
* **`DTOs/`**: Optimized data shapes for API consumers.
* **`Validators/`**: Business rules and validation logic.
* **`Migrations/`**: Database schema versioning.

---

## ⚙️ Getting Started

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server (LocalDB or Express)

### Installation
1. **Clone the repository:**
   ```bash
   git clone [https://github.com/YOUR_USERNAME/CustomerFluent.git](https://github.com/YOUR_USERNAME/CustomerFluent.git)
