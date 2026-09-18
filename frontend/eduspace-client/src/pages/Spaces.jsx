import { useEffect, useMemo, useState } from 'react';
import api from '../services/api';

function Spaces() {
  const [spaces, setSpaces] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [availability, setAvailability] = useState('all');
  const [sortBy, setSortBy] = useState('id');

  useEffect(() => {
    api.get('/Spaces')
      .then((response) => setSpaces(response.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const filteredSpaces = useMemo(() => {
    let result = spaces.filter((space) => {
      const text = `${space.buildingName} ${space.address} ${space.city}`.toLowerCase();
      const matchesSearch = text.includes(search.toLowerCase());

      const matchesAvailability =
        availability === 'all' ||
        (availability === 'available' && space.availability) ||
        (availability === 'unavailable' && !space.availability);

      return matchesSearch && matchesAvailability;
    });

    result = [...result].sort((a, b) => {
      if (sortBy === 'capacity') return b.capacity - a.capacity;
      if (sortBy === 'rent') return Number(a.rentalCost) - Number(b.rentalCost);
      if (sortBy === 'area') return b.floorArea - a.floorArea;
      return a.spaceId - b.spaceId;
    });

    return result;
  }, [spaces, search, availability, sortBy]);

  return (
    <div>
      <div className="page-heading">
        <div>
          <h2>Space Registry</h2>
          <p className="page-subtitle">
            Manage available educational learning spaces.
          </p>
        </div>
      </div>

      <div className="filter-bar">
        <input
          className="search-input"
          placeholder="Search building, address or city..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select value={availability} onChange={(e) => setAvailability(e.target.value)}>
          <option value="all">All Status</option>
          <option value="available">Available</option>
          <option value="unavailable">Unavailable</option>
        </select>

        <select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
          <option value="id">Sort: ID</option>
          <option value="capacity">Sort: Capacity</option>
          <option value="rent">Sort: Rental Cost</option>
          <option value="area">Sort: Area</option>
        </select>
      </div>

      <div className="result-count">
        Showing {filteredSpaces.length} of {spaces.length} spaces
      </div>

      {loading ? (
        <p>Loading spaces...</p>
      ) : (
        <div style={{ overflowX: 'auto' }}>
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Building</th>
                <th>Address</th>
                <th>City</th>
                <th>Capacity</th>
                <th>Area</th>
                <th>Rental Cost</th>
                <th>Status</th>
              </tr>
            </thead>

            <tbody>
              {filteredSpaces.map((space) => (
                <tr key={space.spaceId}>
                  <td>{space.spaceId}</td>
                  <td>{space.buildingName}</td>
                  <td>{space.address}</td>
                  <td>{space.city}</td>
                  <td>{space.capacity}</td>
                  <td>{space.floorArea} sq. ft.</td>
                  <td>Rs. {Number(space.rentalCost).toLocaleString()}</td>
                  <td>{space.availability ? 'Available' : 'Unavailable'}</td>
                </tr>
              ))}

              {filteredSpaces.length === 0 && (
                <tr>
                  <td colSpan="8">No spaces match the selected filters.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default Spaces;
