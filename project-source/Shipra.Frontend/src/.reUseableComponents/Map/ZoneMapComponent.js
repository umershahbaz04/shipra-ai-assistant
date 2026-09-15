import React from "react";
import { Box, CircularProgress, Alert } from "@mui/material";
import { GoogleMap, Polygon, Marker, OverlayView, useJsApiLoader } from "@react-google-maps/api";
import { useGetMapApiKeyReducer } from "../../utilities/helpers/Helpers";

const DEFAULT_CENTER = { lat: 25.276987, lng: 55.296249 }; // UAE default coordinates

const getPolygonCentroid = (coords) => {
  if (!coords || coords.length === 0) return DEFAULT_CENTER;
  let latSum = 0;
  let lngSum = 0;
  coords.forEach((pt) => {
    latSum += pt.lat;
    lngSum += pt.lng;
  });
  return { lat: latSum / coords.length, lng: lngSum / coords.length };
};

const ZoneMapComponent = ({
  zones = [],
  selectedZone = null,
  setSelectedZone = () => {},
  isDrawingMode = false,
  drawnCoords = [],
  handleMapClick = () => {},
  mapContainerStyle = { width: "100%", height: "450px" },
  zoom = 6,
  center = null,
  onMapLoad = () => {},
  readOnly = false,
  mapOptions = {},
  loading = false,
  children,
}) => {
  const mapApiKey = useGetMapApiKeyReducer();
  const { isLoaded, loadError } = useJsApiLoader({
    googleMapsApiKey: mapApiKey ?? process.env.REACT_APP_GOOGLE_API_KEY,
    libraries: ["places"],
  });

  const computedCenter =
    center ||
    (zones.length > 0 && zones[0].parsedCoords?.length > 0
      ? getPolygonCentroid(zones[0].parsedCoords)
      : DEFAULT_CENTER);

  if (loadError) {
    return (
      <Alert severity="error">
        Failed to load Google Maps script. Please verify your map API key.
      </Alert>
    );
  }

  if (!isLoaded) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        style={{ ...mapContainerStyle }}
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box position="relative">
      {loading && (
        <Box
          sx={{
            position: "absolute",
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: "rgba(255, 255, 255, 0.6)",
            zIndex: 10,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <CircularProgress />
        </Box>
      )}

      <GoogleMap
        mapContainerStyle={mapContainerStyle}
        center={computedCenter}
        zoom={zoom}
        onLoad={onMapLoad}
        onClick={handleMapClick}
        options={{
          disableDefaultUI: false,
          zoomControl: true,
          streetViewControl: false,
          mapTypeControl: true,
          draggableCursor: isDrawingMode ? "crosshair" : null,
          ...mapOptions,
        }}
      >
        {/* Render Saved Zones as Polygons with Zone Name Text Labels */}
        {zones.map((zone) => {
          if (!zone.parsedCoords || zone.parsedCoords.length === 0) return null;
          const isSelected =
            !readOnly &&
            selectedZone &&
            (selectedZone.zoneId || selectedZone.ZoneId) ===
              (zone.zoneId || zone.ZoneId);
          const centroid = getPolygonCentroid(zone.parsedCoords);

          return (
            <React.Fragment key={zone.zoneId || zone.ZoneId}>
              <Polygon
                paths={zone.parsedCoords}
                onClick={() => !readOnly && setSelectedZone(zone)}
                onLoad={(polygon) => {
                  if (readOnly) return;
                  try {
                    const path = polygon.getPath();
                    const updatePath = () => {
                      const newCoords = [];
                      for (let i = 0; i < path.getLength(); i++) {
                        const pt = path.getAt(i);
                        newCoords.push({ lat: pt.lat(), lng: pt.lng() });
                      }
                      setSelectedZone((prev) => {
                        if (!prev) return prev;
                        const curId = prev.zoneId || prev.ZoneId;
                        const polyId = zone.zoneId || zone.ZoneId;
                        if (curId === polyId) {
                          return {
                            ...prev,
                            parsedCoords: newCoords,
                            editedCoords: newCoords,
                          };
                        }
                        return prev;
                      });
                    };
                    path.addListener("set_at", updatePath);
                    path.addListener("insert_at", updatePath);
                    path.addListener("remove_at", updatePath);
                  } catch (e) {
                    console.error("Error setting up polygon path listener:", e);
                  }
                }}
                options={{
                  fillColor: isSelected ? "#1976d2" : "#49a51a",
                  fillOpacity: isSelected ? 0.55 : 0.35,
                  strokeColor: isSelected ? "#1565c0" : "#49a51a",
                  strokeWeight: isSelected ? 3 : 1.5,
                  strokeOpacity: 0.8,
                  editable: !readOnly && isSelected,
                  clickable: !readOnly,
                }}
              />
              {/* Clean White Text Zone Name Overlay */}
              <OverlayView
                position={centroid}
                mapPaneName={OverlayView.OVERLAY_MOUSE_TARGET}
              >
                <div
                  onClick={() => !readOnly && setSelectedZone(zone)}
                  style={{
                    transform: "translate(-50%, -50%)",
                    color: "#ffffff",
                    fontWeight: "bold",
                    fontSize: "14px",
                    textShadow:
                      "0px 1px 4px rgba(0, 0, 0, 0.9), 0px 0px 2px rgba(0, 0, 0, 0.9)",
                    cursor: readOnly ? "default" : "pointer",
                    whiteSpace: "nowrap",
                    userSelect: "none",
                    pointerEvents: readOnly ? "none" : "auto",
                  }}
                >
                  {zone.name || zone.Name || "Zone"}
                </div>
              </OverlayView>
            </React.Fragment>
          );
        })}

        {/* Render points being drawn dynamically */}
        {isDrawingMode && drawnCoords && drawnCoords.length > 0 && (
          <>
            <Polygon
              paths={drawnCoords}
              options={{
                fillColor: "#49a51a",
                fillOpacity: 0.4,
                strokeColor: "#2e7d32",
                strokeWeight: 2.5,
              }}
            />
            {drawnCoords.map((pt, idx) => (
              <Marker
                key={idx}
                position={pt}
                icon={{
                  path: window.google?.maps?.SymbolPath?.CIRCLE || 0,
                  scale: 6,
                  fillColor: "#2e7d32",
                  fillOpacity: 1,
                  strokeColor: "#ffffff",
                  strokeWeight: 2,
                }}
              />
            ))}
          </>
        )}

        {/* Render Additional Custom Children (e.g. Delivery Task Markers) */}
        {children}
      </GoogleMap>
    </Box>
  );
};

export default ZoneMapComponent;
