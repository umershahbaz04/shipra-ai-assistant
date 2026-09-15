import { useState } from "react";

const useOpenStreetmapGetLatLng = () => {
  const [isLoadingCoordinates, setIsLoadingCoordinates] = useState({});
  const [error, setError] = useState(null);

  const getAddressCoordinates = async (address) => {
    setIsLoadingCoordinates(true);
    // setIsLoadingCoordinates({ [row.orderNo]: true });
    if (address) {
      setError(null);

      const url = `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(
        address
      )}`;

      try {
        const response = await fetch(url);
        if (!response.ok) {
          throw new Error("Network response was not ok");
        }
        const data = await response.json();
        console.log("Data from API:", data); // Log data from the API
        if (data && data.length > 0) {
          const firstResult = data[0];
          console.log("First result:", firstResult); // Log the first result
          const latitude = parseFloat(firstResult.lat);
          const longitude = parseFloat(firstResult.lon);
          console.log("latitude:", latitude);
          console.log("longitude", longitude);
          setIsLoadingCoordinates(false);
          return { lat: latitude, lng: longitude };
        } else {
          throw new Error("No results found");
        }
      } catch (error) {
        setIsLoadingCoordinates(false);
        setError(`Error fetching address coordinates: ${error.message}`);
      }
    }
  };

  return { getAddressCoordinates, isLoadingCoordinates, error };
};

export default useOpenStreetmapGetLatLng;
