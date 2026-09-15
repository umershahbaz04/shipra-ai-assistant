import MuiAppBar from "@mui/material/AppBar";
import Backdrop from "@mui/material/Backdrop";
import Box from "@mui/material/Box";
import CssBaseline from "@mui/material/CssBaseline";
import Toolbar from "@mui/material/Toolbar";
import { styled, useTheme } from "@mui/material/styles";
import PropTypes from "prop-types";
import * as React from "react";
import { useSelector } from "react-redux";
import { Navigate, useLocation } from "react-router-dom";
import useWindowDimensions from "../../utilities/customHooks/useWindowDimensions";
import TopNavBarLogin from "../topNavBarLogin";
// import { useLocation } from "react-router-dom";
import useMediaQuery from "@mui/material/useMediaQuery";
import { styleSheet } from "../../assets/styles/style.js";
import MainLoader from "../loader/mainLoader";
import UnauthorizedPage from "../unAuthorised";

const drawerWidth = 256;

const DrawerHeader = styled("div")(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  justifyContent: "flex-end",
  padding: theme.spacing(0, 1),
  // necessary for content to be below app bar
  ...theme.mixins.toolbar,
}));
const AppBar = styled(MuiAppBar, {
  shouldForwardProp: (prop) => prop !== "open",
})(({ theme, open }) => ({
  zIndex: theme.zIndex.drawer + 1,
  transition: theme.transitions.create(["width", "margin"], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  background: "#f8f8f8",
  ...(open && {
    marginLeft: drawerWidth,
    width: `calc(100% - ${drawerWidth}px)`,
    transition: theme.transitions.create(["width", "margin"], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  }),
}));

function PrivateRoutes({ children, type, ...rest }) {
  const { window, docs } = rest;
  const [mobileOpen, setMobileOpen] = React.useState(false);
  const [pcOpen, setPcOpen] = React.useState(true);
  const location = useLocation();
  const path = location.pathname.split("/")[1];
  const { width } = useWindowDimensions();
  const theme = useTheme();
  const matches = useMediaQuery(theme.breakpoints.down("xl"));
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  React.useEffect(() => {
    if (matches) {
      setPcOpen(false);
    }
  }, [matches]);

  const adminAuth = {
    isLoading: false,
    isPermitted: true,
  }; /* useSelector((state) => state.adminAuth); */
  const container =
    window !== undefined ? () => window().document.body : undefined;
  if (true) {
    return (
      <Box
        sx={{
          ...styleSheet["@global"][".auth-app-bar .MuiToolbar-root"],
          display: "flex",
        }}
      >
        <CssBaseline />
        <AppBar
          position="fixed"
          color="default"
          enableColorOnDark
          evaluation={0}
          sx={{
            width: { sm: `calc(100%)` },
            ml: { sm: `${pcOpen ? drawerWidth : 60}px` },
            boxShadow: "none",
            zIndex: "1",
          }}
        >
          <Toolbar>
            <TopNavBarLogin />
          </Toolbar>
        </AppBar>

        <Box
          component="main"
          sx={{
            flexGrow: 1,
            p: 3,
            position: "relative",
            width: { sm: `calc(100% - ${drawerWidth}px)` },
            backgroundColor: "white",
            /*minHeight: "100vh",*/
            padding: "0 0 !important",
            paddingLeft: "0px",
            paddingRight: "0px",
            marginTop: "64px",
            width: "100%!important",
          }}
        >
          {adminAuth.isLoading ? (
            <div>
              <br />
              <br />
              <br />
              <br />
              <br />
              <Box
                sx={{
                  p: 8,
                  width: "100%",
                  display: "flex",
                  justifyContent: "center",
                }}
              >
                {/* Loading Please wait... */}
              </Box>
            </div>
          ) : adminAuth.isPermitted ? (
            children
          ) : (
            <UnauthorizedPage />
          )}
        </Box>
        <Backdrop
          sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
          open={adminAuth.isLoading}
        >
          <MainLoader />
        </Backdrop>
      </Box>
    );
  } else {
    return <Navigate to={`/login`} />;
  }
}

PrivateRoutes.propTypes = {
  /**
   * Injected by the documentation to work in an iframe.
   * You won't need it on your project.
   */
  window: PropTypes.func,
};

export default PrivateRoutes;
