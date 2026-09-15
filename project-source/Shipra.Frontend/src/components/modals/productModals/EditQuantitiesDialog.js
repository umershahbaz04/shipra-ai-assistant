import React from "react";
import { Grid, Table, TableBody } from "@mui/material";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { EnumOptions } from "../../../utilities/enum";


export default function EditQuantitiesDialog({
  open,
  handleClose,
  LanguageReducer,
  handleAddQty,
  productStations,
  qtySelectedStation,
  setQtySelectedStation,
  getValueFromRow,
}) {
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={LanguageReducer?.languageType?.EDIT_QUNATITIES_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={"Add Quantity"}
          bg={purple}
          onClick={() => {
            handleAddQty();
            handleClose();
          }}
        />
      }
      style={{ overflow: "hidden", height: "unset" }}
    >
      <Grid container spacing={2} sx={{ mt: "5px" }}>
        <Grid item sm={12} md={12} lg={12}>
          <SelectComponent
            name="reason"
            options={productStations}
            defaulValue={qtySelectedStation}
            value={qtySelectedStation}
            optionLabel={EnumOptions.SELECT_STATION.LABEL}
            optionValue={EnumOptions.SELECT_STATION.VALUE}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setQtySelectedStation(resolvedId);
            }}
          />
        </Grid>
        <Grid item xs={12}>
          <Table
            sx={{
              "&.MuiTable-root .MuiCardHeader-root": {
                paddingLeft: "6px",
                paddingRight: "6px",
              },
            }}
            fullWidth
            aria-label="customized table"
          >
            <TableBody>{getValueFromRow()}</TableBody>
          </Table>
        </Grid>
      </Grid>
    </ModalComponent>
  );
}