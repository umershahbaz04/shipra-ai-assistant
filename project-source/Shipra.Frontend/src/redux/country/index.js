import { createSlice } from "@reduxjs/toolkit";
import { getThisKeyCookie } from "../../utilities/cookies";
import { EnumCookieKeys } from "../../utilities/enum";

const initialCountryId = getThisKeyCookie(EnumCookieKeys.COUNTRY_ID);

const initialState = {
  selectedCountry: initialCountryId ? { countryId: Number(initialCountryId) } : null,
  countries: [],
};

const CountryReducer = createSlice({
  name: "CountryReducer",
  initialState,
  reducers: {
    updateSelectedCountry: (state, action) => {
      state.selectedCountry = action.payload;
    },
    setCountriesList: (state, action) => {
      state.countries = action.payload;
    },
  },
});

export const { updateSelectedCountry, setCountriesList } = CountryReducer.actions;
export default CountryReducer.reducer;
