import CheckIcon from "@mui/icons-material/Check";
import CloseIcon from "@mui/icons-material/Close";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import OpenInNewIcon from "@mui/icons-material/OpenInNew";
import { Avatar, Box, Button, IconButton, Link } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import {
  AddUpdateBranding,
  GetAllCustomDomains,
  GetCustomDomainRecordByCustomDomainId,
  UploadStoreImage,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import AddCustomDomainModal from "../../../components/modals/settingsModals/AddCustomDomainModal";
import {
  ActionButtonCustom,
  centerColumn,
  UploadButton,
  useGetWindowHeight,
  usePagination,
} from "../../../utilities/helpers/Helpers";
import { getBrandingAfterDataChange } from "../../../utilities/helpers/HelpersFilter";
import { successNotification } from "../../../utilities/toast";
import SetupCustomDomainModal from "../../../components/modals/settingsModals/SetupCustomDomainModal";

const EnumTabFilter = Object.freeze({
  DASHBOARD: "/branding",
});

const Branding = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { height: windowHeight } = useGetWindowHeight();
  const { currentPage, pageSize, handlePageChange, handlePageSizeChange } =
    usePagination(0, 25);
  const calculatedHeight = windowHeight - 95 - 45;
  const [openAddCustomDomainModal, setOpenAddCustomDomainModal] =
    useState(false);
  const [imageUrl, setImageUrl] = useState();
  const [allCustomDomain, setAllCustomDomain] = useState([]);
  const [loading, setLoading] = useState(false);
  const [customDomainData, setCustomDomainData] = useState({
    open: false,
    data: [],
    loading: {},
  });

  const brandingData = sessionStorage.getItem("Branding");
  const decoded = brandingData ? decodeURIComponent(brandingData) : null;
  const Branding = decoded ? JSON?.parse(decoded) : decoded;

  const uploadBrandLogo = (e) => {
    const formData = new FormData();
    formData.append("file", e.target.files[0]);
    UploadStoreImage(formData)
      .then((res) => {
        successNotification(res.data.result.message);
        setImageUrl(res?.data?.result?.url);
      })
      .catch((e) => console.log("e", e));
  };

  const SaveBrandingImage = async () => {
    try {
      const response = await AddUpdateBranding(imageUrl);
      if (response?.data.isSuccess) {
        getBrandingAfterDataChange();
        setImageUrl("");
      }
    } catch (e) {}
  };

  const getAllCustomDomains = async () => {
    setLoading(true);
    try {
      const response = await GetAllCustomDomains();
      if (response?.data.isSuccess) {
        setAllCustomDomain(response?.data?.result);
      }
    } catch (e) {
    } finally {
      setLoading(false);
    }
  };

  const handleEditCustomDomain = async (data) => {
    const customDomainId = data?.customDomainId;
    try {
      setCustomDomainData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [customDomainId]: true },
      }));

      const response =
        await GetCustomDomainRecordByCustomDomainId(customDomainId);
      if (response?.data?.isSuccess) {
        const result = response?.data.result;
        setCustomDomainData((prev) => ({
          ...prev,
          data: result,
        }));
        setCustomDomainData((prev) => ({ ...prev, open: true }));
      }
    } catch (error) {
      console.error("Error fetching custom domain:", error);
    } finally {
      setCustomDomainData((prev) => ({
        ...prev,
        loading: { ...prev.loading, [customDomainId]: false },
      }));
    }
  };

  const columns = [
    {
      field: "domainName",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Domain"}</Box>,
      flex: 1,
      renderCell: (params) => (
        <Box>
          <Box display="flex" alignItems="center" gap={1}>
            <Link
              href={`https://${params.value}`}
              target="_blank"
              underline="hover"
              sx={{ color: "#1976d2" }}
            >
              {params.value}
            </Link>
            <OpenInNewIcon fontSize="small" sx={{ color: "#1976d2" }} />
          </Box>
          <Box>
            <span style={{ fontSize: "0.75rem", color: "gray" }}>
              {params.row.domainName}
            </span>
          </Box>
        </Box>
      ),
    },
    {
      field: "statusValue",
      headerName: <Box sx={{ fontWeight: "bold" }}>{"Status"}</Box>,
      flex: 0.5,
    },
    {
      ...centerColumn,
      field: "Action",
      minWidth: 120,
      headerName: <Box sx={{ fontWeight: "600" }}>{"Action"}</Box>,
      renderCell: ({ row }) => {
        return (
          <Box display={"flex"} gap={1} width="100%" justifyContent={"center"}>
            {!row.isDefault && (
              <>
                <Button
                  sx={styleSheet.deleteProductButton}
                  variant="outlined"
                  //   onClick={}
                >
                  <DeleteIcon />
                </Button>
                <ActionButtonCustom
                  onClick={() => handleEditCustomDomain(row)}
                  loading={customDomainData.loading[row.customDomainId]}
                  sx={styleSheet.editProductButton}
                  label={<EditIcon />}
                />
              </>
            )}
          </Box>
        );
      },
      flex: 0.5,
    },
  ];

  useEffect(() => {
    getAllCustomDomains();
  }, []);

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <DataGridTabs
          tabsSmWidth="10px"
          tabsMdWidth="10px"
          tabData={[
            {
              label: "DashBoard",
              route: EnumTabFilter.DASHBOARD,
            },
          ]}
          otherBtns={
            <>
              <ButtonComponent
                title={"Add Custom Domain"}
                onClick={() => setOpenAddCustomDomainModal(true)}
                btnMdWidth={"145px"}
              />
              <UploadButton
                onChange={uploadBrandLogo}
                label="Update Brand Logo"
                background={
                  "var(--primary-color)"
                }
              />
              <Avatar
                variant="rounded"
                sx={{
                  width: 35,
                  height: 35,
                  border: "1px solid #9e9e9e",
                  borderRadius: "8px",
                  p: 0.5,
                  "& img": {
                    objectFit: "contain !important",
                  },
                }}
                src={imageUrl || Branding?.logoUrl}
              />
              {imageUrl && (
                <>
                  <IconButton
                    color="error"
                    size="small"
                    onClick={() => setImageUrl("")}
                  >
                    <CloseIcon />
                  </IconButton>
                  <IconButton
                    color="success"
                    size="small"
                    onClick={SaveBrandingImage}
                  >
                    <CheckIcon />
                  </IconButton>
                </>
              )}
            </>
          }
        />
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            height: calculatedHeight,
          }}
        >
          <DataGrid
            loading={loading}
            headerHeight={40}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row.customDomainId}
            rows={allCustomDomain}
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
        {openAddCustomDomainModal && (
          <AddCustomDomainModal
            open={openAddCustomDomainModal}
            onClose={() => setOpenAddCustomDomainModal(false)}
            getAllCustomDomains={getAllCustomDomains}
          />
        )}
        {customDomainData.open && (
          <SetupCustomDomainModal
            open={customDomainData.open}
            onClose={() =>
              setCustomDomainData((prev) => ({ ...prev, open: false }))
            }
            getAllCustomDomains={getAllCustomDomains}
            setupDomainData={customDomainData?.data}
          />
        )}
      </div>
    </Box>
  );
};

export default Branding;
