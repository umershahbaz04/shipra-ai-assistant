import React, { useState, useEffect } from "react";
import {
  IconButton,
  Button,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Box,
  InputLabel
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { getAllStationLookupFunc } from "../../../apiCallingFunction";
import { getVariantsWithoutStation, assignStationToVariants } from "../../../api/AxiosInterceptors";
import UtilityClass from "../../../utilities/UtilityClass";
import { successNotification, errorNotification } from "../../../utilities/toast";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { styleSheet } from "../../../assets/styles/style";
import { purple } from "../../../utilities/helpers/Helpers";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";

export default function AssignStationModal({ open, handleClose, handleRefresh }) {
  const [stations, setStations] = useState([]);
  const [variants, setVariants] = useState([]);
  const [selectedStation, setSelectedStation] = useState(null);
  const [tableData, setTableData] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (open) {
      loadStations();
      loadVariants();
      setTableData([]);
      setSelectedStation(null);
    }
  }, [open]);

  const loadStations = async () => {
    try {
      let resData = await getAllStationLookupFunc();
      if (Array.isArray(resData)) {
        // filter out the "Select Please" option (productStationId = 0)
        const stationList = resData.filter(s => s.productStationId !== 0);
        setStations(stationList);
      }
    } catch (e) {
      console.log(e);
    }
  };

  const loadVariants = async () => {
    try {
      setIsLoading(true);
      let res = await getVariantsWithoutStation();
      let resData = res.data?.data || res.data?.result || res.data;
      if (Array.isArray(resData)) {
        setVariants(resData.map(v => ({
          productVariantId: v.ProductVariantId || v.productVariantId,
          sku: v.SKU || v.sku,
          productName: v.ProductName || v.productName,
          variantOptionText: v.VariantOptionText || v.variantOptionText,
          displayName: (v.SKU || v.sku) + " - " + (v.ProductName || v.productName) + ((v.VariantOptionText || v.variantOptionText) ? " (" + (v.VariantOptionText || v.variantOptionText) + ")" : "")
        })));
      }
    } catch (e) {
      console.log(e);
    } finally {
      setIsLoading(false);
    }
  };

  const handleVariantSelect = (name, val) => {
    if (!val) {
      setTableData([]);
      return;
    }

    const selectedList = (Array.isArray(val) ? val : [val]).filter(
      (item) => item && item.productVariantId !== 0
    );

    const updatedTableData = selectedList.map((item) => {
      const existing = tableData.find((d) => d.productVariantId === item.productVariantId);
      return {
        ...item,
        quantityAvailable: existing ? existing.quantityAvailable : 0,
        lowQuantityLimit: existing ? existing.lowQuantityLimit : 0,
      };
    });

    setTableData(updatedTableData);
  };

  const handleQtyChange = (variantId, value) => {
    const qty = parseInt(value, 10);
    setTableData(tableData.map(d =>
      d.productVariantId === variantId ? { ...d, quantityAvailable: isNaN(qty) ? 0 : qty } : d
    ));
  };

  const handleLowQtyChange = (variantId, value) => {
    const lowQty = parseInt(value, 10);
    setTableData(tableData.map(d =>
      d.productVariantId === variantId ? { ...d, lowQuantityLimit: isNaN(lowQty) ? 0 : lowQty } : d
    ));
  };

  const handleRemoveRow = (variantId) => {
    setTableData(tableData.filter(d => d.productVariantId !== variantId));
  };

  const handleSubmit = async () => {
    if (!selectedStation) {
      errorNotification("Please select a station");
      return;
    }
    if (tableData.length === 0) {
      errorNotification("Please select at least one variant");
      return;
    }

    const payload = {
      productStationId: selectedStation.productStationId,
      variants: tableData.map(row => ({
        productVariantId: row.productVariantId,
        sku: row.sku,
        quantity: row.quantityAvailable,
        lowQuantityLimit: row.lowQuantityLimit || 0
      }))
    };

    try {
      setIsLoading(true);
      let res = await assignStationToVariants(payload);
      if (res.data && res.data.isSuccess) {
        successNotification(res.data.result?.message || res.data.message || "Assigned successfully");
        if (handleRefresh) handleRefresh();
        handleClose();
      } else {
        errorNotification(res.data?.message || "Failed to assign");
      }
    } catch (e) {
      console.log(e);
      errorNotification("Failed to assign station");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title="Assign Station to Variants"
      actionBtn={
        <ModalButtonComponent
          title="Assign Station"
          loading={isLoading}
          bg={purple}
          onClick={handleSubmit}
        />
      }
    >
      <Grid container spacing={2} sx={{ mb: 3, mt: 1 }}>
        <Grid item xs={12} sm={6}>
          <InputLabel required sx={styleSheet.inputLabel}>
            Select Station
          </InputLabel>
          <SelectComponent
            name="stationId"
            size="md"
            options={stations}
            value={selectedStation}
            optionLabel="sname"
            optionValue="productStationId"
            onChange={(name, val) => setSelectedStation(val)}
          />
        </Grid>
        <Grid item xs={12} sm={6}>
          <InputLabel required sx={styleSheet.inputLabel}>
            Select Variants (Without Station)
          </InputLabel>
          <SelectComponent
            name="variantId"
            size="md"
            options={variants}
            value={tableData}
            multiple={true}
            optionLabel="displayName"
            optionValue="productVariantId"
            onChange={handleVariantSelect}
          />
        </Grid>
      </Grid>

      {tableData.length > 0 && (
        <TableContainer component={Paper} sx={{ mt: 2, boxShadow: 'none', border: '1px solid #e0e0e0' }}>
          <Table size="small">
            <TableHead sx={{ backgroundColor: '#f5f5f5' }}>
              <TableRow>
                <TableCell>SKU</TableCell>
                <TableCell>Product Name</TableCell>
                <TableCell>Variant</TableCell>
                <TableCell width="120">Quantity</TableCell>
                <TableCell width="120">Low Quantity</TableCell>
                <TableCell width="60">Action</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {tableData.map((row) => (
                <TableRow key={row.productVariantId}>
                  <TableCell>{row.sku}</TableCell>
                  <TableCell>{row.productName}</TableCell>
                  <TableCell>{row.variantOptionText || '-'}</TableCell>
                  <TableCell>
                    <TextFieldComponent
                      type="number"
                      size="small"
                      value={row.quantityAvailable}
                      onChange={(e) => handleQtyChange(row.productVariantId, e.target.value)}
                    />
                  </TableCell>
                  <TableCell>
                    <TextFieldComponent
                      type="number"
                      size="small"
                      value={row.lowQuantityLimit ?? 0}
                      onChange={(e) => handleLowQtyChange(row.productVariantId, e.target.value)}
                    />
                  </TableCell>
                  <TableCell>
                    <IconButton size="small" color="error" onClick={() => handleRemoveRow(row.productVariantId)}>
                      <CloseIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </ModalComponent>
  );
}
