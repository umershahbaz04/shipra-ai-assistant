import { createSlice } from "@reduxjs/toolkit";
const initialState = {
  userRoleId: 1,
};
const UserRoleReducer = createSlice({
  name: "userRole",
  initialState,
  reducers: {
    updateUserRole: (state, action) => {
      state.userRoleId = action.payload;
    },
  },
});
export const { updateUserRole } = UserRoleReducer.actions;
export default UserRoleReducer.reducer;
