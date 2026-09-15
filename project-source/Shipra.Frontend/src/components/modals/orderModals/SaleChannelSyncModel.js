import { Grid, InputLabel } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetChannelListByStoreIdForSelection,
  SaleChannelOrderPreProcessor,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function SaleChannelSyncModel(props) {
  let {
    open,
    setOpen,
    allStores,
    setAllOrders,
    resetRowRef,
    allOrderTypeLookup,
    setErrorsList,
    productStations,
    getAllStores,
    getAllStationLookup,
    getAllOrderTypeLookup,
  } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);
  const handleClose = () => {
    setOpen(false);
  };

  const [selectStation, setselectStation] = useState({
    productStationId: 0,
    sname: "Select Please",
  });
  const [store, setStore] = useState({
    storeId: 0,
    storeName: "Select Please",
  });
  const [orderType, setOrderType] = useState({
    orderTypeId: 0,
    orderTypeName: "Select Please",
  });
  const [storeChannel, setStoreChannel] = useState({
    id: 0,
    text: "Select Please",
  });
  const handleSubmitClick = () => {
    if (
      !selectStation?.productStationId ||
      selectStation?.productStationId == 0
    ) {
      errorNotification("Please select Station");
      return false;
    }
    if (!orderType?.orderTypeId || orderType?.orderTypeId == 0) {
      errorNotification("Please select OrderType");
      return false;
    }
    if (!store?.storeId || store?.storeId == 0) {
      errorNotification("Please select store");
      return false;
    }
    if (!storeChannel?.id || storeChannel?.id == 0) {
      errorNotification("Please select store channel");
      return false;
    }
    setIsLoading(true);
    let param = {
      storeId: store?.storeId,
      saleChannelConfigId: storeChannel?.id,
      createdFrom: filterModal.createdFrom,
      createdTo: filterModal.createdTo,
      orderTypeId: orderType.orderTypeId,
    };
    SaleChannelOrderPreProcessor(param)
      .then((res) => {
        setIsLoading(false);
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
        }
        const orderDataArray = res.data.result;
        const updatedOrderDataArray = orderDataArray.map((order, index) => ({
          ...order,
          index,
          stationId: selectStation?.productStationId,
          orderTypeName: orderType?.orderTypeName,
          storeName: store?.storeName,
        }));
        setAllOrders(updatedOrderDataArray);

        const { errors } = res.data;

        if (errors?.FormatException) {
          console.log("FormatException:", errors.FormatException);
          errors.FormatException.forEach((error) => errorNotification(error));
          return;
        }

        if (errors?.Exception) {
          errors.Exception.forEach((error) => errorNotification(error));
          return;
        }

        if (errors?.InvalidParameter) {
          const errorMessages = JSON.parse(errors.InvalidParameter);
          console.log("errorMessages", errorMessages);
          setErrorsList(errorMessages);

          errorMessages.forEach((error, index) => {
            if (!error.IsSuccessed) {
              errorNotification(`Error at row ${index}: ${error.Msg}`);
            }
          });
        }
        setOpen(false);
      })
      .catch((e) => {
        setIsLoading(false);
      });
  };
  const [allStoreChannl, setAllStoreChannl] = useState([]);
  useEffect(() => {
    if (store?.storeId && store?.storeId > 0) {
      getChannelListByStoreIdForSelection();
    }
  }, [store]);
  let getChannelListByStoreIdForSelection = async () => {
    let res = await GetChannelListByStoreIdForSelection(store?.storeId);
    if (res.data.result !== null) {
      setAllStoreChannl(res.data.result);
    }
  };
  const [filterModal, setFilterModal] = useState({
    createdFrom: new Date(),
    createdTo: new Date(),
  });
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={"Sync Sale channel Order (s)"}
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_SYNC_ORDER
          }
          loading={isLoading}
          bg={purple}
          onClick={handleSubmitClick}
        />
      }
    >
      <Grid container spacing={2}>
        <Grid item md={6} sm={6} xs={12}>
          {" "}
          <InputLabel sx={styleSheet.inputLabel} required>
            {
              LanguageReducer?.languageType
                ?.ORDERS_SALE_CHANNEL_ORDERS_FROM_DATE
            }
          </InputLabel>
          <CustomReactDatePickerInput
            value={filterModal.createdFrom}
            onClick={(date) =>
              setFilterModal((prevState) => ({
                ...prevState,
                createdFrom: date,
              }))
            }
          />
        </Grid>
        <Grid item md={6} sm={6} xs={12}>
          {" "}
          <InputLabel sx={styleSheet.inputLabel} required>
            {LanguageReducer?.languageType?.ORDERS_SALE_CHANNEL_ORDERS_TO_DATE}
          </InputLabel>
          <CustomReactDatePickerInput
            value={filterModal.createdTo}
            onClick={(date) =>
              setFilterModal((prevState) => ({
                ...prevState,
                createdTo: date,
              }))
            }
            minDate={filterModal.createdFrom}
            disabled={!filterModal.createdFrom ? true : false}
          />
        </Grid>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            Select Station
          </InputLabel>
          <SelectComponent
            name="station"
            options={productStations}
            getOptionLabel={(option) => option?.sname}
            optionLabel={EnumOptions.SELECT_STATION.LABEL}
            optionValue={EnumOptions.SELECT_STATION.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllStationLookup}
            value={selectStation}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setselectStation(resolvedId);
            }}
          />
        </Grid>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel} required>
            Select OrderType
          </InputLabel>
          <SelectComponent
            name="orderType"
            options={allOrderTypeLookup}
            getOptionLabel={(option) => option?.name}
            optionLabel={EnumOptions.ORDER_TYPE.LABEL}
            optionValue={EnumOptions.ORDER_TYPE.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllOrderTypeLookup}
            value={orderType}
            onChange={(e, newValue) => {
              let resolvedId = newValue ? newValue : null;
              setOrderType(resolvedId);
            }}
          />
        </Grid>
        <Grid item md={12} sm={12} xs={12}>
          {" "}
          <InputLabel sx={styleSheet.inputLabel} required>
            Select Store
          </InputLabel>
          <SelectComponent
            name="store"
            options={allStores}
            value={store}
            optionLabel={EnumOptions.STORE.LABEL}
            optionValue={EnumOptions.STORE.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllStores}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setStore(resolvedId);
            }}
            size={"md"}
          />
        </Grid>
        {store?.storeId && store?.storeId > 0 ? (
          <Grid item md={12} sm={12} xs={12}>
            {" "}
            <InputLabel sx={styleSheet.inputLabel} required>
              Select Store Channel
            </InputLabel>
            <SelectComponent
              name="storeChannel"
              options={allStoreChannl}
              value={storeChannel}
              getOptionLabel={(option) => option.text}
              optionLabel={EnumOptions.ALL_SALE_CHANNEL.LABEL}
              optionValue={EnumOptions.ALL_SALE_CHANNEL.VALUE}
              isRefesh={true}
              handleRefreshClick={getChannelListByStoreIdForSelection}
              onChange={(e, newValue) => {
                const resolvedId = newValue ? newValue : null;
                setStoreChannel(resolvedId);
              }}
              size={"md"}
            />
          </Grid>
        ) : null}
        {/* <Grid item md={12} sm={12} sx={{ textAlign: "right" }}>
            {" "}
            <FormControlLabel
              sx={{ margin: "0px" }}
              control={
                <Checkbox
                  sx={{
                    color: "var(--primary-color)",
                    "&.Mui-checked": {
                      color: "var(--primary-color)",
                    },
                  }}
                  checked={true}
                  onChange={(e) => console.log(e.target.value)}
                  edge="start"
                  disableRipple
                  defaultChecked
                />
              }
              label={"Allow to display in Sale Channel"}
            />
          </Grid> */}
      </Grid>
    </ModalComponent>
  );
}
export default SaleChannelSyncModel;
