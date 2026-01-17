# ⚡ Quick Approval System

<p align="center">
  
<img width="100" height="400" alt="deepseek_mermaid_20260108_443a6f" src="https://github.com/user-attachments/assets/c57beb9f-cfe1-4e40-9089-85ddc20d2848" />

 
</p>

> A **modern, enterprise-grade approval workflow system** built with **.NET Core**, **Domain-Driven Design (DDD)**, and **Clean Architecture**.  
> Designed for scalability, security, and real-world business workflows.

---

## 🌟 Why Quick Approval System?

Organizations often rely on emails, spreadsheets, or manual approvals that are:
- ❌ Hard to track
- ❌ Error-prone
- ❌ Not auditable
- ❌ Slow for managers

**Quick Approval System** solves this by providing:

✅ Configurable approval workflows  
✅ Role-based dashboards (Employee / Manager / Admin)  
✅ Full audit trail & system logs  
✅ Fast, secure, and structured approvals  

---

## 🔄 Project Flow (High Level)

The system follows a **clear, role-driven flow**:

1️⃣ User logs in → Authentication  
2️⃣ Role selection (Employee / Manager / Admin)  
3️⃣ Dashboard loaded based on role  
4️⃣ Workflow-driven actions executed  
5️⃣ Full audit & history maintained  

📌 The diagram above visually represents **every screen and decision point** in the system.

---

## 👥 User Roles & Responsibilities

### 🧑‍💼 Employee
- Create new approval requests 📝  
- Fill and validate request forms ✔️  
- Submit requests for approval 🚀  
- View personal request history 📚  
- Track approval status in real time ⏱️  

---

### 🧑‍💻 Manager
- View pending approvals 📥  
- Open approval details 🔍  
- Approve or reject requests ⚖️  
- View request history & audit trail 📜  
- Process approvals quickly (Quick Action Flow) ⚡  

---

### 🛡️ Admin
- Configure approval workflows 🔄  
- Add/Edit workflow steps 🧩  
- Manage users & roles 👤  
- View system logs & audit data 🔎  
- Maintain system integrity 🛠️  

---

## 🏗️ Architecture Overview

This project strictly follows **DDD + Clean Architecture** principles.


### 🧠 Key Architectural Benefits
- 🔒 Business logic isolated from frameworks
- 🧪 Easy unit testing of domain rules
- 🔁 Infrastructure can be swapped without impact
- 📈 Scales naturally as features grow

---

## 🧩 Core Domain Concepts

### 🧱 Aggregates
- **User** – Identity, roles, access control  
- **Request** – Approval request lifecycle  
- **Workflow** – Configurable approval steps  

### 📌 Value Objects
- Request Status  
- Approval Decision  
- Workflow Step  

### 📜 Business Rules
- Requests must follow configured workflows  
- Only authorized roles can approve steps  
- Requests cannot skip approval stages  
- Rejected requests are immediately closed  

---

## 🔐 Security & Authentication

- 🔑 JWT-based authentication
- 🧾 Role-based authorization (Employee / Manager / Admin)
- 🔒 Passwords hashed using BCrypt
- 🚫 Unauthorized actions blocked at API level

---

## 🛠️ Technology Stack

| Layer | Technology |
|-----|-----------|
| Backend | .NET 8 / ASP.NET Core |
| Architecture | DDD + Clean Architecture |
| Database | SQL Server + EF Core |
| Auth | JWT + Role Claims |
| Validation | FluentValidation |
| CQRS | MediatR |
| Logging | Database-backed System Logs |
| Testing | xUnit + FluentAssertions |


