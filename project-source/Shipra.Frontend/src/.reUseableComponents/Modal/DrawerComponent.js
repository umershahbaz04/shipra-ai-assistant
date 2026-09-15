import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import CloseIcon from "@mui/icons-material/Close";
import { Divider, Typography } from "@mui/material";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Drawer from "@mui/material/Drawer";
import IconButton from "@mui/material/IconButton";
import React, { useState } from "react";
import { ActionButtonCustom } from "../../utilities/helpers/Helpers";
import { Danger } from "../../utilities/helpers/Colors";

const DrawerComponent = ({
  anchor = "right",
  open: propOpen,
  onClose,
  onClick,
  title,
  children,
  loading,
}) => {
  const [open, setOpen] = useState(propOpen || false);

  const handleToggleDrawer = (isOpen) => {
    setOpen(isOpen);
    if (!isOpen && onClose) {
      onClose();
    }
  };

  return (
    <Drawer
      anchor={anchor}
      open={open}
      onClose={() => handleToggleDrawer(false)}
    >
      <Box
        sx={{
          width: { md: "450px", sm: "400px", xs: "360px" },
          display: "flex",
          flexDirection: "column",
          height: "100%",
        }}
        role="presentation"
      >
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            padding: "8px",
            alignItems: "center",
          }}
        >
          <Typography fontSize={20} fontWeight={600}>
            {title}
          </Typography>
          <IconButton
            sx={{
              zIndex: 1,
            }}
            onClick={() => handleToggleDrawer(false)}
          >
            <CloseIcon />
          </IconButton>
        </Box>
        <Divider />
        <Box sx={{ flexGrow: 1, padding: 1 }}>
          {children || "Drawer Content"}
        </Box>
        <Divider />
        <Box
          sx={{
            display: "flex",
            justifyContent: "flex-end",
            padding: 1,
            paddingRight: "12px",
            gap: 1.5,
          }}
        >
          <Button
            sx={{
              padding: 0,
              textTransform: "capitalize",
              borderColor: Danger,
              color: Danger,
              "&:hover": {
                background: Danger,
                color: "#fff !important",
              },
            }}
            variant="outlined"
            onClick={() => handleToggleDrawer(false)}
          >
            Close
          </Button>
          {onClick && (
            <ActionButtonCustom
              label={"Proceed"}
              variant="contained"
              endIcon={<ArrowForwardIcon />}
              onClick={onClick}
              loading={loading}
            />
          )}
        </Box>
      </Box>
    </Drawer>
  );
};

export default DrawerComponent;
