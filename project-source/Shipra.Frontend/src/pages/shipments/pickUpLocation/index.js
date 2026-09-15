import {
  Box,
  Button,
  ButtonGroup,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableHead,
  TableRow,
} from "@mui/material";
import { useEffect, useState } from "react";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import UtilityClass from "../../../utilities/UtilityClass";
import PickUpLocationList from "./list";
import { useSelector } from "react-redux";
import {
  GetActiveCarrierPickupLocationForSelection,
  GetAllActivedCarrier,
} from "../../../api/AxiosInterceptors";
import PickupLocationModal from "../../../components/modals/integrationModals/PickupLocationModal";
import { errorNotification } from "../../../utilities/toast";

const PickUpLocation = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [loading, setLoading] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [allActiveCarrier, setAllActiveCarrier] = useState([]);
  const [selectedActiveCarrier, setSeletedActiveCarrier] = useState();
  const [pickUpLocationData, setPickUpLocationData] = useState([]);
  const [openPickupLocationModal, setOpenPickupLocationModal] = useState(false);

  const getAllActivedCarrier = async () => {
    try {
      const response = await GetAllActivedCarrier({
        filterModel: {
          start: 0,
          length: 1000,
          search: "",
          sortDir: "desc",
          sortCol: 0,
        },
        countryId: 0,
        deliveryServiceId: 0,
      });
      if (response.data.isSuccess) {
        setAllActiveCarrier(response?.data?.result?.list);
      }
    } catch (e) {
    } finally {
    }
  };

  const getActiveCarrierPickupLocationForSelection = async () => {
    setLoading(true);
    try {
      const response = await GetActiveCarrierPickupLocationForSelection(
        selectedActiveCarrier?.ActiveCarrierId || 0,
        selectedActiveCarrier?.CarrierId || 0
      );
      if (response?.data?.isSuccess) {
        const data = response?.data?.result
          .map((item, index) => ({ ...item, index }))
          .slice(1);
        setPickUpLocationData(data);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  const handleFilter = () => {
    getActiveCarrierPickupLocationForSelection();
  };

  useEffect(() => {
    getAllActivedCarrier();
    getActiveCarrierPickupLocationForSelection();
  }, []);

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <Box sx={styleSheet.topNavBar}>
            <Stack
              sx={styleSheet.topNavBarRight}
              direction="row"
              justifyContent="flex-end"
              alignItems="center"
              spacing={1}
            >
              <ButtonGroup
                variant="outlined"
                aria-label="split button"
              ></ButtonGroup>
            </Stack>
          </Box>
          <DataGridTabs
            handleFilterBtnOnClick={() => {
              setIsFilterOpen(!isFilterOpen);
            }}
            tabsSmWidth="10px"
            tabsMdWidth="10px"
            otherBtns={
              <>
                <ButtonComponent
                  title={"Create Pickup Location"}
                  btnMdWidth={"140px"}
                  btnLgWidth={"165px"}
                  onClick={() => setOpenPickupLocationModal(true)}
                />
              </>
            }
            filterData={
              isFilterOpen ? (
                <Table
                  sx={{ ...styleSheet.generalFilterArea }}
                  size="small"
                  aria-label="a dense table"
                >
                  <TableHead>
                    <TableRow>
                      <Grid container spacing={2} sx={{ p: "15px 10px" }}>
                        <Grid item xl={3} lg={3} md={3} sm={6} xs={12}>
                          <InputLabel
                            sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                          >
                            {LanguageReducer?.languageType?.ORDER_CARRIER}
                          </InputLabel>
                          <SelectComponent
                            name="reason"
                            height={28}
                            options={allActiveCarrier}
                            value={selectedActiveCarrier}
                            optionLabel={EnumOptions.ACTIVE_CARRIER.LABEL}
                            optionValue={EnumOptions.ACTIVE_CARRIER.VALUE}
                            onChange={(e, val) => {
                              setSeletedActiveCarrier(val);
                            }}
                          />
                        </Grid>
                        <Grid item md={4} sm={6} xs={12} alignSelf="end">
                          <Stack
                            direction={"row"}
                            sx={{
                              ...styleSheet.filterButtonMargin,
                              display: "row",
                            }}
                            spacing={1}
                          >
                            <Button
                              sx={{
                                ...styleSheet.filterIcon,
                                minWidth: "100px",
                              }}
                              color="inherit"
                              variant="outlined"
                              onClick={() => {
                                handleFilter();
                              }}
                            >
                              {LanguageReducer?.languageType?.CLEAR_FILTER}
                            </Button>
                            <Button
                              sx={{
                                ...styleSheet.filterIcon,
                                minWidth: "100px",
                              }}
                              variant="contained"
                              onClick={() => {
                                handleFilter();
                              }}
                            >
                              {LanguageReducer?.languageType?.FILTER}
                            </Button>
                          </Stack>
                        </Grid>
                      </Grid>
                    </TableRow>
                  </TableHead>
                </Table>
              ) : null
            }
            tabData={[
              {
                label:
                  LanguageReducer?.languageType?.ORDERS_PRICE_CALCULATOR_ALL,
                route: "/pickup-location",
                children: (
                  <PickUpLocationList
                    loading={loading}
                    isFilterOpen={isFilterOpen}
                    pickUpLocationData={pickUpLocationData}
                    pickupLocationIds={selectedActiveCarrier}
                    allActiveCarrier={allActiveCarrier}
                    getActiveCarrierPickupLocation={
                      getActiveCarrierPickupLocationForSelection
                    }
                  />
                ),
              },
            ]}
          />
          {openPickupLocationModal && (
            <PickupLocationModal
              open={openPickupLocationModal}
              onClose={() => setOpenPickupLocationModal(false)}
              showTable={false}
              allActiveCarrier={allActiveCarrier}
              getActiveCarrierPickupLocation={
                getActiveCarrierPickupLocationForSelection
              }
            />
          )}
        </div>
      </Box>
    </>
  );
};

export default PickUpLocation;
