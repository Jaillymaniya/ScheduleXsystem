# 📅 ScheduleX - Smart Timetable Generator

## 📌 Project Overview

ScheduleX is a **smart timetable generation system** designed to automate the scheduling process for educational institutions.
It efficiently manages **courses, faculty, rooms, and subjects** to generate optimized timetables with minimal conflicts.

---

## 🚀 Features

* 🔐 User Authentication (Admin / TT Coordinator)
* 🏫 Department & Course Management
* 📚 Subject & Semester Management
* 👨‍🏫 Faculty Assignment
* 🏢 Room Allocation System
* ⏱️ Time Slot & Break Configuration
* 📊 Automatic Timetable Generation
* 🧩 Conflict Handling (Faculty, Room, Time)
* 📤 Export & Template Support (Planned)

---

## 🏗️ Project Architecture

This project follows **Clean Architecture**:

* **ScheduleX.Core** → Entities & Domain Models
* **ScheduleX.Infrastructure** → Database & Repositories
* **ScheduleX.Web** → UI (ASP.NET Core / Blazor)

---

## 🛠️ Technologies Used

* ASP.NET Core (.NET 9)
* Entity Framework Core
* SQL Server
* Blazor (UI)
* C#

---

## 🗄️ Database Design

The system includes key entities like:

* AcademicYear
* Department
* Course
* Semester
* Subject
* Faculty
* Room
* ScheduleConfig
* TimeSlot
* TimeTableBatch
* TimeTableEntry

---

## ⚙️ Setup Instructions

### 1️⃣ Clone the Repository

```
git clone https://github.com/your-username/schedulex.git
```

### 2️⃣ Open in Visual Studio

* Open `ScheduleX.sln`

### 3️⃣ Configure Database

* Update connection string in:

```
appsettings.json
```

### 4️⃣ Run Migrations

```
Add-Migration InitialCreate
Update-Database
```

### 5️⃣ Run Project

* Press **F5**

---

## 📌 Future Enhancements

* 📊 Advanced timetable optimization algorithm
* 📱 Mobile application support
* 📤 Export to PDF/Excel
* 🤖 AI-based scheduling

---

## 👨‍💻 Team Members

* Jeli Maniya
* Hetvi Vamja
* Shruti Hansaliya

---

## 📜 License

This project is developed for educational purposes.

---
