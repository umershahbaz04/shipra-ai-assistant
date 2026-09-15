import CloseIcon from "@mui/icons-material/Close";
import UploadIcon from "@mui/icons-material/Upload";
import {
  Box,
  Grid,
  IconButton,
  InputLabel,
  Typography,
  styled,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import { useRef, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { UploadPaperlessDocByCarrier } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

// === Helper functions ===
const isImage = (file) => file?.type?.startsWith("image/");
const isPdf = (file) =>
  file &&
  (file.type === "application/pdf" ||
    file.name?.toLowerCase().endsWith(".pdf"));

// === Styled components ===
const PreviewContainer = styled(Box)(({ theme }) => ({
  position: "relative",
  width: "100%",
  height: "100%",
  "&:hover .delete-icon": {
    opacity: 1,
  },
}));

const DeleteIcon = styled(IconButton)(({ theme }) => ({
  position: "absolute",
  top: 8,
  right: 8,
  opacity: 0,
  transition: "opacity 0.3s ease",
  backgroundColor: "rgba(0,0,0,0.4)",
  color: "#fff",
  "&:hover": {
    backgroundColor: "rgba(0,0,0,0.6)",
  },
}));

const documentTypes = [
  { value: "001", label: "Authorization Form" },
  { value: "002", label: "Commercial Invoice" },
  { value: "003", label: "Certificate of Origin" },
  { value: "004", label: "Export Accompanying Document" },
  { value: "005", label: "Export License" },
  { value: "006", label: "Import Permit" },
  { value: "007", label: "One Time NAFTA" },
  { value: "008", label: "Other Document" },
  { value: "009", label: "Power of Attorney" },
  { value: "010", label: "Packing List" },
  { value: "011", label: "SED Document" },
  { value: "012", label: "Shipper's Letter of Instruction" },
  { value: "013", label: "Declaration" },
];

const UPSCarrierUploadFileModal = (props) => {
  const { open, onClose, carrierId, activeCarrierId } = props;
  const [loading, setLoading] = useState(false);
  const [file, setFile] = useState(null);
  const [previewUrl, setPreviewUrl] = useState(null);
  const fileInputRef = useRef();
  const [selectedDocumentType, setselectedDocumentType] = useState([]);

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile) {
      const fileUrl = URL.createObjectURL(selectedFile);
      setFile(selectedFile);
      setPreviewUrl(fileUrl);
    }
  };

  const handleRemoveFile = () => {
    setFile(null);
    setPreviewUrl(null);

    if (fileInputRef.current) {
      fileInputRef.current.value = null;
    }
  };

  const handleUploadFile = async () => {
    if (
      !selectedDocumentType ||
      selectedDocumentType.value === 0 ||
      selectedDocumentType.length === 0
    ) {
      errorNotification("Please select a document type");
      return;
    }
    const formData = new FormData();
    formData.append("DocumentType", selectedDocumentType?.value || "");
    formData.append("File", file);
    formData.append("CarrierId", carrierId);
    formData.append("ActiveCarrierId", activeCarrierId);
    setLoading(true);
    try {
      const response = await UploadPaperlessDocByCarrier(formData);
      if (response.data?.isSuccess) {
        successNotification(response?.data?.result?.message);
        onClose();
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch {
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title="Upload & Preview File"
      actionBtn={
        <ModalButtonComponent
          loading={loading}
          title="Upload"
          bg={purple}
          onClick={handleUploadFile}
        />
      }
    >
      <Grid container spacing={2}>
        <Grid item xs={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {"Select Document Type"}
          </InputLabel>
          <SelectComponent
            name="documentType"
            options={documentTypes}
            value={selectedDocumentType}
            optionLabel="label"
            optionValue="value"
            onChange={(e, val) => {
              setselectedDocumentType(val);
            }}
          />
        </Grid>
        <Grid item xs={12}>
          {!previewUrl ? (
            <Box
              sx={{
                border: "2px dashed #ccc",
                borderRadius: "12px",
                height: 300,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                backgroundColor: "#f9f9f9",
                flexDirection: "column",
                textAlign: "center",
                cursor: "pointer",
                "&:hover": {
                  backgroundColor: "#f0f0f0",
                },
              }}
              onClick={() => fileInputRef.current.click()}
            >
              <UploadIcon sx={{ fontSize: 40, color: purple[500] }} />
              <Typography variant="subtitle1" mt={1}>
                Click to upload file or image
              </Typography>
              <input
                type="file"
                ref={fileInputRef}
                onChange={handleFileChange}
                style={{ display: "none" }}
                accept="image/*,.pdf"
              />
            </Box>
          ) : (
            <PreviewContainer
              sx={{
                border: "1px solid #ccc",
                borderRadius: "12px",
                height: 500,
                overflow: "hidden",
                backgroundColor: "#f9f9f9",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}
            >
              {isImage(file) ? (
                <img
                  src={previewUrl}
                  alt="Preview"
                  style={{
                    maxWidth: "100%",
                    maxHeight: "100%",
                    objectFit: "contain",
                  }}
                />
              ) : isPdf(file) ? (
                <iframe
                  src={previewUrl}
                  title="PDF Preview"
                  width="100%"
                  height="100%"
                  style={{ border: "none" }}
                />
              ) : (
                <Typography>Unsupported file type</Typography>
              )}

              <DeleteIcon
                className="delete-icon"
                onClick={handleRemoveFile}
                size="small"
              >
                <CloseIcon />
              </DeleteIcon>
            </PreviewContainer>
          )}
        </Grid>
      </Grid>
    </ModalComponent>
  );
};

export default UPSCarrierUploadFileModal;
