import { Grid, InputLabel, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  AddUpdateShipperInvoiceAdjustment,
  GetAllSalePersons,
  GetAllTransactionType,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const AddUpdateShipperInvoiceAdjustmentModal = (props) => {
  let { open, onClose, getAllShipperInvoiceAdjustment, rowData } = props;
  const [loading, setLoading] = useState(false);
  const [employeeList, setEmployeeList] = useState([]);
  const [selectedEmployee, setSelectedEmployee] = useState();
  const [amount, setAmount] = useState(0);
  const [allTransactionType, setAllTransactionType] = useState([]);
  const [selectedTransactionType, setSelectedTransactionType] = useState();
  const [textareaValue, setTextareaValue] = useState("");

  const getAllSalePersons = async () => {
    try {
      const response = await GetAllSalePersons();
      if (response?.data?.isSuccess) {
        const employeeOptions = response?.data.result.map((emp) => ({
          ...emp,
          employeeId: emp?.employeeId?.value,
        }));
        setEmployeeList(employeeOptions);
      }
    } catch (e) {}
  };

  const getAllTransactionType = async () => {
    try {
      const response = await GetAllTransactionType();
      if (response?.data?.isSuccess) {
        setAllTransactionType(response?.data?.result);
      }
    } catch (e) {}
  };

  const hanldeCreateInvoiceAdjustment = async () => {
    if (
      selectedTransactionType.length ||
      !selectedTransactionType?.transactionTypeId
    ) {
      errorNotification("Please Select Transaction Type");
      return;
    }
    if (selectedEmployee.length || !selectedEmployee?.SaleChannelConfigId) {
      errorNotification("Please Select Employee");
      return;
    }
    if (!amount || Number(amount) <= 0) {
      errorNotification("Please Enter Amount");
      return;
    }

    setLoading(true);
    try {
      const body = {
        ShipperInvoiceAdjustmentId: rowData?.shipperInvoiceAdjustmentId,
        ShipperInvoiceId: rowData?.shipperInvoiceId,
        TransactionTypeId: selectedTransactionType?.transactionTypeId,
        Amount: amount,
        Comment: textareaValue,
        SaleChannelConfigId: selectedEmployee?.SaleChannelConfigId,
      };
      const response = await AddUpdateShipperInvoiceAdjustment(body);
      if (response?.data?.isSuccess) {
        getAllShipperInvoiceAdjustment();
        successNotification("Action perform Successfully");
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
    getAllSalePersons();
    getAllTransactionType();
  }, []);

  useEffect(() => {
    setAmount(rowData?.amount);
    setTextareaValue(rowData?.comment);
    const defaultTransationType = allTransactionType.filter(
      (dt) => dt.transactionTypeId === rowData?.transactionTypeId
    );
    setSelectedTransactionType(defaultTransationType);
    const defaultEmployee = employeeList.filter(
      (dt) => dt.SaleChannelConfigId === rowData?.saleChannelConfigId
    );
    setSelectedEmployee(defaultEmployee);
  }, [rowData, allTransactionType, employeeList]);

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Invoice Adjustment"}
      actionBtn={
        <ModalButtonComponent
          title={"Create & Update Invoice Adjustment"}
          loading={loading}
          bg={purple}
          onClick={hanldeCreateInvoiceAdjustment}
        />
      }
    >
      <Grid container spacing={1}>
        <Grid item xs={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {"Shipper"}
          </InputLabel>
          <SelectComponent
            name="employee"
            options={employeeList}
            value={selectedEmployee}
            optionLabel={EnumOptions.EMPLOYEE_SALE_PERSON.LABEL}
            optionValue={EnumOptions.EMPLOYEE_SALE_PERSON.VALUE}
            onChange={(e, val) => {
              setSelectedEmployee(val);
            }}
          />
        </Grid>
        <Grid item xs={6} sm={6} md={6}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {"Transcation type"}
          </InputLabel>
          <SelectComponent
            name="tType"
            options={allTransactionType}
            value={selectedTransactionType}
            optionLabel={EnumOptions.TRANSACTION_TYPE.LABEL}
            optionValue={EnumOptions.TRANSACTION_TYPE.VALUE}
            onChange={(e, val) => {
              setSelectedTransactionType(val);
            }}
          />
        </Grid>
        <Grid item xs={6} sm={6} md={6}>
          <InputLabel sx={styleSheet.inputLabel}>{"Amount"}</InputLabel>
          <TextField
            type="number"
            fullWidth
            value={amount}
            placeholder="0"
            onChange={(e) => {
              setAmount(e.target.value);
            }}
            sx={{ "& .MuiInputBase-root": { height: "37px" } }}
          />
        </Grid>
        <Grid item xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>{"Comment"}</InputLabel>
          <TextField
            placeholder="Enter comment"
            multiline
            rows={4}
            value={textareaValue}
            onChange={(e) => setTextareaValue(e.target.value)}
            sx={{ "& .MuiInputBase-root": { minHeight: "100px" } }} // use minHeight
            variant="outlined"
            fullWidth
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
};

export default AddUpdateShipperInvoiceAdjustmentModal;
