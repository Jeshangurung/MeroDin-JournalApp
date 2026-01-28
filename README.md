# Secure Digital Journal Application

A secure, full-stack digital journal application developed using **.NET MAUI Blazor**, designed to support multi-user journaling with strong authentication, clean architecture, and responsive UI. The project demonstrates modern software engineering practices including layered architecture, dependency injection, and secure data handling.

---

## 📌 Project Objectives

- Develop a secure and user-friendly digital journal system  
- Implement multi-user data isolation  
- Apply clean, layered architecture principles  
- Ensure strong authentication and authorization  
- Build a responsive and consistent UI using Blazor  
- Demonstrate full-stack development skills using .NET technologies  

---

## 🛠️ Technology Stack

- **Frontend:** .NET MAUI Blazor  
- **Backend:** C# (.NET)  
- **Database:** SQLite / Entity Framework Core  
- **ORM:** EF Core with LINQ  
- **Authentication:** PIN-based two-tier authentication  
- **Security:** SHA-256 hashing  
- **Architecture:** Layered architecture (UI, Services, Data)  

---

## 🧩 Key Features

- Secure PIN-based authentication  
- Multi-user support with strict UserId-based data isolation  
- Create, edit, delete, and manage journal entries  
- Optimized calendar view for journal navigation  
- Responsive UI with consistent theme design  
- Clean separation of concerns using services and repositories  

---

## 🏗️ Architecture Overview

The application follows a **layered architecture**:

- **Presentation Layer:** Blazor components (UI & state handling)  
- **Service Layer:** Business logic and application services  
- **Data Layer:** EF Core DbContext and repositories  

This approach improves maintainability, scalability, and testability.

---

## 🔐 Security Implementation

- Two-tier authentication using a secure PIN system  
- SHA-256 hashing for PIN storage  
- Authorization checks for all user-specific data  
- Session-based access control  
- No direct exposure of sensitive data  

---

## 🧪 Testing & Quality Assurance

- Manual testing for functionality and UI validation  
- Debugging and performance optimization  
- Error handling and defensive programming practices  

> Note: Automated unit testing was not implemented and is identified as a future improvement.

---

## 🚧 Limitations

- No automated unit tests (e.g., xUnit)  
- Minimal caching implementation  
- Basic logging only  
- No cloud backup or synchronization  

---

## 🚀 Future Enhancements

- Implement Test-Driven Development (TDD)  
- Add CI/CD pipelines  
- Integrate ASP.NET Core Web APIs  
- Add real-time features using SignalR  
- Containerize using Docker  
- Deploy using Azure cloud services  
- Implement cloud backup and synchronization  

---

## 👤 Author

**Jeshan Gurung**  
 
Third Year Application Development Module Project – Secure Digital Journal Application  

---

## 📄 License

This project is developed for **educational purposes**.  
All rights reserved.
