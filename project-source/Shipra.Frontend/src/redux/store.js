import { configureStore } from "@reduxjs/toolkit";
import LanguageReducer from "./language";
import UserProfileSideBarReducer from "./changeSideBar";
import UserProfileImgChangeReducer from "./changeProfileImg";
import UserRoleReducer from "./userRole";
import changeFlagsSlice from "./changeFlags";
import menuPermissionsReducer from "./menuPermissions";
import CountryReducer from "./country";

export const store = configureStore({
  reducer: {
    LanguageReducer: LanguageReducer,
    UserProfileSideBarReducer: UserProfileSideBarReducer,
    UserProfileImgChangeReducer: UserProfileImgChangeReducer,
    UserRoleReducer: UserRoleReducer,
    changeFlagsReducer: changeFlagsSlice,
    menuPermissionsReducer: menuPermissionsReducer,
    CountryReducer: CountryReducer,
  },
});


