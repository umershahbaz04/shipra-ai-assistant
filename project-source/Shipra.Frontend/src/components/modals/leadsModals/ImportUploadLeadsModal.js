import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import {
  Alert,
  Box,
  Button,
  Grid,
  IconButton,
  InputLabel,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  ToggleButton,
  ToggleButtonGroup,
  Fade,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import { useRef, useState, useEffect } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import {
  CreateBulkLeads,
  UploadLeadsData,
  GetAllCountry,
} from "../../../api/AxiosInterceptors";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import CountrySchema from "../../../utilities/helpers/countryschema";
import { EnumOptions } from "../../../utilities/enum";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import Colors from "../../../utilities/helpers/Colors";
import { styleSheet } from "../../../assets/styles/style";

const ImportUploadLeadsModal = (props) => {
  const { open, onClose, onSuccess } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [xlsxFile, setXlsxFile] = useState(null);
  const [isUploading, setIsUploading] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [previewRows, setPreviewRows] = useState([]);
  const [editingRowId, setEditingRowId] = useState(null);
  const [editValues, setEditValues] = useState({});
  const [importMode, setImportMode] = useState("excel"); // 'excel' or 'paste'
  const [pasteData, setPasteData] = useState("");
  const fileInputRef = useRef(null);

  const [countryId, setCountryId] = useState("");
  const [allCountries, setAllCountries] = useState([]);
  const [apiErrorMessage, setApiErrorMessage] = useState("");

  const fetchCountries = async () => {
    try {
      const response = await GetAllCountry();
      if (response?.data?.result) {
        setAllCountries(response.data.result);
      }
    } catch (e) {
      console.error(e);
    }
  };

  useEffect(() => {
    if (open) {
      setXlsxFile(null);
      setCountryId("");
      fetchCountries();
      setPreviewRows([]);
      setEditingRowId(null);
      setEditValues({});
      setImportMode("excel");
      setPasteData("");
      setApiErrorMessage("");
      if (fileInputRef.current) fileInputRef.current.value = "";
    }
  }, [open]);

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (
      file &&
      (file.type ===
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
        file.name.endsWith(".xlsx"))
    ) {
      setXlsxFile(file);
      setPreviewRows([]);
    } else {
      errorNotification("Please select a valid .xlsx file");
    }
  };

  const handleUpload = async () => {
    if (!xlsxFile) {
      errorNotification("Please select a file first");
      return;
    }
    setIsUploading(true);
    try {
      const formData = new FormData();
      formData.append("File", xlsxFile);
      const response = await UploadLeadsData(formData);

      // Response: { result: { successList: [], errors: [] } }
      const result = response?.data?.result;
      const successList = result?.successList || [];
      const errors = result?.errors || [];

      const rowsWithStatus = successList.map((row, index) => {
        const errorEntry = errors.find((e) => String(e.Row || e.row) === String(index + 2));
        return {
          _id: index,
          PhoneNumber: row.phoneNumber || row.PhoneNumber || "",
          ProductName:
            row.productName ||
            row.ProductName ||
            row.ProjectName ||
            row.projectName ||
            "",
          GoogleLocationLink:
            row.googleLocationLink || row.GoogleLocationLink || "",
          hasError: !!errorEntry,
          errorMsg: errorEntry ? (Array.isArray(errorEntry.Msg || errorEntry.msg) ? (errorEntry.Msg || errorEntry.msg).join(", ") : (errorEntry.Msg || errorEntry.msg)) : "",
        };
      });

      // Also add rows that have errors but might not be in successList
      errors.forEach((err) => {
        const rowIndex = parseInt(err.Row || err.row) - 2;
        if (rowIndex >= 0 && rowIndex < rowsWithStatus.length) {
          // Already marked existing row as error above
        } else {
          rowsWithStatus.push({
            _id: rowsWithStatus.length,
            PhoneNumber: err.Item?.PhoneNumber || err.item?.phoneNumber || "",
            ProductName: err.Item?.ProductName || err.item?.productName || "",
            GoogleLocationLink: err.Item?.GoogleLocationLink || err.item?.googleLocationLink || "",
            hasError: true,
            errorMsg: Array.isArray(err.Msg || err.msg) ? (err.Msg || err.msg).join(", ") : (err.Msg || err.msg),
          });
        }
      });

      if (rowsWithStatus.length === 0) {
        errorNotification("No data found in the file or all rows have errors");
      } else {
        setPreviewRows(rowsWithStatus);
      }
    } catch (e) {
      console.error(e);
      errorNotification("Upload failed. Please try again.");
    } finally {
      setIsUploading(false);
    }
  };

  const handleProcessPaste = () => {
    if (!pasteData.trim()) return;

    // Split by new lines, ignoring empty lines
    const lines = pasteData.split(/\r?\n/).filter((line) => line.trim() !== "");

    const parsedRows = lines
      .map((line, index) => {
        // Split by tab (default for Excel/Google Sheets copy-paste)
        const cols = line.split("\t");

        const phoneNumber = cols[0] ? cols[0].trim() : "";
        const productName = cols[1] ? cols[1].trim() : "";

        return {
          _id: index,
          PhoneNumber: phoneNumber,
          ProductName: productName,
          GoogleLocationLink: "",
          hasError: false,
          errorMsg: "",
        };
      })
      .filter((row) => row.PhoneNumber && row.ProductName);

    if (parsedRows.length === 0) {
      errorNotification("No valid data found to process.");
    } else {
      setPreviewRows(parsedRows);
      setPasteData("");
    }
  };

  const handleDelete = (id) => {
    setPreviewRows((prev) => prev.filter((r) => r._id !== id));
  };

  const handleEditStart = (row) => {
    setEditingRowId(row._id);
    setEditValues({ ...row });
  };

  const handleEditSave = () => {
    setPreviewRows((prev) =>
      prev.map((r) =>
        r._id === editingRowId
          ? { ...editValues, hasError: false, errorMsg: "" }
          : r,
      ),
    );
    setEditingRowId(null);
    setEditValues({});
  };

  const handleEditCancel = () => {
    setEditingRowId(null);
    setEditValues({});
  };

  const hasAnyError = previewRows.some((r) => r.hasError);
  const validRows = previewRows.filter((r) => !r.hasError);

  const handleCreate = async () => {
    if (hasAnyError) {
      errorNotification(
        "Please fix or remove all error rows before creating leads",
      );
      return;
    }
    if (validRows.length === 0) {
      errorNotification("No valid rows to create");
      return;
    }
    if (!countryId) {
      errorNotification("Please select a Country");
      return;
    }
    setIsCreating(true);
    setApiErrorMessage("");
    try {
      const payload = {
        Leads: validRows.map((r) => ({
          PhoneNumber: r.PhoneNumber,
          ProductName: r.ProductName,
          CustomerName: r.CustomerName || "",
          Address: r.Address || "",
          GoogleLocationLink: r.GoogleLocationLink || "",
          Amount: r.Amount ? Number(r.Amount) : null,
          CountryId: countryId?.countryId || countryId?.id || countryId,
        })),
      };
      const response = await CreateBulkLeads(payload);
      
      const successList = response?.data?.result?.successList || [];
      const resultErrors = response?.data?.result?.errors || {};
      const apiErrors = response?.data?.errors || {};
      
      // Check if there are any errors returned by backend (either resultErrors or ModelState apiErrors)
      const hasErrors =
        Object.keys(resultErrors).length > 0 ||
        Object.keys(apiErrors).length > 0 ||
        response?.data?.isSuccess === false;

      if (!hasErrors) {
        successNotification(`${validRows.length} Lead(s) created successfully`);
        if (onSuccess) onSuccess();
        handleClose();
      } else {
        const errorMessages = [
          ...Object.values(apiErrors).flat(),
          ...Object.values(resultErrors).flat(),
        ];
        
        setApiErrorMessage(errorMessages.join(", "));

        // Filter out successful rows from the preview table
        let remainingRows = previewRows.filter((row) => {
          const isSuccess = successList.some(
            (s) =>
              (s.phoneNumber && String(s.phoneNumber) === String(row.PhoneNumber)) ||
              (s.PhoneNumber && String(s.PhoneNumber) === String(row.PhoneNumber))
          );
          return !isSuccess;
        });

        // Create an error mapping for each row ID
        const rowErrors = {};

        // 1. Process ModelState style API errors: e.g., "Leads[0].PhoneNumber"
        Object.entries(apiErrors).forEach(([key, messages]) => {
          const match = key.match(/Leads\[(\d+)\]/);
          if (match) {
            const index = parseInt(match[1], 10);
            const targetRow = validRows[index];
            if (targetRow) {
              const msgs = Array.isArray(messages) ? messages : [messages];
              if (!rowErrors[targetRow._id]) {
                rowErrors[targetRow._id] = [];
              }
              rowErrors[targetRow._id].push(...msgs);
            }
          }
        });

        // 2. Process resultErrors (by phone number key)
        Object.entries(resultErrors).forEach(([key, messages]) => {
          const targetRow = previewRows.find(
            (r) => String(r.PhoneNumber) === String(key)
          );
          if (targetRow) {
            const msgs = Array.isArray(messages) ? messages : [messages];
            if (!rowErrors[targetRow._id]) {
              rowErrors[targetRow._id] = [];
            }
            rowErrors[targetRow._id].push(...msgs);
          }
        });

        // Apply specific errors to the remaining rows
        remainingRows = remainingRows.map((row) => {
          // If we have mapped specific errors for this row
          if (rowErrors[row._id] && rowErrors[row._id].length > 0) {
            return {
              ...row,
              hasError: true,
              errorMsg: rowErrors[row._id].join(", "),
            };
          }

          // Fallback check by including phone number in any error message text
          const matchingError = errorMessages.find(
            (msg) => row.PhoneNumber && String(msg).includes(row.PhoneNumber)
          );
          if (matchingError) {
            const regex = new RegExp(`${row.PhoneNumber}\\s*\\(Status:\\s*([^)]+)\\)`);
            const statusMatch = String(matchingError).match(regex);
            return {
              ...row,
              hasError: true,
              errorMsg: statusMatch 
                ? `Lead already exists with status: ${statusMatch[1]}`
                : String(matchingError),
            };
          }

          // Default fallback error message if this row failed but has no specific error
          return {
            ...row,
            hasError: true,
            errorMsg: "Failed to create lead (Duplicate or invalid details)",
          };
        });

        setPreviewRows(remainingRows);

        if (successList.length > 0) {
          successNotification(`${successList.length} Lead(s) created successfully.`);
        }
        errorNotification("Some leads failed to create. Please check the error icon in the table.");
      }
    } catch (e) {
      console.error(e);
      const apiErrors = e?.response?.data?.errors || {};
      const resultErrors = e?.response?.data?.result?.errors || {};
      const errorMessages = [
        ...Object.values(apiErrors).flat(),
        ...Object.values(resultErrors).flat(),
      ];
      
      setApiErrorMessage(errorMessages.join(", ") || "Failed to create leads");
      
      const rowErrors = {};

      Object.entries(apiErrors).forEach(([key, messages]) => {
        const match = key.match(/Leads\[(\d+)\]/);
        if (match) {
          const index = parseInt(match[1], 10);
          const targetRow = validRows[index];
          if (targetRow) {
            const msgs = Array.isArray(messages) ? messages : [messages];
            if (!rowErrors[targetRow._id]) {
              rowErrors[targetRow._id] = [];
            }
            rowErrors[targetRow._id].push(...msgs);
          }
        }
      });

      Object.entries(resultErrors).forEach(([key, messages]) => {
        const targetRow = previewRows.find(
          (r) => String(r.PhoneNumber) === String(key)
        );
        if (targetRow) {
          const msgs = Array.isArray(messages) ? messages : [messages];
          if (!rowErrors[targetRow._id]) {
            rowErrors[targetRow._id] = [];
          }
          rowErrors[targetRow._id].push(...msgs);
        }
      });

      let remainingRows = [...previewRows];
      remainingRows = remainingRows.map((row) => {
        if (rowErrors[row._id] && rowErrors[row._id].length > 0) {
          return {
            ...row,
            hasError: true,
            errorMsg: rowErrors[row._id].join(", "),
          };
        }
        const matchingError = errorMessages.find(
          (msg) => row.PhoneNumber && String(msg).includes(row.PhoneNumber)
        );
        if (matchingError) {
          const regex = new RegExp(`${row.PhoneNumber}\\s*\\(Status:\\s*([^)]+)\\)`);
          const statusMatch = String(matchingError).match(regex);
          return {
            ...row,
            hasError: true,
            errorMsg: statusMatch 
              ? `Lead already exists with status: ${statusMatch[1]}`
              : String(matchingError),
          };
        }
        return {
          ...row,
          hasError: true,
          errorMsg: "Failed to create lead",
        };
      });
      
      setPreviewRows(remainingRows);
      errorNotification("Failed to create leads. Please check the error icon in the table.");
    } finally {
      setIsCreating(false);
    }
  };

  const handleClose = () => {
    setXlsxFile(null);
    setPreviewRows([]);
    setEditingRowId(null);
    setEditValues({});
    setImportMode("excel");
    setPasteData("");
    setCountryId("");
    setApiErrorMessage("");
    if (fileInputRef.current) fileInputRef.current.value = "";
    onClose();
  };

  const editableFields = [
    { key: "PhoneNumber", label: "Phone Number" },
    { key: "ProductName", label: "Product Name" },
    { key: "GoogleLocationLink", label: "Location Link" },
  ];

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      title={
        importMode === "excel"
          ? "Upload Leads from Excel"
          : "Import Leads by Copy/Paste"
      }
      actionBtn={
        previewRows.length > 0 ? (
          <ModalButtonComponent
            title={`Create ${validRows.length} Lead(s)`}
            bg={Colors.succes || "#4CAF50"}
            type="button"
            loading={isCreating}
            onClick={handleCreate}
            disabled={hasAnyError}
          />
        ) : importMode === "excel" ? (
          <ModalButtonComponent
            title={"Upload"}
            bg={purple[700]}
            type="button"
            loading={isUploading}
            onClick={handleUpload}
            disabled={!xlsxFile}
          />
        ) : (
          <ModalButtonComponent
            title={"Process Leads"}
            bg={purple[700]}
            type="button"
            loading={false}
            onClick={handleProcessPaste}
            disabled={!pasteData.trim()}
          />
        )
      }
    >
      <Box sx={{ minWidth: 400 }}>
        {/* Toggle Button for Excel vs Paste */}
        {previewRows.length === 0 && (
          <Box sx={{ display: "flex", justifyContent: "center", mb: 3 }}>
            <ToggleButtonGroup
              value={importMode}
              exclusive
              onChange={(e, val) => {
                if (val !== null) setImportMode(val);
              }}
              aria-label="import mode"
              size="small"
              sx={{
                background: "#f0f0f5",
                borderRadius: "20px",
                padding: "4px",
                border: "none",
                width: "350px", // Increased width
                "& .MuiToggleButtonGroup-grouped": {
                  border: "none",
                  borderRadius: "16px !important",
                  mx: 0.5,
                  flex: 1, // Equal width for buttons
                  py: 0.75,
                  textTransform: "none",
                  fontWeight: 600,
                  fontSize: "13px",
                  color: "#666",
                  transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
                  "&.Mui-selected": {
                    background: purple[700],
                    color: "#fff",
                    boxShadow: "0px 4px 10px rgba(86, 58, 213, 0.25)",
                    "&:hover": {
                      background: purple[800],
                    },
                  },
                  "&:hover": {
                    background: "rgba(0, 0, 0, 0.04)",
                  },
                },
              }}
            >
              <ToggleButton value="excel">Upload Excel</ToggleButton>
              <ToggleButton value="paste">Copy/Paste</ToggleButton>
            </ToggleButtonGroup>
          </Box>
        )}

        {/* Country Picker */}
        {/* {previewRows.length > 0 && ( */}
        <Box
          sx={{ mb: 2, position: "relative", zIndex: 9999, display: "none" }}
        >
          <InputLabel sx={{ mb: 1, fontWeight: "bold" }}>Country *</InputLabel>
          <CountrySchema
            name="country"
            height={35}
            value={countryId}
            onChange={(e, val) => setCountryId(val)}
          />
        </Box>
        {/* )} */}

        {/* File picker */}
        {previewRows.length === 0 && importMode === "excel" && (
          <Fade in={importMode === "excel"}>
            <Box sx={{ mb: 2 }}>
              <InputLabel sx={{ mb: 1, fontWeight: "bold" }}>
                Select Excel File (.xlsx)
              </InputLabel>
              <input
                type="file"
                accept=".xlsx"
                ref={fileInputRef}
                onChange={handleFileChange}
                style={{ padding: "8px 0" }}
              />
              {xlsxFile && (
                <Box sx={{ mt: 1, fontSize: "12px", color: "#555" }}>
                  Selected: <strong>{xlsxFile.name}</strong>
                </Box>
              )}
            </Box>
          </Fade>
        )}

        {/* Paste leads view */}
        {previewRows.length === 0 && importMode === "paste" && (
          <Fade in={importMode === "paste"}>
            <Box sx={{ mb: 2 }}>
              <InputLabel sx={{ mb: 1, fontWeight: "bold" }}>
                Paste Leads Data
              </InputLabel>
              <TextField
                multiline
                minRows={6}
                maxRows={15}
                fullWidth
                placeholder="Paste your leads data here..."
                value={pasteData}
                onChange={(e) => setPasteData(e.target.value)}
                sx={{
                  "& .MuiOutlinedInput-root": {
                    fontSize: "12px",
                    fontFamily:
                      "'Lato Regular', 'Inter Regular', 'Arial' !important",
                  },
                  "& textarea": {
                    resize: "vertical",
                  },
                }}
              />
              <Box sx={{ mt: 1, fontSize: "11px", color: "#666" }}>
                <strong>Note:</strong> <b>Phone number</b> must be always first
                in the first column and <b>product name</b> will be the second
                one. Works with data copied from Excel or Google Sheets.
              </Box>
            </Box>
          </Fade>
        )}

        {/* Preview table */}
        {previewRows.length > 0 && (
          <Box>
            {hasAnyError && (
              <Alert severity="warning" sx={{ mb: 1 }}>
                {apiErrorMessage || "Some rows have errors. Please fix or delete them before creating leads."}
              </Alert>
            )}
            <TableContainer sx={{ maxHeight: 400 }}>
              <Table stickyHeader size="small">
                <TableHead>
                  <TableRow>
                    <TableCell
                      sx={{ fontWeight: "bold", whiteSpace: "nowrap" }}
                    >
                      Phone Number
                    </TableCell>
                    <TableCell
                      sx={{ fontWeight: "bold", whiteSpace: "nowrap" }}
                    >
                      Product Name
                    </TableCell>
                    <TableCell
                      sx={{ fontWeight: "bold", whiteSpace: "nowrap" }}
                    >
                      Location Link
                    </TableCell>
                    <TableCell
                      sx={{ fontWeight: "bold", whiteSpace: "nowrap" }}
                    >
                      Status
                    </TableCell>
                    <TableCell sx={{ fontWeight: "bold" }}>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {previewRows.map((row) =>
                    editingRowId === row._id ? (
                      // Edit mode row
                      <TableRow key={row._id} sx={{ background: "#fffde7" }}>
                        {editableFields.map((f) => (
                          <TableCell key={f.key}>
                            <TextField
                              size="small"
                              value={editValues[f.key] || ""}
                              onChange={(e) =>
                                setEditValues((prev) => ({
                                  ...prev,
                                  [f.key]: e.target.value,
                                }))
                              }
                              placeholder={f.label}
                              sx={{ minWidth: 100 }}
                            />
                          </TableCell>
                        ))}
                        <TableCell>
                          <Box sx={{ fontSize: "11px", color: "green" }}>
                            Editing...
                          </Box>
                        </TableCell>
                        <TableCell>
                          <Stack direction="row" spacing={0.5}>
                            <Button
                              variant="contained"
                              size="small"
                              onClick={handleEditSave}
                              sx={{
                                background: Colors.succes || "#4CAF50",
                                color: "#fff !important",
                                textTransform: "none !important",
                                fontWeight: "bold",
                                borderRadius: "8px !important",
                                minWidth: "60px",
                                height: "30px",
                                fontSize: "12px",
                                "&:hover": {
                                  background: Colors.succes || "#45a049",
                                }
                              }}
                            >
                              Save
                            </Button>
                            <Button
                              variant="contained"
                              size="small"
                              onClick={handleEditCancel}
                              sx={{
                                background: "#9e9e9e",
                                color: "#fff !important",
                                textTransform: "none !important",
                                fontWeight: "bold",
                                borderRadius: "8px !important",
                                minWidth: "65px",
                                height: "30px",
                                fontSize: "12px",
                                "&:hover": {
                                  background: "#757575",
                                }
                              }}
                            >
                              Cancel
                            </Button>
                          </Stack>
                        </TableCell>
                      </TableRow>
                    ) : (
                      // Normal row
                      <TableRow
                        key={row._id}
                        sx={{
                          background: row.hasError ? "#fff3f3" : "inherit",
                          borderLeft: row.hasError
                            ? "3px solid #f44336"
                            : "none",
                        }}
                      >
                        <TableCell sx={{ fontSize: "12px" }}>
                          {row.PhoneNumber}
                        </TableCell>
                        <TableCell sx={{ fontSize: "12px" }}>
                          {row.ProductName}
                        </TableCell>
                        <TableCell sx={{ fontSize: "12px" }}>
                          {row.GoogleLocationLink ? (
                            <a
                              href={row.GoogleLocationLink}
                              target="_blank"
                              rel="noreferrer"
                            >
                              View
                            </a>
                          ) : (
                            "-"
                          )}
                        </TableCell>
                        <TableCell>
                          {row.hasError ? (
                            <Tooltip title={row.errorMsg}>
                              <Box
                                sx={{
                                  color: "#f44336",
                                  fontSize: "11px",
                                  cursor: "pointer",
                                }}
                              >
                                ⚠ Error
                              </Box>
                            </Tooltip>
                          ) : (
                            <Box sx={{ color: "green", fontSize: "11px" }}>
                              ✓ Valid
                            </Box>
                          )}
                        </TableCell>
                        <TableCell>
                          <Stack direction="row" spacing={0.5}>
                            <Tooltip title="Edit">
                              <IconButton
                                size="small"
                                onClick={() => handleEditStart(row)}
                                sx={{ color: Colors.linkColor || "#1976d2" }}
                              >
                                <EditIcon sx={{ fontSize: 16 }} />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title="Delete">
                              <IconButton
                                size="small"
                                onClick={() => handleDelete(row._id)}
                                sx={{ color: "#f44336" }}
                              >
                                <DeleteIcon sx={{ fontSize: 16 }} />
                              </IconButton>
                            </Tooltip>
                          </Stack>
                        </TableCell>
                      </TableRow>
                    ),
                  )}
                </TableBody>
              </Table>
            </TableContainer>
            <Box sx={{ mt: 1, fontSize: "12px", color: "#555" }}>
              Total: <strong>{previewRows.length}</strong> rows | Valid:{" "}
              <strong style={{ color: "green" }}>{validRows.length}</strong> |
              Errors:{" "}
              <strong style={{ color: "#f44336" }}>
                {previewRows.length - validRows.length}
              </strong>
            </Box>
          </Box>
        )}
      </Box>
    </ModalComponent>
  );
};

export default ImportUploadLeadsModal;
