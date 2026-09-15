import React, { useState } from "react";
import Button from "@mui/material/Button";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import MoreVertIcon from "@mui/icons-material/MoreVert";

const MenuIconComponent = ({ menuItems }) => {
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <div>
      <Button
        id="menu-icon-button"
        aria-controls={open ? "menu-icon" : undefined}
        aria-haspopup="true"
        aria-expanded={open ? "true" : undefined}
        onClick={handleClick}
        sx={{ padding: "0px!important", minWidth: "30px" }}
      >
        {<MoreVertIcon />}
      </Button>
      <Menu
        id="menu-icon"
        aria-labelledby="menu-icon-button"
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        sx={{
          "& .MuiList-root": {
            padding: 0,
          },
        }}
        anchorOrigin={{
          vertical: "top",
          horizontal: "left",
        }}
        transformOrigin={{
          vertical: "top",
          horizontal: "left",
        }}
      >
        {menuItems.map((item, index) => (
          <MenuItem
            key={index}
            sx={{
              padding: "0px, 18px !important",
              minHeight: "15px",
              justifyContent: "center",
            }}
            onClick={() => {
              handleClose();
              if (item.onClick) item.onClick();
            }}
          >
            {item.label}
          </MenuItem>
        ))}
      </Menu>
    </div>
  );
};

export default MenuIconComponent;
