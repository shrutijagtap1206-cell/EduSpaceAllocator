import { useEffect, useState } from 'react';
import api from '../services/api';

function Dashboard() {
  const [metrics, setMetrics] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    api.get('/Dashboard')
      .then(response => setMetrics(response.data))
      .catch(() => setError('Unable to load dashboard data.'));
  }, []);

  if (error) {
    return <div className="page-card">{error}</div>;
  }

  if (!metrics) {
    return <div className="page-card">Loading dashboard...</div>;
  }

  const cards = [
    ['Available Spaces', metrics.availableSpaces],
    ['Learning Requests', metrics.learningRequests],
    ['Communities', metrics.communities],
    ['Matched Allocations', metrics.matchedAllocations],
    ['Students Served', metrics.studentsServed],
    ['Partners', metrics.partners],
    ['Courses', metrics.courses],
    ['Scheduled Sessions', metrics.scheduledSessions],
  ];

  return (
    <div>
      <div className="page-header">
        <div>
          <h2>Dashboard</h2>
          <p className="page-subtitle">
            Overview of educational space allocation and community impact.
          </p>
        </div>
      </div>

      <div className="stats-grid">
        {cards.map(([label, value]) => (
          <div className="stat-card" key={label}>
            <span>{label}</span>
            <strong>{value}</strong>
          </div>
        ))}
      </div>

      <div className="content-grid">
        <div className="page-card">
          <h3>Allocation Performance</h3>

          <div className="metric-row">
            <span>Average Optimization Score</span>
            <strong>{metrics.averageOptimizationScore}%</strong>
          </div>

          <div className="metric-row">
            <span>Average Distance</span>
            <strong>{metrics.averageDistanceKm} km</strong>
          </div>

          <div className="metric-row">
            <span>Capacity Utilization</span>
            <strong>{metrics.averageCapacityUtilization}%</strong>
          </div>
        </div>

        <div className="page-card">
          <h3>Platform Status</h3>
          <p>
            The platform is connecting available spaces with educational
            demand using location, capacity and optimization data.
          </p>
          <div className="status-badge">System Active</div>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
