import { usePagination } from "@mui/lab";
import {
  Box,
  Button,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import useDateRangeHook from "../../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import DataGridTabs from "../../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import CustomReactDatePickerInputFilter from "../../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import { styleSheet } from "../../../../assets/styles/style";
import {
  centerColumn,
  GridContainer,
  GridItem,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import UtilityClass from "../../../../utilities/UtilityClass";
import { GetAllOrdersByPayoutId } from "../../../../api/AxiosInterceptors";

const PayoutOrderModal = (props) => {
  const { open, onClose, payoutOrderData, payoutId } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const calculatedHeight = windowHeight - 273;
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isfilterClear, setIsfilterClear] = useState(false);

  const handleFilter = () => {
    GetAllOrdersByPayoutId(startDate, endDate, payoutId);
  };

  const handleFilterClear = async () => {
    setIsfilterClear(true);
    resetDates();
  };

  const columns = [
    {
      field: "OrderNo",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"OrderNo"}</Box>,
      minWidth: 100,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box
            sx={{
              fontWeight: "bold",
            }}
          >
            <Box>{row.OrderNo}</Box>
          </Box>
        );
      },
    },
    {
      field: "CustomerName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Customer Name"}</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{row.CustomerName}</Box>;
      },
    },
    {
      field: "Mobile1",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Mobile No."}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box sx={{ fontWeight: "bold" }}>{row.Mobile1}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "Remarks",
      minWidth: 140,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Remarks"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{row?.Remarks}</Box>;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 140,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Created On"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row?.CreatedOn)}</Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Amount",
      minWidth: 140,
      headerName: <Box sx={{ fontWeight: "600" }}> {"Amount"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{row?.Amount}</Box>;
      },
      flex: 1,
    },
  ];

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="lg"
      title={"Transction Order Modal"}
    >
      <DataGridTabs
        tabsSmWidth={"0px"}
        tabsMdWidth={"0px"}
        filterData={
          isFilterOpen ? (
            <Table
              sx={{ ...styleSheet.generalFilterArea }}
              size="small"
              aria-label="a dense table"
            >
              <TableHead>
                <TableRow>
                  <GridContainer spacing={1} p={1}>
                    <GridItem xs={12} sm={6} md={4} lg={4}>
                      <InputLabel
                        sx={{
                          ...styleSheet.inputLabel,
                          overflow: "unset",
                        }}
                      >
                        {LanguageReducer?.languageType?.ORDER_START_DATE}
                      </InputLabel>
                      <CustomReactDatePickerInputFilter
                        value={startDate}
                        onClick={(date) => setStartDate(date)}
                        size="small"
                        isClearable
                        maxDate={UtilityClass.todayDate()}
                      />
                    </GridItem>
                    <GridItem xs={12} sm={6} md={4} lg={4}>
                      <InputLabel
                        sx={{
                          ...styleSheet.inputLabel,
                          overflow: "unset",
                        }}
                      >
                        {LanguageReducer?.languageType?.ORDER_END_DATE}
                      </InputLabel>
                      <CustomReactDatePickerInputFilter
                        value={endDate}
                        onClick={(date) => setEndDate(date)}
                        size="small"
                        minDate={startDate}
                        disabled={!startDate ? true : false}
                        isClearable
                        maxDate={UtilityClass.todayDate()}
                      />
                    </GridItem>
                    <GridItem xs={12} sm={6} md={4} lg={4}>
                      <Stack
                        alignItems="flex-end"
                        direction="row"
                        spacing={1}
                        sx={{
                          ...styleSheet.filterButtonMargin,
                          height: "100%",
                        }}
                      >
                        <Button
                          sx={{
                            ...styleSheet.filterIcon,
                            minWidth: "100px",
                          }}
                          color="inherit"
                          variant="outlined"
                          onClick={() => {
                            handleFilterClear();
                          }}
                        >
                          {LanguageReducer?.languageType?.ORDER_CLEAR_FILTER}
                        </Button>
                        <Button
                          sx={{
                            ...styleSheet.filterIcon,
                            minWidth: "100px",
                          }}
                          variant="contained"
                          onClick={handleFilter}
                        >
                          {LanguageReducer?.languageType?.ORDERS_FILTER}
                        </Button>
                      </Stack>
                    </GridItem>
                  </GridContainer>
                </TableRow>
              </TableHead>
            </Table>
          ) : null
        }
        handleFilterBtnOnClick={() => {
          setIsFilterOpen(!isFilterOpen);
        }}
      />
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: calculatedHeight,
        }}
      >
        <DataGrid
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row.RowNum}
          rows={payoutOrderData.list || []}
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
    </ModalComponent>
  );
};

export default PayoutOrderModal;
