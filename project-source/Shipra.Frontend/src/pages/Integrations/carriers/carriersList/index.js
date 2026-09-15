import BorderColorIcon from "@mui/icons-material/BorderColor";
import CheckCircleOutlineOutlinedIcon from "@mui/icons-material/CheckCircleOutlineOutlined";
import VisibilityIcon from "@mui/icons-material/Visibility";
import { LoadingButton } from "@mui/lab";
import {
  Box,
  Button,
  Card,
  CircularProgress,
  FormControl,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  List,
  ListItem,
  ListItemText,
  OutlinedInput,
  Popover,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { EnumCarrierFilter } from "..";
import ModalButtonComponent from "../../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ConfigSettingModal from "../../../../.reUseableComponents/Modal/ConfigSettingModal";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import TextFieldComponent from "../../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../../.reUseableComponents/TextField/TextFieldLableComponent";
import {
  DeleteActiveCarrierById,
  GetCarrierWebHookUrlByCarrierId,
  GetCarrierWithServiceAndLocationByCarrierId,
  GetSettingConfigByActiveCarrierId,
  UpdateCarrierAlias,
  getInputRequiredConfig,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import IntegrationConnectModal from "../../../../components/modals/carrierModals/IntegrationConnectModal";
import {
  ActionButtonCustom,
  CicrlesLoading,
  ClipboardIcon,
  DataGridAvatar,
  DataGridHeaderBox,
  fetchMethod,
  placeholders,
  purple,
  useClientSubscriptionReducer,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import {
  EnumChangeFilterModelApiUrls,
  viewTypesEnum,
} from "../../../../utilities/enum";
import { PaginationComponent } from "../../../../utilities/helpers/paginationSchema";
import PickupLocationModal from "../../../../components/modals/integrationModals/PickupLocationModal";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import UPSCarrierPickUpLocationModal from "../../../../components/modals/integrationModals/UPSCarrierPickUpLocationModal";
import UPSCarrierUploadFileModal from "../../../../components/modals/integrationModals/UPSCarrierUploadFileModal";

function CarriersList(props) {
  const {
    setIsRefreshRequired,
    getAllCarriersData,
    isTabChange,
    // setIsTabChange,
    setActiveUrl,
    activeUrl,
    loading,
    setIsLoad,
    isFilterOpen,
    carriersCount,
    viewMode,
    getAllActivedCarrier,
    activeCarriersCount,
  } = props;
  const [anchorEl, setAnchorEl] = useState(null);
  const open = Boolean(anchorEl);
  const [isLoading, setIsLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [loadingStates, setLoadingStates] = useState({});
  const [selectedUpdateAlias, setSelectedUpdateAlias] = useState({});
  const [loadingStatesUpdateAlias, setLoadingStatesUpdateAlias] = useState({});
  const [webhookLoadingStates, setWebhookLoadingStates] = useState({});

  const [carrierData, setCarrierData] = useState();
  const [carrierIdForActivate, setCarrierIdForActivate] = useState();
  const [image, setImage] = useState();
  const [showCopyButton, setShowCopyButton] = useState({});
  const [textToCopy, setTextToCopy] = useState();
  const [openDeleteStore, setOpenDeleteStore] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [OpenViewModal, setOpenViewModal] = useState(false);
  const [viewCarrierData, setViewCarrierData] = useState({});
  const [openEditNickName, setOpenNickNameEdit] = useState(false);
  const [nickNameData, setNickNameData] = useState();
  const [newNickName, setNewNickName] = useState();
  const [configSetting, setConfigSetting] = useState({
    open: false,
    selectedActiveCarrierId: "",
    data: [],
    loading: {},
  });
  const [openPickupLocationModal, setOpenPickupLocationModal] = useState(false);
  const [pickupLocationIds, setPickupLocationIds] = useState();
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [openUpsCarrierMenu, setOpenUpsCarrierMenu] = useState({
    openUPSPickUpLocationModal: false,
    openUPSUploadFileModal: false,
    carrierId: null,
    activeCarrierId: null,
  });
  useEffect(() => {
    setShowCopyButton({});
    setTextToCopy();
    // setIsTabChange(false);
  }, [isTabChange]);
  useEffect(() => {
    setTextToCopy(null);
  }, [activeUrl]);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleActivate = async (data) => {
    setLoadingStates((prevStates) => ({
      ...prevStates,
      [data.CarrierId]: true,
    }));
    try {
      const response = await getInputRequiredConfig(data?.CarrierId);
      setCarrierData(response?.data?.result);
      setImage(data?.CarrierImage);
      setCarrierIdForActivate(data?.CarrierId);
      setModalOpen(true);
    } catch (error) {
      console.error("Error in getting input fields", error.response);
      errorNotification(
        LanguageReducer?.languageType?.UNABLE_TO_GET_INPUT_FIELDS_TOAST
      );
    } finally {
      setLoadingStates((prevStates) => ({
        ...prevStates,
        [data.CarrierId]: false,
      }));
    }
  };

  const handleWebhook = async (data, index) => {
    setTextToCopy();
    setWebhookLoadingStates((prevStates) => ({
      ...prevStates,
      [data.ActiveCarrierId]: true,
    }));
    try {
      const response = await GetCarrierWebHookUrlByCarrierId(
        data.CarrierId,
        data.ActiveCarrierId
      );

      if (response.data?.isSuccess) {
        //copy logic
        setShowCopyButton((prev) => ({ [index]: true }));
        setTextToCopy(response.data.result?.webhookUrl);
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      errorNotification(
        LanguageReducer?.languageType?.INTEGRATION_ERROR_MESSAGE
      );
    } finally {
      setWebhookLoadingStates((prevStates) => ({
        ...prevStates,
        [data.ActiveCarrierId]: false,
      }));
    }
  };
  const handleModalClose = () => {
    setModalOpen(false);
  };
  // handleCopyToClipBoard
  const handleCopyToClipBoard = (index) => {
    successNotification(
      LanguageReducer?.languageType?.INTEGRATION_TEXT_COPIED_TO_CLIPBOARD
    );
    setShowCopyButton({});
    UtilityClass.copyToClipboard(textToCopy);
    setTextToCopy();
  };
  const handleDeleteConfirmation = (data) => {
    setDeleteItemObject(data);
    setOpenDeleteStore(true);
  };

  const handleGetConfigSettingData = async (id) => {
    setConfigSetting((prev) => ({ ...prev, loading: { [id]: true } }));
    const { response } = await fetchMethod(() =>
      GetSettingConfigByActiveCarrierId(id)
    );
    setConfigSetting((prev) => ({
      ...prev,
      loading: { [id]: false },
      open: Boolean(response.result),
      data: response.result || [],
      selectedActiveCarrierId: id,
    }));
    if (!response.result) {
      errorNotification(
        LanguageReducer?.languageType?.INTEGRATION_INVALID_CONFIG_SETTING
      );
    }
  };

  const handleDelete = async () => {
    try {
      let param = {
        activeCarrierId: deleteItemObject.ActiveCarrierId,
      };
      const response = await DeleteActiveCarrierById(param);
      if (response.data?.isSuccess) {
        successNotification("Carrier deactivated successfully");
      } else {
        errorNotification("Carrier deactivated unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      errorNotification(
        LanguageReducer?.languageType?.UNABLE_TO_GET_INPUT_FIELDS_TOAST
      );
    } finally {
      setIsRefreshRequired(true);
      setIsDeletedConfirm(false);
      setOpenDeleteStore(false);
      setDeleteItemObject({});
    }
  };
  useEffect(() => {
    if (isDeletedConfirm) {
      handleDelete();
    }
  }, [isDeletedConfirm]);
  const [updateCarrierAlias, setUpdateCarrierAlias] = useState([]);
  useEffect(() => {
    setUpdateCarrierAlias(props?.carrierData);
  }, [props?.carrierData]);

  const handleUpdateCarrierAlias = (id, event) => {
    const newData = updateCarrierAlias?.map((item) =>
      item.ActiveCarrierId === id
        ? { ...item, CarrierAlias: event.target.value }
        : item
    );
    setUpdateCarrierAlias(newData);
  };
  const handleUpdateNickName = async (data, index) => {
    let carrierAlias = newNickName;
    if (carrierAlias && carrierAlias != "") {
      try {
        setLoadingStatesUpdateAlias((prevStates) => ({
          ...prevStates,
          [data.ActiveCarrierId]: true,
        }));
        let param = {
          ActiveCarrierId: data.ActiveCarrierId,
          CarrierAlias: newNickName,
        };
        const response = await UpdateCarrierAlias(param);
        if (response.data?.isSuccess) {
          //copy logic
          setIsRefreshRequired(true);
          successNotification("Name updated successfully.");
          CloseNickNameEdit();
        } else {
          UtilityClass.showErrorNotificationWithDictionary(
            response?.data?.errors
          );
        }
      } catch (error) {
        console.error("Something went wrong", error.response);
        errorNotification("Something went wrong");
        CloseNickNameEdit();
      } finally {
        setLoadingStatesUpdateAlias((prevStates) => ({
          ...prevStates,
          [data.ActiveCarrierId]: false,
        }));
        setSelectedUpdateAlias({});
      }
    } else {
      errorNotification("Please enter nick name");
    }
  };

  const CloseNickNameEdit = () => {
    setOpenNickNameEdit(false);
  };
  const handleFocus = (event) => event.target.select();

  const handleGetCarrierWithServiceAndLocationByCarrierId = async (
    carrierId,
    activeCarrierId
  ) => {
    try {
      setIsLoading((prevStates) => ({
        ...prevStates,
        [window.location.pathname === EnumCarrierFilter.Active
          ? activeCarrierId
          : carrierId]: true,
      }));

      const { response } = await fetchMethod(() =>
        GetCarrierWithServiceAndLocationByCarrierId(carrierId)
      );
      setViewCarrierData(response.result);
      setOpenViewModal(true);
    } catch (error) {
      console.error("Error fetching carrier data:", error);
    } finally {
      setIsLoading((prevStates) => ({
        ...prevStates,
        [window.location.pathname === EnumCarrierFilter.Active
          ? activeCarrierId
          : carrierId]: false,
      }));
    }
  };
  const columns = [
    {
      field: "Name",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.INTEGRATION_NAME}
        />
      ),
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box>
            <DataGridAvatar
              src={row.CarrierImage}
              name={row.Name}
              onClick={() =>
                handleGetCarrierWithServiceAndLocationByCarrierId(
                  row.CarrierId,
                  row.ActiveCarrierId
                )
              }
            />
          </Box>
        );
      },
    },
    {
      field: "NickName",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.INTEGRATION_NICK_NAME}
        />
      ),
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box>
            <Typography>{row?.CarrierAlias}</Typography>
          </Box>
        );
      },
    },
    {
      field: "WebHookUrl",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.INTEGRATION_WEBHOOK_URL}
        />
      ),
      minWidth: 200,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            {showCopyButton[row.index] && textToCopy && row.ActiveCarrierId && (
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                }}
              >
                <Typography
                  variant="body1"
                  sx={{
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    whiteSpace: "nowrap",
                  }}
                >
                  {textToCopy}
                </Typography>
                <IconButton
                  aria-label="copy to clipboard"
                  edge="end"
                  sx={{ ml: 1 }}
                >
                  <ClipboardIcon text={textToCopy} />
                </IconButton>
              </Box>
            )}
          </>
        );
      },
    },
    {
      field: "Action",
      headerName: <DataGridHeaderBox title={"Action"} />,
      minWidth: 450,
      renderCell: ({ row }) => {
        return (
          <>
            {window.location.pathname === EnumCarrierFilter.Active && (
              <Box sx={{ display: "flex", gap: 1 }}>
                <Button
                  onClick={() => handleDeleteConfirmation(row)}
                  sx={styleSheet.integrationDeactiveButton}
                  variant="contained"
                >
                  {LanguageReducer?.languageType?.INTEGRATION_DEACTIVATE}
                </Button>
                <LoadingButton
                  loading={webhookLoadingStates[row?.ActiveCarrierId]}
                  onClick={() => handleWebhook(row, row.index)}
                  sx={styleSheet.integrationDeactiveButton}
                  variant="contained"
                >
                  {LanguageReducer?.languageType?.INTEGRATION_GET_WEBHOOK_URL}
                </LoadingButton>
                <LoadingButton
                  onClick={() => {
                    setOpenPickupLocationModal(true);
                    setPickupLocationIds(row);
                  }}
                  sx={styleSheet.integrationConfigBtn}
                  variant="contained"
                >
                  Pickup Location
                </LoadingButton>
                {row?.SettingConfig && (
                  <LoadingButton
                    loading={configSetting.loading[row.ActiveCarrierId]}
                    onClick={() =>
                      handleGetConfigSettingData(row.ActiveCarrierId)
                    }
                    sx={{
                      ...styleSheet.integrationConfigBtn,
                      display:
                        window.location.pathname === EnumCarrierFilter.All ||
                        window.location.pathname === EnumCarrierFilter.InActive
                          ? "none"
                          : "inline-flex",
                    }}
                    variant="contained"
                  >
                    {LanguageReducer?.languageType?.INTEGRATION_CONFIG_SETTING}
                  </LoadingButton>
                )}
              </Box>
            )}
            {window.location.pathname === EnumCarrierFilter.All && (
              <Box
                sx={{
                  display: "flex",
                  gap: 0.5,
                  width: "100%",
                  alignItems: "center",
                }}
              >
                <ActionButtonCustom
                  onClick={() => handleActivate(row)}
                  sx={{
                    ...styleSheet.integrationactivatedButton,
                    width: "14%",
                    height: "28px",
                    borderRadius: "4px",
                  }}
                  variant="contained"
                  loading={loadingStates[row.CarrierId]}
                  label={
                    row.Active
                      ? LanguageReducer?.languageType?.INTEGRATION_ACTIVATE
                      : LanguageReducer?.languageType?.INTEGRATION_ACTIVATE
                  }
                ></ActionButtonCustom>
                {row?.GuideUrl && (
                  <ActionButtonCustom
                    onClick={() => window.open(row.GuideUrl, "_blank")}
                    sx={{
                      ...styleSheet.integrationactivatedButton,
                      width: "22%",
                      height: "28px",
                      borderRadius: "4px",
                    }}
                    variant="contained"
                    label={"Guide Url"}
                  ></ActionButtonCustom>
                )}
              </Box>
            )}
          </>
        );
      },
    },
  ];

  const handleOpenActionMenuForUps = (event, data) => {
    event.preventDefault();
    setAnchorEl(event.currentTarget);

    setOpenUpsCarrierMenu((prev) => ({
      ...prev,
      carrierId: data?.CarrierId || null,
      activeCarrierId: data?.ActiveCarrierId || null,
    }));
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const calculatedHeight = isFilterOpen
    ? windowHeight - 300
    : windowHeight - 188;

  const calculatedHeightTable = isFilterOpen
    ? windowHeight - 346
    : windowHeight - 227;

  return (
    <Box>
      {" "}
      <Grid
        container
        sx={{
          border: "1px solid rgba(224, 224, 224, 1)",
          borderRadius: "10px",
          borderTopLeftRadius: "0px !important",
          borderTopRightRadius: "0px !important",
          height: calculatedHeight,
          overflow: "auto",
          background: "#f8f8f8",
        }}
      >
        {loading ? (
          <CicrlesLoading />
        ) : viewMode === viewTypesEnum.GRID ? (
          <>
            {updateCarrierAlias?.map((data, index) => (
              <Grid item md={3} sm={6} xs={12} key={index} p={1}>
                <Card sx={styleSheet.integrationCard} variant="outlined">
                  <Box display="flex" justifyContent="space-between">
                    <Box
                      sx={{ display: "flex", alignItems: "center", gap: 0.5 }}
                    >
                      <Box>
                        {data?.SettingConfig && (
                          <LoadingButton
                            loading={
                              configSetting.loading[data.ActiveCarrierId]
                            }
                            onClick={() =>
                              handleGetConfigSettingData(data.ActiveCarrierId)
                            }
                            sx={{
                              ...styleSheet.integrationConfigBtn,
                              display:
                                window.location.pathname ===
                                  EnumCarrierFilter.All ||
                                window.location.pathname ===
                                  EnumCarrierFilter.InActive
                                  ? "none"
                                  : "inline-flex",
                            }}
                            variant="contained"
                          >
                            {
                              LanguageReducer?.languageType
                                ?.INTEGRATION_CONFIG_SETTING
                            }
                          </LoadingButton>
                        )}
                      </Box>
                      <Box>
                        {data?.CarrierId === 99 && (
                          <LoadingButton
                            sx={{
                              ...styleSheet.integrationConfigBtn,
                              minWidth: "80px !important",
                              backgroundColor: "green",
                              "&:hover": {
                                backgroundColor: "#228B22",
                              },
                              display:
                                window.location.pathname ===
                                  EnumCarrierFilter.All ||
                                window.location.pathname ===
                                  EnumCarrierFilter.InActive
                                  ? "none"
                                  : "inline-flex",
                              "& .MuiButton-endIcon": {
                                marginLeft: "0px",
                                marginRight: "-4px",
                              },
                            }}
                            variant="contained"
                            onClick={(e) => handleOpenActionMenuForUps(e, data)}
                            endIcon={
                              <ExpandMoreIcon
                                sx={{
                                  transform: open
                                    ? "rotate(180deg)"
                                    : "rotate(0deg)",
                                  transition: "transform 0.2s ease",
                                }}
                              />
                            }
                          >
                            Action
                          </LoadingButton>
                        )}
                      </Box>
                    </Box>

                    <Box sx={{ display: "flex", alignItems: "center" }}>
                      <Button
                        disabled={
                          isLoading[
                            window.location.pathname ===
                            EnumCarrierFilter.Active
                              ? data.ActiveCarrierId
                              : data.CarrierId
                          ]
                        }
                        onClick={() =>
                          handleGetCarrierWithServiceAndLocationByCarrierId(
                            data.CarrierId,
                            data.ActiveCarrierId
                          )
                        }
                        className="eyeBtn"
                      >
                        {isLoading[
                          window.location.pathname === EnumCarrierFilter.Active
                            ? data.ActiveCarrierId
                            : data.CarrierId
                        ] ? (
                          <CircularProgress size={24} color="inherit" />
                        ) : (
                          <VisibilityIcon />
                        )}
                      </Button>
                    </Box>
                  </Box>

                  <Box sx={styleSheet.integrationLogoArea}>
                    <img
                      src={data?.CarrierImage}
                      style={{ height: "70px", objectFit: "contain" }}
                      alt={"expressIcon"}
                    />
                  </Box>
                  <Box
                    alignItems="center"
                    display="flex"
                    justifyContent="center"
                  >
                    <Typography variant="h4">{data?.Name}</Typography>
                  </Box>
                  <center>
                    <Box sx={styleSheet.integrationContent}>
                      {data.ActiveCarrierId && (
                        <Box textAlign={"center"} marginTop="5px">
                          <Typography variant="h4" fontSize="13px">
                            {data?.CarrierAlias}
                            {window.location.pathname ===
                              data.ActiveCarrierId && (
                              <LoadingButton
                                onClick={() => {
                                  setNickNameData({ data: data, index: index });
                                  setOpenNickNameEdit(true);
                                }}
                                sx={{
                                  padding: "0px",
                                  minWidth: "40px !important",
                                }}
                                variant="text"
                              >
                                <BorderColorIcon
                                  sx={{
                                    fontSize: 17,
                                  }}
                                />
                              </LoadingButton>
                            )}
                          </Typography>
                        </Box>
                      )}
                    </Box>
                  </center>
                  <br />
                  {showCopyButton[index] &&
                    textToCopy &&
                    data.ActiveCarrierId && (
                      <Grid item md={12} sm={12} xs={12} mx={1} my={2}>
                        <InputLabel sx={styleSheet.inputLabel}>Url</InputLabel>
                        <FormControl fullWidth variant="outlined" disabled>
                          <OutlinedInput
                            placeholder={placeholders.url}
                            onFocus={handleFocus}
                            type={"text"}
                            value={textToCopy}
                            sx={{
                              "& .MuiInputBase-input": {
                                overflow: "hidden",
                                textOverflow: "ellipsis",
                              },
                            }}
                            endAdornment={
                              <InputAdornment position="end">
                                <IconButton
                                  aria-label="toggle password visibility"
                                  // onClick={handleClickShowPassword}
                                  // onMouseDown={handleMouseDownPassword}
                                  edge="end"
                                >
                                  <ClipboardIcon text={textToCopy} />
                                </IconButton>
                              </InputAdornment>
                            }
                            size="small"
                            fullWidth
                          />
                        </FormControl>
                      </Grid>
                    )}
                  <Box sx={styleSheet.integrationCardAction}>
                    {" "}
                    <>
                      {!data?.IsActiveCarrier && (
                        <Box sx={{ display: "flex", gap: 0.5, width: "100%" }}>
                          <ActionButtonCustom
                            onClick={() => handleActivate(data)}
                            sx={styleSheet.integrationactivatedButton}
                            variant="contained"
                            // disabled={data.active}
                            loading={loadingStates[data.CarrierId]}
                            label={
                              data.Active
                                ? LanguageReducer?.languageType
                                    ?.INTEGRATION_ACTIVATE
                                : LanguageReducer?.languageType
                                    ?.INTEGRATION_ACTIVATE
                            }
                          ></ActionButtonCustom>
                          {data?.GuideUrl && (
                            <ActionButtonCustom
                              onClick={() =>
                                window.open(data.GuideUrl, "_blank")
                              }
                              sx={styleSheet.integrationactivatedButton}
                              variant="contained"
                              label={"Guide Url"}
                            ></ActionButtonCustom>
                          )}
                        </Box>
                      )}{" "}
                      {data?.IsActiveCarrier && data?.Active && (
                        <>
                          <Grid display={"flex"} gap={1}>
                            <Button
                              onClick={() => handleDeleteConfirmation(data)}
                              sx={styleSheet.integrationDeactiveButton}
                              variant="contained"
                              // disabled={data.active}
                            >
                              {
                                LanguageReducer?.languageType
                                  ?.INTEGRATION_DEACTIVATE
                              }
                            </Button>
                            <LoadingButton
                              loading={
                                webhookLoadingStates[data?.ActiveCarrierId]
                              }
                              onClick={() => handleWebhook(data, index)}
                              sx={styleSheet.integrationDeactiveButton}
                              variant="contained"
                              // disabled={data.active}
                            >
                              {
                                LanguageReducer?.languageType
                                  ?.INTEGRATION_GET_WEBHOOK_URL
                              }
                            </LoadingButton>
                            <LoadingButton
                              // loading={}
                              onClick={() => {
                                setOpenPickupLocationModal(true);
                                setPickupLocationIds(data);
                              }}
                              sx={styleSheet.integrationConfigBtn}
                              variant="contained"
                            >
                              Pickup Location
                            </LoadingButton>
                          </Grid>
                        </>
                      )}
                    </>
                  </Box>
                </Card>
              </Grid>
            ))}
            {(activeCarriersCount > 50 || carriersCount > 50) && (
              <PaginationComponent
                sx={{
                  height: "50px",
                  alignSelf: "end",
                  margin: "8px",
                }}
                name={"Carriers"}
                dataCount={
                  window.location.pathname === EnumCarrierFilter.All
                    ? carriersCount
                    : activeCarriersCount
                }
                paginationChangeMethod={
                  window.location.pathname === EnumCarrierFilter.All
                    ? getAllCarriersData
                    : getAllActivedCarrier
                }
                paginationMethodUrl={
                  EnumChangeFilterModelApiUrls.GET_ALL_CARRIER.url
                }
                defaultRowsPerPage={
                  EnumChangeFilterModelApiUrls.GET_ALL_CARRIER.length
                }
                setLoading={setIsLoad}
                color="primary"
              />
            )}
          </>
        ) : (
          <Box
            sx={{
              ...styleSheet.allOrderTable,
              height: calculatedHeightTable,
            }}
          >
            <DataGridProComponent
              rows={updateCarrierAlias}
              rowPadding={4}
              columns={
                window.location.pathname === EnumCarrierFilter.All
                  ? columns.filter(
                      (dt) =>
                        dt.field !== "NickName" && dt.field !== "WebHookUrl"
                    )
                  : window.location.pathname === EnumCarrierFilter.InActive
                  ? columns.filter(
                      (dt) =>
                        dt.field !== "NickName" &&
                        dt.field !== "WebHookUrl" &&
                        dt.field !== "Action"
                    )
                  : columns
              }
              loading={loading}
              getRowId={(row) => row.index}
              rowsCount={updateCarrierAlias?.length}
              paginationChangeMethod={() => {}}
              paginationMethodUrl={
                EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.url
              }
              defaultRowsPerPage={
                EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length
              }
              height={calculatedHeightTable}
            />
          </Box>
        )}
      </Grid>
      <Popover
        open={open}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: "bottom",
        }}
        transformOrigin={{
          vertical: "top",
        }}
      >
        <List
          sx={{
            width: 150,
            padding: "2px !important",
          }}
        >
          <ListItem
            sx={{
              padding: "4px 6px",
              justifyContent: "center",
              cursor: "pointer",
              "&:hover": {
                backgroundColor: "#f0f0f0",
              },
            }}
            onClick={() =>
              setOpenUpsCarrierMenu((prev) => ({
                ...prev,
                openUPSPickUpLocationModal: true,
              }))
            }
          >
            <ListItemText
              primary="Choose Pick Up Location"
              sx={{ textAlign: "center" }}
            />
          </ListItem>

          <ListItem
            sx={{
              padding: "4px 6px",
              justifyContent: "center",
              cursor: "pointer",
              "&:hover": {
                backgroundColor: "#f0f0f0",
              },
            }}
            onClick={() =>
              setOpenUpsCarrierMenu((prev) => ({
                ...prev,
                openUPSUploadFileModal: true,
              }))
            }
          >
            <ListItemText primary="Upload File" sx={{ textAlign: "center" }} />
          </ListItem>
        </List>
      </Popover>
      {modalOpen && (
        <IntegrationConnectModal
          open={modalOpen}
          setOpen={setModalOpen}
          carrierData={carrierData}
          carrierId={carrierIdForActivate}
          carrierImage={image}
          getAllCarriersData={getAllCarriersData}
          setIsRefreshRequired={setIsRefreshRequired}
          setActiveUrl={setActiveUrl}
        />
      )}
      <DeleteConfirmationModal
        open={openDeleteStore}
        setOpen={setOpenDeleteStore}
        setIsDeletedConfirm={setIsDeletedConfirm}
        loading={isDeletedConfirm}
        {...props}
      />
      {configSetting.open && (
        <ConfigSettingModal
          open={configSetting.open}
          onClose={() => setConfigSetting((prev) => ({ ...prev, open: false }))}
          data={configSetting}
        />
      )}
      {openPickupLocationModal && (
        <PickupLocationModal
          open={openPickupLocationModal}
          onClose={() => setOpenPickupLocationModal(false)}
          pickupLocationIds={pickupLocationIds}
          showTable={true}
        />
      )}
      {openUpsCarrierMenu.openUPSPickUpLocationModal && (
        <UPSCarrierPickUpLocationModal
          open={openUpsCarrierMenu.openUPSPickUpLocationModal}
          onClose={() =>
            setOpenUpsCarrierMenu((prev) => ({
              ...prev,
              openUPSPickUpLocationModal: false,
            }))
          }
          carrierId={openUpsCarrierMenu?.carrierId}
          activeCarrierId={openUpsCarrierMenu?.activeCarrierId}
        />
      )}
      {openUpsCarrierMenu.openUPSUploadFileModal && (
        <UPSCarrierUploadFileModal
          open={openUpsCarrierMenu.openUPSUploadFileModal}
          onClose={() =>
            setOpenUpsCarrierMenu((prev) => ({
              ...prev,
              openUPSUploadFileModal: false,
            }))
          }
          carrierId={openUpsCarrierMenu?.carrierId}
          activeCarrierId={openUpsCarrierMenu?.activeCarrierId}
        />
      )}
      <ModalComponent
        open={OpenViewModal}
        onClose={() => setOpenViewModal(false)}
        title={""}
        maxWidth="md"
      >
        <Box
          sx={{
            display: "flex",
            flexDirection: { md: "row", sm: "row", xs: "column" },
            margin: "20px",
          }}
        >
          <Box
            sx={{
              width: "270px",
              height: "250px",
              alignItems: "center",
              justifyContent: "center",
              border: "1px solid #d3d3d391",
              borderRadius: "15px",
              mb: 1.5,
            }}
            className={"flex_center"}
          >
            <img
              width="100%"
              height="auto"
              style={{ borderRadius: "15px" }}
              src={viewCarrierData?.CarrierImage}
              alt="Carrier"
            />
          </Box>
          <Box sx={styleSheet.viewMianBox}>
            <Typography color="blue" fontSize="26px" variant="h2">
              {viewCarrierData?.Name}
            </Typography>
            <Box
              sx={{
                width: "100% !important",
                display: "flex",
                flexDirection: { md: "row", sm: "column", xs: "column" },
                justifyContent: { md: "flex-start", sm: "flex-start" },
                alignItems: { md: "center", sm: "flex-start" },
              }}
            >
              <Typography variant="h4">Website:</Typography>
              <Typography sx={{ paddingLeft: "3px" }}>
                {viewCarrierData?.CarrierWebsite}
              </Typography>
            </Box>
            <Box
              sx={{
                width: "100% !important",
                display: "flex",
                flexDirection: { md: "row", sm: "column", xs: "column" },
                justifyContent: { md: "flex-start", sm: "flex-start" },
                alignItems: { md: "center", sm: "flex-start" },
              }}
            >
              <Typography variant="h4">Location:</Typography>
              <Typography sx={{ paddingLeft: "3px" }}>
                {viewCarrierData?.LocationNames}
              </Typography>
            </Box>

            {viewCarrierData?.CarrierFeature?.length > 0 ? (
              <>
                <Typography variant="h4">Features:</Typography>
                <Box paddingLeft="20px">
                  {viewCarrierData.CarrierFeature.map((feature, index) => (
                    <Typography
                      key={index}
                      sx={{ display: "flex", alignItems: "center" }}
                    >
                      <CheckCircleOutlineOutlinedIcon
                        color="primary"
                        fontSize="20px"
                        sx={{ marginRight: "5px" }}
                      />{" "}
                      {feature.Feature}
                    </Typography>
                  ))}
                </Box>
              </>
            ) : (
              <Typography></Typography>
            )}
            {viewCarrierData?.CarrierServices?.length > 0 ? (
              <>
                <Box sx={styleSheet.viewCarrierServices}>
                  {viewCarrierData.CarrierServices.map((feature, index) => (
                    <Typography variant="h5" key={index}>
                      <Box>
                        <img
                          src={feature.ServiceLogo}
                          width="50px"
                          height="50px"
                        />
                      </Box>
                      {feature.ServiceName}
                    </Typography>
                  ))}
                </Box>
              </>
            ) : (
              <Typography></Typography>
            )}
          </Box>
        </Box>
      </ModalComponent>
      <ModalComponent
        open={openEditNickName}
        onClose={CloseNickNameEdit}
        title={LanguageReducer?.languageType?.INTEGRATION_EDIT_NICK_NAME}
        maxWidth="sm"
        actionBtn={
          <ModalButtonComponent
            title={LanguageReducer?.languageType?.INTEGRATION_EDIT_DATA}
            loading={
              loadingStatesUpdateAlias[nickNameData?.data?.ActiveCarrierId]
            }
            bg={purple}
            onClick={() => handleUpdateNickName(nickNameData?.data)}
          />
        }
      >
        <Box>
          <TextFieldLableComponent
            title={LanguageReducer?.languageType?.INTEGRATION_NICK_NAME}
          />
          <TextFieldComponent
            defaultValue={nickNameData?.data?.CarrierAlias}
            name={"name"}
            borderRadius={"0px"}
            onChange={(e) => setNewNickName(e.target.value)}
          />
        </Box>
      </ModalComponent>
    </Box>
  );
}
export default CarriersList;
