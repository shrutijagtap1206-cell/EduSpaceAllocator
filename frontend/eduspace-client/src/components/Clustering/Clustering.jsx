import { useState } from 'react';
import api from '../../services/api';

function Clustering() {
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const runClustering = async () => {
    try {
      setLoading(true);
      setError('');

      const response = await api.post('/Clustering/run?nClusters=3');
      setResults(response.data.results);
    } catch (err) {
      console.error(err);
      setError('Failed to run K-Means clustering.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: '24px' }}>
      <h1>K-Means Clustering</h1>

      <p>
        Communities are grouped using population, student demand,
        literacy rate, and income group.
      </p>

      <button onClick={runClustering} disabled={loading}>
        {loading ? 'Running K-Means...' : 'Run K-Means'}
      </button>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      {results.length > 0 && (
        <table
          style={{
            width: '100%',
            marginTop: '24px',
            borderCollapse: 'collapse'
          }}
        >
          <thead>
            <tr>
              <th>Community</th>
              <th>Population</th>
              <th>Students</th>
              <th>Literacy</th>
              <th>Income</th>
              <th>Cluster</th>
              <th>Demand Level</th>
            </tr>
          </thead>

          <tbody>
            {results.map((community) => (
              <tr key={community.communityId}>
                <td>{community.communityId}</td>
                <td>{community.population}</td>
                <td>{community.studentCount}</td>
                <td>{community.literacyRate}%</td>
                <td>{community.incomeGroup}</td>
                <td>{community.cluster}</td>
                <td><strong>{community.clusterLabel}</strong></td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default Clustering;
