import { Grid, InputLabel, TextField } from "@mui/material";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { CreatePermissionGroups } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";
import { useState } from "react";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";

const groupOptions = [
  { id: 1, name: "Create" },
  { id: 2, name: "View" },
  { id: 3, name: "Edit" },
  { id: 4, name: "Delete" },
  { id: 5, name: "Action" },
];

const CreatePermissionGroup = (props) => {
  const { open, onClose } = props;
  const [groupName, setGroupName] = useState(groupOptions);
  const [groupParent, setGroupParent] = useState("");
  const [loading, setLoading] = useState(false);
  const [selectedGroupName, setSelectedGroupName] = useState();

  const updatePermissionAction = async () => {
    if (groupName === "") {
      errorNotification("Please Enter Group Name");
      return;
    }
    if (groupParent === "") {
      errorNotification("Please Enter Group Parent");
      return;
    }
    setLoading(true);

    const body = {
      GroupName: selectedGroupName.map((item) => item.name).join(","),
      GroupParent: groupParent,
    };

    try {
      const response = await CreatePermissionGroups(body);

      if (response?.data?.isSuccess) {
        successNotification("Permission Update Successfully");
        onClose();
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

  return (
    <ModalComponent
      maxWidth="sm"
      open={open}
      onClose={onClose}
      title={"Create Permission Group"}
      actionBtn={
        <ModalButtonComponent
          loading={loading}
          onClick={updatePermissionAction}
          disabled={loading}
          title={"Create"}
          bg={purple}
        />
      }
    >
      <Grid container spacing={2}>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>{"Group Name"}</InputLabel>
          <SelectComponent
            name="groupName"
            options={groupName}
            value={selectedGroupName}
            height={40}
            multiple={true}
            optionLabel="name"
            optionValue="name"
            onChange={(e, newValue) => {
              setSelectedGroupName(newValue || []);
            }}
          />
        </Grid>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>{"Group Parent"}</InputLabel>
          <TextField
            fullWidth
            size="small"
            placeholder="e.g order,product"
            value={groupParent}
            onChange={(e) => setGroupParent(e.target.value)}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
};

export default CreatePermissionGroup;
