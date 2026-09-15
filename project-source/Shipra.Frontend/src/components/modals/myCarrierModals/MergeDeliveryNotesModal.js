import {
  Alert,
  Box,
  Card,
  Chip,
  Grid,
  InputLabel,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CustomReactDatePickerInputFilter from "../../../.reUseableComponents/TextField/CustomReactDatePickerInputFilter";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { MergeDeliveryNotes } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

function MergeDeliveryNotesModal(props) {
  const { open, setOpen, selectedNotesData, getAllDeliveryNote, resetRowRef } = props;
  const [isLoading, setIsLoading] = useState(false);
  const [targetNote, setTargetNote] = useState(null);
  const [createdDate, setCreatedDate] = useState(new Date());

  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  useEffect(() => {
    if (selectedNotesData && selectedNotesData.length > 0) {
      const firstNote = selectedNotesData[0];
      setTargetNote({
        title: firstNote.NoteNo,
        value: firstNote.DeliveryNoteId,
        ...firstNote,
      });
    }
  }, [selectedNotesData]);

  const handleClose = () => {
    setOpen(false);
  };

  const noteOptions = (selectedNotesData || []).map((note) => ({
    title: `${note.NoteNo} (${note.ShipmentCount || 0} shipments)`,
    value: note.DeliveryNoteId,
    ...note,
  }));

  const handleSubmit = async () => {
    if (!targetNote || !targetNote.value) {
      errorNotification("Please choose a primary Delivery Note.");
      return;
    }

    const sourceIds = selectedNotesData.map((note) => note.DeliveryNoteId);

    const body = {
      SourceDeliveryNoteIds: sourceIds,
      TargetDeliveryNoteId: targetNote.value,
      CreatedDate: createdDate || null,
    };

    setIsLoading(true);
    try {
      const res = await MergeDeliveryNotes(body);
      if (res?.data?.isSuccess) {
        successNotification(res?.data?.result?.message || "Delivery Notes merged successfully!");
        if (resetRowRef) resetRowRef.current = true;
        if (getAllDeliveryNote) getAllDeliveryNote();
        handleClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
      }
    } catch (err) {
      errorNotification("Failed to merge Delivery Notes.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      setOpen={setOpen}
      title="Merge Delivery Notes"
      actionBtn={
        <ModalButtonComponent
          title="Merge Notes"
          bg={purple}
          onClick={handleSubmit}
          loading={isLoading}
        />
      }
    >
      <Stack spacing={2} sx={{ p: "15px" }}>
        {/* Warning Alert Statement */}
        <Alert severity="warning" variant="outlined">
          <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
            Important Warning / Caution:
          </Typography>
          <Typography variant="body2" sx={{ fontSize: "12.5px", mt: 0.5 }}>
            All orders from the non-selected delivery notes will be reassigned into the chosen primary Delivery Note. The other delivery notes will be permanently deleted. <strong>This action cannot be undone.</strong>
          </Typography>
        </Alert>

        {/* Selected Delivery Note Chips */}
        <Box>
          <InputLabel sx={{ ...styleSheet.inputLabel, mb: 0.5 }}>
            Selected Delivery Notes ({selectedNotesData?.length || 0}):
          </InputLabel>
          <Card variant="outlined" sx={{ borderRadius: "10px !important", p: "8px 10px", borderColor: "#e0e0e0" }}>
            <Paper
              sx={{
                display: "flex !important",
                justifyContent: "flex-start !important",
                flexWrap: "wrap !important",
                p: 0,
                m: 0,
              }}
              elevation={0}
            >
              {selectedNotesData?.map((note) => (
                <Box key={note.DeliveryNoteId} sx={{ mr: "8px", mb: "4px", mt: "4px" }}>
                  <Chip
                    sx={{
                      backgroundColor: "var(--primary-color) !important",
                      color: "white !important",
                      borderRadius: "15px",
                      fontWeight: "bold",
                      fontSize: "12px",
                      px: 0.5,
                    }}
                    size="small"
                    icon={
                      <CheckCircleIcon
                        fontSize="small"
                        sx={{ color: "white !important" }}
                      />
                    }
                    label={note.NoteNo}
                  />
                </Box>
              ))}
            </Paper>
          </Card>
        </Box>

        {/* Form Fields */}
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              Select Primary Delivery Note
            </InputLabel>
            <SelectComponent
              name="targetNote"
              options={noteOptions}
              value={targetNote}
              optionLabel="title"
              optionValue="value"
              onChange={(e, val) => setTargetNote(val)}
              height={40}
            />
          </Grid>

          <Grid item xs={12}>
            <InputLabel sx={styleSheet.inputLabel}>
              Delivery Note Date
            </InputLabel>
            <CustomReactDatePickerInputFilter
              value={createdDate}
              onClick={(date) => setCreatedDate(date)}
              size="small"
              maxDate={UtilityClass.todayDate()}
              inputProps={{ style: { padding: "8.5px 14px", fontSize: "14px" } }}
            />
          </Grid>
        </Grid>
      </Stack>
    </ModalComponent>
  );
}

export default MergeDeliveryNotesModal;
