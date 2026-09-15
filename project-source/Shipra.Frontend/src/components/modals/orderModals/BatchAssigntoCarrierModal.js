import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import {
  Box,
  Card,
  Chip,
  CircularProgress,
  InputLabel,
  Paper,
  Stack,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomAutocompleteField from "../../../.reUseableComponents/TextField/CustomAutocompleteField";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  AssignToCarrier,
  CreateDeliveryTask,
  GetActiveCarrierPickupLocationForSelection,
  GetAllCountry,
  GetCalculatedRateByCarrier,
  GetCarrierWayBillsByOrderNos,
  GetDynamicApiCallWithURL,
  GetValidatedOrderAddressByActiveCarrier,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumAwbType, EnumOptions } from "../../../utilities/enum";
import {
  ActionButtonEdit,
  centerColumn,
  CodeBox,
  DataGridHeaderBox,
  fetchMethod,
  ProgressBarWithLabel,
  purple,
  StyledTooltip,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import UpdateAddressFromAssigntoCarrierModal from "./UpdateAddressFromAssigntoCarrierModal";
function BatchAssigntoCarrierModal(props) {
  let {
    open,
    setOpen,
    orderNosData,
    activeCarriers,
    getAllOrders,
    resetRowRef,
    isAssignInHouse,
    setIsAssignInHouse,
  } = props;
  const [chipData, setChipData] = React.useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);
  const [filterdActiveCarriers, setFilterdActiveCarriers] = useState([]);
  const [pickUpOrderData, setPickUpOrderData] = useState([]);
  const [updatedPickUpOrderData, setUpdatedPickUpOrderData] = useState([]);
  const [pickUpLocationData, setPickUpLocationData] = useState([]);
  const [allCountries, setAllCountries] = useState([]);
  const [progressBar, setProgressBar] = useState(0);
  const [progressBarLoading, setProgressBarLoading] = useState(false);
  const [pickUpLocationDataLodaing, setPickUpLocationDataLodaing] =
    useState(false);
  const [tableLoading, setTableLoading] = useState(false);
  const [carrierId, setCarrierId] = useState({
    activeCarrierId: 0,
    name: "Select Please",
  });
  const [configErrors, setConfigErrors] = useState({
    rows: [],
    data: {},
  });
  const [errors, setErrors] = useState({
    rows: [],
    data: {},
  });
  const [openUpdateAddressModal, setOpenUpdateAddressModal] = useState({
    open: false,
    AddressData: {},
    loading: {},
  });
  const [carrierSetting, setCarrierSetting] = useState({});

  const handleClose = () => {
    setOpen(false);
    setIsAssignInHouse(false);
  };

  const shouldShowDeliveryColumn = carrierId?.isRateCheck ? true : false;

  const handleAssigntoCarrier = async () => {
    if (!carrierId?.carrierId || carrierId.carrierId === 0) {
      errorNotification("Please choose carrier");
      return;
    }

    if (!carrierId?.isDispatchExCompany) {
      const hasEmptyPickupLocation = pickUpOrderData.some(
        (dt) => !dt.pickUpLocationId,
      );

      if (hasEmptyPickupLocation) {
        setPickUpOrderData((prev) =>
          prev.map((dt) => ({
            ...dt,
            hasPickUpLocationError: !dt.pickUpLocationId,
          })),
        );
        errorNotification(
          "Pick-up location is missing for some orders. Please select a pick-up location for all orders.",
        );
        return;
      }
    }

    if (!carrierId?.activeCarrierId || carrierId.activeCarrierId === 0) {
      errorNotification("Please select Carrier");
      return;
    }

    const validAddress = pickUpOrderData.every(
      (data) => data.isValidAddress === true || data.IsValidAddress === true,
    );

    if (!validAddress) {
      errorNotification(
        "Some orders have invalid addresses. Please correct them before assigning to the carrier.",
      );
      return;
    }

    const orderNosList = (chipData || []).map((item) => item.label);

    const orderPayload = pickUpOrderData.map(
      (data) => ({
        OrderNo: data.orderNo || data.OrderNo,
        ActiveCarrierPickupLocationId: data.pickUpLocationId,
        ServiceType: data.seletedDeliveryService?.productType,
      }),
    );

    const chunkSize = 10;
    const totalChunks = Math.ceil(orderPayload.length / chunkSize);

    const allAssignedOrderNos = [];
    const allErrors = {};
    const allConfigErrors = {};
    const errorRows = [];
    const configErrorRows = [];

    setIsLoading(true);
    setProgressBar(0);
    setProgressBarLoading(true);

    for (let i = 0; i < totalChunks; i++) {
      const chunk = orderPayload.slice(i * chunkSize, (i + 1) * chunkSize);
      const orderNosChunkArray = orderNosList.slice(
        i * chunkSize,
        (i + 1) * chunkSize,
      );
      const orderNosChunk = orderNosChunkArray.join();

      const param = {
        carrierId: carrierId.carrierId,
        ActiveCarrierId:
          carrierId.carrierContractTypeId === 2 ? 0 : carrierId.activeCarrierId,
        orderList: chunk,
        orderNos: orderNosChunk,
        CarrierContractTypeId: carrierId.carrierContractTypeId,
        ShipraContractCarrierId: carrierId.shipraContractCarrierId,
      };

      try {
        const res = await AssignToCarrier(param);

        if (!res?.data?.isSuccess) {
          const chunkKey = `${i + 1}`;

          if (res.data?.errors) {
            Object.keys(res.data.errors).forEach((orderNo) => {
              allErrors[chunkKey] = res.data.errors[orderNo];
              errorRows.push({ id: chunkKey, orderNo });
            });
          }

          if (res.data?.configErrors) {
            Object.keys(res.data.configErrors).forEach((orderNo) => {
              allConfigErrors[chunkKey] = res.data.configErrors[orderNo];
              configErrorRows.push({ id: chunkKey, orderNo });
            });
          }
        } else {
          allAssignedOrderNos.push(...orderNosChunkArray);
        }
      } catch (err) {
        console.error(`Chunk ${i + 1} failed`, err);
      }

      setProgressBar(Math.round(((i + 1) / totalChunks) * 100));
    }

    setProgressBarLoading(false);
    setIsLoading(false);

    if (allAssignedOrderNos.length > 0) {
      const { response } = await fetchMethod(() =>
        GetCarrierWayBillsByOrderNos({
          orderNos: allAssignedOrderNos.join(),
          awbTypeId: EnumAwbType.CarrierAwbTypeId,
        }),
      );

      if (response) {
        UtilityClass.downloadPdf(response, "Awb");
      }
    }

    setErrors({ data: allErrors, rows: errorRows });
    setConfigErrors({ data: allConfigErrors, rows: configErrorRows });

    if (errorRows.length === 0 && configErrorRows.length === 0) {
      setOpen(false);
      resetRowRef.current = true;
      getAllOrders();
    }
  };

  const handleAssignInHouse = () => {
    let param = {
      orderNos: chipData.map((item) => item.label).join(),
    };
    setIsLoading(true);
    CreateDeliveryTask(param)
      .then((res) => {
        setIsLoading(false);
        if (!res.data.isSuccess) {
          let jsonData = res.data.errors;
          for (const key in jsonData) {
            if (jsonData.hasOwnProperty(key)) {
              const messagesArray = jsonData[key];
              // Loop through the messages array for each key
              for (const message of messagesArray) {
                //   console.log(`${key}: ${message}`);
                errorNotification(message);
              }
            }
          }
        } else {
          successNotification("Order assign to carrier successfully");
          resetRowRef.current = true;
          getAllOrders();
          setOpen(false);
        }
      })
      .catch((e) => {
        setIsLoading(false);
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST,
        );
        console.error("e", e);
      });
  };
  const handleSubmitClick = () => {
    if (isAssignInHouse) {
      handleClose();
      handleAssignInHouse();
    } else {
      handleAssigntoCarrier();
    }
  };

  const handleGetDeliveryServices = async (
    data,
    defaultPickUpLocationId,
    carrierId,
  ) => {
    for (let i in data) {
      const ord_dt = data[i];
      // setPickUpOrderData((prev) => {
      //   const updatedData = [...prev];
      //   updatedData[i] = {
      //     ...ord_dt,
      //     deliveryServicesOptLoading: true,
      //     deliveryServicesOpt: [],
      //   };
      //   return updatedData;
      // });

      // const deliveryServicesOpt = await getCalculatedRateByCarrier(
      //   carrierId,
      //   ord_dt.orderNo,
      //   defaultPickUpLocationId
      // );
      // const deliveryServicesOptData = deliveryServicesOpt?.result?.rateResult;
      // const defaultDeliveryServicesOpt =
      //   deliveryServicesOptData?.length > 0 ? deliveryServicesOptData[0] : null;
      // const deliveryServicesOptError = deliveryServicesOpt?.errors;
      // setPickUpOrderData((prev) => {
      //   const updatedData = [...prev];
      //   updatedData[i] = {
      //     ...ord_dt,
      //     deliveryServicesOptLoading: false,
      //     deliveryServicesOpt: deliveryServicesOptData,
      //     seletedDeliveryService: defaultDeliveryServicesOpt,
      //     DeliveryServiceError: deliveryServicesOptError,
      //   };
      //   return updatedData;
      // });
    }
  };

  const getValidatedOrderAddressByActiveCarrier = async (
    defaultPickUpLocationOpt,
    carrierId,
    defaultPickUpLocationId,
  ) => {
    const body = {
      OrderNos: chipData.map((item) => item.label).join(),
      CarrierId: carrierId?.carrierId,
    };
    setPickUpLocationDataLodaing(true);
    try {
      const response = await GetValidatedOrderAddressByActiveCarrier(body);
      if (response?.data?.isSuccess) {
        let Others = {};
        let config = {};
        try {
          const obj = JSON.parse(carrierId?.config);
          config = JSON.parse(obj?.carriersetting);
          Others = config?.keys?.reduce((acc, dt) => {
            acc[dt] = "";
            return acc;
          }, {});
        } catch (e) {
          console.log("JSON is Invalid", e);
        }
        setCarrierSetting(config);
        const responseData = (response?.data?.result || []).map((dt) => {
          return {
            ...dt,
            hasPickUpLocationError: false,
            pickUpLocationId: defaultPickUpLocationId,
            selectedPickLocation: carrierId?.isDispatchExCompany
              ? defaultPickUpLocationOpt
              : null,
            seletedDeliveryService: null,
            DeliveryServiceError: [],
            isDispatchExCompany: carrierId?.isDispatchExCompany ?? false,
            Others,
            carrierSetting: [],
          };
        });
        setPickUpOrderData(responseData);
        if (carrierId?.isRateCheck && defaultPickUpLocationId) {
          handleGetDeliveryServices(
            responseData,
            defaultPickUpLocationId,
            carrierId,
          );
        }
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setPickUpLocationDataLodaing(false);
    }
  };

  const getActiveCarrierPickupLocationForSelection = async (carrierId) => {
    setTableLoading(true);
    let data = [];
    try {
      const response = await GetActiveCarrierPickupLocationForSelection(
        carrierId?.activeCarrierId,
        carrierId?.carrierId,
      );
      if (response?.data?.isSuccess) {
        data = response?.data?.result.filter((newData, index) => index !== 0);

        setPickUpLocationData(data);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setTableLoading(false);
    }
    return data;
  };

  const getCalculatedRateByCarrier = async (
    carrierId,
    orderNo,
    pickupLocation,
    Others = {},
  ) => {
    let data = [];
    const body = {
      carrierId: carrierId?.carrierId,
      ActiveCarrierId:
        carrierId?.carrierContractTypeId === 2 ? 0 : carrierId?.activeCarrierId,
      orderList: [
        {
          OrderNo: orderNo,
          ActiveCarrierPickupLocationId: pickupLocation,
          Others,
        },
      ],
      orderNos: orderNo,
      CarrierContractTypeId: carrierId?.carrierContractTypeId,
      ShipraContractCarrierId: carrierId?.shipraContractCarrierId,
    };
    try {
      const response = await GetCalculatedRateByCarrier(body);
      if (response?.data) {
        const _pickupDeliveryService = response?.data || [];
        data = _pickupDeliveryService;
      }
      if (!response?.isSuccess)
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
    } catch (e) {
      console.error(e);
    } finally {
      setTableLoading(false);
    }
    return data;
  };

  const handleChangeCarrier = async (e, newValue) => {
    const resolvedId = newValue ? newValue : null;
    setCarrierId(resolvedId);
    const isReolveIdSame =
      carrierId?.activeCarrierId === resolvedId?.activeCarrierId;
    const isResolveIdZero = !resolvedId || resolvedId.activeCarrierId == 0;
    if (isResolveIdZero && !isReolveIdSame) {
      setPickUpOrderData([]);
    }
    if (resolvedId && !isResolveIdZero && !isReolveIdSame) {
      const pickupOptios =
        await getActiveCarrierPickupLocationForSelection(resolvedId);
      const defaultPickUpLocationOpt =
        pickupOptios.length > 0 ? pickupOptios[0] : null;
      const defaultPickUpLocationId =
        pickupOptios.length > 0
          ? pickupOptios[0].activeCarrierPickupLocationId
          : null;
      await getValidatedOrderAddressByActiveCarrier(
        defaultPickUpLocationOpt,
        resolvedId,
        defaultPickUpLocationId,
      );
    }
  };

  const handlePickUpLocationChange = (orderNo, newValue) => {
    setPickUpOrderData((prevData) =>
      prevData.map((item) => {
        if (item.orderNo === orderNo) {
          const pickUpLocationId =
            newValue?.activeCarrierPickupLocationId ?? null;
          return {
            ...item,
            pickUpLocationId,
            selectedPickLocation: newValue,
            hasPickUpLocationError: item.hasPickUpLocationError
              ? !pickUpLocationId
              : false,
            deliveryServicesOptLoading: false,
            deliveryServicesOpt: [],
            seletedDeliveryService: null,
            DeliveryServiceError: [],
          };
        }
        return item;
      }),
    );
    if (
      carrierId?.isRateCheck &&
      newValue?.activeCarrierPickupLocationId &&
      Object.keys(carrierSetting).length === 0
    ) {
      (async () => {
        setPickUpOrderData((prevData) =>
          prevData.map((item) => {
            if (item.orderNo === orderNo) {
              return {
                ...item,
                deliveryServicesOptLoading: true,
              };
            }
            return item;
          }),
        );
        try {
          const deliveryServicesOpt = await getCalculatedRateByCarrier(
            carrierId,
            orderNo,
            newValue.activeCarrierPickupLocationId,
          );
          const deliveryServicesOptData =
            deliveryServicesOpt?.result?.rateResult;
          const deliveryServicesOptError = deliveryServicesOpt?.errors;
          setPickUpOrderData((prevData) =>
            prevData.map((item) => {
              if (item.orderNo === orderNo) {
                return {
                  ...item,
                  deliveryServicesOptLoading: false,
                  deliveryServicesOpt: deliveryServicesOptData,
                  seletedDeliveryService: !deliveryServicesOptData
                    ? null
                    : deliveryServicesOptData[0],
                  DeliveryServiceError: deliveryServicesOptError,
                };
              }
              return item;
            }),
          );
        } catch (error) {
          console.error("Error fetching delivery services:", error);
          setPickUpOrderData((prevData) =>
            prevData.map((item) => {
              if (item.orderNo === orderNo) {
                return {
                  ...item,
                  deliveryServicesOptLoading: false,
                  deliveryServicesOpt: [],
                  seletedDeliveryService: null,
                };
              }
              return item;
            }),
          );
        }
      })();
    }
  };

  const handleDeliveryServiceChange = (orderNo, newValue) => {
    setPickUpOrderData((prevData) =>
      prevData.map((item) => {
        if (item.orderNo === orderNo) {
          return {
            ...item,
            seletedDeliveryService: newValue,
          };
        }
        return item;
      }),
    );
  };

  const handleEditClick = (row) => {
    setOpenUpdateAddressModal((prev) => ({
      ...prev,
      open: true,
      AddressData: row,
    }));
  };

  const getValidatedOrderAddressByActiveCarrierForUpdateData = async () => {
    const body = {
      OrderNos: chipData.map((item) => item.label).join(),
      CarrierId: carrierId?.carrierId,
    };
    setPickUpLocationDataLodaing(true);
    try {
      const response = await GetValidatedOrderAddressByActiveCarrier(body);
      if (response?.data?.isSuccess) {
        setUpdatedPickUpOrderData(response?.data?.result ?? []);
      }
    } catch (e) {
      console.error(e);
    } finally {
      setPickUpLocationDataLodaing(false);
    }
  };

  let getAllCountry = async () => {
    try {
      const res = await GetAllCountry({});
      if (res.data.result != null) setAllCountries(res.data.result);
    } catch (e) {
      console.error(e);
    }
  };

  const getRowClassName = ({ row }) => {
    if (!(row.isValidAddress || row.IsValidAddress) || row.hasPickUpLocationError) {
      return "active-row";
    }
    return "";
  };

  const configColumns = [
    {
      field: "OrderNo",
      headerName: <DataGridHeaderBox title={"Title"} />,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.orderNo} />
          </>
        );
      },
    },
    {
      field: "Errors",
      headerName: <DataGridHeaderBox title={"Errors"} />,
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        const errorsForOrder = configErrors.data[row.id] || [];
        return (
          <>
            {errorsForOrder.map((dt, index) => (
              <Box
                key={index}
                style={{
                  padding: "4px 0",
                  whiteSpace: "normal",
                  wordBreak: "break-word",
                }}
              >
                {index + 1}. {dt}
              </Box>
            ))}
          </>
        );
      },
    },
  ];
  const columns = [
    {
      field: "OrderNo",
      headerName: <DataGridHeaderBox title={"Title"} />,
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.orderNo} />
          </>
        );
      },
    },
    {
      field: "Errors",
      headerName: <DataGridHeaderBox title={"Errors"} />,
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        const errorsForOrder = errors.data[row.id] || [];
        return (
          <>
            {errorsForOrder.map((dt, index) => (
              <Box
                key={index}
                style={{
                  padding: "4px 0",
                  whiteSpace: "normal",
                  wordBreak: "break-word",
                }}
              >
                {index + 1}. {dt}
              </Box>
            ))}
          </>
        );
      },
    },
  ];

  const handleDynamicFieldChange = async (orderNo, fieldKey, newValue) => {
    setPickUpOrderData((prevData) =>
      prevData.map((order) => {
        if (order.orderNo === orderNo) {
          return {
            ...order,
            Others: {
              ...order.Others,
              [fieldKey]: newValue,
            },
          };
        }
        return order;
      }),
    );
    const selectedRow = pickUpOrderData.find((dt) => dt.orderNo === orderNo);
    const selctedRow_pickUpLocationId = selectedRow?.pickUpLocationId;
    const OthersWithOptionObj = { ...selectedRow.Others, [fieldKey]: newValue };
    const Others = Object.entries(OthersWithOptionObj).reduce(
      (acc, [key, valObj]) => {
        acc[key] = valObj ? valObj[carrierSetting[key]?.src?.obj?.id] : "";
        return acc;
      },
      {},
    );

    const allRequiredKeysExists = Object.entries(Others).every(
      ([key, val]) => val && carrierSetting[key]?.required,
    );
    if (allRequiredKeysExists) {
      setPickUpOrderData((prevData) =>
        prevData.map((order) => {
          if (order.orderNo === orderNo) {
            return {
              ...order,
              deliveryServicesOptLoading: true,
              deliveryServicesOpt: [],
            };
          }
          return order;
        }),
      );
      const deliveryServicesOpt = await getCalculatedRateByCarrier(
        carrierId,
        orderNo,
        selctedRow_pickUpLocationId,
        Others,
      );
      const deliveryServicesOptData = deliveryServicesOpt?.result?.rateResult;
      const defaultDeliveryServicesOpt =
        deliveryServicesOptData?.length > 0 ? deliveryServicesOptData[0] : null;
      const deliveryServicesOptError = deliveryServicesOpt?.errors;
      setPickUpOrderData((prevData) =>
        prevData.map((order) => {
          if (order.orderNo === orderNo) {
            return {
              ...order,
              deliveryServicesOptLoading: false,
              deliveryServicesOpt: deliveryServicesOptData,
              seletedDeliveryService: defaultDeliveryServicesOpt,
              DeliveryServiceError: deliveryServicesOptError,
            };
          }
          return order;
        }),
      );
    }
  };

  const generateDynamicCarrierColumns = (carrierSetting) => {
    const dynamicColumns = [];
    if (Array.isArray(carrierSetting?.keys)) {
      carrierSetting.keys.forEach((fieldKey) => {
        const config = carrierSetting[fieldKey];
        if (config?.type === "select") {
          // If data hasn't been fetched yet, do it once
          if (config?.src?.url && config?.data?.length === 0) {
            fetchCarrierFieldOptions(fieldKey);
          }
          dynamicColumns.push({
            field: fieldKey,
            flex: 3,
            headerName: <Box sx={{ fontWeight: "600" }}>{config.label}</Box>,
            renderCell: ({ row }) => {
              const options = carrierSetting[fieldKey].data;
              const getOrderFieldValue = (data, orderNo, fieldKey) => {
                const order = data.find((o) => o.orderNo === orderNo);
                return order?.Others?.[fieldKey] || null;
              };
              return (
                <Box sx={{ p: 1, width: "100%" }}>
                  <CustomAutocompleteField
                    name={fieldKey}
                    options={options}
                    value={getOrderFieldValue(
                      pickUpOrderData,
                      row.orderNo,
                      fieldKey,
                    )}
                    getOptionLabel={(option) =>
                      option?.[config?.src?.obj?.label] || ""
                    }
                    onChange={(e, newValue) =>
                      handleDynamicFieldChange(row.orderNo, fieldKey, newValue)
                    }
                    size="md"
                    onKeyDown={(e) => {
                      if (e.ctrlKey && e.key === "a") {
                        e.stopPropagation();
                      }
                    }}
                  />
                </Box>
              );
            },
          });
        }
      });
    }
    return dynamicColumns;
  };

  const fetchCarrierFieldOptions = async (fieldKey) => {
    const config = carrierSetting[fieldKey];
    if (!config?.src?.url) return;

    try {
      // Replace placeholders
      const url = config.src.url
        .replace("{type}", fieldKey)
        .replace("{carrierid}", carrierId?.carrierId);

      const response = await GetDynamicApiCallWithURL(url);
      const options = response.data.result;

      // Update the original config with fetched options
      carrierSetting[fieldKey].data = options;
    } catch (error) {
      console.error(`Failed to fetch options for ${fieldKey}`, error);
    }
  };

  const orderColumns = [
    {
      field: "OrderNo",
      flex: 1,
      headerName: (
        <Stack direction={"column"}>
          <Box sx={{ fontWeight: "bold" }}>
            {LanguageReducer?.languageType?.ORDERS_ORDER_NO_REF_NO}
          </Box>
        </Stack>
      ),
      renderCell: (params) => {
        return (
          <Stack flexDirection={"row"} alignItems={"center"}>
            <Stack sx={{ textAlign: "" }} direction={"column"}>
              <CodeBox title={params.row.orderNo} onClick={(e) => {}} />
            </Stack>
          </Stack>
        );
      },
    },
    {
      field: "orderAddress",
      flex: 2,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Order Address"}</Box>,
      renderCell: (params) => {
        return (
          <Box disableRipple>
            <>
              <Box>{params.row.orderAddress}</Box>
            </>
          </Box>
        );
      },
    },
    {
      field: "PickUp Location",
      flex: 4,
      headerName: <Box sx={{ fontWeight: "600" }}>{"PickUp Location"}</Box>,
      renderCell: ({ row }) => {
        return (
          <>
            <Box sx={{ p: 1, width: "100%" }}>
              <CustomAutocompleteField
                name="pcikuplocation"
                options={pickUpLocationData}
                value={row.selectedPickLocation}
                getOptionLabel={(option) => option.fullAddress}
                onChange={(e, newValue) =>
                  handlePickUpLocationChange(row.orderNo, newValue)
                }
                size={"md"}
                onKeyDown={(e) => {
                  if (e.ctrlKey && e.key === "a") {
                    e.stopPropagation();
                  }
                }}
              />
            </Box>
            {row?.hasPickUpLocationError && (
              <StyledTooltip
                title={"Please add pickup location from active carreir"}
              ></StyledTooltip>
            )}
          </>
        );
      },
    },
    ...generateDynamicCarrierColumns(carrierSetting),
    ...(shouldShowDeliveryColumn
      ? [
          {
            field: "DeliveryServices",
            flex: 4,
            headerName: (
              <Box sx={{ fontWeight: "600" }}>{"Delivery Services"}</Box>
            ),
            renderCell: ({ row }) => {
              let deliveryErrorMessages = [];

              if (
                row?.DeliveryServiceError &&
                typeof row.DeliveryServiceError === "object"
              ) {
                Object.values(row.DeliveryServiceError)
                  .flat()
                  .forEach((errorItem) => {
                    try {
                      const parsedError =
                        typeof errorItem === "string"
                          ? JSON.parse(errorItem)
                          : errorItem;
                      deliveryErrorMessages.push(
                        ...(Array.isArray(parsedError)
                          ? parsedError.map((err) => err?.Messages || err)
                          : [parsedError]),
                      );
                    } catch {
                      deliveryErrorMessages.push(errorItem);
                    }
                  });
              }
              return (
                <>
                  <Box sx={{ p: 1, width: "100%" }}>
                    <CustomAutocompleteField
                      name="DeliveryServices"
                      options={row.deliveryServicesOpt || []}
                      value={row.seletedDeliveryService}
                      loading={row.deliveryServicesOptLoading}
                      getOptionLabel={(option) => option.text}
                      onChange={(e, newValue) =>
                        handleDeliveryServiceChange(row.orderNo, newValue)
                      }
                      size={"md"}
                      onKeyDown={(e) => {
                        if (e.ctrlKey && e.key === "a") {
                          e.stopPropagation();
                        }
                      }}
                    />
                  </Box>
                  {deliveryErrorMessages.length > 0 && (
                    <StyledTooltip title={deliveryErrorMessages.join(", ")} />
                  )}
                </>
              );
            },
          },
        ]
      : []),
    {
      ...centerColumn,
      field: "Action",
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.ORDERS_ACTION}
        </Box>
      ),
      renderCell: ({ row }) => {
        const invalidMessage = row.invalidProperties?.length
          ? `Please correct the following Address: ${row.invalidProperties
              .map((dt) => `${dt} is invalid`)
              .join(", ")}`
          : "No issues";
        return (
          <>
            {!(row?.isValidAddress || row?.IsValidAddress) && (
              <Box
                sx={{
                  marginLeft:
                    row.invalidProperties?.length > 0 ? "37px" : "0px",
                }}
              >
                <ActionButtonEdit onClick={() => handleEditClick(row)} />
              </Box>
            )}
            {row.invalidProperties?.length > 0 && !(row?.isValidAddress || row?.IsValidAddress) && (
              <StyledTooltip title={invalidMessage}></StyledTooltip>
            )}
          </>
        );
      },
    },
  ];

  useEffect(() => {
    if (activeCarriers && activeCarriers.length > 0) {
      // Get existing activeCarrierIds
      const existingIds = new Set(activeCarriers.map((x) => x.activeCarrierId));

      // Filter carriers that need a new activeCarrierId
      const carriersToUpdate = activeCarriers.filter(
        (x) => x.activeCarrierId === 0 && x.carrierContractTypeId === 2,
      );

      // Assign new activeCarrierId starting from 1, ensuring uniqueness
      let newId = 1;
      carriersToUpdate.forEach((carrier) => {
        while (existingIds.has(newId)) {
          newId++; // Increment to find an available ID
        }
        carrier.activeCarrierId = newId;
        existingIds.add(newId); // Mark this ID as used
      });
      setFilterdActiveCarriers([...activeCarriers]); // Update state
    }
  }, [activeCarriers]);

  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(orderNosData);
    setChipData(cData);
  }, [orderNosData]);

  useEffect(() => {
    if (updatedPickUpOrderData.length > 0) {
      setPickUpOrderData((prevData) =>
        prevData.map((order) => {
          const updatedOrder = updatedPickUpOrderData.find(
            (updOrder) => updOrder.orderId === order.orderId,
          );

          if (updatedOrder) {
            return {
              ...order,
              isValidAddress: updatedOrder.isValidAddress ?? updatedOrder.IsValidAddress,
              IsValidAddress: updatedOrder.isValidAddress ?? updatedOrder.IsValidAddress,
            };
          }

          return order;
        }),
      );
    }
  }, [updatedPickUpOrderData]);

  useEffect(() => {
    getAllCountry();
  }, []);

  return (
    <>
      <ModalComponent
        maxWidth="lg"
        open={open}
        onClose={handleClose}
        disabledDismissBtn={isLoading}
        title={
          !isAssignInHouse
            ? LanguageReducer?.languageType?.ORDER_BATCH_ASSIGN_TO_CARRIER
            : LanguageReducer?.languageType?.ORDER_ASSIGN_TO_CARRIER
        }
        actionBtn={
          <ModalButtonComponent
            bg={purple}
            loading={isLoading}
            onClick={handleSubmitClick}
            disabled={isLoading}
            title={
              isAssignInHouse
                ? LanguageReducer?.languageType?.ORDER_ASSIGN_IN_HOUSE
                : LanguageReducer?.languageType?.ORDER_ASSIGN_TO_CARRIER
            }
          />
        }
      >
        <ProgressBarWithLabel
          value={progressBar}
          loading={progressBarLoading}
        />
        {configErrors.rows.length > 0 && (
          <Box>
            <DataGridComponent
              autoHeight
              bgColor={"#fff"}
              getRowHeight={() => "auto"}
              headerHeight={40}
              sx={{
                fontFamily:
                  "'Lato Regular', 'Inter Regular', 'Arial' !important",
                fontSize: "12px",
                fontWeight: "500",
              }}
              rows={configErrors.rows}
              getRowId={(row) => row.id}
              columns={configColumns}
              checkboxSelection={false}
              disableSelectionOnClick
              pageSize={10}
              rowsPerPageOptions={[5, 10, 15, 25]}
            />
          </Box>
        )}{" "}
        <Card variant="outlined" sx={styleSheet.tagsCard}>
          <Paper
            sx={{
              display: "flex  !important",
              justifyContent: "flex-start  !important",
              flexWrap: "wrap  !important",
              p: 0.5,
              m: 0,
            }}
            elevation={0}
          >
            {chipData?.map((data) => {
              return (
                <Box key={data.key} sx={{ mr: "10px", mb: "8px" }}>
                  <Chip
                    sx={styleSheet.tagsChipStyle}
                    size="small"
                    icon={
                      <CheckCircleIcon
                        fontSize="small"
                        sx={{ color: "white  !important" }}
                      />
                    }
                    deleteIcon={
                      <CloseIcon sx={{ color: "white  !important" }} />
                    }
                    label={data.label}
                    // onDelete={() => { }}
                  />
                </Box>
              );
            })}
          </Paper>
        </Card>
        <br />
        {!isAssignInHouse && (
          <>
            <InputLabel sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.ORDER_SELECT_CARRIER}
            </InputLabel>
            <SelectComponent
              name="carrier"
              options={filterdActiveCarriers}
              value={carrierId}
              height={40}
              getOptionLabel={(option) => option.name || option.Name}
              optionLabel={EnumOptions.CARRIER.LABEL}
              optionValue={EnumOptions.CARRIER.VALUE}
              onChange={handleChangeCarrier}
              size={"md"}
            />
          </>
        )}
        <br />
        {tableLoading ? (
          <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            my={2}
          >
            <CircularProgress />
          </Box>
        ) : (
          pickUpOrderData.length > 0 && (
            <Box>
              <DataGridComponent
                autoHeight
                loading={pickUpLocationDataLodaing}
                bgColor={"#fff"}
                getRowHeight={() => "auto"}
                headerHeight={40}
                sx={{
                  fontFamily:
                    "'Lato Regular', 'Inter Regular', 'Arial' !important",
                  fontSize: "12px",
                  fontWeight: "500",
                }}
                rows={pickUpOrderData}
                getRowId={(row) => row.orderNo || row.OrderNo}
                columns={orderColumns}
                getRowClassName={getRowClassName}
                checkboxSelection={false}
                disableSelectionOnClick
                pageSize={25}
                rowsPerPageOptions={[5, 10, 15, 25]}
              />
            </Box>
          )
        )}
        {errors.rows.length > 0 && (
          <Box>
            <DataGridComponent
              autoHeight
              bgColor={"#fff"}
              getRowHeight={() => "auto"}
              headerHeight={40}
              sx={{
                fontFamily:
                  "'Lato Regular', 'Inter Regular', 'Arial' !important",
                fontSize: "12px",
                fontWeight: "500",
              }}
              rows={errors.rows}
              getRowId={(row) => row.id}
              columns={columns}
              checkboxSelection={false}
              disableSelectionOnClick
              pageSize={10}
              rowsPerPageOptions={[5, 10, 15, 25]}
            />
          </Box>
        )}
        {openUpdateAddressModal.open && (
          <UpdateAddressFromAssigntoCarrierModal
            open={openUpdateAddressModal}
            onClose={() =>
              setOpenUpdateAddressModal((prev) => ({ ...prev, open: false }))
            }
            rowData={openUpdateAddressModal.AddressData}
            CarrierID={carrierId?.carrierId}
            allCountries={allCountries}
            getValidatedOrderAddressByActiveCarrierForUpdateData={
              getValidatedOrderAddressByActiveCarrierForUpdateData
            }
          />
        )}
      </ModalComponent>
    </>
  );
}
export default BatchAssigntoCarrierModal;
