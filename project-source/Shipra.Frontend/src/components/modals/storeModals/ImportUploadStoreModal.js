import { purple } from "@mui/material/colors";
import { useRef, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { errorNotification } from "../../../utilities/toast";
import { UploadStoreFile } from "../../../api/AxiosInterceptors";

const ImportUploadStoreModal = (props) => {
  const { open, onClose, setUploadStoreData, countryId } = props;
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
    } else {
      errorNotification(LanguageReducer?.languageType?.INVALID_FILE_TYPE_TOAST);
    }
  };

  const handleSubmit = async () => {
    if (!xlsxFile) {
      errorNotification("No file selected for upload.");
      return;
    }
    setIsLoading(true);
    const formData = new FormData();
    formData.append("File", xlsxFile);
    formData.append("CountryId", countryId);
    try {
      const response = await UploadStoreFile(formData);
      if (response?.data?.result?.length > 0) {
        const finalResult = response?.data?.result?.map((row, index) => {
          const errors_array_json = response?.data?.errors?.InvalidParameter;
          const parsed_errors_array = errors_array_json
            ? JSON.parse(errors_array_json)
            : [];
          const selectedIndexRow = parsed_errors_array?.find(
            (dt) => dt.Row === index + 1,
          );
          const hasError = selectedIndexRow
            ? !selectedIndexRow?.IsSuccessed
            : false;
          const errorMsg = selectedIndexRow?.Msg;
          return {
            ...row,
            hasError,
            errorMsg,
          };
        });
        setUploadStoreData(finalResult);
      }
    } catch (e) {
    } finally {
      setIsLoading(false);
      onClose();
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
          title={"Upload Store"}
          bg={purple}
          type="submit"
          loading={isLoading}
          onClick={handleSubmit}
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

export default ImportUploadStoreModal;
