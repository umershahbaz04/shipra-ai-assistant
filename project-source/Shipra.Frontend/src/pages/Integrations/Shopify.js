import React, { useState } from "react";
import {
  GoogleMap,
  useJsApiLoader,
  Marker,
  MarkerF,
  Autocomplete,
} from "@react-google-maps/api";
import { Grid, TextField } from "@mui/material";
import { placeholders, useGetMapApiKeyReducer } from "../../utilities/helpers/Helpers";
function Shopify() {
  const mapApiKey = useGetMapApiKeyReducer();
  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: mapApiKey ?? process.env.REACT_APP_GOOGLE_API_KEY,
    libraries: ["places"],
  });
  const [map, setMap] = useState(/** @type google.maps.Map */ (null));
  const [center, setCenter] = useState({ lat: 48.8584, lng: 2.2945 });
  const [markerPosition, setMarkerPosition] = useState({
    lat: 48.8584,
    lng: 2.2945,
  });
  const [searchResult, setSearchResult] = useState("Result: none");

  function onLoad(autocomplete) {
    setSearchResult(autocomplete);
  }
  function onPlaceChanged() {
    if (searchResult != null) {
      const place = searchResult.getPlace();
      const formattedAddress = place.formatted_address;
      const { lat, lng } = place.geometry.location;
      console.log("Latitude:", lat());
      console.log("Longitude:", lng());
      setMarkerPosition({ lat: lat(), lng: lng() });
      map.panTo({ lat: lat(), lng: lng() });
      console.log(`Formatted Address: ${formattedAddress}`);
    } else {
      alert("Please enter text");
    }
  }
  const handleMarkerDragEnd = (e) => {
    const { latLng } = e;
    const lat = latLng.lat();
    const lng = latLng.lng();
    setMarkerPosition({ lat, lng });
  };

  if (!isLoaded) {
    return "isLoading";
  }
  return (
    <Grid sx={{ height: "500px", width: "600px", margin: "30px auto" }}>
      <Autocomplete onPlaceChanged={onPlaceChanged} onLoad={onLoad}>
        <TextField
          placeholder={placeholders.url}
          type="text"
          size="small"
          id="autocomplete"
          name="autocomplete"
          fullWidth
          variant="outlined"
        />
      </Autocomplete>

      <GoogleMap
        mapContainerStyle={{ width: "100%", height: "100%" }}
        center={center} // Initial center coordinates
        zoom={15} // Initial zoom level
        onClick={(e) => {
          setMarkerPosition({ lat: e.latLng.lat(), lng: e.latLng.lng() });
        }}
        onLoad={(map) => setMap(map)}
      >
        <MarkerF
          position={{ lat: markerPosition.lat, lng: markerPosition.lng }}
          draggable={true}
          onDragEnd={handleMarkerDragEnd}
        />
      </GoogleMap>
    </Grid>
  );
}

export default Shopify;
