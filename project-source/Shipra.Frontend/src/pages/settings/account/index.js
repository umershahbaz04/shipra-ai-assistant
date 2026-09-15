import React from "react";
import { Box, Container } from "@mui/material";
import { styleSheet } from "../../../assets/styles/style.js";
import GeneralTabBar from "../../../components/shared/tabsBar";
import { Route, Routes, useParams } from "react-router-dom";
import SettingProfile from "./profile/profile.js";
import Billings from "./billings/billings.js";
import { useSelector } from "react-redux";
import Payment from "./payment/Payment.js";

const TasksList = () => {
  return <Box>Under construction</Box>;
};
const PlansTab = () => {
  return <Box>UI not exist in Figma yet</Box>;
};
function AccountsSettingPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px 60px 30px 30px" }}>
        {" "}
        <GeneralTabBar
          tabData={[
            {
              label: "Plan",
              route: "/account",
            },
            {
              label: "Billing",
              route: "/account/billing",
            },
            { label: "Payment", route: "/account/payment" },
            {
              label: "Profit",
              route: "/account/profile",
            },
          ]}
          {...props}
          disableFilter
          disableSearch
          padding="0px 43px"
          width="auto"
        />{" "}
        <br />
        <Routes>
          <Route path="/" element={<PlansTab />} />
          <Route path="/billing" element={<Billings {...props} />} />
          <Route path="/payment" element={<Payment {...props} />} />
          <Route path="/profile" element={<SettingProfile {...props} />} />
        </Routes>
      </div>
    </Box>
  );
}
export default AccountsSettingPage;
