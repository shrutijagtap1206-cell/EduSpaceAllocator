import { useEffect, useState } from 'react';
import api from '../services/api';

function StudentDemand() {
  const [communities, setCommunities] = useState([]);
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([api.get('/Communities'), api.get('/LearningRequests')])
      .then(([communitiesResponse, requestsResponse]) => {
        setCommunities(communitiesResponse.data);
        setRequests(requestsResponse.data);
      })
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return <div className="loading">Loading student demand...</div>;
  }

  const totalStudents = communities.reduce(
    (sum, community) => sum + community.studentCount,
    0
  );

  const requestedCapacity = requests.reduce(
    (sum, request) => sum + request.studentCapacity,
    0
  );

  return (
    <div>
      <h2>Student Demand</h2>
      <p className="page-subtitle">
        Education demand currently registered in the platform.
      </p>

      <div className="kpi-grid">
        <div className="kpi-card">
          <span>Total Students</span>
          <strong>{totalStudents}</strong>
        </div>

        <div className="kpi-card">
          <span>Requested Capacity</span>
          <strong>{requestedCapacity}</strong>
        </div>

        <div className="kpi-card">
          <span>Communities</span>
          <strong>{communities.length}</strong>
        </div>

        <div className="kpi-card">
          <span>Learning Requests</span>
          <strong>{requests.length}</strong>
        </div>
      </div>

      <div className="feature-card">
        <h3>Community Demand</h3>

        <div className="table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Community</th>
                <th>Students</th>
                <th>Literacy</th>
                <th>Income Group</th>
              </tr>
            </thead>

            <tbody>
              {communities.map((community) => (
                <tr key={community.communityId}>
                  <td>Community {community.communityId}</td>
                  <td>{community.studentCount}</td>
                  <td>{community.literacyRate}%</td>
                  <td>{community.incomeGroup}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default StudentDemand;
