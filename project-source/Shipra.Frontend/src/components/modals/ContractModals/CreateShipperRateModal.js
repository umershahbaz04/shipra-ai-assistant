import {
  Box,
  Grid,
  InputLabel,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import DataGridProComponent from "../../../.reUseableComponents/DataGrid/DataGridProComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  GetAllSalePersons,
  GetAllServiceRateGroupForSelection,
  GetAllShipperRateWithContract,
  UpsertShipperRatesWithSlabs,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import {
  EnumChangeFilterModelApiUrls,
  EnumOptions,
} from "../../../utilities/enum";
import { centerColumn } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const CarrierRateSlabsTable = ({
  rowNum,
  carrierRateSlabs,
  onShipperRateChange,
}) => {
  if (!carrierRateSlabs?.length) {
    return <Box sx={{ p: 2 }}>No rate slabs found</Box>;
  }

  return (
    <Box sx={{ p: 1, background: "#fff" }}>
      <TableContainer
        component={Paper}
        variant="outlined"
        sx={{
          borderColor: "#d0d5dd",
        }}
      >
        <Table
          size="small"
          sx={{
            "& .MuiTableCell-root": {
              padding: "4px 8px",
              fontSize: 12,
              borderColor: "#e4e7ec",
            },
          }}
        >
          <TableHead>
            <TableRow
              sx={{
                backgroundColor: "#f2f4f7",
                "& .MuiTableCell-root": {
                  fontSize: 12,
                  fontWeight: 600,
                  padding: "6px 8px",
                  color: "#344054",
                  borderColor: "#d0d5dd",
                },
              }}
            >
              <TableCell>Weight From</TableCell>
              <TableCell>Weight To</TableCell>
              <TableCell>Carrier Rate</TableCell>
              <TableCell>Shipper Rate</TableCell>
            </TableRow>
          </TableHead>

          <TableBody>
            {carrierRateSlabs.map((slab, slabIndex) => (
              <TableRow
                sx={{ height: 32 }}
                key={slab.ServiceRateGroupSlabId ?? slabIndex}
              >
                <TableCell>{slab.WeightFrom}</TableCell>
                <TableCell>{slab.WeightTo}</TableCell>
                <TableCell>{slab.CarrierRate}</TableCell>
                <TableCell>
                  <TextField
                    size="small"
                    type="number"
                    value={slab.ShipperRate ?? ""}
                    onChange={(e) =>
                      onShipperRateChange(
                        rowNum,
                        slabIndex,
                        Number(e.target.value),
                      )
                    }
                    sx={{
                      "& .MuiInputBase-input": {
                        padding: "4px 6px",
                        fontSize: 12,
                      },
                      width: 150,
                    }}
                  />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};

const CreateShipperRateModal = (props) => {
  let { open, onClose, getShipperRateBySaleChannelConfigGroup } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [shipperLoading, setShipperLoading] = useState();
  const [isLoading, setIsLoading] = useState(false);
  const [selectionModel, setSelectionModel] = useState([]);
  const [expandedRows, setExpandedRows] = useState([]);
  const [employeeList, setEmployeeList] = useState([]);
  const [allShipperRateWithContract, setAllShipperRateWithContract] = useState(
    [],
  );
  const [allServiceRateGroup, setAllServiceRateGroup] = useState([]);
  const [selectedEmployee, setSelectedEmployee] = useState();
  const [selectedServiceRateGroup, setSelectedServiceRateGroup] = useState();
  const [filterRowData, setfilterRowData] = useState([]);

  const getAllSalePersons = async () => {
    try {
      const response = await GetAllSalePersons();
      if (response?.data?.isSuccess) {
        const employeeOptions = response?.data.result.map((emp) => ({
          ...emp,
          employeeId: emp?.employeeId?.value,
        }));
        setEmployeeList(employeeOptions);
      }
    } catch (e) {}
  };

  const getAllServiceRateGroupForSelection = async () => {
    try {
      const response = await GetAllServiceRateGroupForSelection();
      if (response?.data?.isSuccess) {
        setAllServiceRateGroup(response?.data?.result);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {}
  };

  const getAllShipperRateWithContract = async (saleChannelConfigId) => {
    const body = {
      FilterModel: {
        createdFrom: null,
        createdTo: null,
        start: 0,
        length: 10000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
      SaleChannelConfigId: saleChannelConfigId || 0,
    };

    setShipperLoading(true);

    try {
      const response = await GetAllShipperRateWithContract(body);

      if (response?.data?.isSuccess) {
        const result = response?.data?.result;

        setAllShipperRateWithContract(result);

        if (saleChannelConfigId && result?.list?.length) {
          const matchedRowIds = result.list
            .filter((row) => row.saleChannelConfigId === saleChannelConfigId)
            .map((row) => row.rowNum);

          setSelectionModel(matchedRowIds);
        } else {
          setSelectionModel([]);
        }
      }
    } catch (e) {
    } finally {
      setShipperLoading(false);
    }
  };

  const renderDetailPanel = ({ row }) => {
    return (
      <CarrierRateSlabsTable
        rowNum={row.rowNum}
        carrierRateSlabs={row.carrierRateSlabs}
        onShipperRateChange={handleShipperRateChange}
      />
    );
  };

  const handleSelectedRow = (newSelection) => {
    const selectedRoutes = new Set();

    for (let rowNum of newSelection) {
      const row = allShipperRateWithContract.list.find(
        (r) => r.rowNum === rowNum,
      );
      if (!row) continue;

      const routeKey = `${row.from}-${row.to}`;

      if (selectedRoutes.has(routeKey)) {
        errorNotification(
          "This route (origin → destination) is already selected. Please deselect the duplicate first.",
        );
        return;
      }

      selectedRoutes.add(routeKey);
    }

    setSelectionModel(newSelection);
  };

  const handleShipperRateChange = (rowNum, slabIndex, value) => {
    setAllShipperRateWithContract((prev) => ({
      ...prev,
      list: prev.list.map((row) =>
        row.rowNum !== rowNum
          ? row
          : {
              ...row,
              carrierRateSlabs: row.carrierRateSlabs.map((slab, index) =>
                index === slabIndex ? { ...slab, ShipperRate: value } : slab,
              ),
            },
      ),
    }));
  };

  const handleRateChange = (rowNum, value) => {
    setAllShipperRateWithContract((prev) => ({
      ...prev,
      list: prev.list.map((row) =>
        row.rowNum === rowNum
          ? { ...row, shipperAddRate: value === "" ? null : Number(value) }
          : row,
      ),
    }));
  };

  const columns = [
    {
      field: "code",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Code"}</Box>,
      minWidth: 90,
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
      headerName: <Box sx={{ fontWeight: "bold" }}>{"OriginType Name"}</Box>,
      minWidth: 110,
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
      field: "Additionalrate",
      headerName: <Box sx={{ fontWeight: "bold" }}>Additional Rate</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box sx={{ padding: "5px" }}>
            <TextField
              size="small"
              type="number"
              value={row.shipperAddRate ?? ""}
              sx={{
                "& .MuiInputBase-root": { height: "30px", background: "#fff" },
              }}
              onChange={(e) => handleRateChange(row?.rowNum, e.target.value)}
            />
          </Box>
        );
      },
    },
  ];

  const handleEmoloyeeChnage = (e, val) => {
    setSelectedEmployee(val);
    if (!val?.saleChannelConfigId) {
      setSelectionModel([]);
      return;
    }
  };

  const hanldeCreateShipperRates = async () => {
    if (!selectedEmployee?.SaleChannelConfigId) {
      errorNotification("Please Select Employee");
      return;
    }

    const selectedRows = allShipperRateWithContract.list.filter((row) =>
      selectionModel.includes(row.rowNum),
    );

    if (selectedRows.length === 0) {
      errorNotification("Please select atleast 1 row ");
      return;
    }

    setIsLoading(true);

    try {
      const body = {
        items: selectedRows.map((row) => ({
          shipperRateId: row.shipperRateId || 0,
          saleChannelConfigId: selectedEmployee?.SaleChannelConfigId,
          serviceRateGroupId: row.serviceRateGroupId,
          additionalRate: row.shipperAddRate,
          from: row?.from,
          tO: row?.to,
          contractShipperRatesId: row.contractShipperRatesId,
          EmployeeName: selectedEmployee?.EmployeeName,

          slabs: row.carrierRateSlabs.map((slab) => ({
            shipperRateSlabId: slab.ShipperRateSlabId || 0,
            serviceRateGroupSlabId: slab.ServiceRateGroupSlabId,
            rate: slab.ShipperRate,
          })),
        })),
      };

      const response = await UpsertShipperRatesWithSlabs(body);
      if (response?.data?.isSuccess) {
        successNotification("Shipper rate slab create successfully");
        getAllShipperRateWithContract();
        getShipperRateBySaleChannelConfigGroup();
        onClose();
      }
    } catch (e) {
    } finally {
      setIsLoading(false);
    }
  };
  const getAllShipperRates = () => {};

  useEffect(() => {
    if (
      allShipperRateWithContract?.list?.length > 0 &&
      selectedServiceRateGroup &&
      selectedServiceRateGroup?.code !== 0
    ) {
      const filteredRows = allShipperRateWithContract.list.filter(
        (dt) => dt.code === selectedServiceRateGroup.code,
      );
      setfilterRowData(filteredRows);
    } else {
      setfilterRowData(allShipperRateWithContract?.list || []);
    }
  }, [selectedServiceRateGroup, allShipperRateWithContract]);

  useEffect(() => {
    getAllShipperRateWithContract();
    getAllSalePersons();
    getAllServiceRateGroupForSelection();
  }, []);

  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="md"
        title={"Shipper Rates"}
        actionBtn={
          <ModalButtonComponent
            title={"Shipper Rates"}
            loading={isLoading}
            bg={purple}
            onClick={hanldeCreateShipperRates}
          />
        }
      >
        <Box
          sx={{
            bgcolor: "#f5f5f5",
            border: "1px solid rgba(224, 224, 224, 1)",
            borderRadius: 1.25,
            borderBottomLeftRadius: "0px !important",
            borderBottomRightRadius: "0px !important",
            borderBottom: "0px !important",
            padding: 1.25,
          }}
        >
          <Grid container>
            <Grid item xs={12} md={5} p={1}>
              <InputLabel sx={{ ...styleSheet.inputLabel, overflow: "unset" }}>
                {"Shipper"}
              </InputLabel>
              <SelectComponent
                name="employee"
                options={employeeList}
                value={selectedEmployee}
                optionLabel={EnumOptions.EMPLOYEE_SALE_PERSON.LABEL}
                optionValue={EnumOptions.EMPLOYEE_SALE_PERSON.VALUE}
                onChange={(e, val) => {
                  getAllShipperRateWithContract(val?.SaleChannelConfigId);
                  handleEmoloyeeChnage(e, val);
                }}
              />
            </Grid>
            <Grid item xs={12} md={5} p={1}>
              <InputLabel sx={{ ...styleSheet.inputLabel, overflow: "unset" }}>
                {"Service Rate Group"}
              </InputLabel>
              <SelectComponent
                name="employee"
                options={allServiceRateGroup}
                value={selectedServiceRateGroup}
                optionLabel={"id"}
                optionValue={"code"}
                onChange={(e, val) => {
                  setSelectedServiceRateGroup(val);
                }}
              />
            </Grid>
          </Grid>
        </Box>
        <Box>
          <DataGridProComponent
            loading={shipperLoading}
            rows={filterRowData || []}
            columns={columns}
            getRowId={(row) => row.rowNum}
            enableMasterDetail
            getDetailPanelContent={renderDetailPanel}
            getDetailPanelHeight={() => "auto"}
            detailPanelExpandedRowIds={expandedRows}
            onDetailPanelExpandedRowIdsChange={setExpandedRows}
            selectionModel={selectionModel}
            onSelectionModelChange={handleSelectedRow}
            checkboxSelection
            rowsCount={allShipperRateWithContract?.TotalCount}
            paginationChangeMethod={getAllShipperRates}
            keepNonExistentRowsSelected={true}
            paginationMethodUrl={
              EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.url
            }
            defaultRowsPerPage={
              EnumChangeFilterModelApiUrls.GET_ALL_ORDERS.length
            }
            height={450}
          />
        </Box>
      </ModalComponent>
    </>
  );
};

export default CreateShipperRateModal;
