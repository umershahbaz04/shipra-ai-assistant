import React from "react";
import PropTypes from "prop-types";
import { alpha, styled } from "@mui/material/styles";
import { Toolbar, Typography, Button, Menu, MenuItem } from "@mui/material";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import EditIcon from "@mui/icons-material/Edit";
import { useSelector } from "react-redux";

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

export default function EnhancedTableToolbar(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const {
    numSelected,
    handleOpenPrices,
    handleOpenQuantities,
    handleOpenSKU,
    handleCloseQuantities,
    handleCloseSKU,
    handleClosePrices,
    handleOpenVariantAttributes,
  } = props;
  
  const [anchorEl, setAnchorEl] = React.useState(null);
  const open = Boolean(anchorEl);
  
  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <Toolbar
      sx={{
        pl: { sm: 2 },
        pr: { xs: 1, sm: 1 },
        ...(numSelected > 0 && {
          bgcolor: (theme) =>
            alpha(
              theme.palette.primary.main,
              theme.palette.action.activatedOpacity
            ),
        }),
      }}
    >
      {numSelected > 0 ? (
        <Typography
          sx={{ flex: "1 1 100%" }}
          color="inherit"
          variant="subtitle1"
          component="div"
        >
          {numSelected} {LanguageReducer?.languageType?.SELECTED_TEXT || "Selected"}
        </Typography>
      ) : (
        <Typography
          sx={{ flex: "1 1 100%" }}
          color="inherit"
          variant="subtitle1"
          component="div"
        >
          {0} {LanguageReducer?.languageType?.SELECTED_TEXT || "Selected"}
        </Typography>
      )}

      {numSelected > 0 ? (
        <div>
          <Button
            id="demo-customized-button"
            aria-controls={open ? "demo-customized-menu" : undefined}
            aria-haspopup="true"
            aria-expanded={open ? "true" : undefined}
            variant="contained"
            disableElevation
            onClick={handleClick}
            endIcon={<KeyboardArrowDownIcon />}
            sx={{
              background: "var(--primary-color)",
              fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
              textTransform: "capitalize  !important",
            }}
          >
            {LanguageReducer?.languageType?.EDIT_TEXT || "Edit"}
          </Button>
          <StyledMenu
            id="demo-customized-menu"
            MenuListProps={{
              "aria-labelledby": "demo-customized-button",
            }}
            anchorEl={anchorEl}
            open={open}
            onClose={handleClose}
          >
            <MenuItem
              onClick={() => {
                handleOpenPrices();
                handleCloseSKU();
                handleCloseQuantities();
                setAnchorEl(null);
              }}
              disableRipple
            >
              <EditIcon />
              {LanguageReducer?.languageType?.EDIT_PRICES_TEXT || "Edit Prices"}
            </MenuItem>
            <MenuItem
              onClick={() => {
                handleOpenSKU();
                handleClosePrices();
                handleCloseQuantities();
                setAnchorEl(null);
              }}
              disableRipple
            >
              <EditIcon />
              {LanguageReducer?.languageType?.EDIT_SKU_TEXT || "Edit SKU"}
            </MenuItem>
            <MenuItem
              onClick={() => {
                handleOpenQuantities();
                handleCloseSKU();
                handleClosePrices();
                setAnchorEl(null);
              }}
              disableRipple
            >
              <EditIcon />
              {LanguageReducer?.languageType?.EDIT_QUNATITIES_TEXT || "Update Quantity"}
            </MenuItem>
            <MenuItem
              onClick={() => {
                if (handleOpenVariantAttributes) handleOpenVariantAttributes();
                handleCloseSKU();
                handleClosePrices();
                handleCloseQuantities();
                setAnchorEl(null);
              }}
              disableRipple
            >
              <EditIcon />
              Edit Advanced Attributes
            </MenuItem>
          </StyledMenu>
        </div>
      ) : null}
    </Toolbar>
  );
}

EnhancedTableToolbar.propTypes = {
  numSelected: PropTypes.number.isRequired,
};
