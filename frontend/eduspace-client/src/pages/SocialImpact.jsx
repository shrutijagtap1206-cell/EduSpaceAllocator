import { useEffect, useState } from 'react';
import api from '../services/api';

function SocialImpact() {
  const [metrics, setMetrics] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    api.get('/SocialImpact')
      .then(response => setMetrics(response.data))
      .catch(() => setError('Unable to load social impact data.'));
  }, []);

  if (error) return <div className="page-card">{error}</div>;
  if (!metrics) return <div className="page-card">Loading social impact...</div>;

  const cards = [
    ['Students Served', metrics.studentsServed],
    ['Learning Hours', metrics.learningHoursDelivered],
    ['Communities Reached', metrics.communitiesReached],
    ['Spaces Reused', metrics.spacesReused],
    ['Scheduled Sessions', metrics.scheduledSessions],
    ['Total Allocations', metrics.totalAllocations],
  ];

  return (
    <div>
      <div className="page-header">
        <div>
          <h2>Social Impact</h2>
          <p className="page-subtitle">
            Measure how allocated educational spaces contribute to community learning.
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

      <div className="page-card">
        <h3>Impact Overview</h3>
        <p>
          EduSpace Allocator tracks students served, learning hours,
          communities reached and reuse of underutilized spaces.
        </p>
      </div>
    </div>
  );
}

export default SocialImpact;
