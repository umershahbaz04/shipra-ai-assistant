import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import {
  Box,
  Card,
  Chip,
  Grid,
  InputLabel,
  Paper,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { BatchUpdateOrderStatus } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function BatchUpdateOrderStatusModal(props) {
  let {
    open,
    setOpen,
    orderNosData,
    allCarrierTrackingStatus,
    getAllOrders,
    resetRowRef,
  } = props;
  const [chipData, setChipData] = React.useState();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);
  const [carrierStatusId, setCarrierStatusId] = useState({
    carrierTrackingStatusId: 0,
    trackingStatus: "Select Please",
  });
  const [comment, setComment] = useState("");
  const handleClose = () => {
    setOpen(false);
  };

  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(orderNosData);
    setChipData(cData);
  }, [orderNosData]);

  const handleSubmitClick = () => {
    if (
      !carrierStatusId?.carrierTrackingStatusId ||
      carrierStatusId.carrierTrackingStatusId == 0
    ) {
      errorNotification("Please choose carrier status");
      return false;
    }
    let param = {
      carrierStatusId: carrierStatusId.carrierTrackingStatusId,
      orderNos: chipData.map((item) => item.label).join(),
      comments: comment,
    };
    setIsLoading(true);

    BatchUpdateOrderStatus(param)
      .then((res) => {
        setIsLoading(false);
        if (!res.data.isSuccess) {
          var errJson = res.data.errors?.InvalidOrders[0];
          let jsonData = JSON.parse(errJson);
          jsonData?.forEach((obj, i) => {
            let message = `${obj.Message} \n ${obj.OrderNo}`;
            errorNotification(message);
          });
        } else {
          successNotification("Order status update successfully");
          resetRowRef.current = true;
          getAllOrders();
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
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={LanguageReducer?.languageType?.ORDER_BATCH_UPDATE_ORDER_STATUS}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.ORDER_BATCH_UPDATE_ORDER_STATUS}
          loading={isLoading}
          bg={purple}
          onClick={handleSubmitClick}
        />
      }
    >
      <Grid container spacing={2}>
        <Grid item md={12} sm={12}>
          <Card variant="outlined" sx={styleSheet.tagsCard}>
            {/* <Typography sx={styleSheet.tagsCardHeading}>
              {LanguageReducer?.languageType?.TAGS_TEXT}
            </Typography> */}
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
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.ORDER_CARRIER_STATUS}
          </InputLabel>
          <SelectComponent
            name="carrier"
            options={allCarrierTrackingStatus}
            value={carrierStatusId}
            height={40}
            getOptionLabel={(option) => option.trackingStatus}
            optionLabel={EnumOptions.CARRIER_TRACKING_STATUS.LABEL}
            optionValue={EnumOptions.CARRIER_TRACKING_STATUS.VALUE}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setCarrierStatusId(resolvedId);
            }}
            size={"md"}
          />
        </Grid>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {"Comment"}
          </InputLabel>
          <TextField
            multiline
            onChange={(e) => setComment(e.target.value)}
            size="md"
            fullWidth
            variant="outlined"
            rows={2}
          ></TextField>
        </Grid>
      </Grid>
    </ModalComponent>
  );
}
export default BatchUpdateOrderStatusModal;
