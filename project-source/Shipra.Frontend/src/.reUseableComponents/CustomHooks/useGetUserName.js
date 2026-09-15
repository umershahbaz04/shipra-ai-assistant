import { useEffect, useState } from "react";
import {
  GetAllStationLookup,
  GetNextEmployeeUserName,
} from "../../api/AxiosInterceptors";
import { fetchMethod } from "../../utilities/helpers/Helpers";

export default function useGetUserName() {
  const [userName, setUserName] = useState("");
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleNextEmployeeUserName = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetNextEmployeeUserName());
    if (response.isSuccess) {
      setUserName(response.result.data);
    }
    setLoading(false);
  };
  useEffect(() => {
    handleNextEmployeeUserName();
  }, []);
  return { loading, userName };
}
