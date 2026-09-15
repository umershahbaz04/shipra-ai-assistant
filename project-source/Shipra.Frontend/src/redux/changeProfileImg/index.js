import { createSlice } from "@reduxjs/toolkit";
const initialState = {
  isUserProfileImgChange: false,
};
const UserProfileImgChangeReducer = createSlice({
  name: "UserProfileImgChangeReducer",
  initialState,
  reducers: {
    changeProfileImg: (state, action) => {
      state.isUserProfileImgChange = action.payload;
    },
  },
});
export const { changeProfileImg } = UserProfileImgChangeReducer.actions;
export default UserProfileImgChangeReducer.reducer;
