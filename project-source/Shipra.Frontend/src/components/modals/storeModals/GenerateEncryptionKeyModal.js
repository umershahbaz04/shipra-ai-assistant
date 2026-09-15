import { InputLabel } from "@mui/material";
import { purple } from "@mui/material/colors";
import { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import {
  CreateSGenerateEncryptedKeyAgainstStoreAndStationtore,
  GetAllStationLookup,
  GetStoreById,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import {
  ClipboardIcon,
  GridContainer,
  GridItem,
  placeholders,
} from "../../../utilities/helpers/Helpers";
import { successNotification } from "../../../utilities/toast";

const GenerateEncryptionKeyModal = (props) => {
  const { open, onClose, StoreID } = props;
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    setValue,
    control,
  } = useForm();
  const selectedStaion = useWatch({
    name: "Station",
    control,
  });
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [productStations, setProductStations] = useState([]);
  const [loading, setLoading] = useState({
    GenerateKeyLoading: false,
    reGenerateKeyLoading: false,
  });
  const [encryptedKeyValue, setEncryptedKeyValue] = useState("");

  const getAllStationLookup = async () => {
    try {
      const response = await GetAllStationLookup();
      if (response.data.isSuccess) {
        setProductStations(response?.data?.result);
      }
    } catch {}
  };

  const handleGenerateEncryptedKeyAgainstStoreAndStation = async () => {
    setLoading((prev) => ({ ...prev, GenerateKeyLoading: true }));
    try {
      const response =
        await CreateSGenerateEncryptedKeyAgainstStoreAndStationtore(
          StoreID,
          selectedStaion?.productStationId
        );
      if (response.data.isSuccess) {
        successNotification("Key Generate Successfully");
        setEncryptedKeyValue(response.data.result.data);
      }
    } catch {
    } finally {
      setLoading((prev) => ({ ...prev, GenerateKeyLoading: false }));
    }
  };

  useEffect(() => {
    const fetchStoreData = async () => {
      try {
        const response = await GetStoreById(StoreID);
        setEncryptedKeyValue(response?.data?.result?.encryptedKey);
        const encryptedKey = response?.data?.result?.encryptedKey;
        if (encryptedKey) {
          const base64Data = encryptedKey.split(":");
          const decodedString = atob(base64Data[1]);
          const parseDecodedString = JSON.parse(decodedString);
          const defaultStation = productStations.find(
            (dt) => dt?.productStationId === parseDecodedString?.stationId
          );
          setValue("Station", defaultStation);
        }
      } catch (e) {}
    };

    fetchStoreData();
  }, [StoreID, productStations]);

  useEffect(() => {
    getAllStationLookup();
  }, []);
  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Connection key"}
      actionBtn={
        <ModalButtonComponent
          loading={loading.GenerateKeyLoading}
          title={"Generate key"}
          bg={purple}
          onClick={handleSubmit(
            handleGenerateEncryptedKeyAgainstStoreAndStation
          )}
        />
      }
      component={"form"}
    >
      <GridContainer spacing={1}>
        <GridItem xs={11} sm={12} md={12} lg={12}>
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.ORDERS_STATIONS}
          </InputLabel>
          <SelectComponent
            name="Station"
            control={control}
            options={productStations}
            isRHF={true}
            required={true}
            optionLabel={EnumOptions.SELECT_STATION.LABEL}
            optionValue={EnumOptions.SELECT_STATION.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllStationLookup}
            {...register("Station", {
              required: {
                value: true,
              },
            })}
            value={getValues("Station")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("Station", resolvedId);
            }}
            errors={errors}
          />
        </GridItem>
        {encryptedKeyValue && (
          <GridItem xs={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {"Connection Key"}
            </InputLabel>
            <TextFieldComponent
              type={"password"}
              passwordType={true}
              placeholder={placeholders.password}
              value={encryptedKeyValue}
              name={"encryptedKey"}
              endAdornmentBtn={
                <ClipboardIcon size={20} text={encryptedKeyValue} />
              }
            />
          </GridItem>
        )}
      </GridContainer>
    </ModalComponent>
  );
};

export default GenerateEncryptionKeyModal;
