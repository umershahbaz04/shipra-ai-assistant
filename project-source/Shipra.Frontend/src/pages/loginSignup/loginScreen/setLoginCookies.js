import {
  setThisKeyCookie,
  setGenericCheckboxCookies,
  getThisKeyCookie,
  removeThisKeyCookie,
  isShowCountryInTabbarFlag,
} from "../../../utilities/cookies";
import { EnumCookieKeys } from "../../../utilities/enum";
import { GetGenericSetting } from "../../../api/AxiosInterceptors";

const setLoginCookies = async (res) => {
  const data = res.data.result;
  setThisKeyCookie("access_token", res.data.result.access_token);
  setThisKeyCookie("refresh_token", res.data.result.refresh_token);
  setThisKeyCookie("user_name", res.data.result.user_name);
  setThisKeyCookie("clIdentifier", res.data.result.clientPrefix);
  setThisKeyCookie("id_token", res.data.result.id_token);
  setThisKeyCookie("patronTypeId", res?.data?.result?.userRoleId);
  setThisKeyCookie(
    EnumCookieKeys.LOGGEDIN_CLIENT_COUNTRY_ID,
    data?.country?.countryId,
  );
  if (!isShowCountryInTabbarFlag()) {
    setThisKeyCookie(EnumCookieKeys.COUNTRY_ID, data?.country?.countryId);
  } else {
    removeThisKeyCookie(EnumCookieKeys.COUNTRY_ID);
  }
  setThisKeyCookie(EnumCookieKeys.COUNTRY_CODE, data?.country?.mapCountryCode);
  setThisKeyCookie(
    "allowPersonalCarrierContract",
    res?.data?.result?.allowPersonalCarrierContract,
  );
  setThisKeyCookie(
    "allowShipperInvocie",
    res?.data?.result?.allowShipperInvocie,
  );
  setThisKeyCookie("client_code", res?.data?.result?.client_code);
  setThisKeyCookie(
    "expires_in",
    res.data.result.expires_in * 1000 + Date.now(),
  );
  setThisKeyCookie("companyImage", res.data.result.companyImage);
  setThisKeyCookie("email", res.data.result.email);
  setThisKeyCookie(
    EnumCookieKeys.TIME_ZONE,
    data?.region ? data.region?.timeZone : "",
  );
  let country = "";
  if (data?.country) {
    country = JSON.stringify(data?.country);
    setThisKeyCookie(EnumCookieKeys.COUNTRY, country);
  }
  if (data?.restrictedCountry) {
    let restrictedCountry = JSON.stringify(data?.restrictedCountry);
    setThisKeyCookie(EnumCookieKeys.RESTRICTED_COUNTRY, restrictedCountry);
  }
  setThisKeyCookie(
    "isShowMetafield",
    res?.data?.result?.isShowMetafield || false,
  );

  try {
    const genericRes = await GetGenericSetting();
    if (genericRes?.data?.result) {
      const parsedData =
        typeof genericRes.data.result === "string"
          ? JSON.parse(genericRes.data.result)
          : genericRes.data.result;
      setGenericCheckboxCookies(parsedData);
    }
  } catch (e) {
    console.error("Error fetching generic settings during login:", e);
  }
};

export default setLoginCookies;
