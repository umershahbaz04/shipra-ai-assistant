import React from "react";
import {
  Dialog,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
  Typography,
  Box,
  Grid,
  Divider,
  TableContainer,
  Table,
  TableBody,
  Paper,
  TextField,
  Slide,
} from "@mui/material";
import { TableCell, TableRow } from '@mui/material';
import { tableCellClasses } from '@mui/material/TableCell';
import { styled } from '@mui/material/styles';

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
  '&:nth-of-type(odd)': {
    backgroundColor: theme.palette.action.hover,
  },
  '&:last-child td, &:last-child th': {
    border: 0,
  },
}));
import { styleSheet } from "../../../assets/styles/style";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

export default function EditVariantAttributesDialog({
  open,
  handleClose,
  updateQtyShowRecord,
  handleAdvancedAttributeChanged,
  LanguageReducer,
}) {
  const handleFocus = (event) => event.target.select();

  return (
    <Dialog
      fullWidth
      open={open}
      TransitionComponent={Transition}
      keepMounted
      onClose={handleClose}
      maxWidth="md"
      sx={styleSheet?.modelMainClassWithImage || {}}
    >
      <DialogContent sx={{ ...styleSheet?.modelContentArea }}>
        <DialogContentText id="alert-dialog-slide-description">
          <Box sx={styleSheet?.editProductsHeadingAndUpload || {}}>
            <Typography sx={styleSheet?.editProductsHeading || {}} variant="h4">
              Edit Advanced Attributes
            </Typography>
          </Box>
          <Grid container spacing={2} sx={{ mt: "5px" }}>
            <Grid item sm={12} sx={{ mt: "10px" }}>
              <Divider />
            </Grid>
            <Grid item xs={12}>
              <TableContainer component={Paper}>
                <Table fullWidth aria-label="customized table">
                  <TableBody>
                    <StyledTableRow>
                      <StyledTableCell align="center">Variant</StyledTableCell>
                      <StyledTableCell align="center">Barcode</StyledTableCell>
                      <StyledTableCell align="center">Weight</StyledTableCell>
                      <StyledTableCell align="center">Length</StyledTableCell>
                      <StyledTableCell align="center">Width</StyledTableCell>
                      <StyledTableCell align="center">Height</StyledTableCell>
                    </StyledTableRow>
                    {updateQtyShowRecord?.map((item, index) => {
                      return (
                        <StyledTableRow key={index}>
                          <StyledTableCell align="center" component="th" scope="row">
                            {item?.variantOption || "-"}
                          </StyledTableCell>
                          <StyledTableCell align="right">
                            <TextField
                              placeholder="Barcode"
                              onChange={(e) => handleAdvancedAttributeChanged(e, item, 'barcode')}
                              value={item?.barcode || ""}
                              onFocus={handleFocus}
                              fullWidth
                              variant="outlined"
                              size="small"
                            />
                          </StyledTableCell>
                          <StyledTableCell align="right">
                            <TextField
                              placeholder="Weight"
                              type="number"
                              onChange={(e) => handleAdvancedAttributeChanged(e, item, 'weight')}
                              value={item?.weight || ""}
                              onFocus={handleFocus}
                              fullWidth
                              variant="outlined"
                              size="small"
                            />
                          </StyledTableCell>
                          <StyledTableCell align="right">
                            <TextField
                              placeholder="Length"
                              type="number"
                              onChange={(e) => handleAdvancedAttributeChanged(e, item, 'length')}
                              value={item?.length || ""}
                              onFocus={handleFocus}
                              fullWidth
                              variant="outlined"
                              size="small"
                            />
                          </StyledTableCell>
                          <StyledTableCell align="right">
                            <TextField
                              placeholder="Width"
                              type="number"
                              onChange={(e) => handleAdvancedAttributeChanged(e, item, 'width')}
                              value={item?.width || ""}
                              onFocus={handleFocus}
                              fullWidth
                              variant="outlined"
                              size="small"
                            />
                          </StyledTableCell>
                          <StyledTableCell align="right">
                            <TextField
                              placeholder="Height"
                              type="number"
                              onChange={(e) => handleAdvancedAttributeChanged(e, item, 'height')}
                              value={item?.height || ""}
                              onFocus={handleFocus}
                              fullWidth
                              variant="outlined"
                              size="small"
                            />
                          </StyledTableCell>
                        </StyledTableRow>
                      );
                    })}
                  </TableBody>
                </Table>
              </TableContainer>
            </Grid>
          </Grid>
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button
          fullWidth
          variant="contained"
          sx={styleSheet?.modalDismissButton || {}}
          onClick={handleClose}
        >
          {LanguageReducer?.languageType?.DISMISS_TEXT || "Dismiss"}
        </Button>
        <Button
          fullWidth
          variant="contained"
          sx={styleSheet?.modalCarrierSubmitButton || {}}
          onClick={handleClose}
        >
          {LanguageReducer?.languageType?.SUBMIT_TEXT || "Submit"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
