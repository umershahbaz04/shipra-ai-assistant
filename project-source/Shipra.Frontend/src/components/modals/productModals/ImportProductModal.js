import React, { useRef, useState } from "react";
import { useSelector } from "react-redux";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { purple } from "@mui/material/colors";
import { errorNotification } from "../../../utilities/toast";
import { fetchMethod } from "../../../utilities/helpers/Helpers";
import { UploadProducts } from "../../../api/AxiosInterceptors";

const ImportProductModal = (props) => {
  const { open, onClose, setProductData, setErrorsList, handleResetFields } =
    props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [xlsxFile, setXlsxFile] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const fileInputRef = useRef(null);

  const handleFileUpload = (event) => {
    const file = event.target.files[0];
    if (
      file &&
      file.type ===
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ) {
      setXlsxFile(file);
      // File is a valid .xlsx file, you can proceed with further operations
    } else {
      errorNotification(LanguageReducer?.languageType?.INVALID_FILE_TYPE_TOAST);
      // Invalid file format, display an error message or handle accordingly
    }
  };
  const handleUploadFile = async () => {
    if (!xlsxFile) {
      errorNotification("No file selected for upload.");
      return;
    }
    handleResetFields();
    setIsLoading(true);
    const formData = new FormData();
    formData.append("File", xlsxFile);

    try {
      const { response } = await fetchMethod(() => UploadProducts(formData));

      if (response?.result) {
        const finalResult = response.result.map((row, index) => {
          const errors_array_json = response.errors?.InvalidParameter;
          const parsed_errors_array = errors_array_json
            ? JSON.parse(errors_array_json)
            : [];
          const selectedIndexRow = parsed_errors_array?.find(
            (dt) => dt.Row === index + 1
          );
          const hasError = selectedIndexRow
            ? !selectedIndexRow?.IsSuccessed
            : false;
          const errorMsg = selectedIndexRow?.Msg;
          return {
            ...row,
            rowNum: index,
            hasError,
            errorMsg,
          };
        });
        setProductData(finalResult);
        setErrorsList(response?.errors);
      } else {
        errorNotification("No result data in response.");
      }
    } catch (error) {
      errorNotification("File upload failed:", error);
    } finally {
      onClose();
      setIsLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Upload File"}
      actionBtn={
        <ModalButtonComponent
          title={"Upload Product"}
          bg={purple}
          type="submit"
          loading={isLoading}
          onClick={handleUploadFile}
        />
      }
    >
      <input
        style={{ paddingTop: "20px" }}
        type="file"
        accept=".xlsx"
        onChange={handleFileUpload}
        ref={fileInputRef}
      />
    </ModalComponent>
  );
};

export default ImportProductModal;
