import { Box, Grid, InputLabel } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetChannelListByStoreIdForSelection,
  SaleChannelProductPreProcessor,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function SaleChannelProductSyncModel(props) {
  let { open, setOpen, allStores, setAllProducts, resetRowRef, getAllStores } =
    props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);
  const [filterModal, setFilterModal] = useState({
    createdFrom: new Date(),
    createdTo: new Date(),
  });
  const handleClose = () => {
    setOpen(false);
  };

  const [store, setStore] = useState({
    storeId: 0,
    storeName: "Select Please",
  });
  const [storeChannel, setStoreChannel] = useState({
    id: 0,
    text: "Select Please",
  });
  const handleSubmitClick = () => {
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
    };
    SaleChannelProductPreProcessor(param)
      .then((res) => {
        setIsLoading(false);
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data?.errors);
        } else {
          setAllProducts(res.data.result);
          setOpen(false);
        }
      })
      .catch((e) => {
        setIsLoading(false);
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
        );
        console.log("e", e);
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

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={
        LanguageReducer?.languageType
          ?.PRODUCT_SALE_CHANNEL_PRODUCT_SYNC_SALE_CHANNEL_PRODUCTS
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.PRODUCT_SALE_CHANNEL_PRODUCT_SYNC_PRODUCT
          }
          loading={isLoading}
          bg={purple}
          onClick={handleSubmitClick}
        />
      }
    >
      <Box>
        <Grid container spacing={2}>
          <Grid item md={12} sm={12} xs={12}>
            {" "}
            <InputLabel sx={styleSheet.inputLabel} required>
              {
                LanguageReducer?.languageType
                  ?.PRODUCT_SALE_CHANNEL_PRODUCT_FROM_DATE
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
          <Grid item md={12} sm={12} xs={12}>
            {" "}
            <InputLabel sx={styleSheet.inputLabel} required>
              {
                LanguageReducer?.languageType
                  ?.PRODUCT_SALE_CHANNEL_PRODUCT_TO_DATE
              }
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
            {" "}
            <InputLabel sx={styleSheet.inputLabel} required>
              {
                LanguageReducer?.languageType
                  ?.PRODUCT_SALE_CHANNEL_PRODUCT_STORE
              }
            </InputLabel>
            <SelectComponent
              name="store"
              options={allStores}
              value={store}
              getOptionLabel={(option) => option.storeName}
              optionLabel={EnumOptions.STORE.LABEL}
              optionValue={EnumOptions.STORE.VALUE}
              isRefesh={true}
              handleRefreshClick={getAllStores}
              onChange={(e, newValue) => {
                const resolvedId = newValue ? newValue : null;
                setStore(resolvedId);
              }}
            />
          </Grid>
          {store?.storeId && store?.storeId > 0 ? (
            <Grid item md={12} sm={12} xs={12}>
              {" "}
              <InputLabel sx={styleSheet.inputLabel} required>
                {
                  LanguageReducer?.languageType
                    ?.PRODUCT_SALE_CHANNEL_STORE_CHANNEL
                }
              </InputLabel>
              <SelectComponent
                name="storeChannel"
                options={allStoreChannl}
                value={storeChannel}
                optionLabel={EnumOptions.STORE_CHANNEL.LABEL}
                optionValue={EnumOptions.STORE_CHANNEL.VALUE}
                isRefesh={true}
                handleRefreshClick={getChannelListByStoreIdForSelection}
                onChange={(e, newValue) => {
                  const resolvedId = newValue ? newValue : null;
                  setStoreChannel(resolvedId);
                }}
              />
            </Grid>
          ) : null}
        </Grid>
      </Box>
    </ModalComponent>
  );
}
export default SaleChannelProductSyncModel;
