# 09. REST API Documentation

Base URL: `http://localhost:5244/api`

## 1. Authentication Endpoints (`/api/auth`)

### 1.1 User Login
- **Method**: `POST`
- **Path**: `/auth/login`
- **Request Body**:
  ```json
  {
    "email": "admin@eduspace.local",
    "password": "Admin@12345"
  }
  ```
- **Response (200 OK)**:
  ```json
  {
    "success": true,
    "message": "Login successful.",
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "email": "admin@eduspace.local",
    "roles": ["Admin"]
  }
  ```

### 1.2 Current User Profile
- **Method**: `GET`
- **Path**: `/auth/me`
- **Headers**: `Authorization: Bearer <token>`
- **Response (200 OK)**:
  ```json
  {
    "success": true,
    "email": "admin@eduspace.local",
    "roles": ["Admin"]
  }
  ```

---

## 2. Core Resource Endpoints

| Resource | Method | Path | Description |
| :--- | :--- | :--- | :--- |
| **Spaces** | `GET` | `/Spaces` | List all spaces with availability status |
| | `GET` | `/Spaces/{id}` | Retrieve specific space details |
| | `POST` | `/Spaces` | Register space (enforces DataAnnotations) |
| | `PUT` | `/Spaces/{id}` | Update existing space attributes |
| | `DELETE`| `/Spaces/{id}` | Remove space record |
| **Learning Requests** | `GET` | `/LearningRequests` | List all community learning requests |
| | `POST` | `/LearningRequests` | Submit request (enforces DataAnnotations) |
| **Communities** | `GET` | `/Communities` | List urban wards & demand clusters |
| **Allocations** | `GET` | `/Allocations` | Retrieve all computed allocation matches |
| | `POST` | `/Allocations` | Confirm and commit an allocation |
| **Matching** | `POST` | `/Matching/run` | Execute bipartite matching pipeline |
| **Dashboard** | `GET` | `/Dashboard` | Real-time aggregate KPI metrics |
| **Scheduling**| `GET` | `/Scheduling` | List timetabled course sessions |
| **Audit Logs** | `GET` | `/Audit` | System audit trails & history |
| **SDG Metrics**| `GET` | `/SDG` | Social impact & SDG scoring indicators |
