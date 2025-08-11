# Ecommerce_BE_API

This is the backend API for an **Ecommerce Demo Platform**, developed using **ASP.NET Core**. It provides RESTful API services to handle core business logic and database operations.

## 🔧 Tech Stack

- **Framework**: ASP.NET Core 8
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Token)
- **Architecture**: Layered (Controller - Service - Repository - Database)
- **Documentation**: Swagger

## 🚀 Features

- ✅ JWT-based Authentication & Authorization
- ✅ Product Management (CRUD)
- ✅ Category Management
- ✅ User & Customer Management
- ✅ Filtering, Sorting, and Pagination
- ✅ Soft Delete Support
- ✅ Swagger Integration for API testing

## 🔗 Live Demo

You can try the live API with Swagger at:

- 👉 [https://iamkazu.bsite.net/swagger](https://iamkazu.bsite.net/swagger)
- 👉 user: admin/123456

## 📦 Frontend

The frontend is currently under development and will connect to this API. You can follow its progress here:

👉 Frontend Repository: [https://github.com/syhung382/ecommerce-demo-webpage](https://github.com/syhung382/ecommerce-demo-webpage)

## 🔧 Getting Started

To get started with this project, follow these steps:

### Prerequisites

```sh
 - [.NET 8 SDK](https://dotnet.microsoft.com/download)
 - SQL Server instance (local or remote)
```

### **1️⃣ Clone the Repository**

```sh
git clone https://github.com/iftykhar/E-commerce-Hekto.git
cd ecommerce-demo-webpage
```

### Configure appsetting.json
```sh
 {
  "MainConnectionString": "Server=YOUR_SQL_SERVER;Database=YOUR_DATABASE_NAME;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;",
  
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },

  "AllowedHosts": "*",

  "Tokens": {
    // Secret key used to sign JWT tokens
    "Key": "YOUR_SECRET_KEY_HERE",
    // Issuer name used in JWT validation
    "Issuer": "YOUR_ISSUER_NAME",
    // Optional: used for user-specific token validation
    "KeyUser": "YOUR_USER_KEY_SECRET"
  }
}
```

### Apply migrations:
```sh
dotnet ef database update
```

### Run the application:
```sh
dotnet run
```

### Open Swagger for API testing at:
```sh
https://localhost:<port>/swagge
```

## 📌 Notes
This is an ongoing project.

Integration between frontend and backend is done via RESTful APIs.

Swagger UI is enabled for rapid testing and documentation.

## 📜 License

This project is licensed under the **MIT License**.

## 📬 Contact

For any queries or collaboration opportunities, feel free to reach out:

- 📧 Email: iamkazu382@gmail.com
- 🔗 GitHub: https://github.com/syhung382
- 🔗 Upwork: [upword](https://upwork.com/freelancers/~01698b265175ff407b)

Made with ❤️ by [@syhung382](https://github.com/syhung382)