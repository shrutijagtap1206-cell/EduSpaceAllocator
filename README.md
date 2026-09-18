# EduSpace Allocator

**Smart City + Education Infrastructure Platform** for matching underutilized urban physical spaces with community educational demand.

---

## 1. System Overview

EduSpace Allocator addresses the paradox of urban educational infrastructure: while grassroots non-profits and community educators struggle to find affordable, accessible classrooms, commercial buildings, corporate training centers, and municipal halls stand vacant outside peak hours.

Using **PostGIS spatial indexing**, **unsupervised demand clustering (K-Means)**, and **bipartite maximum-weight matching**, EduSpace Allocator transforms vacant square footage into safe, high-impact community learning centers.

```
                         EDUSPACE ALLOCATOR
                                │
             ┌──────────────────┼──────────────────┐
             │                  │                  │
          React             ASP.NET Core        Python
         Frontend               API             AI Engine
     (Vite + Leaflet)         (.NET 10)       (FastAPI + ML)
             │                  │                  │
             │             Authentication       K-Means
             │             Authorization       Matching
             │                  │             Optimization
             │                  │                  │
             └──────────────────┼──────────────────┘
                                │
                         PostgreSQL/PostGIS
                                │
              ┌─────────────────┼─────────────────┐
              │                 │                 │
             GIS             Dashboard        Analytics
              │                 │                 │
              └─────────────────┼─────────────────┘
                                │
                    Social Impact + UN SDGs
```

---

## 2. Technology Stack

- **Frontend**: React 19, Vite, Leaflet, React-Leaflet, Recharts, Axios.
- **Backend API**: ASP.NET Core 10 Web API, C#, Entity Framework Core, ASP.NET Core Identity, JWT Bearer Authentication.
- **Database**: PostgreSQL 16 + PostGIS extension for spatial queries and GiST indexing.
- **AI / Algorithmic Engine**: Python 3.11, FastAPI, scikit-learn (K-Means clustering), NetworkX (Bipartite graph matching).
- **Automated Testing**: xUnit, FluentAssertions, Moq, EF Core InMemory.

---

## 3. Verified Benchmark Dataset & Performance

The platform is seeded and verified with actual geospatial coordinates and educational demand figures across Pune, Maharashtra:

| Metric | Verified Value |
| :--- | :--- |
| **Cataloged Learning Spaces** | **50 spaces** |
| **Registered Urban Communities** | **30 communities** |
| **Active Learning Requests** | **30 requests** |
| **Ecosystem Partners** | **15 NGOs / Institutes** |
| **Curriculum Courses** | **20 courses** |
| **Feasible Match Candidates** | **240 candidate pairs** |
| **Final Matched Allocations** | **21 optimal allocations** |
| **Students Directly Served** | **1,035 students** |
| **Average Commute Distance** | **1.44 km** |
| **Average Space Utilization** | **76.88%** |
| **Cost Efficiency** | **61.86%** |
| **Composite Optimization Score** | **65.28%** |

---

## 4. Key Platform Features

- **Space Registry & Geocoding**: Complete inventory management with geographic coordinates, floor area, capacity, rental rates, and real-time availability.
- **Learning Request Pipeline**: Input validation boundary checking ensuring realistic budgets, positive capacity requirements, and valid coordinates.
- **Interactive GIS Mapping**: Multi-layer Leaflet visualization rendering available learning spaces, student demand centroids, and smart allocation links with real-time detail popups.
- **Smart Allocation Engine**: Bipartite maximum-weight matching balancing travel distance, capacity utilization, and budget constraints.
- **Allocation Results & Confirmation**: Detailed match evaluation with breakdown of optimization components.
- **Automated Timetable Scheduling**: Course assignment to allocated slots with built-in temporal conflict detection.
- **Executive Analytics & Recharts**: Visual distribution of optimization scores and travel distance vs. student reach.
- **Social Impact & SDG 4/10/11 Dashboard**: Quantitative tracking aligned with UN Sustainable Development Goals.
- **ASP.NET Core Identity & RBAC**: Role-based access control with 4 distinct personas (`Admin`, `NGO`, `EducationCoordinator`, `Viewer`) secured by JWT tokens.
- **Automated Unit Testing**: 41 xUnit automated test cases validating DTO rules, matching properties, scheduling conflicts, and security matrix.

---

## 5. Getting Started

### 5.1 Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js 20+](https://nodejs.org/)
- [PostgreSQL 16](https://www.postgresql.org/) with PostGIS extension enabled
- [Python 3.11+](https://www.python.org/)

### 5.2 Backend API Setup
```powershell
cd D:\shruti\EduSpaceAllocator\backend\EduSpaceAllocator.API
dotnet build
dotnet run
```
The API starts on `http://localhost:5244`.

### 5.3 Running Automated Tests
```powershell
cd D:\shruti\EduSpaceAllocator
dotnet test backend\EduSpaceAllocator.Tests
```

### 5.4 Frontend Client Setup
```powershell
cd D:\shruti\EduSpaceAllocator\frontend\eduspace-client
npm install
npm run dev
```
Open `http://localhost:5173` in your browser.

---

## 6. Pre-Configured Test Accounts (RBAC)

| Role | Email | Password | Access Rights |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@eduspace.local` | `Admin@12345` | Full System Access (CRUD, Match, Allocate, Admin) |
| **NGO** | `ngo@eduspace.local` | `NGO@12345` | View, Submit Requests, View Allocations |
| **Education Coordinator** | `coordinator@eduspace.local` | `Coordinator@12345` | View, Schedule Courses, Manage Timetables |
| **Viewer** | `viewer@eduspace.local` | `Viewer@12345` | Read-only access to GIS and Public Dashboards |

*(Switch personas instantaneously using the topbar profile switcher in the web application.)*

---

## 7. Technical Documentation Suite (`docs/`)

Comprehensive viva and evaluation documents are located in `docs/`:
- [`01_Project_Overview.md`](docs/01_Project_Overview.md): Executive summary and benchmark metrics.
- [`02_Problem_Statement.md`](docs/02_Problem_Statement.md): Urban educational infrastructure disparities.
- [`03_System_Architecture.md`](docs/03_System_Architecture.md): Detailed 3-tier architecture and microservice boundaries.
- [`04_Database_Design.md`](docs/04_Database_Design.md): Entity relationship model and PostGIS schema.
- [`05_GIS_Module.md`](docs/05_GIS_Module.md): PostGIS queries, spatial layers, and Haversine distance computations.
- [`06_Machine_Learning.md`](docs/06_Machine_Learning.md): Unsupervised K-Means demand clustering.
- [`07_Matching_Algorithm.md`](docs/07_Matching_Algorithm.md): Bipartite graph formulation and Kuhn-Munkres algorithm.
- [`08_Optimization.md`](docs/08_Optimization.md): Multi-objective scoring formulation and weight calibration.
- [`09_API_Documentation.md`](docs/09_API_Documentation.md): Complete OpenAPI REST endpoints and JWT contracts.
- [`10_Testing.md`](docs/10_Testing.md): Unit testing strategy and 41 test cases breakdown.
- [`11_SDG_Mapping.md`](docs/11_SDG_Mapping.md): UN Sustainable Development Goals (SDG 4, 10, 11).
- [`12_Future_Scope.md`](docs/12_Future_Scope.md): IoT occupancy sensing, dynamic pricing, and multi-city roadmap.
