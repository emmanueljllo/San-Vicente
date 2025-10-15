#  San Vicente Hospital System

Medical appointment management system developed in C# (.NET 6) with Console, Entity Framework Core, and MySQL, designed to digitize and optimize appointment management at San Vicente Hospital.
The goal is to ensure the integrity, consistency, and accessibility of information.

---

##  Technologies used

- C# (.NET 6) — Console Application
- Entity Framework Core
- MySQL
- List<> and Dictionary<TKey, TValue>
- LINQ

---

##  Prerequisites

Before running the project, make sure you have installed:

| Requirement | Recommended version |
|-----------|-------------|
| .NET SDK | 6.0 or higher |
| MySQL Server | Compatible with the configured port |
| Entity Framework Core Tools | `dotnet tool install --global dotnet-ef` |

---

##  Database Configuration

The system connects to MySQL with the following configuration:

DB_CONNECTION=mysql
DB_HOST=168.119.183.3
DB_PORT=3307
DB_DATABASE=eloquent
DB_USERNAME=root
DB_PASSWORD=g0tIFJEQsKHm5$34Pxu1

yaml
Copy code

This string must be configured in `appsettings.json` or within the context of EF Core depending on your implementation.

---

##  Installation and Execution

Follow these steps to set up the project from scratch:

```bash
# 1. Clone the repository
git clone https://github.com/emmanueljllo/San-Vicente.git
cd San-Vicente-master

# 2. Apply migrations (if using EF Core)
dotnet ef migrations add InitialMigration
dotnet ef database update

# 3. Run the application
dotnet run
 Example execution
txt
Copy code
===== SAN VICENTE HOSPITAL SYSTEM =====

1. Register patient
2. Schedule medical appointment
3. List patients
4. Check appointments
5. Exit

Select an option:
(This menu is representative. If the actual menu is different, change it.)

 Diagrams
System diagrams are located at the folder:

Copy code
/Diagrams
Includes:

 Class diagram

 Use case diagram

 Coder Information
Field Information
Name Emmanuel Jaramillo
Clan / Cohort 4 / C#.NET
Email emmanueljaramill@gmail.com
Document 1022148449

 Expected deliverables
 Public repository on GitHub

 Project in .zip file

 Diagrams included

 Complete README with clear instructions
