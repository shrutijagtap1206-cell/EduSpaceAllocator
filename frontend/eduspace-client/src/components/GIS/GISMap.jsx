import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';

function GISMap({ spaces = [] }) {
  const defaultCenter = [18.5204, 73.8567];

  return (
    <MapContainer
      center={defaultCenter}
      zoom={12}
      style={{ height: '600px', width: '100%' }}
    >
      <TileLayer
        attribution='&copy; OpenStreetMap contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />

      {spaces.map(space => (
        <Marker
          key={space.spaceId}
          position={[space.latitude, space.longitude]}
        >
          <Popup>
            <strong>{space.buildingName}</strong>
            <br />
            {space.address}
            <br />
            Capacity: {space.capacity}
          </Popup>
        </Marker>
      ))}
    </MapContainer>
  );
}

export default GISMap;
