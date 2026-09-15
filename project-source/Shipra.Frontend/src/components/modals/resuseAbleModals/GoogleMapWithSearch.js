import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import {
  Autocomplete,
  GoogleMap,
  MarkerF,
  useJsApiLoader,
} from "@react-google-maps/api";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import "../../../assets/styles/googleMapStyle.css";
import { styleSheet } from "../../../assets/styles/style";
import {
  BackdropCustom,
  placeholders,
  purple,
  useGeolocation,
  useGetMapApiKeyReducer,
} from "../../../utilities/helpers/Helpers";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import useGetClientInfo from "../../../.reUseableComponents/CustomHooks/useGetClientInfo";
import { useGetAddressSchema } from "../../../utilities/helpers/addressSchema";
const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function GoogleMapWithSearch(props) {
  const {
    open,
    setOpen,
    isEditMode = false,
    handleMapValues = () => {},
    setAutocomplete,
    setValue,
    splitLatAndLong,
  } = props;
  const mapApiKey = useGetMapApiKeyReducer();
  const {
    selectedAddressSchema,
    addressSchemaSelectData,
    addressSchemaInputData,
    handleSetSchema,
    handleChangeInputAddressSchema,
    handleChangeSelectAddressSchemaAndGetOptions,
  } = useGetAddressSchema();

  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: mapApiKey ?? process.env.REACT_APP_GOOGLE_API_KEY,
    libraries: ["places"],
  });
  const [searchResult, setSearchResult] = useState("Result: none");
  const [map, setMap] = useState(/** @type google.maps.Map */ (null));
  const [center, setCenter] = useState({ lat: 48.8584, lng: 2.2945 });
  const [markerPosition, setMarkerPosition] = useState(null);
  const [autocompleteInput, setAutocompleteInput] = useState("");
  const { autocomplete, error, handleValues } = useGeolocation();
  const {
    loading: loadingClientInfo,
    restrictedCountry,
    clinetCountry,
  } = useGetClientInfo();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    setAutocompleteInput("");
    setOpen(false);
  };
  const handleSubmit = () => {
    let lat = (markerPosition?.lat ? markerPosition?.lat?.toFixed(6) : 0) || 0;
    let lng = (markerPosition?.lng ? markerPosition?.lng?.toFixed(6) : 0) || 0;

    let fixedLatLng = {
      lat: lat,
      lng: lng,
    };
    splitLatAndLong();
    setOpen(false);
    handleValues(fixedLatLng);
  };
  useEffect(() => {
    setAutocomplete(autocomplete);
  }, [autocomplete]);
  //set country coordinate center
  useEffect(() => {
    if (clinetCountry && clinetCountry.latitude && clinetCountry.longitude) {
      setCenter({ lat: clinetCountry.latitude, lng: clinetCountry.longitude });
      setMarkerPosition({
        lat: clinetCountry.latitude,
        lng: clinetCountry.longitude,
      });
    }
  }, [clinetCountry]);

  const [autocompleteBounds, setAutocompleteBounds] = useState(null);

  function onLoad(autocomplete) {
    setSearchResult(autocomplete);
  }
  function onPlaceChanged() {
    if (searchResult != null) {
      const place = searchResult.getPlace();

      if (!place.geometry || !place.geometry.location) {
        // User entered the name of a Place that was not suggested and
        // pressed the Enter key, or the Place Details request failed.
        window.alert("No details available for input: '" + place.name + "'");
        return;
      }

      const formattedAddress = place.formatted_address;
      const { lat, lng } = place.geometry.location;

      setMarkerPosition({ lat: lat(), lng: lng() });
      map.panTo({ lat: lat(), lng: lng() });
    } else {
      alert("Please enter text");
    }
  }
  const getAddressFromLatLng = () => {
    if (map) {
      const geocoder = new window.google.maps.Geocoder();
      geocoder.geocode({ location: markerPosition }, (results, status) => {
        if (status === "OK" && results[0]) {
          setAutocompleteInput(results[0].formatted_address);
        } else {
          // setSearchResult('Address not found');
        }
      });
    }
  };
  const handleMarkerDragEnd = (e) => {
    const { latLng } = e;
    const lat = latLng.lat();
    const lng = latLng.lng();
    setMarkerPosition({ lat, lng });
  };
  useEffect(() => {
    if (markerPosition) {
      getAddressFromLatLng();
      if (!isEditMode) {
        const lat = markerPosition.lat
          ? markerPosition.lat.toFixed(6)
          : "0.000000";
        const lng = markerPosition.lng
          ? markerPosition.lng.toFixed(6)
          : "0.000000";
        if (open) {
          setValue("lat&long", `${lat},${lng}`);
          setValue("latitude", `${lat}`);
          setValue("longitude", `${lng}`);
        }
      }
    }
  }, [markerPosition]);
  useEffect(() => {
    if (navigator && navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (pos) => {},
        (error) => {
          console.error("Error Code = " + error.code + " - " + error.message);
          switch (error.code) {
            case error.PERMISSION_DENIED:
              console.error("User denied the request for Geolocation.");
              break;
            case error.POSITION_UNAVAILABLE:
              console.error("Location information is unavailable.");
              break;
            case error.TIMEOUT:
              console.error("The request to get user location timed out.");
              break;
            case error.UNKNOWN_ERROR:
              console.error("An unknown error occurred.");
              break;
          }

          getAddressFromLatLng();
        },
      );
    }
  }, []);

  const handleFocus = (event) => event.target.select();
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
          title={"Add Location"}
          bg={purple}
          onClick={handleSubmit}
        />
      }
    >
      {isLoaded ? (
        <>
          <Autocomplete
            id="autocomplete"
            onPlaceChanged={onPlaceChanged}
            onLoad={onLoad}
            bounds={autocompleteBounds}
            restrictions={{
              country: restrictedCountry,
            }}
            options={{
              types: ["geocode"],
              strictBounds: true,
            }}
          >
            <TextField
              placeholder={placeholders.searchMap}
              type="text"
              size="small"
              id="autocomplete"
              name="autocomplete"
              fullWidth
              variant="outlined"
              value={autocompleteInput}
              onChange={(e) => setAutocompleteInput(e.target.value)}
            />
          </Autocomplete>
          <br />
          <GoogleMap
            mapContainerStyle={{ width: "100%", height: "60vh" }}
            center={{
              lat: markerPosition ? markerPosition.lat : 0,
              lng: markerPosition ? markerPosition.lng : 0,
            }} // Initial center coordinates
            zoom={5} // Initial zoom level
            onClick={(e) => {
              setMarkerPosition({
                lat: e.latLng.lat(),
                lng: e.latLng.lng(),
              });
            }}
            onLoad={(map) => setMap(map)}
          >
            {markerPosition && (
              <MarkerF
                position={{
                  lat: markerPosition.lat,
                  lng: markerPosition.lng,
                }}
                draggable={true}
                onDragEnd={handleMarkerDragEnd}
              />
            )}
          </GoogleMap>
        </>
      ) : (
        <BackdropCustom open={isLoaded} />
      )}
    </ModalComponent>
  );
}
export default GoogleMapWithSearch;
