import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import { GoogleMap, Marker, useJsApiLoader } from "@react-google-maps/api";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import { useGetMapApiKeyReducer } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function SelectLocationModal(props) {
  const { open, setOpen, setValue } = props;
  const [center, setCenter] = useState({ lat: -0.180653, lng: -78.467834 });
  const [markerPosition, setMarkerPosition] = useState(null);
  const mapApiKey = useGetMapApiKeyReducer();
  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: mapApiKey ?? process.env.REACT_APP_GOOGLE_API_KEY,
    libraries: ["places"],
  });
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    setOpen(false);
  };
  useEffect(() => {
    if (navigator && navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((pos) => {
        const coords = pos.coords;
        console.log("coords", coords);
        setValue("latitude", coords.latitude);
        setValue("longitude", coords.longitude);
        setCenter({
          lat: coords.latitude,
          lng: coords.longitude,
        });
        setMarkerPosition({
          lat: coords.latitude,
          lng: coords.longitude,
        });
      });
    }
  }, []);

  const handleMarkerDragEnd = (e) => {
    const { latLng } = e;
    const lat = latLng.lat();
    const lng = latLng.lng();
    setMarkerPosition({ lat, lng });
  };

  useEffect(() => {
    setMarkerPosition(center);
  }, []);
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={
        LanguageReducer?.languageType?.CURRENT_TEXT +
        " " +
        LanguageReducer?.languageType?.LOCATION_TEXT
      }
      actionBtn={
        <ModalButtonComponent
          title={"Select Location"}
          bg={purple}
          onClick={() => {
            setValue("latitude", markerPosition.lat);
            setValue("longitude", markerPosition.lng);
            handleClose();
          }}
        />
      }
    >
      <div style={{ width: "100%", height: "60vh" }}>
        {isLoaded && (
          <GoogleMap
            mapContainerStyle={{ width: "100%", height: "100%" }}
            center={{
              lat: markerPosition ? markerPosition.lat : 0,
              lng: markerPosition ? markerPosition.lng : 0,
            }} // Initial center coordinates
            zoom={15} // Initial zoom level
            onClick={(e) => {
              setMarkerPosition({ lat: e.latLng.lat(), lng: e.latLng.lng() });
            }}
          >
            {markerPosition && (
              <Marker
                position={{
                  lat: markerPosition.lat,
                  lng: markerPosition.lng,
                }}
                draggable={true}
                onDragEnd={handleMarkerDragEnd}
              />
            )}
          </GoogleMap>
        )}
      </div>
    </ModalComponent>
  );
}
export default SelectLocationModal;
