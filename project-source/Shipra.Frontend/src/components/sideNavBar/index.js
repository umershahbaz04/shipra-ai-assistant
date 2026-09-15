import Undo from '@mui/icons-material/Undo';
import ExpandLess from "@mui/icons-material/ExpandLess";
import ExpandMore from "@mui/icons-material/ExpandMore";
import LogoutIcon from "@mui/icons-material/Logout";
import { Avatar, Box, Chip, FormLabel, Stack, Tooltip } from "@mui/material";
import Badge from "@mui/material/Badge";
import Collapse from "@mui/material/Collapse";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import { styled } from "@mui/material/styles";
import React, { cloneElement, useCallback, useEffect, useState } from "react";
import OutsideClickHandler from "react-outside-click-handler";
import { useDispatch, useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { logout } from "../../api/AxiosInterceptors.js";
import { styleSheet } from "../../assets/styles/style";
import setLogoutCookie from "../../pages/loginSignup/setLogoutCookie.js";
import { updateSelectedCountry } from "../../redux/country";
import {
  getThisKeyCookie,
  removeThisKeyCookie,
} from "../../utilities/cookies/index.js";
import useWindowDimensions from "../../utilities/customHooks/useWindowDimensions";
import { EnumCookieKeys, EnumRoutesUrls } from "../../utilities/enum/index.js";
import {
  handleClearMapApiKey,
  handleDispathShowBadge,
  handleGetAndSetTimeZone,
  purple,
  red,
  useGetBadgeDataReducer,
  useGetLocationOrPath,
  useIsUserProfileChange,
  useIsUserProfileSideBarShow,
} from "../../utilities/helpers/Helpers.js";
import { errorNotification } from "../../utilities/toast/index.js";
import {
  getSelectedTab,
  handleDispatchUserProfile,
  languagesOptions,
} from "../topNavBar/index.js";

const StyledBadge = styled(Badge)(({ theme }) => ({
  "& .MuiBadge-badge": {
    backgroundColor: "#44b700",
    color: "#44b700",
    boxShadow: `0 0 0 2px ${theme.palette.background.paper}`,
    "&::after": {
      position: "absolute",
      top: 0,
      left: 0,
      width: "100%",
      height: "100%",
      borderRadius: "50%",
      animation: "ripple 1.2s infinite ease-in-out",
      border: "1px solid currentColor",
      content: '""',
    },
  },
  "@keyframes ripple": {
    "0%": {
      transform: "scale(.8)",
      opacity: 1,
    },
    "100%": {
      transform: "scale(2.4)",
      opacity: 0,
    },
  },
}));
export const handleAutoRedirect = (
  selectedTabIndex,
  isUserProfileSideBarShow,
  dispatch,
  navigate
) => {
  if (selectedTabIndex === -1) {
    if (isUserProfileSideBarShow) {
      navigate(EnumRoutesUrls.PROFILE, { replace: true });
    } else {
      handleDispatchUserProfile(dispatch, false, navigate);
      navigate(EnumRoutesUrls.ANALYTICS, { replace: true });
    }
  }
};

export const handleLogout = (navigate, LanguageReducer, dispatch) => {
  navigate("/");
  removeThisKeyCookie("isUserProfileSideBarShow");
  if (dispatch) {
    handleClearMapApiKey(dispatch);
    dispatch(updateSelectedCountry(null));
  }
  let body = {
    accessToken: getThisKeyCookie("access_token"),
    RefreshToken: getThisKeyCookie("refresh_token"),
    LogoutFromAllDevices: false,
  };
  setLogoutCookie();
  logout(body)
    .then((res) => {
      setLogoutCookie(res);
    })
    .catch((e) => {
      console.log("e", e);
    });
};

function SideNavBar(props) {
  const location = useLocation();
  const { path } = useGetLocationOrPath();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const badgeData = useGetBadgeDataReducer();
  const { navBarMenu } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const isUserProfileChange = useIsUserProfileChange();

  const handlecall = async (subTabData) => {
    const data = {};
    for (let subtab_dt of subTabData) {
      if (subtab_dt?.hasBadge && subtab_dt?.handleShowBadge) {
        const badgeCount = await subtab_dt?.handleShowBadge();
        data[subtab_dt.path] = badgeCount;
      }
    }
    handleDispathShowBadge(dispatch, data);
  };

  const { width } = useWindowDimensions();
  const adminAuth = {}; /* useSelector((state) => state.adminAuth) */
  const [selected, setSelected] = useState({
    tab: {},
    subTab: {},
    collapseOpen: {},
    sideMenu: {},
  });
  const isUserProfileSideBarShow = useIsUserProfileSideBarShow();
  useEffect(() => {
    const { selectedTab, selectedTabIndex } = getSelectedTab(navBarMenu, path);
    if (selectedTabIndex != -1) {
      if (selectedTab.isCollapse) {
        const selectedSubTabIndex = selectedTab.subTabData.findIndex(
          (sb_tb) => sb_tb.path === path
        );
        setSelected((prev) => ({
          ...prev,
          tab: { [selectedTabIndex]: true },
          subTab: { [selectedSubTabIndex]: true },
          collapseOpen: { [selectedTabIndex]: true },
        }));
        handlecall(selectedTab?.subTabData);
      } else {
        setSelected((prev) => ({
          ...prev,
          tab: { [selectedTabIndex]: true },
          subTab: {},
        }));
      }
    }
  }, [location, isUserProfileSideBarShow]);

  const handleClickCollapseTab = async (item, index) => {
    if (!selected.collapseOpen[index]) {
      setSelected((prev) => ({
        ...prev,
        tab: { [index]: true },
        subTab: {},
        collapseOpen: { [index]: !prev.collapseOpen[index] },
      }));
      navigate(item.tabData.path);
      const data = {};
      for (let subtab_dt of item.subTabData) {
        if (subtab_dt?.hasBadge && subtab_dt?.handleShowBadge) {
          const badgeCount = await subtab_dt?.handleShowBadge();
          data[subtab_dt.path] = badgeCount;
        }
      }
      handleDispathShowBadge(dispatch, data);
    } else {
      setSelected((prev) => ({
        ...prev,
        tab: { [index]: true },
        subTab: {},
        collapseOpen: { [index]: prev.collapseOpen[index] },
      }));
      navigate(item.tabData.path);
      handleRightCick(item.tabData.path);
    }
  };

  const ProfileAvatar = useCallback(() => {
    return (
      <Avatar
        sx={{ width: "30px", height: "30px" }}
        src={getThisKeyCookie("companyImage")}
        aria-label="recipe"
      >
        {getThisKeyCookie("user_name")[0]}
      </Avatar>
    );
  }, [isUserProfileChange]);

  const handleRightCick = (path) => () => {
    window.open(path, "_blank");
  };
  useEffect(() => {
    const { selectedTabIndex } = getSelectedTab(navBarMenu, path);
    if (path === EnumRoutesUrls.NOTFOUND_403) {
      navigate(EnumRoutesUrls.NOTFOUND_403);
    } else {
      handleAutoRedirect(
        selectedTabIndex,
        isUserProfileSideBarShow,
        dispatch,
        navigate
      );
    }
    if (!getThisKeyCookie(EnumCookieKeys.TIME_ZONE)) {
      handleGetAndSetTimeZone();
    }
  }, [path]);

  useEffect(() => {
    // const handleKeyPress = (evt) => {
    //   // const isBackspace = evt.which === 8;
    //   // const isNumericKey = (evt.which >= 48 && evt.which <= 57) || evt.which === 46; // 46 corresponds to the dot character
    //   // // Allow backspace or numeric keys, including the dot
    //   // if (isBackspace || isNumericKey) {
    //   //   // Check if there is already a dot in the input
    //   //   const hasDot = evt.target.value.includes('.');
    //   //   // Prevent input of another dot if one is already present
    //   //   if (evt.which === 46 && hasDot) {
    //   //     evt.preventDefault();
    //   //   }
    //   //   return;
    //   // }
    //   // evt.preventDefault();
    // };
    // const numberInputs = document.querySelectorAll("input[type='number']");
    // numberInputs.forEach((input) => {
    //   input.addEventListener("keypress", handleKeyPress);
    // });
    // // Clean up the event listeners when the component unmounts
    // return () => {
    //   numberInputs.forEach((input) => {
    //     input.removeEventListener("keypress", handleKeyPress);
    //   });
    // };
  }, [location]);
  const languageDirection =
    languagesOptions.find((lang) => lang.name === localStorage.language)?.dir ||
    "ltr";
  return (
    <Box hidden={adminAuth.isLoading} sx={styleSheet.sideBarRoot}>
      {props.pcOpen || width < 600 ? (
        <List
          sx={{
            ...styleSheet.sideBarRootList,
            width: "100%",
            maxWidth: 360,
            bgcolor: "#EEEEEE",
            position: "relative",
            overflow: "auto",
            height: "calc(82vh - 75px)", //calc(100vh - 80px)",
            "& ul": { padding: 0 },
          }}
        >
          {navBarMenu.map((item, index) =>
            item.isCollapse ? (
              <ListItem key={index} disablePadding sx={{ display: "block" }}>
                <ListItemButton
                  sx={{
                    minHeight: 25,
                    paddingTop: "2px",
                    paddingBottom: "2px",
                    borderLeft: selected.tab[index]
                      ? "5px solid var(--primary-color)"
                      : "5px solid #EEEEEE",
                  }}
                  selected={selected.tab[index]}
                  onClick={() => handleClickCollapseTab(item, index)}
                  onContextMenu={handleRightCick(item.tabData.path)}
                >
                  <ListItemIcon sx={{ minWidth: "40px" }}>
                    {cloneElement(item.icon, {
                      isSelected: selected.tab[index],
                    })}
                  </ListItemIcon>
                  <ListItemText
                    sx={{
                      ...styleSheet.sideBarListText,
                      marginLeft: "10px",
                      color: selected.tab[index] ? "var(--primary-color)" : "",
                    }}
                    primary={item.tabData.sideTitle}
                  />

                  {selected.collapseOpen[index] ? (
                    <ExpandLess
                      color="secondary"
                      sx={{
                        color: selected.tab[index] ? "var(--primary-color)" : "#BDBDBD",
                        fontSize: "22px",
                      }}
                    />
                  ) : (
                    <ExpandMore
                      sx={{
                        color: selected.tab[index] ? "var(--primary-color)" : "#BDBDBD",
                        fontSize: "22px",
                      }}
                    />
                  )}
                </ListItemButton>
                <Collapse
                  sx={styleSheet.navCollapseItems}
                  in={selected.collapseOpen[index]}
                  timeout="auto"
                  unmountOnExit
                >
                  <List component="div" disablePadding>
                    {item?.subTabData?.map((item1, index1) => (
                      <ListItemButton
                        selected={selected.subTab[index1]}
                        key={index1}
                        onClick={() => {
                          setSelected((prev) => ({
                            ...prev,
                            tab: { [index]: true },
                            subTab: { [index1]: true },
                          }));
                          navigate(item1.path);
                        }}
                        onContextMenu={handleRightCick(item1.path)}
                        sx={{
                          minHeight: 25,
                          paddingTop: "2px",
                          paddingBottom: "2px",
                          ml: "5px",
                          color: selected.subTab[index1] ? "var(--primary-color)" : "",
                        }}
                      >
                        <ListItemIcon sx={{ minWidth: "40px" }}></ListItemIcon>
                        <ListItemText
                          sx={{
                            ...styleSheet.sideBarListTextSub,
                            marginLeft: "10px",
                            fontFamily:
                              "'Lato Regular', 'Inter Regular', 'Arial' !important",
                          }}
                          primary={item1.sideTitle}
                        />
                        {badgeData[item1.path] && (
                          <Chip
                            label={badgeData[item1.path]}
                            size="small"
                            sx={{
                              mr: 0.5,
                              height: 18,
                              fontSize: "12px",
                              backgroundColor: "var(--primary-color)",
                              color: "#fff",
                              fontWeight: 500,
                              borderRadius: "2px",
                              paddingX: "4px important",
                            }}
                          />
                        )}
                      </ListItemButton>
                    ))}
                  </List>
                </Collapse>
              </ListItem>
            ) : (
              <ListItemButton
                sx={{
                  minHeight: 25,
                  paddingTop: "2px",
                  paddingBottom: "2px",
                  borderLeft: selected.tab[index]
                    ? "5px solid var(--primary-color)"
                    : "5px solid #EEEEEE",
                }}
                onClick={() => {
                  setSelected((prev) => ({
                    ...prev,
                    tab: { [index]: true },
                    subTab: {},
                  }));
                  navigate(item.tabData.path);
                }}
                onContextMenu={handleRightCick(item.tabData.path)}
                selected={selected.tab[index]}
                key={index}
              >
                <ListItemIcon sx={{ minWidth: "40px" }}>
                  {cloneElement(item.icon, {
                    isSelected: selected.tab[index],
                  })}
                </ListItemIcon>
                <ListItemText
                  sx={{
                    ...styleSheet.sideBarListText,
                    marginLeft: "10px",
                    color: selected.tab[index] ? "var(--primary-color)" : "",
                  }}
                  primary={item.tabData.sideTitle}
                />
              </ListItemButton>
            )
          )}
        </List>
      ) : (
        <List sx={{ paddingTop: "0px", height: "59vh" }}>
          {navBarMenu.map((item, index) =>
            item.isCollapse ? (
              <OutsideClickHandler key={index} onOutsideClick={() => {}}>
                <ListItem key={index} disablePadding sx={{ display: "block" }}>
                  <Tooltip
                    title={
                      selected.sideMenu[index] ? "" : item.tabData.sideTitle
                    }
                    placement="right"
                    arrow
                  >
                    <ListItemButton
                      sx={{
                        minHeight: 35,
                        paddingTop: "2px",
                        paddingBottom: "2px",
                        borderLeft: selected.tab[index]
                          ? "5px solid var(--primary-color)"
                          : "5px solid #EEEEEE",
                      }}
                      selected={selected.tab[index]}
                      onClick={() => {
                        setSelected((prev) => ({
                          ...prev,
                          sideMenu: { [index]: !prev.sideMenu[index] },
                        }));
                      }}
                      onContextMenu={handleRightCick(item.tabData.path)}
                    >
                      <ListItemIcon sx={{ minWidth: "40px" }}>
                        {cloneElement(item.icon, {
                          isSelected: selected.tab[index],
                        })}
                      </ListItemIcon>
                    </ListItemButton>
                  </Tooltip>
                  {selected.sideMenu[index] && (
                    <div
                      style={{
                        position: "absolute",
                        top: 0,
                        left: languageDirection === "ltr" ? "65px" : "",
                        right: languageDirection === "ltr" ? "" : 65,
                        background: "white",
                        width: "180px",
                        zIndex: `${999 + index}`,
                      }}
                    >
                      <List component="div" disablePadding>
                        {(item.tabData.path === item.subTabData[0].path
                          ? [...item.subTabData]
                          : [item.tabData, ...item.subTabData]
                        ).map((item1, index1) => (
                          <ListItemButton
                            selected={location.pathname === item1.path}
                            key={index1}
                            onClick={() => {
                              setSelected((prev) => ({
                                ...prev,
                                tab: { [index]: true },
                                subTab: { [index1]: true },
                                collapseOpen: { [index]: true },
                                sideMenu: {},
                              }));
                              navigate(item1.path);
                            }}
                            sx={{
                              minHeight: 25,
                              paddingTop: "2px",
                              paddingBottom: "2px",
                              pl: 4,
                            }}
                            onContextMenu={handleRightCick(item1.path)}
                          >
                            <ListItemText
                              // classes={{ primary: styleSheet.sideBarListText }}
                              sx={{
                                ...styleSheet.sideBarListText,
                              }}
                              primary={item1.sideTitle}
                            />
                            {badgeData[item1.path] && (
                              <Chip
                                label={badgeData[item1.path]}
                                size="small"
                                sx={{
                                  mr: 0.5,
                                  height: 20,
                                  fontSize: "12px",
                                  backgroundColor: "var(--primary-color)",
                                  color: "#fff !important",
                                  fontWeight: 500,
                                  borderRadius: "2px",
                                  paddingX: "4px important",
                                }}
                              />
                            )}
                          </ListItemButton>
                        ))}
                      </List>
                    </div>
                  )}
                </ListItem>
              </OutsideClickHandler>
            ) : (
              <ListItem
                onClick={() => {
                  setSelected((prev) => ({
                    ...prev,
                    tab: { [index]: true },
                    subTab: {},
                    sideMenu: {},
                  }));
                  navigate(item.tabData.path);
                }}
                onContextMenu={handleRightCick(item.tabData.path)}
                key={index}
                selected={selected.tab[index]}
                disablePadding
              >
                <Tooltip
                  title={selected.sideMenu[index] ? "" : item.tabData.sideTitle}
                  placement="right"
                  arrow
                >
                  <ListItemButton
                    sx={{
                      paddingTop: "2px",
                      paddingBottom: "2px",
                      minHeight: 35,
                      justifyContent: props.pcOpen ? "initial" : "center",
                      px: 1.5,
                      borderLeft: selected.tab[index]
                        ? "5px solid var(--primary-color)"
                        : "5px solid #EEEEEE",
                    }}
                  >
                    <ListItemIcon
                      sx={{
                        minWidth: "40px",
                        mr: props.pcOpen ? 3 : "auto",
                        justifyContent: "center",
                      }}
                    >
                      {cloneElement(item.icon, {
                        isSelected: selected.tab[index],
                      })}
                    </ListItemIcon>
                  </ListItemButton>
                </Tooltip>
              </ListItem>
            )
          )}
        </List>
      )}
      <Box
        sx={{
          position: "fixed",
          bottom: 0,
          width: "100%",
        }}
      >
        <List
          sx={{
            ...styleSheet.sideBarRootList,
            width: "100%",
            maxWidth: {
              xs: 256,
              sm: 256,
              md: 360,
              lg: 360,
            },
            bgcolor: "#EEEEEE",
            p: 0,
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          }}
        >
          <ListItem sx={{ p: 0 }}>
            <ListItemButton sx={{ color: "unset" }}>
              <ListItemIcon sx={{ minWidth: "40px" }}>
                <StyledBadge
                  overlap="circular"
                  anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
                  variant="dot"
                >
                  {/* <Avatar
                    sx={{ width: "30px", height: "30px" }}
                    src={getThisKeyCookie("companyImage")}
                    aria-label="recipe"
                  >
                    {getThisKeyCookie("user_name")[0]}
                  </Avatar> */}
                  <ProfileAvatar />
                </StyledBadge>
              </ListItemIcon>{" "}
              <ListItemText
                sx={{
                  ...styleSheet.sideBarListText,
                  marginLeft: "10px",
                }}
                // classes={{ primary: styleSheet.sideBarListText }}
                primary={
                  (props.pcOpen && (
                    <Stack direction={"column"}>
                      <Box>{getThisKeyCookie("user_name")}</Box>
                      <FormLabel sx={{ fontSize: "13px" }}>
                        {getThisKeyCookie("email")}
                      </FormLabel>
                    </Stack>
                  )) ||
                  (props.mobileOpen && (
                    <Stack direction={"column"}>
                      <Box>{getThisKeyCookie("user_name")}</Box>
                      <FormLabel sx={{ fontSize: "13px" }}>
                        {getThisKeyCookie("email")}
                      </FormLabel>
                    </Stack>
                  ))
                }
              />
            </ListItemButton>
          </ListItem>

          {isUserProfileSideBarShow && (
            <>
              <ListItem
                sx={{
                  p: 0,
                  bgcolor: purple,
                  color: "#fff",
                }}
              >
                <Tooltip
                  title={
                    !props.pcOpen &&
                    LanguageReducer?.languageType?.BACK_TO_APP_TEXT
                  }
                  placement="right"
                  arrow
                >
                  <ListItemButton
                    sx={{ color: "unset" }}
                    onClick={() => {
                      handleDispatchUserProfile(dispatch, false, navigate);
                    }}
                  >
                    <ListItemIcon sx={{ minWidth: "40px" }}>
                      <Undo sx={{ color: "#fff" }} />
                    </ListItemIcon>{" "}
                    <ListItemText
                      sx={{
                        ...styleSheet.sideBarListText,
                        marginLeft: "10px",
                      }}
                      primary={
                        (props.pcOpen &&
                          LanguageReducer?.languageType?.BACK_TO_APP_TEXT) ||
                        (props.mobileOpen &&
                          LanguageReducer?.languageType?.BACK_TO_APP_TEXT)
                      }
                    />
                  </ListItemButton>
                </Tooltip>
              </ListItem>
            </>
          )}

          <ListItem sx={{ p: 0, bgcolor: red, color: "#fff" }}>
            <Tooltip
              title={
                (props.pcOpen || props.mobileOpen) &&
                LanguageReducer?.languageType?.LOGOUT_TEXT
              }
              placement="right"
              arrow
            >
              <ListItemButton
                sx={{ color: "unset" }}
                onClick={(e) => handleLogout(navigate, LanguageReducer, dispatch)}
              >
                <ListItemIcon sx={{ minWidth: "40px" }}>
                  <LogoutIcon sx={{ color: "#fff" }} />
                </ListItemIcon>
                <ListItemText
                  sx={{
                    ...styleSheet.sideBarListText,
                    marginLeft: "10px",
                  }}
                  primary={
                    (props.pcOpen &&
                      LanguageReducer?.languageType?.LOGOUT_TEXT) ||
                    (props.mobileOpen &&
                      LanguageReducer?.languageType?.LOGOUT_TEXT)
                  }
                />
              </ListItemButton>
            </Tooltip>
          </ListItem>
        </List>
      </Box>
    </Box>
  );
}
export default SideNavBar;
