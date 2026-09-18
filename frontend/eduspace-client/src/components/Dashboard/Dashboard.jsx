import { useEffect, useState } from 'react';
import api from '../../services/api';

function Dashboard() {
  const [metrics, setMetrics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchDashboard = () => {
    setLoading(true);
    setError(null);
    api
      .get('/Dashboard')
      .then((res) => {
        setMetrics(res.data);
      })
      .catch((err) => {
        console.error(err);
        setError('Unable to connect to server. Please check that the backend is running.');
      })
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchDashboard();
  }, []);

  if (loading) {
    return (
      <div className="state-container loading-state">
        <div className="spinner"></div>
        <p>Loading dashboard...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="state-container error-state">
        <div className="error-icon">⚠️</div>
        <h3>Connection Error</h3>
        <p>{error}</p>
        <button className="primary-btn" onClick={fetchDashboard}>
          Retry Connection
        </button>
      </div>
    );
  }

  const primaryCards = [
    { label: 'Available Spaces', value: `${metrics.availableSpaces} / ${metrics.totalSpaces}`, sub: 'Ready for allocation' },
    { label: 'Learning Requests', value: metrics.learningRequests, sub: 'Community requests' },
    { label: 'Communities', value: metrics.communities, sub: 'Registered clusters' },
    { label: 'Matched Allocations', value: metrics.matchedAllocations, sub: '21 Feasible pairs' },
    { label: 'Students Served', value: metrics.studentsServed.toLocaleString(), sub: 'Target beneficiaries' },
  ];

  const impactCards = [
    { label: 'Avg Travel Distance', value: `${metrics.averageDistanceKm} km`, highlight: 'Reduced Transit' },
    { label: 'Capacity Utilization', value: `${metrics.averageCapacityUtilization}%`, highlight: 'High Efficiency' },
    { label: 'Avg Optimization Score', value: `${metrics.averageOptimizationScore}%`, highlight: 'Multi-Objective' },
    { label: 'Ecosystem Partners', value: `${metrics.partners} NGOs / Edu`, highlight: 'Collaborations' },
    { label: 'Active Courses', value: `${metrics.courses} Programs`, highlight: 'Enriched Curriculum' },
  ];

  return (
    <div className="dashboard-view">
      <div className="page-heading">
        <div>
          <h2>Platform Overview</h2>
          <p className="page-subtitle">
            Real-time education-space allocation performance & infrastructure metrics.
          </p>
        </div>
        <button className="secondary-btn" onClick={fetchDashboard}>
          🔄 Refresh
        </button>
      </div>

      <h3 className="section-title">Core Allocations & Infrastructure</h3>
      <div className="kpi-grid">
        {primaryCards.map((card) => (
          <div className="kpi-card" key={card.label}>
            <span className="kpi-label">{card.label}</span>
            <strong className="kpi-value">{card.value}</strong>
            <span className="kpi-sub">{card.sub}</span>
          </div>
        ))}
      </div>

      <h3 className="section-title" style={{ marginTop: '24px' }}>Efficiency & Social Impact Indicators</h3>
      <div className="kpi-grid">
        {impactCards.map((card) => (
          <div className="kpi-card" key={card.label}>
            <span className="kpi-label">{card.label}</span>
            <strong className="kpi-value">{card.value}</strong>
            <span className="badge badge-recommended">{card.highlight}</span>
          </div>
        ))}
      </div>

      <div className="dashboard-grid" style={{ marginTop: '32px' }}>
        <div className="feature-card">
          <div className="feature-icon">📍</div>
          <h3>GIS Coverage</h3>
          <p>
            Spatial mapping of Pune learning spaces and community demand centroids powered by PostGIS geometry layers.
          </p>
        </div>

        <div className="feature-card">
          <div className="feature-icon">⚡</div>
          <h3>Smart Allocation Engine</h3>
          <p>
            Bipartite maximum-weight matching balancing travel distance, space capacity, and program budget constraints.
          </p>
        </div>

        <div className="feature-card">
          <div className="feature-icon">🎯</div>
          <h3>UN SDG Alignment</h3>
          <p>
            Supporting SDG 4 (Quality Education), SDG 10 (Reduced Inequalities), and SDG 11 (Sustainable Cities & Communities).
          </p>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
