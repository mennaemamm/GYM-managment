# 🏋️ Gym Management System
A full-stack web application for managing gym operations — members, trainers, sessions, and membership plans — built with **ASP.NET Core MVC**.

![Dashboard Preview](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=dotnet) ![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-6DB33F) ![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?logo=microsoftsqlserver)

## 📋 Overview

The Gym Management System centralizes the day-to-day operations of a gym: registering members, managing trainers, scheduling and booking training sessions, and handling membership plans and subscriptions — all through a clean, role-based web interface.

**Goals:**
- Centralize member and plan management
- Manage trainers and session schedules

---

## ✨ Features

- **Trainer Management** — Full CRUD operations
- **Member Management** — Add, update, delete, and view members
- **Plans Management** — Update, deactivate (soft delete), and view plans
- **Membership Management** — Assign training plans to members
- **Session Management** — Full CRUD operations
- **Session Scheduling** — Organize and assign sessions with trainers
- **Session Booking** — Organize and book sessions with members
- **Dashboard** — Analytics and reports
- **Identity & Authentication** — Login, role-based access, and access control via ASP.NET Identity

---

## 🏗️ Architecture

Built using a **three-layer architecture**:

| Layer | Responsibility |
|---|---|
| **Presentation Layer** | ASP.NET MVC Controllers + Razor Views (Bootstrap for UI) |
| **Business Logic Layer** | Services (e.g., `TrainerService`, `SessionService`) containing core logic |
| **Data Access Layer** | Repository pattern wrapping the EF Core `DbContext` |

---

## 🛠️ Technology Stack

- **Backend:** ASP.NET Core MVC
- **ORM:** Entity Framework Core
- **Database:** Microsoft SQL Server
- **Frontend:** Razor Views + Bootstrap, Custom CSS
- **Patterns:** Repository Pattern, Unit of Work, Dependency Injection
- **Libraries:** AutoMapper (ViewModel ↔ Entity mapping)

---

## 🗂️ Domain Model

### Core Entities

**GymUser** *(abstract base)* — shared by Members and Trainers
- Name, Email, Phone, DateOfBirth, Gender
- Embedded Address (Building No, Street, City)

**Member** *(extends GymUser)*
- Photo, JoinDate (auto-set on insertion)
- Relationships: one `HealthRecord`, subscribes to one `Plan` at a time, attends many `Session`s

**Trainer** *(extends GymUser)*
- Specialties (enum: `GeneralFitness`, `Yoga`, `Boxing`, `CrossFit`), HireDate (auto-set)
- Relationships: conducts many `Session`s

**HealthRecord**
- Height, Weight, BloodType, Note, LastUpdate
- Belongs to exactly one Member

**Plan**
- Name, Description, DurationDays (1–365), Price, IsActive
- Can be assigned to many Members

**Category** *(seeded data)*
- CategoryName — associated with many Sessions

**Session**
- Description, Capacity (1–25), StartDate, EndDate
- Conducted by one Trainer, belongs to one Category, attended by many Members

### Supporting / Junction Entities

- **Booking** *(Member ↔ Session)* — BookingDate, IsAttended
- **Membership** *(Member ↔ Plan)* — StartDate, EndDate

### Identity Entities

- **ApplicationUser** — Id, FirstName, LastName, UserName, Email, Phone
- **IdentityRole** — Id, Name, NormalizedName, ConcurrencyStamp

---

## 📐 Entity Relationship Diagram

```
Member  ──1:1──  HealthRecord
Member  ──M:1──  Plan            (via Membership)
Member  ──M:M──  Session         (via Booking)
Trainer ──1:M──  Session
Category──1:M──  Session
```

See the full ER diagram and database schema in the project documentation (`Gym_Management_System.pdf`).

---

## 📏 Business Rules

### Member Management
- Email and phone must be unique and valid
- Egyptian phone format enforced: `(010|011|012|015)XXXXXXXX`
- Cannot delete members with active bookings
- Health record required at registration
- JoinDate is calculated automatically

### Trainer Management
- Email and phone must be unique and valid
- Cannot delete trainers with future sessions
- Must have exactly one specialty assigned
- HireDate is calculated automatically

### Session Management
- Capacity limited to 1–25 (enforced by database constraint)
- EndDate must be after StartDate
- A valid Trainer and Category are required
- Cannot delete sessions with future dates

### Plan Management
- Cannot update or deactivate a plan with active memberships
- Plans can be activated/deactivated (soft delete)
- Duration must be between 1 and 365 days

### Booking Rules
1. Member must have an active membership to book a session
2. Session must have available capacity
3. A member cannot book the same session twice
4. Only future sessions can be booked
5. Only future bookings can be cancelled
6. Attendance can only be marked for ongoing sessions (started but not yet ended)
7. `IsAttended` defaults to `false` on creation
8. Booking, cancellation, and attendance actions require the referenced booking/session to exist

### Membership Rules
1. Membership can only be created for an existing member and an existing, active plan
2. A member cannot hold more than one active membership at a time
3. `EndDate` is auto-calculated as `StartDate + Plan.DurationDays`
4. Status is computed dynamically: `Active` if `EndDate > Now`, otherwise `Expired`
5. Cancelling a plan removes memberships tied to it
6. A membership can only be deleted while it is active

---

## 🧩 MVC Structure

### Controllers
- **HomeController** — Landing page with gym stats
- **MemberController** — Member CRUD, profile, and health record views
- **TrainerController** — Trainer CRUD and profile views
- **SessionController** — Session CRUD, calendar/list views
- **PlanController** — Plan listing, details, edit, activate/deactivate
- **MemberPlanController** — Membership creation, listing, cancellation
- **MemberSessionController** — Session booking, cancellation, attendance listing (upcoming/ongoing)
- **AccountController** — Login, logout, access-denied handling (ASP.NET Identity)

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (8.0 or later recommended)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or full instance)
- Visual Studio 2022 / VS Code

### Setup

```bash
# Clone the repository
git clone https://github.com/RahmaAta/GymManagement.git
cd GymManagement

# Restore dependencies
dotnet restore

# Update the connection string in appsettings.json to point to your SQL Server instance

# Apply EF Core migrations
dotnet ef database update

# Run the application
dotnet run
```

The app will be available at `https://localhost:5001` (or the port configured in `launchSettings.json`).


## 📄 License

This project was built as part of the Route ASP.NET Course.
