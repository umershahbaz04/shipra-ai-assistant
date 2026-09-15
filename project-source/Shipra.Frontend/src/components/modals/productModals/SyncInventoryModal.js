import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import {
  Box,
  Card,
  Chip,
  Grid,
  InputLabel,
  Paper,
  Typography,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAllSaleChannelByLookupIdForSelection,
  GetAllSaleChannelLookupForSelection,
  SaleChannelInventorySync,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import {
  PaymentMethodCheckbox,
  purple,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function SyncInventoryModal(props) {
  const [isLoading, setIsLoading] = useState(false);
  let { open, setOpen, getAll, resetRowRef, selectedItems } = props;
  const [allSaleChannelLookup, setAllSaleChannelLookup] = useState([]);
  const [allSaleChannelConfig, setAllSaleChannelConfig] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [chipData, setChipData] = React.useState([]);

  const [selectedSaleChannel, setSelectedSaleChannel] = useState({
    saleChannelLookupId: 0,
    saleChannelName: "Select Please",
  });
  const [selectedSaleChannelConfig, setSelectedSaleChannelConfig] = useState({
    id: 0,
    text: "Select Please",
  });

  const handleClose = () => {
    setOpen(false);
  };
  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(selectedItems);
    setChipData(cData);
  }, [selectedItems]);

  let gtAllSaleChannelLookupForSelection = async () => {
    let res = await GetAllSaleChannelLookupForSelection();
    if (res.data.result != null) {
      setAllSaleChannelLookup(res.data.result);
    }
  };
  let getAllSaleChannelByLookupIdForSelection = async () => {
    let res = await GetAllSaleChannelByLookupIdForSelection(
      selectedSaleChannel?.saleChannelLookupId
    );
    setAllSaleChannelConfig(res.data.result);
  };

  useEffect(() => {
    gtAllSaleChannelLookupForSelection();
  }, []);
  useEffect(() => {
    getAllSaleChannelByLookupIdForSelection();
  }, [selectedSaleChannel]);

  const [saleChannelTowards, setSaleChannelTowards] = useState({
    saleChannelTowardsShipra: true,
    shipraTowardsSaleChannel: false,
  });
  const paymentMethodoptions = [
    { id: 1, label: "Sale Channels ➠ Shipra" },
    { id: 2, label: "Shipra  ➠ Sale Channels" },
  ];
  const handleCheckboxChange = (optionId) => {
    setselectedPMOption(optionId);

    if (paymentMethodoptions[0].id == optionId) {
      //it means sale channel to ward shipra
      setSaleChannelTowards({
        saleChannelTowardsShipra: true,
        shipraTowardsSaleChannel: false,
      });
    } else {
      setSaleChannelTowards({
        saleChannelTowardsShipra: false,
        shipraTowardsSaleChannel: true,
      });
    }
  };
  const [selectedPMOption, setselectedPMOption] = useState(
    paymentMethodoptions[0].id
  );

  const handleSyncInventory = async () => {
    try {
      if (
        !selectedSaleChannel?.saleChannelLookupId ||
        selectedSaleChannel?.saleChannelLookupId == 0
      ) {
        errorNotification(`Please choose sale channel`);
        return false;
      }
      if (!selectedSaleChannelConfig?.id) {
        errorNotification(`Please choose sale(s) channel`);
        return false;
      }

      setIsLoading(true);

      let body = {
        saleChannelLookupId: selectedSaleChannel?.saleChannelLookupId,
        saleChannelConfigId: selectedSaleChannelConfig.id,
        shipraTowardsSaleChannel: saleChannelTowards.shipraTowardsSaleChannel,
        saleChannelTowardsShipra: saleChannelTowards.saleChannelTowardsShipra,
        productStockSkus: chipData.map((item) => item.label).join(),
      };
      const response = await SaleChannelInventorySync(body);

      if (!response.data.isSuccess) {
        let jsonData = response.data.errors;
        UtilityClass.showErrorNotificationWithDictionary(jsonData);
      } else {
        successNotification("Action perform successfully");
        getAll();
        handleClose();
        resetRowRef.current = true;
      }
    } catch (e) {
      UtilityClass.showErrorNotificationWithDictionary(
        e?.response?.data?.errors
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={LanguageReducer?.languageType?.PRODUCT_INVENTORY_SYNC_INVENTORY}
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType?.PRODUCT_INVENTORY_SYNC_INVENTORY
          }
          loading={isLoading}
          bg={purple}
          onClick={(e) => handleSyncInventory()}
        />
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12}>
          <Card variant="outlined" sx={styleSheet.tagsCard}>
            <Typography sx={styleSheet.tagsCardHeading}>
              {LanguageReducer?.languageType?.PRODUCT_INVENTORY_SKUS}
            </Typography>
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
        </Grid>
        <Grid item md={12} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.PRODUCT_INVENTORY_ALL_SALE_CHANNEL}
          </InputLabel>
          <SelectComponent
            name="allsaleChannel"
            size="md"
            options={allSaleChannelLookup}
            value={selectedSaleChannel}
            optionLabel={EnumOptions.SALE_CHANNEL.LABEL}
            optionValue={EnumOptions.SALE_CHANNEL.VALUE}
            isRefesh={true}
            handleRefreshClick={gtAllSaleChannelLookupForSelection}
            onChange={(e, val) => {
              setSelectedSaleChannel(val);
            }}
          />
        </Grid>
        {selectedSaleChannel?.saleChannelLookupId > 0 && (
          <Grid item md={12} sm={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.PRODUCT_INVENTORY_SALE_CHANNEL}
            </InputLabel>
            <SelectComponent
              name="saleChannelById"
              size="md"
              options={allSaleChannelConfig}
              value={selectedSaleChannelConfig}
              optionLabel={EnumOptions.ALL_SALE_CHANNEL.LABEL}
              optionValue={EnumOptions.ALL_SALE_CHANNEL.VALUE}
              isRefesh={true}
              handleRefreshClick={getAllSaleChannelByLookupIdForSelection}
              onChange={(e, val) => {
                setSelectedSaleChannelConfig(val);
              }}
            />
          </Grid>
        )}
        {selectedSaleChannelConfig.id > 0 &&
          selectedSaleChannel?.saleChannelLookupId > 0 && (
            <Box mt={2} ml={0.7} marginLeft={3}>
              {paymentMethodoptions.map((option) => (
                <>
                  <Grid item md={12} sm={12}>
                    <PaymentMethodCheckbox
                      key={option.id}
                      checked={selectedPMOption === option.id}
                      onChange={() => handleCheckboxChange(option.id)}
                      label={option.label}
                    />
                  </Grid>
                </>
              ))}
            </Box>
          )}
      </Grid>
    </ModalComponent>
  );
}
export default SyncInventoryModal;
