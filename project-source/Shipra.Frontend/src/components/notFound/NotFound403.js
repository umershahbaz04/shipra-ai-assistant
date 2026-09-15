import React from "react";
import { PageMainBox, grey } from "../../utilities/helpers/Helpers";
import forbidden from "../../assets/images/forbidden.png";
import { Box, Typography } from "@mui/material";
import ButtonComponent from "../../.reUseableComponents/Buttons/ButtonComponent";
import { useNavigate } from "react-router-dom";
import { EnumRoutesUrls } from "../../utilities/enum";

export default function NotFound403() {
  const navigate = useNavigate();
  return (
    <PageMainBox>
      <Box
        className={"flex_center"}
        sx={{
          flexDirection: "column !important",
          gap: 1,
          bgcolor: grey,
          p: 2,
        }}
      >
        <Box className={"flex_center"}>
          <Box
            component={"img"}
            src={forbidden}
            width={"100%"}
            height={"100%"}
            maxWidth={50}
          />
          <Typography variant="h5">
            Are you lost you are not supposed to be here. if you need access
            contact to administrator.
          </Typography>
        </Box>
        <Box>
          <ButtonComponent
            title={"Go To Dasboard"}
            onClick={() => navigate(EnumRoutesUrls.ANALYTICS)}
          />
        </Box>
      </Box>
    </PageMainBox>
  );
}
