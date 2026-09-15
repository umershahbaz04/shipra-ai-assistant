import React, { useState } from "react";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { Grid, IconButton, Button, Avatar } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import AddIcon from "@mui/icons-material/Add";
import { CreateCpsettlementPopFile } from "../../../api/AxiosInterceptors";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "../../../utilities/helpers/Helpers";
import PdfIconImage from "../carrierModals/img/PDf2.png";
import FileUploadIcon from "@mui/icons-material/FileUpload";
import { useSelector } from "react-redux";

export default function CarrierSettlementPOPViewImagesModal({
  editdialogOpen,
  handleEditDialogClose,
  CarrierPaymentSettlementId,
  selctedRow,
}) {
  const [images, setImages] = useState([]);
  const [hoveredIndex, setHoveredIndex] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleMouseEnter = (index) => {
    setHoveredIndex(index);
  };

  const handleMouseLeave = () => {
    setHoveredIndex(null);
  };
  const handleImageUpload = (event) => {
    const uploadedImages = Array.from(event.target.files).map(
      (file) => file
      // URL.createObjectURL(file)
    );
    setImages((prevImages) => [...prevImages, ...uploadedImages]);
  };
  // const handleImageUpload = (event) => {
  //     const uploadedImages = Array.from(event.target.files).map((file) => {
  //       const url = URL.createObjectURL(file);
  //       return fetch(url)
  //         .then(res => res.blob())
  //         .then(blob => new File([blob], file.name, { type: file.type }));
  //     });
  //     Promise.all(uploadedImages).then(files => {
  //       setImages((prevImages) => [...prevImages, ...files]);
  //     });
  //   };

  const removeImage = (index) => {
    const newImages = [...images];
    newImages.splice(index, 1);
    setImages(newImages);
  };

  const handleSubmit = async () => {
    try {
      setIsLoading(true);
      const formData = new FormData();
      // Check if images is defined before iterating
      if (images) {
        images.forEach((file) => {
          formData.append("Files", file);
        });
      } else {
        setIsLoading(false);
        console.warn("No images to upload");
      }
      // Add additional data to the FormData
      formData.append(
        "CarrierPaymentSettlementId",
        selctedRow?.CarrierPaymentSettlementId || ""
      );

      // Make sure to handle null or undefined for selctedRow.CarrierPaymentSettlementId
      const response = await CreateCpsettlementPopFile(formData);
      handleEditDialogClose();
    } catch (error) {
      console.error("Error uploading images:", error);
    }
    setIsLoading(false);
  };

  return (
    <ModalComponent
      open={editdialogOpen}
      onClose={handleEditDialogClose}
      maxWidth="md"
      title={
        LanguageReducer?.languageType
          ?.ACCOUNTS_CARRIER_SETTLEMENT_UPLOAD_ALL_FILES
      }
      style={{ overflow: "hidden", height: "unset" }}
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.ACCOUNTS_CARRIER_SETTLEMENT_UPLOAD_FILES
          }
          bg={purple}
          loading={isLoading}
          onClick={handleSubmit}
        />
      }
    >
      <Grid container spacing={2} style={{ padding: "20px" }}>
        {images.length === 0 ? (
          <Grid item md={12} style={{ textAlign: "center" }}>
            <div
              style={{
                padding: "10px",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                borderWidth: "2px",
                borderRadius: "1rem",
                borderStyle: "dashed",
                borderColor: "#cbd5e1",
                backgroundColor: "#f8fafc",
                height: "100%",
              }}
            >
              <label htmlFor="file-upload">
                <IconButton component="span" disableRipple={true}>
                  <FileUploadIcon
                    loading={true}
                    style={{ fontSize: "80", color: "rgb(203, 213, 225)" }}
                  />
                </IconButton>
                <input
                  type="file"
                  accept="image/*, application/pdf"
                  onChange={handleImageUpload}
                  style={{ display: "none" }}
                  id="file-upload"
                  multiple
                />
              </label>
            </div>
          </Grid>
        ) : (
          <>
            {images.map((file, index) => (
              <Grid item md={3} key={index}>
                <div
                  style={{
                    display: "flex",
                    justifyContent: "center",
                    position: "relative",
                    padding: 10,
                    border: "1px solid #ccc",
                    borderRadius: 8,
                    overflow: "hidden",
                    boxShadow: "0 4px 6px rgba(0, 0, 0, 0.2)",
                    minHeight: "125px",
                    width: "100%",
                  }}
                  onMouseEnter={() => handleMouseEnter(index)}
                  onMouseLeave={() => handleMouseLeave()}
                >
                  {file.type.includes("image") ? (
                    <a
                      href={URL.createObjectURL(file)}
                      target="_blank"
                      rel="noopener noreferrer"
                    >
                      <Avatar
                        variant="rounded"
                        src={URL.createObjectURL(file)}
                        alt={`File ${index}`}
                        style={{
                          width: "100%",
                          height: "125px",
                        }}
                      />
                    </a>
                  ) : (
                    <Avatar
                      variant="rounded"
                      src={PdfIconImage}
                      style={{
                        width: "100px",
                        minHeight: "100px",
                      }}
                    ></Avatar>
                  )}
                  {hoveredIndex === index && (
                    <IconButton
                      style={{
                        position: "absolute",
                        top: 8,
                        right: 8,
                        background:
                          "linear-gradient(45deg, #FE6B8B 30%, #FF8E53 90%)",
                        borderRadius: "50%",
                        color: "red",
                      }}
                      onClick={() => removeImage(index)}
                    >
                      <CloseIcon style={{ fontSize: "15px" }} />
                    </IconButton>
                  )}
                </div>
              </Grid>
            ))}
            <Grid item md={3} style={{ textAlign: "center" }}>
              <label htmlFor="file-upload">
                <div
                  style={{
                    position: "relative",
                    padding: 8,
                    border: "1px solid #ccc",
                    borderRadius: 8,
                    overflow: "hidden",
                    boxShadow: "0 4px 6px rgba(0, 0, 0, 0.1)",
                    cursor: "pointer",
                    minHeight: "145px",
                    width: "100%",
                  }}
                >
                  <IconButton component="span" disableRipple={true}>
                    <AddIcon
                      style={{ fontSize: 40, color: "gray", marginTop: "25px" }}
                    />
                  </IconButton>
                  <input
                    type="file"
                    accept="image/*, application/pdf"
                    onChange={handleImageUpload}
                    style={{ display: "none" }}
                    id="file-upload"
                    multiple
                  />
                </div>
              </label>
            </Grid>
          </>
        )}
      </Grid>
    </ModalComponent>
  );
}
