import { Box, Grid, InputLabel } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { GetAllSalePersonForSelection, AssignSalespersonToLead } from "../../../api/AxiosInterceptors";
import { errorNotification, successNotification } from "../../../utilities/toast";
import { styleSheet } from "../../../assets/styles/style";
import { purple } from "@mui/material/colors";

const AssignSalespersonModal = (props) => {
  const { open, onClose, selectedLeads, onSuccess } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [salespersons, setSalespersons] = useState([]);
  const [selectedSalesperson, setSelectedSalesperson] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (open) {
      fetchSalespersons();
      setSelectedSalesperson(null);
    }
  }, [open]);

  const fetchSalespersons = async () => {
    try {
      const response = await GetAllSalePersonForSelection();
      if (response?.data?.result) {
        setSalespersons(response.data.result);
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleAssign = async () => {
    if (!selectedSalesperson) {
      errorNotification("Please select a salesperson");
      return;
    }
    if (!selectedLeads || selectedLeads.length === 0) {
      errorNotification("No leads selected");
      return;
    }
    setLoading(true);
    try {
      const payload = {
        LeadIds: selectedLeads,
        SalespersonId: selectedSalesperson?.id || selectedSalesperson,
      };
      const response = await AssignSalespersonToLead(payload);
      if (response?.data?.isSuccess !== false) {
        successNotification("Salesperson assigned successfully");
        if (onSuccess) onSuccess();
        onClose();
      } else {
        errorNotification("Failed to assign salesperson");
      }
    } catch (e) {
      console.error(e);
      errorNotification("Failed to assign salesperson");
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      title={"Assign Salesperson"}
      maxWidth="sm"
      actionBtn={
        <ModalButtonComponent
          title={"Assign"}
          bg={purple[700]}
          type="button"
          onClick={handleAssign}
          loading={loading}
        />
      }
    >
      <Box sx={{ mt: 2 }}>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <InputLabel sx={{ ...styleSheet.inputLabel, mb: 0.5 }}>
              Select Salesperson ({selectedLeads?.length || 0} lead(s) selected)
            </InputLabel>
            <SelectComponent
              name="salesperson"
              options={salespersons}
              optionLabel={"text"}
              optionValue={"id"}
              value={selectedSalesperson}
              getOptionLabel={(option) => option?.text || ""}
              onChange={(e, val) => setSelectedSalesperson(val)}
            />
          </Grid>
        </Grid>
      </Box>
    </ModalComponent>
  );
};

export default AssignSalespersonModal;
