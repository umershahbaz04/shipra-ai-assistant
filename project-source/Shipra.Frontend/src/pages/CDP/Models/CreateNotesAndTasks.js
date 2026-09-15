import { InputLabel } from "@mui/material";
import { useState } from "react";
import { useForm } from "react-hook-form";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import { CreateCDPCustomerNotesAndTasks } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import {
    GridContainer,
    GridItem,
    purple,
} from "../../../utilities/helpers/Helpers";
import {
    errorNotification,
    successNotification,
} from "../../../utilities/toast";

const NotesAndTasksOptions = [
  { label: "Note", value: 1 },
  { label: "Task", value: 2 },
];

const CreateNotesAndTasks = (props) => {
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
  const selectedType = watch("NotesAndTasks");

  const onSubmit = async (data) => {
    setLoading(true);
    const body = {
      CustomerId: customerId,
      Notes: selectedType?.value === 1 ? data.note : null,
      Tasks: selectedType?.value === 2 ? data.task : null,
    };
    try {
      const response = await CreateCDPCustomerNotesAndTasks(body);
      if (response?.data?.isSuccess) {
        successNotification("Notes and Tasks created successfully");
        reset();
        onClose();
      } else {
        errorNotification("Failed to create Notes and Tasks");
      }
    } catch (error) {
      errorNotification("An error occurred while creating Notes and Tasks");
    } finally {
      setLoading(false);
    }
  };

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Notes and Tasks"}
      actionBtn={
        <ModalButtonComponent
          title={"Add Notes and Tasks"}
          loading={loading}
          bg={purple}
          onClick={handleSubmit(onSubmit)}
        />
      }
    >
      <GridContainer spacing={2}>
        <GridItem xs={12} sm={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {"Notes and Tasks"}
          </InputLabel>
          <SelectComponent
            name="NotesAndTasks"
            control={control}
            options={NotesAndTasksOptions}
            isRHF
            required
            optionLabel="label"
            optionValue="value"
            onChange={(e, val) => {
              setValue("NotesAndTasks", val);
              setValue("note", "");
              setValue("task", "");
            }}
            errors={errors}
          />
        </GridItem>
        {selectedType?.value === 1 && (
          <GridItem xs={12} sm={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {"Note"}
            </InputLabel>

            <TextFieldComponent
              isRHF
              name="note"
              placeholder="Enter Note"
              value={watch("note") || ""}
              onChange={(e) => {
                setValue("note", e.target.value, {
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
              {"Task"}
            </InputLabel>

            <TextFieldComponent
              isRHF
              name="task"
              placeholder="Enter Task"
              value={watch("task") || ""}
              onChange={(e) => {
                setValue("task", e.target.value, {
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

export default CreateNotesAndTasks;
