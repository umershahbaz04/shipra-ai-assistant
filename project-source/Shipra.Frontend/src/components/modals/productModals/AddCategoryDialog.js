import React from "react";
import { Box, InputLabel, TextField } from "@mui/material";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";

import { styleSheet } from "../../../assets/styles/style";
import { placeholders, purple } from "../../../utilities/helpers/Helpers";

export default function AddCategoryDialog({
  open,
  handleClose,
  LanguageReducer,
  createCategory,
  categoryName,
  setCategoryName,
  handleFocus,
}) {
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={LanguageReducer?.languageType?.ADD_CATEGORY_TEXT}
      actionBtn={
        <ModalButtonComponent
          title={LanguageReducer?.languageType?.ADD_CATEGORY_TEXT}
          bg={purple}
          onClick={createCategory}
        />
      }
      style={{ overflow: "hidden", height: "unset" }}
    >
      <Box>
        <InputLabel required sx={styleSheet.inputLabelAddProduct}>
          {LanguageReducer?.languageType?.CATEGORY_NAME_TEXT}
        </InputLabel>
        <TextField
          placeholder={placeholders.category_name}
          value={categoryName}
          onChange={(e) => setCategoryName(e.target.value)}
          fullWidth
          variant="outlined"
          onFocus={handleFocus}
          size="small"
        />
      </Box>
    </ModalComponent>
  );
}