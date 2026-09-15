import FileDownloadOutlinedIcon from "@mui/icons-material/FileDownloadOutlined";
import { LoadingButton } from "@mui/lab";
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Menu,
  Stack,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import { DataGrid } from "@mui/x-data-grid";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import {
  CreateCarrierPaymentSettlementWithFile,
  GetActiveCarriersForSelection,
  GetCarrierSettlementSamplefile,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumPaymentMethod } from "../../../utilities/enum";
import {
  ActionButtonEdit,
  StyledTooltip,
  amountFormat,
  centerColumn,
  purple,
  rightColumn,
  usePagination,
  sampleRows,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import ImportSettlementModal from "./ImportSettlementModal";
import UpdateOrderAmountModal from "./updateOrderAmountModal";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function CreateSettlementModal(props) {
  let { open, setOpen, getAllCODPendings, carrierId } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [settlementData, setSettlementData] = useState([]);
  const [selectedSettlement, setSelectedSettlement] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [errorsList, setErrorsList] = useState([]);
  const [openImportSettlementModal, setOpenImportSettlementModal] =
    useState(false);
  const [openUpdateOrderAmountModal, setOpenUpdateOrderAmountModal] =
    useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    setOpen(false);
  };

  const downloadSample = async () => {
    let res = await GetCarrierSettlementSamplefile();
    // console.log("GetCarrierSettlementSamplefile", res.data);
    const link = document.createElement("a");
    link.href = res.data.result.url;
    link.download = "sampleCarrierSettlement.xlsx";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const [carrierData, setCarrierData] = useState([]);
  const getAllCarriersData = async () => {
    try {
      const response = await GetActiveCarriersForSelection();
      if (response?.data?.result) {
        setCarrierData(response?.data?.result);
      }
    } catch (error) {
      console.error("Error in updating getting these carriers", error.response);
      // errorNotification(
      //   LanguageReducer?.languageType?.UNABLE_TO_UPDATE_THE_PRODUCT_TOAST
      // );
    }
  };
  useEffect(() => {
    getAllCarriersData();
  }, []);

  const createCarrierPaymentSettlement = () => {
    setIsLoading(true);

    //console.log("settlementData", settlementData);
    let list = [];
    for (let index = 0; index < settlementData.length; index++) {
      // console.log("settlementData[index]", settlementData[index]);
      let tempObj = {
        orderNo: settlementData[index]?.orderNo,
        amount: settlementData[index]?.fileAmount,
        paymentRef: settlementData[index]?.paymentRef,
        paymentDate: settlementData[index]?.paymentDate,
      };
      list.push(tempObj);
    }
    let params = {
      carrierId: carrierId,
      list: list,
    };
    // console.log("params", params);
    CreateCarrierPaymentSettlementWithFile(params)
      .then((res) => {
        // console.log("res", res);
        if (res?.data?.isSuccess) {
          successNotification(
            LanguageReducer?.languageType
              ?.CARRIER_PAYMENT_SETTLEMENT_CREATED_SUCCESSFULLY_TOAST
          );
          getAllCODPendings();
          handleClose();
          setSettlementData([]);
          setSelectedSettlement(null);
        } else {
          if (res.data.errors) {
            UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
          }
        }
        setIsLoading(false);
      })
      .catch((e) => {
        if (e.response?.data?.errors) {
          UtilityClass.showErrorNotificationWithDictionary(
            e.response.data.errors
          );
        } else {
          errorNotification(
            LanguageReducer?.languageType
              ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
          );
        }

        console.log("e", e);
        setIsLoading(false);
      });
  };

  const columns = [
    {
      field: "carrierTrackingNo",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_TRACKING_NO}
        </Box>
      ),
    },
    {
      field: "paymentRef",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {
            LanguageReducer?.languageType
              ?.ACCOUNTS_COD_PENDING_PAYMENT_REFERENCE
          }
        </Box>
      ),
    },
    {
      field: "paymentStatus",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_PAYMENT_STATUS}
        </Box>
      ),
    },
    {
      field: "orderType",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_ORDER_TYPE}
        </Box>
      ),
    },
    {
      ...centerColumn,
      field: "paymentDate",
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_PAYMENT_DATE}
        </Box>
      ),
      renderCell: (params) => (
        <Box sx={{ textAlign: "center" }}>
          {UtilityClass.convertUtcToLocalAndGetDate(params.row.paymentDate)}
        </Box>
      ),
    },
    {
      ...rightColumn,
      field: "fileAmount",
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_FILE_COD}
        </Box>
      ),
      renderCell: (params) => (
        <Stack alignItems={"center"} direction={"row"} gap={1}>
          <Box>{amountFormat(params?.row?.fileAmount)}</Box>
          <Box>{<StyledTooltip title="COD value in file" />}</Box>
        </Stack>
      ),
    },
    {
      ...rightColumn,
      field: "dbAmount",
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_DB_COD}
        </Box>
      ),
      renderCell: (params) => (
        <Stack alignItems={"center"} direction={"row"} gap={1}>
          <Box>{amountFormat(params?.row?.dbAmount)}</Box>
          <Box>
            {
              <StyledTooltip
                title={
                  LanguageReducer?.languageType
                    ?.ACCOUNTS_COD_PENDING_VALUE_SET_WHEN_CREATING_ORDER
                }
              />
            }
          </Box>
        </Stack>
      ),
    },
    {
      ...rightColumn,
      field: "diffrence",
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_DIFFERENCE}
        </Box>
      ),
      renderCell: (params) => <Box>{amountFormat(params?.row?.diffrence)}</Box>,
    },

    {
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_ACTION}
        </Box>
      ),
      flex: 1,
      renderCell: (params) => {
        const index = errorsList.findIndex((error) => {
          // console.log(
          //   "error.Row == params.row.index",
          //   error.Row,
          //   params.row.index
          // );
          return error.Row == params.row.index && error.IsSuccessed == false;
        });
        //console.log("indexindexindex", index);
        return (
          <>
            {params?.row?.paymentMethodId ==
              EnumPaymentMethod.CashOnDelivery && (
              <Box>
                <ActionButtonEdit
                  onClick={(e) => {
                    setSelectedSettlement(params?.row);
                  }}
                />
                {index != -1 && (
                  <StyledTooltip title={errorsList[index].Msg}></StyledTooltip>
                )}
              </Box>
            )}
          </>
        );
      },
    },
  ];
  const getRowClassName = (params) => {
    // console.log("params.row.difference", params.row.diffrence);
    if (params?.row?.paymentMethodId == EnumPaymentMethod.Prepaid) {
      return "";
    }
    if (parseInt(params?.row.fileAmount) != parseInt(params?.row.dbAmount)) {
      return "active-row";
    }
    for (let i = 0; i < errorsList.length; i++) {
      if (
        params.row.index == errorsList[i].Row &&
        errorsList[i].IsSuccessed === false
      )
        return "active-row"; // CSS class name for active rows
    }
    return "";
  };
  useEffect(() => {
    // console.log("selectedSettlement", selectedSettlement);
    if (selectedSettlement) {
      setOpenUpdateOrderAmountModal(true);
    }
  }, [selectedSettlement]);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={
        LanguageReducer?.languageType
          ?.ACCOUNTS_COD_PENDING_CREATE_SETTLEMENT_FROM_FILE
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.ACCOUNTS_COD_PENDING_CREATE_SETTLEMENT
          }
          loading={isLoading}
          bg={purple}
          onClick={createCarrierPaymentSettlement}
        />
      }
      style={{ overflow: "hidden", height: "unset" }}
    >
      <Box sx={styleSheet.topNavBar} style={{ marginBottom: "15px" }}>
        <Box sx={styleSheet.topNavBarLeft}>
          <Button
            variant="contained"
            sx={{ ...styleSheet.addNewItemsButton }}
            onClick={() => setOpenImportSettlementModal(true)}
          >
            {LanguageReducer?.languageType?.ACCOUNTS_COD_PENDING_IMPORT}
          </Button>{" "}
          <Button
            variant="contained"
            sx={{ ...styleSheet.addNewItemsButtonOutlined }}
            onClick={() => downloadSample()}
          >
            <FileDownloadOutlinedIcon />
            {
              LanguageReducer?.languageType
                ?.ACCOUNTS_COD_PENDING_DOWNLOAD_SAMPLE
            }
          </Button>
        </Box>
        {/* <Stack
            sx={styleSheet.topNavBarRight}
            direction="row"
            justifyContent="flex-end"
            alignItems="center"
            spacing={1}
          >
            {isLoading ? (
              <Button
                fullWidth
                variant="contained"
                sx={styleSheet.addNewItemsButton}
                disabled
              >
                <CircularProgress
                  sx={{
                    color: "white",
                    height: "25px !important",
                    width: "25px !important",
                  }}
                />
              </Button>
            ) : (
              <Button
                onClick={createCarrierPaymentSettlement}
                variant="contained"
                sx={{ ...styleSheet.addNewItemsButton }}
              >
                {LanguageReducer?.languageType?.CONFIRM_TEXT}
              </Button>
            )}
          </Stack> */}
      </Box>
      <Box
        sx={styleSheet.allOrderTable}
        style={{ overflow: "auto", maxHeight: "calc(75vh - 200px)" }}
      >
        <DataGrid
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row.index}
          getRowClassName={getRowClassName}
          rows={settlementData}
          columns={columns}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 15, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          checkboxSelection
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
                    setOpenUpdateOrderAmountModal(true);
                    setAnchorEl(null);
                  }}
                >
                  <ListItemText
                    primary={LanguageReducer?.languageType?.EDIT_TEXT}
                  />
                </ListItemButton>
              </ListItem>
            </List>
          </Box>
        </Menu>
      </Box>
      {openImportSettlementModal ? (
        <ImportSettlementModal
          open={openImportSettlementModal}
          setOpen={setOpenImportSettlementModal}
          setSettlementData={setSettlementData}
          setErrorsList={setErrorsList}
          carrierData={carrierData}
        />
      ) : null}
      {selectedSettlement ? (
        <UpdateOrderAmountModal
          open={openUpdateOrderAmountModal}
          setOpen={setOpenUpdateOrderAmountModal}
          selectedSettlement={selectedSettlement}
          setSelectedSettlement={setSelectedSettlement}
          settlementData={settlementData}
          setSettlementData={setSettlementData}
        />
      ) : null}
    </ModalComponent>
  );
}
export default CreateSettlementModal;
