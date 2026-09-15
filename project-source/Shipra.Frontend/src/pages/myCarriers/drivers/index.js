import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Avatar,
  Box,
  Button,
  Divider,
  Grid,
  IconButton,
  InputLabel,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import AddDriverModal from "../../../components/modals/myCarrierModals/AddDriverModal";
import StatusBadge from "../../../components/shared/statudBadge";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  DeleteDriver,
  GetAllDrivers,
  GetAllEmployeesForSelection,
  GetGenderForSelection,
} from "../../../api/AxiosInterceptors";
import GeneralTabBar from "../../../components/shared/tabsBar";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import {
  ActionButtonDelete,
  CodeBox,
  DialerBox,
  MailtoBox,
  centerColumn,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import EditEmployeeModal from "../../../components/modals/settingsModals/EditEmployeeModal";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import EditDriverModal from "../../../components/modals/myCarrierModals/EditDriverModal";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import useDateRangeHook from "../../../.reUseableComponents/CustomHooks/useDateRangeHook";

function DriversPage(props) {
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
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const [allDrivers, setAllDrivers] = useState(false);
  const [isAllListLoading, setIsAllListLoading] = useState(false);
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [allEmployees, setAllEmployees] = useState([]);
  const [allGenderForSelection, setAllGenderForSelection] = useState([]);
  const [selectedRowData, setSelectedRowData] = useState();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isShowFilter, setIsShowFilter] = useState(true);
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [openEditModel, setOpenEditModel] = useState(false);
  const [openDeleteRecord, setOpenDeleteRecord] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const handleFilterRest = () => {
    resetDates();
  };
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const handleEditActionClick = (cTarget, data) => {
    // setAnchorEl(cTarget);
    setSelectedRowData(data);
    handleEditEmployee();
  };
  const handleEditEmployee = () => {
    setOpenEditModel(true);
  };
  const handleDeleteConfirmation = (data) => {
    setSelectedRowData(data);
    setOpenDeleteRecord(true);
  };
  const handleDelete = () => {
    if (selectedRowData) {
      let param = {
        DriverId: selectedRowData?.DriverId,
      };
      DeleteDriver(param)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
          } else {
            successNotification("Driver deleted successfully");
            getAllDrivers();
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
    console.log(isDeletedConfirm);
    if (isDeletedConfirm) {
      handleDelete();
    }
  }, [isDeletedConfirm]);

  const getFiltersFromState = () => {
    let search = "";
    let filters = {
      filterModel: {
        createdFrom: startDateFormated ? startDateFormated : null,
        createdTo: endDateFormated ? endDateFormated : null,
        start: 0,
        length: 1000,
        search: search,
        sortDir: "desc",
        sortCol: 0,
      },
    };
    return filters;
  };
  let getGenderForSelection = async () => {
    let res = await GetGenderForSelection();
    console.log("data:::", res.data);
    if (res.data.result !== null) {
      let modifiedArray = res.data.result.map((item) => ({
        id: item.genderId,
        text: item.genderName,
      }));
      setAllGenderForSelection(modifiedArray);
    }
  };
  let getAllDrivers = async () => {
    setIsAllListLoading(true);
    let params = getFiltersFromState();
    let res = await GetAllDrivers(params);
    console.log("GetAllDrivers", res.data.result);
    if (res.data.result !== null) {
      setAllDrivers(res.data.result);
    }
    setIsAllListLoading(false);
  };
  useEffect(() => {
    getGenderForSelection();
    getAllDrivers();
  }, []);
  useEffect(() => {
    if (isFilterReset) {
      getAllDrivers();
      setIsFilterReset(false);
    }
  }, [isFilterReset]);

  //#region
  const columns = [
    {
      field: "DriverCode",
      headerName: <Box sx={{ fontWeight: "600" }}>{" User Name"}</Box>,
      flex: 1,
      renderCell: (params) => {
        return (
          <CodeBox
            onClick={(e) => handleEditActionClick(e.currentTarget, params.row)}
            title={params?.row.DriverCode}
          />
        );
      },
    },
    {
      field: "DriverName",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Name"}</Box>,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              <Avatar
                sx={{ width: 30, height: 30, fontSize: "13px" }}
                src={params.row.EmployeeImage}
              ></Avatar>
              <Box>{params.row.DriverName}</Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Gender",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.GENDER_NAME_TEXT}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          params.row.Gender && (
            <StatusBadge
              title={params.row.Gender}
              color="#1E1E1E;"
              bgColor="#EAEAEA"
            />
          )
        );
      },
    },
    {
      ...centerColumn,
      field: "DOB",
      headerName: <Box sx={{ fontWeight: "600" }}> {"DOB"}</Box>,
      renderCell: (params) => (
        <Box>{UtilityClass.convertUtcToLocalAndGetDate(params.row.DOB)}</Box>
      ),
      flex: 1,
    },
    {
      field: "MobileNo",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Mobile No"}</Box>,
      flex: 1,
      renderCell: (params) => {
        return <DialerBox phone={params.row.MobileNo} />;
      },
    },
    {
      field: "Email",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.EMAIL_TEXT}
        </Box>
      ),
      // description: "This column has a value getter and is not sortable.",
      sortable: false,
      renderCell: (params) => {
        return <MailtoBox email={params.row.Email} />;
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACTION}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box>
            {/* <IconButton
              onClick={(e) =>
                handleEditActionClick(e.currentTarget, params.row)
              }
            >
              <MoreVertIcon />
            </IconButton> */}
            <ActionButtonDelete
              label=""
              onClick={(e) => handleDeleteConfirmation(params?.row)}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];

  //#endregion
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            "& .MuiDataGrid-root": {
              borderRadius: "8px 8px 8px 8px !important",
            },
          }}
        >
          <GeneralTabBar
            id="dashboard-dropdown"
            tabScreen="driver"
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
                          {"Start Date"}
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
                          {"End Date"}
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
                        sx={{
                          ...styleSheet.filterButtonMargin,
                          height: "100%",
                        }}
                      >
                        <Button
                          sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                          color="inherit"
                          variant="outlined"
                          onClick={() => {
                            handleFilterRest();
                          }}
                        >
                          {LanguageReducer?.languageType?.CLEAR_FILTER_TEXT}
                        </Button>
                        <Button
                          sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                          variant="contained"
                          onClick={() => {
                            getAllDrivers();
                          }}
                        >
                          {"Filter"}
                        </Button>
                      </Stack>
                    </Grid>
                  </Grid>
                </TableRow>
              </TableHead>
            </Table>
          ) : null}{" "}
          <DataGrid
            loading={isAllListLoading}
            rowHeight={40}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            rows={allDrivers.list ? allDrivers.list : []}
            getRowId={(row) => row.DriverId}
            columns={columns}
            pagination
            page={currentPage}
            pageSize={pageSize}
            rowsPerPageOptions={[5, 10, 15, 25]}
            paginationMode="client"
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
          />
          <Menu
            anchorEl={anchorEl}
            id="power-search-menu"
            open={Boolean(anchorEl)}
            onClose={() => {
              setAnchorEl(null);
            }}
            PaperProps={{
              elevation: 0,
              sx: {
                overflow: "visible",
                filter: "drop-shadow(0px 2px 8px rgba(0,0,0,0.32))",
                mt: 1.5,
                "& .MuiAvatar-root": {
                  width: 32,
                  height: 32,
                  ml: -0.5,
                  mr: 1,
                },
                "&:before": {
                  content: '""',
                  display: "block",
                  position: "absolute",
                  top: 0,
                  right: 14,
                  width: 10,
                  height: 10,
                  bgcolor: "background.paper",
                  transform: "translateY(-50%) rotate(45deg)",
                  zIndex: 0,
                },
              },
            }}
            transformOrigin={{ horizontal: "right", vertical: "top" }}
            anchorOrigin={{ horizontal: "right", vertical: "bottom" }}
          >
            <Box sx={{ width: "180px" }}>
              <List disablePadding>
                <ListItem
                  onClick={() => {
                    handleEditEmployee();
                    setAnchorEl(null);
                  }}
                  disablePadding
                >
                  <ListItemButton>
                    <ListItemText
                      primary={LanguageReducer?.languageType?.EDIT_TEXT}
                    />
                  </ListItemButton>
                </ListItem>
                <Divider />
                {/* <ListItem
                  onClick={() => {
                    handleDeleteConfirmation();
                    setAnchorEl(null);
                  }}
                  disablePadding
                >
                  <ListItemButton>
                    <ListItemText
                      primary={LanguageReducer?.languageType?.DELETE_TEXT}
                    />
                  </ListItemButton>
                </ListItem> */}
              </List>
            </Box>
          </Menu>
        </Box>
      </div>
      {open && (
        <AddDriverModal
          open={open}
          setOpen={setOpen}
          // {...props}
          getAllDrivers={getAllDrivers}
          allGenderForSelection={allGenderForSelection}
        />
      )}
      {openEditModel && (
        <EditDriverModal
          open={openEditModel}
          setOpen={setOpenEditModel}
          {...props}
          getAllEmployees={getAllDrivers}
          selectedRowData={selectedRowData}
          allGenderForSelection={allGenderForSelection}
        />
      )}
      <DeleteConfirmationModal
        open={openDeleteRecord}
        setOpen={setOpenDeleteRecord}
        setIsDeletedConfirm={setIsDeletedConfirm}
        loading={isDeletedConfirm}
        {...props}
      />
    </Box>
  );
}
export default DriversPage;
