import FilterAltOutlinedIcon from "@mui/icons-material/FilterAltOutlined";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import { TabContext } from "@mui/lab";
import {
  Box,
  Button,
  ClickAwayListener,
  Fade,
  Grow,
  ListItemIcon,
  Menu,
  MenuItem,
  MenuList,
  Paper,
  Popper,
  Stack,
  Tab,
  Tabs,
} from "@mui/material";
import { alpha, styled } from "@mui/material/styles";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { styleSheet } from "../../../assets/styles/style";
import {
  ToggleButtonComponent,
  useGetBreakPoint,
} from "../../../utilities/helpers/Helpers.js";
import MenuIconComponent from "../../../.reUseableComponents/Buttons/MenuIconComponent.js";

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
          theme.palette.action.selectedOpacity,
        ),
      },
    },
  },
}));

const GeneralTabBar = (props) => {
  let {
    options,
    onChangeMenu,
    handleTabChange,
    placeholder,
    value,
    placement,
    id,
    minWidth,
    tabData,
    disableFilter,
    disableSearch,
    width,
    padding,
    tabScreen,
    setOpenAddStore,
    setOpenSettlementModal,
    isFilterOpen,
    setIsFilterOpen,
    exportToXLSX,
    syncInventory,
    setAddEmployee,
    setViewMode,
    viewMode,
    handleAssignProduct,
    handleUnassignProduct,
  } = props;
  const location = useLocation();
  const belowMdScreen = useGetBreakPoint("sm");
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const navigate = useNavigate();
  const [anchorElAction, setAnchorElAction] = useState(null);

  const openAction = Boolean(anchorElAction);

  const handleClick = (event) => {
    setAnchorElAction(event.currentTarget);
  };

  const handleCloseMenuOld = () => {
    setAnchorElAction(null);
  };
  //for dynamic option
  const anchorRef = React.useRef(null);
  const [selectedIndex, setSelectedIndex] = React.useState("");
  const [open, setOpen] = React.useState(false);

  useEffect(() => {
    if (value) {
      let index = options.findIndex((item) => item.value === value);
      setSelectedIndex(index);
    }
  }, [value]);

  const handleMenuItemClick = (event, index) => {
    setSelectedIndex(index);
    setOpen(false);
    if (options[index]?.value) {
      onChangeMenu(options[index]?.value);
    }
  };

  const handleToggle = () => {
    setOpen((prevOpen) => !prevOpen);
  };

  const handleClose = (event) => {
    if (anchorRef.current && anchorRef.current.contains(event.target)) {
      return;
    }
    setOpen(false);
  };

  const menuItems = [
    ...(tabScreen === "products"
      ? [
          {
            label: LanguageReducer?.languageType?.PRODUCTS_ADD_PRODUCTS,
            onClick: () => navigate("/add-products"),
          },
          {
            label: "Upload Product",
            onClick: () => navigate("/upload-product"),
          },
        ]
      : []),
    ...(tabScreen === "store"
      ? [
          {
            label: LanguageReducer?.languageType?.STORE_ADD,
            onClick: () => setOpenAddStore(true),
          },
        ]
      : []),
  ];

  return (
    <>
      <Box
        sx={{
          ...styleSheet.generalTabBarArea,
          width: width ? width : "auto",
          display: width ? "inline-block" : "flex",
          mb: width ? "20px" : "",
        }}
      >
        <Box sx={{ ...styleSheet.TabBarArea, width: width ? "100%" : "85%" }}>
          <TabContext>
            <Box>
              <Tabs
                onChange={handleTabChange}
                value={location.pathname}
                TabIndicatorProps={{
                  sx: { background: "black", color: "black" },
                }}
                variant="scrollable"
                allowScrollButtonsMobile
                aria-label="agent-roster-tabs"
                sx={{
                  ...styleSheet.customTabsUI,
                  justifyContent: "space-around",
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
                  />
                ))}{" "}
              </Tabs>
            </Box>
          </TabContext>
        </Box>
        <Box
          hidden={disableFilter && disableSearch ? true : false}
          sx={styleSheet.filterAndSearchArea}
        >
          <Stack
            direction="row"
            justifyContent="flex-end"
            alignItems="center"
            spacing={1}
          >
            {/* <Button
            sx={{ border: " 1px solid rgba(30, 30, 30, 0.2)", width: "26px", minWidth: "26px", height: "26px" }}
            color="inherit"
            variant="outlined"
          >
            <img src={SearchIcon} style={{ width: "10px" }} alt="SearchIcon" />
          </Button> */}
            {options && (
              <>
                <Button
                  sx={{ ...styleSheet.filterIcon, minWidth: "90px" }}
                  variant="contained"
                  ref={anchorRef}
                  size="small"
                  aria-controls={open ? id : undefined}
                  aria-describedby={open ? id : undefined}
                  aria-expanded={open ? "true" : undefined}
                  aria-label="select merge strategy"
                  aria-haspopup="menu"
                  onClick={handleToggle}
                  endIcon={<KeyboardArrowDownIcon />}
                >
                  {placeholder}
                </Button>
                <Popper
                  open={open}
                  id={open ? id : undefined}
                  anchorEl={anchorRef.current}
                  transition
                  disablePortal
                  placement={placement}
                  sx={{ zIndex: 10000 }}
                >
                  {({ TransitionProps }) => (
                    <Fade {...TransitionProps} timeout={350}>
                      <Grow {...TransitionProps}>
                        <Paper>
                          <ClickAwayListener onClickAway={handleClose}>
                            <MenuList
                              sx={{
                                minWidth: minWidth
                                  ? `${
                                      Number(minWidth.split("px")[0]) + 40
                                    }px !important`
                                  : "162px !important",
                              }}
                              id={id}
                              autoFocusItem
                            >
                              {options.map((option, index) => (
                                <MenuItem
                                  key={index}
                                  selected={index === selectedIndex}
                                  onClick={(event) =>
                                    handleMenuItemClick(event, index)
                                  }
                                  value={option.value ? option.value : option}
                                >
                                  {option.icon ? (
                                    <ListItemIcon>{option.icon}</ListItemIcon>
                                  ) : null}
                                  {option.title ? option.title : option}
                                </MenuItem>
                              ))}
                            </MenuList>
                          </ClickAwayListener>
                        </Paper>
                      </Grow>
                    </Fade>
                  )}
                </Popper>
              </>
            )}
            {belowMdScreen &&
              (tabScreen === "products" ? (
                <>
                  <ToggleButtonComponent
                    value={viewMode}
                    onChange={(prevevent, nextView) => {
                      setViewMode(nextView);
                    }}
                  />
                </>
              ) : null)}
            {!belowMdScreen ? (
              <>
                {tabScreen === "store" ? (
                  <Button
                    sx={{ ...styleSheet.filterIconColord, minWidth: "100px" }}
                    color="inherit"
                    variant="outlined"
                    onClick={() => {
                      setOpenAddStore(true);
                    }}
                  >
                    {LanguageReducer?.languageType?.STORE_ADD}
                  </Button>
                ) : null}
                {tabScreen === "driver" ? (
                  <Button
                    sx={{ ...styleSheet.filterIconColord, minWidth: "110px" }}
                    color="inherit"
                    variant="outlined"
                    onClick={() => {
                      setAddEmployee(true);
                    }}
                  >
                    {"Add Driver"}
                  </Button>
                ) : null}
                {tabScreen === "products" ? (
                  <>
                    <ToggleButtonComponent
                      value={viewMode}
                      onChange={(prevevent, nextView) => {
                        setViewMode(nextView);
                      }}
                    />
                    <Button
                      sx={{ ...styleSheet.filterIconColord, minWidth: "120px" }}
                      color="inherit"
                      variant="outlined"
                      onClick={() => {
                        navigate("/add-products");
                      }}
                    >
                      {LanguageReducer?.languageType?.PRODUCTS_ADD_PRODUCTS}
                    </Button>
                    <Button
                      sx={{ ...styleSheet.filterIconColord, minWidth: "120px" }}
                      color="inherit"
                      variant="outlined"
                      onClick={() => {
                        navigate("/upload-product");
                      }}
                    >
                      {"Upload Product"}
                    </Button>
                    <Button
                      sx={{ ...styleSheet.filterIconColord, minWidth: "120px" }}
                      color="inherit"
                      variant="outlined"
                      onClick={handleAssignProduct}
                    >
                      {"Assign Product"}
                    </Button>
                    <Button
                      sx={{ ...styleSheet.filterIconColord, minWidth: "130px" }}
                      color="inherit"
                      variant="outlined"
                      onClick={handleUnassignProduct}
                    >
                      {"Unassign Product"}
                    </Button>
                  </>
                ) : null}

                {tabScreen === "codPending" ? (
                  <Button
                    sx={{ ...styleSheet.filterIconColord, minWidth: "200px" }}
                    color="inherit"
                    variant="outlined"
                    onClick={() => {
                      setOpenSettlementModal(true);
                    }}
                  >
                    {
                      LanguageReducer?.languageType
                        ?.CREATE_SETTLEMENT_FROM_FILE_TEXT
                    }
                  </Button>
                ) : null}
                {tabScreen === "inventory" ? (
                  <>
                    <Button
                      sx={{
                        ...styleSheet.filterIcon,
                        minWidth: "100px",
                        marginLeft: "5px",
                      }}
                      variant="contained"
                      aria-controls={
                        openAction ? "demo-customized-menu" : undefined
                      }
                      aria-haspopup="true"
                      aria-expanded={openAction ? "true" : undefined}
                      disableElevation
                      onClick={handleClick}
                      endIcon={<KeyboardArrowDownIcon />}
                    >
                      {LanguageReducer?.languageType?.ACTION}
                    </Button>
                    <StyledMenu
                      id="demo-customized-menu"
                      MenuListProps={{
                        "aria-labelledby": "demo-customized-button",
                      }}
                      anchorEl={anchorElAction}
                      open={openAction}
                      onClose={handleCloseMenuOld}
                    >
                      <MenuItem
                        onClick={() => {
                          exportToXLSX();
                          handleCloseMenuOld();
                        }}
                        disableRipple
                      >
                        Export to Excel
                      </MenuItem>
                      <MenuItem
                        onClick={() => {
                          handleCloseMenuOld();
                          syncInventory();
                        }}
                        disableRipple
                      >
                        Sync Inventory
                      </MenuItem>
                    </StyledMenu>
                  </>
                ) : null}
              </>
            ) : null}
            {tabScreen === "user" ? (
              <Button
                sx={{ ...styleSheet.filterIconColord, minWidth: "110px" }}
                color="inherit"
                variant="outlined"
                onClick={() => {
                  setAddEmployee(true);
                }}
              >
                {"Add User"}
              </Button>
            ) : null}
            {tabScreen === "tax" ? (
              <Button
                sx={{ ...styleSheet.filterIconColord, minWidth: "110px" }}
                color="inherit"
                variant="outlined"
                onClick={() => {
                  setAddEmployee(true);
                }}
              >
                {LanguageReducer?.languageType?.SETTING_MANAGE_TAX_ADD_TAX}
              </Button>
            ) : null}
            {props.extraBtn}
            <Button
              sx={{
                ...styleSheet.filterIconColord,
                minWidth: {
                  md: "90px",
                  sm: "90px !important",
                  xs: "40px !important",
                },
              }}
              color="inherit"
              variant="outlined"
              onClick={() => setIsFilterOpen(!isFilterOpen)}
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
            {belowMdScreen && menuItems.length > 0 ? (
              <MenuIconComponent menuItems={menuItems} />
            ) : null}
          </Stack>
        </Box>
      </Box>
    </>
  );
};
export default GeneralTabBar;
