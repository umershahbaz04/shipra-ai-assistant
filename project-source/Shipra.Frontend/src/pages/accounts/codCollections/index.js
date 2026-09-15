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
import CODCollectionList from "./codCollectionsList";

function CODCollections(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
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

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <GeneralTabBar
          isFilterOpen={isFilterOpen}
          setIsFilterOpen={setIsFilterOpen}
          tabData={[
            {
              label: "All",
              route: "/delivery-notes",
            },
            {
              label: "In Progress",
              route: "/delivery-notes/inprogress",
            },
            {
              label: "Completed",
              route: "/delivery-notes/completed",
            },
          ]}
          {...props}
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
                        {
                          LanguageReducer?.languageType
                            ?.ACCOUNTS_COD_PENDING_START_DATE
                        }
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
                        {
                          LanguageReducer?.languageType
                            ?.ACCOUNTS_COD_PENDING_END_DATE
                        }
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
                    sx={{ ...styleSheet.filterButtonMargin, display: "row" }}
                    <Stack alignItems="right" direction="row" spacing={1}>
                      <Button
                        sx={{ ...styleSheet.filterIcon, minWidth: "100px" }}
                        color="inherit"
                        variant="outlined"
                        onClick={() => {
                          reset();
                        }}
                      >
                        {LanguageReducer?.languageType?.ORDER_CLEAR_FILTER}
                      </Button>
                    </Stack>
                  </Grid>
                </Grid>
              </TableRow>
            </TableHead>
          </Table>
        ) : null}
        <CODCollectionList />
      </div>
    </Box>
  );
}
export default CODCollections;
