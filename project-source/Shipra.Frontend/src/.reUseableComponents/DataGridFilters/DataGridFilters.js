import React from "react";
import { styleSheet } from "../../assets/styles/style";
import { Box, Table, TableHead } from "@mui/material";
export default function FilterOfDataGrid(props) {
  const { children } = props;
  return (
    <Table
      sx={{ ...styleSheet.generalFilterArea }}
      size="small"
      aria-label="a dense table"
    >
      <TableHead>
        <Box p={1}>{children}</Box>
      </TableHead>
    </Table>
  );
}
