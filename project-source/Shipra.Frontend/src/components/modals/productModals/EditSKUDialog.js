import React from "react";
import { TableContainer, Table, TableBody, Paper, TextField } from "@mui/material";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";



import { TableCell, TableRow, styled } from "@mui/material";
import { tableCellClasses } from "@mui/material/TableCell";

const StyledTableCell = styled(TableCell)(({ theme }) => ({
  [`&.${tableCellClasses.head}`]: {
    backgroundColor: theme.palette.common.black,
    color: theme.palette.common.white,
  },
  [`&.${tableCellClasses.body}`]: {
    fontSize: 14,
  },
}));

const StyledTableRow = styled(TableRow)(({ theme }) => ({
  "&:nth-of-type(odd)": {
    backgroundColor: theme.palette.action.hover,
  },
  // hide last border
  "&:last-child td, &:last-child th": {
    border: 0,
  },
}));

import { placeholders, purple } from "../../../utilities/helpers/Helpers";

export default function EditSKUDialog({
  open,
  handleClose,
  LanguageReducer,
  handleUpdateSKU,
  updateQtyShowRecord,
  handleSkuChanged,
  handleFocus,
}) {
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title={LanguageReducer?.languageType?.EDIT_SKU_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.SUBMIT_TEXT}
          bg={purple}
          onClick={() => {
            handleUpdateSKU();
            handleClose();
          }}
        />
      }
      style={{ overflow: "hidden", height: "unset" }}
    >
      <TableContainer component={Paper}>
        <Table fullWidth aria-label="customized table">
          <TableBody>
            {updateQtyShowRecord.map((items, index) => (
              <StyledTableRow key={index}>
                <StyledTableCell align="center" component="th" scope="row">
                  {items?.variantOption}
                </StyledTableCell>
                <StyledTableCell align="right">
                  <TextField
                    placeholder={placeholders.sku}
                    onChange={(e) => handleSkuChanged(e, items)}
                    value={items?.SKU}
                    onFocus={handleFocus}
                    fullWidth
                    variant="outlined"
                    size="small"
                  />
                </StyledTableCell>
              </StyledTableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </ModalComponent>
  );
}