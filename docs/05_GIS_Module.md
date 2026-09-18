# 05. GIS & Geospatial Module

## 1. Overview
The GIS module enables spatial visualization, geographic indexing, and proximity computations. Using **PostGIS** on the server and **Leaflet / React-Leaflet** on the client, the platform provides interactive spatial awareness of educational demand versus physical capacity across Pune.

---

## 2. Spatial Querying & Geodesic Distance
Geographic distances are computed using the Haversine formula on spherical coordinates:
$$d = 2R \arcsin \left( \sqrt{\sin^2\left(\frac{\Delta \phi}{2}\right) + \cos(\phi_1)\cos(\phi_2)\sin^2\left(\frac{\Delta \lambda}{2}\right)} \right)$$
where $R = 6371 \text{ km}$ (Earth radius), $\phi$ is latitude in radians, and $\lambda$ is longitude in radians.

In PostgreSQL, spatial indexing with GiST allows bounding box queries and rapid neighborhood radius scans:
```sql
SELECT s.*
FROM "Spaces" s
WHERE ST_DWithin(
    ST_MakePoint(s."Longitude", s."Latitude")::geography,
    ST_MakePoint(73.8567, 18.5204)::geography,
    5000 -- 5 km search radius
);
```

---

## 3. Client-Side GIS Visualization Architecture
The map utilizes `react-leaflet` with custom layer controls:

```
┌───────────────────────────────────────────────┐
│               GIS MAPPING                     │
│                                               │
│   ● [Space Marker] (Blue pin)                 │
│         │                                     │
│         │  (Polyline connection)              │
│         ▼                                     │
│   ● [Demand Centroid] (Red circle)            │
│                                               │
│   Layers Control (Top-Right):                 │
│   [x] Learning Spaces                         │
│   [x] Learning Demand                         │
│   [x] Smart Allocations                       │
└───────────────────────────────────────────────┘
```

### Interactive Popup Attributes
- **Space Pin**: Building Name, Address, Capacity, Floor Area, Rental Cost, Availability.
- **Demand Centroid**: Organization Name, Program Type, Student Capacity, Budget, Target Community.
- **Allocation Polyline**: Match ID, Distance in km, Capacity Utilization %, Optimization Score %.
