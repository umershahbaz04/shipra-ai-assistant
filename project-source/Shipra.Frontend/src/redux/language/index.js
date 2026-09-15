import { createSlice } from "@reduxjs/toolkit";
import { languages } from "./languageConst";
const initialState = {
  languageType: languages[localStorage.language] || languages["English"],
  language: localStorage.language || "English",
};
const LanguageReducer = createSlice({
  name: "LanguageReducer",
  initialState,
  reducers: {
    updateLanguage: (state, action) => {
      state.language = action.payload;
      state.languageType = languages[action.payload];
    },
  },
});
export const { updateLanguage } = LanguageReducer.actions;
export default LanguageReducer.reducer;
