import { useEffect, useState } from 'react';
import api from '../services/api';

function SDGDashboard() {
  const [sdgs, setSdgs] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    api.get('/SDG')
      .then(response => setSdgs(response.data))
      .catch(() => setError('Unable to load SDG data.'));
  }, []);

  if (error) return <div className="page-card">{error}</div>;
  if (!sdgs) return <div className="page-card">Loading SDG dashboard...</div>;

  return (
    <div>
      <div className="page-header">
        <div>
          <h2>SDG Dashboard</h2>
          <p className="page-subtitle">
            Platform indicators mapped to the Sustainable Development Goals.
          </p>
        </div>
      </div>

      <div className="stats-grid">
        {Object.entries(sdgs).map(([key, sdg]) => (
          <div className="page-card" key={key}>
            <div className="sdg-number">
              {key.replace('sdg', 'SDG ')}
            </div>

            <h3>{sdg.name}</h3>
            <p>{sdg.indicator}</p>
            <strong className="sdg-value">{sdg.value}</strong>
          </div>
        ))}
      </div>
    </div>
  );
}

export default SDGDashboard;
