import FilterAltOutlinedIcon from "@mui/icons-material/FilterAltOutlined";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import { TabContext } from "@mui/lab";
import {
  Box,
  Button,
  CircularProgress,
  MenuItem,
  Tab,
  Tabs,
} from "@mui/material";
import React from "react";
import { useSelector } from "react-redux";
import { Link, useLocation } from "react-router-dom";
import { styleSheet } from "../../assets/styles/style";
import {
  MenuComponent,
  useGetBreakPoint,
  useMenu,
} from "../../utilities/helpers/Helpers";

export default function DataGridTabs(props) {
  const {
    handleTabChange,
    tabData,
    handleFilterBtnOnClick,
    actionBtnMenuData,
    otherBtns,
    customBorderRadius,
    filterData,
    responsiveButton,
    tabsSmWidth,
    tabsMdWidth,
    loading,
    isRouteChangeable = true,
    selectedTab,
  } = props;
  const location = useLocation();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const {
    anchorEl: actionMenuAnchorEl,
    open: actionMenuOpen,
    handleOpen: handleActionMenuOpen,
    handleClose: handleActionMenuClose,
  } = useMenu();
  const belowMdScreen = useGetBreakPoint("sm");
  const handleShowTabChildren = () => {
    if (Array.isArray(tabData)) {
      const selectedTab = tabData?.find(
        (dt) => dt.route === window.location.pathname
      );
      return selectedTab?.children;
    }
  };
  return (
    <>
      <Box
        className={"flex_between"}
        p={1}
        sx={{
          borderRadius: customBorderRadius || "8px 8px 0px 0px",
          border: "1px solid rgba(224, 224, 224, 1)",
          borderBottom: "none",
          background: "#F8F8F8",
        }}
      >
        <TabContext>
          {loading ? (
            <Box display="flex" justifyContent="center" alignItems="center">
              <CircularProgress size={20} />
            </Box>
          ) : (
            <Tabs
              onChange={handleTabChange}
              value={isRouteChangeable ? location.pathname : selectedTab}
              variant="scrollable"
              allowScrollButtonsMobile
              aria-label="agent-roster-tabs"
              sx={{
                ...styleSheet.customTabsUI,
                alignItems: "center",
                minHeight: 26,
                width: {
                  xs: tabsSmWidth || 250,
                  sm: tabsMdWidth || 350,
                  md: 450,
                  lg: 550,
                },
              }}
            >
              {tabData?.map((item, index) => (
                <Tab
                  key={index}
                  sx={{
                    fontSize: "12px",
                    fontWeight: 600,
                    minWidth: "50px",
                    padding: {
                      xs: "8px 10px",
                      sm: "8px 12px",
                      md: "12px 16px",
                    },
                  }}
                  label={item.label}
                  value={isRouteChangeable ? item.route : item?.value}
                  to={item.route}
                  component={isRouteChangeable && Link}
                />
              ))}{" "}
            </Tabs>
          )}
        </TabContext>

        <Box className={"flex_center"} gap={1}>
          {otherBtns}
          {actionBtnMenuData && (
            <>
              <Button
                sx={{
                  border: " 1px solid rgba(30, 30, 30, 0.2)   !important",
                  textTransform: "capitalize  !important",
                  height: "29px",
                  fontSize: { md: "10px", lg: "12px" },
                  padding: "5px 5px",
                  minWidth: { md: "75px", lg: "90px" },
                }}
                variant="contained"
                aria-haspopup="true"
                disableElevation
                onClick={handleActionMenuOpen}
                endIcon={<KeyboardArrowDownIcon />}
              >
                {LanguageReducer?.languageType?.ORDERS_ACTION}
              </Button>
              <MenuComponent
                anchorEl={actionMenuAnchorEl}
                open={actionMenuOpen}
                onClose={handleActionMenuClose}
              >
                {actionBtnMenuData.map((dt) => {
                  return (
                    <MenuItem
                      onClick={() => {
                        dt.onClick();
                        handleActionMenuClose();
                      }}
                      disableRipple
                    >
                      {dt.title}
                    </MenuItem>
                  );
                })}
              </MenuComponent>
            </>
          )}
          {handleFilterBtnOnClick && (
            <Button
              sx={{
                ...styleSheet.filterIconColord,
                minWidth: {
                  lg: "80px !important",
                  md: "70px !important",
                  sm: "70px !important",
                  xs: "40px !important",
                },
              }}
              color="inherit"
              variant="outlined"
              onClick={handleFilterBtnOnClick}
              startIcon={
                !belowMdScreen ? (
                  <FilterAltOutlinedIcon fontSize="small" />
                ) : null
              }
            >
              {!belowMdScreen ? (
                LanguageReducer?.languageType?.ORDERS_FILTER
              ) : (
                <FilterAltOutlinedIcon fontSize="small" />
              )}
            </Button>
          )}
          <Box>{responsiveButton}</Box>
        </Box>
      </Box>
      <Box>{filterData}</Box>
      <Box>{handleShowTabChildren()}</Box>
    </>
  );
}
