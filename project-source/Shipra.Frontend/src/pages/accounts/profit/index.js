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
import React, { useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { Route, Routes } from "react-router-dom";
import { styleSheet } from "../../../assets/styles/style";
import GeneralTabBar from "../../../components/shared/tabsBar";
import ProfitList from "./profitList";
function Profit(props) {
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const { register, reset, control } = useForm({
    defaultValues: { startDate: null, endDate: null },
  });

  useWatch({
    name: "startDate",
    control,
  });
  useWatch({
    name: "endDate",
    control,
  });
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px 60px 30px 30px" }}>
        {" "}
        <GeneralTabBar
          isFilterOpen={isFilterOpen}
          setIsFilterOpen={setIsFilterOpen}
          tabData={[
            {
              label:
                LanguageReducer?.languageType?.PRODUCT_ALL +
                " " +
                LanguageReducer?.languageType?.SHIPMENTS_TEXT,
              route: "/delivery-tasks",
            },
            {
              label: "Unassigned",
              route: "/delivery-tasks/unassigned",
            },
            {
              label: "Assigned",
              route: "/delivery-tasks/assigned",
            },
          ]}
          {...props}
          disableFilter
          disableSearch
          // width="auto"
        />
        {isFilterOpen ? (
          <Table
            sx={{ ...styleSheet.generalFilterArea }}
            size="small"
            aria-label="a dense table"
          >
            <TableHead>
              <TableRow>
                <Grid container spacing={2} sx={{ p: "6px" }}>
                  <Grid item xl={2} lg={3} md={4} sm={6} xs={12}>
                    <Stack alignItems="center" direction="row" spacing={1}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {"Start Date"}
                      </InputLabel>
                      <TextField
                        sx={{ minWidth: "150px" }}
                        type="date"
                        inputProps={{
                          style: {
                            padding: "2px 10px 2px 10px",
                            fontSize: "14px",
                          },
                        }}
                        size="small"
                        id="startDate"
                        name="startDate"
                        fullWidth
                        variant="outlined"
                        {...register("startDate")}
                      />
                    </Stack>
                  </Grid>
                  <Grid item xl={2} lg={3} md={4} sm={6} xs={12}>
                    <Stack alignItems="center" direction="row" spacing={1}>
                      <InputLabel
                        sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                      >
                        {"End Date"}
                      </InputLabel>
                      <TextField
                        sx={{ minWidth: "150px" }}
                        inputProps={{
                          style: {
                            padding: "2px 10px 2px 10px",
                            fontSize: "14px",
                          },
                        }}
                        type="date"
                        size="small"
                        id="endDate"
                        name="endDate"
                        fullWidth
                        variant="outlined"
                        {...register("endDate")}
                      />
                    </Stack>
                  </Grid>
                  <Grid item xl={2} lg={3} md={4} sm={6} xs={12}>
                    <Stack alignItems="right" direction="row" spacing={1}>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          reset();
                        }}
                      >
                        {LanguageReducer?.languageType?.CLEAR_FILTER_TEXT}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}
        <Routes>
          <Route path="/" element={<ProfitList />} />
          <Route path="/unassigned" element={<ProfitList />} />
          <Route path="/assigned" element={<ProfitList />} />
        </Routes>
      </div>
    </Box>
  );
}
export default Profit;
