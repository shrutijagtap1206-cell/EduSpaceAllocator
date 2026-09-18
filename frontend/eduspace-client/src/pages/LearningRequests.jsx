import { useEffect, useMemo, useState } from 'react';
import api from '../services/api';

function LearningRequests() {
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [program, setProgram] = useState('all');
  const [sortBy, setSortBy] = useState('id');

  useEffect(() => {
    api.get('/LearningRequests')
      .then((response) => setRequests(response.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const programs = [...new Set(requests.map((r) => r.programType))];

  const filteredRequests = useMemo(() => {
    let result = requests.filter((request) => {
      const text = `${request.organization} ${request.programType} ${request.preferredLocation}`.toLowerCase();

      return (
        text.includes(search.toLowerCase()) &&
        (program === 'all' || request.programType === program)
      );
    });

    result = [...result].sort((a, b) => {
      if (sortBy === 'students') return b.studentCapacity - a.studentCapacity;
      if (sortBy === 'budget') return Number(b.budget) - Number(a.budget);
      return a.requestId - b.requestId;
    });

    return result;
  }, [requests, search, program, sortBy]);

  return (
    <div>
      <div className="page-heading">
        <div>
          <h2>Learning Requests</h2>
          <p className="page-subtitle">
            Educational organizations requesting learning spaces.
          </p>
        </div>
      </div>

      <div className="filter-bar">
        <input
          className="search-input"
          placeholder="Search organization, program or location..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select value={program} onChange={(e) => setProgram(e.target.value)}>
          <option value="all">All Programs</option>
          {programs.map((item) => (
            <option key={item} value={item}>{item}</option>
          ))}
        </select>

        <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
          <option value="id">Sort: ID</option>
          <option value="students">Sort: Students</option>
          <option value="budget">Sort: Budget</option>
        </select>
      </div>

      <div className="result-count">
        Showing {filteredRequests.length} of {requests.length} requests
      </div>

      {loading ? (
        <p>Loading requests...</p>
      ) : (
        <div style={{ overflowX: 'auto' }}>
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Organization</th>
                <th>Program</th>
                <th>Students</th>
                <th>Budget</th>
                <th>Preferred Location</th>
              </tr>
            </thead>

            <tbody>
              {filteredRequests.map((request) => (
                <tr key={request.requestId}>
                  <td>{request.requestId}</td>
                  <td>{request.organization}</td>
                  <td>{request.programType}</td>
                  <td>{request.studentCapacity}</td>
                  <td>Rs. {Number(request.budget).toLocaleString()}</td>
                  <td>{request.preferredLocation}</td>
                </tr>
              ))}

              {filteredRequests.length === 0 && (
                <tr>
                  <td colSpan="6">No learning requests match the selected filters.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default LearningRequests;
