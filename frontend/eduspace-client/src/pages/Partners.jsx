import { useEffect, useMemo, useState } from 'react';
import api from '../services/api';

export default function Partners() {
  const [items, setItems] = useState([]);
  const [search, setSearch] = useState('');
  const [type, setType] = useState('all');
  const [status, setStatus] = useState('all');

  useEffect(() => {
    api.get('/Partners')
      .then((response) => setItems(response.data))
      .catch(console.error);
  }, []);

  const types = [...new Set(items.map((x) => x.partnerType))];

  const filtered = useMemo(() => {
    return items.filter((item) => {
      const text = `${item.organizationName} ${item.contactPerson} ${item.email}`.toLowerCase();

      return (
        text.includes(search.toLowerCase()) &&
        (type === 'all' || item.partnerType === type) &&
        (status === 'all' || item.status === status)
      );
    });
  }, [items, search, type, status]);

  return (
    <div>
      <div className="page-heading">
        <div>
          <h2>Partners</h2>
          <p className="page-subtitle">
            Organizations supporting learning-space delivery.
          </p>
        </div>
      </div>

      <div className="filter-bar">
        <input
          className="search-input"
          placeholder="Search organization or contact..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select value={type} onChange={(e) => setType(e.target.value)}>
          <option value="all">All Partner Types</option>
          {types.map((item) => (
            <option key={item} value={item}>{item}</option>
          ))}
        </select>

        <select value={status} onChange={(e) => setStatus(e.target.value)}>
          <option value="all">All Status</option>
          <option value="Active">Active</option>
          <option value="Inactive">Inactive</option>
        </select>
      </div>

      <div className="result-count">
        Showing {filtered.length} of {items.length} partners
      </div>

      <div className="card">
        <table className="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Organization</th>
              <th>Type</th>
              <th>Contact</th>
              <th>Status</th>
            </tr>
          </thead>

          <tbody>
            {filtered.map((item) => (
              <tr key={item.partnerId}>
                <td>{item.partnerId}</td>
                <td>{item.organizationName}</td>
                <td>{item.partnerType}</td>
                <td>{item.contactPerson}</td>
                <td>{item.status}</td>
              </tr>
            ))}

            {filtered.length === 0 && (
              <tr>
                <td colSpan="5">No partners match the selected filters.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
