import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Grid,
  InputLabel,
  Paper,
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
import React, { useRef, useState } from "react";
import {
  ActionButtonCustom,
  ClipboardIcon,
  CodeBox,
  CrossIconButton,
  DialerBox,
  GridContainer,
  GridItem,
  amountFormat,
  green,
  pink,
  placeholders,
  purple,
  useGetMapApiKeyReducer,
  useNavigateSetState,
  yellow,
} from "../../../utilities/helpers/Helpers";
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
import HomeIcon from "@mui/icons-material/Home";
import PhoneIcon from "@mui/icons-material/Phone";
import { blue } from "@mui/material/colors";
import UtilityClass from "../../../utilities/UtilityClass";
import ContentCopyOutlinedIcon from "@mui/icons-material/ContentCopyOutlined";
import { useForm, useWatch } from "react-hook-form";
import {
  EnumOrderType,
  EnumNotificationTypes,
  EnumAwbType,
  EnumCarrierTrackingStatus,
  EnumNavigateState,
} from "../../../utilities/enum";
import {
  GoogleMap,
  Marker,
  useLoadScript,
  DirectionsRenderer,
  MarkerF,
  useJsApiLoader,
} from "@react-google-maps/api";
import { useSelector } from "react-redux";

import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { styleSheet } from "../../../assets/styles/style";
import {
  GetOrderInvoiceByOrderNos,
  GetStripeInvoiceByUrl,
  GetWayBillsByOrderNos,
  SendNotificationToCustomer,
  UpdateCustomerEmail,
} from "../../../api/AxiosInterceptors";
import { useNavigate } from "react-router-dom";
import Colors from "../../../utilities/helpers/Colors";
import PaymentsIcon from "@mui/icons-material/Payments";
import BadgeComponent from "../../../.reUseableComponents/Badge/BadgeComponent";

export default function OrderItemDetailModal(props) {
  const { open, onClose, data } = props;
  const mapApiKey = useGetMapApiKeyReducer();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [value, setSelectedTabValue] = React.useState("1");
  const [showCustomerEmail, setShowCustomerEmail] = React.useState(false);

  const styles = {
    nameLogo: {
      width: 100,
      height: 100,
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
    Address: data?.order?.CustomerFullAddress,
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
  const downloadWayBillsByOrderNos = (orderNo, awbTypeId) => {
    const params = {
      orderNos: orderNo,
      awbTypeId: awbTypeId,
    };
    GetWayBillsByOrderNos(params)
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
          <Grid item xs={6}>
            <Box component={Paper} sx={styles.paper} elevation={4}>
              <Typography variant="h5" textAlign={"center"}>
                STORE NAME
              </Typography>
              <Box display={"flex"} gap={3}>
                <Box bgcolor={"#c6d2d9"} sx={styles.nameLogo} flexShrink={0}>
                  <Typography fontSize={50}>
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
          <Grid item xs={6}>
            <Box component={Paper} sx={styles.paper} elevation={4}>
              <Typography variant="h5" textAlign={"center"}>
                CUSTOMER
              </Typography>
              <Box display={"flex"} gap={3}>
                <Box bgcolor={"var(--primary-color)47"} sx={styles.nameLogo} flexShrink={0}>
                  <Typography fontSize={50}>
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

          <Grid item xs={6}>
            <Box component={Paper} sx={styles.paper} elevation={4}>
              <Typography variant="h5" textAlign={"center"}>
                PAYMENT INFO
              </Typography>
              <Box display={"flex"} flexDirection={"column"} gap={1.5}>
                <Box className={"flex_center"} gap={1}>
                  <Box display={"flex"} alignItems={"center"} gap={1}>
                    <Box bgcolor={Colors.succes} sx={styles.homeLogo}>
                      <PaymentsIcon
                        sx={{ width: "15px", color: "white" }}
                        fontSize="small"
                      />
                    </Box>
                    <Typography variant="h5">Payment Method:</Typography>
                  </Box>
                  <Typography variant="h5" fontWeight={400}>
                    {data?.order?.PaymentMethod}
                  </Typography>
                </Box>
                <Box className={"flex_between"}>
                  <GridContainer>
                    <GridItem>
                      <Typography variant={"h5"}>
                        Amount to be collected
                      </Typography>
                    </GridItem>

                    <GridItem className={"flex_between"}>
                      <Typography variant={"h5"} fontWeight={400}>
                        {amountFormat(data?.order?.Amount)}
                      </Typography>

                      <Typography variant={"h5"} fontWeight={400}>
                        Total : {amountFormat(data?.order?.Amount)}
                      </Typography>
                    </GridItem>
                  </GridContainer>
                </Box>
                <Box className={"flex_between"}>
                  <GridContainer>
                    <GridItem>
                      <Typography variant={"h5"}>Item Value</Typography>
                    </GridItem>
                    <GridItem className={"flex_between"}>
                      <Typography variant={"h5"} fontWeight={400}>
                        {data?.order?.ItemValue}
                      </Typography>
                      <Typography variant={"h5"} fontWeight={400}>
                        Total : {data?.order?.ItemValue}
                      </Typography>
                    </GridItem>
                  </GridContainer>
                </Box>
              </Box>
            </Box>
          </Grid>
          <Grid item xs={6} display={"none"}>
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
                <Button
                  variant="contained"
                  onClick={() =>
                    sendCustomerNotification(
                      data?.order?.OrderNo,
                      EnumNotificationTypes.SendPayment,
                    )
                  }
                  disabled={!data?.order?.StripeInvoiceHostURL}
                >
                  Click to send
                </Button>
              </Box>
              <Box className={"flex_between"} pt={1}>
                <Box display={"flex"} alignItems={"center"} gap={1}>
                  <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                    Send Notification For Shipping
                  </Typography>
                </Box>
                <Button
                  variant="contained"
                  onClick={() =>
                    sendCustomerNotification(
                      data?.order?.OrderNo,
                      EnumNotificationTypes.SendShipping,
                    )
                  }
                >
                  Click to send
                </Button>
              </Box>
              <Box className={"flex_between"} pt={1}>
                <Box display={"flex"} alignItems={"center"} gap={1}>
                  <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                    Send Invoice
                  </Typography>
                </Box>
                <Button
                  variant="contained"
                  onClick={() =>
                    sendCustomerNotification(
                      data?.order?.OrderNo,
                      EnumNotificationTypes.SendInvoice,
                    )
                  }
                >
                  Click to send
                </Button>
              </Box>
            </Box>
          </Grid>
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
            : data?.order?.CarrierTrackingStatusId ===
              EnumCarrierTrackingStatus.Delivered
            ? true
            : false,
      },
    });
  };
  const statusInfo =
    EnumCarrierTrackingStatus.properties[data?.order?.CarrierTrackingStatusId];

  return (
    <Dialog
      open={open}
      onClose={onClose}
      fullWidth
      maxWidth={"lg"}
      PaperProps={{
        sx: {
          background:
            "linear-gradient(to bottom, #eee 0%,#eee 40%,#fff 40%, #fff 100%)",
          borderRadius: "20px",
        },
      }}
    >
      <DialogTitle>
        <GridContainer>
          <GridItem xs={3}>
            <BriefHeader
              title={"Order No:"}
              value={data?.order?.OrderNo}
              copyBtn
            />
          </GridItem>
          <GridItem xs={3}>
            <BriefHeader
              title={"Order Type:"}
              value={data?.order?.OrderTypeName}
            />
          </GridItem>
          <GridItem xs={3}>
            <BriefHeader
              title={"Order Date:"}
              value={UtilityClass.convertUtcToLocal(data?.order?.OrderDate)}
            />
          </GridItem>
          <GridItem xs={3}>
            <BriefHeader
              title={"Created On:"}
              value={UtilityClass.convertUtcToLocal(data?.order?.CreatedOn)}
            />
          </GridItem>

          <GridItem xs={3} style={{ paddingTop: 0 }}>
            <BriefHeader title={"Ref No:"} value={data?.order?.RefNo} copyBtn />
          </GridItem>
          <GridItem xs={3} style={{ paddingTop: 0 }}>
            <BriefHeader
              title={"Tracking Status:"}
              value={data?.order?.TrackingStatus}
              statusColor={statusInfo.color}
            />
          </GridItem>
          <GridItem xs={3} style={{ paddingTop: 0 }}>
            <BriefHeader
              title={"Carrier Name:"}
              value={data?.order?.CarrierName}
            />
          </GridItem>
          <GridItem xs={3} style={{ paddingTop: 0 }}>
            <BriefHeader
              title={"Carrier Tracking:"}
              value={UtilityClass.convertUtcToLocal(
                data?.order?.CarrierTrackingNo,
              )}
            />
          </GridItem>
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
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <TabContext value={value}>
            <TabList onChange={handleChange} aria-label="lab API tabs example">
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
            </TabList>
          </TabContext>
          <Box sx={{ display: "flex" }} gap={1}>
            {/* {data?.order?.StripeInvoiceHostURL && (
              <Button
                variant="contained"
                sx={styleSheet.addStoreButton}
                onClick={() =>
                  downloadStripeInvoice(data?.order?.StripeInvoicePDFURL)
                }
              >
                Print Stripe Invoice
              </Button>
            )} */}
            {/* {data?.order?.OrderTypeId == EnumOrderType.FullFillable && (
              <Button
                variant="contained"
                sx={styleSheet.addStoreButton}
                onClick={() => downloadOrderInvoice(data?.order?.OrderNo)}
              >
                Print Invoice
              </Button>
            )} */}
            {/* <Button
              variant="contained"
              sx={styleSheet.addStoreButton}
              onClick={() =>
                downloadWayBillsByOrderNos(
                  data?.order?.OrderNo,
                  EnumAwbType.CarrierAwbTypeId
                )
              }
            >
              Print Carrier Label
            </Button> */}

            <ActionButtonCustom
              onClick={(event) => {
                handleEditClick(1);
              }}
              label={
                data?.order?.IsInhouseCarrier || data?.order?.CarrierId > 1000
                  ? "Update Order"
                  : data?.order?.TrackingStatus === "Delivered"
                    ? "View Order"
                    : "Update Order"
              }
              height={{ ...styleSheet.orderInfoModelButtn }}
            />
            {data?.order?.StripeInvoiceHostURL && (
              <ActionButtonCustom
                background={Colors.pdf}
                onClick={() =>
                  downloadStripeInvoice(data?.order?.StripeInvoicePDFURL)
                }
                label={" Print Stripe Invoice"}
                height={{ ...styleSheet.orderInfoModelButtn }}
              />
            )}
            <ActionButtonCustom
              background={Colors.pdf}
              onClick={() => downloadOrderInvoice(data?.order?.OrderNo)}
              label={" Print Invoice"}
              height={{ ...styleSheet.orderInfoModelButtn }}
            />
            <ActionButtonCustom
              background={Colors.succes}
              onClick={() =>
                downloadWayBillsByOrderNos(
                  data?.order?.OrderNo,
                  EnumAwbType.Awb4x6TypeId,
                )
              }
              label={"Print Label"}
              height={{ ...styleSheet.orderInfoModelButtn }}
            />
          </Box>
        </Box>
      </DialogTitle>
      <DialogContent>
        <Box height={400}>
          <TabContext value={value}>
            <TabPanel value="1" sx={{ padding: 0 }}>
              <BriefDetals />
            </TabPanel>
            <TabPanel value="2" sx={{ padding: 0 }}>
              <Box component={Paper} elevation={4} py={1}>
                <Timeline
                  sx={{
                    [`& .${timelineContentClasses.root}`]: {
                      flex: 5,
                    },
                  }}
                >
                  {data?.orderTrackingHistory.map((value, index, arr) => (
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
                        {index !== data?.orderTrackingHistory.length - 1 && (
                          <TimelineConnector />
                        )}
                      </TimelineSeparator>
                      <TimelineContent color={"#000"}>
                        <Typography variant="h5">
                          <span style={{ fontWeight: 400 }}>
                            Status changed to
                          </span>{" "}
                          {value.TrackingStatus}
                          <span style={{ fontWeight: 400 }}>by</span>{" "}
                          {value.CreatedByName}
                        </Typography>
                      </TimelineContent>
                    </TimelineItem>
                  ))}
                </Timeline>
              </Box>
            </TabPanel>
            <TabPanel value="3" sx={{ padding: 0 }}>
              <Box component={Paper} elevation={4} py={1}>
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
                          {value.NoteDescription}
                          <span style={{ fontWeight: 400 }}>by</span>{" "}
                          {value.CreatedByName}
                        </Typography>
                      </TimelineContent>
                    </TimelineItem>
                  ))}
                </Timeline>
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
    </Dialog>
  );
}
