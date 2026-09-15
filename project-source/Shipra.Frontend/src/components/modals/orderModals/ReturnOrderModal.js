import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import {
  GetAllReturnStatusForSelection,
  UpdateReturnStatus,
} from "../../../api/AxiosInterceptors";
import { EnumOptions, EnumReturnStatus } from "../../../utilities/enum";
import {
  fetchMethod,
  GridContainer,
  GridItem,
} from "../../../utilities/helpers/Helpers";
import UtilityClass from "../../../utilities/UtilityClass";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
const initialSmsFieldsData = {
  comment: "",
};
const ReturnOrderModal = (props) => {
  const { open, onClose, rowData, getAllOrderReturn } = props;
  const [returnComment, setReturnComment] = useState(initialSmsFieldsData);
  const [loading, setLoading] = useState(false);
  const [allReturnStatus, setAllReturnStatus] = useState([]);
  const [selectedReturnStatus, setSelectedReturnStatus] = useState();
  const [openDelete, setOpenDelete] = useState(false);
  const [returnStatusApprove, setReturnStatusApprove] = useState(false);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleChange = (e) => {
    const { name, value } = e.target;
    setReturnComment((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };
  const getAllReturnStatusForSelection = async () => {
    try {
      const { response } = await fetchMethod(GetAllReturnStatusForSelection);
      if (response?.result) {
        setAllReturnStatus(response?.result);
      } else {
        console.log("No data found");
      }
    } catch (error) {
      console.error("Error fetching order returns:", error);
    }
  };
  const handleCheckStaus = () => {
    if (
      rowData?.ReturnStatusId === EnumReturnStatus.APPROVED.VALUE &&
      selectedReturnStatus?.returnStatusLookupId !==
        EnumReturnStatus.APPROVED.VALUE
    ) {
      setReturnStatusApprove(true);
      setOpenDelete(true);
    }
  };
  const handleApproved = async () => {
    if (returnStatusApprove) {
      setLoading(true);
      await UpdateReturnStatus(
        rowData?.ReturnId,
        selectedReturnStatus?.returnStatusLookupId,
        returnComment.comment
      )
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
          } else {
            successNotification("Status Update successfully.");
          }
        })
        .catch((e) => {})
        .finally(() => {
          setReturnComment(initialSmsFieldsData);
          setSelectedReturnStatus();
          setLoading(false);
          getAllOrderReturn();
          onClose();
        });
      setOpenDelete(false);
    }
  };
  const handleUpdateStatus = async () => {
    if (
      !selectedReturnStatus ||
      (typeof selectedReturnStatus === "object" &&
        Object.keys(selectedReturnStatus).length === 0) ||
      selectedReturnStatus?.returnStatusLookupId === 0
    ) {
      errorNotification("Please Select Status");
      return;
    }
    handleCheckStaus();
    if (rowData?.ReturnStatusId !== EnumReturnStatus.APPROVED.VALUE) {
      setLoading(true);
      await UpdateReturnStatus(
        rowData?.ReturnId,
        selectedReturnStatus?.returnStatusLookupId,
        returnComment.comment
      )
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
          } else {
            successNotification("Status Update successfully.");
          }
        })
        .catch((e) => {})
        .finally(() => {
          setReturnComment(initialSmsFieldsData);
          setSelectedReturnStatus();
          setLoading(false);
          getAllOrderReturn();
          onClose();
        });
    }
  };
  useEffect(() => {
    const defaultValue = allReturnStatus.find(
      (returnStatus) =>
        returnStatus?.returnStatusLookupId === rowData?.ReturnStatusId
    );
    setSelectedReturnStatus(defaultValue);
    setReturnComment((prev) => ({
      ...prev,
      comment: rowData?.ReturnComment,
    }));
  }, [allReturnStatus, rowData]);

  useEffect(() => {
    getAllReturnStatusForSelection();
  }, []);
  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="sm"
        title={LanguageReducer?.languageType?.ORDER_UPDATE_STATUS}
        actionBtn={
          <ModalButtonComponent
            title={LanguageReducer?.languageType?.ORDER_UPDATE_STATUS}
            bg={purple}
            loading={loading}
            onClick={(e) => handleUpdateStatus()}
          />
        }
      >
        <GridContainer spacing={1}>
          <GridItem sm={12} md={12} lg={12} xs={12}>
            <TextFieldLableComponent
              title={LanguageReducer?.languageType?.ORDER_RETURN_STATUS}
            />
            <SelectComponent
              name="returnStatus"
              options={allReturnStatus}
              value={selectedReturnStatus}
              optionLabel={EnumOptions.RETURN_STATUS.LABEL}
              optionValue={EnumOptions.RETURN_STATUS.VALUE}
              isRefesh={true}
              handleRefreshClick={getAllReturnStatusForSelection}
              onChange={(e, val) => {
                setSelectedReturnStatus(val);
              }}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12} xs={12}>
            <TextFieldLableComponent
              required={false}
              title={LanguageReducer?.languageType?.ORDER_COMMENT}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter Comment"}
              value={returnComment.comment}
              name="comment"
              onChange={handleChange}
            />
          </GridItem>
        </GridContainer>
        {openDelete && (
          <DeleteConfirmationModal
            open={openDelete}
            setOpen={setOpenDelete}
            handleDelete={handleApproved}
            heading={
              LanguageReducer?.languageType
                ?.ORDER_RETURN_ORDER_ARE_YOU_SURE_YOU_WANT_TO_CHANGE_THIS_RETURN_STATUS
            }
            message={
              LanguageReducer?.languageType
                ?.ORDER_RETURN_ORDER_SELECTED_ORDERS_STATUS_WILL_BE_CHANGE_IMMEDIATELY_YOU_CAN_UNDO_THIS_ACTION
            }
            buttonText={"yes"}
          />
        )}
      </ModalComponent>
    </>
  );
};

export default ReturnOrderModal;
