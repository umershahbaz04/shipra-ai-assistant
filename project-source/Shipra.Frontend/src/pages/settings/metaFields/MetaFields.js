import ArrowForwardIosIcon from "@mui/icons-material/ArrowForwardIos";
import DeleteIcon from "@mui/icons-material/Delete";
import { Box, ButtonBase, Typography } from "@mui/material";
import IconButton from "@mui/material/IconButton";
import { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import useDeleteConfirmation from "../../../.reUseableComponents/CustomHooks/useDeleteConfirmation.js";
import DeleteConfirmationModal from "../../../.reUseableComponents/Modal/DeleteConfirmationModal.js";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent.js";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import { OrderIcon, ShipmentIcon } from "../../../components/sideNavBar/icons";
import { EnumOptions } from "../../../utilities/enum/index.js";
import {
  DeleteIconButton,
  GreyBox,
  GridContainer,
  GridItem,
  PageMainBox,
  greyBorder,
} from "../../../utilities/helpers/Helpers";
import { useGetAllMetafields } from "../../../utilities/helpers/HelpersFilter.js";
import { ProfileDetailsBox } from "../../Profile/Profile/Profile";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast/index.js";
import { CreateUpdateClientMetaField } from "../../../api/AxiosInterceptors.js";

const tabsData = [
  { id: 1, icon: <OrderIcon /> },
  { id: 2, icon: <ShipmentIcon /> },
];

const metafieldsTypeOptions = [
  { id: 1, label: "text" },
  { id: 2, label: "number" },
  { id: 3, label: "date" },
  { id: 4, label: "select" },
  { id: 5, label: "checkbox" },
];

const Tab = ({ title, icon, onClick, selected }) => {
  return (
    <Box
      className={"flex_between"}
      component={ButtonBase}
      onClick={onClick}
      sx={{
        height: 30,
        bgcolor: selected ? "#e0f7fa" : "#fff",
        border: greyBorder,
        borderRadius: 1.25,
        p: 1,
      }}
    >
      <Box className={"flex_between"} gap={1}>
        {icon}
        <Typography variant="h5">{title}</Typography>
      </Box>
      <ArrowForwardIosIcon fontSize="small" />
    </Box>
  );
};

export default function MetaFields() {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [errors, setErrors] = useState([]);
  const [selectedEntityId, setSelectedEntityId] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const isInitialLoad = useRef(true);
  const singleMetaField = {
    name: "",
    description: "",
    type: "",
    selectOptions: [{ id: "", label: "" }],
    defaultValue: false,
    value: null,
  };

  const { openDelete, setOpenDelete } = useDeleteConfirmation();
  const { loading, metafields, setMetafields } = useGetAllMetafields();

  const handleInvalid = (e) => {
    setErrors([...errors, e.target.name]);
  };

  const handleFilterError = (e, _name) => {
    const name = e ? e.target.name : _name;
    let newerrors = errors.filter((er) => er !== name);
    setErrors(newerrors);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    const [field, metaIndex, fieldIndex] = name.split("_");
    handleFilterError(e);
    setMetafields((prev) => {
      const _metaFields = [...prev];
      const _selectedEntity = { ..._metaFields[metaIndex] };
      const _settingConfig = [..._selectedEntity.settingConfig];
      const _selectedField = { ..._settingConfig[fieldIndex] };
      _selectedField[field] = value;
      _settingConfig[fieldIndex] = _selectedField;
      _selectedEntity.settingConfig = _settingConfig;
      _metaFields[metaIndex] = _selectedEntity;
      return _metaFields;
    });
  };

  const handleChangeAutoComplete = (name, value) => {
    const [field, metaIndex, fieldIndex] = name.split("_");
    handleFilterError(null, name);
    setMetafields((prev) => {
      const _metaFields = [...prev];
      const _selectedEntity = { ..._metaFields[metaIndex] };
      const _settingConfig = [..._selectedEntity.settingConfig];
      const _selectedField = { ..._settingConfig[fieldIndex] };
      _selectedField[field] = value;
      _settingConfig[fieldIndex] = _selectedField;
      _selectedEntity.settingConfig = _settingConfig;
      _metaFields[metaIndex] = _selectedEntity;
      return _metaFields;
    });
  };

  const handleSelectOptionChange = (
    metaIndex,
    fieldIndex,
    optIndex,
    newValue
  ) => {
    setMetafields((prev) => {
      const _fields = [...prev];
      const _entity = { ..._fields[metaIndex] };
      const _config = [..._entity.settingConfig];
      const _field = { ..._config[fieldIndex] };
      const updatedOptions = [..._field.selectOptions];
      updatedOptions[optIndex] = {
        ...updatedOptions[optIndex],
        label: newValue,
        id: newValue,
      };

      _field.selectOptions = updatedOptions;
      _config[fieldIndex] = _field;
      _entity.settingConfig = _config;
      _fields[metaIndex] = _entity;

      return _fields;
    });
  };

  const handleSubmit = async (e, data) => {
    e.preventDefault();
    // console.log(metafields);
    setIsLoading(true);
    const body = {
      clientMetaFields: metafields,
    };
    try {
      const response = await CreateUpdateClientMetaField(body);
      if (response.data.isSuccess) {
        successNotification("Action Perform Successfull");
      } else {
        errorNotification("An error occurred while saving the MetaFields.");
      }
    } catch (e) {
    } finally {
      setIsLoading(false);
    }
  };

  // console.log(metafields);

  useEffect(() => {
    if (metafields.length > 0 && isInitialLoad.current) {
      setSelectedEntityId(metafields[0].entityMetaFieldId);
      isInitialLoad.current = false;
    }
  }, [metafields]);

  return (
    <>
      <PageMainBox>
        <GridContainer>
          <GridItem md={4} sm={12} xs={12}>
            <ProfileDetailsBox
              title={
                LanguageReducer?.languageType
                  ?.SETTING_META_FIELD_META_FIELD_DEFINITION
              }
              description={
                LanguageReducer?.languageType
                  ?.SETTING_META_FIELD_ADD_CUSTOM_DATA
              }
            >
              <Box className={"flex_col"} gap={1}>
                {metafields.map((field, index) => {
                  const tabIcon = tabsData.find(
                    (tab) => tab.id === field.entityMetaFieldId
                  )?.icon;
                  return (
                    <Tab
                      key={field.entityMetaFieldId}
                      title={field.typeName}
                      icon={tabIcon}
                      onClick={() => {
                        setErrors([]);
                        setSelectedEntityId(field.entityMetaFieldId);
                      }}
                      selected={selectedEntityId === field.entityMetaFieldId}
                    />
                  );
                })}
              </Box>
            </ProfileDetailsBox>
          </GridItem>

          <GridItem md={8} sm={12} xs={12}>
            <ProfileDetailsBox
              xsWidth={"200px"}
              title={`${
                metafields.find((m) => m.entityMetaFieldId === selectedEntityId)
                  ?.typeName || ""
              } ${
                LanguageReducer?.languageType
                  ?.SETTING_META_FIELD_META_FIELD_DEFINITION
              }`}
              description={
                LanguageReducer?.languageType
                  ?.SETTING_META_FIELD_ADD_CUSTOM_DATA
              }
            >
              <GridContainer spacing={1}>
                <GridItem xs={12}>
                  {metafields.length > 0 && (
                    <form onSubmit={handleSubmit} onInvalid={handleInvalid}>
                      <GridContainer spacing={1}>
                        {metafields
                          .map((metaField, metaIndex) => ({
                            ...metaField,
                            metaIndex,
                          }))
                          .filter(
                            (metaField) =>
                              metaField.entityMetaFieldId === selectedEntityId
                          )
                          .flatMap((metaField) =>
                            metaField.settingConfig.map(
                              (mt_field, fieldIndex) => (
                                <GridItem
                                  xs={12}
                                  key={`${metaField.metaIndex}_${fieldIndex}`}
                                >
                                  <GreyBox
                                    sx={{
                                      bgcolor: "white",
                                      position: "relative",
                                    }}
                                  >
                                    <GridContainer spacing={1}>
                                      <GridItem xs={12}>
                                        <TextFieldLableComponent
                                          title={"Name"}
                                        />
                                        <TextFieldComponent
                                          name={`name_${metaField.metaIndex}_${fieldIndex}`}
                                          value={mt_field.name}
                                          onChange={handleChange}
                                          errors={errors}
                                        />
                                      </GridItem>

                                      <GridItem xs={12}>
                                        <TextFieldLableComponent
                                          title={"Description"}
                                          required={false}
                                        />
                                        <TextFieldComponent
                                          name={`description_${metaField.metaIndex}_${fieldIndex}`}
                                          value={mt_field.description}
                                          onChange={handleChange}
                                          required={false}
                                        />
                                      </GridItem>

                                      <GridItem xs={12}>
                                        <TextFieldLableComponent
                                          title={"Type"}
                                        />
                                        <SelectComponent
                                          value={mt_field.type}
                                          name={`type_${metaField.metaIndex}_${fieldIndex}`}
                                          required={true}
                                          options={metafieldsTypeOptions}
                                          optionLabel={
                                            EnumOptions.METAFIELD_TYPES.LABEL
                                          }
                                          optionValue={
                                            EnumOptions.METAFIELD_TYPES.VALUE
                                          }
                                          errors={errors}
                                          onChange={handleChangeAutoComplete}
                                        />
                                      </GridItem>

                                      {mt_field.type?.label === "select" && (
                                        <GridItem xs={12}>
                                          <TextFieldLableComponent
                                            title={"Select Options"}
                                          />
                                          <GridContainer spacing={1}>
                                            {mt_field.selectOptions?.map(
                                              (opt, optIndex) => (
                                                <GridItem
                                                  xs={12}
                                                  key={optIndex}
                                                >
                                                  <Box
                                                    display="flex"
                                                    alignItems="center"
                                                    gap={1}
                                                  >
                                                    <TextFieldComponent
                                                      name={`value_${metaField.metaIndex}_${fieldIndex}_${optIndex}`}
                                                      value={opt.label}
                                                      onChange={(e) =>
                                                        handleSelectOptionChange(
                                                          metaField.metaIndex,
                                                          fieldIndex,
                                                          optIndex,
                                                          e.target.value
                                                        )
                                                      }
                                                      errors={errors}
                                                      fullWidth
                                                    />
                                                    {optIndex === 0 ? (
                                                      <ButtonComponent
                                                        title={"+ More Option"}
                                                        btnMdWidth={"140px"}
                                                        btnLgWidth={"140px"}
                                                        onClick={() => {
                                                          setMetafields(
                                                            (prev) => {
                                                              const updated = [
                                                                ...prev,
                                                              ];
                                                              updated[
                                                                metaField
                                                                  .metaIndex
                                                              ].settingConfig[
                                                                fieldIndex
                                                              ].selectOptions.push(
                                                                {
                                                                  id: "",
                                                                  label: "",
                                                                }
                                                              );
                                                              return updated;
                                                            }
                                                          );
                                                        }}
                                                      />
                                                    ) : (
                                                      <IconButton
                                                        onClick={() => {
                                                          setMetafields(
                                                            (prev) => {
                                                              const updated = [
                                                                ...prev,
                                                              ];
                                                              updated[
                                                                metaField
                                                                  .metaIndex
                                                              ].settingConfig[
                                                                fieldIndex
                                                              ].selectOptions.splice(
                                                                optIndex,
                                                                1
                                                              );
                                                              return updated;
                                                            }
                                                          );
                                                        }}
                                                        size="small"
                                                        color="error"
                                                      >
                                                        <DeleteIcon fontSize="small" />
                                                      </IconButton>
                                                    )}
                                                  </Box>
                                                </GridItem>
                                              )
                                            )}
                                          </GridContainer>
                                        </GridItem>
                                      )}

                                      {mt_field.type?.label === "checkbox" && (
                                        <GridItem xs={12}>
                                          <Box
                                            display="flex"
                                            alignItems="center"
                                            gap={2}
                                          >
                                            <TextFieldLableComponent
                                              title={"Default Checked"}
                                            />
                                            <input
                                              type="checkbox"
                                              name={`defaultValue_${metaField.metaIndex}_${fieldIndex}`}
                                              checked={
                                                mt_field.defaultValue || false
                                              }
                                              onChange={(e) => {
                                                setMetafields((prev) => {
                                                  const updated = [...prev];
                                                  updated[
                                                    metaField.metaIndex
                                                  ].settingConfig[
                                                    fieldIndex
                                                  ].defaultValue =
                                                    e.target.checked;
                                                  return updated;
                                                });
                                              }}
                                            />
                                          </Box>
                                        </GridItem>
                                      )}
                                    </GridContainer>

                                    <DeleteIconButton
                                      sx={{
                                        position: "absolute",
                                        zIndex: 10,
                                        top: 0,
                                        right: 0,
                                      }}
                                      onClick={() => {
                                        setMetafields((prev) => {
                                          const updated = [...prev];
                                          updated[
                                            metaField.metaIndex
                                          ].settingConfig.splice(fieldIndex, 1);
                                          return updated;
                                        });
                                      }}
                                    />
                                  </GreyBox>
                                </GridItem>
                              )
                            )
                          )}

                        <GridItem xs={12} textAlign={"right"}>
                          <Box
                            sx={{
                              display: "flex",
                              gap: 1,
                              justifyContent: "end",
                              alignItems: "center",
                            }}
                          >
                            <ButtonComponent
                              title={
                                LanguageReducer?.languageType
                                  ?.SETTING_META_FIELD_ADD_META_FIELD
                              }
                              onClick={() => {
                                setErrors([]);
                                setMetafields((prev) =>
                                  prev.map((metaField) => {
                                    if (
                                      metaField.entityMetaFieldId ===
                                      selectedEntityId
                                    ) {
                                      return {
                                        ...metaField,
                                        settingConfig: [
                                          ...metaField.settingConfig,
                                          singleMetaField,
                                        ],
                                      };
                                    }
                                    return metaField;
                                  })
                                );
                              }}
                            />
                            <ButtonComponent
                              title={
                                LanguageReducer?.languageType
                                  ?.SETTING_META_FIELD_SAVE
                              }
                              type={"submit"}
                              loading={isLoading}
                            />
                          </Box>
                        </GridItem>
                      </GridContainer>
                    </form>
                  )}
                </GridItem>
              </GridContainer>
            </ProfileDetailsBox>
          </GridItem>
        </GridContainer>
      </PageMainBox>

      {openDelete && (
        <DeleteConfirmationModal
          open={openDelete}
          setOpen={setOpenDelete}
          handleDelete={() => {}}
          messageDetails={"All data will be removed of this field."}
        />
      )}
    </>
  );
}
