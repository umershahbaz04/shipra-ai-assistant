import { useEffect, useState } from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  GetAllInvoiceStatus,
  UpdateShipperInvoiceStatus,
} from "../../../api/AxiosInterceptors";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import { Grid, InputLabel, TextField } from "@mui/material";
import { styleSheet } from "../../../assets/styles/style";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { EnumOptions } from "../../../utilities/enum";

const UpdateShipperInvoiceStatusModal = (props) => {
  let { open, onClose, rowData, getAllShipperInvoice } = props;
  const [loading, setLoading] = useState(false);
  const [allInoviceStatus, setAllInoviceStatus] = useState([]);
  const [selectedInvoiceStatus, setSelectedInvoiceStatus] = useState();
  const [refNo, setRefNo] = useState("");

  const getAllInvoiceStatus = async () => {
    const response = await GetAllInvoiceStatus();
    if (response?.data?.isSuccess) {
      setAllInoviceStatus(response?.data?.result);
    }
  };

  const hanldeUpdateShipperInvoiceStatus = async () => {
    if (!selectedInvoiceStatus || !selectedInvoiceStatus.invoiceStatusId) {
      errorNotification("Please Select Invoice Status");
      return;
    }
    setLoading(true);
    try {
      const body = {
        ShipperInvoiceId: rowData?.shipperInvoiceId,
        InvoiceStatusId: selectedInvoiceStatus?.invoiceStatusId,
        RefNo: refNo,
      };
      const response = await UpdateShipperInvoiceStatus(body);
      if (response?.data?.isSuccess) {
        getAllShipperInvoice();
        successNotification("Invoice Status Update Successfullu");
        onClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getAllInvoiceStatus();
  }, []);

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Shipper Rates"}
      actionBtn={
        <ModalButtonComponent
          title={"Update Shipper Invoice Status"}
          loading={loading}
          bg={purple}
          onClick={hanldeUpdateShipperInvoiceStatus}
        />
      }
    >
      <Grid container spacing={1}>
        <Grid item xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {"Invoice Status"}
          </InputLabel>
          <SelectComponent
            name="invoiceStatus"
            options={allInoviceStatus}
            value={selectedInvoiceStatus}
            optionLabel={EnumOptions.INVOICE_STATUS.LABEL}
            optionValue={EnumOptions.INVOICE_STATUS.VALUE}
            onChange={(e, val) => {
              setSelectedInvoiceStatus(val);
            }}
          />
        </Grid>
        <Grid item xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>{"Ref No"}</InputLabel>
          <TextField
            type="text"
            fullWidth
            value={refNo}
            placeholder="SH12321"
            onChange={(e) => {
              setRefNo(e.target.value);
            }}
            sx={{ "& .MuiInputBase-root": { height: "37px" } }}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
};

export default UpdateShipperInvoiceStatusModal;
