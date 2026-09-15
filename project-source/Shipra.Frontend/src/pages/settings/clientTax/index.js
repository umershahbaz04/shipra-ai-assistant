import {
  Box,
  Button,
  CircularProgress,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style.js";
import AddClientTaxModal from "../../../components/modals/settingsModals/AddClientTaxModal";
import StatusBadge from "../../../components/shared/statudBadge";
import {
  GetAllClientTax,
  GetAllTaxForSelection,
  EnableDisableTax,
  GetClientTaxById,
} from "../../../api/AxiosInterceptors.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter.js";
import GeneralTabBar from "../../../components/shared/tabsBar/index.js";
import EditClientTaxModal from "../../../components/modals/settingsModals/EditClientTaxModal";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal.js";
import UtilityClass from "../../../utilities/UtilityClass.js";

import Colors from "../../../utilities/helpers/Colors.js";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook.js";
import {
  CodeBox,
  DisableButton,
  EnableButton,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers.js";

function UsersPage(props) {
  const {
    startDate,
    endDate,
    setStartDate,
    setEndDate,
    startDateFormated,
    endDateFormated,
  } = useDateRangeHook();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const [allClientTax, setAllClientTax] = useState({
    loading: false,
    list: [],
  });
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  // const [startDate, setStartDate] = useState(null);
  // const [endDate, setEndDate] = useState(null);
  const [allTaxForSelection, setAllTaxForSelection] = useState([]);
  const [selectedRowData, setSelectedRowData] = useState();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isShowFilter, setIsShowFilter] = useState(true);
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [openDeleteRecord, setOpenDeleteRecord] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const [modalHeading, setModalHeading] = useState("");
  const [modalMessage, setModalMessage] = useState("");
  const [modalButtonText, setModalButtonText] = useState("");
  const [buttonColor, setButtonColor] = useState("");
  const [taxInfo, setTaxInfo] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  let getAllClientTax = async () => {
    setAllClientTax((prev) => ({
      ...prev,
      loading: true,
    }));
    let res = await GetAllClientTax(
      startDateFormated ? startDateFormated : null,
      endDateFormated ? endDateFormated : null
    );
    if (res.data.result !== null) {
      setAllClientTax((prev) => ({
        loading: false,
        list: res.data.result.list,
      }));
    }
  };
  let getAllTaxForSelection = async () => {
    let res = await GetAllTaxForSelection();
    if (res.data.result !== null) {
      setAllTaxForSelection(res.data.result);
    }
  };
  useEffect(() => {
    getAllClientTax();
    getAllTaxForSelection();
  }, []);

  const handleFilterClear = () => {
    handleFilterRest();
    setIsfilterClear(true);
  };
  const handleFilterRest = () => {
    setStartDate(null);
    setEndDate(null);
  };
  useEffect(() => {
    if (isfilterClear) {
      getAllClientTax();
    }
    setIsfilterClear(false);
  }, [isfilterClear]);

  const handleDeleteConfirmation = (data, actionType) => {
    if (actionType === "disable") {
      setModalHeading("Are you sure to disable this item?");
      setModalMessage(
        "After this action the item will be disabled immediately but you can undo this action anytime."
      );
      setModalButtonText("Disable");
      setButtonColor(Colors.danger);
    } else if (actionType === "enable") {
      setModalHeading("Are you sure to enable this item?");
      setModalMessage(
        "After this action the item will be enabled immediately but you can undo this action anytime."
      );
      setModalButtonText("Enable");
      setButtonColor(Colors.succes);
    }
    setSelectedRowData(data);
    setOpenDeleteRecord(true);
  };
  const handleDelete = () => {
    if (selectedRowData) {
      EnableDisableTax({
        clientTaxId: selectedRowData.ClientTaxId,
        isActive: selectedRowData.Active === true ? 0 : 1,
      })
        .then((res) => {
          if (!res?.data?.isSuccess) {
            errorNotification("Unable to change client tax status");
          } else {
            successNotification("Client tax status changed successfully");
            getAllClientTax();
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Something went wrong");
        })
        .finally(() => {
          setSelectedRowData(null);
          setIsDeletedConfirm(false);
          setOpenDeleteRecord(false);
        });
    }
  };

  useEffect(() => {
    if (isDeletedConfirm) {
      handleDelete();
    }
  }, [isDeletedConfirm]);

  const handleUpdateTaxNo = (selectedRowData) => {
    if (selectedRowData) {
      setTaxInfo((prev) => ({
        ...prev,
        loading: { [selectedRowData.ClientTaxId]: true },
      }));
      GetClientTaxById({
        clientTaxId: selectedRowData.ClientTaxId,
      })
        .then((res) => {
          console.log("res:::", res);
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.errors);
          } else {
            setTaxInfo((prev) => ({
              ...prev,
              open: true,
              data: res?.data?.result,
            }));
          }
        })
        .catch((e) => {
          console.log("e", e);
          errorNotification("Something went wrong");
        })
        .finally(() => {
          setTaxInfo((prev) => ({
            ...prev,
            loading: { [selectedRowData.ClientTaxId]: false },
          }));
        });
    }
  };
  const columns = [
    {
      field: "Name",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_MANAGE_TAX_NAME}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            {taxInfo.loading[params.row.ClientTaxId] ? (
              <CircularProgress size={20} />
            ) : (
              <>
                <CodeBox
                  title={params.row.Name}
                  onClick={() => handleUpdateTaxNo(params.row)}
                />
              </>
            )}
          </>
        );
      },
    },

    {
      field: "Percentage",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTING_MANAGE_TAX_PERCENTAGE}
        </Box>
      ),
      flex: 1,
    },

    {
      field: "Active",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_MANAGE_TAX_STATUS}
        </Box>
      ),
      flex: 1,

      renderCell: (params) => {
        let isActive = params.row.Active;
        let title = isActive ? "Active" : "InActive";
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              <Box>
                <StatusBadge
                  title={title}
                  color={isActive == false ? "#fff;" : "#fff;"}
                  bgColor={isActive === false ? Colors.danger : Colors.succes}
                />
              </Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      field: "Action",
      minWidth: 60,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_MANAGE_TAX_ACTION}
        </Box>
      ),
      renderCell: ({ row, params }) => {
        return (
          !row?.IsClient && (
            <Box>
              {row?.Active ? (
                <DisableButton
                  onClick={() => handleDeleteConfirmation(row, "disable")}
                />
              ) : (
                <EnableButton
                  onClick={() => handleDeleteConfirmation(row, "enable")}
                />
              )}
            </Box>
          )
        );
      },
    },
  ];

  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 160.5
    : windowHeight - navbarHeight - 65;

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <GeneralTabBar
          id="dashboard-dropdown"
          tabScreen="tax"
          placeholder="Action"
          isFilterOpen={isFilterOpen}
          setIsFilterOpen={setIsFilterOpen}
          tabData={[]}
          disableSearch
          {...props}
          setAddEmployee={setOpen}
        />
        {isFilterOpen && isShowFilter ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <TableRow>
                <Grid container spacing={2} sx={{ p: "15px 6px" }}>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.START_DATE}
                      </InputLabel>

                      <CustomReactDatePickerInputFilter
                        value={startDate}
                        onClick={(date) => setStartDate(date)}
                        size="small"
                        isClearable
                        maxDate={UtilityClass.todayDate()}

                        // inputProps={{ style: { padding: "4px 5px" } }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Grid>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {LanguageReducer?.languageType?.END_DATE}
                      </InputLabel>
                      <CustomReactDatePickerInputFilter
                        value={endDate}
                        onClick={(date) => setEndDate(date)}
                        size="small"
                        minDate={startDate}
                        disabled={!startDate ? true : false}
                        isClearable
                        maxDate={UtilityClass.todayDate()}

                        // inputProps={{ style: { padding: "4px 5px" } }}
                      />
                    </Grid>
                  </Grid>
                  <Grid item xl={2} lg={2} md={2} sm={6} xs={12}>
                    <Stack
                      alignItems="flex-end"
                      direction="row"
                      spacing={1}
                      sx={{ ...styleSheet.filterButtonMargin, height: "100%" }}
                    >
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          handleFilterClear();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER}
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllClientTax();
                        }}
                      >
                        {LanguageReducer?.languageType?.FILTER}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}{" "}
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            height: calculatedHeight,
          }}
        >
          <DataGrid
            loading={allClientTax.loading}
            rowHeight={40}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row.ClientTaxId}
            rows={allClientTax.list}
            columns={columns}
            pagination
            page={currentPage}
            pageSize={pageSize}
            rowsPerPageOptions={[5, 10, 15, 25]}
            paginationMode="client"
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
          />
        </Box>
      </div>
      {open && (
        <AddClientTaxModal
          open={open}
          setOpen={setOpen}
          {...props}
          getAllClientTax={getAllClientTax}
          getAllTaxForSelection={getAllTaxForSelection}
          allTaxForSelection={allTaxForSelection}
        />
      )}
      {taxInfo.open && (
        <EditClientTaxModal
          open={taxInfo.open}
          setTaxInfo={setTaxInfo}
          {...props}
          selectedRowData={selectedRowData}
          allTaxForSelection={allTaxForSelection}
          getAllClientTax={getAllClientTax}
          getAllTaxForSelection={getAllTaxForSelection}
          taxInfo={taxInfo}
        />
      )}
      <DeleteConfirmationModal
        open={openDeleteRecord}
        setOpen={setOpenDeleteRecord}
        setIsDeletedConfirm={setIsDeletedConfirm}
        loading={isDeletedConfirm}
        {...props}
        heading={modalHeading}
        message={modalMessage}
        buttonText={modalButtonText}
        buttonColor={buttonColor}
      />
    </Box>
  );
}
export default UsersPage;
