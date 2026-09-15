import {
  ActionButtonCustom,
  centerColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import { DataGrid } from "@mui/x-data-grid";
import { Box } from "@mui/material";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../../assets/styles/style";
import { useState } from "react";
import EditIcon from "@mui/icons-material/Edit";
import { GetServiceRateGroupById } from "../../../../api/AxiosInterceptors";
import UtilityClass from "../../../../utilities/UtilityClass";
import CreateServiceRateModal from "../../../../components/modals/ContractModals/CreateServiceRateModal";

const ServiceRateGroupList = (props) => {
  const { loading, isFilterOpen, allServiceRateGroup, getAllServiceRateGroup } =
    props;
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;

  const [serviceGroupData, setServiceGroupData] = useState({
    open: false,
    data: [],
    loading: {},
  });

  const handleEditServiceGroup = async (id) => {
    try {
      setServiceGroupData((prev) => ({
        ...prev,
        loading: {
          ...prev.loading,
          [id]: true,
        },
      }));
      const response = await GetServiceRateGroupById(id);
      if (response?.data?.isSuccess) {
        const result = response.data.result;
        const fromAddress = JSON.parse(result.entityFromAddress);
        const toAddress = JSON.parse(result.entityToAddress);
        fromAddress.countryId = Number(fromAddress.countryId);
        fromAddress.country = Number(fromAddress.countryId);
        toAddress.countryId = Number(toAddress.countryId);
        toAddress.country = Number(toAddress.countryId);

        result.entityFromAddress = fromAddress;
        result.entityToAddress = toAddress;
        setServiceGroupData((prev) => ({
          ...prev,
          open: true,
          data: result,
        }));
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (error) {
    } finally {
      setServiceGroupData((prev) => ({
        ...prev,
        loading: {
          ...prev.loading,
          [id]: false,
        },
      }));
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
      field: "additionalRate",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Additional Rate"}</Box>,
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box disableRipple>
            <>
              <Box>{row?.additionalRate}</Box>
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Action"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box>
            <ActionButtonCustom
              onClick={() => handleEditServiceGroup(row.serviceRateGroupId)}
              loading={serviceGroupData.loading[row.serviceRateGroupId]}
              sx={styleSheet.editProductButton}
              label={<EditIcon />}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];

  return (
    <>
      <Box
        sx={{
          ...styleSheet.allOrderTable,
          height: calculatedHeight,
        }}
      >
        <DataGrid
          loading={loading}
          rowHeight={40}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row.rowNum}
          rows={allServiceRateGroup || []}
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
        {serviceGroupData.open && (
          <CreateServiceRateModal
            open={serviceGroupData.open}
            onClose={() =>
              setServiceGroupData((prev) => ({
                ...prev,
                open: false,
                data: {},
              }))
            }
            rowdata={serviceGroupData.data}
            getAllServiceRateGroup={getAllServiceRateGroup}
          />
        )}
      </Box>
    </>
  );
};

export default ServiceRateGroupList;
