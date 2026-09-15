import {
  Checkbox,
  FormControlLabel,
  Grid,
  InputLabel,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import {
  CreateProductStation,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import UtilityClass from "../../../utilities/UtilityClass";
import { placeholders, purple } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function AddProductStationModal(props) {
  const [isLoading, setIsLoading] = useState(false);
  let { open, setOpen, carrierId, getAll } = props;
  const [allStores, setAllStores] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const [productStationName, setProductStationName] = useState("");
  const [isDefault, setIsDefault] = useState(false);

  const handleClose = () => {
    setOpen(false);
  };
  let getAllStores = async () => {
    let res = await GetStoresForSelection();
    if (res.data.result !== null) {
      setAllStores(res.data.result);
    }
  };

  useEffect(() => {
    getAllStores();
  }, []);
  const handleConnect = async () => {
    setIsLoading(true);
    try {
      if (productStationName == "") {
        errorNotification(`Please enter station name`);
        return false;
      }
      let body = {
        name: productStationName,
        isDefault: isDefault,
      };
      console.log("body", body);
      const response = await CreateProductStation(body);

      if (!response.data.isSuccess) {
        let jsonData = response.data.errors;
        UtilityClass.showErrorNotificationWithDictionary(jsonData);
      } else {
        successNotification("Product station created successfully");
        handleClose();
        getAll(true);
      }
    } catch (error) {
      console.error("Error to connect", error.response);
      errorNotification("Unable to create");
    } finally {
      setIsLoading(false);
    }
  };

  const handleFocus = (event) => event.target.select();

  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      title={
        LanguageReducer?.languageType
          ?.PRODUCT_PRODUCT_STATIONS_ADD_PRODUCT_STATIONS
      }
      actionBtn={
        <ModalButtonComponent
          title={
            LanguageReducer?.languageType
              ?.PRODUCT_PRODUCT_STATIONS_ADD_PRODUCT_STATIONS
          }
          loading={isLoading}
          bg={purple}
          onClick={(e) => handleConnect()}
        />
      }
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={12} sm={12} xs={12}>
          <InputLabel sx={styleSheet.inputLabel} required>
            {
              LanguageReducer?.languageType
                ?.PRODUCT_PRODUCT_STATIONS_STATION_NAME
            }
          </InputLabel>
          <TextField
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
              defaultChecked={isDefault}
            />
          }
          label={
            LanguageReducer?.languageType
              ?.PRODUCT_PRODUCT_STATIONS_MARK_AS_DEFAULT
          }
        />
      </Grid>
    </ModalComponent>
  );
}
export default AddProductStationModal;
