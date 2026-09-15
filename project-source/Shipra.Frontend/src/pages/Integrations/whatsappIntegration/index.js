import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
} from "@mui/material";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import { styleSheet } from "../../../assets/styles/style";

import { Route, Routes } from "react-router-dom";
import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader";
import { ActionButtonCustom } from "../../../utilities/helpers/Helpers";
import { useState } from "react";
import WhatsappIntegrationList from "./list";
import { GetAllWhatsappActivate } from "../../../api/AxiosInterceptors";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import AddWhatsAppModel from "../../../components/modals/integrationModals/AddWhatsAppModel";

const WhatsappIntegration = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [loading, setLoading] = useState(false);
  const [allWhatsappActivate, setAllWhatsappActivate] = useState([]);
  const [openWhatsAppModel, setOpenWhatsAppModel] = useState(false);
  const handleFilterRest = () => {
    setStartDate(null);
    setEndDate(null);
  };
  const getAllWhatsappActivate = async () => {
    setLoading(true);
    try {
      const response = await GetAllWhatsappActivate(startDate, endDate);
      if (response.data.isSuccess) {
        setAllWhatsappActivate(response.data.result.list);
      }
    } catch (e) {
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    getAllWhatsappActivate();
  }, []);
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <DataGridHeader
          tabs={true}
          tabData={[
            {
              label:
                LanguageReducer?.languageType?.INTEGRATION_SMS_INTEGRATION_ALL,
              route: "/whatsapp",
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.INTEGRATION_SMS_INTEGRATION_ACTIVE,
              route: "/whatsapp/active",
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.INTEGRATION_SMS_INTEGRATION_INACTIVE,
              route: "/whatsapp/in-active",
            },
          ]}
        >
          <Box className={"flex_center"} gap={1}>
            <ActionButtonCustom
              onClick={(event) => {
                setOpenWhatsAppModel(true);
              }}
              label={"Add Whatsapp Config"}
            />
          </Box>
        </DataGridHeader>
        {isFilterOpen ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <Stack direction={"column"} m={1}>
                <Stack direction={{ xs: "column", sm: "row" }} spacing={1}>
                  <Grid>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {LanguageReducer?.languageType?.START_DATE}
                    </InputLabel>

                    <CustomReactDatePickerInput
                      value={startDate}
                      onClick={(date) => setStartDate(date)}
                      size="small"
                      isClearable
                      inputProps={{ style: { padding: "4px 5px" } }}
                    />
                  </Grid>
                  <Grid>
                    <InputLabel
                      sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                    >
                      {LanguageReducer?.languageType?.END_DATE}
                    </InputLabel>
                    <CustomReactDatePickerInput
                      value={endDate}
                      onClick={(date) => setEndDate(date)}
                      width="180px"
                      size="small"
                      minDate={startDate}
                      disabled={!startDate ? true : false}
                      isClearable
                      inputProps={{ style: { padding: "4px 5px" } }}
                    />
                  </Grid>

                  <Grid>
                    <Stack mt={2} direction={"row"}>
                      <Button
                        sx={{
                          ...styleSheet.filterIcon,
                          minWidth: "100px",
                          marginLeft: "5px",
                        }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          handleFilterRest();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER}
                      </Button>

                      <Button
                        sx={{
                          ...styleSheet.filterIcon,
                          minWidth: "100px",
                          marginLeft: "5px",
                        }}
                        variant="contained"
                        onClick={() => {
                          getAllWhatsappActivate();
                        }}
                      >
                        {LanguageReducer?.languageType?.FILTER}
                      </Button>
                    </Stack>
                  </Grid>
                </Stack>
              </Stack>
            </TableHead>
          </Table>
        ) : null}
        {openWhatsAppModel && (
          <AddWhatsAppModel
            open={openWhatsAppModel}
            onClose={() => setOpenWhatsAppModel(false)}
            getAllWhatsappActivate={getAllWhatsappActivate}
          />
        )}
        <Routes>
          <Route
            path="/"
            element={
              <WhatsappIntegrationList
                rows={allWhatsappActivate}
                loading={loading}
                getAllWhatsappActivate={getAllWhatsappActivate}
              />
            }
          />
          <Route
            path="/active"
            element={
              <WhatsappIntegrationList
                rows={allWhatsappActivate.filter((item) => item.Active)}
                loading={loading}
                getAllWhatsappActivate={getAllWhatsappActivate}
              />
            }
          />
          <Route
            path="/in-active"
            element={
              <WhatsappIntegrationList
                rows={allWhatsappActivate.filter((item) => !item.Active)}
                loading={loading}
                getAllWhatsappActivate={getAllWhatsappActivate}
              />
            }
          />
        </Routes>
      </div>
    </Box>
  );
};

export default WhatsappIntegration;
