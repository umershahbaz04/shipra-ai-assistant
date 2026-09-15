import { LoadingButton } from "@mui/lab";
import {
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  FormControlLabel,
  Grid,
  InputLabel,
  Switch,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import {
  ActiveProductStationById,
  DeleteProductStationById,
  GetProductStationById,
  UpdateProductStation,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { placeholders, purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function UpdateProductStationModal(props) {
  const [isLoading, setIsLoading] = useState(false);
  let { open, setOpen, getAll, stationData, setStationData } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const [productStationName, setProductStationName] = useState("");
  const [isDefault, setIsDefault] = useState(false);
  const [isActiveChecked, setIsActiveChecked] = useState(false);
  const getStationById = () => {
    GetProductStationById(stationData.productStationId)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res?.errors);
        } else {
          setStationData(res?.data?.result);
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Something went wrong");
      })
      .finally(() => {});
  };

  const handleDelete = async () => {
    try {
      let param = {
        productStationId: stationData.productStationId,
      };
      const response = await DeleteProductStationById(param);
      if (response.data?.isSuccess) {
        getAll();
        getStationById();
        successNotification("Station deactivated successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
    }
  };
  const handleActive = async () => {
    try {
      let param = {
        productStationId: stationData.productStationId,
      };
      const response = await ActiveProductStationById(param);
      if (response.data?.isSuccess) {
        getAll();
        getStationById();
        successNotification("Station activated successfully");
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
    } finally {
    }
  };

  const handleClose = () => {
    setOpen(false);
  };

  const handleUpdate = async () => {
    setIsLoading(true);
    try {
      if (productStationName == "") {
        errorNotification(`Please enter station name`);
        return false;
      }
      let body = {
        productStationId: stationData?.productStationId,
        name: productStationName,
        isDefault: isDefault,
      };
      const response = await UpdateProductStation(body);
      if (!response.data.isSuccess) {
        let jsonData = response.data.errors;
        UtilityClass.showErrorNotificationWithDictionary(jsonData);
      } else {
        successNotification("Product station updated successfully");
        handleClose();
        getAll(true);
      }
    } catch (error) {
      console.error("Error to connect", error.response);
      errorNotification("Unable to updated");
    } finally {
      setIsLoading(false);
    }
  };
  const handelAction = (isActiveClicked) => {
    if (isActiveClicked) {
      handleActive();
    } else {
      handleDelete();
    }
  };
  const handleActiveInActive = (e) => {
    handelAction(!isActiveChecked);
    setIsActiveChecked(!isActiveChecked);
  };
  useEffect(() => {
    if (stationData) {
      setProductStationName(stationData?.name);
      setIsActiveChecked(stationData?.active);
      setIsDefault(stationData?.isDefault);
    }
  }, []);
  const handleFocus = (event) => event.target.select();

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={"Update Product Station"}
      actionBtn={
        stationData?.active && (
          <ModalButtonComponent
            title={"Update Product Station"}
            loading={isLoading}
            bg={purple}
            onClick={(e) => handleUpdate()}
          />
        )
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel} required>
            Station Name
          </InputLabel>
          <TextField
            disabled={!stationData?.active}
            placeholder={placeholders.station_name}
            size="small"
            fullWidth
            variant="outlined"
            value={productStationName}
            onChange={(e) => setProductStationName(e.target.value)}
          />
        </Grid>
      </Grid>

      <Grid item md={12} sm={12} sx={{ textAlign: "right" }}>
        <FormControlLabel
          // sx={{ margin: "3px 0px" }}
          control={
            <Checkbox
              disabled={!stationData?.active}
              sx={{
                color: "var(--primary-color)",
                "&.Mui-checked": {
                  color: "var(--primary-color)",
                },
              }}
              checked={isDefault}
              onChange={(e) => setIsDefault(e.target.checked)}
              edge="start"
              disableRipple
              defaultChecked
            />
          }
          label={"Mark as default"}
        />
        {!isDefault && (
          <FormControlLabel
            control={
              <Switch
                disabled={stationData?.isDefault}
                checked={isActiveChecked}
                defaultChecked={isActiveChecked}
                onClick={() => {
                  handleActiveInActive();
                }}
              />
            }
            label={isActiveChecked ? "Active" : "In Active"}
          />
        )}
      </Grid>
    </ModalComponent>
  );
}
export default UpdateProductStationModal;
