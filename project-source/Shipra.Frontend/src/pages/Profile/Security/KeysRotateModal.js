import React from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple, amber } from "@mui/material/colors";
import { Box, Typography } from "@mui/material";

const KeysRotateModal = (props) => {
  const { open, onClose, isLoading } = props;
  return (
    <div>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth={"sm"}
        title={"Client Keys"}
        actionBtn={
          <ModalButtonComponent
            title={"Save"}
            loading={isLoading}
            bg={purple}
            type="submit"
          />
        }
      >
        <Box
          sx={{
            backgroundColor: amber[100],
            border: `1px solid ${amber[700]}`,
            borderRadius: "4px",
            padding: "16px",
            marginBottom: "20px",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Typography variant="h6" color="textPrimary">
            Your connection will be lost. Please save your work before
            proceeding.
          </Typography>
        </Box>
      </ModalComponent>
    </div>
  );
};

export default KeysRotateModal;
