import {
  Avatar,
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  Stack,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import React, { useRef, useState } from "react";
import { useSelector } from "react-redux";
import CircularProgressBarForTableRowWitIcon from "../../../../.reUseableComponents/progressBar/CircularProgressBarForTableRowWitIcon";
import {
  DisableProductStock,
  EnableProductStock,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import StatusBadge from "../../../../components/shared/statudBadge";
import UtilityClass from "../../../../utilities/UtilityClass";
import Colors from "../../../../utilities/helpers/Colors";
import {
  ClipboardIcon,
  CodeBox,
  DataGridRenderGreyBox,
  DescriptionBox,
  DisableButton,
  EnableButton,
  amountFormat,
  centerColumn,
  navbarHeight,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";

function LowInventoryList(props) {
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [inventoryItem, setInventoryItem] = useState();
  const [open, setOpen] = useState(false);

  // const [products, setProducts] = useState([]);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { inventory, getAllProductInventory, isGridLoading, isFilterOpen } =
    props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const gridApiRef = useRef(null);

  const [editInventoryModalOpen, setEditInventoryModalOpen] = useState(false);
  const [stockHistoryModal, setStockHistoryModal] = useState(false);

  const handleEditInventoryModalClose = () => {
    setEditInventoryModalOpen(false);
  };
  const handleEditInventoryModalOpen = (data) => {
    setInventoryItem(data);
    setEditInventoryModalOpen(true);
  };
  const handleStockHistoryModalClose = () => {
    setStockHistoryModal(false);
  };
  const handleStockHistoryModalOpen = () => {
    setStockHistoryModal(true);
  };

  const [openDelete, setOpenDelete] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [loadingStates, setLoadingStates] = useState(false);
  const [loadingEnableStates, setLoadingEnableStates] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const handleDeleteConfirmation = (data) => {
    setDeleteItemObject(data);
    setOpenDelete(true);
  };
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  const handleDelete = async () => {
    try {
      let param = {
        ProductStockId: deleteItemObject.ProductStockId,
      };
      setLoadingStates(true);

      const response = await DisableProductStock(param);
      if (response.data?.isSuccess) {
        getAllProductInventory();
        successNotification("inventory deactivated successfully");
      } else {
        errorNotification("inventory deactivated unsuccessfull");
      }
    } catch (error) {
      if (error?.response.data.errors) {
        UtilityClass.showErrorNotificationWithDictionary(
          error?.response?.data?.errors
        );
      }
      console.error("Something went wrong", error.response);
    } finally {
      setLoadingStates(false);
      setOpenDelete(false);
      setDeleteItemObject({});
    }
  };
  const handleEnableProduct = async (data) => {
    try {
      let param = {
        ProductStockId: data.ProductStockId,
      };
      setLoadingEnableStates((prev) => ({
        ...prev,
        loading: { [data.ProductStockId]: true },
      }));
      const response = await EnableProductStock(param);
      if (response.data?.isSuccess) {
        getAllProductInventory();
        successNotification("product activated successfully");
      } else {
        errorNotification("product activated unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      if (error?.response.data.errors) {
        UtilityClass.showErrorNotificationWithDictionary(
          error?.response?.data?.errors
        );
      }
    } finally {
      setLoadingEnableStates((prev) => ({
        ...prev,
        loading: { [data.ProductStockId]: false },
      }));
    }
  };
  const columns = [
    {
      field: "Title",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_TITLE}
        </Box>
      ),
      flex: 1,
      minWidth: 220,
      renderCell: (params) => {
        return (
          <Box className={"flex_center"} gap={1}>
            <Avatar
              src={params.row.FeatureImage}
              sx={{ width: 30, height: 30, fontSize: "13px" }}
            >
              {params.row.ProductName[0]}
            </Avatar>
            <Box>
              {params.row.ProductName}
              <DataGridRenderGreyBox title={params.row.VarientOption} />
            </Box>
          </Box>
        );
      },
    },
    {
      field: "SKU",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_SKU}
        </Box>
      ),
      minWidth: 150,
      flex: 1,

      renderCell: (params) => {
        return (
          // <Box
          //   display={"flex"}
          //   flexDirection={"column"}
          //   // alignItems={"center"}
          //   pt={1.5}
          // >
          <Box display={"flex"} flexDirection={"column"}>
            <Box>
              <CodeBox
                title={params.row.ProductSku || ""}
                onClick={() => {
                  setInventoryItem(params.row);
                  handleStockHistoryModalOpen();
                }}
                copyBtn={params.row.ProductSku && true}
              />
            </Box>
            <DataGridRenderGreyBox title={params.row.SKU} />
          </Box>
        );
      },
    },
    {
      field: "StoreName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_STORE_NAME}
        </Box>
      ),
      width: 120,
    },
    {
      field: "StationName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_PRODUCT_STATION}
        </Box>
      ),
      width: 120,
    },
    {
      field: "Sale Price",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_SALE_PRICE}
        </Box>
      ),
      minWidth: 80,
      ...rightColumn,
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{amountFormat(row.Price)} </Box>;
      },
    },
    {
      field: "LowQuantityLimit",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_LOW_QTY}
        </Box>
      ),
      minWidth: 90,
      ...centerColumn,
      flex: 1,
      renderCell: ({ row }) => {
        return <CodeBox title={row.LowQuantityLimit || "0"} hasColor={false} />;
      },
    },

    {
      field: "QuantityAvailable",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCT_INVENTORY_QTY_AVAILABLE}
        </Box>
      ),
      minWidth: 90,
      ...centerColumn,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Stack direction={"row"}>
            <CodeBox title={row.QuantityAvailable || "0"} hasColor={false} />
          </Stack>
        );
      },
    },

    {
      ...centerColumn,
      field: "Status",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_STATUS}
        </Box>
      ),
      width: 80,

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

                {/* <Typography>{params.row.ProductStockStatus || ""}</Typography> */}
              </Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 60,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCT_LOW_INVENTORY_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        return (
          <Box>
            {row?.Active ? (
              <DisableButton onClick={() => handleDeleteConfirmation(row)} />
            ) : (
              <EnableButton
                loading={loadingEnableStates.loading[row.ProductStockId]}
                onClick={() => handleEnableProduct(row)}
              />
            )}
          </Box>
        );
      },
    },
  ];
  const getRowClassName = (params) => {
    for (let i = 0; i < inventory?.length; i++) {
      if (params.row.Active != true) return "active-row"; // CSS class name for active rows
    }
    return "";
  };
  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 67 - 86
    : windowHeight - navbarHeight - 67;
  return (
    <>
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: calculatedHeight,
        }}
      >
        <DataGrid
          loading={isGridLoading}
          rowHeight={42}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row.ProductStockId}
          rows={inventory}
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
          getRowClassName={getRowClassName}
          height={calculatedHeight}
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
              <ListItem disablePadding>
                <ListItemButton
                  onClick={() => {
                    setOpen(true);
                    setAnchorEl(null);
                  }}
                >
                  <ListItemText
                    primary={LanguageReducer?.languageType?.VIEW_TEXT}
                  />
                </ListItemButton>
              </ListItem>
              <ListItem
                onClick={() => {
                  handleEditInventoryModalOpen();
                }}
                disablePadding
              >
                <ListItemButton>
                  <ListItemText
                    primary={LanguageReducer?.languageType?.EDIT_TEXT}
                  />
                </ListItemButton>
              </ListItem>
            </List>
          </Box>
        </Menu>
        <DeleteConfirmationModal
          open={openDelete}
          setOpen={setOpenDelete}
          loading={loadingStates}
          handleDelete={handleDelete}
          heading={"Are you sure to disable this item?"}
          message={
            "After this action the item will be disabled immediately but you can undo this action anytime."
          }
          buttonText={"Disable"}
        />
      </Box>
    </>
  );
}
export default LowInventoryList;
