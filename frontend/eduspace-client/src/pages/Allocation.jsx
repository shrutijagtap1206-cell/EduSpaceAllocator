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
        'Unable to run Smart Allocation. Check that the backend and AI engine are running.'
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
            Find suitable learning spaces for registered educational demand.
          </p>
        </div>

        <button
          className="primary-button"
          onClick={runAllocation}
          disabled={loading}
        >
          {loading ? 'Running Allocation...' : 'Run Smart Allocation'}
        </button>
      </div>

      {error && <div className="alert error">{error}</div>}

      {!result && !loading && !error && (
        <div className="feature-card empty-state">
          <h3>Ready to allocate</h3>
          <p>
            Start Smart Allocation to analyze available spaces and learning
            requests and generate optimized recommendations.
          </p>
        </div>
      )}

      {loading && (
        <div className="feature-card empty-state">
          <h3>Allocation engine is running...</h3>
          <p>
            Evaluating available spaces, educational demand, location,
            capacity and budget constraints.
          </p>
        </div>
      )}

      {result && (
        <>
          <div className="kpi-grid">
            <div className="kpi-card">
              <span>Feasible Candidates</span>
              <strong>{result.candidateCount}</strong>
            </div>

            <div className="kpi-card">
              <span>Matched Allocations</span>
              <strong>{result.matchCount}</strong>
            </div>

            <div className="kpi-card">
              <span>Students Served</span>
              <strong>{result.summary?.totalStudentsServed ?? 0}</strong>
            </div>

            <div className="kpi-card">
              <span>Average Distance</span>
              <strong>
                {result.summary?.averageDistanceKm ?? 0} km
              </strong>
            </div>

            <div className="kpi-card">
              <span>Capacity Utilization</span>
              <strong>
                {(
                  (result.summary?.averageCapacityUtilization ?? 0) * 100
                ).toFixed(1)}
                %
              </strong>
            </div>

            <div className="kpi-card">
              <span>Optimization Score</span>
              <strong>
                {(
                  (result.summary?.overallOptimizationScore ?? 0) * 100
                ).toFixed(1)}
                %
              </strong>
            </div>
          </div>

          <div className="feature-card">
            <div className="page-heading">
              <div>
                <h3>Generated Allocation Plan</h3>
                <p className="page-subtitle">
                  {result.matchCount} space-demand pairings generated.
                </p>
              </div>
            </div>

            <div className="table-wrap">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Match</th>
                    <th>Space</th>
                    <th>Request</th>
                    <th>Students</th>
                    <th>Distance</th>
                    <th>Capacity</th>
                    <th>Cost</th>
                    <th>Optimization</th>
                  </tr>
                </thead>

                <tbody>
                  {result.matches?.map((match) => (
                    <tr key={match.matchId}>
                      <td>{match.matchId}</td>
                      <td>Space {match.spaceId}</td>
                      <td>Request {match.requestId}</td>
                      <td>{match.studentsServed}</td>
                      <td>{match.distanceKm} km</td>
                      <td>
                        {(match.capacityUtilization * 100).toFixed(1)}%
                      </td>
                      <td>
                        {(match.costEfficiency * 100).toFixed(1)}%
                      </td>
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
          </div>
        </>
      )}
    </div>
  );
}

export default Allocation;
