import { useEffect, useState } from "react";
import {
  GetAllStationLookup,
  GetClientInfo,
  GetGoogleMapRestrictedCountry,
  GetNextEmployeeUserName,
} from "../../api/AxiosInterceptors";
import { fetchMethod } from "../../utilities/helpers/Helpers";
import { getThisKeyCookie, setThisKeyCookie } from "../../utilities/cookies";
import { EnumCookieKeys } from "../../utilities/enum";

export default function useGetClientInfo() {
  const [restrictedCountry, setRestrictedCountry] = useState(["ae"]);
  const [clinetCountry, setClientCountry] = useState();
  const [loading, setLoading] = useState(false);
  // handleGetAllCities
  const handleGetRestrictedCountry = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() =>
      GetGoogleMapRestrictedCountry()
    );
    if (response.isSuccess) {
      setThisKeyCookie(
        EnumCookieKeys.RESTRICTED_COUNTRY,
        JSON.stringify(response.result.data)
      );
      setRestrictedCountry(response.result.data);
    }
    setLoading(false);
  };
  const handleGetClientInfo = async () => {
    setLoading(true);
    const { response } = await fetchMethod(() => GetClientInfo());
    if (response.isSuccess) {
      setThisKeyCookie(
        EnumCookieKeys.COUNTRY,
        JSON.stringify(response.result.data)
      );
      setClientCountry(response.result.data);
    }
    setLoading(false);
  };
  useEffect(() => {
    if (getThisKeyCookie(EnumCookieKeys.COUNTRY) == "undefined") {
      handleGetClientInfo();
    } else {
      if (getThisKeyCookie(EnumCookieKeys.COUNTRY) != "undefined") {
        const data = JSON.parse(getThisKeyCookie(EnumCookieKeys.COUNTRY));
        setClientCountry(data);
      }
    }
  }, []);
  useEffect(() => {
    if (getThisKeyCookie(EnumCookieKeys.RESTRICTED_COUNTRY) == "undefined") {
      handleGetRestrictedCountry();
    } else {
      if (getThisKeyCookie(EnumCookieKeys.RESTRICTED_COUNTRY) != "undefined") {
        const data = JSON.parse(
          getThisKeyCookie(EnumCookieKeys.RESTRICTED_COUNTRY)
        );
        setRestrictedCountry(data);
      }
    }
  }, []);
  return { loading, restrictedCountry, clinetCountry };
}
