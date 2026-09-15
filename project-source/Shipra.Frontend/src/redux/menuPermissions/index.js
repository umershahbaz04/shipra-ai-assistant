import { createSlice } from "@reduxjs/toolkit";

const getSavedMenuPermissions = () => {
  try {
    const saved =
      sessionStorage.getItem("menuPermissions") ??
      localStorage.getItem("menuPermissions");
    return saved !== null && saved !== undefined ? JSON.parse(saved) : null;
  } catch (e) {
    return null;
  }
};

const savedData = getSavedMenuPermissions();

const initialState = {
  menuPermissions: savedData !== null ? savedData : [],
  isLoading: false,
  hasFetched: savedData !== null,
};

const menuPermissionsSlice = createSlice({
  name: "menuPermissions",
  initialState,
  reducers: {
    setMenuPermissions: (state, action) => {
      state.menuPermissions = action.payload || [];
      state.hasFetched = true;
      state.isLoading = false;
      try {
        const json = JSON.stringify(action.payload || []);
        sessionStorage.setItem("menuPermissions", json);
        localStorage.setItem("menuPermissions", json);
      } catch (e) {
        console.error("Error storing menuPermissions", e);
      }
    },
    setMenuPermissionsLoading: (state, action) => {
      state.isLoading = action.payload;
    },
    clearMenuPermissions: (state) => {
      state.menuPermissions = [];
      state.hasFetched = false;
      state.isLoading = false;
      sessionStorage.removeItem("menuPermissions");
      localStorage.removeItem("menuPermissions");
    },
  },
});

export const { setMenuPermissions, setMenuPermissionsLoading, clearMenuPermissions } =
  menuPermissionsSlice.actions;

export default menuPermissionsSlice.reducer;
