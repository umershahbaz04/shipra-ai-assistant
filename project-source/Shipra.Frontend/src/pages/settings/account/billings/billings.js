import AddBoxIcon from "@mui/icons-material/AddBox";
import { Box, Button, Card, Divider, Grid, IconButton, InputLabel, TextField, Typography } from "@mui/material";
import { Stack } from "@mui/system";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import american_express from "../../../../assets/images/american_express.png";
import brandico_visa from "../../../../assets/images/brandico_visa.png";
import mastercard from "../../../../assets/images/mastercard.png";
import paypalIcon from "../../../../assets/images/paypal.png";
import swift from "../../../../assets/images/swift.png";
import western_union from "../../../../assets/images/western_union.png";
import { styleSheet } from "../../../../assets/styles/style.js";

function Billings(props) {
  const [values, setValues] = useState({});
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  return (
    <Box sx={styleSheet.profileSettingArea}>
      <Typography sx={styleSheet.paymentMethodHeading} variant="h5">
        Payment Method
      </Typography>
      <Typography sx={styleSheet.paymentMethodText}>Add a credit card to get started with Shippo.</Typography>
      <Card variant="outlined" sx={styleSheet.paymentMethodCardArea}>
        <Stack direction={"row"} spacing={2}>
          <IconButton sx={{ padding: "0px" }}><img src={paypalIcon} alt="paypalIcon" /></IconButton>
          <IconButton sx={{ padding: "0px" }}><img src={western_union} alt="western_union" /></IconButton>
          <IconButton sx={{ padding: "0px" }}><img src={mastercard} alt="mastercard" /></IconButton>
          <IconButton sx={{ padding: "0px" }}><img src={swift} alt="swift" /></IconButton>
          <IconButton sx={{ padding: "0px" }}><img src={american_express} alt="american_express" /></IconButton>
          <IconButton sx={{ padding: "0px" }}><img src={brandico_visa} alt="brandico_visa" /></IconButton>
        </Stack>
        <Button sx={styleSheet.addAccountButton}>
          <AddBoxIcon fontSize="small" />
        </Button>
      </Card>
      <Box sx={{ mb: "15px" }}>
        <InputLabel sx={styleSheet.inputLabel}>{LanguageReducer?.languageType?.CARD_NUMBER_TEXT}</InputLabel>
        <TextField placeholder={"Card number"} size="small" fullWidth variant="outlined" />
      </Box>
      <Box sx={{ mb: "15px" }}>
        <InputLabel sx={styleSheet.inputLabel}>CVC</InputLabel>
        <TextField placeholder="CVC" size="small" fullWidth variant="outlined" />
      </Box>
      <Grid container spacing={2} sx={{ mb: "15px" }}>
        <Grid item sm={12} md={6}>
          <InputLabel sx={styleSheet.inputLabel}>Exp Month</InputLabel>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DatePicker views={["month"]} renderInput={(params) => <TextField size="small" {...params} helperText={null} />} />
          </LocalizationProvider>
        </Grid>
        <Grid item sm={12} md={6}>
          {" "}
          <InputLabel sx={styleSheet.inputLabel}>Exp Year</InputLabel>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DatePicker views={["year"]} renderInput={(params) => <TextField size="small" {...params} helperText={null} />} />
          </LocalizationProvider>
        </Grid>
      </Grid>
      <Divider />
      <br />
      <Typography sx={styleSheet.paymentMethodHeading} variant="h5">
        Promo Code
      </Typography>
      <Typography sx={styleSheet.paymentMethodText}>Have a promo code? Lucky you! Enter it here:</Typography>
      <Box sx={{ mb: "15px" }}>
        <TextField placeholder="Enter Promo Code" size="small" fullWidth variant="outlined" />
      </Box>
      <Button sx={styleSheet.saveButtonUI} variant="contained" fullWidth>
        {LanguageReducer?.languageType?.APPLY_TEXT}
      </Button>
    </Box>
  );
}
export default (Billings);
