import CloudUploadIcon from "@mui/icons-material/CloudUpload";
import {
  Box,
  Card,
  Checkbox,
  FormControlLabel,
  Grid,
  Typography,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ImageListComponent from "../../../.reUseableComponents/ImageList/Image";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import {
  CreateImageGallery,
  DeleteProductMediaById,
  GetAllImageGalleries,
} from "../../../api/AxiosInterceptors";
import {
  CustomColorLabelledOutline,
  ImageWithSkeleton,
} from "../../../utilities/helpers/Helpers";
import { successNotification } from "../../../utilities/toast";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal";

const ImageGalleryModal = (props) => {
  const {
    open,
    onClose,
    loading,
    uploadedImages,
    setUploadedImages,
    varientIndex,
    variantIndexFlag,
    setVariantIndexFlag,
    setCombinations,
    groupedCombination,
  } = props;
  const [selectedGalleryImage, setSelectedGalleryImage] = useState([]);
  const [allGalleryImage, setallGalleryImage] = useState([]);
  const [openDeleteImageModel, setOpenDeleteImageModel] = useState({
    open: false,
    productMediaId: null,
  });
  const [deleteImageModelLoading, setDeleteImageModelLoading] = useState(false);

  const getAllImageGalleries = async () => {
    try {
      const response = await GetAllImageGalleries();
      if (response?.data?.isSuccess) {
        setallGalleryImage(response?.data.result);
      }
    } catch (e) {}
  };

  const handleToggle = (image) => {
    const exists = selectedGalleryImage.some(
      (img) => img.imageGalleryId === image.imageGalleryId
    );

    if (variantIndexFlag) {
      if (exists) {
        setSelectedGalleryImage([]);
      } else {
        setSelectedGalleryImage([image]);
      }
    } else {
      if (exists) {
        setSelectedGalleryImage((prev) =>
          prev.filter((img) => img.imageGalleryId !== image.imageGalleryId)
        );
      } else {
        setSelectedGalleryImage((prev) => [...prev, image]);
      }
    }
  };

  const handleDeleteProductMediaById = async () => {
    if (!openDeleteImageModel.productMediaId.productMediaId) return;
    setDeleteImageModelLoading(true);
    try {
      const response = await DeleteProductMediaById(
        openDeleteImageModel.productMediaId.productMediaId
      );
      if (response.data.isSuccess) {
        successNotification("Image Delete Successfully");
        setUploadedImages((prev) =>
          prev.filter(
            (img) =>
              img.productMediaId !==
              openDeleteImageModel.productMediaId.productMediaId
          )
        );
      }
      setSelectedGalleryImage((prev) =>
        prev.filter(
          (img) =>
            img.imageGalleryId !==
            openDeleteImageModel.productMediaId.imageGalleryId
        )
      );
    } catch (error) {
      console.error(error);
    } finally {
      setDeleteImageModelLoading(false);
      setOpenDeleteImageModel({ open: false, productMediaId: null });
    }
  };

  const handleRemoveImage = (index) => {
    const image = uploadedImages[index];

    if (image?.productMediaId) {
      setOpenDeleteImageModel({
        open: true,
        productMediaId: image,
      });
    } else {
      const updatedImages = [...selectedGalleryImage];
      const updatedImagesMain = [...uploadedImages];
      updatedImages.splice(index, 1);
      setSelectedGalleryImage(updatedImages);
      updatedImagesMain.splice(index, 1);
      setUploadedImages(updatedImagesMain);
    }
  };

  const handleFileUpload = async (event) => {
    const files = event.target.files;
    const newFiles = [];

    for (let i = 0; i < files.length; i++) {
      newFiles.push({
        img: URL.createObjectURL(files[i]),
        title: files[i].name,
        file: files[i],
      });
    }

    const formData = new FormData();

    newFiles.forEach((item, index) => {
      formData.append(`List[${index}].File`, item.file);
      formData.append(`List[${index}].Description`, "");
    });

    if (newFiles.length > 0) {
      try {
        const response = await CreateImageGallery(formData);
        if (response.data.isSuccess) {
          if (variantIndexFlag) {
            setSelectedGalleryImage(response.data.result);
          } else {
            setSelectedGalleryImage((prev) => [
              ...prev,
              ...response.data.result,
            ]);
          }
          successNotification("Image Upload Successfully");
          getAllImageGalleries();
        }
      } catch (e) {}
    }
  };
  const handleReorder = (newImageList) => {
    setSelectedGalleryImage(newImageList);
  };

  const handleSaveImage = () => {
    if (variantIndexFlag) {
      setCombinations((prev) =>
        prev.map((item) =>
          item.SKU === groupedCombination[varientIndex].SKU
            ? {
                ...item,
                imageUrl: selectedGalleryImage[0]?.imageUrl || "",
                imageGalleryId: selectedGalleryImage[0]?.imageGalleryId,
              }
            : item
        )
      );
      setVariantIndexFlag(false);
    } else {
      const existingIds = uploadedImages.map((img) => img.imageGalleryId);
      const newImages = selectedGalleryImage.filter(
        (img) => !existingIds.includes(img.imageGalleryId)
      );

      setUploadedImages((prev) => [...prev, ...newImages]);
    }
    onClose();
  };

  useEffect(() => {
    if (variantIndexFlag) {
      const matchedObject = groupedCombination.filter(
        (_, index) => index === varientIndex
      );
      const uploadedIds = matchedObject.map((img) => img.imageGalleryId);
      const defaultSeletedImage = allGalleryImage.filter((img) =>
        uploadedIds.includes(img.imageGalleryId)
      );
      const newImagesToAdd = defaultSeletedImage.filter(
        (img) =>
          !selectedGalleryImage.some(
            (selected) => selected.imageGalleryId === img.imageGalleryId
          )
      );
      if (newImagesToAdd.length > 0 && selectedGalleryImage.length === 0) {
        setSelectedGalleryImage((prev) => [...prev, ...newImagesToAdd]);
      }
    } else {
      const uploadedIds = uploadedImages.map((img) => img.imageGalleryId);
      const defaultSelectedImages = allGalleryImage.filter((img) =>
        uploadedIds.includes(img.imageGalleryId)
      );
      setSelectedGalleryImage((prev) => [
        ...prev,
        ...defaultSelectedImages.filter(
          (img) => !prev.find((p) => p.imageGalleryId === img.imageGalleryId)
        ),
      ]);
    }
  }, [uploadedImages, allGalleryImage]);

  useEffect(() => {
    getAllImageGalleries();
  }, []);

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="lg"
      title={"Assign products to Store"}
      actionBtn={
        <ModalButtonComponent
          loading={loading}
          title={"Save"}
          bg={purple}
          onClick={handleSaveImage}
        />
      }
    >
      <Grid container spacing={2}>
        <Grid item xs={12}>
          {/* Hidden file input */}
          <input
            type="file"
            id="upload-image"
            accept="image/jpeg, image/png, image/gif, image/heic, image/webp"
            style={{ display: "none" }}
            onChange={handleFileUpload}
            multiple={variantIndexFlag ? false : true}
          />

          {/* Label acts as clickable area for file input */}
          <Box
            sx={{
              border: "2px dashed #ccc",
              borderRadius: 2,
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              justifyContent: "center",
              cursor: "pointer",
              px: 1.25,
              "&:hover": {
                borderColor: "#1976d2",
              },
            }}
          >
            {selectedGalleryImage.length > 0 ? (
              <ImageListComponent
                itemData={selectedGalleryImage}
                cols={4}
                handleRemoveImage={handleRemoveImage}
                onChange={handleFileUpload}
                onReorder={handleReorder}
                multiple={variantIndexFlag ? false : true}
              />
            ) : (
              <label htmlFor="upload-image" style={{ width: "100%" }}>
                <Box textAlign="center">
                  <CloudUploadIcon
                    sx={{ fontSize: 40, color: "#1976d2", mb: 1 }}
                  />
                  <Typography
                    sx={{
                      color: "grey",
                      fontSize: "20px",
                      textAlign: "center",
                    }}
                  >
                    Upload Image
                  </Typography>
                </Box>
              </label>
            )}
          </Box>
        </Grid>
      </Grid>
      <CustomColorLabelledOutline
        isCollapse={true}
        label={"Gallery"}
        height={"100%"}
      >
        <Grid container spacing={2}>
          {allGalleryImage.map((image) => {
            const isChecked = selectedGalleryImage.some(
              (img) => img.imageGalleryId === image.imageGalleryId
            );

            return (
              <Grid
                item
                key={image.imageGalleryId}
                xs={12}
                sm={6}
                md={3}
                lg={3}
              >
                <Card
                  sx={{
                    width: "auto",
                    height: 200,
                    position: "relative",
                    border: isChecked ? "2px solid #1976d2" : "1px solid #ccc",
                    boxShadow: isChecked ? 4 : 1,
                    borderRadius: 2,
                    overflow: "hidden",
                  }}
                >
                  <ImageWithSkeleton
                    imageUrl={image.imageUrl}
                    alt={image.fileName}
                  />
                  <FormControlLabel
                    control={
                      <Checkbox
                        checked={isChecked}
                        onChange={() => handleToggle(image)}
                        sx={{
                          position: "absolute",
                          top: 8,
                          left: 8,
                          backgroundColor: "rgba(255,255,255,0.7)",
                          borderRadius: "50%",
                        }}
                      />
                    }
                    label=""
                  />
                </Card>
              </Grid>
            );
          })}
        </Grid>
      </CustomColorLabelledOutline>
      {openDeleteImageModel.open && (
        <DeleteConfirmationModal
          open={openDeleteImageModel.open}
          setOpen={() =>
            setOpenDeleteImageModel({ open: false, productMediaId: null })
          }
          handleDelete={handleDeleteProductMediaById}
          loading={deleteImageModelLoading}
          heading="Are you sure you want to delete this image"
          message="This image will be permanently removed from this product and cannot be recovered."
          buttonText="Delete"
        />
      )}
    </ModalComponent>
  );
};

export default ImageGalleryModal;
