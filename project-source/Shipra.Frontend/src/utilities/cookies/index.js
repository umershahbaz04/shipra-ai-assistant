import Cookies from "js-cookie";

/* ------------------------------  SET COOKIE DATA FUNCTION  -------------------------------- */
export const setThisKeyCookie = (key, value) => {
  Cookies.set(key, value);
};

/* ------------------------------  GET COOKIE DATA FUNCTION  -------------------------------- */
export const getThisKeyCookie = (key) => {
  return Cookies.get(key);
};

/* ------------------------------  REMOVE COOKIE DATA FUNCTION  -------------------------------- */
export const removeThisKeyCookie = (key) => {
  return Cookies.remove(key);
};

/* ------------------------------  SET POLICY COOKIE DATA FUNCTION  -------------------------------- */
export const setPolicyCookie = () => {
  let expireDate = new Date();
  expireDate.setFullYear(expireDate.getFullYear(), expireDate.getMonth() + 6);
  Cookies.set("cookiepolicy", "true", expireDate);
};

/* ------------------------------  GET POLICY COOKIE DATA FUNCTION  -------------------------------- */
export const getPolicyCookie = () => {
  return Cookies.get("cookiepolicy");
};

/* ------------------------------  SET GENERIC CHECKBOX SETTINGS COOKIES FUNCTION  -------------------------------- */
export const setGenericCheckboxCookies = (configSettings) => {

  if (!configSettings) return;

  let settings = configSettings;
  if (typeof configSettings === "string") {
    try {
      settings = JSON.parse(configSettings);
    } catch (e) {
      return;
    }
  }

  if (!Array.isArray(settings)) return;

  settings.forEach((section) => {
    if (section?.inputData && Array.isArray(section.inputData)) {
      section.inputData.forEach((item) => {
        if (
          item?.type &&
          String(item.type).toLowerCase() === "checkbox" &&
          item?.key
        ) {
          const val = item.value === true || item.value === "true";
          setThisKeyCookie(item.key, val);
        }
      });
    }
  });
};

/* ------------------------------  GET SHOW COUNTRY IN TABBAR FLAG FUNCTION  -------------------------------- */
export const isShowCountryInTabbarFlag = () => {
  const val = getThisKeyCookie("showCountryInTabbar");
  return val === true || val === "true";
};

