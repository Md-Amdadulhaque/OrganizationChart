# Organization Chart — Work Log

This document details all the work done on the Organization Chart application during this session.
It is meant to be read top-to-bottom to understand *what* was built, *why*, and the *key gotchas* discovered along the way.

**Stack:** ASP.NET Core (.NET 10) Web API + EF Core (SQL Server) backend · Angular v22 (zoneless, standalone components, Angular Material) frontend.

---

## Table of contents

1. [User feature (frontend CRUD)](#1-user-feature-frontend-crud)
2. [Department delete](#2-department-delete)
3. [Import feature fix](#3-import-feature-fix)
4. [The zoneless change-detection bug (important!)](#4-the-zoneless-change-detection-bug-important)
5. [Excel reader rewrite (real file structure)](#5-excel-reader-rewrite-real-file-structure)
6. [Dashboard](#6-dashboard)
7. [Organization chart](#7-organization-chart)
8. [Authentication & authorization](#8-authentication--authorization)
9. [Default credentials & how to run](#9-default-credentials--how-to-run)
10. [Known follow-ups / cleanup](#10-known-follow-ups--cleanup)

---

## 1. User feature (frontend CRUD)

**Goal:** Build the User part of the UI, following the existing Department pattern.

The backend (controller, service, DTOs, model) already existed for Users. The frontend was only stubs, so the following was built:

| File | What it does |
|------|--------------|
| `core/services/user.service.ts` | **Was broken** (`@Service` — an invalid decorator). Rewritten as a proper `UserService` with `getAll / getById / create / update / delete`, mirroring `DepartmentService`. |
| `features/users/user-list/user-list.ts` + `.html` | Material table listing users (Employee #, First/Last name, Title, Department) with Add/Edit/Delete actions. |
| `features/users/user-form/user-form.ts` + `.html` | Reactive form handling **both create and edit** (via the `:id` route param). Because `departmentId` is required, it loads departments via `DepartmentService` into a `mat-select` dropdown. |
| `app.routes.ts` | Added `users/create` and `users/edit/:id` routes; removed a duplicate `departments/edit/:id` entry. |

**Bonus bug fixed:** `department-form.html` called `OnSubmit()` but the method is `onSubmit()`. This typo was **breaking the entire build**, so it was corrected.

---

## 2. Department delete

**Goal:** Wire up the Department list's Delete button (the `delete()` service method existed but the button did nothing).

- `department-list.ts` — added `deleteDepartment(id)`: shows a confirmation dialog, calls `departmentService.delete(id)`, then reloads the list.
- `department-list.html` — connected the existing button via `(click)="deleteDepartment(d.id)"`.

---

## 3. Import feature fix

**Goal:** Fix the broken Import feature.

- `features/import/import.ts` — **the four methods (`onDepartmentSelected`, `onUserSelected`, `importDepartments`, `importUsers`) were written *outside* the class body** (after the closing `}`), so the component was an empty shell. It also never injected `ImportService`, never declared the `departmentFile`/`userFile` fields, and didn't import `MatButtonModule`. Rewritten as a proper standalone component.
- `core/services/import.service.ts` — imported `environment` from `../../../environments/environment` (**plural**), but the folder is `environment` (**singular**). Path corrected.

---

## 4. The zoneless change-detection bug (important!)

**Symptom reported:** "department list not showing."

**Investigation:** The API returned data (verified via `curl` and the browser network tab — the console even logged the array), but the table rendered zero rows.

**Root cause:** This app runs on **Angular v22 in zoneless mode** — there is no `zone.js` dependency and no change-detection provider. In zoneless Angular, a view only re-renders when something *notifies* the framework (a signal change, the `async` pipe, a template event, etc.). The classic pattern used throughout the codebase —

```ts
this.service.getAll().subscribe(data => this.items = data);
```

— assigns a plain property, which **never tells Angular to re-render**. So the data arrived but the view stayed empty.

**The fix:** Convert data-bound properties to **signals**, whose `.set()` notifies Angular.

```ts
items = signal<Item[]>([]);           // was: items: Item[] = []
// ...
next: data => this.items.set(data),   // was: this.items = data
```

```html
<table [dataSource]="items()">        <!-- was: [dataSource]="items" -->
```

Applied to `department-list`, `user-list`, and the `user-form` department dropdown.

### 🔑 The rule going forward
In this zoneless app, **anything the template displays from an async source must be a `signal`** (or use the `async` pipe). Reactive-form *inputs* are fine as plain `patchValue` — `ControlValueAccessor` writes to the DOM directly and doesn't need change detection.

---

## 5. Excel reader rewrite (real file structure)

**Goal:** Update the backend Excel import to match the actual spreadsheets (`cmn_department.xlsx`, `sys_user.xlsx` — ServiceNow-style exports with many columns).

**Discovery:** The originals had headers at fixed column *numbers* (1, 2, 3). The real files scatter the needed fields across many columns:

**`cmn_department.xlsx`** (11 cols): `ID`→Code, `Name`→Name, `Parent`→parent (matched by name).

**`sys_user.xlsx`** (47 cols): `User ID`→EmployeeNumber, `First name`, `Last name`, `Title`, `Department`→dept lookup by name.

> ⚠️ **Key decision:** The `Employee number` column is **completely empty** in the source file, so `EmployeeNumber` is mapped to the **`User ID`** column (values like `0002`, `0018`). The header names are defined as constants at the top of `ImportService.cs`, so changing the source column is a one-line edit.

**The change:** `Services/ImportService.cs` was rewritten to **map columns by header name** instead of fixed positions:
- `BuildHeaderMap(worksheet)` reads row 1 and maps each header → column number (case-insensitive).
- `GetCell(worksheet, columns, row, header)` reads a cell by header name.

This is robust to column reordering and self-documenting.

**Verified:** 20 departments imported (with parent hierarchy resolved) and 95 users imported (linked to departments by name), all with Japanese text intact.

---

## 6. Dashboard

**Goal:** Build a dashboard.

`features/dashboard/dashboard.{ts,html,scss}` — computes stats live from the existing `/departments` and `/users` endpoints (no new backend). Uses `forkJoin` to load both, and **signals + `computed`** for all derived values (required for zoneless).

**Contents:**
- **Stat tiles:** Total Users, Departments, Top-level Departments, Avg Users/Dept. (The Users and Departments tiles route to their list pages.)
- **Bar charts (CSS-only, no chart library):** Users per Department (ranked) and Top Job Titles (top 8).
- Loading, error, and empty states.

---

## 7. Organization chart

**Goal:** Build the org chart.

Backend `GET /api/organizationchart` already returned a nested department tree (each node: department info + users + children). The frontend service was broken (`@Service` stub) and was fixed.

| File | What it does |
|------|--------------|
| `core/services/organization-chart.service.ts` | Fixed broken stub → proper `OrganizationChartService.getChart()`. |
| `core/models/organization-chart.ts` | Added the optional `manager` field to match the API. |
| `features/organization-chart/organization-chart.{ts,html,scss}` | Recursive, collapsible department tree. |

**Implementation note:** Uses a **single component with a recursive template** (`*ngTemplateOutlet` pointing at itself) rather than a separate child component. Collapse/expand state is tracked in `signal<Set<number>>` keyed by department id.

**Features:**
- Each department card shows name, code, and headcount badges — **"N here"** (direct members) and **"N total"** (including sub-departments, computed recursively).
- Collapse/expand any branch; Expand-all / Collapse-all buttons.
- Show/hide members per department (`LastName FirstName · Title · EmployeeNumber`).

> Note: the API's `manager` field is always `null` — the backend never assigns it. A follow-up could populate it (e.g. highest-ranking title per department).

---

## 8. Authentication & authorization

**Goal:** Add user login (authentication) and role-based access (authorization).

**Chosen design (recommended defaults):** JWT bearer tokens · a **separate `AuthUser` accounts table** (kept apart from imported HR data) · **Admin vs Viewer** roles · a seeded admin account, no public signup.

### Backend

| File | Purpose |
|------|---------|
| `Domain/Models/AuthUser.cs` | Account entity (`Username`, `PasswordHash`, `Role`, `CreatedAt`) + a `Roles` constants class (`Admin`, `Viewer`). |
| `Auth/PasswordHasher.cs` | **PBKDF2 (SHA-256)** hashing using only framework primitives — no heavy Identity dependency. Hash format: `iterations.salt.subkey`. |
| `DTOs/Auth/*` | `LoginDto`, `RegisterDto`, `AuthResponseDto`. |
| `Services/AuthService.cs` | `LoginAsync`, `RegisterAsync`, `SeedAdminAsync`, and JWT generation (with a role claim). |
| `Controllers/AuthController.cs` | `POST /api/auth/login` (public), `POST /api/auth/register` (Admin only), `GET /api/auth/me`. |
| `Program.cs` | Registered `AuthService`; added JWT bearer authentication + `UseAuthentication`; on startup runs `MigrateAsync()` then `SeedAdminAsync()`. |
| `Data/AppDbContext.cs` | Added `DbSet<AuthUser>` with a unique index on `Username`. |
| EF migration `AddAuthUsers` | Creates the `AuthUsers` table. Auto-applied on startup. |
| `appsettings.json` | Added `Jwt` (Key/Issuer/Audience/ExpiresMinutes) and `SeedAdmin` (Username/Password) sections. |

**Authorization on existing controllers:**
- `Departments`, `Users`, `OrganizationChart` → `[Authorize]` (any logged-in user can read).
- Create / Update / Delete on Departments & Users → `[Authorize(Roles = "Admin")]`.
- `Import` controller → `[Authorize(Roles = "Admin")]`.

### Frontend

| File | Purpose |
|------|---------|
| `core/models/auth.ts` | `LoginRequest`, `AuthResponse`, `RegisterRequest`. |
| `core/services/auth.service.ts` | Signal-based session: `login` / `logout`, token in `localStorage`, and computed `isAuthenticated` / `isAdmin` / `username` / `role`. Auto-drops an expired token. |
| `core/interceptors/auth.interceptor.ts` | Attaches `Authorization: Bearer <token>` to requests; on any `401`, logs out and redirects to `/login`. |
| `core/guards/auth.guard.ts` | `authGuard` (must be logged in) and `adminGuard` (must be Admin). |
| `features/auth/login/login.{ts,html,scss}` | Login page. |
| `app.config.ts` | Registered the interceptor via `provideHttpClient(withInterceptors([authInterceptor]))`. |
| `app.routes.ts` | Added public `/login`; guarded all pages with `authGuard`; guarded import/create/edit with `adminGuard`. |
| `app.ts` + `app.html` | Toolbar shows `username (Role)` + Logout; nav hidden until logged in; **Import link hidden for non-admins**. |
| `department-list` / `user-list` | Inject `AuthService`; **Add/Edit/Delete buttons hidden for viewers** (Actions cell shows "—"). |

### Verified end-to-end

| Scenario | Result |
|----------|--------|
| No token → any page | redirected to `/login` |
| Login `admin` | dashboard, full nav incl. Import, all buttons |
| Login viewer | no Import link, Actions show "—", no Add/Edit |
| Viewer → `/import` or `/users/create` directly | redirected to `/dashboard` |
| Backend: no token | `401` |
| Backend: viewer POST / register | `403` |
| Backend: bad password | `401` |

---

## 9. Default credentials & how to run

**Seeded admin account:** `admin` / `Admin@123`
**Test viewer account** (created during verification, in DB): `viewer1` / `View@123`

**Backend** (from `backend/OrganizationChart.Api`):
```bash
dotnet run --urls http://localhost:5044
```
On startup it applies EF migrations and seeds the admin account.

**Frontend** (from `frontend/organization-chart-ui`):
```bash
npm start
```
Runs on `http://localhost:4200` (CORS on the backend allows this origin).

---

## 10. Known follow-ups / cleanup

- **⚠️ Security before production:** `Jwt:Key` and `SeedAdmin:Password` in `appsettings.json` are dev placeholders in plaintext. Move them to user-secrets / environment variables and rotate the key.
- **`*.spec.ts` files** across the project have broken imports from the original scaffolding (e.g. `department.service.spec.ts` imports from a non-existent path). They don't affect `ng build` and were left as-is.
- **`manager` field** on org-chart nodes is always `null` (backend never sets it).
- ~~**Delete on the User list** is not yet wired~~ — **done.** User-list Delete now calls `deleteUser(id)` (confirm → `userService.delete` → reload). Department delete of a non-empty department now returns a clean **409** from the API and shows a friendly message instead of a silent 500.
- Possible next feature: an **admin-only "Accounts" page** to create/list/deactivate login accounts from the UI.
- The `viewer1` test account can be deleted if not needed.

---

*Nothing in this session was committed to git (per request). All changes are in the working tree.*
