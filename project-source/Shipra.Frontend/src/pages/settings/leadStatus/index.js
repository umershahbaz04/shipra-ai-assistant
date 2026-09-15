import { Box, Tab } from "@mui/material";
import { TabContext, TabList, TabPanel } from "@mui/lab";
import React, { useState, useRef } from "react";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import { PageMainBox } from "../../../utilities/helpers/Helpers";
import { ProfileDetailsBox } from "../../Profile/Profile/Profile";
import LeadTabsSetup from "./LeadTabsSetup";
import ClientLeadStatusSetup from "./ClientLeadStatusSetup";
import Colors from "../../../utilities/helpers/Colors";

function LeadStatusPage() {
  const [tabValue, setTabValue] = useState("1");
  const [isTabUpdating, setIsTabUpdating] = useState(false);

  const leadTabsRef = useRef();
  const clientStatusRef = useRef();

  const handleChangeTab = (event, newValue) => {
    setTabValue(newValue);
  };

  const handleRightBtn = () => {
    if (tabValue === "1") {
      return (
        <div style={{ display: "flex", gap: "10px" }}>
          <ButtonComponent
            title="Add New Tab"
            onClick={() => leadTabsRef.current?.triggerAddTab()}
          />
          <ButtonComponent
            title="Update Lead Tab"
            loading={isTabUpdating}
            onClick={() => leadTabsRef.current?.triggerUpdateTab()}
          />
        </div>
      );
    } else {
      return (
        <ButtonComponent
          title="Add New Status"
          onClick={() => clientStatusRef.current?.triggerAddNew()}
        />
      );
    }
  };

  return (
    <PageMainBox>
      <ProfileDetailsBox rightBtn={handleRightBtn()}>
        <TabContext value={tabValue}>
          <Box sx={{ borderBottom: 1, borderColor: "divider", mb: 2 }}>
            <TabList
              onChange={handleChangeTab}
              sx={{
                "& .MuiTabs-indicator": { backgroundColor: Colors.primary },
                "& .Mui-selected": { color: `${Colors.primary} !important` },
              }}
            >
              <Tab
                label="Lead Tabs Setup"
                value="1"
                disableRipple
                sx={{
                  textTransform: "none",
                  fontWeight: 600,
                  fontSize: "13px",
                  minWidth: "120px",
                }}
              />
              <Tab
                label="Lead Status Setup"
                value="2"
                disableRipple
                sx={{
                  textTransform: "none",
                  fontWeight: 600,
                  fontSize: "13px",
                  minWidth: "120px",
                }}
              />
            </TabList>
          </Box>
          <TabPanel value="1" sx={{ p: 0 }}>
            <LeadTabsSetup ref={leadTabsRef} setIsTabUpdating={setIsTabUpdating} />
          </TabPanel>
          <TabPanel value="2" sx={{ p: 0 }}>
            <ClientLeadStatusSetup ref={clientStatusRef} />
          </TabPanel>
        </TabContext>
      </ProfileDetailsBox>
    </PageMainBox>
  );
}

export default LeadStatusPage;
