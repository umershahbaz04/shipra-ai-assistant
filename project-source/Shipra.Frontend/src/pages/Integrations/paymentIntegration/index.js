import {
  Box,
  Button,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import React, { useEffect, useState, useRef } from "react";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import {
  GetAllPPActivate,
  GetAllPPLookupForSelection,
} from "../../../api/AxiosInterceptors";
import CustomReactDatePickerInput from "../../../.reUseableComponents/TextField/CustomReactDatePickerInput";

import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader";
import { ActionButtonCustom } from "../../../utilities/helpers/Helpers";

import PaymentIntegrationList from "./list";
import PaymentConfigModal from "../../../components/modals/integrationModals/PaymentConfigModal";
import { Route, Routes } from "react-router-dom";

const EnumOrderDashboardAction = {
  EXCELEXPORT: "ExcelExport",
};
function PaymentIntegrationPage(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isFilterReset, setIsFilterReset] = useState(false);
  const [allPPActivate, setAllPPActivate] = useState([]);
  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);
  const [isGridLoading, setIsGridLoading] = useState(false);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const [inputFields, setInputFields] = useState([]);
  const [openConnectModal, setOpenConnectModal] = useState(false);

  const getFiltersFromState = () => {
    let search = inputFields.join();
    let filters = {
      filterModel: {
        createdFrom: startDate,
        createdTo: endDate,
        start: 0,
        length: 10,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
    };

    return filters;
  };
  const getAllPPActivate = async () => {
    let params = getFiltersFromState();

    setIsGridLoading(true);
    await GetAllPPActivate(params)
      .then((res) => {
        const response = res.data;
        if (response.result) {
          setAllPPActivate(response.result);
        }
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setIsGridLoading(false);
      });
  };

  useEffect(() => {
    getAllPPActivate();
  }, []);

  useEffect(() => {
    if (isFilterReset) {
      getAllPPActivate();
    }
  }, [isFilterReset]);
  const handleFilterRest = () => {
    setStartDate(null);
    setEndDate(null);
  };
  const getAllActive = () => {
    let obj = {
      TotalCount: 0,
      list: [],
    };
    obj.list = allPPActivate?.list?.filter((item) => {
      return item?.Active;
    });
    obj.TotalCount = allPPActivate.TotalCount;
    return obj;
  };
  const getAllInActive = () => {
    let obj = {
      TotalCount: 0,
      list: [],
    };
    obj.list = allPPActivate?.list?.filter((item) => {
      return !item?.Active;
    });
    obj.TotalCount = allPPActivate.TotalCount;
    return obj;
  };
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <DataGridHeader
          tabs={true}
          tabData={[
            {
              label:
                LanguageReducer?.languageType
                  ?.INTEGRATION_PAYMENT_INTEGRATION_ALL,
              route: "/payment",
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.INTEGRATION_PAYMENT_INTEGRATION_ACTIVE,
              route: "/payment/active",
            },
            {
              label:
                LanguageReducer?.languageType
                  ?.INTEGRATION_PAYMENT_INTEGRATION_INACTIVE,
              route: "/payment/in-active",
            },
          ]}
        >
          <Box className={"flex_center"} gap={1}>
            <ActionButtonCustom
              onClick={(event) => {
                setOpenConnectModal(true);
              }}
              label={
                LanguageReducer?.languageType
                  ?.INTEGRATION_PAYMENT_INTEGRATION_ADD_PAYMENT_CONFIG
              }
            />
            {/* <ActionFilterButton
                onClick={() =>
                  !isFilterOpen ? setIsFilterOpen(true) : setIsFilterOpen(false)
                }
              /> */}
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
                          setIsFilterReset(true);
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
                          getAllPPActivate();
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
        <Routes>
          <Route
            path="/"
            element={
              <PaymentIntegrationList
                rows={allPPActivate}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                loading={isGridLoading}
                getAllPPActivate={getAllPPActivate}
              />
            }
          />
          <Route
            path="/active"
            element={
              <PaymentIntegrationList
                rows={getAllActive()}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                loading={isGridLoading}
                getAllPPActivate={getAllPPActivate}
              />
            }
          />
          <Route
            path="/in-active"
            element={
              <PaymentIntegrationList
                rows={getAllInActive()}
                getOrdersRef={getOrdersRef}
                resetRowRef={resetRowRef}
                loading={isGridLoading}
                getAllPPActivate={getAllPPActivate}
              />
            }
          />
        </Routes>
      </div>
      {openConnectModal && (
        <PaymentConfigModal
          open={openConnectModal}
          setOpen={setOpenConnectModal}
          getAll={getAllPPActivate}
        />
      )}
    </Box>
  );
}
export default PaymentIntegrationPage;
