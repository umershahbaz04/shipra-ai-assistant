import React from "react";
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
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function UpdateOutscanModal(props) {
  let { open, setOpen } = props;
  const [chipData, setChipData] = React.useState([
    { key: 0, label: "Angular" },
  ]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    setOpen(false);
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={
        LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_BATCH_UPDATE
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.MY_CARRIER_DELIVERY_TASKS_BATCH_OUTSCAN
          }
          bg={purple}
          onClick={handleClose}
        />
      }
    >
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
      <InputLabel sx={styleSheet.inputLabel}>
        {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_STATUS}
      </InputLabel>
      <TextField select size="small" fullWidth variant="outlined">
        <MenuItem value="" selected disabled>
          Select
        </MenuItem>
        <MenuItem value="Lahore">Pending</MenuItem>
        <MenuItem value="Lahore">Completed</MenuItem>
        <MenuItem value="Lahore">Expire</MenuItem>
        <MenuItem value="Lahore">Closed</MenuItem>
      </TextField>
      <br />
      <br />
      <InputLabel sx={styleSheet.inputLabel}>
        {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_COMMENTS}
      </InputLabel>
      <TextField
        placeholder={`${LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_TAGS}...`}
        multiline
        rows={4}
        size="small"
        fullWidth
        variant="outlined"
      />
    </ModalComponent>
  );
}
export default UpdateOutscanModal;
