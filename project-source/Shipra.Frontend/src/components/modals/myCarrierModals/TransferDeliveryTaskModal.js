import {
  Box,
  Grid,
  InputLabel,
  Typography,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  FormControlLabel,
  Checkbox,
  Radio,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { DataGrid } from "@mui/x-data-grid";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateDeliveryTaskAndAddToExistingNote,
  GetDriverLatestDeliveryNoteToday,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { EnumOptions } from "../../../utilities/enum";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import Colors from "../../../utilities/helpers/Colors";
import {
  centerColumn,
  purple,
} from "../../../utilities/helpers/Helpers";

function TransferDeliveryTaskModal(props) {
  let {
    open,
    setOpen,
    orderNosData,
    allDrivers,
    getAllDeliveryTask,
    resetRowRef,
    handleResetOutscanModel,
    getDriversForSelection,
  } = props;
  const [isLoading, setIsLoading] = React.useState(false);
  const [driverId, setDriverId] = React.useState(0);
  const [notes, setNotes] = useState([]);
  const [selectedNoteNo, setSelectedNoteNo] = useState("");
  const [fetchingNote, setFetchingNote] = useState(false);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const handleClose = () => {
    setOpen(false);
  };

  const handleDriverChange = async (e, value) => {
    setDriverId(value);
    setNotes([]);
    setSelectedNoteNo("");
    if (value && value.DriverId) {
      setFetchingNote(true);
      try {
        const res = await GetDriverLatestDeliveryNoteToday(value.DriverId);
        if (res.data.isSuccess && res.data.result && res.data.result.data) {
          setNotes(res.data.result.data);
          if (res.data.result.data.length > 0) {
            setSelectedNoteNo(res.data.result.data[0].noteNo);
          }
        } else {
          setNotes([]);
        }
      } catch (err) {
        setNotes([]);
      } finally {
        setFetchingNote(false);
      }
    } else {
      setNotes([]);
    }
  };

  const handleSubmit = () => {
    if (!driverId || driverId == 0) {
      errorNotification("Please choose driver");
      return false;
    }
    let param = {
      driverId: driverId.DriverId,
      orderNos: orderNosData.join(),
      assigningDate: new Date(),
      AddToExisting: selectedNoteNo ? true : false,
      NoteNo: selectedNoteNo || "",
      IsTransfer: true,
    };
    setIsLoading(true);
    CreateDeliveryTaskAndAddToExistingNote(param)
      .then((res) => {
        if (!res.data.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Transferred successfully");
          resetRowRef.current = true;
          getAllDeliveryTask();
          if (handleResetOutscanModel) handleResetOutscanModel();
          handleClose();
        }
      })
      .catch((err) => {
        UtilityClass.showErrorNotification(err);
      })
      .finally(() => {
        setIsLoading(false);
      });
  };





  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      setOpen={setOpen}
      title="Transfer / Create Delivery Task"
      actionBtn={
        <ModalButtonComponent
          title="Submit"
          bg={purple}
          onClick={handleSubmit}
          loading={isLoading || fetchingNote}
        />
      }
    >
      <Grid container spacing={1} sx={{ p: "15px" }}>
        <Grid item xl={6} lg={6} md={6}>
          <Grid>
            <InputLabel required sx={styleSheet.inputLabel}>
              {LanguageReducer?.languageType?.MY_CARRIER_DELIVERY_TASKS_SELECT_DRIVER}
            </InputLabel>
            <SelectComponent
              name="reason"
              options={allDrivers || []}
              value={driverId}
              isRefesh={true}
              handleRefreshClick={getDriversForSelection}
              optionLabel={EnumOptions.DRIVER.LABEL}
              optionValue={EnumOptions.DRIVER.VALUE}
              onChange={handleDriverChange}
            />
          </Grid>
        </Grid>
      </Grid>

      {driverId !== 0 && (
        <Grid container spacing={1} sx={{ p: "15px", pt: 0 }}>
          <Grid item xs={12}>
            {fetchingNote ? (
              <Box sx={{ padding: "10px", borderRadius: "5px", backgroundColor: Colors.lightWarning, border: `1px solid ${Colors.warning}` }}>
                <Typography variant="body2">Checking for today's delivery notes...</Typography>
              </Box>
            ) : notes.length > 0 ? (
              <Box>
                <Typography variant="body2" sx={{ mb: 1 }}>
                  <strong>Notice:</strong> This driver has active delivery notes for today. The latest one is auto-selected, but you can select a different one.
                </Typography>
                {notes.map((note, index) => (
                  <Accordion key={note.noteNo} defaultExpanded={index === 0}>
                    <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                      <FormControlLabel
                        aria-label="Acknowledge"
                        onClick={(event) => event.stopPropagation()}
                        onFocus={(event) => event.stopPropagation()}
                        control={
                          <Checkbox
                            checked={selectedNoteNo === note.noteNo}
                            onChange={(e) => {
                              if (e.target.checked) setSelectedNoteNo(note.noteNo);
                              else setSelectedNoteNo("");
                            }}
                          />
                        }
                        label={
                          <Typography variant="subtitle2">
                            <strong>Note No:</strong> {note.noteNo} | <strong>Pending:</strong> {note.totalPendingCount} | <strong>Created:</strong> {UtilityClass.convertUtcToLocalAndGetDate(note.createdOn)}
                          </Typography>
                        }
                      />
                    </AccordionSummary>
                    <AccordionDetails sx={{ bgcolor: "#fafafa" }}>
                      {note.orders && note.orders.length > 0 ? (
                        <TableContainer component={Paper} elevation={0} variant="outlined">
                          <Table size="small">
                            <TableHead sx={{ bgcolor: "#eee" }}>
                              <TableRow>
                                <TableCell><strong>Order No</strong></TableCell>
                                <TableCell><strong>Tracking No</strong></TableCell>
                                <TableCell><strong>Customer</strong></TableCell>
                                <TableCell><strong>Address</strong></TableCell>
                              </TableRow>
                            </TableHead>
                            <TableBody>
                              {note.orders.map((o) => (
                                <TableRow key={o.orderNo}>
                                  <TableCell>{o.orderNo}</TableCell>
                                  <TableCell>{o.trackingNo}</TableCell>
                                  <TableCell>{o.customerName}</TableCell>
                                  <TableCell>{o.deliveryAddress}</TableCell>
                                </TableRow>
                              ))}
                            </TableBody>
                          </Table>
                        </TableContainer>
                      ) : (
                        <Typography variant="body2" color="textSecondary">No orders found in this note.</Typography>
                      )}
                    </AccordionDetails>
                  </Accordion>
                ))}
              </Box>
            ) : (
              <Box sx={{ padding: "10px", borderRadius: "5px", backgroundColor: Colors.lightWarning, border: `1px solid ${Colors.warning}` }}>
                <Typography variant="body2">
                  <strong>Notice:</strong> This driver does not have a delivery note for today. A <strong>new Delivery Note</strong> will be created.
                </Typography>
              </Box>
            )}
          </Grid>
        </Grid>
      )}
    </ModalComponent>
  );
}

export default TransferDeliveryTaskModal;
