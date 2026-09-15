import { Box, Grid } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { GetAllCarrierTrackingStatusForSelection } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { ActionButtonCustom } from "../../../utilities/helpers/Helpers";
import { DataGrid } from "@mui/x-data-grid";
import {
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import CreateCarrierStatusModal from "../../../components/modals/settingsModals/CreateCarrierStatusModal";

function CarrierStatusPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [load, setLoad] = useState(false);
  const [allCarrierStatusData, setAllCarrierStatusData] = useState([]);
  
  const getAllCarrierStatus = async () => {
    setLoad(true);
    try {
      const response = await GetAllCarrierTrackingStatusForSelection();
      let sortedData = response?.data?.result || [];
      sortedData.sort((a, b) => b.carrierTrackingStatusId - a.carrierTrackingStatusId);
      setAllCarrierStatusData(sortedData);
    } catch (error) {
      console.error("Error in getting carrier statuses", error.response);
    } finally {
      setLoad(false);
    }
  };

  useEffect(() => {
    getAllCarrierStatus();
  }, []);

  const [openAddModal, setOpenAddModal] = useState(false);
  const handleOpen = () => {
    setOpenAddModal(true);
  };

  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } = usePagination(0, 10);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);

  const columns = [
    {
      field: "trackingStatus",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          Status Name
        </Box>
      ),
      minWidth: 150,
      flex: 1,
    }
  ];

  const calculatedHeight = windowHeight - navbarHeight - 70;

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
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
              label="Add Carrier Status"
            />
          </Grid>
          <Box
            sx={{
              ...styleSheet.allOrderTable,
              height: calculatedHeight,
            }}
          >
            <DataGrid
              loading={load}
              rowHeight={40}
              headerHeight={40}
              sx={{
                fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
                fontSize: "12px",
                fontWeight: "500",
              }}
              getRowId={(row) => row?.carrierTrackingStatusId}
              rows={allCarrierStatusData}
              columns={columns}
              disableSelectionOnClick
              pagination
              page={currentPage}
              pageSize={pageSize}
              rowsPerPageOptions={[5, 10, 15, 25]}
              paginationMode="client"
              onPageChange={handlePageChange}
              onPageSizeChange={handlePageSizeChange}
            />
          </Box>
        </Grid>
      </div>
      {openAddModal && (
        <CreateCarrierStatusModal
          open={openAddModal}
          setOpen={setOpenAddModal}
          getAllCarrierStatus={getAllCarrierStatus}
        />
      )}
    </Box>
  );
}
export default CarrierStatusPage;
