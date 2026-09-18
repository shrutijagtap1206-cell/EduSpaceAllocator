import { useEffect, useMemo, useState } from 'react';
import api from '../services/api';

function Communities() {
  const [communities, setCommunities] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [income, setIncome] = useState('all');
  const [sortBy, setSortBy] = useState('id');

  useEffect(() => {
    api.get('/Communities')
      .then((response) => setCommunities(response.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const filteredCommunities = useMemo(() => {
    let result = communities.filter((community) => {
      const idText = String(community.communityId);

      return (
        (idText.includes(search) || community.incomeGroup.toLowerCase().includes(search.toLowerCase())) &&
        (income === 'all' || community.incomeGroup === income)
      );
    });

    result = [...result].sort((a, b) => {
      if (sortBy === 'students') return b.studentCount - a.studentCount;
      if (sortBy === 'population') return b.population - a.population;
      if (sortBy === 'literacy') return b.literacyRate - a.literacyRate;
      return a.communityId - b.communityId;
    });

    return result;
  }, [communities, search, income, sortBy]);

  return (
    <div>
      <div className="page-heading">
        <div>
          <h2>Community Registry</h2>
          <p className="page-subtitle">
            Community demographic and education-demand information.
          </p>
        </div>
      </div>

      <div className="filter-bar">
        <input
          className="search-input"
          placeholder="Search community ID or income group..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select value={income} onChange={(e) => setIncome(e.target.value)}>
          <option value="all">All Income Groups</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
        </select>

        <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
          <option value="id">Sort: ID</option>
          <option value="students">Sort: Students</option>
          <option value="population">Sort: Population</option>
          <option value="literacy">Sort: Literacy</option>
        </select>
      </div>

      <div className="result-count">
        Showing {filteredCommunities.length} of {communities.length} communities
      </div>

      {loading ? (
        <p>Loading communities...</p>
      ) : (
        <div style={{ overflowX: 'auto' }}>
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Population</th>
                <th>Students</th>
                <th>Literacy Rate</th>
                <th>Income Group</th>
              </tr>
            </thead>

            <tbody>
              {filteredCommunities.map((community) => (
                <tr key={community.communityId}>
                  <td>{community.communityId}</td>
                  <td>{community.population.toLocaleString()}</td>
                  <td>{community.studentCount.toLocaleString()}</td>
                  <td>{community.literacyRate}%</td>
                  <td>{community.incomeGroup}</td>
                </tr>
              ))}

              {filteredCommunities.length === 0 && (
                <tr>
                  <td colSpan="5">No communities match the selected filters.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default Communities;
