import HomeIcon from "@mui/icons-material/Home";
import PhoneIcon from "@mui/icons-material/Phone";
import {
  TabContext,
  TabList,
  TabPanel,
  Timeline,
  TimelineConnector,
  TimelineContent,
  TimelineDot,
  TimelineItem,
  TimelineOppositeContent,
  TimelineSeparator,
  timelineContentClasses,
} from "@mui/lab";
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Grid,
  InputLabel,
  Paper,
  Stack,
  Tab,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import {
  DirectionsRenderer,
  GoogleMap,
  MarkerF,
  useJsApiLoader,
} from "@react-google-maps/api";
import React, { useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import {
  EnumAwbType,
  EnumDetailsModalType,
  EnumNavigateState,
  EnumOrderType,
} from "../../utilities/enum";
import {
  ActionButtonCustom,
  ClipboardIcon,
  CodeBox,
  DialerBox,
  GridContainer,
  GridItem,
  amountFormat,
  downloadWayBillsByOrderNos,
  getColorByName,
  handleCopyToClipBoard,
  placeholders,
  useGetMapApiKeyReducer,
  useNavigateSetState,
} from "../../utilities/helpers/Helpers";
import UtilityClass from "../../utilities/UtilityClass";

import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import PaymentsIcon from "@mui/icons-material/Payments";
import BadgeComponent from "../../.reUseableComponents/Badge/BadgeComponent";
import {
  CreateOrderNote,
  GetAllCarrierShipmentPodFiles,
  GetCarrierWayBillsByOrderNos,
  GetMetaFieldByEntityId,
  GetOrderInvoiceByOrderNos,
  GetValidateDocumentSetting,
  SendNotificationToCustomer,
  UpdateCustomerEmail,
} from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import AddBillingModal from "../../components/modals/orderModals/AddBillingModal";
import MetaFieldOrderDashboardModal from "../../components/modals/orderModals/MetaFieldOrderDashboardModal";
import Colors from "../../utilities/helpers/Colors";
import { errorNotification, successNotification } from "../../utilities/toast";
import FileViewver from "../FileViewer/FileViewer";

export default function OrderItemDetailModal(props) {
  const {
    open,
    onClose,
    data,
    isSendNotification,
    isUpdateOrderBtn,
    printCarrier,
    modelType,
    setIsNoteAdded,
    isNoteAdded,
    isMetaFieldExist,
    // loading,
    // orderNo
  } = props;
  const mapApiKey = useGetMapApiKeyReducer();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [value, setSelectedTabValue] = React.useState("1");
  const [showCustomerEmail, setShowCustomerEmail] = React.useState(false);
  const [openBillingOrder, setOpenBillingOrder] = useState(false);
  const [carrierPODFiles, setCarrierPODFiles] = useState([]);
  const [podLoading, setPodLoading] = useState(false);
  const [
    openMetaFieldOrderDashboardModel,
    setOpenMetaFieldOrderDashboardModel,
  ] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    getValues,
    reset,
    control,
  } = useForm();
  const styles = {
    nameLogo: {
      width: { md: 100, sm: 75, xs: 65 },
      height: { md: 100, sm: 75, xs: 65 },
      color: "#FFF",
      borderRadius: "50%",
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
    },
    homeLogo: {
      width: 25,
      height: 25,
      borderRadius: "50%",
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
    },
    paper: {
      p: 3,
      height: "100%",
    },
    headerFont: {
      fontSize: 14,
      fontWeight: 600,
    },
    detailsFont: {
      fontSize: 14,
      fontWeight: 400,
    },
  };

  const [documentSetting, setDocumentSetting] = useState({});
  const getOrdersRef = useRef([]);

  const handleMetaField = (data) => async () => {
    try {
      setOpenMetaFieldOrderDashboardModel((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data.OrderNo]: true },
      }));
      const response = await GetMetaFieldByEntityId(data.OrderId);
      if (response?.data?.isSuccess) {
        if (response.data.result.length > 0) {
          setOpenMetaFieldOrderDashboardModel((prev) => ({
            ...prev,
            open: true,
            data: response?.data?.result,
          }));
        } else {
          errorNotification("MetaField does not exist agaist this order");
        }
      }
    } catch (error) {
      console.error("Error fetching MetaField data:", error);
    } finally {
      setOpenMetaFieldOrderDashboardModel((prev) => ({
        ...prev,
        loading: { ...prev.loading, [data.OrderNo]: false },
      }));
    }
  };

  let getValidateDocumentSetting = async () => {
    let res = await GetValidateDocumentSetting();
    if (res.data.result !== null) {
      setDocumentSetting(res.data.result);
    }
  };
  useEffect(() => {
    getValidateDocumentSetting();
  }, []);
  useEffect(() => {
    setCarrierPODFiles([]);
  }, [open]);
  const creteNote = async (notedData) => {
    const body = {
      orderNo: data.order.OrderNo,
      noteDescription: notedData.note,
    };

    CreateOrderNote(body)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Unable to create note");
        } else {
          successNotification("Note create successfully");
          setIsNoteAdded((prev) => ({ ...prev, isAdded: true }));
          // onClose();
          reset();
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Unable to create note");
      });

    console.log("body", body);
  };
  const handleChange = (event, newValue) => {
    setSelectedTabValue(newValue);
  };

  const senderDetail = {
    Name: data?.order?.StoreName,
    Mobile: data?.order?.CustomerServiceNo,
    Address: data?.order?.StoreAddress,
  };
  const recieverDetail = {
    Name: data?.order?.CustomerName,
    Mobile: data?.order?.Mobile1,
    Address: data?.order?.CustomerAddress,
    Email: data?.order?.CustomerEmail,
    OrderAddressId: data.order?.OrderAddressId,
  };
  //   BriefDetals
  const sendCustomerNotification = (orderNo, notificationType) => {
    if (!recieverDetail.Email) {
      setShowCustomerEmail(true);
    } else {
      const params = {
        orderNo,
        notificationType,
      };
      SendNotificationToCustomer(params)
        .then((res) => {
          console.log("res:::", res);
          if (!res?.data?.isSuccess) {
            errorNotification(
              "Something went wrong while sending notification",
            );
            errorNotification(res?.data?.customErrorMessage);
            setIsLoading(false);
          } else {
            successNotification("Notification send successfully");
            setIsLoading(false);
          }
        })
        .catch((e) => {
          console.log("e", e);
          setIsLoading(false);
          errorNotification("Unable to send notification");
        });
    }
  };

  const [isLoading, setIsLoading] = useState(false);
  const emailRef = useRef("");
  const updateEmail = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    const params = {
      email: emailRef.current.value,
      orderAddressId: recieverDetail.OrderAddressId,
    };
    UpdateCustomerEmail(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          errorNotification("Unable to update email");
          errorNotification(res?.data?.customErrorMessage);
          setIsLoading(false);
        } else {
          successNotification("Email Update Successfully");
          setShowCustomerEmail(false);
          setIsLoading(false);
        }
      })
      .catch((e) => {
        setIsLoading(false);
        errorNotification("Something went wrong");
      });
  };

  const downloadStripeInvoice = (stripePdfUrl) => {
    const link = document.createElement("a");
    link.href = stripePdfUrl;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const getAllCarrierShipmentPodFile = async () => {
    setPodLoading(true);
    const body = {
      orderNo: data?.order?.OrderNo,
    };
    try {
      const response = await GetAllCarrierShipmentPodFiles(body);
      if (response?.data?.isSuccess) {
        if (response?.data?.result?.podFiles?.length > 0) {
          setCarrierPODFiles(response?.data?.result?.podFiles);
        } else {
          errorNotification("No carrier pod file found");
        }
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {
    } finally {
      setPodLoading(false);
    }
  };

  const downloadOrderInvoice = (orderNo) => {
    GetOrderInvoiceByOrderNos(orderNo)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadPdf(res.data, "Awb");
          setIsLoading(false);
        } else {
          successNotification("Notification send successfully");
          setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        setIsLoading(false);
        errorNotification("Unable to send notification");
      });
  };
  const downloadCarrierWayBillsByOrderNos = (orderNo, awbTypeId) => {
    const params = {
      orderNos: orderNo,
      awbTypeId: awbTypeId,
    };
    GetCarrierWayBillsByOrderNos(params)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.downloadPdf(res.data, "Awb");
          setIsLoading(false);
        } else {
          setIsLoading(false);
        }
      })
      .catch((e) => {
        console.log("e", e);
        setIsLoading(false);
      });
  };
  const BriefHeader = ({ title, value, copyBtn, statusColor }) => {
    return (
      <Box display={"flex"} gap={1}>
        <Typography variant="h5">{title}</Typography>
        <Box display={"flex"}>
          {!statusColor && (
            <Typography variant="h5" fontWeight={400}>
              {value}
            </Typography>
          )}
          {statusColor && value && (
            <BadgeComponent title={value} color={statusColor} />
          )}
          {copyBtn && value && (
            <ClipboardIcon sx={{ padding: "5px 10px" }} text={value} />
          )}
        </Box>
      </Box>
    );
  };
  const BriefDetals = () => {
    return (
      <Box>
        <Grid container spacing={1}>
          <Grid item sm={12} xs={12} md={6}>
            <Box component={Paper} sx={styles.paper} elevation={4}>
              <Typography variant="h5" textAlign={"center"}>
                STORE INFO
              </Typography>
              <Box
                sx={{
                  display: "flex",
                  flexDirection: { md: "row", sm: "row", xs: "column" },
                  alignItems: "center",
                  gap: { md: 3, sm: 3, xs: 1 },
                }}
              >
                <Box bgcolor={"#c6d2d9"} sx={styles.nameLogo} flexShrink={0}>
                  <Typography
                    sx={{ fontSize: { md: "50px", sm: "40px", xs: "35px" } }}
                  >
                    {UtilityClass.getTwoCharacters(
                      senderDetail.Name.toUpperCase(),
                    )}
                  </Typography>
                </Box>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                  <Typography sx={styles.headerFont}>
                    {senderDetail.Name}
                  </Typography>

                  <Box display={"flex"} alignItems={"center"} gap={1}>
                    <Box
                      sx={styles.homeLogo}
                      bgcolor={"#0000ff54"}
                      flexShrink={0}
                    >
                      <PhoneIcon
                        sx={{ width: "15px", color: "#fff" }}
                        fontSize="small"
                      />
                    </Box>
                    <Typography variant="h5" fontWeight={400}>
                      <DialerBox phone={senderDetail.Mobile} />
                    </Typography>
                  </Box>
                  <Box display={"flex"} gap={1}>
                    <Box
                      sx={styles.homeLogo}
                      bgcolor={"#263238"}
                      flexShrink={0}
                    >
                      <HomeIcon
                        sx={{ width: "15px", color: "#fff" }}
                        fontSize="small"
                      />
                    </Box>
                    <Typography sx={styles.detailsFont}>
                      {senderDetail.Address}.
                    </Typography>
                  </Box>
                </Box>
              </Box>
            </Box>
          </Grid>
          <Grid item sm={12} xs={12} md={6}>
            <Box component={Paper} sx={styles.paper} elevation={4}>
              <Typography variant="h5" textAlign={"center"}>
                CUSTOMER INFO
              </Typography>
              <Box
                sx={{
                  display: "flex",
                  flexDirection: { md: "row", sm: "row", xs: "column" },
                  alignItems: "center",
                  gap: { md: 3, sm: 3, xs: 1 },
                }}
              >
                <Box bgcolor={"var(--primary-color)47"} sx={styles.nameLogo} flexShrink={0}>
                  <Typography
                    sx={{ fontSize: { md: "50px", sm: "40px", xs: "35px" } }}
                  >
                    {UtilityClass.getTwoCharacters(
                      recieverDetail.Name.toUpperCase(),
                    )}
                  </Typography>
                </Box>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                  <Typography sx={styles.headerFont}>
                    {recieverDetail.Name}
                  </Typography>

                  <Box display={"flex"} alignItems={"center"} gap={1}>
                    <Box
                      sx={styles.homeLogo}
                      bgcolor={"#0000ff54"}
                      flexShrink={0}
                    >
                      <PhoneIcon
                        sx={{ width: "15px", color: "#fff" }}
                        fontSize="small"
                      />
                    </Box>
                    <Typography sx={styles.detailsFont}>
                      <DialerBox phone={recieverDetail.Mobile} />
                    </Typography>
                  </Box>
                  <Box display={"flex"} gap={1}>
                    <Box
                      sx={styles.homeLogo}
                      bgcolor={"#263238"}
                      flexShrink={0}
                    >
                      <HomeIcon
                        sx={{ width: "15px", color: "#fff" }}
                        fontSize="small"
                      />
                    </Box>
                    <Typography sx={styles.detailsFont}>
                      {recieverDetail.Address}.
                    </Typography>
                  </Box>
                </Box>
              </Box>
            </Box>
          </Grid>

          <Grid item sm={12} xs={12} md={6} mb={1}>
            <Box
              component={Paper}
              sx={{ ...styles.paper, p: { xs: 2, sm: 3 } }}
              elevation={4}
            >
              <Typography variant="h5" textAlign="center">
                PAYMENT INFO
              </Typography>
              <Box display="flex" flexDirection="column" gap={1.5}>
                <Box
                  className="flex_center"
                  gap={1}
                  flexDirection={{ xs: "column", sm: "row" }}
                  alignItems={{ xs: "flex-start", sm: "center" }}
                >
                  <Box display="flex" alignItems="center" gap={1}>
                    <Box bgcolor={Colors.succes} sx={styles.homeLogo}>
                      <PaymentsIcon
                        sx={{ width: "15px", color: "white" }}
                        fontSize="small"
                      />
                    </Box>
                    <Typography variant="h5">Payment Method:</Typography>
                    <Typography variant="h5" fontWeight={400}>
                      {data?.order?.PaymentMethod}
                    </Typography>
                  </Box>
                </Box>
                <Box
                  className="flex_between"
                  flexDirection={{ xs: "column", sm: "row" }}
                >
                  <GridContainer>
                    <GridItem>
                      <Typography variant="h5">
                        Amount to be collected
                      </Typography>
                    </GridItem>
                    <GridItem
                      className="flex_between"
                      flexDirection={{ xs: "column", sm: "row" }}
                    >
                      <Typography variant="h5" fontWeight={400}>
                        {amountFormat(data?.order?.Amount)}
                      </Typography>
                      <Typography variant="h5" fontWeight={400}>
                        Total : {amountFormat(data?.order?.Amount)}
                      </Typography>
                    </GridItem>
                  </GridContainer>
                </Box>
                <Box
                  className="flex_between"
                  flexDirection={{ xs: "column", sm: "row" }}
                >
                  <GridContainer>
                    <GridItem>
                      <Typography variant="h5">Item Value</Typography>
                    </GridItem>
                    <GridItem
                      className="flex_between"
                      flexDirection={{ xs: "column", sm: "row" }}
                    >
                      <Typography variant="h5" fontWeight={400}>
                        {data?.order?.ItemValue}
                      </Typography>
                      <Typography variant="h5" fontWeight={400}>
                        Total : {data?.order?.ItemValue}
                      </Typography>
                    </GridItem>
                  </GridContainer>
                </Box>
              </Box>
            </Box>
          </Grid>

          {(data?.order?.Description || data?.order?.Remarks) && (
            <Grid item sm={12} xs={12} md={6} mb={1}>
              <Box component={Paper} sx={styles.paper} elevation={4}>
                {data?.order?.Description && (
                  <Box sx={{ alignItems: "center" }}>
                    <Typography variant="h4">Description</Typography>
                    <Typography ml={1} mb={1} sx={styles.detailsFont}>
                      {data?.order?.Description}
                    </Typography>
                  </Box>
                )}
                {data?.order?.Remarks && (
                  <Box sx={{ alignItems: "center" }}>
                    <Typography variant="h5">Remark</Typography>
                    <Typography ml={1} sx={styles.detailsFont}>
                      {data?.order?.Remarks}
                    </Typography>
                  </Box>
                )}
              </Box>
            </Grid>
          )}

          {/* {isSendNotification && (
            <Grid item xs={6}>
              <Box
                component={Paper}
                sx={styles.paper}
                height={"100%"}
                elevation={4}
              >
                <Typography variant="h5" textAlign={"center"}>
                  SEND NOTIFICATION
                </Typography>
                {showCustomerEmail && <EmailComponent />}
                <Box className={"flex_between"}>
                  <Box display={"flex"} alignItems={"center"} gap={1}>
                    <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                      Send Notification For Payment
                    </Typography>
                  </Box>
                  <ActionButtonCustom
                    onClick={() =>
                      sendCustomerNotification(
                        data?.order?.OrderNo,
                        EnumNotificationTypes.SendPayment
                      )
                    }
                    disabled={!data?.order?.StripeInvoiceHostURL}
                    label={" Click to send"}
                    height={"35px"}
                  />
                </Box>
                <Box className={"flex_between"} pt={1}>
                  <Box display={"flex"} alignItems={"center"} gap={1}>
                    <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                      Send Notification For Shipping
                    </Typography>
                  </Box>
                  <ActionButtonCustom
                    onClick={() =>
                      sendCustomerNotification(
                        data?.order?.OrderNo,
                        EnumNotificationTypes.SendShipping
                      )
                    }
                    label={" Click to send"}
                    height={"35px"}
                  />
                </Box>
                <Box className={"flex_between"} pt={1}>
                  <Box display={"flex"} alignItems={"center"} gap={1}>
                    <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                      Send Invoice
                    </Typography>
                  </Box>
                  <ActionButtonCustom
                    onClick={() =>
                      sendCustomerNotification(
                        data?.order?.OrderNo,
                        EnumNotificationTypes.SendInvoice
                      )
                    }
                    label={" Click to send"}
                    height={"35px"}
                  />
                </Box>
              </Box>
            </Grid>
          )} */}
        </Grid>
      </Box>
    );
  };
  //   MapDetails
  const defaultLocation = { lat: 23.4241, lng: 53.8478 };

  const MapDetails = () => {
    const { isLoaded } = useJsApiLoader({
      googleMapsApiKey: mapApiKey ?? process.env.REACT_APP_GOOGLE_API_KEY,
      libraries: ["places"],
    });

    const [directions, setDirections] = useState(null);
    const mapLocation = data?.mapLocation;
    let destination = {
      lat: mapLocation.destinationLatitude
        ? mapLocation.destinationLatitude
        : 0,
      lng: mapLocation.destinationLongitude
        ? mapLocation.destinationLongitude
        : 0,
    };
    let origin = {
      lat: mapLocation.origionLatitude ? mapLocation.origionLatitude : 0,
      lng: mapLocation.origionLongitude ? mapLocation.origionLongitude : 0,
    };
    let directionsService;

    //function that is calling the directions service
    const changeDirection = (origin, destination) => {
      directionsService.route(
        {
          origin: origin,
          destination: destination,
          travelMode: window.google.maps.TravelMode.DRIVING,
        },
        (result, status) => {
          if (status === window.google.maps.DirectionsStatus.OK) {
            setDirections(result);
          } else {
            console.error(`error fetching directions ${result}`);
          }
        },
      );
    };

    const onMapLoad = (map) => {
      directionsService = new window.google.maps.DirectionsService();
      //load default origin and destination
      changeDirection(origin, destination);
    };
    return (
      <Grid container>
        {isLoaded ? (
          <GoogleMap
            center={defaultLocation}
            zoom={12}
            onLoad={(map) => onMapLoad(map)}
            mapContainerStyle={{
              height: "400px",
              width: "100%",
              borderRadius: "4px",
            }}
          >
            {
              <>
                {directions && <DirectionsRenderer directions={directions} />}
                <MarkerF position={defaultLocation} />
              </>
            }
          </GoogleMap>
        ) : (
          "Loading..."
        )}
      </Grid>
    );
  };
  const FulfillableOrderItems = () => {
    return (
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label="simple table" size="small">
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 600 }}>Product Name</TableCell>
              <TableCell sx={{ fontWeight: 600 }} align="left">
                Stock Sku
              </TableCell>
              <TableCell sx={{ fontWeight: 600 }} align="right">
                Price
              </TableCell>
              <TableCell sx={{ fontWeight: 600 }} align="left">
                Description
              </TableCell>
              <TableCell sx={{ fontWeight: 600 }} align="center">
                Quantity
              </TableCell>
              <TableCell sx={{ fontWeight: 600 }} align="right">
                Discount
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data?.orderItemsInfo?.map((row) => (
              <TableRow
                key={row.name}
                sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
              >
                <TableCell component="th" scope="row">
                  {row.ProductName}
                </TableCell>
                <TableCell align="left">
                  <CodeBox title={row.ProductStockSku} />
                </TableCell>
                <TableCell align="right">{amountFormat(row.Price)}</TableCell>
                <TableCell align="left">{row.OrderItemDescription}</TableCell>
                <TableCell align="center">
                  <CodeBox title={row.OrderItemQuantity} />
                </TableCell>
                <TableCell align="right">
                  {amountFormat(row.OrderItemDiscount)}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    );
  };
  const RegularOrderItems = () => {
    return (
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label="simple table" size="small">
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 600 }} align="right">
                Price
              </TableCell>
              <TableCell sx={{ fontWeight: 600 }} align="right">
                Description
              </TableCell>
              <TableCell sx={{ fontWeight: 600 }} align="right">
                Quantity
              </TableCell>
              <TableCell sx={{ fontWeight: 600 }} align="right">
                Discount
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data?.orderItemsInfo?.map((row) => (
              <TableRow
                key={row.name}
                sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
              >
                <TableCell align="right">{amountFormat(row.Price)}</TableCell>
                <TableCell align="right">{row.OrderItemDescription}</TableCell>
                <TableCell align="right">{row.OrderItemQuantity}</TableCell>
                <TableCell align="right">
                  {amountFormat(row.OrderItemDiscount)}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    );
  };
  const EmailComponent = () => {
    return (
      <Box pb={1}>
        <Box>
          <InputLabel required>Email</InputLabel>
          <TextField
            type="text"
            size="small"
            id="email"
            name="email"
            fullWidth
            variant="outlined"
            placeholder={placeholders.email}
            inputRef={emailRef}
          ></TextField>
        </Box>
        <Box display={"flex"} justifyContent={"right"}>
          <Button
            sx={{
              marginTop: "5px",
              display: "flex",
              justifyContent: "right",
            }}
            variant="contained"
            onClick={(e) => updateEmail(e)}
          >
            Save
          </Button>
        </Box>
      </Box>
    );
  };
  // const navigate = useNavigate();
  const { setNavigateState } = useNavigateSetState();
  const handleEditClick = () => {
    const stData = {
      order: data.order,
    };
    let oNavaigate = "/edit-order-regular";
    if (data?.order.OrderTypeId == EnumOrderType.FullFillable) {
      oNavaigate = "/edit-order-fulfillable";
    }
    setNavigateState(oNavaigate, {
      stData,
      [EnumNavigateState.EDIT_ORDER.pageName]: {
        [EnumNavigateState.EDIT_ORDER.state.disabledInput]:
          data?.order?.IsInhouseCarrier || data?.order?.CarrierId > 1000
            ? false
            : data?.order?.CarrierId
            ? true
            : false,
      },
    });
  };

  const statusInfo = getColorByName(data?.order?.TrackingStatus);

  const orderNumber = [data?.order?.OrderNo];
  const handleWayBillsByOrderNos = () => {
    if (documentSetting.DocumentTemplateId !== null) {
      downloadWayBillsByOrderNos(
        data?.order?.OrderNo,
        documentSetting.DocumentTemplateId,
      );
    } else {
      setOpenBillingOrder(true);
    }
  };
  return (
    <Dialog
      open={open}
      onClose={onClose}
      fullWidth
      maxWidth={"lg"}
      PaperProps={{
        sx: {
          background:
            "linear-gradient(to bottom, #eee 0%,#eee 75%,#fff 40%, #fff 100%)",
          borderRadius: "20px",
          height: "610px",
        },
      }}
    >
      <DialogContent>
        <Box height={500}>
          <GridContainer spacing={1}>
            <GridItem md={3} sm={6} xs={12}>
              <BriefHeader
                title={"Order No:"}
                value={data?.order?.OrderNo}
                copyBtn
              />
            </GridItem>
            <GridItem md={3} sm={6} xs={12}>
              <BriefHeader
                title={"Ref No:"}
                value={data?.order?.RefNo}
                copyBtn
              />
            </GridItem>
            <GridItem md={3} sm={6} xs={12}>
              <BriefHeader
                title={"Order Type:"}
                value={data?.order?.OrderTypeName}
              />
            </GridItem>
            <GridItem md={3} sm={6} xs={12}>
              <BriefHeader
                title={"Order Date:"}
                value={UtilityClass.convertUtcToLocalAndGetDate(
                  data?.order?.OrderDate,
                )}
              />
            </GridItem>
            <GridItem md={3} sm={6} xs={12}>
              <BriefHeader
                title={"Created On:"}
                value={UtilityClass.convertUtcToLocal(data?.order?.CreatedOn)}
              />
            </GridItem>
            <GridItem md={3} sm={6} xs={12}>
              <BriefHeader
                title={"Tracking Status:"}
                value={data?.order?.TrackingStatus}
                statusColor={statusInfo.color}
              />
            </GridItem>
            <GridItem md={3} sm={6} xs={12}>
              <BriefHeader
                title={"Carrier Name:"}
                value={data?.order?.CarrierName}
              />
            </GridItem>
            <GridItem md={3} sm={6} xs={12}>
              <BriefHeader
                title={"Delivery TypeName:"}
                value={data?.order?.DeliveryTypeName}
                statusColor={"#ff9800"}
              />
            </GridItem>
            {data?.order?.CarrierTrackingNo && (
              <GridItem md={3} sm={6} xs={12}>
                <BriefHeader
                  title={"Carrier Tracking No:"}
                  value={data?.order?.CarrierTrackingNo}
                />
              </GridItem>
            )}
          </GridContainer>
          <Box
            sx={{
              mt: 2,
              "& .MuiTabs-indicator": {
                display: "none",
              },
              "& .Mui-selected": {
                fontWeight: "600 !important",
              },
              "& .MuiTabs-flexContainer": {
                gap: 5,
              },
              display: { lg: "flex" },
              justifyContent: "space-between",
              alignItems: "center",
              mt: 2,
              mb: 3,
            }}
          >
            <TabContext value={value}>
              <TabList
                onChange={handleChange}
                variant="scrollable"
                aria-label="lab API tabs example"
                sx={{ gap: "15px" }}
              >
                <Tab
                  label="Brief"
                  value="1"
                  disableRipple
                  sx={{
                    fontWeight: 600,
                    color: "rgba(0, 0, 0, 0.87)",
                    textTransform: "none",
                    padding: 0,
                    minWidth: 0,
                  }}
                />
                <Tab
                  label="History"
                  value="2"
                  disableRipple
                  sx={{
                    fontWeight: 600,
                    color: "rgba(0, 0, 0, 0.87)",
                    textTransform: "none",
                    padding: 0,
                    minWidth: 0,
                  }}
                />
                <Tab
                  label="Note"
                  value="3"
                  disableRipple
                  sx={{
                    fontWeight: 600,
                    color: "rgba(0, 0, 0, 0.87)",
                    textTransform: "none",
                    padding: 0,
                    minWidth: 0,
                  }}
                />
                <Tab
                  label="Map"
                  value="4"
                  disableRipple
                  sx={{
                    fontWeight: 600,
                    color: "rgba(0, 0, 0, 0.87)",
                    textTransform: "none",
                    padding: 0,
                    minWidth: 0,
                  }}
                />
                <Tab
                  label="Order Details"
                  value="5"
                  disableRipple
                  sx={{
                    fontWeight: 600,
                    color: "rgba(0, 0, 0, 0.87)",
                    textTransform: "none",
                    padding: 0,
                    minWidth: 0,
                  }}
                />
                <Tab
                  label="POD Files"
                  value="6"
                  disableRipple
                  sx={{
                    fontWeight: 600,
                    color: "rgba(0, 0, 0, 0.87)",
                    textTransform: "none",
                    padding: 0,
                    minWidth: 0,
                  }}
                />
              </TabList>
            </TabContext>
            <Grid
              container
              spacing={1}
              sx={{
                justifyContent: "flex-end",
                flexWrap: { xs: "wrap", md: "nowrap" },
              }}
            >
              <Grid item>
                <ActionButtonCustom
                  background={Colors.view}
                  onClick={() =>
                    data?.trackUrl && window.open(data.trackUrl, "_blank")
                  }
                  label={"Copy Tracking Url"}
                  height={{ ...styleSheet.orderInfoModelButtn }}
                  startIcon={
                    <ContentCopyIcon
                      fontSize="small"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleCopyToClipBoard(data?.trackUrl);
                      }}
                    />
                  }
                />
              </Grid>

              {printCarrier && (
                <Grid item>
                  <ActionButtonCustom
                    onClick={() =>
                      downloadCarrierWayBillsByOrderNos(
                        data?.order?.OrderNo,
                        EnumAwbType.CarrierAwbTypeId,
                      )
                    }
                    label={"Print Carrier Label"}
                    height={{ ...styleSheet.orderInfoModelButtn }}
                  />
                </Grid>
              )}

              {isUpdateOrderBtn && (
                <Grid item>
                  <ActionButtonCustom
                    onClick={(event) => handleEditClick(1)}
                    label={
                      data?.order?.IsInhouseCarrier || data?.order?.CarrierId > 1000
                        ? "Update Order"
                        : data?.order?.CarrierId
                        ? "View Order"
                        : "Update Order"
                    }
                    height={{ ...styleSheet.orderInfoModelButtn }}
                  />
                </Grid>
              )}

              {data?.order?.StripeInvoiceHostURL && (
                <Grid item>
                  <ActionButtonCustom
                    background={Colors.pdf}
                    onClick={() =>
                      downloadStripeInvoice(data?.order?.StripeInvoicePDFURL)
                    }
                    label={"Print Stripe Invoice"}
                    height={{ ...styleSheet.orderInfoModelButtn }}
                  />
                </Grid>
              )}

              {data?.order?.StripeInvoiceHostURL &&
                modelType === EnumDetailsModalType.ORDER && (
                  <Grid item>
                    <ActionButtonCustom
                      background={Colors.pdf}
                      onClick={() =>
                        handleCopyToClipBoard(data?.order?.StripeInvoiceHostURL)
                      }
                      label={"Copy Payment Link"}
                      height={{ ...styleSheet.orderInfoModelButtn }}
                    />
                  </Grid>
                )}

              <Grid item>
                <ActionButtonCustom
                  background={Colors.pdf}
                  onClick={() => downloadOrderInvoice(data?.order?.OrderNo)}
                  label={"Print Invoice"}
                  height={{ ...styleSheet.orderInfoModelButtn }}
                />
              </Grid>
              {isMetaFieldExist === 1 && (
                <Grid item>
                  <Box>
                    <ActionButtonCustom
                      label="MetaField"
                      loading={
                        openMetaFieldOrderDashboardModel.loading[
                          data?.order?.OrderNo
                        ]
                      }
                      height={{ ...styleSheet.orderInfoModelButtn }}
                      onClick={handleMetaField(data?.order)}
                    />
                  </Box>
                </Grid>
              )}
              <Grid item>
                <ActionButtonCustom
                  background={Colors.succes}
                  onClick={() => handleWayBillsByOrderNos()}
                  label={"Print Label"}
                  height={{ ...styleSheet.orderInfoModelButtn }}
                />
              </Grid>
            </Grid>
          </Box>
          <TabContext value={value}>
            <TabPanel value="1" sx={{ padding: 0 }}>
              <BriefDetals />
            </TabPanel>
            <TabPanel value="2" sx={{ padding: 0 }}>
              <Box
                component={Paper}
                elevation={4}
                py={1}
                sx={{ height: "350px", overflowY: "auto" }}
              >
                <Grid container spacing={2}>
                  <Grid item md={8}>
                    <Typography variant="h3" fontWeight="600" m={2}>
                      History
                    </Typography>{" "}
                    <Timeline
                      sx={{
                        [`& .${timelineContentClasses.root}`]: {
                          flex: 5,
                        },
                      }}
                    >
                      {data?.orderTrackingHistory.map((value, index, arr) => (
                        <TimelineItem key={index}>
                          {" "}
                          <TimelineOppositeContent
                            color="textSecondary"
                            fontSize={"14px"}
                          >
                            <Typography variant="h5" fontWeight={400}>
                              {UtilityClass.convertUtcToLocal(value.CreatedOn)}
                            </Typography>
                          </TimelineOppositeContent>
                          <TimelineSeparator>
                            <TimelineDot sx={styleSheet.timelineDot} />
                            {index !==
                              data?.orderTrackingHistory.length - 1 && (
                              <TimelineConnector />
                            )}
                          </TimelineSeparator>
                          <TimelineContent color={"#000"}>
                            <Typography variant="h5">
                              <span style={{ fontWeight: 400 }}>
                                Status changed to
                              </span>{" "}
                              {value.TrackingStatus}{" "}
                              <span style={{ fontWeight: 400 }}>by</span>{" "}
                              {value.CreatedByName}
                            </Typography>
                          </TimelineContent>
                        </TimelineItem>
                      ))}
                    </Timeline>
                  </Grid>
                  <Grid item md={4}>
                    <Typography variant="h3" fontWeight="600" m={2}>
                      Remarks
                    </Typography>{" "}
                    {data?.order?.Remarks ? (
                      <Typography
                        variant="h5"
                        fontWeight="400"
                        padding={"6px 16px"}
                      >
                        {data.order.Remarks}
                      </Typography>
                    ) : (
                      <Typography
                        variant="h5"
                        fontWeight="400"
                        padding={"6px 16px"}
                      >
                        No remarks
                      </Typography>
                    )}
                  </Grid>
                </Grid>
              </Box>
            </TabPanel>

            <TabPanel value="3" sx={{ padding: 0 }}>
              <Box component={Paper} elevation={4} py={1}>
                <form onSubmit={handleSubmit(creteNote)}>
                  <Stack sx={{ padding: "20px" }} direction="row" spacing={2}>
                    <Box sx={{ marginBottom: "10px", width: "100%" }}>
                      <InputLabel required sx={styleSheet.inputLabel}>
                        Add Note
                      </InputLabel>
                      <TextField
                        placeholder=""
                        size="small"
                        id="note"
                        name="note"
                        fullWidth
                        variant="outlined"
                        {...register("note", {
                          required: {
                            value: true,
                            message:
                              LanguageReducer?.languageType
                                ?.FIELD_REQUIRED_TEXT,
                          },
                          pattern: {
                            value: /^(?!\s*$).+/,
                            message:
                              LanguageReducer?.languageType
                                ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                          },
                        })}
                        error={Boolean(errors.note)} // set error prop
                        helperText={errors.channenotelName?.message}
                      />
                    </Box>

                    <Box
                      item
                      xs={2}
                      sx={{
                        display: "flex",
                        alignItems: "flex-end",
                        marginBottom: "10px",
                        flex: "none",
                      }}
                    >
                      <ActionButtonCustom
                        label={"Add Note"}
                        type="submit"
                        height={{ ...styleSheet.orderInfoModelButtn }}
                      />
                    </Box>
                  </Stack>
                </form>
                {isNoteAdded.loading[isNoteAdded.orderNo] ? (
                  <Box sx={{ padding: "20px" }}>
                    <Typography
                      sx={{ ...styleSheet.orderInfoModelLoadingText }}
                    >
                      Loading
                    </Typography>
                  </Box>
                ) : (
                  <Timeline
                    sx={{
                      [`& .${timelineContentClasses.root}`]: {
                        flex: 5,
                      },
                    }}
                  >
                    {data?.orderNote.map((value, index, arr) => (
                      <TimelineItem>
                        <TimelineOppositeContent
                          color="textSecondary"
                          fontSize={"14px"}
                        >
                          <Typography variant="h5" fontWeight={400}>
                            {UtilityClass.convertUtcToLocal(value.CreatedOn)}
                          </Typography>
                        </TimelineOppositeContent>
                        <TimelineSeparator>
                          <TimelineDot sx={styleSheet.timelineDot} />
                          {index !== data?.orderNote.length - 1 && (
                            <TimelineConnector />
                          )}
                        </TimelineSeparator>
                        <TimelineContent color={"#000"}>
                          <Typography variant="h5">
                            <span style={{ fontWeight: 400 }}>Note</span>{" "}
                            {value.NoteDescription}{" "}
                            <span style={{ fontWeight: 400 }}>by</span>{" "}
                            {value.CreatedByName}
                          </Typography>
                        </TimelineContent>
                      </TimelineItem>
                    ))}
                  </Timeline>
                )}
              </Box>
            </TabPanel>
            <TabPanel value="4" sx={{ padding: 0 }}>
              <MapDetails />
            </TabPanel>
            <TabPanel value="5" sx={{ padding: 0 }}>
              <Box component={Paper} elevation={4} py={2} px={2}>
                <Typography variant="h5">Description</Typography>
                <Typography variant="h5">{data?.order?.Description}</Typography>
                <Typography pt={3} variant="h4" mb={2}>
                  Items
                </Typography>
                {data?.order.OrderTypeId == EnumOrderType.FullFillable ? (
                  <FulfillableOrderItems />
                ) : (
                  <RegularOrderItems />
                )}
              </Box>
            </TabPanel>
            <TabPanel value="6" sx={{ padding: 0 }}>
              <Box component={Paper} elevation={4} py={2} px={2}>
                <Grid container spacing={2}>
                  {data?.podFiles.length ? (
                    data?.podFiles.map((file) => (
                      <Grid
                        item
                        lg={3}
                        md={4}
                        xs={12}
                        key={file.orderPodfileId}
                      >
                        <FileViewver file={file} />
                      </Grid>
                    ))
                  ) : (
                    <Grid item xs={12}>
                      {data?.order?.CarrierId === null && (
                        <Box sx={{ padding: "100px", textAlign: "center" }}>
                          No POD file found
                        </Box>
                      )}
                    </Grid>
                  )}
                  {data?.order?.CarrierId > 0 && (
                    <Grid item lg={12} md={12} xs={12}>
                      {carrierPODFiles.length ? (
                        carrierPODFiles?.map((file_dt) =>
                          file_dt?.fileUrls?.map((file) => (
                            <Grid item xs={12} key={file.orderPodfileId}>
                              <b>{file_dt?.name}</b>
                              <FileViewver file={{ filePath: file }} />
                            </Grid>
                          )),
                        )
                      ) : (
                        <Grid item xs={12} sm={12} md={12}>
                          <Box
                            sx={{
                              display: "flex",
                              justifyContent: "center",
                              padding: "100px",
                            }}
                          >
                            <ActionButtonCustom
                              label={"View Carrier POD File"}
                              onClick={() => getAllCarrierShipmentPodFile()}
                              loading={podLoading}
                            />
                          </Box>
                        </Grid>
                      )}
                    </Grid>
                  )}
                </Grid>
              </Box>
            </TabPanel>
          </TabContext>
        </Box>
      </DialogContent>
      <DialogActions sx={{ p: 0 }}>
        <Button
          fullWidth
          variant="contained"
          sx={styleSheet.modalDismissButton}
          onClick={() => {
            onClose();
          }}
        >
          {"Dismiss"}
        </Button>
      </DialogActions>
      {openBillingOrder && (
        <AddBillingModal
          open={openBillingOrder}
          orderNosData={orderNumber}
          onClose={() => setOpenBillingOrder(false)}
          documentSetting={documentSetting}
        />
      )}
      {openMetaFieldOrderDashboardModel.open && (
        <MetaFieldOrderDashboardModal
          onClose={() =>
            setOpenMetaFieldOrderDashboardModel((prev) => ({
              ...prev,
              open: false,
            }))
          }
          data={openMetaFieldOrderDashboardModel.data}
          open={openMetaFieldOrderDashboardModel.open}
        />
      )}
    </Dialog>
  );
}
