import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import SelectComponent from "../../.reUseableComponents/TextField/SelectComponent";
import { setCountriesList, updateSelectedCountry } from "../../redux/country";
import { setThisKeyCookie, removeThisKeyCookie, isShowCountryInTabbarFlag } from "../cookies";
import { EnumCookieKeys, EnumOptions } from "../enum";
import { handleGetAllCountries } from "./addressSchema";

export const CountrySchema = ({
  name = "country",
  value,
  onChange,
  height = 37,
  placeholder = "Select Country",
  disabled = false,
  required = false,
  addPleaseSelectOptionOnClear = true,
  ...restProps
}) => {
  const dispatch = useDispatch();
  const isShowCountryInTabbar = isShowCountryInTabbarFlag();
  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry,
  );
  const reduxCountries = useSelector(
    (state) => state.CountryReducer?.countries,
  );

  const [countriesList, setCountriesListState] = useState(reduxCountries || []);
  const [loading, setLoading] = useState(false);

  // Fetch countries if not available in Redux
  useEffect(() => {
    let isMounted = true;
    const fetchCountries = async () => {
      if (!reduxCountries || reduxCountries.length === 0) {
        setLoading(true);
        const data = await handleGetAllCountries();
        if (isMounted) {
          setCountriesListState(data || []);
          dispatch(setCountriesList(data || []));
          setLoading(false);
        }
      } else {
        setCountriesListState(reduxCountries);
      }
    };
    fetchCountries();
    return () => {
      isMounted = false;
    };
  }, [reduxCountries, dispatch]);

  // Determine current selected country object
  const matchedReduxCountry =
    countriesList.find(
      (c) =>
        c[EnumOptions.COUNTRY.VALUE] ===
        reduxSelectedCountry?.[EnumOptions.COUNTRY.VALUE],
    ) || reduxSelectedCountry;

  const selectedValue =
    value !== undefined && value !== null
      ? value
      : isShowCountryInTabbar
        ? matchedReduxCountry
        : null;

  // Whenever Redux selected country changes, inform parent via `onChange` (Global mode only)
  useEffect(() => {
    if (isShowCountryInTabbar && onChange) {
      onChange(name, matchedReduxCountry || null);
    }
  }, [reduxSelectedCountry, countriesList, isShowCountryInTabbar]);

  const handleChange = (fieldName, newValue, triggeredAction) => {
    const selectedObj =
      newValue && newValue[EnumOptions.COUNTRY.VALUE] !== 0 ? newValue : null;

    if (isShowCountryInTabbar) {
      dispatch(updateSelectedCountry(selectedObj));
      if (selectedObj?.[EnumOptions.COUNTRY.VALUE]) {
        setThisKeyCookie(
          EnumCookieKeys.COUNTRY_ID,
          selectedObj[EnumOptions.COUNTRY.VALUE],
        );
      } else {
        removeThisKeyCookie(EnumCookieKeys.COUNTRY_ID);
      }
    }

    if (onChange) {
      onChange(fieldName || name, selectedObj, triggeredAction);
    }
  };

  return (
    <SelectComponent
      name={name}
      options={countriesList}
      value={selectedValue}
      onChange={handleChange}
      height={height}
      isLoading={loading}
      disabled={disabled}
      required={required}
      placeholder={placeholder}
      optionLabel={EnumOptions.COUNTRY.LABEL}
      optionValue={EnumOptions.COUNTRY.VALUE}
      addPleaseSelectOptionOnClear={addPleaseSelectOptionOnClear}
      {...restProps}
    />
  );
};

export default CountrySchema;
