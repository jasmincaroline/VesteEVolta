# VesteEVolta API

VesteEVolta is a REST API developed using **ASP.NET Core** as part of the **CodeRDiversity Program**.

The project focuses on building a structured and scalable backend application, applying best practices such as layered architecture, use of DTOs, dependency injection, and automated testing.



## Technologies Used

- .NET  
- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server  
- NUnit (Unit Testing)  
- Swagger (OpenAPI)  



## Architecture Overview

The project follows a layered architecture pattern to ensure separation of concerns, maintainability, and scalability.

###  Folder Structure

```bash
VesteEVolta/
├── Controllers/
├── DTO/
├── Services/
│   └── Interfaces/
├── Models/
├── Data/
├── Migrations/
├── Program.cs
├── appsettings.json
└── VesteEVolta.csproj

```
###  Layer Responsibilities

- **Controllers** → Manage API endpoints  
- **Services** → Contain business rules and application logic  
- **DTOs** → Control data exposure between layers  
- **Models** → Represent database entities  
- **Data** → Handle database access via Entity Framework Core  
- **Tests** → Validate business logic using NUnit  

---
This project was developed by **[Ester Souza](https://www.linkedin.com/in/estersouza/)** and **[Jasmin Caroline](https://www.linkedin.com/in/jasmincaroline)** during the **CodeRDiversity Program**,  
taught by **[Camille Gachido](https://www.linkedin.com/in/camille-gachido/)**,  
powered by **[Prosper Digital Skills](https://prosperdigitalskills.com)**,  
and sponsored by **[RDI Software](https://www.rdisoftware.com/)**.



