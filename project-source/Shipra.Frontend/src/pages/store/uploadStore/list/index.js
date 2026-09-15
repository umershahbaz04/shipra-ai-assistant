import { usePagination } from "@mui/lab";
import { Avatar, Box, Stack } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useSelector } from "react-redux";
import EditIcon from "@mui/icons-material/Edit";
import { styleSheet } from "../../../../assets/styles/style";
import {
  centerColumn,
  CodeBox,
  navbarHeight,
  useGetWindowHeight,
  ActionButtonCustom,
  AnchorBox,
  ClipboardIcon,
  DialerBox,
  MailtoBox,
  StyledTooltip,
} from "../../../../utilities/helpers/Helpers";
import { useState } from "react";
import UploadStoreModal from "../../../../components/modals/storeModals/UploadStoreModal";
import { errorNotification } from "../../../../utilities/toast";

const UploadStoreList = (props) => {
  const {
    loading,
    uploadStoreData,
    setUploadStoreData,
    selectionModel,
    setSelectionModel,
    allCountries,
  } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const calculatedHeight = windowHeight - navbarHeight - 194;
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const [editStore, setEditStore] = useState({
    rowData: [],
    open: false,
  });

  const handleEditUploadStore = (data) => {
    setEditStore((prev) => ({
      ...prev,
      rowData: data,
      open: true,
    }));
  };

  const handleSelectedRow = (selectedRowNums) => {
    if (!selectedRowNums?.length) {
      setSelectionModel([]);
      return;
    }

    const validRows = selectedRowNums.filter((rowNum) => {
      const row = uploadStoreData.find((r) => r.rowNum === rowNum);
      return row && !row.hasError;
    });

    const attemptedErrorRows = selectedRowNums.filter((rowNum) => {
      const row = uploadStoreData.find((r) => r.rowNum === rowNum);
      return row && row.hasError;
    });

    setSelectionModel(validRows);

    attemptedErrorRows.forEach((rowNum) => {
      const row = uploadStoreData.find((r) => r.rowNum === rowNum);
      errorNotification(`Row ${row.rowNum}: ${row.errorMsg}`);
    });
  };

  const columns = [
    {
      field: "Store Code",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_CODE}
        </Box>
      ),
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.storeCode} />
            <ClipboardIcon text={params.row.storeCode} />
          </>
        );
      },
    },
    {
      field: "name",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_NAME}
        </Box>
      ),
      minWidth: 130,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              <Avatar
                sx={{
                  width: 25,
                  height: 25,
                  fontSize: "13px",
                  color: "var(--primary-color)",
                  background: "rgba(86, 58, 213, 0.3)",
                }}
              >
                {params.row.storeName?.slice(0, 1)}
              </Avatar>
              <Box>{params.row.storeName || ""}</Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "storeCompany",
      minWidth: 120,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_COMPANY}
        </Box>
      ),
    },
    {
      field: "CustomerServiceNo",
      minWidth: 140,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.STORE_SERVICE_NO}
        </Box>
      ),
      renderCell: (params) => {
        return <DialerBox phone={params.row.customerServiceNo} />;
      },
    },
    {
      field: "email",
      minWidth: 160,
      flex: 1,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.STORE_EMAIL}
        </Box>
      ),
      sortable: false,
      renderCell: (params) => {
        return <MailtoBox email={params.row.email} />;
      },
    },
    {
      field: "URLs",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.STORE_URL}
        </Box>
      ),
      minWidth: 110,
      flex: 1,
      renderCell: (params) => {
        return <AnchorBox href={params.row.urls} />;
      },
    },
    {
      ...centerColumn,
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.ACTION}
        </Box>
      ),
      minWidth: 120,
      renderCell: ({ row }) => {
        return (
          <Box sx={{ position: "relative" }}>
            <ActionButtonCustom
              onClick={() => handleEditUploadStore(row)}
              sx={styleSheet.editProductButton}
              label={<EditIcon />}
            />
            {row?.hasError && (
              <StyledTooltip
                title={row?.errorMsg}
                sx={{ position: "absolute", bottom: -6 }}
              />
            )}
          </Box>
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
          getRowId={(row) => row.rowNum}
          rows={uploadStoreData || []}
          columns={columns}
          disableSelectionOnClick
          pagination
          page={currentPage}
          pageSize={pageSize}
          rowsPerPageOptions={[5, 10, 25]}
          paginationMode="client"
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          checkboxSelection={true}
          selectionModel={selectionModel}
          onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
          getRowClassName={({ row }) => (row?.hasError ? "active-row" : "")}
          height={calculatedHeight}
        />
      </Box>
      {editStore.open && (
        <UploadStoreModal
          open={editStore.open}
          onClose={() =>
            setEditStore((prev) => ({
              ...prev,
              open: false,
            }))
          }
          storeData={editStore.rowData}
          setUploadStoreData={setUploadStoreData}
          allCountries={allCountries}
        />
      )}
    </>
  );
};

export default UploadStoreList;
