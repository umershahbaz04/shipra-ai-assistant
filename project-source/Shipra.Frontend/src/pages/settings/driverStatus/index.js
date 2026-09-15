import { Backdrop, Box, CircularProgress, Grid } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { GetAllDriverCtssetting } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import DriverStatusList from "./driverStatusList";
import { ActionButtonCustom } from "../../../utilities/helpers/Helpers";
import AddDriverStatusModal from "../../../components/modals/settingsModals/AddDriverStatusModal";

function DriverStatusPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [load, setLoad] = useState(false);
  const [allDriverStatusData, setSllDriverStatusData] = useState();
  const getAllDriverStatus = async () => {
    setLoad(true);
    try {
      const response = await GetAllDriverCtssetting();
      setSllDriverStatusData(response?.data?.result);
    } catch (error) {
      console.error("Error in updating getting these carriers", error.response);
    } finally {
      setLoad(false);
    }
  };
  useEffect(() => {
    getAllDriverStatus();
  }, []);

  const [openAddDriverStatus, setOpenAddDriverStatus] = useState(false);
  const handleOpen = () => {
    setOpenAddDriverStatus(true);
  };
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <Grid
          sx={{
            border: "1px solid rgba(0, 0, 0, 0.12)",
            borderRadius: "8px",
          }}
        >
          <Grid
            sm={"12"}
            display={"flex"}
            alignSelf={"end"}
            justifyContent={"end"}
            padding={"10px"}
            sx={{
              background: "#f8f8f8",
              border: "1px none",
              borderRadius: "8px 8px 0px 0px",
            }}
          >
            <ActionButtonCustom
              onClick={() => handleOpen()}
              label={
                LanguageReducer?.languageType
                  ?.SETTING_DRIVER_STATUSES_ADD_DRIVER_STATUS
              }
            />
          </Grid>
          <Box>
            <DriverStatusList
              loading={load}
              allDriverStatusData={allDriverStatusData}
              getAllDriverStatus={getAllDriverStatus}
            />
          </Box>
        </Grid>
      </div>
      {openAddDriverStatus && (
        <AddDriverStatusModal
          open={openAddDriverStatus}
          setOpen={setOpenAddDriverStatus}
          exsitingStatues={allDriverStatusData?.list}
          getAllDriverStatus={getAllDriverStatus}
        />
      )}
    </Box>
  );
}
export default DriverStatusPage;
