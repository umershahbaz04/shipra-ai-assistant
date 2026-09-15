import React, { useEffect, useState, forwardRef, useImperativeHandle } from "react";
import { Box, Switch } from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent";
import { GetAllClientLeadStatus, ActiveDeactiveClientLeadStatus } from "../../../api/AxiosInterceptors";
import { fetchMethod, ActionButtonCustom, centerColumn } from "../../../utilities/helpers/Helpers";
import { errorNotification, successNotification } from "../../../utilities/toast";
import AddClientLeadStatusModal from "../../../components/modals/leadsModals/AddClientLeadStatusModal";
import { styleSheet } from "../../../assets/styles/style";

const ClientLeadStatusSetup = forwardRef((props, ref) => {
  const [statuses, setStatuses] = useState([]);
  const [loading, setLoading] = useState(false);
  const [openModal, setOpenModal] = useState(false);
  const [editingStatus, setEditingStatus] = useState(null);

  useImperativeHandle(ref, () => ({
    triggerAddNew() {
      handleAddNew();
    }
  }));

  const fetchStatuses = async () => {
    setLoading(true);
    try {
      const res = await fetchMethod(() => GetAllClientLeadStatus({
        FilterModel: { Start: 0, Length: 1000, Search: "", SortCol: 0, SortDir: "ASC" }
      }));
      if (Array.isArray(res?.response?.result)) {
        setStatuses(res.response.result);
      } else {
        setStatuses([]);
      }
    } catch (e) {
      console.error(e);
      errorNotification("Failed to fetch lead statuses");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchStatuses();
  }, []);

  const handleToggleActive = async (row) => {
    try {
      const res = await fetchMethod(() => ActiveDeactiveClientLeadStatus({
        ClientLeadStatusId: row.ClientLeadStatusId,
        IsActive: !row.Active
      }));
      if (res?.response?.isSuccess) {
        successNotification(`Status ${row.Active ? 'deactivated' : 'activated'} successfully`);
        fetchStatuses();
      } else {
        errorNotification("Error updating status");
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleEdit = (row) => {
    setEditingStatus(row);
    setOpenModal(true);
  };

  const handleAddNew = () => {
    setEditingStatus(null);
    setOpenModal(true);
  };

  const columns = [
    {
      field: "Description",
      headerName: <Box sx={{ fontWeight: "600" }}>Status Name</Box>,
      flex: 1,
      minWidth: 120,
    },
    {
      field: "DisplayOrder",
      headerName: <Box sx={{ fontWeight: "600" }}>Display Order</Box>,
      flex: 1,
      minWidth: 120,
      ...centerColumn,
    },
    {
      field: "Active",
      headerName: <Box sx={{ fontWeight: "600" }}>Active</Box>,
      flex: 1,
      minWidth: 120,
      ...centerColumn,
      renderCell: (params) => (
        <Switch
          checked={params.row.Active}
          onChange={() => handleToggleActive(params.row)}
          size="small"
        />
      ),
    },
    {
      field: "actions",
      headerName: <Box sx={{ fontWeight: "600" }}>Action</Box>,
      flex: 1,
      minWidth: 120,
      sortable: false,
      disableColumnMenu: true,
      ...centerColumn,
      renderCell: (params) => (
        <ActionButtonCustom
          onClick={() => handleEdit(params.row)}
          sx={styleSheet.editProductButton}
          label={<EditIcon />}
        />
      ),
    }
  ];

  return (
    <Box>
      <Box sx={{ height: 500, width: "100%" }}>
        <DataGridComponent
          rows={statuses}
          columns={columns}
          loading={loading}
          getRowId={(row) => row.ClientLeadStatusId}
          rowPerPage={10}
        />
      </Box>

      {openModal && (
        <AddClientLeadStatusModal
          open={openModal}
          setOpen={setOpenModal}
          editingStatus={editingStatus}
          onSuccess={fetchStatuses}
        />
      )}
    </Box>
  );
});

export default ClientLeadStatusSetup;
