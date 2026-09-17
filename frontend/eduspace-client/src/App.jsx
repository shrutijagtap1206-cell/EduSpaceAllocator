import { useEffect, useState } from 'react';
import { getSpaces } from './services/spaceService';
import GISMap from './components/GIS/GISMap';
import Clustering from './components/Clustering/Clustering';

function App() {
  const [spaces, setSpaces] = useState([]);
  const [showClustering, setShowClustering] = useState(false);

  useEffect(() => {
    getSpaces()
      .then(setSpaces)
      .catch(console.error);
  }, []);

  return (
    <div style={{ padding: '24px', fontFamily: 'Arial, sans-serif' }}>
      <h1>EduSpace Allocator</h1>

      <div style={{ marginBottom: '20px' }}>
        <button
          onClick={() => setShowClustering(false)}
          style={{ marginRight: '10px' }}
        >
          GIS Mapping
        </button>

        <button onClick={() => setShowClustering(true)}>
          K-Means Clustering
        </button>
      </div>

      {showClustering ? (
        <Clustering />
      ) : (
        <>
          <h2>GIS Mapping</h2>

          <div
            style={{
              background: '#f5f5f5',
              padding: '12px 16px',
              borderRadius: '8px',
              marginBottom: '16px'
            }}
          >
            <strong>Legend:</strong> ?? Available Learning Spaces
          </div>

          <GISMap spaces={spaces} />

          <h2 style={{ marginTop: '24px' }}>
            Available Learning Spaces
          </h2>

          {spaces.map((space) => (
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
              <p><strong>Capacity:</strong> {space.capacity}</p>
              <p><strong>Floor Area:</strong> {space.floorArea} sq. ft.</p>
              <p><strong>Rental Cost:</strong> Rs. {space.rentalCost}</p>
            </div>
          ))}
        </>
      )}
    </div>
  );
}

export default App;
