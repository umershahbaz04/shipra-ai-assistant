import Edit from '@mui/icons-material/Edit';
import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  Stack
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { GetExpenseById } from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import EditExpenseModal from "../../../../components/modals/expenseModals/EditExpenseModal";
import UpdateOutScanModal from "../../../../components/modals/myCarrierModals/UpdateOutscanModal";
import UtilityClass from "../../../../utilities/UtilityClass";
import {
  amountFormat,
  centerColumn,
  CodeBox,
  EditButton,
  navbarHeight,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";

function DriverExpenseList(props) {
  const {
    getOrdersRef,
    loading,
    resetRowRef,
    rows,
    getAllDriverExpense,
    isFilterOpen,
  } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [openUpdate, setOpenUpdate] = useState(false);
  const [selectedRowData, setSelectedRowData] = useState({});
  const [expenseForEdit, setExpenseForEdit] = useState({});

  const [editModal, setEditModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const handleItemActionDropDown = (e, data) => {
    setSelectedRowData(data);
    setAnchorEl(e.currentTarget);
  };
  const handleEditExpense = async (data) => {
    setEditModal((prev) => ({ ...prev, loading: { [data.ExpenseId]: true } }));

    let res = await GetExpenseById(data?.ExpenseId);
    if (res.data.result !== null) {
      setExpenseForEdit(res.data.result);
    }
    setEditModal((prev) => ({
      ...prev,
      data: res.data.result || [],
      open: true,
      loading: { [data.ExpenseId]: false },
    }));
  };
  // const rows = {
  //   list: [
  //     {
  //       ExpenseId: "d5367387-a794-4968-89a8-ccfadb025455",
  //       Amount: 7000.0,
  //       Active : null,
  //       Details: "det",
  //       DriverId: "d8ccc085-5499-4d0d-9b4e-86c64734c3b8",
  //       DriverCode: "d8ccc085",
  //       EmployeeCode: "d8ccc085",
  //       DriverName: "d8ccc085",
  //       ExpenseDate: "2023-09-13T12:52:39.163",
  //       ExpenseCategoryName: "cat",
  //       ExpenseCategoryId: 0,
  //       CreatedOn: "2023-09-13T04:51:12.353",
  //     },
  //   ],
  // };
  //#region table colum
  const columns = [
    {
      field: "NoteNo",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_NOTE_NO}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <CodeBox
              title={params?.row.NoteNo}
              hasColor={false}
              copyBtn={true}
            />
          </>
        );
      },
    },
    {
      field: "DriverName",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_DRIVER}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        return (
          params.row.DriverName && (
            <Box>
              <Stack
                direction="row"
                justifyContent="center"
                alignItems="center"
                spacing={1}
              >
                <Box>{params.row.DriverName}</Box>
              </Stack>
            </Box>
          )
        );
      },
    },
    {
      field: "DriverCode",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_DRIVER_CODE}
        </Box>
      ),
      flex: 1,
    },

    {
      field: "ExpenseCategoryName",
      //   headerAlign: "center",
      //   align: "center",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_EXPENSE_CATEGORY}
        </Box>
      ),
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center", cursor: "pointer" }}
            disableRipple
          >
            <>
              <Box sx={{ fontWeight: "bold" }}>
                {params.row.ExpenseCategoryName}
              </Box>
            </>
          </Box>
        );
      },
    },

    {
      ...centerColumn,
      field: "ExpenseDate",
      minWidth: 140,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_EXPENSE_DATE}
        </Box>
      ),
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.ExpenseDate)}
        </Box>
      ),
      flex: 1,
    },
    // {
    //   field: "Details",
    //   headerName: <Box sx={{ fontWeight: "600" }}> {"Details"}</Box>,
    //   flex: 1,
    // },
    // {
    //   field: "CreatedOn",
    //   headerName: <Box sx={{ fontWeight: "600" }}>{" CreatedOn"}</Box>,
    //   renderCell: (params) => (
    //     <Box>
    //       {UtilityClass.convertUtcToLocalAndGetDate(params.row.CreatedOn)}
    //     </Box>
    //   ),
    //   flex: 1,
    // },
    {
      ...rightColumn,
      field: "Amount",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_AMOUNT}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => <Box>{amountFormat(params?.row?.Amount)}</Box>,
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 120,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.MY_CARRIER_EXPENSES_TASKS_ACTION}
        </Box>
      ),
      renderCell: (params) => {
        // onClick={(e) => setAnchorEl(e.currentTarget)}
        return (
          <Box>
            {/* <IconButton
              onClick={(e) => handleItemActionDropDown(e, params.row)}
            >
              <MoreVertIcon />
            </IconButton> */}

            <EditButton
              onClick={() => handleEditExpense(params?.row)}
              loading={editModal.loading[params.row.ExpenseId]}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];
  //#endregion
  const [selectionModel, setSelectionModel] = useState([]);

  const handleSelectedRow = (oNos) => {
    setSelectionModel(oNos);
    getOrdersRef.current = oNos;
  };
  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
    }
  }, [resetRowRef.current]);

  const [openAddExpense, setOpenAddExpense] = useState(false);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 156.5
    : windowHeight - navbarHeight - 65;

  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: calculatedHeight,
      }}
    >
      <DataGrid
        loading={loading}
        rowHeight={45}
        getRowHeight={() => "43px"}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
        }}
        rows={rows?.list ? rows?.list : []}
        getRowId={(row) => row.ExpenseId}
        columns={columns}
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        selectionModel={selectionModel}
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
        <Box sx={{ width: "200px" }}>
          <List disablePadding>
            <ListItem
              disablePadding
              onClick={() => {
                setAnchorEl(null);
              }}
            >
              <ListItemButton onClick={() => setOpenAddExpense(true)}>
                <ListItemIcon sx={{ minWidth: "30px" }}>
                  <Edit />
                </ListItemIcon>
                <ListItemText primary="Edit Expense" />
              </ListItemButton>
            </ListItem>
          </List>
        </Box>
      </Menu>
      <UpdateOutScanModal
        open={openUpdate}
        setOpen={setOpenUpdate}
        {...props}
      />

      {editModal.open && (
        <EditExpenseModal
          open={editModal.open}
          setOpen={setOpenAddExpense}
          onClose={() => setEditModal((prev) => ({ ...prev, open: false }))}
          expenseForEdit={editModal.data}
          selectedRowData={selectedRowData}
          getAllDriverExpense={getAllDriverExpense}
        />
      )}
    </Box>
  );
}
export default DriverExpenseList;
