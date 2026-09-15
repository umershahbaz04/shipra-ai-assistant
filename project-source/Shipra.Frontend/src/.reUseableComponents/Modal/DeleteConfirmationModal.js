import { Box, Typography } from "@mui/material";
import React from "react";
import ConfirmationIcon from "../../assets/images/confirmationIcon.png";
import { Danger } from "../../utilities/helpers/Colors";
import ModalButtonComponent from "../Buttons/ModalButtonComponent";
import ModalComponent from "./ModalComponent";

function DeleteConfirmationModal(props) {
  let {
    open,
    setOpen = () => {},
    onClose = () => {},
    loading = false,
    handleDelete = () => {},
    buttonColor = Danger,
    heading = "Are you sure you want to deactivate this carrier?",
    message = "This carrier will be deactivate immediately. You can't undo this action.",
    messageDetails = "",
    buttonText = "Deactivate",
  } = props;

  const handleClose = () => {
    onClose();
    setOpen(false);
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      actionBtn={
        <ModalButtonComponent
          title={buttonText}
          loading={loading}
          bg={buttonColor}
          onClick={() => {
            handleDelete();
            if (props?.setIsDeletedConfirm) {
              props?.setIsDeletedConfirm(true);
            }
          }}
        />
      }
    >
      <Box display={"flex"} gap={1}>
        <Box width={100} height={100}>
          <Box
            component={"img"}
            width={"100%"}
            height={"100%"}
            sx={{ objectFit: "contain" }}
            src={ConfirmationIcon}
          />
        </Box>
        <Box alignSelf={"center"}>
          <Typography variant="h3">{heading}</Typography>
          <Typography variant="h5" fontWeight={300} mt={0.25}>
            {message + messageDetails}
          </Typography>
        </Box>
      </Box>
    </ModalComponent>
  );
}
export default DeleteConfirmationModal;
