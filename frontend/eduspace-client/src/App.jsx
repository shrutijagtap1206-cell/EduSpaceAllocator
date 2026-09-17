import { useEffect, useState } from 'react';
import { getSpaces } from './services/spaceService';

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
    <div style={{ padding: '2rem', fontFamily: 'Arial, sans-serif' }}>
      <h1>EduSpace Allocator</h1>
      <h2>Available Learning Spaces</h2>

      {loading && <p>Loading spaces...</p>}

      {error && <p>{error}</p>}

      {!loading && !error && spaces.length === 0 && (
        <p>No spaces found in the database.</p>
      )}

      {!loading && !error && spaces.length > 0 && (
        <div>
          {spaces.map(space => (
            <div
              key={space.spaceId}
              style={{
                border: '1px solid #ccc',
                padding: '1rem',
                marginBottom: '1rem',
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
          ))}
        </div>
      )}
    </div>
  );
}

export default App;

