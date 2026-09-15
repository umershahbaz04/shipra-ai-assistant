import { LoadingButton } from "@mui/lab";
import { Box, Divider, Grid, InputLabel, Slide, Stack } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  DownloadInventoryHistorySummaryExcel,
  ExcelExportProductStockHistory,
  GetAllReasons,
  GetProductStockHistoryByStockId,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import {
  ClipboardIcon,
  CodeBox,
  centerColumn,
  usePagination,
} from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function StockHistoryModal({ open, handleClose, inventoryItem }) {
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    resetDates,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const gridApiRef = useRef(null);
  const [allReasons, setAllReasons] = useState([]);
  const [stockHistory, setStockHistory] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isLoadingSummary, setIsLoadingSummary] = useState(false);
  const [isLoadingHistory, setIsLoadingHistory] = useState(false);
  const [isExcelExport, setIsExcelExport] = useState(false);
  const [reasonId, setReasonId] = useState(0);

  const [quantity, setQuantity] = useState(0);

  const columns = [
    {
      field: "SKU",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SKU_TEXT}
        </Box>
      ),
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <CodeBox title={row.SKU} hasColor={false} />
            <ClipboardIcon text={row.SKU} />
          </>
        );
      },
    },
    {
      field: "Reason",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCT_TEXT}{" "}
          {LanguageReducer?.languageType?.REASON_TEXT}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "Comment",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.COMMENT_TEXT}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "CreatedOn",
      ...centerColumn,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.CREATED_ON_TEXT}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row?.CreatedOn)}
        </Box>
      ),
    },
    {
      field: "PreviousQuantity",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PREVIOUS_QUANTITY_TEXT}
        </Box>
      ),
      flex: 1,
      ...centerColumn,
      renderCell: ({ row }) => {
        return (
          <>
            <CodeBox title={row.PreviousQuantity} hasColor={false} />
          </>
        );
      },
    },
    {
      field: "NewQuantity",
      headerName: <Box sx={{ fontWeight: "600" }}> {"New Quantity"}</Box>,
      flex: 1,
      ...centerColumn,
      renderCell: ({ row }) => {
        return (
          <>
            <CodeBox title={row.NewQuantity} hasColor={false} />
          </>
        );
      },
    },
  ];

  const handleGetAllReasons = async () => {
    try {
      const response = await GetAllReasons();
      setAllReasons(response.data.result);
    } catch (error) {
      console.error("Error fetching reasons:", error.response);
    }
  };
  const downloadInventoryHistorySummaryExcel = async () => {
    setIsLoadingSummary(true);
    try {
      let params = {
        filterModel: {
          createdFrom: startDateFormated ? startDateFormated : null,
          createdTo: endDateFormated ? endDateFormated : null,
          start: 0,
          length: 10000,
          search: "",
          sortDir: "desc",
          sortCol: 0,
        },
        productStockId: inventoryItem?.ProductStockId,
        reasonId: reasonId?.id,
      };
      console.log("params", params);
      const response = await DownloadInventoryHistorySummaryExcel(params);
      console.log(
        "response of api of downloadInventoryHistorySummaryExcel",
        response
      );
      const blob = new Blob([response.data], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      const downloadUrl = URL.createObjectURL(blob);

      const link = document.createElement("a");
      link.href = downloadUrl;
      link.download = `Summary_${Date.now()}.xlsx`;
      link.click();
      URL.revokeObjectURL(downloadUrl);
      setIsLoadingSummary(false);
    } catch (error) {
      console.error(
        "Error fetching downloadInventoryHistorySummaryExcel:",
        error.response
      );
      setIsLoadingSummary(false);
    }
  };

  const excelExportProductStockHistory = async () => {
    setIsExcelExport(true);
    try {
      let params = {
        filterModel: {
          createdFrom: startDateFormated ? startDateFormated : null,
          createdTo: endDateFormated ? endDateFormated : null,
          start: 0,
          length: 10000,
          search: "",
          sortDir: "desc",
          sortCol: 0,
        },
        productStockId: inventoryItem?.ProductStockId,
        reasonId: reasonId?.id,
      };
      console.log("params", params);
      const response = await ExcelExportProductStockHistory(params);
      setIsExcelExport(false);

      console.log(
        "response of api of ExcelExportProductStockHistory",
        response
      );
      const blob = new Blob([response.data], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      const downloadUrl = URL.createObjectURL(blob);

      const link = document.createElement("a");
      link.href = downloadUrl;
      link.download = `History_${Date.now()}.xlsx`;
      link.click();

      URL.revokeObjectURL(downloadUrl);
    } catch (error) {
      console.error(
        "Error fetching ExcelExportProductStockHistory:",
        error.response
      );
    }
  };

  const getProductStockHistoryByStockId = async (data) => {
    setIsLoading(true);
    setIsLoadingHistory(true);

    let params = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 10000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      productStockId: inventoryItem?.ProductStockId,
      reasonId: reasonId?.id,
    };
    const response = await GetProductStockHistoryByStockId(params);

    if (response.data.result) setStockHistory(response.data.result.list);
    setIsLoading(false);
    setIsLoadingHistory(false);
  };

  useEffect(() => {
    handleGetAllReasons();
    getProductStockHistoryByStockId();
  }, []);
  useEffect(() => {
    if (allReasons && allReasons.length > 0) {
      setReasonId(allReasons[0]);
    }
  }, [allReasons]);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  // Inside your component
  // const divRef = useRef(null);
  // const [innerHeight, setInnerHeight] = useState(0);

  // // Function to update inner height
  // const updateInnerHeight = () => {
  //   if (divRef.current) {
  //     setInnerHeight(divRef.current.clientHeight);
  //   }
  // };

  // // Update inner height on component mount and when component updates
  // useEffect(() => {
  //   updateInnerHeight();
  // }, []); // Run only once on component mount

  // useEffect(() => {
  //   // Add event listener to update inner height when window resizes
  //   window.addEventListener('resize', updateInnerHeight);
  //   return () => {
  //     // Cleanup function to remove event listener
  //     window.removeEventListener('resize', updateInnerHeight);
  //   };
  // }, []); // Run only once on component mount
  // console.log(innerHeight)
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="xl"
      title={LanguageReducer?.languageType?.STOCK_HISTORY_TEXT}
      height={"80vh"}
    >
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          height: "100%",
          maxHeight: "100vh",
        }}
      >
        <Grid
          container
          spacing={2}
          // ref={divRef}
        >
          <Grid item sm={12}>
            <Divider />
          </Grid>
          <Grid item xs={12} sm={12} md={2}>
            <InputLabel sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.CREATED_FROM_DATE_TEXT}
            </InputLabel>

            <CustomReactDatePickerInput
              value={startDate}
              onClick={(date) => setStartDate(date)}
              size="small"
              isClearable
            />
          </Grid>
          <Grid item xs={12} sm={12} md={2}>
            <InputLabel sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.CREATED_TO_DATE_TEXT}
            </InputLabel>
            <CustomReactDatePickerInput
              value={endDate}
              onClick={(date) => setEndDate(date)}
              width="180px"
              size="small"
              minDate={startDate}
              disabled={!startDate ? true : false}
              isClearable
            />
          </Grid>
          <Grid item xs={12} sm={12} md={2}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.REASON_TEXT}
            </InputLabel>
            <SelectComponent
              name="reason"
              options={allReasons}
              value={reasonId}
              optionLabel={EnumOptions.SELECTED_REASON.LABEL}
              optionValue={EnumOptions.SELECTED_REASON.VALUE}
              isRefesh={true}
              handleRefreshClick={handleGetAllReasons}
              onChange={(e, value) => {
                setReasonId(value);
              }}
            />
          </Grid>
          <Grid item md={6} xs={12}>
            <Stack
              direction="row"
              justifyContent="flex-end"
              alignItems={"flex-end"}
              height={"100%"}
            >
              <LoadingButton
                loading={isLoading}
                variant="contained"
                sx={{ ...styleSheet.addCarrierButton }}
                type="submit"
                onClick={getProductStockHistoryByStockId}
              >
                {LanguageReducer?.languageType?.LOAD_TEXT}
              </LoadingButton>
              <LoadingButton
                loading={isLoadingSummary}
                variant="contained"
                sx={{ ...styleSheet.addCarrierButton }}
                onClick={downloadInventoryHistorySummaryExcel}
              >
                {LanguageReducer?.languageType?.EXPORT_SUMMARY_TEXT}
              </LoadingButton>
              <LoadingButton
                loading={isExcelExport}
                variant="contained"
                sx={{ ...styleSheet.addCarrierButton }}
                onClick={excelExportProductStockHistory}
              >
                {LanguageReducer?.languageType?.EXPORT_HISTORY_TEXT}
              </LoadingButton>
            </Stack>
          </Grid>
        </Grid>
        {/* <Grid item container spacing={1} sm={12} sx={{ mt: "5px" }}>
                <Grid item xs={3}>
                  <InputLabel sx={styleSheet.inputLabel}>
                    {LanguageReducer?.languageType?.QTY_COMMITED_TEXT}
                  </InputLabel>
                  <Typography variant="p">{0}</Typography>
                </Grid>
                <Grid item xs={3}>
                  <InputLabel sx={styleSheet.inputLabel}>
                    {LanguageReducer?.languageType?.QTY_INCOMING_TEXT}
                  </InputLabel>
                  <Typography variant="p">{0}</Typography>
                </Grid>
                <Grid item xs={3}>
                  <InputLabel sx={styleSheet.inputLabel}>
                    {LanguageReducer?.languageType?.QTY_AVAILABLE_TEXT}
                  </InputLabel>
                  <Typography variant="p">{0}</Typography>
                </Grid>
                <Grid item xs={3}>
                  <InputLabel sx={styleSheet.inputLabel}>
                    {LanguageReducer?.languageType?.QTY_DAMAGE_TEXT}
                  </InputLabel>
                  <Typography variant="p">{0}</Typography>
                </Grid>
              </Grid> */}

        <DataGrid
          loading={isLoadingHistory}
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "400",
            backgroundColor: "white",
            //  height: `calc(80vh - ${innerHeight}px)`,
            marginTop: "10px",
            // overflowY: "scroll",
            // flexGrow:1
          }}
          getRowId={(row) => row.ProductStockHistorytId}
          rows={stockHistory}
          columns={columns}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 15, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          apiRef={gridApiRef}
          onGridReady={(params) => (gridApiRef.current = params.api)}
        />
      </Box>
    </ModalComponent>
  );
}
export default StockHistoryModal;
