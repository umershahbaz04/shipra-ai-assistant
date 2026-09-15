import EmailIcon from "@mui/icons-material/Email";
import PersonIcon from "@mui/icons-material/Person";
import { Box, InputLabel, Paper, TextField, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useDispatch } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import { inputTypesEnum } from "../../../.reUseableComponents/Modal/ConfigSettingModal.js";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent.js";
import {
  GetClientProfile,
  UpdateClient,
  UpdateClientImage,
} from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";
import { changeProfileImg } from "../../../redux/changeProfileImg/index.js";
import { getThisKeyCookie, setThisKeyCookie } from "../../../utilities/cookies";
import { EnumOptions } from "../../../utilities/enum/index.js";
import {
  CicrlesLoading,
  GridContainer,
  GridItem,
  LoadingTextField,
  PageMainBox,
  UploadProfile,
  fetchMethod,
  fetchMethodResponse,
  getLowerCase,
  grey,
  handleFocus,
  handleGetAllRegionTimeZone,
  handleGetAndSetTimeZone,
  placeholders,
  useGetAllCountries,
  useGetAllRegionTimeZone,
  useIsUserProfileChange,
  useLanguageReducer,
  usePhoneInput,
} from "../../../utilities/helpers/Helpers";
import {
  SchemaTextField,
  addressSchemaEnum,
  useGetAddressSchema,
} from "../../../utilities/helpers/addressSchema.js";
import CountrySchema from "../../../utilities/helpers/countryschema.js";

export const ProfileDetailsBox = ({
  title,
  description,
  rightBtn,
  children,
  sx = {},
  xsWidth,
}) => {
  return (
    <>
      <Box component={Paper} p={2} bgcolor={grey} sx={sx}>
        <Box className={"flex_between"}>
          <Box sx={{ width: { md: "auto", sm: "auto", xs: xsWidth } }}>
            <Typography variant="h4">{title}</Typography>
            {description && (
              <Typography variant="h6" fontSize={13}>
                {description}
              </Typography>
            )}
          </Box>

          {rightBtn}
        </Box>
        {children && <Box mt={2}>{children}</Box>}
      </Box>
    </>
  );
};
export const ProfileIconBox = ({ icon }) => {
  return (
    <Box
      width={60}
      height={60}
      className={"flex_center"}
      borderRadius={"50%"}
      bgcolor={"#ff4155"}
    >
      {React.cloneElement(icon, {
        fontSize: "large",
        sx: { color: "white" },
      })}
    </Box>
  );
};
const InfoBox = ({ title, value }) => {
  return (
    <>
      <Typography variant="h5">{title}</Typography>
      <Typography variant="h6" fontSize={13}>
        {value}
      </Typography>
    </>
  );
};

export default function Profile() {
  const [submitData, setSubmitData] = useState({
    selectedFile: null,
    fileUrl: null,
    loading: false,
  });
  const [client, setClient] = useState({
    loading: true,
    data: {},
  });
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    setValue,
    control,
    unregister,
  } = useForm();
  useWatch({
    name: "selectedRegionTimeZone",
    control,
  });
  const {
    selectedAddressSchema,
    selectedAddressSchemaWithObjValue,
    addressSchemaSelectData,
    addressSchemaInputData,
    handleSetSchema,
    handleChangeInputAddressSchema,
    handleChangeSelectAddressSchemaAndGetOptions,
    handleSetSchemaValueForUpdate,
  } = useGetAddressSchema();

  const dispatch = useDispatch();
  const isUserProfileChange = useIsUserProfileChange();
  const LanguageReducer = useLanguageReducer();

  const { countries, handleGetAllCountries } = useGetAllCountries();
  const { loading: regionTimeZoneLoading, regionTimeZone } =
    useGetAllRegionTimeZone();

  const handleUpdateProfile = async (values) => {
    console.log(values);
    let imgUrl = null;
    if (submitData.selectedFile) {
      const formData = new FormData();
      formData.append("file", submitData.selectedFile);
      const { response: uploadImgResponse } = await fetchMethod(
        () => UpdateClientImage(formData),
        setSubmitData
      );
      imgUrl = uploadImgResponse.result.url;
    }

    const body = {
      clientName: getValues("name"),
      clientImage: imgUrl || submitData.fileUrl,
      clientCompanyName: getValues("companyName"),
      mobile: getValues("mobile1"),
      phone: getValues("mobile2"),
      licenseNo: "",
      regionTimeZoneId: getValues("selectedRegionTimeZone").id,
      clientAddress: {
        addressId: 0,
        countryId: selectedAddressSchema.country,
        cityId: selectedAddressSchema.city,
        areaId: selectedAddressSchema.area,
        streetAddress: selectedAddressSchema.streetAddress,
        streetAddress2: selectedAddressSchema.streetAddress2,
        houseNo: "",
        buildingName: "",
        landmark: "",
        workEmail: "",
        provinceId: selectedAddressSchema.province,
        pinCodeId: selectedAddressSchema.pinCode,
        stateId: selectedAddressSchema.state,
        fullAddress: "",
        zip: "",
        addressTypeId: 0,
        latitude: 0,
        longitude: 0,
      },
    };
    const { response } = await fetchMethod(
      () => UpdateClient(body),
      setSubmitData
    );
    fetchMethodResponse(
      response,
      "Updated successfully",
      handleGetAndSetTimeZone
    );
    if (response) {
      if (response.isSuccess) {
        dispatch(changeProfileImg(!isUserProfileChange));

        if (imgUrl) {
          setThisKeyCookie("companyImage", imgUrl);
        }
      }
    }
  };

  const handleGetClientProfile = async () => {
    setClient((prev) => ({ ...prev, loading: true }));
    const { response } = await fetchMethod(GetClientProfile);
    if (response.result) {
      const data = response.result;
      const _regionTimeZone = await handleGetAllRegionTimeZone();
      const _selectedRegionTimeZone = _regionTimeZone.find(
        (dt) => dt.id === data.regionTimeZoneId
      );
      if (data) {
        await handleSetSchemaValueForUpdate(data, setValue);
        setValue("name", data.clientName);
        setValue("companyName", data.clientCompanyName);
        setValue("mobile1", data.mobile);
        setValue("mobile2", data.phone);
        setValue("selectedRegionTimeZone", _selectedRegionTimeZone);
      }
    }
    setClient((prev) => ({ ...prev, loading: false }));
  };

  useEffect(() => {
    handleGetClientProfile();
  }, []);

  const PhoneInput1 = usePhoneInput("mobile1", control, getValues, true);
  const PhoneInput2 = usePhoneInput("mobile2", control, getValues, false, true);
  return (
    <>
      {client.loading ? (
        <CicrlesLoading />
      ) : (
        <PageMainBox>
          <GridContainer>
            {/* personal information */}
            <GridItem xs={12}>
              <Box component={Paper} p={2} bgcolor={grey}>
                <Typography variant="h4">
                  {LanguageReducer?.SETTING_PROFILE_PERSONAL_INFORMATION}
                </Typography>

                <GridContainer component={"form"}>
                  <GridItem xs={12}>
                    <Box
                      className={"flex_center"}
                      justifyContent={"start !important"}
                      gap={1}
                    >
                      <Box>
                        <UploadProfile
                          file={submitData.selectedFile}
                          onChange={(e) => {
                            const file = e.target.files[0];
                            setSubmitData((prev) => ({
                              ...prev,
                              selectedFile: file,
                            }));
                          }}
                          url={submitData.fileUrl}
                        />
                      </Box>
                      <Box>
                        <InfoBox
                          title={getThisKeyCookie("user_name")}
                          value={getThisKeyCookie("email")}
                        />
                      </Box>
                    </Box>
                  </GridItem>
                  <GridItem md={6} sm={6} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer.SETTING_PROFILE_NAME}
                    </InputLabel>
                    <TextField
                      placeholder={placeholders.name}
                      onFocus={handleFocus}
                      type="text"
                      size="small"
                      fullWidth
                      variant="outlined"
                      {...register("name", {
                        required: {
                          value: true,
                          message: LanguageReducer?.FIELD_REQUIRED_TEXT,
                        },
                        pattern: {
                          value: /^(?!\s*$).+/,
                          message:
                            LanguageReducer?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                        },
                      })}
                      error={Boolean(errors.name)} // set error prop
                      helperText={errors.name?.message}
                      sx={{
                        background: "#fff",
                        "& input": { fontSize: "12px !important" },
                      }}
                    />
                  </GridItem>
                  <GridItem md={6} sm={6} xs={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      {LanguageReducer.SETTING_PROFILE_COMPANY_NAME}
                    </InputLabel>
                    <TextField
                      placeholder={placeholders.name}
                      onFocus={handleFocus}
                      type="text"
                      size="small"
                      fullWidth
                      variant="outlined"
                      {...register("companyName", {
                        required: {
                          value: false,
                        },
                      })}
                      sx={{
                        background: "#fff",
                        "& input": { fontSize: "12px !important" },
                      }}
                    />
                  </GridItem>
                  <GridItem md={6} sm={6} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {LanguageReducer.SETTING_PROFILE_MOBILE_NO}
                    </InputLabel>
                    <PhoneInput1 />
                  </GridItem>
                  <GridItem md={6} sm={6} xs={12}>
                    <InputLabel sx={styleSheet.inputLabel}>
                      {LanguageReducer.SETTING_PROFILE_PHONE_NO}
                    </InputLabel>
                    <PhoneInput2 />
                  </GridItem>
                  <GridItem md={6} sm={6} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {`${LanguageReducer.SETTING_PROFILE_COUNTRY}`}
                    </InputLabel>
                    <CountrySchema
                      name="country"
                      control={control}
                      isRHF={true}
                      required={true}
                      isRefesh={true}
                      handleRefreshClick={handleGetAllCountries}
                      {...register("country", {
                        required: {
                          value: true,
                        },
                      })}
                      value={getValues("country")}
                      onChange={(event, newValue) => {
                        const resolvedId = newValue ? newValue : null;
                        handleSetSchema(
                          "country",
                          resolvedId,
                          setValue,
                          unregister
                        );
                      }}
                      errors={errors}
                    />
                  </GridItem>

                  {[...addressSchemaSelectData, ...addressSchemaInputData].map(
                    (input, index) => (
                      <GridItem md={6} sm={6} xs={12}>
                        <SchemaTextField
                          loading={input.loading}
                          disabled={input.disabled}
                          isRHF={true}
                          type={input.type}
                          name={input.key}
                          required={input.required}
                          optionLabel={addressSchemaEnum[input.key]?.LABEL}
                          optionValue={addressSchemaEnum[input.key]?.VALUE}
                          register={register}
                          options={input.options}
                          errors={errors}
                          label={input.label}
                          value={getValues(input.key)}
                          bgcolor="#fff"
                          onChange={
                            getLowerCase(input.type) === inputTypesEnum.SELECT
                              ? (name, value) => {
                                  handleChangeSelectAddressSchemaAndGetOptions(
                                    input.key,
                                    index,
                                    value,
                                    setValue,
                                    input.key
                                  );
                                }
                              : (e) => {
                                  handleChangeInputAddressSchema(
                                    input.key,
                                    e.target.value,
                                    setValue
                                  );
                                }
                          }
                        />
                      </GridItem>
                    )
                  )}
                  <GridItem md={6} sm={6} xs={12}>
                    <InputLabel required sx={styleSheet.inputLabel}>
                      {`${LanguageReducer.SETTING_PROFILE_REGION_TIME_ZONE}`}
                    </InputLabel>
                    {regionTimeZoneLoading ? (
                      <LoadingTextField />
                    ) : (
                      <SelectComponent
                        name="selectedRegionTimeZone"
                        control={control}
                        isRHF={true}
                        required={true}
                        options={regionTimeZone}
                        optionLabel={EnumOptions.TIME_ZONE.LABEL}
                        optionValue={EnumOptions.TIME_ZONE.VALUE}
                        {...register("selectedRegionTimeZone", {
                          required: {
                            value: true,
                          },
                        })}
                        value={getValues("selectedRegionTimeZone")}
                        onChange={(event, newValue) => {
                          const resolvedId = newValue ? newValue : null;
                          setValue("selectedRegionTimeZone", resolvedId);
                        }}
                        errors={errors}
                      />
                    )}
                  </GridItem>
                  <GridItem xs={12} textAlign={"right"}>
                    <ButtonComponent
                      loading={submitData.loading}
                      title={LanguageReducer.SETTING_PROFILE_UPDATE}
                      onClick={handleSubmit(handleUpdateProfile)}
                    />
                  </GridItem>
                </GridContainer>
              </Box>
            </GridItem>
            {/* user name */}
            <GridItem xs={12}>
              <ProfileDetailsBox
                title={LanguageReducer.SETTING_PROFILE_USERNAME}
                description={
                  "You can use the following username to sign in to your account and also to reset your password if you ever forget it."
                }
              >
                <GridContainer>
                  <GridItem>
                    <Box
                      className={"flex_center"}
                      justifyContent={"start !important"}
                      gap={1}
                    >
                      <Box>
                        <Box>
                          <ProfileIconBox icon={<PersonIcon />} />
                        </Box>
                      </Box>
                      <Box>
                        <InfoBox title={getThisKeyCookie("user_name")} />
                      </Box>
                    </Box>
                  </GridItem>
                </GridContainer>
              </ProfileDetailsBox>
            </GridItem>
            {/* email address */}
            <GridItem xs={12}>
              <ProfileDetailsBox
                title={LanguageReducer.SETTING_PROFILE_EMAIL_ADDRESS}
                description={
                  "You can use the following email addresses to receive notifications or corresponding emails."
                }
              >
                <GridContainer>
                  <GridItem>
                    <Box
                      className={"flex_center"}
                      justifyContent={"start !important"}
                      gap={1}
                    >
                      <Box>
                        <ProfileIconBox icon={<EmailIcon />} />
                      </Box>
                      <Box>
                        <InfoBox title={getThisKeyCookie("email")} />
                      </Box>
                    </Box>
                  </GridItem>
                </GridContainer>
              </ProfileDetailsBox>
            </GridItem>
          </GridContainer>
        </PageMainBox>
      )}
    </>
  );
}
