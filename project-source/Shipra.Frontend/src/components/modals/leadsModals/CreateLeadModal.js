import { Box, Grid, InputLabel, TextField, FormHelperText } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import CountrySchema from "../../../utilities/helpers/countryschema";
import { CreateBulkLeads, GetAllCountry } from "../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { styleSheet } from "../../../assets/styles/style";
import { purple } from "@mui/material/colors";
import useOrderMobileDuplicateCheck from "../../../.reUseableComponents/CustomHooks/useMobileDuplicateCheck";

const CreateLeadModal = (props) => {
  const { open, onClose, onSuccess } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [loading, setLoading] = useState(false);

  const [phoneNumber, setPhoneNumber] = useState("");
  const [productName, setProductName] = useState("");
  const [countryId, setCountryId] = useState("");
  const [allCountries, setAllCountries] = useState([]);

  const duplicateWarning = useOrderMobileDuplicateCheck(phoneNumber);

  useEffect(() => {
    if (open) {
      setPhoneNumber("");
      setCountryId("");
      fetchCountries();
    }
  }, [open]);

  const fetchCountries = async () => {
    try {
      const response = await GetAllCountry();
      if (response?.data?.result) {
        setAllCountries(response.data.result);
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleSubmit = async () => {
    if (!phoneNumber) {
      errorNotification("Please enter Mobile No");
      return;
    }
    if (!productName) {
      errorNotification("Please enter Product Name");
      return;
    }
    if (!countryId) {
      errorNotification("Please select a Country");
      return;
    }

    setLoading(true);
    try {
      const payload = {
        Leads: [
          {
            PhoneNumber: phoneNumber,
            ProductName: productName,
            CountryId: countryId?.countryId || countryId?.id || countryId,
          },
        ],
      };

      const res = await CreateBulkLeads(payload);
      if (res?.data?.isSuccess) {
        successNotification("Lead created successfully");
        if (onSuccess) onSuccess();
        if (onClose) onClose();
      } else {
        const errorMsg = res?.data?.errors
          ? Object.values(res.data.errors).flat().join(", ")
          : "Failed to create lead";
        errorNotification(errorMsg);
      }
    } catch (e) {
      console.error(e);
      const errorMsg = e?.response?.data?.errors
        ? Object.values(e.response.data.errors).flat().join(", ")
        : "An error occurred";
      errorNotification(errorMsg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      setOpen={onClose}
      onClose={onClose}
      title="Create Lead"
      maxWidth="md"
      actionBtn={
        <ModalButtonComponent
          title="Create"
          bg={purple}
          onClick={handleSubmit}
          loading={loading}
        />
      }
    >
      <Box sx={{ p: 2 }}>
        <Grid container spacing={2}>
          <Grid item xs={12} sx={{ display: "none" }}>
            <InputLabel sx={{ ...styleSheet.inputLabel }}>Country *</InputLabel>
            <CountrySchema
              name="country"
              height={35}
              value={countryId}
              onChange={(e, val) => setCountryId(val)}
            />
          </Grid>
          <Grid item xs={12}>
            <InputLabel sx={{ ...styleSheet.inputLabel }}>
              Mobile No *
            </InputLabel>
            <TextField
              size="small"
              fullWidth
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              placeholder="Enter Mobile No"
              error={duplicateWarning.isDuplicate}
              sx={duplicateWarning.isDuplicate ? {
                "& .MuiOutlinedInput-root": {
                  backgroundColor: "rgba(255, 0, 0, 0.12) !important",
                }
              } : {}}
            />
            {duplicateWarning.isDuplicate && (
              <FormHelperText sx={{ color: "#d32f2f", marginLeft: "14px" }}>
                Warning: Duplicate Mobile! Match found in Order: {duplicateWarning.orderNo} ({duplicateWarning.daysAgo} days ago)
              </FormHelperText>
            )}
          </Grid>
          <Grid item xs={12}>
            <InputLabel sx={{ ...styleSheet.inputLabel }}>
              Product Name *
            </InputLabel>
            <TextField
              size="small"
              fullWidth
              value={productName}
              onChange={(e) => setProductName(e.target.value)}
              placeholder="Enter Product Name"
            />
          </Grid>
        </Grid>
      </Box>
    </ModalComponent>
  );
};

export default CreateLeadModal;
