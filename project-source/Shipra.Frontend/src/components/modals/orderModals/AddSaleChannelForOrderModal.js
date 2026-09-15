import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import { Box, Card, Chip, InputLabel, Paper } from "@mui/material";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetChannelListByStoreIdForSelection,
  UpdateOrderWithSaleChannel,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

const AddSaleChannelForOrderModal = (props) => {
  const { open, onClose, orderNosData, storeId, getAllOrders } = props;
  const [chipData, setChipData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [allSaleChannel, setAllSaleChannel] = useState([]);
  const [selectedOrderSaleChannel, setSelectedOrderSaleChannel] = useState();

  const getChannelListByStoreIdForSelection = async () => {
    setSelectedOrderSaleChannel(null);
    if (storeId) {
      let res = await GetChannelListByStoreIdForSelection(storeId);
      setAllSaleChannel(res.data.result || []);
    }
  };

  const createOrderSaleChannel = async () => {
    const _orderNos = chipData.map((item) => item.label).join();
    if (
      selectedOrderSaleChannel == null ||
      selectedOrderSaleChannel === 0 ||
      selectedOrderSaleChannel?.id === 0
    ) {
      errorNotification("Please Select a Sale Channel");
      return;
    }
    setLoading(true);
    try {
      const response = await UpdateOrderWithSaleChannel(
        _orderNos,
        selectedOrderSaleChannel?.id,
        false
      );
      if (response?.data?.isSuccess) {
        successNotification(
          "Sales channel added successfully for the selected orders."
        );
        getAllOrders();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
      onClose();
    }
  };

  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(orderNosData);
    setChipData(cData);
  }, [orderNosData]);

  useEffect(() => {
    getChannelListByStoreIdForSelection();
  }, []);
  return (
    <ModalComponent
      maxWidth="sm"
      open={open}
      onClose={onClose}
      title={"Add Sale Channel"}
      actionBtn={
        <ModalButtonComponent
          loading={loading}
          onClick={createOrderSaleChannel}
          disabled={loading}
          title={"Add Sale Channel"}
        />
      }
    >
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
                  deleteIcon={<CloseIcon sx={{ color: "white  !important" }} />}
                  label={data.label}
                />
              </Box>
            );
          })}
        </Paper>
      </Card>
      <Box>
        <InputLabel sx={styleSheet.inputLabel}>{"Sale Channel"}</InputLabel>
        <SelectComponent
          name="saleChannel"
          options={allSaleChannel}
          value={selectedOrderSaleChannel}
          height={40}
          optionLabel={EnumOptions.STORE_CHANNEL.LABEL}
          optionValue={EnumOptions.STORE_CHANNEL.VALUE}
          onChange={(e, val) => {
            setSelectedOrderSaleChannel(val);
          }}
        />
      </Box>
    </ModalComponent>
  );
};

export default AddSaleChannelForOrderModal;
