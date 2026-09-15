import { createSlice } from "@reduxjs/toolkit";
import { cookie_isUserProfileSideBarShow } from "../../utilities/helpers/Helpers";
const initialState = {
  isUserProfileSideBarShow: cookie_isUserProfileSideBarShow,
};
const UserProfileSideBarReducer = createSlice({
  name: "UserProfileSideBarReducer",
  initialState,
  reducers: {
    changeSideBarMenu: (state, action) => {
      state.isUserProfileSideBarShow = action.payload;
    },
  },
});
export const { changeSideBarMenu } = UserProfileSideBarReducer.actions;
export default UserProfileSideBarReducer.reducer;
