import { useEffect, useState } from 'react';
import { getSpaces } from './services/spaceService';
import GISMap from './components/GIS/GISMap';

function App() {
  const [spaces, setSpaces] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    getSpaces()
      .then(data => {
        setSpaces(data);
        setLoading(false);
      })
      .catch(err => {
        console.error(err);
        setError('Failed to load spaces from the API.');
        setLoading(false);
      });
  }, []);

  return (
    <div style={{ padding: '24px', fontFamily: 'Arial, sans-serif' }}>
      <h1>EduSpace Allocator</h1>

      <h2>GIS Mapping</h2>

      <div
        style={{
          background: '#f5f5f5',
          padding: '12px 16px',
          borderRadius: '8px',
          marginBottom: '16px'
        }}
      >
        <strong>Legend:</strong> 🟦 Available Learning Spaces
      </div>

      {loading && <p>Loading spaces...</p>}

      {error && (
        <p style={{ color: 'red' }}>
          {error}
        </p>
      )}

      {!loading && !error && (
        <>
          <GISMap spaces={spaces} />

          <h2 style={{ marginTop: '24px' }}>
            Available Learning Spaces
          </h2>

          {spaces.length === 0 ? (
            <p>No spaces found.</p>
          ) : (
            spaces.map(space => (
              <div
                key={space.spaceId}
                style={{
                  border: '1px solid #ddd',
                  padding: '16px',
                  marginBottom: '12px',
                  borderRadius: '8px'
                }}
              >
                <h3>{space.buildingName}</h3>
                <p><strong>Address:</strong> {space.address}</p>
                <p><strong>City:</strong> {space.city}</p>
                <p><strong>Capacity:</strong> {space.capacity}</p>
                <p><strong>Floor Area:</strong> {space.floorArea} sq. ft.</p>
                <p><strong>Rental Cost:</strong> Rs. {space.rentalCost}</p>
                <p>
                  <strong>Available:</strong>{' '}
                  {space.availability ? 'Yes' : 'No'}
                </p>
              </div>
            ))
          )}
        </>
      )}
    </div>
  );
}

export default App;
