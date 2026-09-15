import { createSlice } from "@reduxjs/toolkit";
import { SUCCESS_N_FAILED_REDUX_STATE } from "../constants";
const initialState = {
  isNumericKeyInputPress: false,
  isApiCalling: {},
  filterModel: {},
  successNfailedData: SUCCESS_N_FAILED_REDUX_STATE,
  clientSubscription: {
    isTrialMode: false,
    data: {},
  },
  badgeData: {},
  mapApiKey: null,
};
const changeFlagsSlice = createSlice({
  name: "flags",
  initialState,
  reducers: {
    changeNumericKeyInputPressFlag: (state, action) => {
      state.isNumericKeyInputPress = action.payload;
    },
    changeApiCallingFlag: (state, action) => {
      const apiName = action.payload.apiName;
      state.isApiCalling[apiName] = {};
      state.isApiCalling[apiName].loading = action.payload.loading;
      state.isApiCalling[apiName].msg = action.payload.msg;
    },
    changeApiFilterModel: (state, action) => {
      const apiName = action.payload.apiName;
      state.filterModel[apiName] = {};
      state.filterModel[apiName].start =
        action.payload.page * action.payload.pageSize;
      state.filterModel[apiName].length = action.payload.pageSize;
    },
    changeClientSubscription: (state, action) => {
      // console.log(action.payload);
      state.clientSubscription.isTrialMode = action.payload.isOnTrail;
      state.clientSubscription.data = action.payload;
    },
    changeSuccessNFaildedData: (state, action) => {
      console.log(action.payload);
      state.successNfailedData = action.payload;
    },
    changeBadgeData: (state, action) => {
      state.badgeData = { ...state.badgeData, ...action.payload };
    },
    updateBadgeData: (state, action) => {
      const [path, count] = Object.entries(action.payload)[0];
      state.badgeData[path] = count;
    },
    setMapApiKey: (state, action) => {
      state.mapApiKey = action.payload;
    },
    clearMapApiKey: (state) => {
      state.mapApiKey = null;
    },
  },
});
export const {
  changeNumericKeyInputPressFlag,
  changeApiCallingFlag,
  changeApiFilterModel,
  changeClientSubscription,
  changeSuccessNFaildedData,
  changeBadgeData,
  updateBadgeData,
  setMapApiKey,
  clearMapApiKey,
} = changeFlagsSlice.actions;

export default changeFlagsSlice.reducer;
