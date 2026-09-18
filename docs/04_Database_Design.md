# 04. Database Design & Entity Relationships

## 1. Entity-Relationship Diagram (Mermaid)

```mermaid
erDiagram
    Community ||--o{ LearningRequest : submits
    Community {
        int CommunityId PK
        string Name
        string City
        double Latitude
        double Longitude
        int TargetStudents
    }

    LearningRequest ||--o{ Allocation : generates
    LearningRequest {
        int RequestId PK
        string Organization
        string ProgramType
        int StudentCapacity
        decimal Budget
        string PreferredLocation
        double PreferredLatitude
        double PreferredLongitude
        int CommunityId FK
    }

    Space ||--o{ Allocation : fulfills
    Space {
        int SpaceId PK
        string BuildingName
        string Address
        string City
        double Latitude
        double Longitude
        double FloorArea
        int Capacity
        decimal RentalCost
        boolean Availability
    }

    Allocation ||--o{ Schedule : timetabled
    Allocation ||--o{ SocialImpact : evaluates
    Allocation {
        int AllocationId PK
        string MatchId
        int SpaceId FK
        int RequestId FK
        double DistanceKm
        double CapacityUtilization
        double CostEfficiency
        double OptimizationScore
        int StudentsServed
        string Status
        timestamp AllocatedAt
    }

    Course ||--o{ Schedule : runs_in
    Course {
        int CourseId PK
        string CourseName
        string Category
        int DurationWeeks
    }

    Schedule {
        int ScheduleId PK
        int AllocationId FK
        int CourseId FK
        string DayOfWeek
        time StartTime
        time EndTime
    }

    Partner {
        int PartnerId PK
        string Name
        string Type
        string ContactEmail
    }

    AuditLog {
        int LogId PK
        string Action
        string EntityName
        int EntityId
        string PerformedBy
        timestamp Timestamp
    }
```

---

## 2. Table Schema Overview

### 2.1 Spaces
- `SpaceId` (Serial, PK)
- `BuildingName` (VARCHAR(150), NOT NULL)
- `Address` (VARCHAR(250), NOT NULL)
- `City` (VARCHAR(100), NOT NULL)
- `Latitude` (DOUBLE PRECISION, NOT NULL)
- `Longitude` (DOUBLE PRECISION, NOT NULL)
- `FloorArea` (DOUBLE PRECISION, NOT NULL)
- `Capacity` (INT, NOT NULL)
- `RentalCost` (NUMERIC(12,2), NOT NULL)
- `Availability` (BOOLEAN, DEFAULT TRUE)

### 2.2 LearningRequests
- `RequestId` (Serial, PK)
- `Organization` (VARCHAR(150), NOT NULL)
- `ProgramType` (VARCHAR(100), NOT NULL)
- `StudentCapacity` (INT, NOT NULL)
- `Budget` (NUMERIC(12,2), NOT NULL)
- `PreferredLocation` (VARCHAR(250), NOT NULL)
- `PreferredLatitude` (DOUBLE PRECISION, NOT NULL)
- `PreferredLongitude` (DOUBLE PRECISION, NOT NULL)
- `CommunityId` (INT, FK -> Communities)

### 2.3 Allocations
- `AllocationId` (Serial, PK)
- `MatchId` (VARCHAR(50), UNIQUE)
- `SpaceId` (INT, FK -> Spaces)
- `RequestId` (INT, FK -> LearningRequests)
- `DistanceKm` (DOUBLE PRECISION)
- `CapacityUtilization` (DOUBLE PRECISION)
- `CostEfficiency` (DOUBLE PRECISION)
- `OptimizationScore` (DOUBLE PRECISION)
- `StudentsServed` (INT)
- `Status` (VARCHAR(50))
- `AllocatedAt` (TIMESTAMP WITH TIME ZONE)
