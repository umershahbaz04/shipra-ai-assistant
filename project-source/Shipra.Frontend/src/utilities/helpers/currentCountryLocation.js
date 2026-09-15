import axios from "axios";
import React, { useEffect, useState } from "react";
const useCurrentCountryLocation = () => {
  const [data, setData] = useState(null);
  const [location, setLocation] = useState({ latitude: null, longitude: null });
  const [error, setError] = useState(null);

  const getCountryName = async (latitude, longitude) => {
    const url = `https://nominatim.openstreetmap.org/reverse?format=json&lat=${latitude}&lon=${longitude}`;
    try {
      const response = await axios.get(url);
      const data = response.data;

      if (data) {
        setData(data.address);
      } else {
        setError("Country not found");
      }
    } catch (err) {
      console.error("Error fetching country:", err);
      setError("Failed to fetch country data");
    }
  };

  const handleSuccess = (position) => {
    const latitude = position.coords.latitude;
    const longitude = position.coords.longitude;
    setLocation({ latitude, longitude });
    getCountryName(latitude, longitude);
  };

  const handleError = (error) => {
    console.log("Geolocation error:", error);
    setError("Unable to retrieve your location");
  };

  useEffect(() => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(handleSuccess, handleError);
    } else {
      setError("Geolocation not supported");
    }
  }, []);

  return { data, location, error };
};

export default useCurrentCountryLocation;
