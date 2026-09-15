import Edit from '@mui/icons-material/Edit';
import MoreVertIcon from "@mui/icons-material/MoreVert";
import {
  Avatar,
  Box,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Menu,
  Stack,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { styleSheet } from "../../../../assets/styles/style";
import UtilityClass from "../../../../utilities/UtilityClass";
import { usePagination } from "../../../../utilities/helpers/Helpers";

function ExpenseList(props) {
  const { getOrdersRef, loading, resetRowRef, rows } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [openUpdate, setOpenUpdate] = useState(false);
  const [selectedRowData, setSelectedRowData] = useState({});

  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const handleItemActionDropDown = (e, data) => {
    setSelectedRowData(data);
    setAnchorEl(e.currentTarget);
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
      field: "DriverName",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Driver"}</Box>,
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
                <Avatar
                  sx={{
                    width: 25,
                    height: 25,
                    fontSize: "13px",
                    color: "var(--primary-color)",
                    background: "rgba(86, 58, 213, 0.3)",
                  }}
                >
                  {params.row.DriverName?.slice(0, 1)}
                </Avatar>
                <Box>{params.row.DriverName}</Box>
              </Stack>
            </Box>
          )
        );
      },
    },
    {
      field: "DriverCode",
      headerName: <Box sx={{ fontWeight: "600" }}> {"DriverCode"}</Box>,
      flex: 1,
    },
    {
      field: "NoteNo",
      headerName: <Box sx={{ fontWeight: "600" }}> {"NoteNo"}</Box>,
      flex: 1,
    },
    {
      field: "ExpenseCategoryName",
      //   headerAlign: "center",
      //   align: "center",
      headerName: <Box sx={{ fontWeight: "600" }}>{"Expense Category"}</Box>,
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
      field: "ExpenseDate",
      headerName: <Box sx={{ fontWeight: "600" }}>{" Expense Date"}</Box>,
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.ExpenseDate)}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "Details",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Details"}</Box>,
      flex: 1,
    },
    {
      field: "CreatedOn",
      headerName: <Box sx={{ fontWeight: "600" }}>{" CreatedOn"}</Box>,
      renderCell: (params) => (
        <Box>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.CreatedOn)}
        </Box>
      ),
      flex: 1,
    },
    {
      field: "Amount",
      headerName: <Box sx={{ fontWeight: "600" }}> {"Amount"}</Box>,
      flex: 1,
    },
    {
      field: "Action",
      hide: true,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACTION}
        </Box>
      ),
      renderCell: (params) => {
        // onClick={(e) => setAnchorEl(e.currentTarget)}
        return (
          <Box>
            <IconButton
              onClick={(e) => handleItemActionDropDown(e, params.row)}
            >
              <MoreVertIcon />
            </IconButton>
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
  return (
    <Box sx={styleSheet.allOrderTable}>
      <DataGrid
        loading={loading}
        // getRowHeight={() => "auto"}
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
    </Box>
  );
}
export default ExpenseList;
