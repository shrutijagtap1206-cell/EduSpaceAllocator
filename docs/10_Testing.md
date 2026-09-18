# 10. Automated Testing & Verification Suite

## 1. Test Architecture
The test suite is built in `backend/EduSpaceAllocator.Tests` using:
- **xUnit**: Test runner and theory test cases.
- **FluentAssertions**: Readable, expressive assertion statements.
- **Moq**: Interface and dependency mocking.
- **Microsoft.EntityFrameworkCore.InMemory**: Isolated in-memory database simulation.

---

## 2. Test Suites Summary

| Test Suite | Purpose | Tests |
| :--- | :--- | :--- |
| **`ValidationTests.cs`** | DTO DataAnnotations validation (SpaceDto, LearningRequestDto) checking required fields, string lengths, coordinate bounds, positive capacities, budgets. | 21 Theory cases + 2 Facts |
| **`OptimizationTests.cs`** | Utilization clamping ($\le 1.0$), cost efficiency bounding, distance linear decay, and weighted composite scoring. | 6 Facts & Theories |
| **`MatchingTests.cs`** | Feasibility constraint checks (distance $\le 5$ km, capacity $\ge$ demand, budget $\ge$ rent, availability), and one-to-one bipartite matching property. | 6 Facts |
| **`ScheduleConflictTests.cs`** | Time slot overlap detection on identical days, consecutive non-overlapping slots, and different day isolation. | 3 Facts |
| **`AuthTests.cs`** | JWT token generation, claims encoding (email, name, role), signature validation, and RBAC matrix enforcement. | 5 Facts & Theories |

---

## 3. Running Test Suites
```bash
cd D:\shruti\EduSpaceAllocator
dotnet test backend\EduSpaceAllocator.Tests
```

### Verified Test Run Result
```text
Test run for D:\shruti\EduSpaceAllocator\backend\EduSpaceAllocator.Tests\bin\Debug\net10.0\EduSpaceAllocator.Tests.dll (.NETCoreApp,Version=v10.0)
Passed!  - Failed: 0, Passed: 41, Skipped: 0, Total: 41, Duration: 1 s
```
All 41 unit tests pass with zero failures.
