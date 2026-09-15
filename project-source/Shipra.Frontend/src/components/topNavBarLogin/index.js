import {
  Box,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  Skeleton,
  Stack,
} from "@mui/material";
import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { styleSheet } from "../../assets/styles/style";
import { updateLanguage } from "../../redux/language/index.js";
import { languagesOptions } from "../topNavBar/index.js";
import { useImageLoader } from "../../utilities/helpers/Helpers.js";
function TopNavBar(props) {
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [selectedLanguage, setSelectedLanguage] = React.useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const dispatch = useDispatch();
  const brandingData = sessionStorage.getItem("Branding");
  const decoded = brandingData ? decodeURIComponent(brandingData) : null;
  const Branding = decoded ? JSON?.parse(decoded) : decoded;
  const loaded = useImageLoader(Branding?.logoUrl);

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
  }, []);
  return (
    <Box sx={styleSheet.topNavBar}>
      <Box sx={styleSheet.topNavBarLeft}>
        <Box sx={{ position: "relative", width: "auto", maxWidth: 220, height: 50, display: "flex", alignItems: "center" }}>
          <Box
            component="img"
            src={(!Branding?.logoUrl || Branding?.logoUrl === "/logo.svg") ? "/logo.svg" : Branding?.logoUrl}
            alt="Brand Logo"
            sx={{
              height: "100%",
              width: "auto",
              objectFit: "contain",
            }}
          />
        </Box>
      </Box>
      <Stack
        sx={styleSheet.topNavBarRight}
        direction="row"
        justifyContent="flex-end"
        alignItems="center"
        spacing={1}
      >
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
            <img style={{ width: "23px" }} src={selectedLanguage.icon} />
            <Box sx={styleSheet.languageDropdownTitle}>
              {selectedLanguage?.code}
            </Box>
          </Stack>
        </IconButton>
      </Stack>
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
            {languagesOptions.map((lang) => (
              <ListItem disablePadding key={lang.name}>
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
    </Box>
  );
}
export default TopNavBar;
