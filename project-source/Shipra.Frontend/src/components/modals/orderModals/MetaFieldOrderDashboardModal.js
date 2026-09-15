import {
    Box,
    Checkbox,
    FormControlLabel,
    Grid,
    InputLabel,
    TextField,
} from "@mui/material";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { styleSheet } from "../../../assets/styles/style";
import { inputTypesEnum } from "../../../utilities/enum";
import {
    CustomColorLabelledOutline,
    getLowerCase
} from "../../../utilities/helpers/Helpers";

const MetaFieldOrderDashboardModal = (props) => {
  const { data, onClose, open } = props;
  let parseData = [];
  if (data && data.length > 0 && data !== "undefined") {
    parseData = JSON.parse(data[0]?.settingConfig);
  }
  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title={"MetaField"}
    >
      <CustomColorLabelledOutline isCollapse={true} label={"MetaField"}>
        <Grid container spacing={1}>
          {parseData?.map((input, input_index) => (
            <Grid
              item
              xs={12}
              sm={6}
              md={6}
              key={input.name}
              paddingTop={"0px!important"}
            >
              <Box marginBottom={1}>
                <InputLabel
                  required={input.required}
                  sx={styleSheet.inputLabel}
                >
                  {input?.name}
                </InputLabel>

                {/* SELECT FIELD */}
                {getLowerCase(input.type.label) === inputTypesEnum.SELECT && (
                  <SelectComponent
                    height={40}
                    name={input.name}
                    options={input.selectOptions}
                    optionLabel="label"
                    optionValue="id"
                    value={input.value}
                    disabled
                  />
                )}

                {/* TEXT / NUMBER / DATE FIELD */}
                {(getLowerCase(input.type.label) === inputTypesEnum.TEXT ||
                  getLowerCase(input.type.label) === inputTypesEnum.NUMBER ||
                  getLowerCase(input.type.label) === inputTypesEnum.DATE) && (
                  <TextField
                    type={getLowerCase(input.type.label)}
                    placeholder={input.description}
                    size="small"
                    fullWidth
                    variant="outlined"
                    required={input.required}
                    value={input.value || ""}
                    disabled
                  />
                )}

                {/* CHECKBOX FIELD */}
                {getLowerCase(input.type.label) === inputTypesEnum.CHECKBOX && (
                  <FormControlLabel
                    control={
                      <Checkbox
                        sx={{
                          color: "var(--primary-color)",
                          "&.Mui-checked": {
                            color: "var(--primary-color)",
                          },
                        }}
                        disabled
                        checked={input.value ?? input.defaultValue ?? false}
                      />
                    }
                  />
                )}
              </Box>
            </Grid>
          ))}
        </Grid>
      </CustomColorLabelledOutline>
    </ModalComponent>
  );
};

export default MetaFieldOrderDashboardModal;
