import { Box, InputLabel, TextField, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreaetNotificationConfig,
  DeleteNotificationConfig,
  GetAllCarrierTrackingStatusForSelection,
  GetAllSMSActivate,
  GetAllWhatsappActivate,
  GetNotificationConfigByTypeId,
  GetNotificationEvents,
  GetNotificationTypes,
  UpdateNotificationConfig,
} from "../../../api/AxiosInterceptors";
import emailIcon from "../../../assets/images/email.svg";
import selectedEmailIcon from "../../../assets/images/email_selected.svg";
import smsIcon from "../../../assets/images/message.svg";
import selectedSmsIcon from "../../../assets/images/message_selected.svg";
import whatsAppIcon from "../../../assets/images/whatsapp.svg";
import selectedWhatsAppIcon from "../../../assets/images/whatsApp_selected.svg";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions, inputTypesEnum } from "../../../utilities/enum";
import Colors from "../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  checkObjHasSpecificProperty,
  CrossIconButton,
  fetchMethod,
  getBooleanFromObject,
  getOptionValueObjectByValue,
  GreyBox,
  GridContainer,
  GridItem,
  lightgrey,
  navbarHeight,
  purple,
  red,
  selectedTabBg,
  useClientSubscriptionReducer,
  useGetWindowHeight,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { useSelector } from "react-redux";

const handleGetNotificationEvents = async () => {
  const { response } = await fetchMethod(GetNotificationEvents);
  const options = response?.result?.filter(
    (item) => item.notificationEventId !== 0
  );
  return options;
};

const handleGetAllCarrierTrackingStatusForSelection = async () => {
  const { response } = await fetchMethod(
    GetAllCarrierTrackingStatusForSelection
  );
  const options = response?.result;
  return options;
};

const handleGAllSMSActivate = async () => {
  const { response } = await fetchMethod(() =>
    GetAllSMSActivate({
      filterModel: {
        createdFrom: null,
        createdTo: null,
        start: 0,
        length: 1000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
    })
  );
  const id = response?.result?.list;
  return id;
};
const handleGAllWhatsAppActivate = async () => {
  const { response } = await fetchMethod(() => GetAllWhatsappActivate());
  const id = response?.result?.list;
  return id;
};

const notificationEventsOptionsMethodsByType = Object.freeze({
  1: handleGetNotificationEvents,
  3: handleGetNotificationEvents,
});

const notificationServiceRowsOptionsLabelAndValue = Object.freeze({
  1: {
    label: EnumOptions.NOTIFICATION_SERVICE.LABEL,
    value: EnumOptions.NOTIFICATION_SERVICE.VALUE,
  },
  3: {
    label: EnumOptions.WHATSAPP_NOTIFICATION_SERVICE.LABEL,
    value: EnumOptions.WHATSAPP_NOTIFICATION_SERVICE.VALUE,
  },
});

const notificationEventsRowsOptionsLabelAndValue = Object.freeze({
  1: {
    2: {
      label: EnumOptions.CARRIER_TRACKING_STATUS.LABEL,
      value: EnumOptions.CARRIER_TRACKING_STATUS.VALUE,
    },
  },
  3: {
    2: {
      label: EnumOptions.CARRIER_TRACKING_STATUS.LABEL,
      value: EnumOptions.CARRIER_TRACKING_STATUS.VALUE,
    },
  },
});

const notificationEventsRowsOptionsMethods = Object.freeze({
  1: { 2: handleGetAllCarrierTrackingStatusForSelection },
  3: { 2: handleGetAllCarrierTrackingStatusForSelection },
});

const notificationServiceMethodsByType = Object.freeze({
  1: handleGAllSMSActivate,
  3: handleGAllWhatsAppActivate,
});

const notificationTypeImgs = Object.freeze({
  1: { deseleced: smsIcon, selected: selectedSmsIcon },
  2: { deseleced: emailIcon, selected: selectedEmailIcon },
  3: { deseleced: whatsAppIcon, selected: selectedWhatsAppIcon },
});

const notificationSchema = [
  {
    notificationTypeId: 1,
    events: [
      {
        notificationEventId: 1,
        schema: [
          {
            type: "text",
          },
        ],
      },
      {
        notificationEventId: 2,
        schema: [
          {
            type: "select",
            multiple: true,
          },
          {
            type: "text",
          },
        ],
      },
    ],
  },
  {
    notificationTypeId: 3,
    events: [
      {
        notificationEventId: 1,
        schema: [
          {
            type: "text",
          },
        ],
      },
      {
        notificationEventId: 2,
        schema: [
          {
            type: "select",
            multiple: true,
          },
          {
            type: "text",
          },
        ],
      },
      {
        notificationEventId: 3,
        schema: [
          {
            type: "text",
          },
        ],
      },
    ],
  },
];

const transformNotificationType = (notificationType) => {
  return notificationType.events.reduce((acc, event) => {
    acc[event.notificationEventId] = event.schema.map((item) => {
      if (item.type === "text") {
        return { ...item, value: "" };
      } else if (item.type === "select") {
        return { ...item, value: [], options: [] };
      }
      return item;
    });
    return acc;
  }, {});
};

const transformSubmitdata = (notificationType) => {
  return notificationType.events.reduce((acc, event) => {
    acc[event.notificationEventId] = [];
    return acc;
  }, {});
};

const NotificationPage = () => {
  const [selectedNotificationSchema, setSelectedNotificationSchema] = useState(
    {}
  );
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const initialSubmitdataFields = {
    saveBtnClicked: false,
    loading: false,
    data: {},
    selectedNotificationEvent: {},
    selectedActivatedService: {},
    selectedNotificationType: null,
    selectedNotificationTypeLabel: "",
    updateMode: false,
  };
  const [submitdata, setSubmitdata] = useState(initialSubmitdataFields);
  const [notificationEventOptions, setNotificationEventOptions] = useState([]);
  const [notificationServiceOptions, setNotificationServiceOptions] = useState(
    []
  );
  const [notificationTypes, setNotificationTypes] = useState([]);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);

  const getNotificationTypes = async () => {
    const { response } = await fetchMethod(GetNotificationTypes);
    let filteredResult = response?.result?.filter(
      (item) => item.notificationTypeId !== 0
    );
    if (response?.isSuccess) {
      setNotificationTypes(filteredResult);
    }
  };

  const handleSetSelectedNotificationEventOptions = async (typeId, eventId) => {
    let _options = [];
    const hasOptionsExist = selectedNotificationSchema[eventId]?.some(
      (dt) => dt.type === inputTypesEnum.SELECT && dt.options.length > 0
    );
    if (
      eventId &&
      checkObjHasSpecificProperty(
        notificationEventsRowsOptionsMethods[typeId],
        eventId
      ) &&
      !hasOptionsExist
    ) {
      _options = await notificationEventsRowsOptionsMethods[typeId][eventId]();
      setSelectedNotificationSchema((prev) => {
        const _selectedNotificationSchema = { ...prev };
        _selectedNotificationSchema[eventId] = _selectedNotificationSchema[
          eventId
        ]?.map((dt) =>
          dt.type === inputTypesEnum.SELECT
            ? {
                ...dt,
                options: _options,
              }
            : dt
        );
        return _selectedNotificationSchema;
      });
    }
    return _options;
  };

  const handleGetNotificationConfigByTypeId = async (NotificationTypeId) => {
    let servicesOptions = [];
    let selectedNotificationEventOptions = [];
    const { response } = await fetchMethod(() =>
      GetNotificationConfigByTypeId(NotificationTypeId)
    );
    const responseData = response?.result;
    let _submitData = {
      ...initialSubmitdataFields,
    };
    if (responseData?.length > 0) {
      for (const dt of responseData) {
        // for _notificationConfigIdsForUpdate
        // update submitdata.data
        // debugger;
        servicesOptions =
          servicesOptions.length > 0
            ? servicesOptions
            : await handleSetSNotificationServiceOptions(
                submitdata.selectedNotificationType
              );
        const _selectedActivatedService = getOptionValueObjectByValue(
          servicesOptions,
          notificationServiceRowsOptionsLabelAndValue[dt.notificationTypeId]
            ?.value,
          dt.serviceTypeId
        );
        const _selectedNotificationSchema = getOptionValueObjectByValue(
          notificationSchema,
          "notificationTypeId",
          dt.notificationTypeId
        );
        const _selectedNotificationSchemaEvent = getOptionValueObjectByValue(
          _selectedNotificationSchema.events,
          "notificationEventId",
          dt.notificationEventId
        );
        if (!getBooleanFromObject(_submitData.selectedActivatedService)) {
          _submitData.selectedActivatedService = _selectedActivatedService;
        }
        selectedNotificationEventOptions =
          selectedNotificationEventOptions.length > 0
            ? selectedNotificationEventOptions
            : await handleSetSelectedNotificationEventOptions(
                dt.notificationTypeId,
                dt.notificationEventId
              );
        const _dataWithValue = _selectedNotificationSchemaEvent?.schema?.map(
          (rowCell) => {
            if (rowCell?.type === inputTypesEnum.SELECT) {
              const idsArr = dt.config.split(",").map(Number);
              const idsOptions = idsArr.map((optId) => {
                const selectedOpt = getOptionValueObjectByValue(
                  selectedNotificationEventOptions,
                  notificationEventsRowsOptionsLabelAndValue[
                    dt.notificationTypeId
                  ][dt.notificationEventId].value,
                  optId
                );
                return selectedOpt;
              });
              return {
                ...rowCell,
                value: idsOptions,
                options: selectedNotificationEventOptions,
              };
            } else {
              return {
                ...rowCell,
                value: dt.text,
              };
            }
          }
        );

        if (Array.isArray(_submitData.data[dt.notificationEventId])) {
          _submitData.data[dt.notificationEventId].push({
            notificationConfigId: dt.notificationConfigId,
            hasError: false,
            data: _dataWithValue,
          });
        } else {
          _submitData.data[dt.notificationEventId] = [
            {
              notificationConfigId: dt.notificationConfigId,
              hasError: false,
              data: _dataWithValue,
            },
          ];
        }
      }
      _submitData.selectedNotificationType =
        submitdata.selectedNotificationType;
      _submitData.selectedNotificationTypeLabel =
        submitdata.selectedNotificationTypeLabel;
      _submitData.updateMode = responseData?.length > 0;
      setSubmitdata(_submitData);
    }
    return { updateMode: responseData?.length > 0 };
  };

  const handleDeleteNotificationConfig = async (notificationConfigId) => {
    const { response } = await fetchMethod(() =>
      DeleteNotificationConfig(notificationConfigId)
    );
  };

  const handleChange = (id, rowIndex, elementIndex, val) => {
    setSubmitdata((prev) => {
      const updatedData = JSON.parse(JSON.stringify(prev.data)); // Deep clone
      updatedData[id][rowIndex]["data"][elementIndex].value = val;
      const hasRowError = updatedData[id][rowIndex]["data"].some(
        (cell) =>
          !cell.value || (Array.isArray(cell.value) && cell.value.length === 0)
      );
      if (submitdata.saveBtnClicked) {
        updatedData[id][rowIndex]["hasError"] = hasRowError;
      }
      return { ...prev, data: updatedData };
    });
  };

  const checkHasEmptyRow = () => {
    const { data } = submitdata;
    let hasError;
    hasError = Object.values(data).some((events) =>
      events.some((row) =>
        row.data.some(
          (cell) =>
            !cell.value ||
            (Array.isArray(cell.value) && cell.value.length === 0)
        )
      )
    );
    const updatedData = Object.entries(data).reduce((acc, [key, eventData]) => {
      eventData = eventData.map((dt) => {
        const hasRowError = dt.data.some(
          (cell) =>
            !cell.value ||
            (Array.isArray(cell.value) && cell.value.length === 0)
        );
        return { ...dt, hasError: hasRowError };
      });
      acc[key] = eventData;

      return acc;
    }, {});
    setSubmitdata((prev) => ({
      ...prev,
      data: updatedData,
    }));
    return { hasError, updatedData };
  };

  const transformSubmitData = (myData = {}) => {
    const data = Object.entries(myData.data).reduce(
      (eventAcc, [eventKey, eventArrs]) => {
        eventArrs.forEach((rowVal) => {
          let transformedData = {
            notificationConfigId: rowVal.notificationConfigId,
            serviceTypeId:
              myData.selectedActivatedService[
                notificationServiceRowsOptionsLabelAndValue[
                  submitdata.selectedNotificationType
                ]?.value
              ],
            text: "",
            notificationTypeId: submitdata.selectedNotificationType,
            notificationEventId: Number(eventKey),
            config: "",
          };

          rowVal.data.forEach((dt) => {
            if (dt.type === inputTypesEnum.SELECT) {
              const optIdsArr = dt.value.map(
                (optVal) =>
                  optVal[
                    notificationEventsRowsOptionsLabelAndValue[
                      submitdata.selectedNotificationType
                    ][eventKey].value
                  ]
              );
              transformedData.config = String(optIdsArr);
            } else {
              transformedData.text = dt.value;
            }
          });

          eventAcc.push(transformedData);
        });

        return eventAcc;
      },
      []
    );
    return data;
  };

  const handleCreateUpdateNotification = async () => {
    const { hasError, updatedData } = checkHasEmptyRow();
    setSubmitdata((prevState) => ({ ...prevState, saveBtnClicked: true }));
    if (!hasError) {
      const params = { list: transformSubmitData(submitdata) };
      setSubmitdata((prevState) => ({ ...prevState, loading: true }));
      await (submitdata.updateMode
        ? UpdateNotificationConfig(params)
        : CreaetNotificationConfig(params)
      )
        .then((res) => {
          if (!res?.data?.isSuccess) {
            errorNotification("Unable to Create");
          } else {
            successNotification(
              submitdata.updateMode
                ? "Update Successfully"
                : "Saved Successfully"
            );
          }
          setSubmitdata((prevState) => ({ ...prevState, loading: false }));
        })
        .catch((e) => {
          errorNotification("Something went wrong");
          setSubmitdata((prevState) => ({ ...prevState, loading: false }));
        });
    } else {
      Object.entries(updatedData).forEach(([key, dt]) => {
        const hasEventError = dt.some((row) =>
          row.data.some(
            (cell) =>
              !cell.value ||
              (Array.isArray(cell.value) && cell.value.length === 0)
          )
        );
        if (hasEventError) {
          const erroredSelectedNotificationEvent =
            notificationEventOptions?.find(
              (dt) => dt[EnumOptions.NOTIFICATION_EVENTS.VALUE] == key
            );
          errorNotification(
            `Error(s) in: ${
              erroredSelectedNotificationEvent?.[
                EnumOptions.NOTIFICATION_EVENTS.LABEL
              ]
            }`
          );
        }
      });
    }
  };

  const handleAddRow = (_selectedNotificationType) => {
    if (
      submitdata.selectedNotificationEvent[
        EnumOptions.NOTIFICATION_EVENTS.VALUE
      ]
    ) {
      setSubmitdata((prev) => ({
        ...prev,
        data: {
          ...prev.data,
          [_selectedNotificationType]: prev.data[_selectedNotificationType]
            ? [
                ...prev.data[_selectedNotificationType],
                {
                  notificationConfigId: "",
                  hasError: false,
                  data: selectedNotificationSchema[_selectedNotificationType],
                },
              ]
            : [
                {
                  notificationConfigId: "",
                  hasError: false,
                  data: selectedNotificationSchema[_selectedNotificationType],
                },
              ],
        },
      }));
    } else {
      errorNotification("Please Select Event");
    }
  };

  // for change event and service
  const handleChangeNotificationServiceAndEvent = (name, val) => {
    setSubmitdata((prev) => ({
      ...prev,
      [name]: val,
    }));
    // for get options if row has select input
    const selectedNotificationEventId =
      val[EnumOptions.NOTIFICATION_EVENTS.VALUE];
    handleSetSelectedNotificationEventOptions(
      submitdata.selectedNotificationType,
      selectedNotificationEventId
    );
  };

  const handleRemoveRow = async (id, rowIndex, configId) => {
    setSubmitdata((prev) => {
      const updatedData = JSON.parse(JSON.stringify(prev.data));
      updatedData[id].splice(rowIndex, 1);
      return { ...prev, data: updatedData };
    });
    if (configId) {
      await handleDeleteNotificationConfig(configId);
    }
  };

  const handleSetSNotificationServiceOptions = async (id) => {
    let _options = [];
    _options = await notificationServiceMethodsByType[id]();
    setNotificationServiceOptions(_options);
    return _options;
  };

  const handleSetSNotificationEventOptions = async (id) => {
    const _options = await notificationEventsOptionsMethodsByType[id]();
    setNotificationEventOptions(_options);
  };

  const handleGetAllOptionOfSelectedEventType = async () => {
    const { updateMode } = await handleGetNotificationConfigByTypeId(
      submitdata.selectedNotificationType
    );
    await handleSetSNotificationEventOptions(
      submitdata.selectedNotificationType
    );
    if (!updateMode) {
      await handleSetSNotificationServiceOptions(
        submitdata.selectedNotificationType
      );
    }
  };

  useEffect(() => {
    const checkSelectedNotificationTypeHasSchema = notificationSchema.some(
      (dt) => dt.notificationTypeId === submitdata.selectedNotificationType
    );
    if (
      submitdata.selectedNotificationType &&
      checkSelectedNotificationTypeHasSchema
    ) {
      handleGetAllOptionOfSelectedEventType();
    }
  }, [submitdata.selectedNotificationType]);

  useEffect(() => {
    const hasEmptyData = Object.values(submitdata.data).every(
      (arr) => arr.length === 0
    );
    if (hasEmptyData) {
      setSubmitdata((prev) => ({ ...prev, updateMode: false }));
    }
  }, [submitdata.data]);

  useEffect(() => {
    getNotificationTypes();
  }, []);

  return (
    <>
      <Box sx={{ padding: "10px" }}>
        <GridContainer>
          {notificationTypes.map((data) =>
            !submitdata.selectedNotificationType ||
            submitdata.selectedNotificationType === data.notificationTypeId ? (
              <GridItem
                md={4}
                sm={4}
                xs={12}
                lg={4}
                key={data.notificationTypeId}
                sx={{ position: "relative" }}
              >
                <Box width="100%" display="flex" alignItems="center">
                  <GreyBox
                    sx={{
                      ...styleSheet.notificationGreyBox,
                      cursor: "pointer",
                      color:
                        submitdata.selectedNotificationType ===
                        data.notificationTypeId
                          ? purple
                          : "inherit",
                    }}
                    bg={
                      submitdata.selectedNotificationType ===
                      data.notificationTypeId
                        ? selectedTabBg
                        : undefined
                    }
                    className="flex_center"
                    width="100%"
                    onClick={() => {
                      const selectedSchema = notificationSchema.find(
                        (dt) =>
                          dt.notificationTypeId === data.notificationTypeId
                      );
                      setSubmitdata({
                        ...initialSubmitdataFields,
                        selectedNotificationType: data.notificationTypeId,
                        selectedNotificationTypeLabel: data.typeName,
                      });
                      setNotificationServiceOptions([]);
                      setNotificationEventOptions([]);
                      if (selectedSchema) {
                        const _selectedNotificationSchema =
                          transformNotificationType(selectedSchema);
                        const _submitData = transformSubmitdata(selectedSchema);
                        setSubmitdata({
                          ...initialSubmitdataFields,
                          selectedNotificationType: data.notificationTypeId,
                          selectedNotificationTypeLabel: data.typeName,
                          data: _submitData,
                        });
                        setSelectedNotificationSchema(
                          _selectedNotificationSchema
                        );
                      }
                    }}
                  >
                    <Box gap={1} className={"flex_center"}>
                      <img
                        style={{ width: 30, height: 30 }}
                        src={
                          submitdata.selectedNotificationType ===
                          data.notificationTypeId
                            ? notificationTypeImgs[data.notificationTypeId]
                                .selected
                            : notificationTypeImgs[data.notificationTypeId]
                                .deseleced
                        }
                      />
                      <Typography
                        sx={{
                          height: "100%",
                        }}
                        variant="h5"
                      >
                        {data.typeName}
                      </Typography>
                    </Box>
                  </GreyBox>
                  {submitdata.selectedNotificationType ===
                    data.notificationTypeId && (
                    <Box position="absolute" right="0">
                      <CrossIconButton
                        color={red}
                        sx={{
                          marginLeft: "5px",
                          cursor: "pointer",
                          "&:hover": {
                            border: "1px none",
                            borderRadius: "50px",
                            bgcolor: "#f8a7a5ff",
                          },
                        }}
                        onClick={() => {
                          setSubmitdata(initialSubmitdataFields);
                          setSelectedNotificationSchema({});
                        }}
                      />
                    </Box>
                  )}
                </Box>
              </GridItem>
            ) : null
          )}
        </GridContainer>
        {submitdata.selectedNotificationType && (
          <GridContainer sx={{ paddingTop: "10px" }}>
            <GridItem xs={12} sm={12} md={9}>
              <GreyBox
                sx={{
                  maxHeight: windowHeight - navbarHeight - 95,
                  overflowY: "auto",
                  overflowX: "hidden",
                }}
              >
                <Box padding="5px" display="flex" justifyContent="center">
                  <Box width="100%">
                    <InputLabel sx={styleSheet.inputLabel}>
                      {submitdata.selectedNotificationTypeLabel} Service
                    </InputLabel>
                    <SelectComponent
                      name="selectedActivatedService"
                      options={notificationServiceOptions}
                      optionLabel={
                        notificationServiceRowsOptionsLabelAndValue[
                          submitdata.selectedNotificationType
                        ]?.label
                      }
                      optionValue={
                        notificationServiceRowsOptionsLabelAndValue[
                          submitdata.selectedNotificationType
                        ]?.value
                      }
                      value={submitdata.selectedActivatedService}
                      onChange={handleChangeNotificationServiceAndEvent}
                    />
                  </Box>
                </Box>
                <Box padding="5px" display="flex" justifyContent="center">
                  <Box width="100%">
                    <InputLabel sx={styleSheet.inputLabel}>
                      {submitdata.selectedNotificationTypeLabel} Event
                    </InputLabel>
                    <SelectComponent
                      name="selectedNotificationEvent"
                      options={notificationEventOptions}
                      optionLabel={EnumOptions.NOTIFICATION_EVENTS.LABEL}
                      optionValue={EnumOptions.NOTIFICATION_EVENTS.VALUE}
                      value={submitdata.selectedNotificationEvent}
                      onChange={handleChangeNotificationServiceAndEvent}
                    />
                  </Box>
                  <Box alignSelf="end" paddingBottom="5px" paddingLeft="5px">
                    <ActionButtonCustom
                      label="Add Row"
                      onClick={() =>
                        handleAddRow(
                          submitdata.selectedNotificationEvent[
                            EnumOptions.NOTIFICATION_EVENTS.VALUE
                          ]
                        )
                      }
                    />
                  </Box>
                </Box>
                {submitdata.data[
                  submitdata.selectedNotificationEvent[
                    EnumOptions.NOTIFICATION_EVENTS.VALUE
                  ]
                ]?.map((item, rowIndex) => (
                  <Box
                    sx={{
                      padding: "10px",
                      paddingBottom: "18px",
                      display: "flex",
                      alignItems: "end",
                      gap: 1.5,
                      bgcolor: item.hasError ? lightgrey : "inherit",
                    }}
                    key={rowIndex}
                  >
                    <Box alignSelf="center" marginTop="20px">
                      <Typography variant="h5">
                        {submitdata.selectedNotificationEvent?.eventName}
                      </Typography>
                    </Box>
                    {item?.data?.map((row, elementIndex) => (
                      <Box flexGrow={1}>
                        <InputLabel sx={styleSheet.inputLabel}>
                          Message {rowIndex + 1}
                        </InputLabel>

                        {row.type === inputTypesEnum.SELECT ? (
                          <SelectComponent
                            multiple={row.multiple}
                            optionLabel={
                              notificationEventsRowsOptionsLabelAndValue?.[
                                submitdata.selectedNotificationType
                              ]?.[
                                submitdata.selectedNotificationEvent?.[
                                  EnumOptions.NOTIFICATION_EVENTS.VALUE
                                ]
                              ]?.label
                            }
                            optionValue={
                              notificationEventsRowsOptionsLabelAndValue?.[
                                submitdata.selectedNotificationType
                              ]?.[
                                submitdata.selectedNotificationEvent?.[
                                  EnumOptions.NOTIFICATION_EVENTS.VALUE
                                ]
                              ]?.value
                            }
                            name="carrierStatus"
                            options={row.options}
                            value={row.value}
                            onChange={(e, val) =>
                              handleChange(
                                submitdata.selectedNotificationEvent[
                                  EnumOptions.NOTIFICATION_EVENTS.VALUE
                                ],
                                rowIndex,
                                elementIndex,
                                val
                              )
                            }
                          />
                        ) : (
                          <TextField
                            sx={{
                              "& .MuiInputBase-root": {
                                bgcolor: "white",
                              },
                              "& .MuiInputBase-root input": {
                                fontSize: "12px",
                              },
                            }}
                            type="text"
                            size="small"
                            fullWidth
                            variant="outlined"
                            value={row.value}
                            onChange={(e) =>
                              handleChange(
                                submitdata.selectedNotificationEvent[
                                  EnumOptions.NOTIFICATION_EVENTS.VALUE
                                ],
                                rowIndex,
                                elementIndex,
                                e.target.value
                              )
                            }
                          />
                        )}
                      </Box>
                    ))}
                    <Box>
                      <ActionButtonCustom
                        marginBottom={"3px"}
                        label="Remove"
                        background={Colors.danger}
                        onClick={() =>
                          handleRemoveRow(
                            submitdata.selectedNotificationEvent[
                              EnumOptions.NOTIFICATION_EVENTS.VALUE
                            ],
                            rowIndex,
                            item.notificationConfigId
                          )
                        }
                      />
                    </Box>
                  </Box>
                ))}
                <GridItem xs={12}>
                  <Box display={"flex"} justifyContent={"end"} marginTop="20px">
                    <ActionButtonCustom
                      label={submitdata.updateMode ? "Update" : "Save"}
                      disabled={
                        !submitdata.selectedNotificationEvent[
                          EnumOptions.NOTIFICATION_EVENTS.VALUE
                        ] ||
                        (!submitdata.selectedActivatedService[
                          EnumOptions.NOTIFICATION_SERVICE.VALUE
                        ] &&
                          !submitdata.selectedActivatedService[
                            EnumOptions.WHATSAPP_NOTIFICATION_SERVICE.VALUE
                          ]) ||
                        Object.values(submitdata.data).every(
                          (arr) => arr.length === 0
                        )
                      }
                      background={Colors.primary}
                      loading={submitdata.loading}
                      onClick={handleCreateUpdateNotification}
                    />
                  </Box>
                </GridItem>
              </GreyBox>
            </GridItem>
            <GridItem xs={12} sm={12} md={3}>
              <GreyBox>
                <Box
                  display={"flex"}
                  justifyContent={"center"}
                  marginBottom={"20px"}
                >
                  <Typography variant="h3">
                    {
                      LanguageReducer?.languageType
                        ?.SETTING_NOTIFICATION_SPECIFIERS
                    }{" "}
                  </Typography>
                </Box>
                <Box
                  display={"flex"}
                  flexDirection={"column"}
                  gap={1.5}
                  textAlign={"center"}
                  marginBottom={"10px"}
                >
                  <Typography variant="h5">{`{{orderNo}}`}</Typography>
                  <Typography variant="h5">{`{{carrierTracking}}`}</Typography>
                  <Typography variant="h5">{`{{customerName}}`}</Typography>
                  <Typography variant="h5"> {`{{amount}}`} </Typography>
                  <Typography variant="h5"> {`{{status}}`} </Typography>
                </Box>
              </GreyBox>
            </GridItem>
          </GridContainer>
        )}
      </Box>
    </>
  );
};

export default NotificationPage;
