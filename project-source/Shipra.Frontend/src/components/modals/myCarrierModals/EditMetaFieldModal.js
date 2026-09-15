import { Grid, InputLabel, TextField, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { UpdateMetaFieldForOrders } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";

function EditMetaFieldModal({
  open,
  setOpen,
  selectedDeliveryTasks, // orderNos
  allDeliveryTask,
  getAllDeliveryTask,
}) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [loading, setLoading] = useState(false);
  const [metaFields, setMetaFields] = useState([]);
  const [orderIds, setOrderIds] = useState([]);

  useEffect(() => {
    if (open && selectedDeliveryTasks && selectedDeliveryTasks.length > 0) {
      const selectedRows = allDeliveryTask?.list?.filter((item) =>
        selectedDeliveryTasks.includes(item.OrderNo)
      );

      const ids = selectedRows.map((item) => item.OrderId);
      setOrderIds(ids);

      const rowWithMeta = selectedRows.find(
        (item) => item.SettingConfigData && item.SettingConfigData.length > 0
      );

      if (rowWithMeta) {
        setMetaFields(JSON.parse(JSON.stringify(rowWithMeta.SettingConfigData)));
      } else {
        setMetaFields([]);
      }
    }
  }, [open, selectedDeliveryTasks, allDeliveryTask]);

  const handleClose = () => {
    setOpen(false);
  };

  const handleValueChange = (index, value) => {
    const updated = [...metaFields];
    updated[index].value = value;
    setMetaFields(updated);
  };

  const handleSave = async () => {
    setLoading(true);
    try {
      const payload = {
        OrderIds: orderIds,
        SettingConfig: JSON.stringify(metaFields),
      };
      const res = await UpdateMetaFieldForOrders(payload);
      if (res.data.isSuccess) {
        successNotification(res.data.message || "Updated successfully");
        setOpen(false);
        getAllDeliveryTask();
      } else {
        errorNotification(res.data.message || "Failed to update");
      }
    } catch (e) {
      console.error(e);
      errorNotification("An error occurred");
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      title={"Edit Additional Fields"}
      maxWidth="md"
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.SAVE || "SAVE"}
          onClick={handleSave}
          loading={loading}
          disabled={loading || metaFields.length === 0}
        />
      }
    >
      <Grid container spacing={2} sx={{ p: "15px" }}>
        {metaFields.length === 0 ? (
          <Grid item xs={12}>
            <Typography>
              No additional fields schema found in the selected rows.
            </Typography>
          </Grid>
        ) : (
          metaFields.map((field, index) => {
            let rawType = field.type || field.Type || "";
            if (typeof rawType === "object" && rawType !== null) {
              rawType = rawType.value || rawType.label || rawType.name || "";
            }
            const fieldType = String(rawType).toLowerCase();
            
            let rawOptions = field.options || field.Options || field.selectOptions || field.SelectOptions || [];
            if (typeof rawOptions === "string") {
              try { rawOptions = JSON.parse(rawOptions); } catch (e) { rawOptions = []; }
            }
            if (!Array.isArray(rawOptions)) rawOptions = [];
            
            const normalizedOptions = rawOptions.map(opt => {
              if (typeof opt === "string" || typeof opt === "number") {
                return { label: String(opt), value: String(opt) };
              }
              if (typeof opt === "object" && opt !== null) {
                return {
                  label: opt.label || opt.name || opt.text || opt.title || String(opt.value || ""),
                  value: opt.value !== undefined ? opt.value : opt.id !== undefined ? opt.id : opt.label || opt.name,
                  ...opt
                };
              }
              return opt;
            });

            const optValKey = normalizedOptions.length > 0 && normalizedOptions[0].id !== undefined ? "id" : "value";

            let currentValue = field.value || null;
            if (currentValue && typeof currentValue === "object" && currentValue[optValKey] === undefined) {
              currentValue = { ...currentValue, [optValKey]: currentValue.value || currentValue.id || currentValue.label };
            }

            return (
              <Grid item xs={12} md={12} key={index}>
                <InputLabel sx={{ ...styleSheet.inputLabel }}>
                  {field.name || field.Name}
                </InputLabel>
                {fieldType === "select" || fieldType === "dropdown" ? (
                  <SelectComponent
                    name={field.name || field.Name}
                    options={normalizedOptions}
                    value={currentValue}
                    optionLabel="label"
                    optionValue={optValKey}
                    onChange={(name, val) => {
                      // remove the extra 'value' field if it was added, to preserve original schema structure
                      if (val && typeof val === "object" && optValKey === "id" && val.value !== undefined) {
                        const { value, ...rest } = val;
                        handleValueChange(index, rest);
                      } else {
                        handleValueChange(index, val);
                      }
                    }}
                  />
              ) : (
                <TextField
                  type={fieldType === "number" ? "number" : fieldType === "date" ? "date" : "text"}
                  size="small"
                  fullWidth
                  value={
                    typeof field.value === "object" && field.value !== null
                      ? field.value.label || field.value.value || field.value.name || ""
                      : field.value || ""
                  }
                  onChange={(e) => handleValueChange(index, e.target.value)}
                />
              )}
            </Grid>
          );
        })
        )}
      </Grid>
    </ModalComponent>
  );
}

export default EditMetaFieldModal;
