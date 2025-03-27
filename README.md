# Employee Management System (EMS)

## Overview
EMS (Employee Management System) is a web-based application built using **ASP.NET Core Web API** with **Entity Framework Core**, **JWT Authentication**, and **Role-Based Access Control (RBAC)**. The system allows **Admins** to manage employees, track leaves, handle timesheets, and generate reports.



## Features 🎯
- User authentication & authorization (Employee/Admin)
- CRUD operations for Employees & Departments
- Timesheet management
- Leave application system
- Reporting and analytics
- Admin Dashboard

## *Technology Stack*
- *Backend:* ASP.NET Core Web API (C#)
- *Database:* SQL Server (EF Core)
- *Authentication:* JWT (JSON Web Tokens)
- *Dependency Injection:* Scoped services
- *Logging:* Serilog (optional)
- *Frontend (Future Scope):* Angular / React

---
# Project Structure
```
📦 EMS (Solution)
│
├── 📂 EMS.API
│   ├── 📂 Controllers
│
├── 📂 EMS.Application
│   ├── 📂 DTOs
│   ├── 📂 Interfaces(Services)
│   ├── 📂 Services
│
├── 📂 EMS.Domain
│   ├── 📂 Entities
│   ├── 📂 Interfaces(Repositories)
│
├── 📂 EMS.Infrastructure
│   ├── 📂 Data
│   ├── 📂 Migrations
│   ├── 📂 Repositories
│
├── 📂 EMS.Shared
│   ├── 📂 Helpers
│
├── 📂 Properties
├── 📄 appsettings.json
├── 📄 Program.cs
```
# Employee Management System API 📌

This API provides authentication, user management, timesheet tracking, leave management, and reporting functionalities.

## Authentication 🔑
| Method | Endpoint | Description |
|--------|---------|-------------|
| **POST** | `/api/Auth/Login` | User login with email & password |
| **POST** | `/api/Auth/Logout` | User can logout |
| **POST** | `/api/Auth/send-reset-link` |send Reset user password invitation |
| **POST** | `/api/Auth/reset-password` |Reset user password using that link|

---

## Department Management 📁
| Method | Endpoint | Description |
|--------|---------|-------------|
| **GET** | `/api/Department/Get-All-Departments` | Get all departments With Employee Details |
| **GET** | `/api/Department/Get-All-Departments-By-Id{{id}}` | Get department by ID |
| **POST** | `/api/Department/Create-Department` | Create a new department |
| **PUT** | `/api/Department/Update-Department-By-Id{{id}}` | Update department details |
| **DELETE** | `/api/Department/Delete-Department-By-Id{{id}}` | Delete a department |

---

## User Management👨‍💼
| Method | Endpoint | Description |
|--------|---------|-------------|
| **POST** | `/api/User/CreateAdmin` | Create Admin |
| **POST** | `/api/User/CreateEmployee` | Create Employee |
| **GET** | `/api/User/Get-All-Admins` | Get all Admins |
| **GET** | `/api/User/Get-All-employees` | Get all Employees |
| **PUT** | `/api/User/update-Employee-By-Admin/:empId` | Update employee by Admin |
| **PUT** | `/api/User/activation-deactivation` | Activate-Deactivate Employee |
| **GET** | `/api/User/My-profile` | Get user profile(User id will be taken from token) |
| **GET** | `/api/User/:employeeId/details-with-timesheets` | Timesheet of the Employee By Employee Id |
| **GET** | `/api/User/:employeeId/export-timesheets` |Export Timesheet of the Employee By Employee Id |
| **PUT** | `/api/User/update-profile` |  Update employee by Employee |

---

## Leave Management 📆
| Method | Endpoint | Description |
|--------|---------|-------------|
| **POST** | `/api/Leave/apply-Leave` | Apply for leave |
| **POST** | `/api/Leave/approve-reject-leave` | Aproove or reject leave by Admin |

---

## Dashboard  📁
| Method | Endpoint | Description |
|--------|---------|-------------|
| **GET** | `/api/dashboard/employee-Dashboard/:employeeId` | Get Employee Basic Deatils with latest timesheets and Leave Balance|
| **GET** | `/api/dashboard/admin-Dashboard` | Get the pending leaves of  the employees  |
| **GET** | `/api/dashboard/most-active-Employee` | Get the most active employee(Working hours) |
| **GET** | `/api/dashboard/leave-analytics-of-Employee` | Get each employees leave days |

---
## Reports 📊
| Method | Endpoint | Description |
|--------|---------|-------------|
| **GET** | `/api/Reports/employee-work-hours-report` | Get employees weekly or monthly reports with the week number and month number using start date and end date|

---

## Timesheet Management 🕒
| Method | Endpoint | Description |
|--------|---------|-------------|
| **GET** | `/api/Timesheet/MyTimeSheets` | Get all logged in Employee timesheets |
| **GET** | `/api/Timesheet/Time-Sheet-By-Id{{id}}` | Get timesheet of logged in user  by TimesheetId  |
| **POST** | `/api/Timesheet` | Create a new timesheet entry |
| **PUT** | `/api/Timesheet` | Update timesheet entry |

---
## Setup Instructions 🛠️


1. Clone the repository:
```sh
   git clone https://github.com/VasuKansagaraBacancy/EMS-Backend.git
```  
2. Navigate to the project directory:  
```sh
   cd EMS--Backend-REST-API
```  
3. Restore dependencies:  
```sh
   dotnet restore
```
4. Configure the database in appsettings.json.
5. Run database migrations:  
```sh
   dotnet ef database update
```  
6. Start the API:  
```sh
   dotnet run
```   
7. Access Swagger UI at:  
   
    http://localhost:7006/swagger/index.html



