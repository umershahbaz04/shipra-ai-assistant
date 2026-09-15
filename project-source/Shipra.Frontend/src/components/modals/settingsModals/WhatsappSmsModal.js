import WhatsAppIcon from "@mui/icons-material/WhatsApp";
import { Card, CardContent, Grid, IconButton, Typography } from "@mui/material";
import React from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import {
  ClipboardIcon,
  useWhatsAppMsg,
  WhatsAppMsg,
} from "../../../utilities/helpers/Helpers";

const WhatsappSmsModal = (props) => {
  const { open, onClose, dataForWhatsappSms } = props;
  const { whatsAppMsgRef, handleSendWhatsAppMsg } = useWhatsAppMsg();
  const handleWhatsappClick = () => {
    handleSendWhatsAppMsg();
  };

  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="sm"
        title={"Send Sms"}
      >
        <Grid container spacing={2} mt={2}>
          <Grid item xs={12}>
            <Card
              elevation={3}
              sx={{ backgroundColor: "#fff", padding: "16px" }}
            >
              <CardContent>
                <Typography variant="h6" fontWeight="bold" gutterBottom>
                  Login Credentials:
                </Typography>
                <Typography variant="body1" gutterBottom>
                  <strong>Username:</strong>{" "}
                  {dataForWhatsappSms?.userName || "N/A"}
                  <ClipboardIcon text={dataForWhatsappSms?.userName} />
                </Typography>
                <Typography variant="body1">
                  <strong>Password:</strong>{" "}
                  {dataForWhatsappSms?.password || "N/A"}
                  <ClipboardIcon text={dataForWhatsappSms?.password} />
                </Typography>
              </CardContent>
            </Card>
          </Grid>

          <Grid
            item
            xs={12}
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              mt: 2,
            }}
          >
            <Typography variant="h6" sx={{ fontWeight: "600" }}>
              Share this message with the employee via WhatsApp:
            </Typography>
            <IconButton
              onClick={handleWhatsappClick}
              sx={{
                color: "#25D366",
                backgroundColor: "white",
                "&:hover": { backgroundColor: "#e0f7ec" },
                boxShadow: "0px 4px 6px rgba(0, 0, 0, 0.1)",
              }}
            >
              <WhatsAppIcon fontSize="large" />
            </IconButton>
          </Grid>
        </Grid>
      </ModalComponent>
      <WhatsAppMsg
        phoneNumber={dataForWhatsappSms?.mobile}
        whatsAppMsgRef={whatsAppMsgRef}
        message={`Hello *${dataForWhatsappSms?.employeeName}*,\n\nWelcome to **Shipra**\n\nHere are your login credentials:\n*Username:* ${dataForWhatsappSms?.userName}\n*Password:* ${dataForWhatsappSms?.password}\n*Website URL:* ${process.env.REACT_APP_URL}\n\nPlease keep this information secure. If you have any questions, feel free to reach out to our *support team*.`}
      />
    </>
  );
};

export default WhatsappSmsModal;
