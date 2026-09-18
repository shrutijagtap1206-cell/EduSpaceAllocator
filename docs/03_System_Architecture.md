# 03. System Architecture

## 1. High-Level Architectural Diagram

```
                    EduSpace Allocator
                            |
          +-----------------+----------------+
          |                 |                |
       React            ASP.NET Core      Python
      Frontend             API           AI Engine
   (Vite + Leaflet)      (.NET 10)     (FastAPI + ML)
          |                 |                |
          |            PostgreSQL        scikit-learn
          |              PostGIS         NetworkX
          |                 |                |
          +-----------------+----------------+
                            |
                       GIS Dashboard
```

---

## 2. Component Responsibilities

### 2.1 Frontend Presentation Tier (React 19 + Vite)
- **Interactive Geospatial Visualizer**: Built with `react-leaflet`, rendering interactive vector overlays for Spaces (markers), Demand Centroids (circles), and Smart Allocation Links (polylines).
- **Executive KPI Dashboard**: Real-time aggregated statistics directly connected to backend metrics endpoints.
- **Analytics & Trends**: Recharts-powered graphs for capacity utilization, distance distribution, and optimization rankings.
- **Role-Based Views**: Dynamic persona switcher and JWT-authenticated session management.

### 2.2 Application Logic & API Tier (ASP.NET Core 10 Web API)
- **Identity & Security**: ASP.NET Core Identity with EF Core store, JWT bearer token issuance, and role-based authorization (Admin, NGO, EducationCoordinator, Viewer).
- **Service Layer**: Decoupled service architecture (`SpaceService`, `AllocationService`, `ScheduleService`, `AuditService`, `DashboardService`, `SDGService`).
- **Data Validation Pipeline**: DataAnnotation-enforced DTO boundary checking returning consistent RFC 9110 status responses.
- **Microservice Orchestration**: Communicates with the Python AI Engine via high-throughput HTTP client.

### 2.3 AI & Algorithmic Engine (Python 3.11 + FastAPI)
- **Spatial Clustering**: Unsupervised K-Means clustering with Scikit-Learn to detect geographic demand hot-spots.
- **Bipartite Graph Matching**: NetworkX implementation of Maximum Weight Bipartite Matching to maximize global utility.

### 2.4 Data Persistence Tier (PostgreSQL 16 + PostGIS)
- **Relational Tables**: Communities, Spaces, LearningRequests, Allocations, Schedules, Partners, Courses, AuditLogs.
- **Spatial Indexing**: GiST indexes on spatial geometries for sub-millisecond distance calculation using `ST_DistanceSphere` and `ST_DWithin`.
- **Identity Schema**: Standardized ASP.NET Core Identity authentication tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`).
