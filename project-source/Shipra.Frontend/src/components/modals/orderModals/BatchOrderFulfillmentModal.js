import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import { Box, Card, Chip, Grid, InputLabel, Paper } from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { BatchOrderFulfillment } from "../../../api/AxiosInterceptors";
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
function BatchOrderFulfillmentModal(props) {
  let {
    open,
    setOpen,
    orderNosData,
    allFullFillment,
    getAllOrders,
    resetRowRef,
  } = props;
  const [chipData, setChipData] = React.useState();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);
  const handleClose = () => {
    setOpen(false);
  };

  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(orderNosData);
    setChipData(cData);
  }, [orderNosData]);

  const [fullfilmentStatusId, setFullfilmentStatusId] = useState({
    id: 0,
    text: "Select Please",
  });
  const handleSubmitClick = () => {
    if (!fullfilmentStatusId?.id || fullfilmentStatusId?.id == 0) {
      errorNotification("Please choose fulfillment status");
      return false;
    }

    let param = {
      fullfilmentStatusId: fullfilmentStatusId?.id,
      orderNos: chipData.map((item) => item.label).join(),
    };
    setIsLoading(true);

    BatchOrderFulfillment(param)
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
          successNotification("Order fulfilled successfully");
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
      title={LanguageReducer?.languageType?.ORDER_BATCH_FULLFILLMENT}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.ORDER_BATCH_FULLFILLMENT}
          loading={isLoading}
          bg={purple}
          onClick={handleSubmitClick}
        />
      }
    >
      <Grid container spacing={2}>
        <Grid item md={12} sm={12}>
          {" "}
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
        <Grid item md={12} sm={12} mb={4}>
          {" "}
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.ORDER_SELECT_FULFILLMENT_STATUS}
          </InputLabel>
          <SelectComponent
            name="fullfillmentstatus"
            options={allFullFillment}
            value={fullfilmentStatusId}
            optionLabel={EnumOptions.FULL_FILLMENT_STATUS.LABEL}
            optionValue={EnumOptions.FULL_FILLMENT_STATUS.VALUE}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setFullfilmentStatusId(resolvedId);
            }}
            size={"md"}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
}
export default BatchOrderFulfillmentModal;
