import { useState } from "react";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { CreateCDPCustomerSegmentsAndLabels } from "../../../api/AxiosInterceptors";
import {
  GridContainer,
  GridItem,
  purple,
} from "../../../utilities/helpers/Helpers";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import { useForm } from "react-hook-form";
import { InputLabel } from "@mui/material";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import { styleSheet } from "../../../assets/styles/style";

const segmentsAndLabelsOptions = [
  { label: "Segment", value: 1 },
  { label: "Label", value: 2 },
];

const CreateSegmentsAndLabels = (props) => {
  const { open, onClose, customerId } = props;

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    control,
    watch,
    reset,
    getValues,
  } = useForm();

  const [loading, setLoading] = useState(false);
  const selectedType = watch("segmentsAndLabels");

  const onSubmit = async (data) => {
    setLoading(true);
    const body = {
      CustomerId: customerId,
      Segments: selectedType?.value === 1 ? data.segmentName : null,
      Labels: selectedType?.value === 2 ? data.labelName : null,
    };
    try {
      const response = await CreateCDPCustomerSegmentsAndLabels(body);
      if (response?.data?.isSuccess) {
        successNotification("Segments and Labels created successfully");
        reset();
        onClose();
      } else {
        errorNotification("Failed to create Segments and Labels");
      }
    } catch (error) {
      errorNotification("An error occurred while creating Segments and Labels");
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Segments and Labels"}
      actionBtn={
        <ModalButtonComponent
          title={"Add Segments and Labels"}
          loading={loading}
          bg={purple}
          onClick={handleSubmit(onSubmit)}
        />
      }
    >
      <GridContainer spacing={2}>
        <GridItem xs={12} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {"Segments and Labels"}
          </InputLabel>
          <SelectComponent
            name="segmentsAndLabels"
            control={control}
            options={segmentsAndLabelsOptions}
            isRHF
            required
            optionLabel="label"
            optionValue="value"
            onChange={(e, val) => {
              setValue("segmentsAndLabels", val);
              setValue("segmentName", "");
              setValue("labelName", "");
            }}
            errors={errors}
          />
        </GridItem>
        {selectedType?.value === 1 && (
          <GridItem xs={12} sm={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {"Segment Name"}
            </InputLabel>

            <TextFieldComponent
              isRHF
              name="segmentName"
              placeholder="Enter Segment"
              value={watch("segmentName") || ""}
              onChange={(e) => {
                setValue("segmentName", e.target.value, {
                  shouldValidate: true,
                });
              }}
              errors={errors}
            />
          </GridItem>
        )}
        {selectedType?.value === 2 && (
          <GridItem xs={12} sm={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {"Label Name"}
            </InputLabel>

            <TextFieldComponent
              isRHF
              name="labelName"
              placeholder="Enter Label"
              value={watch("labelName") || ""}
              onChange={(e) => {
                setValue("labelName", e.target.value, {
                  shouldValidate: true,
                });
              }}
              errors={errors}
            />
          </GridItem>
        )}
      </GridContainer>
    </ModalComponent>
  );
};

export default CreateSegmentsAndLabels;
