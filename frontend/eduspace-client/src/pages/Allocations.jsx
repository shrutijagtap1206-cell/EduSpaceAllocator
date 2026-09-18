import { useEffect, useMemo, useState } from 'react';
import api from '../services/api';

function Allocations() {
  const [allocations, setAllocations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('all');
  const [sortBy, setSortBy] = useState('id');

  const load = () => {
    setLoading(true);

    api.get('/Allocations')
      .then((response) => setAllocations(response.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
  }, []);

  const filtered = useMemo(() => {
    let result = allocations.filter((allocation) => {
      const text = `${allocation.matchId} ${allocation.spaceId} ${allocation.requestId}`.toLowerCase();

      return (
        text.includes(search.toLowerCase()) &&
        (status === 'all' || allocation.status === status)
      );
    });

    result = [...result].sort((a, b) => {
      if (sortBy === 'optimization') {
        return b.optimizationScore - a.optimizationScore;
      }

      if (sortBy === 'distance') {
        return a.distanceKm - b.distanceKm;
      }

      if (sortBy === 'students') {
        return b.studentsServed - a.studentsServed;
      }

      return a.allocationId - b.allocationId;
    });

    return result;
  }, [allocations, search, status, sortBy]);

  const statuses = [...new Set(allocations.map((a) => a.status))];

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

      <div className="filter-bar">
        <input
          className="search-input"
          placeholder="Search match, space or request..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select value={status} onChange={(e) => setStatus(e.target.value)}>
          <option value="all">All Statuses</option>
          {statuses.map((item) => (
            <option key={item} value={item}>{item}</option>
          ))}
        </select>

        <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
          <option value="id">Sort: ID</option>
          <option value="optimization">Sort: Optimization</option>
          <option value="distance">Sort: Distance</option>
          <option value="students">Sort: Students Served</option>
        </select>
      </div>

      <div className="result-count">
        Showing {filtered.length} of {allocations.length} allocations
      </div>

      <div className="feature-card">
        {filtered.length === 0 ? (
          <div className="empty-state">
            <h3>No matching allocations</h3>
            <p>Try changing the search or filters.</p>
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
                {filtered.map((allocation) => (
                  <tr key={allocation.allocationId}>
                    <td>{allocation.matchId}</td>
                    <td>{allocation.spaceId}</td>
                    <td>{allocation.requestId}</td>
                    <td>{allocation.studentsServed}</td>
                    <td>{allocation.distanceKm} km</td>
                    <td>{(allocation.optimizationScore * 100).toFixed(1)}%</td>
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
