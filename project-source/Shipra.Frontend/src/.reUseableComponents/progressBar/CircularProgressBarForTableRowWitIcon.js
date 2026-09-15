import { CircularProgress, IconButton } from "@mui/material";
import React from "react";
import { styleSheet } from "../../assets/styles/style";
import Edit from '@mui/icons-material/Edit';

export default function CircularProgressBarForTableRowWitIcon({
  loading = false,
  onClick = () => {},
  color,
  icon,
}) {
  return (
    <IconButton onClick={onClick}>
      {loading ? (
        <CircularProgress sx={{ ...styleSheet.circularProgressForTableRow }} />
      ) : (
        <>
          {icon ? icon : <Edit sx={{ ...styleSheet.gridActionButtonIcon }} />}
        </>
      )}
    </IconButton>
  );
}
