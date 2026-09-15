import {
  AssignmentTurnedInOutlined,
  DeleteSweepOutlined,
  Inventory2Outlined,
  LocalShippingOutlined,
  SettingsBackupRestoreOutlined,
  SwapHorizOutlined,
} from "@mui/icons-material";
import {
  Avatar,
  Box,
  Card,
  CardContent,
  Chip,
  CircularProgress,
  Grid,
  Stack,
  Typography,
  Alert,
} from "@mui/material";
import { useEffect, useState } from "react";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import OtpInput from "react-otp-input";
import {
  GetWipeOutSectionsLookup,
  SendFactoryResetOtp,
  WipeOutClientData,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { successNotification } from "../../../utilities/toast";
import { setThisKeyCookie, getThisKeyCookie } from "../../../utilities/cookies";

const sectionIcons = {
  All: DeleteSweepOutlined,
  Products: Inventory2Outlined,
  Orders: LocalShippingOutlined,
  Returns: SwapHorizOutlined,
  Delivery: AssignmentTurnedInOutlined,
};

const sectionStyles = {
  All: {
    background: "linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%)",
    iconBg: "rgba(255,255,255,0.2)",
    iconColor: "#ffffff",
    textColor: "#ffffff",
  },
  Products: {
    background: "linear-gradient(135deg, #0f766e 0%, #14b8a6 100%)",
    iconBg: "rgba(255,255,255,0.2)",
    iconColor: "#ffffff",
    textColor: "#ffffff",
  },
  Orders: {
    background: "linear-gradient(135deg, #2563eb 0%, #3b82f6 100%)",
    iconBg: "rgba(255,255,255,0.2)",
    iconColor: "#ffffff",
    textColor: "#ffffff",
  },
  Returns: {
    background: "linear-gradient(135deg, #dc2626 0%, #f97316 100%)",
    iconBg: "rgba(255,255,255,0.2)",
    iconColor: "#ffffff",
    textColor: "#ffffff",
  },
  Delivery: {
    background: "linear-gradient(135deg, #b45309 0%, #f59e0b 100%)",
    iconBg: "rgba(255,255,255,0.2)",
    iconColor: "#ffffff",
    textColor: "#ffffff",
  },
  default: {
    background: "linear-gradient(135deg, #e2e8f0 0%, #f8fafc 100%)",
    iconBg: "#eef2ff",
    iconColor: "#4338ca",
    textColor: "#0f172a",
  },
};

const FactoryReset = () => {
  const [wipeOutSections, setWipeOutSections] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [openConfirmModal, setOpenConfirmModal] = useState(false);
  const [selectedSection, setSelectedSection] = useState(null);
  const [resetLoading, setResetLoading] = useState(false);
  const [openOtpModal, setOpenOtpModal] = useState(false);
  const [otp, setOtp] = useState("");
  const [otpError, setOtpError] = useState("");
  const [otpLoading, setOtpLoading] = useState(false);
  const [otpValidated, setOtpValidated] = useState(false);
  const [showFinalConfirmation, setShowFinalConfirmation] = useState(false);

  const getWipeOutSectionsLookup = async () => {
    try {
      setLoading(true);
      const response = await GetWipeOutSectionsLookup();
      console.log(response);
      if (response?.data?.isSuccess) {
        setWipeOutSections(response?.data?.result || []);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  const handleCardClick = (section) => {
    setOtp("");
    setOtpError("");
    setSelectedSection(section);
    setOpenOtpModal(true);
    handleSendOtp();
  };

  const handleSendOtp = async () => {
    try {
      setOtpLoading(true);
      setOtpError("");
      const response = await SendFactoryResetOtp();
      if (response?.data?.isSuccess) {
        setThisKeyCookie("factory_reset_otp", response?.data?.result?.data);
        successNotification("OTP sent successfully. Please check your email.");
      } else {
        setOtpError("Failed to send OTP. Please try again.");
      }
    } catch (e) {
      console.error("SendFactoryResetOtp Error:", e);
      setOtpError("Failed to send OTP. Please try again.");
    } finally {
      setOtpLoading(false);
    }
  };

  const handleValidateOtp = () => {
    if (!otp || otp.length !== 6) {
      setOtpError("Please enter a valid 6-digit OTP");
      return;
    }

    const savedOtp = getThisKeyCookie("factory_reset_otp");
    console.log("Entered OTP:", otp);
    console.log("Saved OTP from cookies:", savedOtp);

    if (otp === savedOtp) {
      setOtpValidated(true);
      setOtpError("");
      setOpenOtpModal(false);
      setShowFinalConfirmation(true);
    } else {
      setOtpError("OTP does not match. Please try again.");
    }
  };

  const handleCloseOtpModal = () => {
    setOpenOtpModal(false);
    setOtp("");
    setOtpError("");
    setOtpValidated(false);
    setSelectedSection(null);
  };

  const handleWipeOutConfirmed = async () => {
    if (!selectedSection) return;

    try {
      setResetLoading(true);
      const response = await WipeOutClientData(selectedSection.name);
      if (response?.data?.isSuccess) {
        successNotification("Factory Reset Complete");
        setShowFinalConfirmation(false);
        setSelectedSection(null);
        setOtpValidated(false);
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors,
        );
      }
    } catch (e) {
      console.error("WipeOutClientData API Error:", e);
    } finally {
      setResetLoading(false);
    }
  };

  const handleCloseFinalConfirmation = () => {
    setShowFinalConfirmation(false);
    setOtpValidated(false);
    setSelectedSection(null);
  };

  useEffect(() => {
    getWipeOutSectionsLookup();
  }, []);

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          {loading ? (
            <Box sx={{ display: "flex", justifyContent: "center", py: 8 }}>
              <CircularProgress />
            </Box>
          ) : error ? (
            <Box
              sx={{
                p: 3,
                borderRadius: 3,
                bgcolor: "#fff5f5",
                color: "#b42318",
                border: "1px solid #fecaca",
              }}
            >
              {error}
            </Box>
          ) : (
            <Grid container spacing={3}>
              {wipeOutSections.map((section) => {
                const Icon =
                  sectionIcons[section.name] || SettingsBackupRestoreOutlined;
                const styles =
                  sectionStyles[section.name] || sectionStyles.default;

                return (
                  <Grid item xs={12} sm={6} lg={4} key={section.id}>
                    <Card
                      elevation={0}
                      onClick={() => handleCardClick(section)}
                      sx={{
                        cursor: "pointer",
                        height: "100%",
                        borderRadius: 4,
                        border: "1px solid #e5e7eb",
                        overflow: "hidden",
                        transition: "transform 0.2s ease, box-shadow 0.2s ease",
                        "&:hover": {
                          transform: "translateY(-4px)",
                          boxShadow: "0 12px 30px rgba(15, 23, 42, 0.08)",
                        },
                      }}
                    >
                      <Box
                        sx={{
                          p: 3,
                          background: styles.background,
                          color: styles.textColor,
                        }}
                      >
                        <Stack
                          direction="row"
                          justifyContent="space-between"
                          alignItems="center"
                          sx={{ mb: 2 }}
                        >
                          <Avatar
                            sx={{
                              bgcolor: styles.iconBg,
                              color: styles.iconColor,
                              width: 48,
                              height: 48,
                            }}
                          >
                            <Icon />
                          </Avatar>
                          <Chip
                            label={
                              section.name === "All"
                                ? "High impact"
                                : "Scoped reset"
                            }
                            size="small"
                            sx={{
                              bgcolor: "rgba(255,255,255,0.2)",
                              color: "inherit",
                              fontWeight: 600,
                              border: "1px solid rgba(255,255,255,0.25)",
                            }}
                          />
                        </Stack>
                        <Typography variant="h6" sx={{ fontWeight: 700 }}>
                          {section.name}
                        </Typography>
                      </Box>

                      <CardContent sx={{ p: 3, bgcolor: "#ffffff" }}>
                        <Typography
                          variant="body1"
                          color="text.secondary"
                          sx={{ mb: 2 }}
                        >
                          {section.description}
                        </Typography>
                        <Box
                          sx={{
                            display: "flex",
                            justifyContent: "space-between",
                            alignItems: "center",
                            mt: 2,
                          }}
                        >
                          <Typography
                            variant="caption"
                            sx={{
                              color: "text.disabled",
                              fontWeight: 700,
                              textTransform: "uppercase",
                              letterSpacing: 1.2,
                            }}
                          >
                            Reset scope
                          </Typography>
                          <Box
                            component="span"
                            sx={{
                              px: 1.5,
                              py: 0.75,
                              borderRadius: 999,
                              bgcolor: "#eef2ff",
                              color: "#4338ca",
                              fontSize: 12,
                              fontWeight: 700,
                            }}
                          >
                            {section.name === "All" ? "Full data" : "Selective"}
                          </Box>
                        </Box>
                      </CardContent>
                    </Card>
                  </Grid>
                );
              })}
            </Grid>
          )}
        </div>
      </Box>

      {/* OTP Verification Modal */}
      <ModalComponent
        open={openOtpModal}
        onClose={handleCloseOtpModal}
        title="Verify OTP"
        maxWidth="sm"
        actionBtn={
          <ModalButtonComponent
            title="Validate OTP"
            loading={otpLoading}
            onClick={handleValidateOtp}
            disabled={otp.length !== 6}
          />
        }
      >
        <Box sx={{ py: 2 }}>
          <Typography sx={{ mb: 3, color: "text.secondary", fontSize: "15px" }}>
            Enter the 6-digit OTP sent to your email
          </Typography>

          {otpError && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {otpError}
            </Alert>
          )}

          <Box
            sx={{
              display: "flex",
              justifyContent: "center",
              my: 5,
              gap: 0.5,
            }}
          >
            <OtpInput
              value={otp}
              onChange={setOtp}
              numInputs={6}
              separator={<span style={{ width: "8px" }}></span>}
              inputStyle={{
                width: "60px",
                height: "60px",
                fontSize: "24px",
                fontWeight: "700",
                borderRadius: "12px",
                border: "2px solid #e5e7eb",
                backgroundColor: "#f9fafb",
                transition: "all 0.3s ease",
                textAlign: "center",
                letterSpacing: "4px",
              }}
              focusStyle={{
                borderColor: "#4f46e5",
                backgroundColor: "#f3f4f6",
                boxShadow: "0 0 0 4px rgba(79, 70, 229, 0.1)",
                outline: "none",
              }}
              containerStyle={{
                display: "flex",
                gap: "8px",
                justifyContent: "center",
              }}
              renderInput={(props) => <input {...props} />}
            />
          </Box>
        </Box>
      </ModalComponent>

      {/* Final Confirmation Modal */}
      <DeleteConfirmationModal
        open={showFinalConfirmation}
        setOpen={setShowFinalConfirmation}
        onClose={handleCloseFinalConfirmation}
        loading={resetLoading}
        handleDelete={handleWipeOutConfirmed}
        heading={`Confirm Reset of ${selectedSection?.name}?`}
        message="⚠️ IMPORTANT: This action cannot be undone. All data in this section will be permanently deleted from your system."
        messageDetails={`\n${selectedSection?.description || ""}`}
        buttonText="Reset"
        buttonColor="#dc2626"
      />
    </>
  );
};

export default FactoryReset;
