import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import { Box, Card, Chip, InputLabel, Paper } from "@mui/material";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CreateAbleSelectComponent from "../../../.reUseableComponents/TextField/CreateAbleSelectComponent";
import {
  CreateClientOrderLabel,
  CreateClientOrderLabelLookup,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { graphColorsArr, orderLabelColor } from "../../../utilities/helpers/Helpers";
import { successNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const AddOrderLabelModal = (props) => {
  const {
    open,
    onClose,
    orderNosData,
    getAllOrders,
    orderLabel,
    setOrderLabel,
  } = props;
  const [chipData, setChipData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selectedOrderLabels, setselectedOrderLabels] = useState();

  const createClientOrderLabel = async () => {
    const _orderNos = chipData.map((item) => item.label).join();
    const ordrLabels = selectedOrderLabels.map((item) => ({
      label: item.labelName,
      colorCode: item.colorCode,
    }));
    const body = {
      OrderNos: _orderNos,
      labels: ordrLabels,
    };
    console.log(body);
    setLoading(true);
    try {
      const response = await CreateClientOrderLabel(body);
      console.log(response);
      if (response?.data?.isSuccess) {
        successNotification(response?.data?.result?.message);
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

  const handleCreateLabel = async (inputValue) => {
    const randomIndex = Math.floor(Math.random() * orderLabelColor.length);
    const newOption = {
      clientOrderLabelLookupId: Date.now(),
      labelName: inputValue,
      colorCode: orderLabelColor[randomIndex],
    };
    setOrderLabel((prev) => [...prev, newOption]);
    setselectedOrderLabels((prev) => [...(prev || []), newOption]);
    try {
      const response = await CreateClientOrderLabelLookup(
        newOption?.labelName,
        newOption?.colorCode
      );
      if (response?.data?.isSuccess) {
        successNotification(response?.data?.result?.message);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    }
  };

  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(orderNosData);
    setChipData(cData);
  }, [orderNosData]);

  return (
    <ModalComponent
      maxWidth="sm"
      open={open}
      onClose={onClose}
      title={"Add Order Labels"}
      actionBtn={
        <ModalButtonComponent
          loading={loading}
          onClick={createClientOrderLabel}
          disabled={loading}
          title={"Add Order Labels"}
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
        <InputLabel sx={styleSheet.inputLabel}>{"Order Label"}</InputLabel>
        <CreateAbleSelectComponent
          name="orderLabel"
          options={orderLabel}
          value={selectedOrderLabels}
          onCreateOption={handleCreateLabel}
          height={40}
          optionLabel={EnumOptions.ORDER_LABELS.LABEL}
          optionValue={EnumOptions.ORDER_LABELS.VALUE}
          onChange={(e, val) => {
            setselectedOrderLabels(val);
          }}
        />
      </Box>
    </ModalComponent>
  );
};

export default AddOrderLabelModal;
