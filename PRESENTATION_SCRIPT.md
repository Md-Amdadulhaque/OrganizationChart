# Presentation Video Script — Organization Chart System

**Target length:** 5–7 minutes. Record your screen (1080p) with voice-over.

**Before you start recording — setup checklist**
- [ ] SQL Server running.
- [ ] Backend running: from `backend/OrganizationChart.Api` → `dotnet run --urls http://localhost:5044`
- [ ] Frontend running: from `frontend/organization-chart-ui` → `npm start` (open `http://localhost:4200`)
- [ ] Have the two Excel files handy: `cmn_department.xlsx`, `sys_user.xlsx`.
- [ ] **Start from an empty database** (so the import demo shows real effect). If your DB already
      has data and you want a clean run, drop/recreate `OrganizationChartDb` and restart the
      backend (it re-migrates and re-seeds the admin).
- [ ] Log out so you begin at the login screen.
- [ ] Close other tabs / notifications; zoom the browser to ~110% so text is readable on video.

---

## Scene 1 — Intro (0:00–0:30)

> "Hi, this is the **Organization Chart System** — a full-stack app for managing a company's
> departments and employees and visualizing the reporting structure.
> It's built with an **Angular** front end and an **ASP.NET Core** Web API backed by
> **SQL Server**, and it supports **role-based login**. Let me walk you through it."

*(Show the login screen.)*

---

## Scene 2 — Authentication & roles (0:30–1:30)

1. On the login page: *"Every page requires login. I'll sign in as an administrator."*
2. Enter **admin / Admin@123**, click **Sign in**.
3. *"Now I'm on the dashboard. Notice the top bar shows my username and role — Admin — and a
   Logout button."*
4. Point at the left nav: *"As an admin I can see every section, including **Import**."*

> Optional contrast (recommended): later you'll log in as a viewer to show the difference.

---

## Scene 3 — Excel import (1:30–2:45)

1. Click **Import** in the nav.
2. *"The source data comes from Excel exports. I'll import departments first."*
3. Under **Departments**, choose `cmn_department.xlsx` → click **Import Departments** →
   *"Departments imported."*
4. Under **Users**, choose `sys_user.xlsx` → click **Import Users** → *"Users imported."*
5. *"The importer matches spreadsheet columns **by header name**, so it handles these wide
   ServiceNow exports and resolves each department's parent and each employee's department
   automatically."*

---

## Scene 4 — Dashboard (2:45–3:30)

1. Click **Dashboard**.
2. *"The dashboard summarizes the data we just imported."*
3. Point to the tiles: *"Total users, total departments, top-level departments, and average
   users per department."*
4. Point to the charts: *"And two breakdowns — users per department, and the most common job
   titles — computed live from the API."*

---

## Scene 5 — Departments & Users CRUD (3:30–4:45)

1. Click **Departments**. *"Here's the full department list."*
2. Click **Add Department**, fill code + name (optionally a parent), **Save** →
   *"The new department appears in the list."*
3. Click **Edit** on that row, change the name, **Save** → *"Edits persist."*
4. Click **Delete** on the department you just created (an empty one) → confirm →
   *"An empty department is removed."*
5. *"If I try to delete a department that still has people or sub-departments, the app blocks it
   with a clear message — so we never orphan records."* *(Optionally demonstrate on a populated
   department to show the message.)*
6. Click **Users**, show the list; briefly open **Add User** to show the form and the department
   dropdown. *(Add/edit/delete work the same way.)*

---

## Scene 6 — Organization chart (4:45–5:45)

1. Click **Organization Chart**.
2. *"This is the reporting structure as a collapsible tree."*
3. Point to a node: *"Each department shows its direct headcount and its **total** headcount,
   including all sub-departments."*
4. Click a chevron to **collapse/expand** a branch; use **Collapse all / Expand all**.
5. Click **Show N members** on a department to reveal the people in it.

---

## Scene 7 — Viewer role & wrap-up (5:45–6:45)

1. Click **Logout**.
2. Log in as **viewer1 / View@123**.
3. *"As a **Viewer**, the experience is read-only. Notice the **Import** link is gone, and the
   Add / Edit / Delete buttons are hidden."* *(Show the Departments or Users list — actions show
   a dash.)*
4. *"This isn't just the UI hiding buttons — the API itself rejects write attempts from a viewer,
   so the rules are enforced end-to-end."*
5. Closing: *"That's the Organization Chart System — import, manage, and visualize an
   organization, with secure role-based access. Thanks for watching."*

---

## Talking points to weave in (if time allows)
- **Zoneless Angular with signals** for reactive, efficient rendering.
- **JWT authentication** with **PBKDF2**-hashed passwords.
- **EF Core code-first** migrations; the database schema is created and the admin seeded
  automatically on first run.
- **Defense in depth**: authorization enforced on both client and server.

## Recording tips
- Rehearse once with the checklist done, then record in a single take if you can.
- Speak to *what the reviewer sees on screen* — click, then narrate.
- If a step errors on camera, pause, fix, and re-record that scene; keep segments short.
