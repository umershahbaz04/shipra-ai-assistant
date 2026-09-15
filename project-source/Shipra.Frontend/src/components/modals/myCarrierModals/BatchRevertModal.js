import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import { Box, Card, Chip, Paper, Typography } from "@mui/material";
import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { RevertDeliveryTaskByOrderNos } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

function BatchRevertModal(props) {
  let { open, setOpen, orderNosData, getAllDeliveryTask, resetRowRef } = props;
  const [chipData, setChipData] = React.useState([]);
  const [isLoading, setIsLoading] = React.useState(false);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(orderNosData);
    setChipData(cData);
  }, [orderNosData]);
  const handleClose = () => {
    setOpen(false);
  };
  const handleSubmit = () => {
    let param = {
      orderNos: chipData.map((item) => item.label).join(),
    };

    setIsLoading(true);
    RevertDeliveryTaskByOrderNos(param)
      .then((res) => {
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification(
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_REVERT_SUCCESSFULLY
          );
          resetRowRef.current = true;
          getAllDeliveryTask();
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
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={
        LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_BATCH_REVERT
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_BATCH_REVERT
          }
          bg={purple}
          onClick={(e) => handleSubmit()}
          loading={isLoading}
        />
      }
    >
      <Card variant="outlined" sx={styleSheet.tagsCard}>
        <Typography sx={styleSheet.tagsCardHeading}>
          {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_ORDER_NOS}
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
                  // onDelete={() => {}}
                />
              </Box>
            );
          })}
        </Paper>
      </Card>
    </ModalComponent>
  );
}
export default BatchRevertModal;
