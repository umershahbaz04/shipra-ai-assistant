import {
  Box,
  CircularProgress,
  IconButton,
  Menu,
  Stack,
  Tooltip,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import { DataGridPro, useGridApiRef } from "@mui/x-data-grid-pro";
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import StatusBadge from "../../../components/shared/statudBadge";
import UpdateOrderAddressLatLangModal from "../../../components/modals/myCarrierModals/UpdateOrderAddressLatLangModal";
import ContactOrdersModal from "../../../components/modals/leadsModals/ContactOrdersModal";
import EditLeadModal from "../../../components/modals/leadsModals/EditLeadModal";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  EnumLeadStatus,
  EnumTableName,
} from "../../../utilities/enum";
import Colors, { Danger, Success, Warning } from "../../../utilities/helpers/Colors";
import {
  DialerBox,
  MapButton,
  centerColumn,
  navbarHeight,
  useClientSubscriptionReducer,
  useGetWindowHeight,
  usePagination,
  getElementByInnerHTML,
  ActionButtonCustom,
  DescriptionBoxWithChild,
  CodeBox,
} from "../../../utilities/helpers/Helpers";
import {
  useGetAllEmployeeColumnConfiguration,
  useSaveColumnConfig,
} from "../../../utilities/helpers/HelpersFilter";
import useOpenStreetmapGetLatLng from "../../../.reUseableComponents/CustomHooks/useOpenStreetmapGetLatLng";

function LeadsList(props) {
  const {
    allLead,
    getOrdersRef,
    loading,
    resetRowRef,
    setSelectedLeads,
    getAllLead,
    isFilterOpen,
    setOpenUpdateStatusModal,
    setSelectedLeadForStatus,
  } = props;

  const navigate = useNavigate();
  const apiRef = useGridApiRef();
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [columnVisibilityModel, setColumnVisibilityModel] = useState({});
  const [flag, setFlag] = useState(false);
  const [showContent, setShowContent] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const [openContactModal, setOpenContactModal] = useState(false);
  const [selectedContactMobile, setSelectedContactMobile] = useState(null);
  const [loadingContact, setLoadingContact] = useState({});

  const handleContactClick = (e, row) => {
    e.stopPropagation();
    const leadId = row.LeadId || row.PhoneNumber;
    setLoadingContact((prev) => ({ ...prev, [leadId]: true }));
    setTimeout(() => {
      setSelectedContactMobile(row.PhoneNumber);
      setOpenContactModal(true);
      setLoadingContact((prev) => ({ ...prev, [leadId]: false }));
    }, 500);
  };

  const [openEditModal, setOpenEditModal] = useState(false);
  const [selectedLeadForEdit, setSelectedLeadForEdit] = useState(null);
  const [loadingEdit, setLoadingEdit] = useState({});

  const handleEditLeadClick = (e, row) => {
    e.stopPropagation();
    const leadId = row.LeadId || row.leadId || row.PhoneNumber + row.ProductName;
    setLoadingEdit((prev) => ({ ...prev, [leadId]: true }));
    setTimeout(() => {
      setSelectedLeadForEdit(row);
      setOpenEditModal(true);
      setLoadingEdit((prev) => ({ ...prev, [leadId]: false }));
    }, 500);
  };

  const [slectedAddress, setSelectedAddress] = useState({
    open: false,
    isLoading: {},
    coordinates: null,
    selectedItem: null,
  });
  const handleCloseUpdateMap = () => {
    setSelectedAddress({
      open: false,
      isLoading: {},
      coordinates: null,
      selectedItem: null,
    });
  };

  const { getAddressCoordinates, isLoadingCoordinates } =
    useOpenStreetmapGetLatLng();

  const handleOpenModal = (row) => {
    const getCordinates = async () => {
      if (row) {
        try {
          const coordinates = await getAddressCoordinates(row.Address || row.CityName);
          setSelectedAddress({
            open: true,
            isLoading: isLoadingCoordinates,
            coordinates: coordinates
              ? {
                lat: parseFloat(coordinates?.lat)
                  ? parseFloat(coordinates?.lat)?.toFixed(5)
                  : 0,
                lng: parseFloat(coordinates?.lng)
                  ? parseFloat(coordinates?.lng)?.toFixed(5)
                  : 0,
              }
              : null,
            selectedItem: row,
          });
        } catch (error) {
          console.error("Error fetching coordinates:", error);
        }
      }
    };
    getCordinates();
  };

  const columns = [
    {
      field: "PhoneNumber",
      headerName: <Box sx={{ fontWeight: "600" }}>Phone Number</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        const leadId = params.row.LeadId || params.row.PhoneNumber;
        const isLoading = loadingContact[leadId];
        return (
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            {isLoading ? (
              <CircularProgress size={14} thickness={5} sx={{ color: Colors.linkColor }} />
            ) : (
              <CodeBox
                title={params.row.PhoneNumber}
                onClick={(e) => handleContactClick(e, params.row)}
                sx={{
                  textDecoration: "underline",
                  cursor: "pointer",
                }}
              />
            )}
          </Box>
        );
      },
    },
    {
      field: "ProductName",
      headerName: <Box sx={{ fontWeight: "600" }}>Product Name</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.ProductName}</span>
      ),
    },
    {
      field: "CountryName",
      headerName: <Box sx={{ fontWeight: "600" }}>Country</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: (params) => (
        <span style={{ fontWeight: 500, fontSize: "12px", color: "#333" }}>{params.row.CountryName || "-"}</span>
      ),
    },
    {
      field: "DraftOrderNo",
      headerName: <Box sx={{ fontWeight: "600" }}>Draft Order #</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: (params) => (
        <span style={{ fontWeight: 500, fontSize: "12px", color: "#333" }}>{params.row.DraftOrderNo || "-"}</span>
      ),
    },
    {
      field: "OrderNo",
      headerName: <Box sx={{ fontWeight: "600" }}>Order #</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: (params) => (
        <span style={{ fontWeight: 500, fontSize: "12px", color: "#333" }}>{params.row.OrderNo || "-"}</span>
      ),
    },
    {
      field: "CustomerName",
      headerName: <Box sx={{ fontWeight: "600" }}>Customer</Box>,
      minWidth: 120,
      flex: 1,
      renderCell: (params) => (
        <span style={{ fontWeight: 600, fontSize: "12px", color: "#333" }}>{params.row.CustomerName || "-"}</span>
      ),
    },
    {
      field: "GoogleLocationLink",
      headerName: <Box sx={{ fontWeight: "600" }}>Location</Box>,
      minWidth: 220,
      flex: 1.5,
      renderCell: (params) => (
        <Box sx={{ display: "flex", flexDirection: "column", justifyContent: "center", py: 1, width: "100%" }}>
          {params.row.CustomerFullAddress ? (
            <Tooltip title={params.row.CustomerFullAddress} placement="top" arrow>
              {params.row.CustomerFullAddress}
            </Tooltip>
          ) : (
            <span style={{ fontSize: "11px", color: "#999", fontStyle: "italic", marginBottom: params.row.GoogleLocationLink ? "4px" : "0" }}>No address</span>
          )}
          {params.row.GoogleLocationLink && (
            <a
              href={params.row.GoogleLocationLink}
              target="_blank"
              rel="noreferrer"
              style={{ color: Colors.linkColor, fontSize: "11px", textDecoration: "none", display: "inline-flex", alignItems: "center", gap: "3px" }}
            >
              <span style={{ fontSize: "12px" }}>📍</span> View Map
            </a>
          )}
        </Box>
      ),
    },
    {
      field: "SalespersonName",
      headerName: <Box sx={{ fontWeight: "600" }}>Salesperson</Box>,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => (
        <span style={styleSheet.tableText}>{params.row.SalespersonName || "-"}</span>
      ),
    },
    {
      field: "LeadStatus",
      headerName: <Box sx={{ fontWeight: "600" }}>Lead Status</Box>,
      minWidth: 130,
      flex: 1,
      ...centerColumn,
      renderCell: (params) => {
        const statusColors = [
          { bgClr: "rgba(0, 163, 218, 0.1)", textClr: "#007ea8", dotClr: "#00A3DA" }, // Light Blue
          { bgClr: "rgba(76, 175, 80, 0.1)", textClr: "#2e7d32", dotClr: "#4caf50" }, // Green
          { bgClr: "rgba(255, 152, 0, 0.1)", textClr: "#e65100", dotClr: "#ff9800" }, // Orange
          { bgClr: "rgba(156, 39, 176, 0.1)", textClr: "#7b1fa2", dotClr: "#9c27b0" }, // Purple
          { bgClr: "rgba(244, 67, 54, 0.1)", textClr: "#c62828", dotClr: "#f44336" }, // Red
          { bgClr: "rgba(63, 81, 181, 0.1)", textClr: "#303f9f", dotClr: "#3f51b5" }, // Indigo
          { bgClr: "rgba(233, 30, 99, 0.1)", textClr: "#c2185b", dotClr: "#e91e63" }, // Pink
          { bgClr: "rgba(0, 150, 136, 0.1)", textClr: "#00796b", dotClr: "#009688" }, // Teal
        ];
        
        let { bgClr, textClr, dotClr } = statusColors[(params.row.LeadStatusId || 0) % statusColors.length];
        
        const statusName = (params.row.LeadStatus || "").toLowerCase();
        if (statusName.includes("open")) {
          bgClr = "rgba(0, 163, 218, 0.1)"; textClr = "#007ea8"; dotClr = "#00A3DA";
        } else if (statusName.includes("progress")) {
          bgClr = "rgba(255, 152, 0, 0.1)"; textClr = "#e65100"; dotClr = "#ff9800";
        } else if (statusName.includes("call")) {
          bgClr = "rgba(156, 39, 176, 0.1)"; textClr = "#7b1fa2"; dotClr = "#9c27b0";
        } else if (statusName.includes("complete")) {
          bgClr = "rgba(76, 175, 80, 0.1)"; textClr = "#2e7d32"; dotClr = "#4caf50";
        } else if (statusName.includes("answer") || statusName.includes("need") || statusName.includes("cancel")) {
          bgClr = "rgba(244, 67, 54, 0.1)"; textClr = "#c62828"; dotClr = "#f44336";
        }


        return (
          <Box sx={{ display: "inline-flex", alignItems: "center", gap: 1 }}>
            <Box
              sx={{
                display: "inline-flex",
                width: "fit-content",
                alignItems: "center",
                justifyContent: "center",
                gap: "6px",
                background: bgClr,
                color: textClr,
                padding: "4px 12px",
                borderRadius: "20px",
                fontSize: "12px",
                fontWeight: "600",
                letterSpacing: "0.3px",
                boxShadow: "0 1px 2px rgba(0,0,0,0.02)",
              }}
            >
              <span
                style={{
                  width: "6px",
                  height: "6px",
                  borderRadius: "50%",
                  backgroundColor: dotClr,
                  display: "inline-block",
                }}
              ></span>
              {params.row.LeadStatus}
            </Box>
            {params.row.LeadStatus?.toLowerCase() !== "completed" && (
              <IconButton 
                size="small" 
                onClick={(e) => {
                  e.stopPropagation();
                  setSelectedLeadForStatus({
                    leadId: params.row.LeadId,
                    leadStatusId: params.row.LeadStatusId
                  });
                  setOpenUpdateStatusModal(true);
                }}
              >
                <EditIcon sx={{ fontSize: "16px", color: Colors.primary }} />
              </IconButton>
            )}
          </Box>
        );
      },
    },
    {
      field: "CreatedOn",
      headerName: <Box sx={{ fontWeight: "600" }}>Created On</Box>,
      minWidth: 130,
      flex: 1,
      ...centerColumn,
      renderCell: (params) => (
        <Box>{UtilityClass.convertUtcToLocalAndGetDate(params.row.CreatedOn)}</Box>
      ),
    },
    {
      field: "actions",
      headerName: <Box sx={{ fontWeight: "600" }}>Action</Box>,
      minWidth: 80,
      flex: 0,
      sortable: false,
      disableColumnMenu: true,
      ...centerColumn,
      renderCell: (params) => {
        if (params.row.LeadStatusId === EnumLeadStatus.Completed) {
          return null;
        }
        return (
          <ActionButtonCustom
            onClick={(e) => handleEditLeadClick(e, params.row)}
            loading={loadingEdit[params.row.LeadId || params.row.leadId || params.row.PhoneNumber + params.row.ProductName]}
            sx={styleSheet.editProductButton}
            label={<EditIcon />}
          />
        );
      },
    },
  ];

  const [selectionModel, setSelectionModel] = useState([]);
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 10);

  const handleSelectedRow = (ids) => {
    setSelectionModel(ids);
    if (getOrdersRef) getOrdersRef.current = ids;
    if (setSelectedLeads) setSelectedLeads(ids);
  };

  const getAllEmployeeColumnConfiguration = useGetAllEmployeeColumnConfiguration(
    setColumnVisibilityModel,
    EnumTableName.DELIVERY_TASK_TABLE
  );

  const handleColumnVisibilityModelChange = useSaveColumnConfig(
    setColumnVisibilityModel,
    EnumTableName.DELIVERY_TASK_TABLE
  );

  useEffect(() => {
    if (resetRowRef && resetRowRef.current) {
      if (getOrdersRef) getOrdersRef.current = [];
      resetRowRef.current = false;
      setSelectionModel([]);
    }
  }, [resetRowRef?.current]);

  useEffect(() => {
    setFlag((prev) => !prev);
  }, [loading]);

  useEffect(() => {
    const selectedElement = getElementByInnerHTML("MUI X Missing license key");
    if (selectedElement) {
      setShowContent(true);
      selectedElement.style.display = "none";
    }
  }, [flag]);

  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 222
    : windowHeight - navbarHeight - 67;

  const rows = allLead?.list ?? [];

  return (
    <Box
      sx={{
        ...styleSheet.allOrderTable,
        height: calculatedHeight,
        "& .MuiDataGrid-main div": {
          opacity: showContent ? 1 : 0,
        },
      }}
    >
      <DataGridPro
        apiRef={apiRef}
        loading={loading}
        getRowHeight={() => "auto"}
        headerHeight={40}
        sx={{
          fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
          fontSize: "12px",
          fontWeight: "500",
          "& .MuiDataGrid-cell": {
            display: "flex",
            alignItems: "center",
          },
        }}
        rows={rows}
        getRowId={(row) => row.LeadId || row.leadId || row.PhoneNumber + row.ProductName}
        columns={columns}
        disableRowSelectionOnClick
        pagination
        page={currentPage}
        pageSize={pageSize}
        rowsPerPageOptions={[5, 10, 15, 25]}
        paginationMode="client"
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        checkboxSelection
        rowSelectionModel={selectionModel}
        onRowSelectionModelChange={(ids) => handleSelectedRow(ids)}
        columnVisibilityModel={columnVisibilityModel}
        onColumnVisibilityModelChange={handleColumnVisibilityModelChange}
      />

      <Menu
        anchorEl={anchorEl}
        id="leads-row-menu"
        open={Boolean(anchorEl)}
        onClose={() => setAnchorEl(null)}
        PaperProps={{
          elevation: 0,
          sx: {
            overflow: "visible",
            filter: "drop-shadow(0px 2px 8px rgba(0,0,0,0.32))",
            mt: 1.5,
          },
        }}
        transformOrigin={{ horizontal: "right", vertical: "top" }}
        anchorOrigin={{ horizontal: "right", vertical: "bottom" }}
      />

      {slectedAddress.open && (
        <UpdateOrderAddressLatLangModal
          open={slectedAddress.open}
          setOpen={() => handleCloseUpdateMap()}
          slectedAddress={slectedAddress}
          setSelectedAddress={setSelectedAddress}
          getAllLead={getAllLead}
        />
      )}

      <ContactOrdersModal
        open={openContactModal}
        onClose={() => setOpenContactModal(false)}
        mobileNumber={selectedContactMobile}
      />

      <EditLeadModal
        open={openEditModal}
        onClose={() => setOpenEditModal(false)}
        lead={selectedLeadForEdit}
        onSuccess={getAllLead}
      />
    </Box>
  );
}

export default LeadsList;
