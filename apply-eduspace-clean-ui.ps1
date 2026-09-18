$ErrorActionPreference = "Stop"

$root = "D:\shruti\EduSpaceAllocator"
$frontend = Join-Path $root "frontend\eduspace-client"
$backend = Join-Path $root "backend\EduSpaceAllocator.API"

if (!(Test-Path $frontend)) { throw "Frontend path not found: $frontend" }
if (!(Test-Path $backend)) { throw "Backend path not found: $backend" }

function Write-Utf8([string]$Path, [string]$Content) {
    $dir = Split-Path $Path -Parent
    if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    Set-Content -Path $Path -Value $Content -Encoding utf8
}

# ============================================================
# 1. CLEAN USER-ORIENTED FRONTEND NAVIGATION
#    Algorithms remain internal. No K-Means/Bipartite sidebar.
# ============================================================

Write-Utf8 "$frontend\src\App.jsx" @'
import { useEffect, useState } from 'react';
import { getSpaces } from './services/spaceService';
import Dashboard from './components/Dashboard/Dashboard';
import GISMap from './components/GIS/GISMap';
import Spaces from './pages/Spaces';
import LearningRequests from './pages/LearningRequests';
import Communities from './pages/Communities';
import Allocations from './pages/Allocations';
import StudentDemand from './pages/StudentDemand';
import Allocation from './pages/Allocation';
import Analytics from './pages/Analytics';

function App() {
  const [activePage, setActivePage] = useState('dashboard');
  const [spaces, setSpaces] = useState([]);

  useEffect(() => {
    getSpaces().then(setSpaces).catch(console.error);
  }, []);

  const menu = [
    ['dashboard', 'Dashboard'],
    ['spaces', 'Space Registry'],
    ['requests', 'Learning Requests'],
    ['communities', 'Communities'],
    ['demand', 'Student Demand'],
    ['gis', 'GIS Mapping'],
    ['allocation', 'Smart Allocation'],
    ['allocations', 'Allocation Results'],
    ['analytics', 'Analytics'],
  ];

  const renderPage = () => {
    switch (activePage) {
      case 'spaces':
        return <Spaces />;
      case 'requests':
        return <LearningRequests />;
      case 'communities':
        return <Communities />;
      case 'demand':
        return <StudentDemand />;
      case 'gis':
        return (
          <>
            <h2>GIS Mapping</h2>
            <p className="page-subtitle">
              Explore available learning spaces on the map.
            </p>
            <GISMap spaces={spaces} />
          </>
        );
      case 'allocation':
        return <Allocation />;
      case 'allocations':
        return <Allocations />;
      case 'analytics':
        return <Analytics />;
      default:
        return <Dashboard />;
    }
  };

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-title">EduSpace</div>
          <div className="brand-subtitle">Allocator</div>
        </div>

        <nav className="sidebar-nav">
          {menu.map(([id, label]) => (
            <button
              key={id}
              className={`nav-button ${activePage === id ? 'active' : ''}`}
              onClick={() => setActivePage(id)}
            >
              {label}
            </button>
          ))}
        </nav>
      </aside>

      <main className="main-content">
        <header className="topbar">
          <div>
            <h1>EduSpace Allocator</h1>
            <span>Smart Education Infrastructure Platform</span>
          </div>

          <div className="system-status">
            <span className="status-dot" />
            System Active
          </div>
        </header>

        <section className="page-content">
          {renderPage()}
        </section>
      </main>
    </div>
  );
}

export default App;
'@

# ============================================================
# 2. REAL DATA DASHBOARD
# ============================================================

Write-Utf8 "$frontend\src\components\Dashboard\Dashboard.jsx" @'
import { useEffect, useState } from 'react';
import api from '../../services/api';

function Dashboard() {
  const [data, setData] = useState({
    spaces: [],
    requests: [],
    communities: [],
    allocations: [],
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      api.get('/Spaces'),
      api.get('/LearningRequests'),
      api.get('/Communities'),
      api.get('/Allocations'),
    ])
      .then(([spaces, requests, communities, allocations]) => {
        setData({
          spaces: spaces.data,
          requests: requests.data,
          communities: communities.data,
          allocations: allocations.data,
        });
      })
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return <div className="loading">Loading dashboard...</div>;
  }

  const studentsServed = data.allocations.reduce(
    (sum, allocation) => sum + (allocation.studentsServed || 0),
    0
  );

  const cards = [
    ['Available Spaces', data.spaces.filter((s) => s.availability).length],
    ['Learning Requests', data.requests.length],
    ['Communities', data.communities.length],
    ['Matched Allocations', data.allocations.length],
    ['Students Served', studentsServed],
  ];

  return (
    <div>
      <div className="page-heading">
        <div>
          <h2>Dashboard</h2>
          <p className="page-subtitle">
            Overview of education-space allocation activity.
          </p>
        </div>
      </div>

      <div className="kpi-grid">
        {cards.map(([label, value]) => (
          <div className="kpi-card" key={label}>
            <span>{label}</span>
            <strong>{value}</strong>
          </div>
        ))}
      </div>

      <div className="dashboard-grid">
        <div className="feature-card">
          <h3>Smart Allocation</h3>
          <p>
            Match educational requests with suitable available spaces using
            location, capacity and budget constraints.
          </p>
        </div>

        <div className="feature-card">
          <h3>GIS Coverage</h3>
          <p>
            View learning spaces geographically and understand where
            educational infrastructure is available.
          </p>
        </div>

        <div className="feature-card">
          <h3>Demand Analysis</h3>
          <p>
            Analyze community and student-demand patterns to support
            infrastructure planning.
          </p>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
'@

# ============================================================
# 3. USER-FACING SMART ALLOCATION PAGE
#    K-Means + Bipartite Matching happen behind this feature.
# ============================================================

Write-Utf8 "$frontend\src\pages\Allocation.jsx" @'
import { useState } from 'react';
import api from '../services/api';

function Allocation() {
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const runAllocation = async () => {
    setLoading(true);
    setError('');

    try {
      const response = await api.post('/Optimization/run');
      setResult(response.data);
    } catch (err) {
      console.error(err);
      setError(
        err.response?.data?.message ||
          'Allocation failed. Make sure the ASP.NET API and AI engine are running.'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="page-heading">
        <div>
          <h2>Smart Allocation</h2>
          <p className="page-subtitle">
            Generate suitable learning-space allocations from current data.
          </p>
        </div>

        <button
          className="primary-button"
          onClick={runAllocation}
          disabled={loading}
        >
          {loading ? 'Running...' : 'Run Smart Allocation'}
        </button>
      </div>

      {error && <div className="alert error">{error}</div>}

      {result && (
        <>
          <div className="kpi-grid">
            <div className="kpi-card">
              <span>Candidate Matches</span>
              <strong>{result.candidateCount}</strong>
            </div>

            <div className="kpi-card">
              <span>Allocations</span>
              <strong>{result.matchCount}</strong>
            </div>

            <div className="kpi-card">
              <span>Students Served</span>
              <strong>{result.summary?.totalStudentsServed ?? 0}</strong>
            </div>

            <div className="kpi-card">
              <span>Optimization Score</span>
              <strong>
                {((result.summary?.overallOptimizationScore ?? 0) * 100).toFixed(1)}%
              </strong>
            </div>
          </div>

          <div className="feature-card">
            <h3>Allocation Results</h3>

            {result.matches?.length ? (
              <div className="table-wrap">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Match</th>
                      <th>Space</th>
                      <th>Request</th>
                      <th>Distance</th>
                      <th>Capacity</th>
                      <th>Cost</th>
                      <th>Reach</th>
                      <th>Score</th>
                    </tr>
                  </thead>

                  <tbody>
                    {result.matches.map((match) => (
                      <tr key={match.matchId}>
                        <td>{match.matchId}</td>
                        <td>{match.spaceId}</td>
                        <td>{match.requestId}</td>
                        <td>{match.distanceKm} km</td>
                        <td>
                          {(match.capacityUtilization * 100).toFixed(1)}%
                        </td>
                        <td>
                          {(match.costEfficiency * 100).toFixed(1)}%
                        </td>
                        <td>{(match.studentReach * 100).toFixed(1)}%</td>
                        <td>
                          <strong>
                            {(match.optimizationScore * 100).toFixed(1)}%
                          </strong>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <p>No feasible allocations were generated.</p>
            )}
          </div>
        </>
      )}

      {!result && !error && (
        <div className="feature-card empty-state">
          <h3>Ready to allocate</h3>
          <p>
            Run Smart Allocation to analyze educational demand and available
            spaces and produce allocation recommendations.
          </p>
        </div>
      )}
    </div>
  );
}

export default Allocation;
'@

# ============================================================
# 4. STUDENT DEMAND USER PAGE
# ============================================================

Write-Utf8 "$frontend\src\pages\StudentDemand.jsx" @'
import { useEffect, useState } from 'react';
import api from '../services/api';

function StudentDemand() {
  const [communities, setCommunities] = useState([]);
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([api.get('/Communities'), api.get('/LearningRequests')])
      .then(([communitiesResponse, requestsResponse]) => {
        setCommunities(communitiesResponse.data);
        setRequests(requestsResponse.data);
      })
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return <div className="loading">Loading student demand...</div>;
  }

  const totalStudents = communities.reduce(
    (sum, community) => sum + community.studentCount,
    0
  );

  const requestedCapacity = requests.reduce(
    (sum, request) => sum + request.studentCapacity,
    0
  );

  return (
    <div>
      <h2>Student Demand</h2>
      <p className="page-subtitle">
        Education demand currently registered in the platform.
      </p>

      <div className="kpi-grid">
        <div className="kpi-card">
          <span>Total Students</span>
          <strong>{totalStudents}</strong>
        </div>

        <div className="kpi-card">
          <span>Requested Capacity</span>
          <strong>{requestedCapacity}</strong>
        </div>

        <div className="kpi-card">
          <span>Communities</span>
          <strong>{communities.length}</strong>
        </div>

        <div className="kpi-card">
          <span>Learning Requests</span>
          <strong>{requests.length}</strong>
        </div>
      </div>

      <div className="feature-card">
        <h3>Community Demand</h3>

        <div className="table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Community</th>
                <th>Students</th>
                <th>Literacy</th>
                <th>Income Group</th>
              </tr>
            </thead>

            <tbody>
              {communities.map((community) => (
                <tr key={community.communityId}>
                  <td>Community {community.communityId}</td>
                  <td>{community.studentCount}</td>
                  <td>{community.literacyRate}%</td>
                  <td>{community.incomeGroup}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default StudentDemand;
'@

# ============================================================
# 5. SAVED ALLOCATION RESULTS
# ============================================================

Write-Utf8 "$frontend\src\pages\Allocations.jsx" @'
import { useEffect, useState } from 'react';
import api from '../services/api';

function Allocations() {
  const [allocations, setAllocations] = useState([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);

    api
      .get('/Allocations')
      .then((response) => setAllocations(response.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  if (loading) {
    return <div className="loading">Loading allocation results...</div>;
  }

  return (
    <div>
      <div className="page-heading">
        <div>
          <h2>Allocation Results</h2>
          <p className="page-subtitle">
            Saved recommendations produced by the allocation engine.
          </p>
        </div>

        <button className="secondary-button" onClick={load}>
          Refresh
        </button>
      </div>

      <div className="feature-card">
        {allocations.length === 0 ? (
          <div className="empty-state">
            <h3>No saved allocations</h3>
            <p>Run Smart Allocation to create allocation results.</p>
          </div>
        ) : (
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Match ID</th>
                  <th>Space</th>
                  <th>Request</th>
                  <th>Students</th>
                  <th>Distance</th>
                  <th>Optimization</th>
                  <th>Status</th>
                </tr>
              </thead>

              <tbody>
                {allocations.map((allocation) => (
                  <tr key={allocation.allocationId}>
                    <td>{allocation.matchId}</td>
                    <td>{allocation.spaceId}</td>
                    <td>{allocation.requestId}</td>
                    <td>{allocation.studentsServed}</td>
                    <td>{allocation.distanceKm} km</td>
                    <td>
                      {(allocation.optimizationScore * 100).toFixed(1)}%
                    </td>
                    <td>
                      <span className="status-badge">
                        {allocation.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

export default Allocations;
'@

# ============================================================
# 6. ANALYTICS PAGE
# ============================================================

Write-Utf8 "$frontend\src\pages\Analytics.jsx" @'
import { useEffect, useState } from 'react';
import api from '../services/api';

function Analytics() {
  const [allocations, setAllocations] = useState([]);

  useEffect(() => {
    api
      .get('/Allocations')
      .then((response) => setAllocations(response.data))
      .catch(console.error);
  }, []);

  const average = (key) =>
    allocations.length
      ? allocations.reduce((sum, item) => sum + (item[key] || 0), 0) /
        allocations.length
      : 0;

  return (
    <div>
      <h2>Analytics</h2>
      <p className="page-subtitle">
        Allocation performance indicators.
      </p>

      <div className="kpi-grid">
        <div className="kpi-card">
          <span>Average Distance</span>
          <strong>{average('distanceKm').toFixed(2)} km</strong>
        </div>

        <div className="kpi-card">
          <span>Capacity Utilization</span>
          <strong>{(average('capacityUtilization') * 100).toFixed(1)}%</strong>
        </div>

        <div className="kpi-card">
          <span>Cost Efficiency</span>
          <strong>{(average('costEfficiency') * 100).toFixed(1)}%</strong>
        </div>

        <div className="kpi-card">
          <span>Optimization Score</span>
          <strong>{(average('optimizationScore') * 100).toFixed(1)}%</strong>
        </div>
      </div>
    </div>
  );
}

export default Analytics;
'@

# ============================================================
# 7. CLEAN APPLICATION STYLING
# ============================================================

Write-Utf8 "$frontend\src\index.css" @'
* {
  box-sizing: border-box;
}

body {
  margin: 0;
  font-family: Inter, Arial, sans-serif;
  background: #f4f6fa;
  color: #172033;
}

button {
  font: inherit;
}

.app-shell {
  min-height: 100vh;
  display: flex;
}

.sidebar {
  width: 250px;
  min-height: 100vh;
  background: #111827;
  color: white;
  padding: 28px 16px;
  position: fixed;
  left: 0;
  top: 0;
}

.brand {
  padding: 0 14px 28px;
}

.brand-title {
  font-size: 28px;
  font-weight: 800;
}

.brand-subtitle {
  color: #aeb8c9;
  margin-top: 2px;
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.nav-button {
  width: 100%;
  border: 0;
  background: transparent;
  color: #d8deea;
  text-align: left;
  padding: 12px 14px;
  border-radius: 9px;
  cursor: pointer;
}

.nav-button:hover {
  background: #1f2937;
}

.nav-button.active {
  background: #374151;
  color: white;
  font-weight: 700;
}

.main-content {
  margin-left: 250px;
  width: calc(100% - 250px);
}

.topbar {
  height: 96px;
  background: white;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 34px;
}

.topbar h1 {
  margin: 0;
  font-size: 25px;
}

.topbar span {
  color: #667085;
}

.system-status {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #475467;
}

.status-dot {
  width: 9px;
  height: 9px;
  border-radius: 50%;
  background: #22c55e;
}

.page-content {
  padding: 32px;
  max-width: 1400px;
}

.page-heading {
  display: flex;
  justify-content: space-between;
  gap: 20px;
  align-items: center;
}

h2 {
  margin: 0 0 7px;
  font-size: 30px;
}

.page-subtitle {
  color: #667085;
  margin-top: 0;
}

.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(170px, 1fr));
  gap: 16px;
  margin: 24px 0;
}

.kpi-card,
.feature-card {
  background: white;
  border: 1px solid #e4e7ec;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 1px 2px rgba(16, 24, 40, 0.04);
}

.kpi-card span {
  display: block;
  color: #667085;
  font-size: 14px;
}

.kpi-card strong {
  display: block;
  margin-top: 8px;
  font-size: 27px;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 18px;
}

.feature-card h3 {
  margin-top: 0;
}

.primary-button,
.secondary-button {
  border: 0;
  border-radius: 8px;
  padding: 11px 16px;
  cursor: pointer;
  font-weight: 700;
}

.primary-button {
  background: #1d4ed8;
  color: white;
}

.primary-button:disabled {
  opacity: 0.6;
  cursor: wait;
}

.secondary-button {
  background: #e5e7eb;
  color: #172033;
}

.table-wrap {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th,
.data-table td {
  text-align: left;
  padding: 13px 12px;
  border-bottom: 1px solid #eaecf0;
  white-space: nowrap;
}

.data-table th {
  color: #475467;
  font-size: 13px;
}

.status-badge {
  display: inline-block;
  background: #ecfdf3;
  padding: 5px 9px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 700;
}

.alert {
  padding: 13px 16px;
  border-radius: 8px;
  margin: 16px 0;
}

.alert.error {
  background: #fef3f2;
  color: #b42318;
}

.loading,
.empty-state {
  padding: 40px 10px;
  text-align: center;
  color: #667085;
}

@media (max-width: 900px) {
  .sidebar {
    width: 210px;
  }

  .main-content {
    margin-left: 210px;
    width: calc(100% - 210px);
  }

  .dashboard-grid {
    grid-template-columns: 1fr;
  }
}
'@

# ============================================================
# 8. BACKEND PERSISTENCE
#    POST /api/Optimization/run now saves AI matches in Allocations.
# ============================================================

Write-Utf8 "$backend\Controllers\OptimizationController.cs" @'
using System.Net.Http.Json;
using EduSpaceAllocator.API.Data;
using EduSpaceAllocator.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduSpaceAllocator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptimizationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public OptimizationController(
        AppDbContext context,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient("AIEngine");
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunOptimization()
    {
        var spaces = await _context.Spaces
            .AsNoTracking()
            .Where(s => s.Availability)
            .Select(s => new
            {
                spaceId = s.SpaceId,
                buildingName = s.BuildingName,
                address = s.Address,
                latitude = s.Latitude,
                longitude = s.Longitude,
                capacity = s.Capacity,
                rentalCost = s.RentalCost,
                availability = s.Availability
            })
            .ToListAsync();

        var requests = await _context.LearningRequests
            .AsNoTracking()
            .Select(r => new
            {
                requestId = r.RequestId,
                organization = r.Organization,
                programType = r.ProgramType,
                studentCapacity = r.StudentCapacity,
                budget = r.Budget,
                preferredLocation = r.PreferredLocation,
                preferredLatitude = r.PreferredLatitude,
                preferredLongitude = r.PreferredLongitude,
                communityId = r.CommunityId
            })
            .ToListAsync();

        var communities = await _context.Communities
            .AsNoTracking()
            .Select(c => new
            {
                communityId = c.CommunityId,
                population = c.Population,
                studentCount = c.StudentCount,
                literacyRate = c.LiteracyRate,
                incomeGroup = c.IncomeGroup
            })
            .ToListAsync();

        if (spaces.Count == 0)
        {
            return BadRequest(new
            {
                message = "No available spaces found."
            });
        }

        if (requests.Count == 0)
        {
            return BadRequest(new
            {
                message = "No learning requests found."
            });
        }

        var response = await _httpClient.PostAsJsonAsync(
            "/optimization",
            new
            {
                spaces,
                requests,
                communities
            });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            return StatusCode(
                (int)response.StatusCode,
                new
                {
                    message = "AI engine optimization failed.",
                    details = error
                });
        }

        var result =
            await response.Content.ReadFromJsonAsync<OptimizationResponse>();

        if (result == null)
        {
            return StatusCode(
                502,
                new
                {
                    message = "AI engine returned an empty response."
                });
        }

        foreach (var match in result.Matches)
        {
            var existing = await _context.Allocations
                .FirstOrDefaultAsync(a => a.MatchId == match.MatchId);

            if (existing == null)
            {
                _context.Allocations.Add(new Allocation
                {
                    SpaceId = match.SpaceId,
                    RequestId = match.RequestId,
                    MatchId = match.MatchId,
                    DistanceKm = match.DistanceKm,
                    MatchScore = match.MatchScore,
                    CapacityUtilization = match.CapacityUtilization,
                    CostEfficiency = match.CostEfficiency,
                    StudentReach = match.StudentReach,
                    StudentsServed = match.StudentsServed,
                    OptimizationScore = match.OptimizationScore,
                    Status = "Recommended"
                });
            }
            else
            {
                existing.DistanceKm = match.DistanceKm;
                existing.MatchScore = match.MatchScore;
                existing.CapacityUtilization = match.CapacityUtilization;
                existing.CostEfficiency = match.CostEfficiency;
                existing.StudentReach = match.StudentReach;
                existing.StudentsServed = match.StudentsServed;
                existing.OptimizationScore = match.OptimizationScore;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(result);
    }

    private sealed class OptimizationResponse
    {
        public int CandidateCount { get; set; }
        public int MatchCount { get; set; }
        public List<OptimizationMatch> Matches { get; set; } = new();
        public OptimizationSummary Summary { get; set; } = new();
    }

    private sealed class OptimizationMatch
    {
        public string MatchId { get; set; } = string.Empty;
        public int SpaceId { get; set; }
        public int RequestId { get; set; }
        public double DistanceKm { get; set; }
        public double MatchScore { get; set; }
        public double CapacityUtilization { get; set; }
        public double CostEfficiency { get; set; }
        public double StudentReach { get; set; }
        public int StudentsServed { get; set; }
        public double OptimizationScore { get; set; }
    }

    private sealed class OptimizationSummary
    {
        public int TotalStudentsServed { get; set; }
        public double AverageDistanceKm { get; set; }
        public double AverageCapacityUtilization { get; set; }
        public double AverageCostEfficiency { get; set; }
        public double AverageStudentReach { get; set; }
        public double OverallOptimizationScore { get; set; }
    }
}
'@

Write-Host ""
Write-Host "EduSpace Allocator implementation update applied to source files." -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANT: this does NOT start or stop your servers." -ForegroundColor Yellow
Write-Host ""
Write-Host "Next verification:" -ForegroundColor Cyan
Write-Host "1) Stop the running ASP.NET API terminal before rebuilding if its EXE is locked."
Write-Host "2) cd $backend"
Write-Host "3) dotnet build"
Write-Host "4) cd $frontend"
Write-Host "5) npm run build"
Write-Host "6) Start AI engine on port 8000."
Write-Host "7) Start ASP.NET API on port 5244."
Write-Host "8) Start React with npm run dev."
Write-Host "9) Open Smart Allocation and click Run Smart Allocation."
Write-Host "10) Open Allocation Results and verify the saved row appears."
