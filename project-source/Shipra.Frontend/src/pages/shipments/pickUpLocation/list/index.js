import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { usePagination } from "@mui/lab";
import { Avatar, Box, Button, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState } from "react";
import { useSelector } from "react-redux";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  DeletepickLocationById,
  GetActiveCarrierPickupLocationbyid,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import PickupLocationModal from "../../../../components/modals/integrationModals/PickupLocationModal";
import {
  ActionButtonCustom,
  centerColumn,
  CodeBox,
  DataGridHeaderBox,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import { successNotification } from "../../../../utilities/toast";

const PickUpLocationList = (props) => {
  const {
    isFilterOpen,
    loading,
    pickUpLocationData,
    getActiveCarrierPickupLocation,
    pickupLocationIds,
    allActiveCarrier
  } = props;
  const { height: windowHeight } = useGetWindowHeight();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const [openDelete, setOpenDelete] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState(null);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [editLoading, setEditLoading] = useState({
    open: false,
    data: null,
    loading: {},
  });

  const handleDeletePickUpLocation = async () => {
    setDeleteLoading(true);
    try {
      const response = await DeletepickLocationById(deleteItemObject);
      if (response.data.isSuccess) {
        successNotification("PickUp Location Deleted Successfully");
        getActiveCarrierPickupLocation();
      }
    } catch (e) {
      console.error(e);
    } finally {
      setDeleteLoading(false);
      setOpenDelete(false);
    }
  };

  const handleEditPickUpLocation = async (id) => {
    setEditLoading((prev) => ({
      ...prev,
      open: false,
      loading: { ...prev.loading, [id]: true },
    }));

    try {
      const response = await GetActiveCarrierPickupLocationbyid(id);
      if (response.data.isSuccess) {
        setEditLoading((prev) => ({
          ...prev,
          open: true,
          data: response.data.result,
        }));
      }
    } catch (e) {
      console.error(e);
    } finally {
      setEditLoading((prev) => ({
        ...prev,
        loading: { ...prev.loading, [id]: false },
      }));
    }
  };

  const column = [
    {
      field: "locationName",
      headerName: <DataGridHeaderBox title={"Location Name"} />,
      minWidth: 300,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <CodeBox title={row.locationName} />
          </>
        );
      },
    },
    {
      field: "Carrier",
      headerName: <DataGridHeaderBox title={"Carrier"} />,
      minWidth: 300,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
              {row?.carrierName && (
                <Avatar
                  variant="rounded"
                  src={row?.carrierImage}
                  alt={row?.carrierName}
                  sx={{
                    width: 35,
                    height: 35,
                    bgcolor: "#fff",
                    borderRadius: "8px",
                    p: 0.5,
                    "& img": {
                      objectFit: "contain !important",
                    },
                  }}
                />
              )}
              <Typography variant="h6">{row?.carrierName}</Typography>
            </Box>
          </>
        );
      },
    },
    {
      field: "Address",
      headerName: <DataGridHeaderBox title={"PickUp Address"} />,
      minWidth: 300,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <>
            <CodeBox title={row.fullAddress} />
          </>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 80,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Action"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box display={"flex"} gap={1} width="100%" justifyContent={"center"}>
            <Button
              sx={styleSheet.deleteProductButton}
              variant="outlined"
              onClick={() => {
                setOpenDelete(true);
                setDeleteItemObject(row?.activeCarrierPickupLocationId);
              }}
              aria-label={`Delete lable ${row.LabelName}`}
            >
              <DeleteIcon />
            </Button>
            <ActionButtonCustom
              onClick={() =>
                handleEditPickUpLocation(row?.activeCarrierPickupLocationId)
              }
              loading={
                !!editLoading.loading[row?.activeCarrierPickupLocationId]
              }
              sx={styleSheet.editProductButton}
              label={<EditIcon />}
            />
          </Box>
        );
      },
      flex: 1,
    },
  ];

  const calculatedHeight = isFilterOpen
    ? windowHeight - 95 - 122
    : windowHeight - 95 - 36;

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
          rowHeight={50}
          headerHeight={40}
          sx={{
            fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
            fontSize: "12px",
            fontWeight: "500",
          }}
          getRowId={(row) => row.activeCarrierPickupLocationId}
          rows={pickUpLocationData || []}
          columns={column}
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
      {openDelete && (
        <DeleteConfirmationModal
          open={openDelete}
          setOpen={setOpenDelete}
          loading={deleteLoading}
          handleDelete={handleDeletePickUpLocation}
          heading={"Confirm Deletion of Pickup Location"}
          message={
            "The selected pickup location will be permanently deleted. This action cannot be undone."
          }
          buttonText={"yes"}
        />
      )}
      {editLoading.open && (
        <PickupLocationModal
          open={editLoading.open}
          onClose={() =>
            setEditLoading((prev) => ({
              ...prev,
              open: false,
              data: null,
            }))
          }
          showTable={false}
          allActiveCarrier={allActiveCarrier}
          getActiveCarrierPickupLocation={getActiveCarrierPickupLocation}
          pickupLocation={editLoading.data}
          pickupLocationIds={pickupLocationIds}
        />
      )}
    </>
  );
};

export default PickUpLocationList;
