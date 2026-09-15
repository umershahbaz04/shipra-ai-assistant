import {
  Avatar,
  Box,
  Button,
  Divider,
  Grid,
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
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal.js";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter.js";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent.js";
import {
  EnableDisableEmploye,
  GetAllEmployees,
  GetGenderForSelection,
} from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";
import AddEmployeeModal from "../../../components/modals/settingsModals/AddEmployee.js";
import EditEmployeeModal from "../../../components/modals/settingsModals/EditEmployeeModal.js";
import ResetPasswordModal from "../../../components/modals/settingsModals/ResetPasswordModal.js";
import WhatsappSmsModal from "../../../components/modals/settingsModals/WhatsappSmsModal.js";
import StatusBadge from "../../../components/shared/statudBadge";
import GeneralTabBar from "../../../components/shared/tabsBar/index.js";
import UtilityClass from "../../../utilities/UtilityClass.js";
import { EnumOptions } from "../../../utilities/enum/index.js";
import Colors from "../../../utilities/helpers/Colors.js";
import {
  ActionButtonCustom,
  centerColumn,
  ClipboardIcon,
  CodeBox,
  DialerBox,
  DisableButton,
  EnableButton,
  MailtoBox,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetAllClientUserRole,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers.js";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";

function UsersPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [open, setOpen] = useState(false);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [openEditModel, setOpenEditModel] = useState(false);
  const [allEmployees, setAllEmployees] = useState([]);
  const [allGenderForSelection, setAllGenderForSelection] = useState([]);
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [selectedRowData, setSelectedRowData] = useState();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isShowFilter, setIsShowFilter] = useState(true);
  const [isfilterClear, setIsfilterClear] = useState(false);
  const [isAllListLoading, setIsAllListLoading] = useState(false);
  const [openDeleteRecord, setOpenDeleteRecord] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const [selectedUserRole, setselectedUserRole] = useState([]);
  const [loadingEnableStates, setLoadingEnableStates] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [dataForWhatsappSms, setdataForWhatsappSms] = useState({});
  const [openWhatsappSmsModal, setOpenWhatsappSmsModal] = useState(false);
  const [resetPasswordModal, setResetPasswordModal] = useState({
    open: false,
    rowData: [],
  });
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);

  let getAllEmployees = async (clientUserRoleId) => {
    setIsAllListLoading(true);
    let res = await GetAllEmployees({
      filterModel: {
        createdFrom: startDate ? startDate : null,
        createdTo: endDate ? endDate : null,
        start: 0,
        length: 10000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      UserRoleId:
        clientUserRoleId === 0 || clientUserRoleId
          ? clientUserRoleId
          : selectedUserRole?.clientUserRoleId || 0,
    });
    if (res.data.result !== null) {
      setIsAllListLoading(false);
      setAllEmployees(res.data.result.list);
    }
  };
  let getGenderForSelection = async () => {
    setIsAllListLoading(true);
    let res = await GetGenderForSelection();
    if (res.data.result !== null) {
      let modifiedArray = res.data.result.map((item) => ({
        id: item.genderId,
        text: item.genderName,
      }));
      setAllGenderForSelection(modifiedArray);
    }
  };
  useEffect(() => {
    getAllEmployees();
    getGenderForSelection();
  }, []);
  const { clientUserRole } = useGetAllClientUserRole();
  const handleFilterClear = () => {
    handleFilterRest();
    setIsfilterClear(true);
  };
  const handleFilterRest = () => {
    setStartDate(null);
    setEndDate(null);
    setselectedUserRole(0);
  };
  useEffect(() => {
    if (isfilterClear) {
      getAllEmployees();
    }
    setIsfilterClear(false);
  }, [isfilterClear]);
  const columns = [
    {
      field: "EmployeeCode",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTING_USER_USER_NAME}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <CodeBox
              onClick={(e) =>
                handleEditActionClick(e.currentTarget, params.row)
              }
              title={params?.row.EmployeeCode}
              bold={"bold"}
            />
            <ClipboardIcon text={params.row.EmployeeCode} />
          </>
        );
      },
    },
    {
      field: "EmployeeName",
      minWidth: 170,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_USER_NAME}
        </Box>
      ),
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
              <Box>{params.row.EmployeeName}</Box>
            </Stack>
          </Box>
        );
      },
    },

    {
      field: "MobileNo",
      minWidth: 150,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_USER_PHONE}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return <DialerBox phone={params.row.MobileNo} />;
      },
    },
    {
      field: "WorkEmail",
      minWidth: 210,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_USER_EMAIL}
        </Box>
      ),
      // description: "This column has a value getter and is not sortable.",
      sortable: false,
      renderCell: (params) => {
        return <MailtoBox email={params.row.WorkEmail} />;
      },
      flex: 1,
    },
    {
      field: "GenderName",
      ...centerColumn,
      minWidth: 90,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_USER_GENDER}
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
      field: "EmployeeType",
      ...centerColumn,
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTING_USER_EMPLOYEE_TYPE}
        </Box>
      ),
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
              <Box>{params.row?.EmployeeTypeName}</Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      field: "RoleName",
      ...centerColumn,
      minWidth: 130,
      headerName: <Box sx={{ fontWeight: "600" }}>{"User Role"}</Box>,
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
              <Box>{params.row?.RoleName}</Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      field: "DateOfBirth",
      ...centerColumn,
      minWidth: 90,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.SETTING_USER_DOB}
        </Box>
      ),

      renderCell: (params) => (
        <Box>
          {params.row.DateOfBirth &&
            new Date(params.row.DateOfBirth).toLocaleDateString("en-IR")}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "Status",
      ...centerColumn,
      minWidth: 90,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_USER_STATUS}
        </Box>
      ),
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
      minWidth: 180,
      ...centerColumn,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.SETTING_USER_ACTION}
        </Box>
      ),
      renderCell: ({ row, params }) => {
        return (
          !row?.IsClient && (
            <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
              {row?.Active ? (
                <DisableButton onClick={() => handleDeleteConfirmation(row)} />
              ) : (
                <EnableButton
                  loading={loadingEnableStates.loading[row.ProductStockId]}
                  onClick={(e) => handleDeleteConfirmation(row)}
                />
              )}
              <ActionButtonCustom
                label={"Reset Password"}
                width={"80px"}
                onClick={() =>
                  setResetPasswordModal((prev) => ({
                    ...prev,
                    open: true,
                    rowData: row,
                  }))
                }
              />
            </Box>
          )
        );
      },
    },
  ];
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
      EnableDisableEmploye(selectedRowData.EmployeeId)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            errorNotification("Unable to delete employee");
          } else {
            successNotification("Employee deleted successfully");
            getAllEmployees();
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

  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 150
    : windowHeight - navbarHeight - 65;

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <GeneralTabBar
          id="dashboard-dropdown"
          tabScreen="user"
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
                        {LanguageReducer?.languageType?.SETTING_USER_START_DATE}
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
                        {LanguageReducer?.languageType?.SETTING_USER_END_DATE}
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
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {LanguageReducer?.languageType?.SETTING_USER_USER_ROLE}
                    </InputLabel>
                    <SelectComponent
                      optionLabel={EnumOptions.USER_ROLE.LABEL}
                      optionValue={EnumOptions.USER_ROLE.VALUE}
                      height={28}
                      name="userRole"
                      options={clientUserRole}
                      value={selectedUserRole}
                      getOptionLabel={(option) => option?.roleName}
                      onChange={(e, val) => {
                        setselectedUserRole(val);
                      }}
                    />
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
                        {
                          LanguageReducer?.languageType
                            ?.SETTING_USER_CLEAR_FILTER
                        }
                      </Button>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        variant="contained"
                        onClick={() => {
                          getAllEmployees();
                        }}
                      >
                        {LanguageReducer?.languageType?.SETTING_USER_FILTER}
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
            loading={isAllListLoading}
            rowHeight={40}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row.EmployeeId}
            rows={allEmployees}
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
                <ListItem
                  onClick={(e) => {
                    setAnchorEl(null);
                  }}
                  disablePadding
                >
                  <ListItemButton>
                    <ListItemText
                      primary={LanguageReducer?.languageType?.DELETE_TEXT}
                    />
                  </ListItemButton>
                </ListItem>
              </List>
            </Box>
          </Menu>
        </Box>
      </div>
      {open && (
        <AddEmployeeModal
          open={open}
          setOpen={setOpen}
          {...props}
          getAllEmployees={getAllEmployees}
          allGenderForSelection={allGenderForSelection}
          setdataForWhatsappSms={setdataForWhatsappSms}
          setOpenWhatsappSmsModal={setOpenWhatsappSmsModal}
          getGenderForSelection={getGenderForSelection}
        />
      )}
      {openEditModel && (
        <EditEmployeeModal
          open={openEditModel}
          setOpen={setOpenEditModel}
          {...props}
          getAllEmployees={getAllEmployees}
          selectedRowData={selectedRowData}
          allGenderForSelection={allGenderForSelection}
          getGenderForSelection={getGenderForSelection}
        />
      )}
      {openWhatsappSmsModal && (
        <WhatsappSmsModal
          open={openWhatsappSmsModal}
          onClose={() => setOpenWhatsappSmsModal(false)}
          dataForWhatsappSms={dataForWhatsappSms}
        />
      )}
      <DeleteConfirmationModal
        open={openDeleteRecord}
        setOpen={setOpenDeleteRecord}
        setIsDeletedConfirm={setIsDeletedConfirm}
        loading={isDeletedConfirm}
        {...props}
        heading={"Are you sure to disable this item?"}
        message={
          "After this action the item will be disabled immediately but you can undo this action anytime."
        }
        buttonText={"Disable"}
      />
      {resetPasswordModal.open && (
        <ResetPasswordModal
          open={resetPasswordModal.open}
          onClose={() =>
            setResetPasswordModal((prev) => ({ ...prev, open: false }))
          }
          data={resetPasswordModal.rowData}
        />
      )}
    </Box>
  );
}
export default UsersPage;
