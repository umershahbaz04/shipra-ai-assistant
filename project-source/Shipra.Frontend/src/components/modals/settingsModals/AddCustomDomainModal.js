import { Box, InputLabel, Link, TextField, Typography } from "@mui/material";
import { purple } from "@mui/material/colors";
import { useState } from "react";
import { useForm } from "react-hook-form";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import { styleSheet } from "../../../assets/styles/style";
import { GridContainer, GridItem } from "../../../utilities/helpers/Helpers";
import { useSelector } from "react-redux";
import StarIcon from "@mui/icons-material/Star";
import OpenInNewIcon from "@mui/icons-material/OpenInNew";
import { AddUpdateCustomDomain } from "../../../api/AxiosInterceptors";
import { successNotification } from "../../../utilities/toast";

const AddCustomDomainModal = (props) => {
  const { open, onClose, getAllCustomDomains } = props;
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    setValue,
    control,
  } = useForm();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [loading, setLoading] = useState(false);

  const handleCreateDomain = async (data) => {
    const cleanedDomain = data?.domainName?.replace(
      /^(https?:\/\/)?(www\.)?/,
      ""
    );
    setLoading(true);
    try {
      const response = await AddUpdateCustomDomain(cleanedDomain);
      if (response.data.isSuccess) {
        successNotification("Domain Name Save Successfully");
        getAllCustomDomains();
        onClose();
      }
    } catch (e) {
    } finally {
      setLoading(false);
    }
  };
  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="sm"
      title={"Add Custom Domain"}
      actionBtn={
        <ModalButtonComponent
          title={"Add Domain"}
          bg={purple}
          loading={loading}
          onClick={handleSubmit(handleCreateDomain)}
        />
      }
      component={"form"}
    >
      <GridContainer spacing={1}>
        <GridItem sm={12} md={12} lg={12}>
          <GridItem xs={12}>
            <InputLabel required sx={styleSheet.inputLabel}>
              {"Domain Name"}
            </InputLabel>
            <TextField
              type="text"
              size="small"
              id="domainName"
              name="domainName"
              fullWidth
              variant="outlined"
              {...register("domainName", {
                required: {
                  value: true,
                  message: LanguageReducer?.languageType?.FIELD_REQUIRED_TEXT,
                },
                pattern: {
                  value: /^(?!\s*$).+/,
                  message:
                    LanguageReducer?.languageType
                      ?.INPUT_SHOULD_NOT_BE_ONLY_SPACES,
                },
              })}
              error={Boolean(errors.domainName)}
              helperText={errors.domainName?.message}
              placeholder={"Domain Name"}
            />
          </GridItem>
        </GridItem>
      </GridContainer>
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        bgcolor="rgba(230, 230, 250, 0.6)"
        border="1px solid rgba(213, 213, 231, 0.6)"
        px={2}
        py={1.5}
        mt={6}
        borderRadius={1}
        sx={{
          color: "#72a0f5",
          fontFamily: "Inter, sans-serif",
          fontSize: "14px",
        }}
      >
        <Box display="flex" alignItems="center" gap={1}>
          <StarIcon sx={{ fontSize: 18 }} />
          <Typography fontWeight={600} fontSize="14px" color="inherit">
            Need a domain?
          </Typography>
        </Box>

        <Link
          href="#"
          target="_blank"
          underline="none"
          display="flex"
          alignItems="center"
          color="inherit"
          fontWeight={600}
          fontSize="14px"
        >
          Learn more&nbsp;
          <OpenInNewIcon sx={{ fontSize: 14 }} />
        </Link>
      </Box>
    </ModalComponent>
  );
};

export default AddCustomDomainModal;
