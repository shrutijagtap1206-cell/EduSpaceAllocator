import { useEffect, useState } from 'react';
import {
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  Legend,
  CartesianGrid,
  LineChart,
  Line,
} from 'recharts';
import api from '../services/api';

function Analytics() {
  const [allocations, setAllocations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchAllocations = () => {
    setLoading(true);
    setError(null);
    api
      .get('/Allocations')
      .then((response) => {
        setAllocations(response.data || []);
      })
      .catch((err) => {
        console.error(err);
        setError('Unable to connect to server. Please check that the backend is running.');
      })
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchAllocations();
  }, []);

  if (loading) {
    return (
      <div className="state-container loading-state">
        <div className="spinner"></div>
        <p>Loading analytics...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="state-container error-state">
        <div className="error-icon">⚠️</div>
        <h3>Connection Error</h3>
        <p>{error}</p>
        <button className="primary-btn" onClick={fetchAllocations}>
          Retry
        </button>
      </div>
    );
  }

  if (allocations.length === 0) {
    return (
      <div className="state-container empty-state">
        <div className="empty-icon">📊</div>
        <h3>No Allocations Found</h3>
        <p>No allocation results available to analyze. Run Smart Allocation first.</p>
      </div>
    );
  }

  const average = (key) =>
    allocations.length
      ? allocations.reduce((sum, item) => sum + (Number(item[key]) || 0), 0) /
        allocations.length
      : 0;

  const totalStudents = allocations.reduce(
    (sum, item) => sum + (Number(item.studentsServed) || 0),
    0
  );

  const avgDistance = average('distanceKm');
  const avgUtilization = average('capacityUtilization') * 100;
  const avgCostEfficiency = average('costEfficiency') * 100;
  const avgOptimization = average('optimizationScore') * 100;

  // Chart 1 data: Top Allocations by Optimization Score (%)
  const chartDataScores = allocations.slice(0, 10).map((a) => ({
    name: a.matchId || `Alloc #${a.allocationId}`,
    score: Math.round((Number(a.optimizationScore) || 0) * 100),
    utilization: Math.round((Number(a.capacityUtilization) || 0) * 100),
    distance: Number(Number(a.distanceKm || 0).toFixed(2)),
  }));

  // Chart 2 data: Distribution of Distance vs Students
  const chartDataDistance = allocations.slice(0, 10).map((a) => ({
    name: a.matchId || `Alloc #${a.allocationId}`,
    distanceKm: Number(Number(a.distanceKm || 0).toFixed(2)),
    students: a.studentsServed || 0,
  }));

  return (
    <div className="analytics-page">
      <div className="page-heading">
        <div>
          <h2>Allocation Analytics & Performance</h2>
          <p className="page-subtitle">
            Quantitative evaluation of multi-objective matching efficiency.
          </p>
        </div>
        <button className="secondary-btn" onClick={fetchAllocations}>
          🔄 Refresh
        </button>
      </div>

      <div className="kpi-grid">
        <div className="kpi-card">
          <span className="kpi-label">Average Distance</span>
          <strong className="kpi-value">{avgDistance.toFixed(2)} km</strong>
          <span className="badge badge-recommended">Proximity Focused</span>
        </div>

        <div className="kpi-card">
          <span className="kpi-label">Capacity Utilization</span>
          <strong className="kpi-value">{avgUtilization.toFixed(1)}%</strong>
          <span className="badge badge-approved">Space Optimized</span>
        </div>

        <div className="kpi-card">
          <span className="kpi-label">Cost Efficiency</span>
          <strong className="kpi-value">{avgCostEfficiency.toFixed(1)}%</strong>
          <span className="badge badge-recommended">Budget Adherent</span>
        </div>

        <div className="kpi-card">
          <span className="kpi-label">Student Reach</span>
          <strong className="kpi-value">{totalStudents.toLocaleString()}</strong>
          <span className="badge badge-completed">Students Served</span>
        </div>

        <div className="kpi-card">
          <span className="kpi-label">Optimization Score</span>
          <strong className="kpi-value">{avgOptimization.toFixed(1)}%</strong>
          <span className="badge badge-approved">Composite Score</span>
        </div>
      </div>

      <div className="charts-grid" style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', marginTop: '32px' }}>
        <div className="chart-card" style={{ background: '#1e293b', borderRadius: '12px', padding: '20px', border: '1px solid #334155' }}>
          <h3 style={{ marginBottom: '16px', fontSize: '1.1rem', color: '#f8fafc' }}>
            Optimization Score & Utilization by Allocation (%)
          </h3>
          <ResponsiveContainer width="100%" height={320}>
            <BarChart data={chartDataScores} margin={{ top: 10, right: 20, left: -10, bottom: 40 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
              <XAxis dataKey="name" stroke="#94a3b8" angle={-45} textAnchor="end" height={60} />
              <YAxis stroke="#94a3b8" domain={[0, 100]} />
              <Tooltip contentStyle={{ backgroundColor: '#0f172a', borderColor: '#475569', color: '#f8fafc' }} />
              <Legend verticalAlign="top" wrapperStyle={{ paddingBottom: '10px' }} />
              <Bar dataKey="score" name="Optimization Score %" fill="#3b82f6" radius={[4, 4, 0, 0]} />
              <Bar dataKey="utilization" name="Capacity Utilization %" fill="#10b981" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>

        <div className="chart-card" style={{ background: '#1e293b', borderRadius: '12px', padding: '20px', border: '1px solid #334155' }}>
          <h3 style={{ marginBottom: '16px', fontSize: '1.1rem', color: '#f8fafc' }}>
            Travel Distance (km) vs Students Served
          </h3>
          <ResponsiveContainer width="100%" height={320}>
            <LineChart data={chartDataDistance} margin={{ top: 10, right: 20, left: -10, bottom: 40 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
              <XAxis dataKey="name" stroke="#94a3b8" angle={-45} textAnchor="end" height={60} />
              <YAxis stroke="#94a3b8" />
              <Tooltip contentStyle={{ backgroundColor: '#0f172a', borderColor: '#475569', color: '#f8fafc' }} />
              <Legend verticalAlign="top" wrapperStyle={{ paddingBottom: '10px' }} />
              <Line type="monotone" dataKey="distanceKm" name="Distance (km)" stroke="#f59e0b" strokeWidth={3} dot={{ r: 5 }} />
              <Line type="monotone" dataKey="students" name="Students Served" stroke="#ec4899" strokeWidth={2} dot={{ r: 4 }} />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
}

export default Analytics;
