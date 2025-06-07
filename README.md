# CleanArchitectureDemo-Catalog

A modular .NET Web API built with **Minimal APIs**, **Clean Architecture**, and **CQRS** using **MediatR**, **FluentValidation**, and **In-Memory DB**.

> **"Build a .NET Web API with Clean Architecture (SQL Backend, Swagger/Scalar, JWT)"**

---

## 🚀 Features

✅ Minimal APIs using .NET 9 
✅ Clean Architecture (Application, Infrastructure, API layers)  
✅ CQRS with MediatR  
✅ JWT-based Authentication & Role-based Authorization  
✅ FluentValidation (with manual triggering in Minimal API)  
✅ Caching for product queries  
✅ In-Memory database for demo simplicity  
✅ Ready for SQL backend swap  
✅ Sliced folder structure for better maintainability

---

## 🧱 Tech Stack

| Component       | Library/Framework                        |
|----------------|-------------------------------------------|
| .NET Runtime    | [.NET 9](https://dotnet.microsoft.com)   |
| API             | Minimal API (no controllers)             |
| Architecture    | Clean Architecture                       |
| ORM             | EF Core with In-Memory DB                |
| CQRS            | MediatR                                  |
| Validation      | FluentValidation                         |
| Auth            | JWT (with Roles)                         |
| Caching         | IMemoryCache                             |

---

## 🗂️ Folder Structure
```bash
/src
/Catalog.API --> Minimal API setup
/Catalog.Application --> Business logic, CQRS, DTOs, Validators
/Catalog.Infrastructure --> EFCore, Services, Auth, Seeding
```
---

## 🧪 How to Run

1. **Clone this repository**  
   ```bash
   git clone https://github.com/srielango/CleanArchitectureDemo-Catalog.git
   
   
2. **Open in Visual Studio 2022+**

3. **Run the API**
The app starts at https://localhost:5001 or http://localhost:5000.

4. **Test Auth**
```bash
Use /api/login to obtain a JWT token
```
Use the token to access protected endpoints like POST, PUT, DELETE /products

## 📦 Seeding Info
The API seeds:

## 📦 Products: A list of sample products

## 👤 Users:
```bash
Admin: admin / password with role: Admin
User: viewer / password with role: Viewer
```
## 📘 Sample Requests
🔐 Login (to get JWT token)

POST /api/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password"
}

## 📦 Get All Products (no auth needed)
GET /products

## ➕ Create Product (JWT Required)
POST /products
Authorization: Bearer <your_token_here>
Content-Type: application/json

{
  "name": "New Product",
  "description": "Great product",
  "price": 99.99
}

## 📌 Notes
Automatic validation is not supported by Minimal APIs — FluentValidation is triggered manually.
Swagger is replaced by Scalar for API exploration (can be added optionally).
Easily extendable to SQL Server by changing the DB provider in Program.cs.
