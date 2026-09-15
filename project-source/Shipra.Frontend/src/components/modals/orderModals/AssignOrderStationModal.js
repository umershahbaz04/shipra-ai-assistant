import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CloseIcon from "@mui/icons-material/Close";
import {
  Box,
  Card,
  Chip,
  Grid,
  InputLabel,
  Paper,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { getAllStationLookupFunc } from "../../../apiCallingFunction";
import { AssignStationToOrders } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

export default function AssignOrderStationModal({
  open,
  handleClose,
  handleRefresh,
  selectedOrderIds,
  selectedOrderNos = [],
}) {
  const [chipData, setChipData] = useState([]);
  const [stations, setStations] = useState([]);
  const [selectedStation, setSelectedStation] = useState(null);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (open) {
      loadStations();
      setSelectedStation(null);
    }
  }, [open]);

  useEffect(() => {
    const cData = UtilityClass.getChipDataFromTrackingArr(selectedOrderNos);
    setChipData(cData);
  }, [selectedOrderNos]);

  const loadStations = async () => {
    try {
      let res = await getAllStationLookupFunc();
      if (res) {
        // Filter out "Select Please" option (productStationId = 0)
        const stationList = res.filter((s) => s.productStationId !== 0);
        setStations(stationList);
      }
    } catch (e) {
      console.log(e);
    }
  };

  const handleSubmitClick = async () => {
    if (!selectedStation || !selectedStation.productStationId) {
      errorNotification("Please select a station");
      return false;
    }
    if (!selectedOrderIds || selectedOrderIds.length === 0) {
      errorNotification("Please select at least one order");
      return false;
    }

    const payload = {
      stationId: selectedStation.productStationId,
      orderIds: selectedOrderIds,
    };

    setIsLoading(true);
    try {
      let res = await AssignStationToOrders(payload);
      setIsLoading(false);
      if (res.data && res.data.isSuccess) {
        successNotification(res.data.message || "Station assigned successfully");
        if (handleRefresh) handleRefresh();
        handleClose();
      } else {
        if (res.data?.errors) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          errorNotification(res.data?.message || "Failed to assign station");
        }
      }
    } catch (e) {
      setIsLoading(false);
      console.log(e);
      if (e?.response?.data?.errors) {
        UtilityClass.showErrorNotificationWithDictionary(e.response.data.errors);
      } else {
        errorNotification(e?.response?.data?.message || "Failed to assign station");
      }
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      title="Assign Station to Orders"
      actionBtn={
        <ModalButtonComponent
          title="Assign Station"
          loading={isLoading}
          bg={purple}
          onClick={handleSubmitClick}
        />
      }
    >
      <Grid container spacing={2}>
        <Grid item md={12} sm={12}>
          <Card variant="outlined" sx={styleSheet.tagsCard}>
            <Paper
              sx={{
                display: "flex !important",
                justifyContent: "flex-start !important",
                flexWrap: "wrap !important",
                p: 0.5,
                m: 0,
              }}
              elevation={0}
            >
              {chipData?.map((data) => {
                return (
                  <Box key={data.key} sx={{ mr: "10px", mb: "8px" }}>
                    <Chip
                      sx={styleSheet.tagsChipStyle}
                      size="small"
                      icon={
                        <CheckCircleIcon
                          fontSize="small"
                          sx={{ color: "white !important" }}
                        />
                      }
                      deleteIcon={
                        <CloseIcon sx={{ color: "white !important" }} />
                      }
                      label={data.label}
                    />
                  </Box>
                );
              })}
            </Paper>
          </Card>
        </Grid>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.ORDERS_STATIONS}
          </InputLabel>
          <SelectComponent
            name="station"
            options={stations}
            value={selectedStation}
            height={40}
            getOptionLabel={(option) => option.sname}
            optionLabel={EnumOptions.SELECT_STATION.LABEL}
            optionValue={EnumOptions.SELECT_STATION.VALUE}
            onChange={(e, newValue) => {
              const resolvedVal = newValue ? newValue : null;
              setSelectedStation(resolvedVal);
            }}
            size={"md"}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
}
