import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import { LoadingButton } from "@mui/lab";
import {
  Box,
  Button,
  Card,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  InputLabel,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import {
  CreateCarrierPaymentSettlement,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function CreateSettlementModelWithSelection(props) {
  let {
    open,
    setOpen,
    orderNosData,
    getAllCODPendings,
    allCODPendings,
    resetRowRef,
    carrierId,
  } = props;
  const [chipData, setChipData] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [paymentDate, setPaymentDate] = useState(new Date());
  const [paymentRef, setPaymentRef] = useState(UtilityClass.GetPaymentRef());

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(orderNosData);
    setChipData(cData);
  }, [orderNosData]);
  const handleClose = () => {
    setOpen(false);
  };
  const handleSubmit = () => {
    if (paymentDate == "") {
      errorNotification(
        LanguageReducer?.languageType
          ?.MY_CARRIER_COD_PENDING_PLEASE_CHOOSE_PAYMENT_DATE
      );
      return false;
    }
    if (paymentRef == "") {
      errorNotification(
        LanguageReducer?.languageType
          ?.MY_CARRIER_COD_PENDING_PLEASE_CHOOSE_PAYMENT_REFERENCE
      );
      return false;
    }

    const filteredOrders = allCODPendings.filter((order) => {
      return chipData.some((label) => label.label === order.OrderNo);
    });
    const actualArray = filteredOrders.map((order) => ({
      OrderNo: order.OrderNo,
      Amount: order.Amount,
      paymentRef: paymentRef,
      paymentDate: UtilityClass.getFormatedDateWithoutTime(paymentDate),
    }));

    let param = {
      list: actualArray,
      CarrierId: carrierId,
    };

    setIsLoading(true);
    CreateCarrierPaymentSettlement(param)
      .then((res) => {
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification(
            LanguageReducer?.languageType
              ?.MY_CARRIER_COD_PENDING_SETTLEMENT_CREATED_SUCCESSFULLY
          );
          resetRowRef.current = true;
          getAllCODPendings();
          setOpen(false);
        }
      })
      .catch((e) => {
        errorNotification(
          LanguageReducer?.languageType
            ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
        );
        console.log("e", e);
      })
      .finally((e) => {
        setIsLoading(false);
      });
  };
  const handleRefChange = (e) => {
    setPaymentRef(e.target.value);
  };
  const getPaymentRef = (e) => {
    setPaymentRef(UtilityClass.GetPaymentRef());
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={
        LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_CREATE_SETTLEMENT
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_COD_PENDING_CREATE_SETTLEMENT
          }
          loading={isLoading}
          bg={purple}
          onClick={(e) => handleSubmit()}
        />
      }
    >
      <Card variant="outlined" sx={styleSheet.tagsCard}>
        <Typography sx={styleSheet.tagsCardHeading}>
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_ORDER_NOS}
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
          {chipData.map((data) => {
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
                  onDelete={() => {}}
                />
              </Box>
            );
          })}
        </Paper>
      </Card>
      <br />
      <InputLabel required sx={styleSheet.inputLabel}>
        {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_PAYMENT_DATE}
      </InputLabel>
      <CustomReactDatePickerInput
        value={paymentDate}
        onClick={(date) => setPaymentDate(date)}
        size="small"
        // isClearable

        // inputProps={{ style: { width: "200px" } }}
      />
      <br />
      <br />
      <Stack justifyContent={"space-between"} direction={"row"}>
        <InputLabel required sx={styleSheet.inputLabel}>
          {LanguageReducer?.languageType?.MY_CARRIER_COD_PENDING_PAYMENT_REF}
        </InputLabel>
        <ButtonComponent
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_COD_PENDING_AUTO_GENERATE_REF
          }
          onClick={getPaymentRef}
        />
      </Stack>
      <TextFieldComponent
        required
        onChange={handleRefChange}
        value={paymentRef}
      />
    </ModalComponent>
  );
}
export default CreateSettlementModelWithSelection;
