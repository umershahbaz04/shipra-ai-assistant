import {
  centerColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import { DataGrid } from "@mui/x-data-grid";
import {
  Box,
  CircularProgress,
  Grid,
  TextField,
  Typography,
} from "@mui/material";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import { useEffect, useState } from "react";
import UtilityClass from "../../../../utilities/UtilityClass";
import { GetShipperRateBySaleChannelConfig } from "../../../../api/AxiosInterceptors";

const ShipperRatesList = (props) => {
  const { sideLoading, isFilterOpen, allShipperRateBySaleChannel } = props;
  const [loading, setLoading] = useState(false);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const [selectedId, setSelectedId] = useState(null);
  const [search, setSearch] = useState("");
  const [allShipperRate, setAllShipperRate] = useState([]);

  const filteredList = allShipperRateBySaleChannel.filter((e) =>
    e?.employeeName?.toLowerCase()?.includes(search.toLowerCase())
  );

  const handleSelect = (e) => {
    setSelectedId(e.saleChannelConfigId);
    getShipperRateBySaleChannelConfig(e.saleChannelConfigId);
  };

  const getShipperRateBySaleChannelConfig = async (employeeId) => {
    setLoading(true);
    try {
      const response = await GetShipperRateBySaleChannelConfig(employeeId);
      if (response?.data?.isSuccess) {
        setAllShipperRate(response?.data?.result);
      }
    } catch (e) {
    } finally {
      setLoading(false);
    }
  };

  const columns = [
    {
      field: "code",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Code"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box sx={{ fontWeight: "bold" }}>{row?.code}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "originTypeName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Origin Type Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.originTypeName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "fromName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"From"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.fromName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "toName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"To"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.toName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "serviceName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Service Name"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.serviceName}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "AdditionalRate",
      minWidth: 170,
      flex: 1,
      headerName: <Box sx={{ fontWeight: "600" }}>{"AdditionalRate"}</Box>,
      renderCell: ({ row }) => {
        return <Box>{(row?.additionalRate).toFixed(2)}</Box>;
      },
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 150,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Created On"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box>{UtilityClass.convertUtcToLocalAndGetDate(row?.createdOn)}</Box>
        );
      },
      flex: 1,
    },
  ];
  useEffect(() => {
    if (allShipperRateBySaleChannel.length) {
      setSelectedId(allShipperRateBySaleChannel[0].saleChannelConfigId);
      getShipperRateBySaleChannelConfig(
        allShipperRateBySaleChannel[0].saleChannelConfigId
      );
    }
  }, [allShipperRateBySaleChannel]);

  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;

  return (
    <>
      <Grid container spacing={1}>
        <Grid item md={4} sm={12} xs={12}>
          <Box
            sx={{
              border: "1px solid #ced4da",
              borderRight: "0px !important",
              backgroundColor: "rgb(247, 248, 248) !important",
              p: 1,
              height: calculatedHeight,
              overflowY: "auto",
              overflowX: "hidden",

              scrollbarWidth: "thin",
              scrollbarColor: "#b0b0b0 transparent",

              "&::-webkit-scrollbar": {
                width: "6px",
              },
              "&::-webkit-scrollbar-track": {
                background: "transparent",
              },
              "&::-webkit-scrollbar-thumb": {
                backgroundColor: "#b0b0b0",
                borderRadius: "8px",
              },
              "&::-webkit-scrollbar-thumb:hover": {
                backgroundColor: "#909090",
              },
            }}
          >
            <Typography variant="h5" fontWeight="600" mb={1}>
              Shipper List
            </Typography>
            <TextField
              fullWidth
              size="small"
              placeholder="Search Shipper"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              sx={{ mb: 2 }}
            />
            <Box>
              {sideLoading ? (
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    py: 3,
                  }}
                >
                  <CircularProgress />
                </Box>
              ) : (
                filteredList.map((e) => {
                  const isSelected = e.saleChannelConfigId === selectedId;

                  return (
                    <Box
                      key={e.saleChannelConfigId}
                      onClick={() => handleSelect(e)}
                      sx={{
                        px: 1.5,
                        py: 1,
                        cursor: "pointer",
                        borderLeft: isSelected
                          ? "5px solid var(--primary-color)"
                          : "5px solid transparent",
                        backgroundColor: isSelected
                          ? "rgba(86, 58, 213, 0.08)"
                          : "transparent",
                        color: isSelected ? "var(--primary-color)" : "#444",
                        fontWeight: isSelected ? 600 : 500,
                        fontSize: "13px",
                        transition: "0.2s",
                        "&:hover": {
                          backgroundColor: "#f5f7ff",
                        },
                      }}
                    >
                      {e.employeeName}
                    </Box>
                  );
                })
              )}
            </Box>
          </Box>
        </Grid>

        <Grid item md={8} sm={12} xs={12} px={"0px !important"}>
          <Box
            sx={{
              ...styleSheet.allOrderTable,
              height: calculatedHeight,
              "& .MuiDataGrid-root": {
                borderRadius: "0px 0px 8px 0px !important",
              },
            }}
          >
            <DataGrid
              loading={loading}
              rowHeight={40}
              headerHeight={40}
              sx={{
                fontFamily:
                  "'Lato Regular', 'Inter Regular', 'Arial' !important",
                fontSize: "12px",
                fontWeight: "500",
              }}
              getRowId={(row) => row.shipperRateId}
              rows={allShipperRate}
              columns={columns}
              disableSelectionOnClick
              pagination
              page={currentPage}
              pageSize={pageSize}
              rowsPerPageOptions={[5, 10, 15, 25]}
              paginationMode="client"
              onPageChange={handlePageChange}
              onPageSizeChange={handlePageSizeChange}
            />
          </Box>
        </Grid>
      </Grid>
    </>
  );
};

export default ShipperRatesList;
