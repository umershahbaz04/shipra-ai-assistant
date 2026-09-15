import { useState } from "react";

const useGoogleMapGeocodeGetLatLng = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const getLatLng = (address) => {
    setIsLoading(true);
    return new Promise((resolve, reject) => {
      const geocoder = new window.google.maps.Geocoder();
      geocoder.geocode({ address: address }, (results, status) => {
        setIsLoading(false);
        if (status === "OK") {
          const lat = results[0].geometry.location.lat();
          const lng = results[0].geometry.location.lng();
          resolve({ lat, lng });
        } else {
          setError(new Error("Error getting latitude and longitude"));
          reject(new Error("Error getting latitude and longitude"));
        }
      });
    });
  };

  return { getLatLng, isLoading, error };
};

export default useGoogleMapGeocodeGetLatLng;
