import React, { useEffect, useState } from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { Grid, IconButton, Button, Typography } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import {
  GetAllCpsettlementPopFilesById,
  DeleteCpsettlementPopFile,
} from "../../../api/AxiosInterceptors";
import { useSelector } from "react-redux";

export default function UploadCarrierSettlementFileModal({
  dialogOpen,
  handleDialogClose,
  selctedRow,
}) {
  const [itemData, setItemData] = useState([]);
  const [hoveredIndex, setHoveredIndex] = useState(null);
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [selectedImageId, setSelectedImageId] = useState(null);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleMouseEnter = (index) => {
    setHoveredIndex(index);
  };

  const handleMouseLeave = () => {
    setHoveredIndex(null);
  };
  useEffect(() => {
    if (dialogOpen) {
      getAllCpSettlementPopFiles();
    }
  }, [dialogOpen]);

  const getAllCpSettlementPopFiles = async () => {
    try {
      const response = await GetAllCpsettlementPopFilesById({
        carrierPaymentSettlementId: selctedRow?.CarrierPaymentSettlementId,
      });
      const data = response.data.result;
      setItemData(data);
    } catch (error) {
      console.error("Error fetching image data:", error);
    }
  };

  const removeImage = async (cpsettlementPopFileId) => {
    try {
      const response = await DeleteCpsettlementPopFile({
        carrierPaymentSettlementId: selctedRow?.CarrierPaymentSettlementId,
        cpsettlementPopFileId: cpsettlementPopFileId,
      });
      if (response.status === 200) {
        setItemData(
          itemData.filter(
            (item) => item.cpsettlementPopFileId !== cpsettlementPopFileId
          )
        );
      } else {
        console.error("Failed to remove image:", response.data);
      }
    } catch (error) {
      console.error("Error removing image:", error);
    }
    setDeleteModalOpen(false);
  };
  const handleDeleteButtonClick = (cpsettlementPopFileId) => {
    setSelectedImageId(cpsettlementPopFileId);
    setDeleteModalOpen(true);
  };

  return (
    <>
      <ModalComponent
        open={dialogOpen}
        onClose={handleDialogClose}
        maxWidth="md"
        title={
          LanguageReducer?.languageType
            ?.ACCOUNTS_CARRIER_SETTLEMENT_VIEW_ALL_IMAGES
        }
        style={{ overflow: "hidden", height: "unset" }}
      >
        {itemData.length === 0 ? (
          <Typography variant="body1" style={{ textAlign: "center" }}>
            <h3>
              {
                LanguageReducer?.languageType
                  ?.ACCOUNTS_CARRIER_SETTLEMENT_NO_IMAGES_AVAILABLE
              }
            </h3>
          </Typography>
        ) : (
          <Grid container spacing={2}>
            {itemData.map((item, index) => (
              <Grid item xs={3} key={index}>
                <div
                  style={{
                    display: "flex",
                    justifyContent: "center",
                    position: "relative",
                    padding: 8,
                    border: "1px solid #ccc",
                    borderRadius: 8,
                    overflow: "hidden",
                    boxShadow: "0 4px 6px rgba(0, 0, 0, 0.1)",
                    cursor: "pointer",
                    height: "100%",
                    width: "100%",
                  }}
                  onMouseEnter={() => handleMouseEnter(index)}
                  onMouseLeave={() => handleMouseLeave()}
                >
                  {hoveredIndex === index && (
                    <IconButton
                      style={{
                        position: "absolute",
                        top: 8,
                        right: 8,
                        background:
                          "linear-gradient(45deg, #fcb2c2 30%, #efb9a7 90%)",
                        borderRadius: "50%",
                        color: "rgba(255, 0, 0, 0.584)",
                      }}
                      onClick={() =>
                        handleDeleteButtonClick(item.cpsettlementPopFileId)
                      }
                    >
                      <CloseIcon style={{ fontSize: "15px" }} />
                    </IconButton>
                  )}
                  {item.filePath &&
                    (item.filePath.endsWith(".pdf") ? (
                      <embed
                        src={item.filePath}
                        width="100%"
                        minHeight="100px"
                        type="application/pdf"
                      />
                    ) : (
                      <a
                        href={item.filePath}
                        target="_blank"
                        rel="noopener noreferrer"
                      >
                        <img
                          srcSet={`${item.filePath}?w=164&h=164&fit=crop&auto=format&dpr=2 2x`}
                          src={`${item.filePath}?w=164&h=164&fit=crop&auto=format`}
                          alt={`Image ${index}`}
                          loading="lazy"
                          style={{
                            objectFit: "cover",
                            width: "100%",
                            height: "100%",
                          }}
                        />
                      </a>
                    ))}
                </div>
              </Grid>
            ))}
          </Grid>
        )}
      </ModalComponent>
      {/* Delete confirmation modal */}
      <ModalComponent
        open={deleteModalOpen}
        onClose={() => setDeleteModalOpen(false)}
        maxWidth="xs"
        title="Delete Confirmation"
        style={{ overflow: "hidden", height: "unset" }}
        actionBtn={
          <ModalButtonComponent
            title={"Yes"}
            onClick={() => removeImage(selectedImageId)}
          />
        }
      >
        <Typography variant="body1" style={{ textAlign: "center" }}>
          {
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_ARE_YOU_SURE_YOU_WANT_TO_DELETE_THIS_IMAGE
          }
        </Typography>
        <Grid
          container
          justifyContent="center"
          style={{ marginTop: 16 }}
        ></Grid>
      </ModalComponent>
    </>
  );
}
