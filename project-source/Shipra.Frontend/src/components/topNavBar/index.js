import {
  Avatar,
  Box,
  CircularProgress,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  Stack,
  Typography,
} from "@mui/material";
import React, { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import ButtonComponent from "../../.reUseableComponents/Buttons/ButtonComponent";
import avatarIcon from "../../assets/images/avatar.png";
import cnFlag from "../../assets/images/cnFlag.png";
import saFlag from "../../assets/images/saFlag.png";
import pakFlag from "../../assets/images/pakFlag.png";
import Category from "../../assets/images/topNav/Category.png";
import notificationAlert from "../../assets/images/topNav/notificationAlert.png";
import usFlag from "../../assets/images/us_flag.png";
import { styleSheet } from "../../assets/styles/style";
import { changeSideBarMenu } from "../../redux/changeSideBar";
import { updateLanguage } from "../../redux/language";
import {
  getThisKeyCookie,
  setThisKeyCookie,
  removeThisKeyCookie,
  isShowCountryInTabbarFlag,
} from "../../utilities/cookies";
import CountrySchema from "../../utilities/helpers/countryschema";
import SelectComponentWithRest from "../../.reUseableComponents/TextField/SelectComponentWithRest";
import { setCountriesList, updateSelectedCountry } from "../../redux/country";
import { EnumCookieKeys, EnumOptions } from "../../utilities/enum";
import { handleGetAllCountries } from "../../utilities/helpers/addressSchema";
import { EnumRoutesUrls, EnumUserType } from "../../utilities/enum";
import Colors from "../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  calculateDaysBetween,
  CodeBox,
  CrossIconButton,
  handleDispatSuccessNFaildedData,
  handleGetClientSubscription,
  MenuComponent,
  purple,
  useClientSubscriptionReducer,
  useGetBreakPoint,
  useGetLocationOrPath,
  useIsApiCallingFlagReducer,
  useIsUserProfileChange,
  useIsUserProfileSideBarShow,
  useMenu,
  useSuccessNFaildedDataReducer,
  useUserRoleReducer,
  useWhatsAppMsg,
  WhatsAppMsg,
} from "../../utilities/helpers/Helpers";
import { handleLogout } from "../sideNavBar";
import { ChargeCoinICon } from "../../utilities/helpers/SvgIcons";
import ChargeAccountModal from "../modals/navbarModals/ChargeAccountModal";
import HelpCenterIcon from "@mui/icons-material/HelpCenter";
import { SUCCESS_N_FAILED_REDUX_STATE } from "../../redux/constants";

export const handleDispatchUserProfile = (dispatch, value, navigate) => {
  dispatch(changeSideBarMenu(value));
  navigate(value ? EnumRoutesUrls.PROFILE : EnumRoutesUrls.ANALYTICS);
  setThisKeyCookie("isUserProfileSideBarShow", value);
};

export const getSelectedTab = (navBarMenu, path) => {
  let selectedTab = null;
  let selectedTabIndex = -1;
  // tab index
  selectedTabIndex = navBarMenu.findIndex(
    (data) =>
      data?.tabData?.path === path ||
      data?.subTabData?.some((rt) => rt.path === path) ||
      data?.otherRoutes?.some((other_rt) => other_rt.path === path),
  );

  selectedTab = selectedTabIndex > -1 ? navBarMenu[selectedTabIndex] : null;

  return { selectedTab, selectedTabIndex };
};

export const languagesOptions = [
  {
    name: "English",
    code: "EN",
    dir: "ltr",
    icon: usFlag,
  },
  {
    name: "Chinese",
    code: "CN",
    dir: "ltr",
    icon: cnFlag,
  },
  {
    name: "Arabic",
    code: "AR",
    dir: "rtl",
    icon: saFlag,
  },
  {
    name: "Urdu",
    code: "UR",
    dir: "rtl",
    icon: pakFlag,
  },
];
function TopNavBar(props) {
  const { navBarMenu } = props;
  const navigate = useNavigate();
  const { whatsAppMsgRef, handleSendWhatsAppMsg } = useWhatsAppMsg();
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [selectedLanguage, setSelectedLanguage] = React.useState({});

  const isUserProfileSideBarShow = useIsUserProfileSideBarShow();
  const isUserProfileChange = useIsUserProfileChange();
  const apiCallingData = useIsApiCallingFlagReducer();
  const successNfailedData = useSuccessNFaildedDataReducer();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { path } = useGetLocationOrPath();
  const dispatch = useDispatch();
  const [openChargeAccountModal, setOpenChargeAccountModal] = useState(false);

  const isShowCountryInTabbar = isShowCountryInTabbarFlag();

  // const getTitle = () => {};

  const belowMdScreen = useGetBreakPoint("sm");

  const reduxSelectedCountry = useSelector(
    (state) => state.CountryReducer?.selectedCountry,
  );
  const reduxCountries = useSelector(
    (state) => state.CountryReducer?.countries,
  );

  const [countriesList, setCountriesListState] = useState(reduxCountries || []);
  const [countryLoading, setCountryLoading] = useState(false);

  useEffect(() => {
    let isMounted = true;
    const fetchCountries = async () => {
      if (!reduxCountries || reduxCountries.length === 0) {
        setCountryLoading(true);
        const data = await handleGetAllCountries();
        if (isMounted) {
          setCountriesListState(data || []);
          dispatch(setCountriesList(data || []));
          setCountryLoading(false);
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

  const matchedReduxCountry = reduxSelectedCountry
    ? (countriesList.find(
        (c) =>
          c[EnumOptions.COUNTRY.VALUE] ===
          (reduxSelectedCountry?.[EnumOptions.COUNTRY.VALUE] ||
            reduxSelectedCountry?.countryId ||
            reduxSelectedCountry?.id ||
            reduxSelectedCountry),
      ) || reduxSelectedCountry)
    : null;

  const selectedCountryValue = matchedReduxCountry;

  const handleCountryChange = (fieldName, newValue, triggeredAction) => {
    const selectedObj =
      newValue && newValue[EnumOptions.COUNTRY.VALUE] !== 0 ? newValue : null;

    dispatch(updateSelectedCountry(selectedObj));
    if (selectedObj?.[EnumOptions.COUNTRY.VALUE]) {
      setThisKeyCookie(
        EnumCookieKeys.COUNTRY_ID,
        selectedObj[EnumOptions.COUNTRY.VALUE],
      );
    } else {
      removeThisKeyCookie(EnumCookieKeys.COUNTRY_ID);
    }
  };

  const getTitle = (selectedTab) => {
    let title = "";
    if (selectedTab) {
      const selectedTabData = selectedTab.tabData;
      // for collapse tab
      if (selectedTab.isCollapse) {
        // for tab
        if (selectedTabData.path === path) {
          title = selectedTabData?.topTitle
            ? selectedTabData?.topTitle
            : selectedTabData?.sideTitle;
        } else {
          // for subtab
          const selectedSubTabIndex = selectedTab.subTabData.findIndex(
            (sb_tb) => sb_tb.path === path,
          );
          if (selectedSubTabIndex > -1) {
            title = selectedTab.subTabData[selectedSubTabIndex].topTitle
              ? selectedTab.subTabData[selectedSubTabIndex].topTitle
              : selectedTab.subTabData[selectedSubTabIndex].sideTitle;
          } else {
            // for other routes
            const selectedOtherRouteIndex = selectedTab.otherRoutes.findIndex(
              (other_rt) => other_rt.path === path,
            );

            if (selectedOtherRouteIndex > -1) {
              title = selectedTab.otherRoutes[selectedOtherRouteIndex].topTitle
                ? selectedTab.otherRoutes[selectedOtherRouteIndex].topTitle
                : selectedTab.otherRoutes[selectedOtherRouteIndex].sideTitle;
            }
          }
        }
      }
      // for not collapse tab
      else {
        const selectedOtherRouteIndex = selectedTab?.otherRoutes?.findIndex(
          (other_rt) => other_rt.path === path,
        );

        if (selectedOtherRouteIndex > -1) {
          title = selectedTab.otherRoutes[selectedOtherRouteIndex].topTitle
            ? selectedTab.otherRoutes[selectedOtherRouteIndex].topTitle
            : selectedTab.otherRoutes[selectedOtherRouteIndex].sideTitle;
        } else {
          title = selectedTabData?.topTitle
            ? selectedTabData?.topTitle
            : selectedTabData?.sideTitle;
        }
      }
    } else {
      title = "Forbidden";
    }
    return { title };
  };
  const {
    anchorEl: profileMenuAnchorEl,
    open: profileMenuOpen,
    handleOpen: handleProfileMenuOpen,
    handleClose: handleProfileMenuClose,
  } = useMenu();
  const userRoleId = useUserRoleReducer();
  // useEffect(() => {
  //   dispatch(updateUserRole(1));
  // }, []);
  const ProfileAvatar = useCallback(() => {
    return (
      <Avatar
        sx={{
          width: 35,
          height: 35,
        }}
        src={getThisKeyCookie("companyImage") || avatarIcon}
      />
    );
  }, [isUserProfileChange]);

  const isSuperAdmin =
    getThisKeyCookie("patronTypeId") == EnumUserType.Admin || userRoleId == 0;

  useEffect(() => {
    const defaultLangaugeObj = languagesOptions[0];
    const localStorageLanguage = localStorage.language;
    const isLanguageExist = languagesOptions.some(
      (lang) => lang.name === localStorageLanguage,
    );
    dispatch(
      updateLanguage(
        isLanguageExist ? localStorageLanguage : defaultLangaugeObj.name,
      ),
    );
    localStorage.setItem(
      "language",
      isLanguageExist ? localStorageLanguage : defaultLangaugeObj.name,
    );
    const _selectedLanguage = languagesOptions.find(
      (lang) => lang.name === localStorageLanguage,
    );
    setSelectedLanguage(_selectedLanguage || defaultLangaugeObj);

    // for check trial mode
    handleGetClientSubscription(dispatch);
  }, []);

  const clientSubscriptionData = useClientSubscriptionReducer();

  const renderSubscriptionMessage = () => {
    const isTrialMode = clientSubscriptionData.isTrialMode;
    const isSubscriptionCanceled =
      clientSubscriptionData?.data?.isSubscriptionCancel;
    const daysLeft = isTrialMode
      ? calculateDaysBetween(clientSubscriptionData?.data?.trialEnd)
      : calculateDaysBetween(clientSubscriptionData?.data?.currentPeriodEnd);

    const periodType = isTrialMode ? "trial" : "subscription";
    const endMessage =
      daysLeft > 0
        ? `Your ${periodType} period ends in just ${daysLeft} days! Currently, you've no subscription plan.`
        : `Your ${periodType} period will end today! Currently, you've no subscription plan.`;

    return (
      <>
        <span style={{ display: "flex", alignItems: "center", gap: "5px" }}>
          {endMessage}{" "}
          <CodeBox
            fs={14}
            title={"Click here"}
            onClick={() => {
              handleDispatchUserProfile(dispatch, true, navigate);
              navigate(EnumRoutesUrls.BILLING);
            }}
          />
          {isTrialMode && !isSubscriptionCanceled && (
            <>activate your subscription now!!!</>
          )}
          {!isTrialMode && isSubscriptionCanceled && (
            <>to renew your subscription.</>
          )}
          {isTrialMode && isSubscriptionCanceled && (
            <>to activate your subscription.</>
          )}
        </span>
        {/* <span style={{ display: "flex", alignItems: "center", gap: "5px" }}>
          Your active subscription has been switched to a trial period, which
          will end in 30 days due to a failed payment method. Please click here
          to activate your subscription before it concludes. If you have any
          questions or issues, feel free to contact us.
        </span> */}
      </>
    );
  };

  return (
    <>
      <Box sx={{ width: "100%" }}>
        {(clientSubscriptionData.isTrialMode ||
          clientSubscriptionData?.data?.isSubscriptionCancel) && (
          <Box className={"flex_center active-row"} sx={{ p: 1 }}>
            <Typography variant="h5">{renderSubscriptionMessage()}</Typography>
          </Box>
        )}

        <Box sx={{ ...styleSheet.topNavBar, px: 1.25 }}>
          <Typography
            variant="h3"
            sx={{ fontSize: { md: "18px", sm: "16px", xs: "14px" } }}
          >
            {getTitle(getSelectedTab(navBarMenu, path).selectedTab).title}
          </Typography>
          <Stack
            sx={styleSheet.topNavBarRight}
            direction="row"
            justifyContent="flex-end"
            alignItems="center"
            spacing={1}
          >
            {Object.values(apiCallingData || {}).map(
              (dt, index) =>
                dt?.loading && (
                  <Box
                    key={`api-loading-${index}`}
                    className={"flex_between"}
                    gap={1}
                  >
                    <CircularProgress size={14} />
                    <Typography variant="h3" color={purple} fontWeight={300}>
                      {`${dt?.msg}...`}
                    </Typography>
                  </Box>
                ),
            )}
            {isShowCountryInTabbar && (
              <Box sx={{ width: 160, minWidth: 160, maxWidth: 160 }}>
                <SelectComponentWithRest
                  name="country"
                  options={countriesList}
                  value={selectedCountryValue}
                  onChange={handleCountryChange}
                  height={32}
                  isLoading={countryLoading}
                  placeholder="Country"
                  optionLabel={EnumOptions.COUNTRY.LABEL}
                  optionValue={EnumOptions.COUNTRY.VALUE}
                  addPleaseSelectOptionOnClear={false}
                  formatOptionLabel={(option, { context }) => {
                    if (context === "value") {
                      return option.mapCountryCode || option.name;
                    }
                    return option.name && option.mapCountryCode
                      ? `${option.name} (${option.mapCountryCode})`
                      : option.name || option.mapCountryCode;
                  }}
                />
              </Box>
            )}
            <IconButton
              sx={{
                borderRadius: "10px",
              }}
              onClick={(e) => setAnchorEl(e.currentTarget)}
            >
              <Stack
                sx={styleSheet.languageDropdownBox}
                direction="row"
                justifyContent="center"
                alignItems="center"
                spacing={1}
              >
                <img style={{ width: "23px" }} src={selectedLanguage?.icon} />
                <Box sx={styleSheet.languageDropdownTitle}>
                  {selectedLanguage?.code}
                </Box>
              </Stack>
            </IconButton>
            <Box>
              <ActionButtonCustom
                label={"Charge"}
                startIcon={<ChargeCoinICon />}
                onClick={() => setOpenChargeAccountModal(true)}
              />
            </Box>
            <Menu
              anchorEl={anchorEl}
              id="power-search-menu"
              open={Boolean(anchorEl)}
              onClose={() => {
                setAnchorEl(null);
              }}
              PaperProps={{
                elevation: 0,
                sx: {
                  overflow: "visible",
                  filter: "drop-shadow(0px 2px 8px rgba(0,0,0,0.32))",
                  mt: 1.5,
                  "& .MuiAvatar-root": {
                    width: 32,
                    height: 32,
                    ml: -0.5,
                    mr: 1,
                  },
                  "&:before": {
                    content: '""',
                    display: "block",
                    position: "absolute",
                    top: 0,
                    right: 14,
                    width: 10,
                    height: 10,
                    bgcolor: "background.paper",
                    transform: "translateY(-50%) rotate(45deg)",
                    zIndex: 0,
                  },
                },
              }}
              transformOrigin={{ horizontal: "right", vertical: "top" }}
              anchorOrigin={{ horizontal: "right", vertical: "bottom" }}
            >
              <Box sx={{ width: "160px" }}>
                <List disablePadding>
                  {languagesOptions.map((lang, index) => (
                    <ListItem key={lang.name || index} disablePadding>
                      <ListItemButton
                        onClick={() => {
                          setAnchorEl(null);
                          dispatch(updateLanguage(lang.name));
                          localStorage.setItem("language", lang.name);
                          setSelectedLanguage(lang);
                        }}
                      >
                        <ListItemIcon sx={{ minWidth: "30px" }}>
                          <img
                            style={{ width: "23px" }}
                            src={lang.icon}
                            alt="flag icon"
                          />
                        </ListItemIcon>
                        <ListItemText primary={lang.name} />
                      </ListItemButton>
                    </ListItem>
                  ))}
                </List>
              </Box>
            </Menu>
            {/* <IconButton>
          <img src={SearchIcon} style={{ width: "18px" }} alt="SearchIcon" />
        </IconButton> */}
            {!belowMdScreen && (
              <>
                <IconButton>
                  <img
                    style={{ width: "23px" }}
                    src={notificationAlert}
                    alt="notification alert"
                  />
                </IconButton>
                <IconButton>
                  <HelpCenterIcon
                    color="primary"
                    onClick={() => handleSendWhatsAppMsg()}
                  />
                </IconButton>
              </>
            )}
            <IconButton onClick={handleProfileMenuOpen}>
              <ProfileAvatar />
              {/* <Avatar
            sx={{
              width: 35,
              height: 35,
            }}
            src={getThisKeyCookie("companyImage") || avatarIcon}
          /> */}
            </IconButton>
            <MenuComponent
              anchorEl={profileMenuAnchorEl}
              open={profileMenuOpen}
              onClose={handleProfileMenuClose}
            >
              <Box p={1.25} minWidth={300}>
                <Box
                  className={"flex_center"}
                  justifyContent={"start !important"}
                  gap={1}
                >
                  <Box>
                    <Avatar
                      sx={{
                        width: 60,
                        height: 60,
                      }}
                      src={getThisKeyCookie("companyImage") || avatarIcon}
                    />
                  </Box>
                  <Box>
                    <Typography variant="h5">
                      {getThisKeyCookie("user_name")}
                    </Typography>
                    <Typography variant="h6" fontSize={13}>
                      {" "}
                      {getThisKeyCookie("email")}
                    </Typography>
                  </Box>
                </Box>

                <Box className={"flex_between"} mt={1}>
                  {isUserProfileSideBarShow && isSuperAdmin ? (
                    <ButtonComponent
                      title={"Back to App"}
                      onClick={() => {
                        handleDispatchUserProfile(dispatch, false, navigate);
                        handleProfileMenuClose();
                      }}
                    />
                  ) : (
                    isSuperAdmin && (
                      <ButtonComponent
                        title={"Setting"}
                        onClick={() => {
                          handleDispatchUserProfile(dispatch, true, navigate);
                          handleProfileMenuClose();
                        }}
                      />
                    )
                  )}
                  <ButtonComponent
                    title={"Log Out"}
                    bg={Colors.danger}
                    onClick={() =>
                      handleLogout(navigate, LanguageReducer, dispatch)
                    }
                  />
                </Box>
              </Box>
            </MenuComponent>
          </Stack>
        </Box>
        {successNfailedData?.show && (
          <Box
            className={"flex_center green_row"}
            sx={{ p: 1, position: "relative", height: 40 }}
          >
            {successNfailedData?.loading && (
              <Box
                className={"flex"}
                gap={1}
                sx={{ position: "absolute", left: 20 }}
              >
                <CircularProgress size={14} />
                <Typography variant="h5">loading...</Typography>
              </Box>
            )}
            <Typography variant="h5">{successNfailedData?.msg}</Typography>
            <CrossIconButton
              fontSize={"small"}
              sx={{ position: "absolute", right: 0 }}
              onClick={() => {
                handleDispatSuccessNFaildedData(dispatch, {
                  show: false,
                  ...SUCCESS_N_FAILED_REDUX_STATE,
                });
              }}
            />
          </Box>
        )}
      </Box>
      {openChargeAccountModal && (
        <ChargeAccountModal
          open={openChargeAccountModal}
          onClose={() => setOpenChargeAccountModal(false)}
        />
      )}
      <WhatsAppMsg
        phoneNumber={"971566595024"}
        whatsAppMsgRef={whatsAppMsgRef}
        message={`Hi Support Team,\nI need support with my account.\n\n*Username*: ${getThisKeyCookie(
          "user_name",
        )}\n*Client Code*: ${getThisKeyCookie("client_code")}`}
      />
    </>
  );
}
export default TopNavBar;
