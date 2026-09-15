import React, { useEffect, useState } from "react";
import {
  ActionButtonCustom,
  centerColumn,
  GridContainer,
  GridItem,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import {
  Box,
  Button,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
  Tooltip,
} from "@mui/material";
import CustomReactDatePickerInputFilter from "../../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import { DataGrid } from "@mui/x-data-grid";
import UtilityClass from "../../../../utilities/UtilityClass";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import { useSelector } from "react-redux";
import { usePagination } from "@mui/lab";
import useDateRangeHook from "../../../../.reUseableComponents/CustomHooks/useDateRangeHook";
import { styleSheet } from "../../../../assets/styles/style";
import FileDownloadOutlinedIcon from "@mui/icons-material/FileDownloadOutlined";
import {
  ExcelExportAllTransaction,
  GetAllTransaction,
} from "../../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import DataGridTabs from "../../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import { EnumOptions } from "../../../../utilities/enum";
import SelectComponent from "../../../../.reUseableComponents/TextField/SelectComponent";

const WalletTransctionModal = (props) => {
  const { open, onClose } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const { startDate, endDate, setStartDate, setEndDate, resetDates } =
    useDateRangeHook();
  const today = UtilityClass.todayDate();
  const sevenDaysAgo = new Date(today);
  sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 7);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const calculatedHeight = windowHeight - 273;
  const [loading, setLoading] = useState(false);
  const [tableloading, setTableLoading] = useState(false);
  const [allTransction, setAllTransction] = useState([]);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [filterTransction, setfilterTransction] = useState([
    { transactionTypeId: 1, transactionName: "Customer Credit" },
    { transactionTypeId: 2, transactionName: "Paymentlink Credit" },
    { transactionTypeId: 3, transactionName: "Payout Debit" },
    { transactionTypeId: 4, transactionName: "Refund Debit" },
    { transactionTypeId: 5, transactionName: "Services Charge Debit" },
    { transactionTypeId: 6, transactionName: "Payout Request Debit" },
  ]);
  const [selectedFilterTransctionId, setSelectedFilterTransctionId] =
    useState();
  const handleFilter = () => {
    getAllTransaction(
      startDate,
      endDate,
      selectedFilterTransctionId?.transactionTypeId
    );
  };

  const handleFilterClear = async () => {
    setIsfilterClear(true);
    resetDates();
  };

  const excelExportAllTransaction = async () => {
    setLoading(true);
    try {
      const response = await ExcelExportAllTransaction(
        startDate,
        endDate,
        selectedFilterTransctionId?.transactionTypeId
      );
      if (response) {
        UtilityClass.downloadExcel(response.data, "Transctions");
        successNotification("Excel Download successful.");
        onClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(response.data.errors);
      }
    } catch (error) {
      errorNotification("An error occurred while downloading excel:", error);
    } finally {
      setLoading(false);
    }
  };

  const getAllTransaction = async () => {
    setTableLoading(true);
    try {
      const response = await GetAllTransaction(startDate, endDate);
      if (response) {
        setAllTransction(response?.data?.result.list);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(response.data.errors);
      }
    } catch (error) {
      console.log(error);
    } finally {
      setTableLoading(false);
    }
  };

  const columns = [
    {
      field: "TransactionName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_TRANSCTION_NAME}
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box sx={{ fontWeight: "bold" }}>{row.TransactionName}</Box>;
      },
    },
    {
      field: "TransactionNo",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_TRANSCTION_NO}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box sx={{ fontWeight: "bold" }}>{row.TransactionNo}</Box>;
      },
    },
    {
      field: "Description",
      headerAlign: "center",
      align: "left",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_WALLET_DESCRIPTION}
        </Box>
      ),
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box>
            <Tooltip title={row.Description} arrow>
              <Box
                sx={{
                  whiteSpace: "nowrap",
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                  maxWidth: 200,
                }}
              >
                {row.Description}
              </Box>
            </Tooltip>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_WALLET_CREATED_ON}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row?.CreatedOn)}</Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Credit",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_WALLET_CREDIT}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.Credit}</Box>;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Debit",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ORDERS_WALLET_DEBIT}
        </Box>
      ),
      renderCell: ({ row }) => {
        return <Box>{row?.Debit}</Box>;
      },
      flex: 1,
    },
  ];

  useEffect(() => {
    getAllTransaction();
    setStartDate(sevenDaysAgo);
    setEndDate(today);
  }, []);
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="lg"
        title={LanguageReducer?.languageType?.ORDERS_WALLET_TRANSCTION_HISTORY}
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
                        <InputLabel
                          sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                        >
                          {
                            LanguageReducer?.languageType
                              ?.ORDERS_WALLET_TRANSCTION_TYPE
                          }
                        </InputLabel>
                        <SelectComponent
                          height={28}
                          name="reason"
                          options={filterTransction}
                          value={selectedFilterTransctionId}
                          optionLabel={EnumOptions.TRANSCTION.LABEL}
                          optionValue={EnumOptions.TRANSCTION.VALUE}
                          onChange={(e, val) => {
                            setSelectedFilterTransctionId(val);
                          }}
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
          otherBtns={
            <ActionButtonCustom
              label={
                LanguageReducer?.languageType?.ORDERS_WALLET_EXCEL_DOWNLOAD
              }
              onClick={() => {
                excelExportAllTransaction();
              }}
              startIcon={<FileDownloadOutlinedIcon />}
            />
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
            loading={tableloading}
            rowHeight={40}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row.RowNum}
            rows={allTransction || []}
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
    </>
  );
};

export default WalletTransctionModal;
