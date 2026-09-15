import { Box } from "@mui/material";
import { GoogleMap, MarkerF, Polygon, OverlayView } from "@react-google-maps/api";
import React, {
  forwardRef,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react";
import { fetchMethod, useCustomJsApiLoader } from "../../../utilities/helpers/Helpers";
import { GetAllZonesWithCoords } from "../../../api/AxiosInterceptors";
import ZoneMapComponent from "../../../.reUseableComponents/Map/ZoneMapComponent";

const getPolygonCentroid = (coords) => {
  if (!coords || coords.length === 0) return { lat: 0, lng: 0 };
  let latSum = 0;
  let lngSum = 0;
  coords.forEach((pt) => {
    latSum += pt.lat;
    lngSum += pt.lng;
  });
  return { lat: latSum / coords.length, lng: lngSum / coords.length };
};

function DeliveryTaskMapComponent(
  {
    setSelectedDeliveryTasks,
    allDeliveryTask,
    setAllDeliveryTask,
    mapChildRef,
    defaultCard,
  },
  ref,
) {
  const { isLoaded } = useCustomJsApiLoader();
  const [markers, setMarkers] = useState([]);
  const [zones, setZones] = useState([]);

  const [center, setCenter] = useState({ lat: 0, lng: 0 });

  const [selectedMarkers, setSelectedMarkers] = useState([]);
  const [map, setMap] = useState(/** @type google.maps.Map */ (null));

  // Fetch Zones with Polygon Coords
  useEffect(() => {
    const fetchZones = async () => {
      try {
        const { response } = await fetchMethod(() => GetAllZonesWithCoords());
        if (response?.isSuccess || response?.result || response?.Result) {
          const rawZones =
            response?.result?.zones ||
            response?.result?.Zones ||
            response?.zones ||
            response?.Zones ||
            (Array.isArray(response?.result) ? response.result : []);
          const zoneList = rawZones.map((z) => {
            let parsedCoords = [];
            try {
              parsedCoords =
                typeof z.coords === "string"
                  ? JSON.parse(z.coords)
                  : z.coords || z.Coords || [];
            } catch (e) {}
            return { ...z, parsedCoords };
          });
          setZones(zoneList);
        }
      } catch (err) {
        console.error("Error fetching zones in delivery tasks map:", err);
      }
    };
    fetchZones();
  }, []);

  // Expose the resetState method via ref
  // Method to reset the state
  const resetState = () => {
    setSelectedMarkers([]); // Reset the count state
  };
  console.log(allDeliveryTask?.list);
  useEffect(() => {
    const allDTask = allDeliveryTask?.list;
    if (allDTask) {
      const filtredData = allDTask.filter(
        (x) => x.IsValidLatLng == 1 && x?.Latitude != 0 && x?.Longitude != 0,
      );
      if (filtredData && filtredData.length > 0) {
        setMarkers(filtredData);
        setCenter({
          lat: filtredData.length > 0 ? filtredData[0].Latitude : 0,
          lng: filtredData.length > 0 ? filtredData[0].Longitude : 0,
        });
      } else {
        setMarkers([]);
        setCenter({ lat: 0, lng: 0 });
      }
    }
  }, [allDeliveryTask]);
  const handleMarkerListClick = (marker) => {
    const markerIndex = selectedMarkers.findIndex(
      (selectedMarker) => selectedMarker.OrderNo === marker.OrderNo,
    );
    if (markerIndex === -1) {
      setSelectedMarkers([...selectedMarkers, marker]);
    } else {
      const filteredMarkers = selectedMarkers.filter(
        (selectedMarker) => selectedMarker.OrderNo !== marker.OrderNo,
      );
      setSelectedMarkers(filteredMarkers);
    }
  };
  useEffect(() => {
    const orderNos = selectedMarkers.map((item) => item.OrderNo);

    setSelectedDeliveryTasks(orderNos);
  }, [selectedMarkers]);

  const getMarkerIconUrl = (status, isSelected) => {
    let color = "";
    if (isSelected) color = "blue";
    else if (status === "Completed") color = "green";
    else if (status === "Pending") color = "yellow";
    else if (status === "Started") color = "purple";
    else color = "red";

    return `http://maps.google.com/mapfiles/ms/icons/${color}-dot.png`;
  };
  useImperativeHandle(mapChildRef, () => ({
    Rest() {
      resetState();
    },
  }));
  const mapOptions = {
    styles: [
      {
        featureType: "poi",
        elementType: "labels",
        stylers: [{ visibility: "off" }],
      },
      // Add more style rules as needed
    ],
  };
  return (
    <div id="wrapper" style={{ position: "relative" }}>
      {markers.length == 0 && <h3>No valid order address found.</h3>}
      <div style={{ height: "70vh", width: "100%" }}>
        {isLoaded ? (
          markers?.length > 0 ? (
            <>
              <ZoneMapComponent
                zones={zones}
                readOnly={true}
                mapContainerStyle={{ width: "100%", height: "70vh" }}
                center={center}
                zoom={5}
                onMapLoad={(map) => setMap(map)}
                mapOptions={mapOptions}
              >
                {markers?.map((marker, index) => {
                  return marker?.IsValidLatLng == 1 ? (
                    <MarkerF
                      key={marker.DeliveryTaskId}
                      position={{ lat: marker.Latitude, lng: marker.Longitude }}
                      options={{
                        icon: {
                          url: getMarkerIconUrl(
                            marker.DeliveryTaskStatus,
                            selectedMarkers.includes(marker),
                          ),
                          scaledSize: new window.google.maps.Size(45, 45),
                        },
                        animation: window.google.maps.Animation.DROP, // Add animation here
                      }}
                      onClick={() => handleMarkerListClick(marker)}
                    />
                  ) : null;
                })}
              </ZoneMapComponent>

              <div
                id="over_map"
                style={{
                  position: "absolute",
                  top: 10,
                  zIndex: 0,
                  // background: "#ffff
                  maxHeight: "60vh",
                  overflowY: "auto",
                  padding: "0px 10px",
                  scrollbarWidth: "none", // For Firefox
                  "-ms-overflow-style": "none", // For IE and Edge
                  "&::-webkit-scrollbar": {
                    display: "none", // Hide the scrollbar
                  },
                }}
              >
                <Box>
                  <Box
                    style={{
                      background: "#ffff",
                      padding: "0px 10px 5px 10px",
                      marginBottom: "5px",
                    }}
                  >
                    <Box>Total: {defaultCard.total}</Box>
                    <Box>Selected: {selectedMarkers.length}</Box>
                    <Box>
                      Assigned Shipments: {defaultCard.assignedShipment}
                    </Box>
                    <Box>UnAssigned Shipments: {defaultCard.unassigned}</Box>
                  </Box>
                  {selectedMarkers.map((marker) => (
                    <Box
                      key={marker.DeliveryTaskId}
                      onClick={() => handleMarkerListClick(marker)}
                      style={{
                        background: "#2fffa1",
                        padding: "0px 10px 5px 10px",
                        marginBottom: "5px",
                      }}
                    >
                      <Box>Store Name: {marker.StoreName}</Box>
                      <Box>Store Contact: {marker.CustomerServiceNo}</Box>
                      <Box>Order No: {marker.OrderNo}</Box>
                      <Box>Status: {marker.DeliveryTaskStatus}</Box>
                      <Box>Address: {marker.CustomerFullAddress}</Box>
                      <Box>Region: {marker.RegionName}</Box>
                    </Box>
                  ))}
                </Box>
              </div>
              {/* <div>
              {selectedMarkers.map((marker) => (
                <div
                  key={marker.DeliveryTaskId}
                  onClick={() => handleMarkerListClick(marker)}
                >
                  <p>Order No: {marker.OrderNo}</p>
                  <p>Status: {marker.DeliveryTaskStatus}</p>
                  <p>Latitude: {marker.Latitude}</p>
                  <p>Longitude: {marker.Longitude}</p>
                </div>
              ))}
            </div> */}
            </>
          ) : (
            ""
          )
        ) : (
          "Loading..."
        )}
      </div>
    </div>
  );
}

export default forwardRef(DeliveryTaskMapComponent);
