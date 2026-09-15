import { Box, Tab, Tabs } from "@mui/material";
import { TabContext } from "@mui/lab";

import React from "react";
import { Link, useLocation } from "react-router-dom";
import { styleSheet } from "../../assets/styles/style";

import { alpha, styled } from "@mui/material/styles";
import { makeStyles } from "@material-ui/core/styles";

const StyledMenu = styled((props) => (
  <Menu
    elevation={0}
    anchorOrigin={{
      vertical: "bottom",
      horizontal: "right",
    }}
    transformOrigin={{
      vertical: "top",
      horizontal: "right",
    }}
    {...props}
  />
))(({ theme }) => ({
  "& .MuiPaper-root": {
    borderRadius: 6,
    marginTop: theme.spacing(1),
    minWidth: 180,
    color:
      theme.palette.mode === "light"
        ? "rgb(55, 65, 81)"
        : theme.palette.grey[300],
    boxShadow:
      "rgb(255, 255, 255) 0px 0px 0px 0px, rgba(0, 0, 0, 0.05) 0px 0px 0px 1px, rgba(0, 0, 0, 0.1) 0px 10px 15px -3px, rgba(0, 0, 0, 0.05) 0px 4px 6px -2px",
    "& .MuiMenu-list": {
      padding: "4px 0",
    },
    "& .MuiMenuItem-root": {
      "& .MuiSvgIcon-root": {
        fontSize: 18,
        color: theme.palette.text.secondary,
        marginRight: theme.spacing(1.5),
      },
      "&:active": {
        backgroundColor: alpha(
          theme.palette.primary.main,
          theme.palette.action.selectedOpacity
        ),
      },
    },
  },
}));
const useStyles = makeStyles((theme) => ({
  tabsRoot: {
    minHeight: "26px !important",
    height: "26px !important",
  },
  tabRoot: {
    minHeight: "30px",
    height: "30px",
  },
}));
export default function DataGridHeader(props) {
  const { children, tabs = true, tabData = [] } = props;
  const location = useLocation();
  const classes = useStyles();

  return (
    <Box
      display={"flex"}
      alignItems={"center"}
      justifyContent={tabs ? "space-between" : "flex-end"}
      bgcolor={"#F8F8F8"}
      border={"1px solid rgba(224, 224, 224, 1)"}
      borderRadius={"8px 8px 0px 0px"}
      borderBottom={"none"}
      padding={"7px"}
      sx={{}}
    >
      <Box
        sx={{
          ...styleSheet.TabBarArea,
          width: props?.width
            ? "100%"
            : {
                xs: 155,
                sm: 350,
                md: 500,
              },
        }}
      >
        <TabContext>
          <Box>
            <Tabs
              onChange={props?.handleTabChange}
              value={location.pathname}
              TabIndicatorProps={{
                sx: {
                  background: "black",
                  color: "black",
                  minHeight: "26px!important",
                },
              }}
              variant="scrollable"
              allowScrollButtonsMobile
              aria-label="agent-roster-tabs"
              sx={{
                ...styleSheet.customTabsUI,
                justifyContent: "space-around",
              }}
              classes={{
                root: classes.tabsRoot,
              }}
            >
              {tabData?.map((item, index) => (
                <Tab
                  key={index}
                  sx={{
                    fontSize: "12px",
                    fontWeight: 600,
                    minWidth: "40px",
                    padding: {
                      xs: "8px 8px",
                      sm: "10px 14px",
                      md: "12px 16px",
                    },
                  }}
                  label={item.label}
                  value={item.route}
                  to={item.route}
                  component={Link}
                  onClick={() => {
                    item.onclick && item.onclick();
                  }}
                />
              ))}{" "}
            </Tabs>
          </Box>
        </TabContext>
      </Box>

      {children}
    </Box>
  );
}
