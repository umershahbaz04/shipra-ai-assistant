import { Typography } from "@material-ui/core";
import { Box } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";
import maintenancePic from "../../assets/images/maintenance pic.png";
import {
  grey,
  useClientSubscriptionReducer,
  useGetWindowHeight,
} from "../../utilities/helpers/Helpers";
import { EnumRoutesUrls } from "../../utilities/enum";

const Maintenance = () => {
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height } = useGetWindowHeight(clientSubscriptionData);
  const navigate = useNavigate();

  // setInterval(() => {
  //   if (JSON.parse(localStorage.isDeploying)) {
  //     navigate(EnumRoutesUrls.ANALYTICS);
  //   }
  // }, 2000);

  return (
    <Box
      display={"flex"}
      sx={{
        height: height,
      }}
    >
      <Box
        flexBasis={"100%"}
        className={"flex_center"}
        bgcolor={grey}
        sx={{ px: 2.5, flexDirection: "column", gap: 1 }}
      >
        <Box
          component={"img"}
          src={maintenancePic}
          width={"50%"}
          sx={{ borderRadius: "20px" }}
        />
        <Typography variant="h4" style={{ color: "rgb(57 67 180)" }}>
          We'll be back soon.
        </Typography>
      </Box>
    </Box>
  );
};

export default Maintenance;
