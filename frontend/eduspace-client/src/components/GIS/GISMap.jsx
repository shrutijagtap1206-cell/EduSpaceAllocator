import { useEffect, useMemo, useState } from 'react';
import {
  MapContainer,
  TileLayer,
  Marker,
  Popup,
  Polyline,
  CircleMarker,
  Tooltip,
  LayersControl,
  LayerGroup
} from 'react-leaflet';

import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

import api from '../../services/api';

const spaceIcon = new L.Icon({
  iconUrl:
    'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34]
});

function GISMap() {
  const [spaces, setSpaces] = useState([]);
  const [requests, setRequests] = useState([]);
  const [allocations, setAllocations] = useState([]);
  const [showConnections, setShowConnections] = useState(true);

  useEffect(() => {
    Promise.all([
      api.get('/Spaces'),
      api.get('/LearningRequests'),
      api.get('/Allocations')
    ])
      .then(([spacesResponse, requestsResponse, allocationsResponse]) => {
        setSpaces(spacesResponse.data || []);
        setRequests(requestsResponse.data || []);
        setAllocations(allocationsResponse.data || []);
      })
      .catch(console.error);
  }, []);

  const spaceLookup = useMemo(
    () =>
      Object.fromEntries(
        spaces.map((space) => [space.spaceId, space])
      ),
    [spaces]
  );

  const requestLookup = useMemo(
    () =>
      Object.fromEntries(
        requests.map((request) => [request.requestId, request])
      ),
    [requests]
  );

  const validSpaces = spaces.filter(
    (space) =>
      Number.isFinite(Number(space.latitude)) &&
      Number.isFinite(Number(space.longitude))
  );

  const validRequests = requests.filter(
    (request) =>
      Number.isFinite(Number(request.preferredLatitude)) &&
      Number.isFinite(Number(request.preferredLongitude))
  );

  const center =
    validSpaces.length > 0
      ? [
          Number(validSpaces[0].latitude),
          Number(validSpaces[0].longitude)
        ]
      : [18.5204, 73.8567];

  const validConnections = allocations.filter((allocation) => {
    const space = spaceLookup[allocation.spaceId];
    const request = requestLookup[allocation.requestId];

    return (
      space &&
      request &&
      Number.isFinite(Number(space.latitude)) &&
      Number.isFinite(Number(space.longitude)) &&
      Number.isFinite(Number(request.preferredLatitude)) &&
      Number.isFinite(Number(request.preferredLongitude))
    );
  });

  return (
    <div>
      <div className="map-toolbar">
        <label className="map-toggle">
          <input
            type="checkbox"
            checked={showConnections}
            onChange={(event) =>
              setShowConnections(event.target.checked)
            }
          />
          Show allocation connections
        </label>
      </div>

      <div className="map-container">
        <MapContainer
          center={center}
          zoom={12}
          style={{ height: '600px', width: '100%' }}
        >
          <TileLayer
            attribution="&copy; OpenStreetMap contributors"
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />

          <LayersControl position="topright">
            <LayersControl.Overlay checked name="Learning Spaces">
              <LayerGroup>
                {validSpaces.map((space) => (
                  <Marker
                    key={`space-${space.spaceId}`}
                    position={[
                      Number(space.latitude),
                      Number(space.longitude)
                    ]}
                    icon={spaceIcon}
                  >
                    <Popup>
                      <strong>{space.buildingName}</strong>
                      <br />
                      {space.address}
                      <br />
                      Capacity: {space.capacity}
                      <br />
                      Area: {space.floorArea} sq. ft.
                      <br />
                      Rent: ₹{Number(space.rentalCost).toLocaleString()}
                      <br />
                      Available:{' '}
                      {space.availability ? 'Yes' : 'No'}
                    </Popup>
                  </Marker>
                ))}
              </LayerGroup>
            </LayersControl.Overlay>

            <LayersControl.Overlay checked name="Learning Demand">
              <LayerGroup>
                {validRequests.map((request) => (
                  <CircleMarker
                    key={`request-${request.requestId}`}
                    center={[
                      Number(request.preferredLatitude),
                      Number(request.preferredLongitude)
                    ]}
                    radius={8}
                  >
                    <Tooltip>
                      {request.organization} — {request.programType}
                    </Tooltip>

                    <Popup>
                      <strong>{request.organization}</strong>
                      <br />
                      Request ID: {request.requestId}
                      <br />
                      Program: {request.programType}
                      <br />
                      Students: {request.studentCapacity}
                      <br />
                      Budget: ₹
                      {Number(request.budget).toLocaleString()}
                      <br />
                      Location: {request.preferredLocation}
                    </Popup>
                  </CircleMarker>
                ))}
              </LayerGroup>
            </LayersControl.Overlay>

            {showConnections && (
              <LayersControl.Overlay
                checked
                name="Smart Allocations"
              >
                <LayerGroup>
                  {validConnections.map((allocation) => {
                    const space =
                      spaceLookup[allocation.spaceId];
                    const request =
                      requestLookup[allocation.requestId];

                    return (
                      <Polyline
                        key={`allocation-${allocation.allocationId}`}
                        positions={[
                          [
                            Number(space.latitude),
                            Number(space.longitude)
                          ],
                          [
                            Number(request.preferredLatitude),
                            Number(request.preferredLongitude)
                          ]
                        ]}
                        pathOptions={{ weight: 3 }}
                      >
                        <Tooltip sticky>
                          {allocation.matchId} —{' '}
                          {allocation.distanceKm} km
                        </Tooltip>

                        <Popup>
                          <strong>{allocation.matchId}</strong>
                          <br />
                          Space: {space.buildingName}
                          <br />
                          Request: {request.organization}
                          <br />
                          Students: {allocation.studentsServed}
                          <br />
                          Distance: {allocation.distanceKm} km
                          <br />
                          Capacity utilization:{' '}
                          {(
                            allocation.capacityUtilization * 100
                          ).toFixed(1)}
                          %
                          <br />
                          Optimization:{' '}
                          {(
                            allocation.optimizationScore * 100
                          ).toFixed(1)}
                          %
                        </Popup>
                      </Polyline>
                    );
                  })}
                </LayerGroup>
              </LayersControl.Overlay>
            )}
          </LayersControl>
        </MapContainer>
      </div>
    </div>
  );
}

export default GISMap;
