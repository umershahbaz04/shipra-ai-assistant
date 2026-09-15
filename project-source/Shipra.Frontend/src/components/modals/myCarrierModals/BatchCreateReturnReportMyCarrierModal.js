import React, { useEffect } from "react";
import {
  Card,
  Dialog,
  DialogContent,
  Box,
  Chip,
  DialogActions,
  Button,
  DialogTitle,
  Typography,
  Paper,
  InputLabel,
  TextField,
  MenuItem,
} from "@mui/material";
import { styleSheet } from "../../../assets/styles/style";
import Slide from "@mui/material/Slide";
import { useSelector } from "react-redux";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import {
  CreateMyCarrierReturnReport,
  RevertDeliveryTaskByOrderNos,
} from "../../../api/AxiosInterceptors";
import { LoadingButton } from "@mui/lab";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function BatchCreateReturnReportMyCarrierModal(props) {
  let { open, setOpen, orderNosData, getAll, resetRowRef } = props;
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
    CreateMyCarrierReturnReport(param)
      .then((res) => {
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Return report created successfully");
          resetRowRef.current = true;
          getAll();
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
      maxWidth="sm"
      title={"Create Return Report"}
      actionBtn={
        <ModalButtonComponent
          title={"Create Return Report"}
          bg={purple}
          onClick={(e) => handleSubmit()}
          loading={isLoading}
        />
      }
    >
      <Card variant="outlined" sx={styleSheet.tagsCard}>
        <Typography sx={styleSheet.tagsCardHeading}>{"Order Nos"}</Typography>
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
export default BatchCreateReturnReportMyCarrierModal;
