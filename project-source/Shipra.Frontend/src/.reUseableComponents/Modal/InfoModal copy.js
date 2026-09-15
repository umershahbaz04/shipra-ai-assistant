import {
  Box,
  Button,
  Dialog,
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
  CrossIconButton,
  CustomClipboardIcon,
  green,
  pink,
  placeholders,
  purple,
  yellow,
} from "../../utilities/helpers/Helpers";
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
import UtilityClass from "../../utilities/UtilityClass";
import ContentCopyOutlinedIcon from "@mui/icons-material/ContentCopyOutlined";
import { useForm, useWatch } from "react-hook-form";
import {
  EnumOrderType,
  EnumNotificationTypes,
  EnumAwbType,
  EnumCarrierTrackingStatus,
} from "../../../src/utilities/enum";
import {
  GoogleMap,
  Marker,
  useJsApiLoader,
  DirectionsRenderer,
  MarkerF,
} from "@react-google-maps/api";
import { useSelector } from "react-redux";

import { errorNotification, successNotification } from "../../utilities/toast";
import ButtonGroups from "../../components/shared/buttonGroup";
import { styleSheet } from "../../assets/styles/style";
import {
  GetOrderInvoiceByOrderNos,
  GetStripeInvoiceByUrl,
  GetWayBillsByOrderNos,
  SendNotificationToCustomer,
  UpdateCustomerEmail,
} from "../../api/AxiosInterceptors";
import BadgeComponent from "../Badge/BadgeComponent";
import CreditScoreIcon from '@mui/icons-material/CreditScore';
export default function InfoModal(props) {
  const { open, onClose, data } = props;

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
    },
    headerFont: {
      fontSize: 16,
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
    OrderAddressId: data.order.OrderAddressId,
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
          if (!res?.data?.isSuccess) {
            errorNotification(
              "Something went wrong while sending notification"
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
          successNotification("Somthing went wrong");
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
  const BriefDetals = () => {
    return (
      <Box>
        <Grid container spacing={1} pt={1}>
          <Grid item xs={6}>
            <Box
              component={Paper}
              sx={styles.paper}
              display={"flex"}
              gap={3}
              elevation={4}
            >
              <Box bgcolor={"#c6d2d9"} sx={styles.nameLogo} flexShrink={0}>
                <Typography fontSize={50}>
                  {UtilityClass.getTwoCharacters(
                    senderDetail.Name.toUpperCase()
                  )}
                </Typography>
              </Box>
              <Box display={"flex"} flexDirection={"column"} gap={1}>
                <Typography sx={styles.headerFont}>STORE NAME</Typography>
                <Typography sx={styles.headerFont} mt={-1}>
                  {senderDetail.Name}
                </Typography>

                <Box display={"flex"} alignItems={"center"} gap={1}>
                  <Box sx={styles.homeLogo} bgcolor={"#0000ff54"} flexShrink={0}>
                    <PhoneIcon sx={{ width: "15px", color: "#fff" }} />
                  </Box>
                  <Typography sx={styles.detailsFont}>
                    {senderDetail.Mobile}
                  </Typography>
                </Box>
                <Box display={"flex"} gap={1}>
                  <Box sx={styles.homeLogo} bgcolor={"#263238"} flexShrink={0}>
                    <HomeIcon sx={{ width: "15px", color: "#fff" }} />
                  </Box>
                  <Typography sx={styles.detailsFont}>
                    {senderDetail.Address}.
                  </Typography>
                </Box>
              </Box>
            </Box>
          </Grid>
          <Grid item xs={6}>
            <Box
              component={Paper}
              sx={styles.paper}
              display={"flex"}
              gap={3}
              elevation={4}
              height={"100%"}
            >
              <Box bgcolor={"var(--primary-color)47"} sx={styles.nameLogo} flexShrink={0}>
                <Typography fontSize={50}>
                  {UtilityClass.getTwoCharacters(
                    recieverDetail.Name.toUpperCase()
                  )}
                </Typography>
              </Box>
              <Box display={"flex"} flexDirection={"column"} gap={1}>
                <Typography sx={styles.headerFont}>CUSTOMER</Typography>
                <Typography sx={styles.headerFont} mt={-1}>
                  {recieverDetail.Name}
                </Typography>

                <Box display={"flex"} alignItems={"center"} gap={1}>
                  <Box sx={styles.homeLogo} bgcolor={"#0000ff54"} flexShrink={0}>
                    <PhoneIcon sx={{ width: "15px", color: "#fff" }} />
                  </Box>
                  <Typography sx={styles.detailsFont}>
                    {recieverDetail.Mobile}
                  </Typography>
                </Box>
                <Box display={"flex"} gap={1}>
                  <Box sx={styles.homeLogo} bgcolor={"#263238"} flexShrink={0}>
                    <HomeIcon sx={{ width: "15px", color: "#fff" }} />
                  </Box>
                  <Typography sx={styles.detailsFont}>
                    {recieverDetail.Address}.
                  </Typography>
                </Box>
              </Box>
            </Box>
          </Grid>

          <Grid item xs={6}>
            <Box component={Paper} sx={styles.paper} elevation={4}>
              <Typography sx={styles.headerFont} textAlign={"center"}>
                PAYMENT INFO
              </Typography>
              <Box display={"flex"} flexDirection={"column"} gap={3}>
                <Box className={"flex_between"}>
                  <Box display={"flex"} alignItems={"center"} gap={1}>
                    <Box
                      sx={{ ...styles.nameLogo, width: 25, height: 25 }}
                      bgcolor={"#118d1f6e"}
                    >
                      <CreditScoreIcon sx={{ width: "15px" }} />
                    </Box>
                    <Typography sx={styles.headerFont}>
                      Payment Method:{" "}
                      <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                        {data?.order?.PaymentMethod}{" "}
                      </Box>{" "}
                    </Typography>
                  </Box>
                </Box>
                <Box className={"flex_between"}>
                  <Typography sx={{ ...styles.detailsFont, fontWeight: 600,width:"100px" }}>
                    AMOUNT TO BE COLLECTED
                  </Typography>
                  <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                    {data?.order?.Amount}
                  </Typography>
                  <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                    Total : {data?.order?.Amount}
                  </Typography>
                </Box>
                <Box className={"flex_between"}>
                  <Typography sx={{ ...styles.detailsFont, fontWeight: 600,width:"100px" }}>
                    ITEM VALUE
                  </Typography>
                  <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                    {data?.order?.ItemValue}
                  </Typography>
                  <Typography sx={{ ...styles.detailsFont, fontWeight: 600 }}>
                    Total : {data?.order?.ItemValue}
                  </Typography>
                </Box>
              </Box>
            </Box>
          </Grid>
          <Grid item xs={6}>
            <Box
              component={Paper}
              sx={styles.paper}
              height={"100%"}
              elevation={4}
            >
              <Typography sx={styles.headerFont} textAlign={"center"}>
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
        </Grid>
      </Box>
    );
  };
  //   MapDetails
  const defaultLocation = { lat: 23.4241, lng: 53.8478 };

  const MapDetails = () => {
    const { isLoaded } = useJsApiLoader({
      googleMapsApiKey: process.env.REACT_APP_GOOGLE_API_KEY,
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
          travelMode: google.maps.TravelMode.DRIVING,
        },
        (result, status) => {
          if (status === google.maps.DirectionsStatus.OK) {
            setDirections(result);
          } else {
            console.error(`error fetching directions ${result}`);
          }
        }
      );
    };

    const onMapLoad = (map) => {
      directionsService = new google.maps.DirectionsService();
      //load default origin and destination
      changeDirection(origin, destination);
    };

    return (
      <>
        <Grid container>
          {isLoaded ? (
            <GoogleMap
              center={defaultLocation}
              zoom={12}
              onLoad={(map) => onMapLoad(map)}
              mapContainerStyle={{ height: "400px", width: "100%" }}
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
      </>
    );
  };
  const FulfillableOrderItems = () => {
    return (
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell>Product Name</TableCell>
              <TableCell align="right">Stock Sku</TableCell>
              <TableCell align="right">Price</TableCell>
              <TableCell align="right">Description</TableCell>
              <TableCell align="right">Quantity</TableCell>
              <TableCell align="right">Discount</TableCell>
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
                <TableCell align="right">{row.ProductStockSku}</TableCell>
                <TableCell align="right">{row.Price}</TableCell>
                <TableCell align="right">{row.OrderItemDescription}</TableCell>
                <TableCell align="right">{row.OrderItemQuantity}</TableCell>
                <TableCell align="right">{row.OrderItemDiscount}</TableCell>
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
        <Table sx={{ minWidth: 650 }} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell align="right">Price</TableCell>
              <TableCell align="right">Description</TableCell>
              <TableCell align="right">Quantity</TableCell>
              <TableCell align="right">Discount</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data?.orderItemsInfo?.map((row) => (
              <TableRow
                key={row.name}
                sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
              >
                <TableCell align="right">{row.Price}</TableCell>
                <TableCell align="right">{row.OrderItemDescription}</TableCell>
                <TableCell align="right">{row.OrderItemQuantity}</TableCell>
                <TableCell align="right">{row.OrderItemDiscount}</TableCell>
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

  const statusInfo = EnumCarrierTrackingStatus.properties[data?.order?.CarrierTrackingStatusId];
  console.log("statusinfo",  statusInfo)
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
        },
      }}
    >
      <DialogTitle>
        <Box className={"flex_between"}>
          <Box>
            <Typography sx={styles.headerFont}>
              Order No:{" "}
              <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                {data?.order?.OrderNo}{" "}
                <CustomClipboardIcon
                  fontSize="inherit"
                  sx={{ cursor: "pointer" }}
                  onClick={() =>
                    UtilityClass.copyToClipboard(data?.order?.OrderNo)
                  }
                />
              </Box>{" "}
            </Typography>
            <Typography sx={styles.headerFont}>
              Ref No :{" "}
              <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                {data?.order?.RefNo}
                {data?.order?.RefNo && (
                  <CustomClipboardIcon
                    fontSize="inherit"
                    onClick={() =>
                      UtilityClass.copyToClipboard(data?.order?.RefNo)
                    }
                  />
                )}
              </Box>{" "}
            </Typography>
          </Box>
          <Box>
            <Typography sx={styles.headerFont}>
              Order Type:{" "}
              <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                {data?.order?.OrderTypeName}
              </Box>
            </Typography>
            <Typography sx={styles.headerFont}>
              <Box sx={{display:"flex", alignItems:"center"}}>
                Tracking Status:
               <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                <BadgeComponent
                  title={data?.order?.TrackingStatus}
                  color={statusInfo.color}
                />
              </Box>{" "}
              </Box>
            </Typography>
          </Box>
          <Box>
            <Typography sx={styles.headerFont}>
              Order Date:{" "}
              <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                {UtilityClass.convertUtcToLocal(data?.order?.OrderDate)}
              </Box>
            </Typography>
            <Typography sx={styles.headerFont}>
              Carrier Name:{" "}
              <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                {data?.order?.CarrierName}
              </Box>{" "}
            </Typography>
          </Box>
          <Box>
            <Typography sx={styles.headerFont}>
              Created On:{" "}
              <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                {UtilityClass.convertUtcToLocal(data?.order?.CreatedOn)}
              </Box>
            </Typography>
            <Typography sx={styles.headerFont}>
              Carrier Tracking:{" "}
              <Box sx={{ fontWeight: "bold", m: 1, display: "inline" }}>
                {data?.order?.CarrierTrackingNo}{" "}
                {data?.order?.CarrierTrackingNo && (
                  <ContentCopyOutlinedIcon
                    fontSize="inherit"
                    sx={{ cursor: "pointer" }}
                    onClick={() =>
                      UtilityClass.copyToClipboard(
                        data?.order?.CarrierTrackingNo
                      )
                    }
                  />
                )}
              </Box>{" "}
            </Typography>
          </Box>
        </Box>
        <Box
          sx={{
            mt: 2,
            "& .MuiTabs-indicator": {
              display: "none",
            },
            "& .Mui-selected": {
              fontWeight: "600 !important",
            },
            display: "flex",
            justifyContent: "space-between",
          }}
        >
          <TabContext value={value}>
            <TabList onChange={handleChange} aria-label="lab API tabs example">
              <Tab
                label="Brief"
                value="1"
                disableRipple
                sx={{
                  textTransform: "none",
                }}
              />
              <Tab
                label="History"
                value="2"
                disableRipple
                sx={{ textTransform: "none" }}
              />
              <Tab
                label="Note"
                value="3"
                disableRipple
                sx={{ textTransform: "none" }}
              />
              <Tab
                label="Map"
                value="4"
                disableRipple
                sx={{ textTransform: "none" }}
              />
              <Tab
                label="Order Details"
                value="5"
                disableRipple
                sx={{ textTransform: "none" }}
              />
            </TabList>
          </TabContext>
          <Box sx={{ display: "flex", marginRight:"25px" }} gap={1}>
            {data?.order?.StripeInvoiceHostURL && (
              <ActionButtonCustom
                onClick={() =>
                  downloadStripeInvoice(data?.order?.StripeInvoicePDFURL)
                }
                label={" Print Stripe Invoice"}
                height={{ ...styleSheet.orderInfoModelButtn }}
              />
            )}
            {/* {data?.order?.OrderTypeId == EnumOrderType.FullFillable && ( */}

            <ActionButtonCustom
              onClick={() => downloadOrderInvoice(data?.order?.OrderNo)}
              label={" Print Invoice"}
              height={{ ...styleSheet.orderInfoModelButtn }}
            />
            {/* )} */}
            <ActionButtonCustom
              onClick={() =>
                downloadWayBillsByOrderNos(
                  data?.order?.OrderNo,
                  EnumAwbType.CarrierAwbTypeId
                )
              }
              label={" Print Carrier Label"}
              height={{ ...styleSheet.orderInfoModelButtn }}
            />
            <ActionButtonCustom
              onClick={() =>
                downloadWayBillsByOrderNos(
                  data?.order?.OrderNo,
                  EnumAwbType.Awb4x6TypeId
                )
              }
              height={{ ...styleSheet.orderInfoModelButtn }}
              label={"  Print Label"}
            />
          </Box>
        </Box>
      </DialogTitle>
      <DialogContent>
        <Box height={500} pt={1}>
          <TabContext value={value}>
            <TabPanel value="1" sx={{ paddingTop: 0 }}>
              <BriefDetals />
            </TabPanel>
            <TabPanel value="2" sx={{ paddingTop: 0 }}>
              <Box component={Paper} elevation={4} py={1}>
                <Timeline
                  sx={{
                    [`& .${timelineContentClasses.root}`]: {
                      flex: 7,
                    },
                  }}
                >
                  {data?.orderTrackingHistory.map((value, index, arr) => (
                    <TimelineItem>
                      <TimelineOppositeContent color="textSecondary" fontSize={"14px"}>
                        {UtilityClass.convertUtcToLocal(value.CreatedOn)}
                      </TimelineOppositeContent>
                      <TimelineSeparator>
                        <TimelineDot sx={styleSheet.timelineDot}/>
                        {index !== data?.orderTrackingHistory.length - 1 && (
                          <TimelineConnector />
                        )}
                      </TimelineSeparator>
                      <TimelineContent color={"#000"}>
                        <Typography component="div" sx={{ display: "inline" }}>
                          <Box sx={{ display: "inline" }}>
                            Status changed to
                          </Box>
                          <Box
                            sx={{ fontWeight: "bold", m: 1, display: "inline" }}
                          >
                            {value.TrackingStatus}
                          </Box>{" "}
                          by{" "}
                          <Box
                            sx={{ fontWeight: "bold", m: 1, display: "inline" }}
                          >
                            {" "}
                            {value.CreatedByName}
                          </Box>{" "}
                        </Typography>
                      </TimelineContent>
                    </TimelineItem>
                  ))}
                </Timeline>
              </Box>
            </TabPanel>
            <TabPanel value="3" sx={{ paddingTop: 0 }}>
              <Box component={Paper} elevation={4} py={1}>
                <Timeline
                  sx={{
                    [`& .${timelineContentClasses.root}`]: {
                      flex: 7,
                    },
                  }}
                >
                  {data?.orderNote.map((value, index, arr) => (
                    <TimelineItem>
                      <TimelineOppositeContent color="textSecondary" fontSize={"14px"}>
                        {UtilityClass.convertUtcToLocal(value.CreatedOn)}
                      </TimelineOppositeContent>
                      <TimelineSeparator>
                        <TimelineDot sx={styleSheet.timelineDot}/>
                        {index !== data?.orderNote.length - 1 && (
                          <TimelineConnector />
                        )}
                      </TimelineSeparator>
                      <TimelineContent color={"#000"}>
                        <Typography component="div" sx={{ display: "inline" }}>
                          <Box sx={{ display: "inline" }}>Note</Box>
                          <Box
                            sx={{ fontWeight: "bold", m: 1, display: "inline" }}
                          >
                            {value.NoteDescription}
                          </Box>{" "}
                          by{" "}
                          <Box
                            sx={{ fontWeight: "bold", m: 1, display: "inline" }}
                          >
                            {" "}
                            {value.CreatedByName}
                          </Box>{" "}
                        </Typography>
                      </TimelineContent>
                    </TimelineItem>
                  ))}
                </Timeline>
              </Box>
            </TabPanel>
            <TabPanel value="4" sx={{ paddingTop: 0 }}>
              <MapDetails />
            </TabPanel>
            <TabPanel value="5" sx={{ paddingTop: 0 }}>
              <Box component={Paper} elevation={4} py={2} px={2}>
                <Typography variant="h4">Description</Typography>
                <Typography variant="p">{data?.order?.Description}</Typography>
                <Typography pt={3} variant="h4">
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
    </Dialog>
  );
}
