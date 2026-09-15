import { Box, CircularProgress } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import { GetWhatsappActivateById } from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UpdateWhatsAppModal from "../../../../components/modals/integrationModals/UpdateWhatsAppModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import {
  centerColumn,
  CodeBox,
  navbarHeight,
  useGetWindowHeight,
  usePagination,
} from "../../../../utilities/helpers/Helpers";
import { errorNotification } from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";

const WhatsappIntegrationList = (props) => {
  const { rows, loading, getAllWhatsappActivate } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { height: windowHeight } = useGetWindowHeight();
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const calculatedHeight = windowHeight - navbarHeight - 65;
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });

  const handleEditClick = (data) => {
    if (data) {
      setInfoModal((prev) => ({
        ...prev,
        loading: { [data.WhatsAppActivateId]: true },
      }));

      GetWhatsappActivateById(data.WhatsAppActivateId)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res?.data?.errors);
          } else {
            setInfoModal((prev) => ({
              ...prev,
              open: true,
              data: res?.data?.result,
            }));
          }
        })
        .catch((e) => {
          console.error("e", e);
          errorNotification("Something went wrong");
        })
        .finally(() => {
          setInfoModal((prev) => ({
            ...prev,
            loading: { [data.WhatsAppActivateId]: false },
          }));
        });
    }
  };

  const columns = [
    {
      field: "ServiceName",
      headerName: (
        <Box sx={{ fontWeight: "bold" }}>
          {
            LanguageReducer?.languageType
              ?.INTEGRATION_SMS_INTEGRATION_SERVICE_NAME
          }
        </Box>
      ),
      minWidth: 110,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            {infoModal.loading[row.WhatsAppActivateId] ? (
              <CircularProgress size={20} />
            ) : (
              <>
                <CodeBox
                  title={row.ServiceName}
                  onClick={() => handleEditClick(row)}
                />
              </>
            )}
          </>
        );
      },
    },
    {
      ...centerColumn,
      field: "CreatedOn",
      minWidth: 130,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {
            LanguageReducer?.languageType
              ?.INTEGRATION_SMS_INTEGRATION_CREATE_DATE
          }
        </Box>
      ),
      renderCell: (params) => {
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            {UtilityClass.convertUtcToLocalAndGetDate(params.row.CreatedOn)}
          </Box>
        );
      },
      flex: 1,
    },
    {
      ...centerColumn,
      field: "Status",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.INTEGRATION_SMS_INTEGRATION_STATUS}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        let isActive = params.row.Active;
        let title = isActive ? "Active" : "InActive";
        return (
          <Box
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"center"}
            sx={{ textAlign: "center" }}
            disableRipple
          >
            <>
              <StatusBadge
                title={title}
                color={isActive == false ? "#fff;" : "#fff;"}
                bgColor={isActive === false ? "#dc3545;" : "#28a745;"}
              />
            </>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "IsDefault",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.INTEGRATION_SMS_INTEGRATION_DEFAULT}
        </Box>
      ),
      minWidth: 90,
      flex: 1,
      renderCell: (params) => {
        let isDefault = params.row.IsDefault;
        let title = isDefault ? "yes" : "no";
        return (
          isDefault && (
            <Box
              display={"flex"}
              flexDirection={"column"}
              justifyContent={"center"}
              sx={{ textAlign: "center" }}
              disableRipple
            >
              <>
                <StatusBadge
                  title={title}
                  color={isDefault == false ? "#fff;" : "#fff;"}
                  bgColor={isDefault && "#28a745;"}
                />
              </>
            </Box>
          )
        );
      },
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
          getRowId={(row) => row?.WhatsAppActivateId}
          rows={rows ? rows : []}
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
      {infoModal.open && (
        <UpdateWhatsAppModal
          open={infoModal.open}
          onClose={() =>
            setInfoModal((prev) => ({
              ...prev,
              open: false,
            }))
          }
          UpdateData={infoModal.data}
          getAllWhatsappActivate={getAllWhatsappActivate}
        />
      )}
    </>
  );
};

export default WhatsappIntegrationList;
