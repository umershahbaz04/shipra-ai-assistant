import {
  GoogleMap,
  Marker,
  DirectionsRenderer,
  useJsApiLoader,
} from "@react-google-maps/api";
import React, { useState, useEffect, useMemo } from "react";
import "./style.css";
import { useGetMapApiKeyReducer } from "../../../utilities/helpers/Helpers";

const center = {
  lat: 25.2,
  lng: 55.4,
};

export default function MapComponent({ locations }) {
  const [map, setMap] = useState(null);
  const [directions, setDirections] = useState(null);
  const mapApiKey = useGetMapApiKeyReducer();

  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: mapApiKey ?? process.env.REACT_APP_GOOGLE_API_KEY,
    libraries: ["places"],
  });

  const filteredLocations = useMemo(
    () => locations.filter((location) => location.isValid),
    [locations],
  );

  useEffect(() => {
    if (filteredLocations.length >= 2 && window.google) {
      const origin = {
        lat: filteredLocations[0].lat,
        lng: filteredLocations[0].lng,
      };
      const destination = {
        lat: filteredLocations[filteredLocations.length - 1].lat,
        lng: filteredLocations[filteredLocations.length - 1].lng,
      };
      const waypoints = filteredLocations.slice(1, -1).map((loc) => ({
        location: { lat: loc.lat, lng: loc.lng },
        stopover: true,
      }));

      const directionsService = new window.google.maps.DirectionsService();
      directionsService.route(
        {
          origin,
          destination,
          waypoints,
          travelMode: window.google.maps.TravelMode.DRIVING,
        },
        (result, status) => {
          if (status === "OK") {
            setDirections(result);
          } else {
            console.error("Directions request failed due to " + status);
          }
        },
      );
    }
  }, [filteredLocations]);

  const mapOptions = {
    styles: [
      {
        featureType: "poi",
        elementType: "labels",
        stylers: [{ visibility: "off" }],
      },
    ],
  };

  return (
    <>
      {isLoaded && filteredLocations.length > 0 && (
        <GoogleMap
          mapContainerStyle={{ width: "100%", height: "70vh" }}
          center={center}
          zoom={5}
          onLoad={(map) => setMap(map)}
          mapTypeId="roadmap"
          options={mapOptions}
        >
          {/* Numbered markers */}
          {filteredLocations.map((loc, index) => (
            <Marker
              key={loc.orderNo}
              position={{ lat: loc.lat, lng: loc.lng }}
              icon={{
                url: `https://chart.googleapis.com/chart?chst=d_map_pin_letter&chld=${
                  index + 1
                }|B30000|FFFFFF`,
                scaledSize: new window.google.maps.Size(40, 40),
              }}
              label={{
                className: "icon",
                text: `${index + 1}`,
                color: "#fff",
                fontWeight: "bold",
              }}
            />
          ))}

          {directions && (
            <DirectionsRenderer
              directions={directions}
              options={{
                polylineOptions: {
                  strokeColor: "#B30000",
                  strokeOpacity: 1.0,
                  strokeWeight: 5,
                },
              }}
            />
          )}
        </GoogleMap>
      )}
    </>
  );
}
