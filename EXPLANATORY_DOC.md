# Organization Chart System — Explanatory Document

**Submission date:** July 2026
**Author:** Md-Amdadulhaque

---

## 1. Overview

The **Organization Chart System** is a full-stack web application for managing a company's
departments and employees and visualizing the reporting structure as an interactive
organization chart. Administrators can import department and employee data from Excel files
(the format exported by ServiceNow), maintain that data through the UI, and everyone can
explore it through a dashboard and a collapsible org chart.

The application supports **role-based access**: administrators manage data; viewers have
read-only access.

---

## 2. Technology stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | Angular v22 (standalone components, **zoneless** change detection with signals), Angular Material |
| **Backend** | ASP.NET Core Web API (.NET 10) |
| **Data access** | Entity Framework Core 10 (code-first migrations) |
| **Database** | SQL Server |
| **Excel parsing** | ClosedXML |
| **Authentication** | JWT bearer tokens; PBKDF2 password hashing |

**Solution structure**

```
OrganizationChart/
├── backend/
│   ├── OrganizationChart.Api/         # Web API: controllers, services, DTOs, auth, migrations
│   └── OrganizationChart.Domain/      # Domain entities (Department, User, AuthUser, history)
└── frontend/
    └── organization-chart-ui/         # Angular app (features, core services/models/guards)
```

---

## 3. Features

### 3.1 Authentication & authorization
- Login screen; JWT issued on success and stored client-side.
- Two roles: **Admin** (full create/update/delete + Excel import) and **Viewer** (read-only).
- Enforced on **both** ends: `[Authorize]` attributes on the API, plus route guards and hidden
  UI controls on the client. A viewer who tries an admin URL is redirected; a viewer's API
  write returns **403**.

### 3.2 Departments
- List, create, edit, and delete departments.
- Each department has a code, a name, and an optional parent department (self-referencing
  hierarchy).
- Deletion is **blocked** (HTTP 409 with a clear message) while a department still has members
  or sub-departments, preventing accidental orphaning.

### 3.3 Users (employees)
- List, create, edit, and delete employees.
- Each user has an employee number, first/last name, title, and a department.

### 3.4 Excel import
- Admins upload the source spreadsheets (`cmn_department.xlsx`, `sys_user.xlsx`).
- The reader maps columns **by header name** (not fixed position), so it is robust to the
  many-column ServiceNow exports and to column reordering.
- Departments are imported first (with parent relationships resolved by name), then users
  (linked to their department by name). Duplicate rows are skipped.

### 3.5 Dashboard
- Key metrics: total users, total departments, top-level departments, average users per
  department.
- Two CSS bar charts: **users per department** and **top job titles**.
- All figures are computed live from the API.

### 3.6 Organization chart
- A recursive, collapsible tree of the department hierarchy.
- Each department shows its direct headcount and its **total** headcount (including all
  sub-departments), and can expand to list its members.
- Expand-all / collapse-all controls.

---

## 4. Data model

```
Department
  Id, DepartmentCode, Name, ParentDepartmentId? ──┐ (self-reference: parent / children)
                                                  │
User (employee)                                   │
  Id, EmployeeNumber, FirstName, LastName, Title, DepartmentId ──> Department

AuthUser (login account — separate from employees)
  Id, Username, PasswordHash, Role, CreatedAt
```

- Foreign keys use **restrict** on delete, so a department cannot be removed while it is
  referenced by users or child departments.
- `AuthUser` is intentionally separate from the employee `User` table: login accounts and
  imported HR records are different concerns.

---

## 5. API surface

| Method & route | Access | Purpose |
|----------------|--------|---------|
| `POST /api/auth/login` | Public | Log in, receive a JWT |
| `POST /api/auth/register` | Admin | Create a new account |
| `GET /api/auth/me` | Authenticated | Current identity |
| `GET /api/departments` · `GET /api/departments/{id}` | Authenticated | Read |
| `POST/PUT/DELETE /api/departments…` | Admin | Write |
| `GET /api/users` · `GET /api/users/{id}` | Authenticated | Read |
| `POST/PUT/DELETE /api/users…` | Admin | Write |
| `POST /api/import/departments` · `/api/import/users` | Admin | Excel import |
| `GET /api/organizationchart` | Authenticated | Full department tree with members |

---

## 6. Notable design decisions

- **Zoneless Angular + signals.** The app runs without `zone.js`. All data the UI renders from
  an async source is held in Angular **signals**, so views update correctly without Zone-based
  change detection.
- **Header-based Excel mapping.** Because the source files have dozens of columns, the importer
  resolves each needed field by its header text rather than a hard-coded column index.
  *(One data note: the source `Employee number` column is empty, so the per-person identifier
  is taken from the `User ID` column — configurable via a constant.)*
- **Defense in depth for authorization.** The UI hides actions a viewer cannot perform, but the
  API independently enforces roles, so the rules hold even if the client is bypassed.
- **Safe deletes.** Restrictive foreign keys plus a friendly 409 response prevent accidental
  data loss.

---

## 7. How to run

**Prerequisites:** .NET 10 SDK, Node.js + npm, SQL Server, and a connection string in
`backend/OrganizationChart.Api/appsettings.json`.

**Backend** (from `backend/OrganizationChart.Api`):
```bash
dotnet run --urls http://localhost:5044
```
On first run it applies EF migrations and seeds the default admin account.

**Frontend** (from `frontend/organization-chart-ui`):
```bash
npm install
npm start
```
Open `http://localhost:4200`.

**Default login:** `admin` / `Admin@123`

**First-time data load:** log in as admin → **Import** → upload `cmn_department.xlsx`, then
`sys_user.xlsx`.

---

## 8. Security note (for production)

The `Jwt:Key` and `SeedAdmin:Password` values in `appsettings.json` are development
placeholders. Before any real deployment, move them to environment variables / user-secrets
and rotate the signing key.

---

*A companion internal build log with per-file change history is available in `WORK_LOG.md`.
A demo walkthrough for the presentation video is in `PRESENTATION_SCRIPT.md`.*
