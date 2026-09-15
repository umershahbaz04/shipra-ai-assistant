import { Grid, InputLabel } from "@mui/material";
import { useMemo, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { UpdatePermissionActions } from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import { purple } from "../../../utilities/helpers/Helpers";
import { successNotification } from "../../../utilities/toast";
import UtilityClass from "../../../utilities/UtilityClass";

const UpdatePermissionAction = (props) => {
  const { open, onClose, allPermissionAction, allPermissionGroupLookup } =
    props;
  const [loading, setLoading] = useState(false);
  const [selectedController, setSelectedController] = useState();
  const [selectedPermissionAction, setSelectedPermissionAction] = useState();
  const [selectedPermissionGroup, setSelectedPermissionGroup] = useState();

  console.log({
    selectedController,
    selectedPermissionAction,
    selectedPermissionGroup,
  });

  const controllerOptions = useMemo(() => {
    const unique = new Map();

    allPermissionAction?.forEach((item) => {
      if (!unique.has(item.controllerName)) {
        unique.set(item.controllerName, item);
      }
    });

    return Array.from(unique.values());
  }, [allPermissionAction]);

  const permissionActionOptions = useMemo(() => {
    if (!selectedController) return [];
    return allPermissionAction?.filter(
      (x) => x.controllerName === selectedController.controllerName,
    );
  }, [selectedController, allPermissionAction]);

  const updatePermissionAction = async () => {
    setLoading(true);

    const body = {
      controllerName: selectedController?.controllerName,
      actions: selectedPermissionAction.map((x) => x.actionName),
      permissionGroupId: selectedPermissionGroup?.permissionGroupId,
    };

    try {
      const response = await UpdatePermissionActions(body);

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
      title={"Update Permission Action"}
      actionBtn={
        <ModalButtonComponent
          loading={loading}
          onClick={updatePermissionAction}
          disabled={loading}
          title={"Update"}
          bg={purple}
        />
      }
    >
      <Grid container spacing={2}>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>{"Controller"}</InputLabel>
          <SelectComponent
            name="controller"
            options={controllerOptions}
            value={selectedController}
            height={40}
            optionLabel={EnumOptions.CONTROLLER_NAME.LABEL}
            optionValue={EnumOptions.CONTROLLER_NAME.VALUE}
            onChange={(e, newValue) => {
              setSelectedController(newValue || null);
              setSelectedPermissionAction(null);
            }}
          />
        </Grid>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {"Permission Action"}
          </InputLabel>
          <SelectComponent
            name="permissionAction"
            options={permissionActionOptions}
            value={selectedPermissionAction}
            height={40}
            multiple={true}
            optionLabel={EnumOptions.PERMISSION_ACTION.LABEL}
            optionValue={EnumOptions.PERMISSION_ACTION.VALUE}
            onChange={(e, newValue) => {
              setSelectedPermissionAction(newValue || null);
            }}
          />
        </Grid>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>
            {"Permission Group"}
          </InputLabel>
          <SelectComponent
            name="permissionGroup"
            options={allPermissionGroupLookup}
            value={selectedPermissionGroup}
            height={40}
            optionLabel={EnumOptions.PERMISSION_GROUP.LABEL}
            optionValue={EnumOptions.PERMISSION_GROUP.VALUE}
            onChange={(e, newValue) => {
              setSelectedPermissionGroup(newValue || null);
            }}
          />
        </Grid>
      </Grid>
    </ModalComponent>
  );
};

export default UpdatePermissionAction;
