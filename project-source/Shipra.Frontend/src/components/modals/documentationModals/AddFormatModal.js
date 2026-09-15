import {
  Box,
  Checkbox,
  Chip,
  FormControlLabel,
  Grid,
  InputLabel,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useState } from "react";
import { useSelector } from "react-redux";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import {
  ActionButtonCustom,
  ActionButtonDelete,
  ActionButtonEdit,
  ClipboardIcon,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification
} from "../../../utilities/toast";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function AddFormatModal(props) {
  let { open, setOpen } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const [allPropertyType, setAllPropertyType] = useState([
    {
      id: 1,
      text: "string",
    },
    {
      id: 2,
      text: "int",
    },
    {
      id: 3,
      text: "DateTime",
    },
    {
      id: 4,
      text: "decimal",
    },
    {
      id: 5,
      text: "object",
    },
    {
      id: 6,
      text: "list",
    },
  ]);
  const [propertyName, setPropertyName] = useState("");
  const [propertyType, setPropertyType] = useState({
    id: 1,
    text: "string",
  });
  const [isRequired, setIsRequired] = useState(false);
  const [description, setDescription] = useState("");
  const [isUpdateRecord, setIsUpdateRecord] = useState(false);
  const [recordForEdit, setRecordForEdit] = useState();

  const [format, setFormat] = useState([]);
  const tableStyle = {
    td: {
      padding: "6px 0px",
      fontWeight: "bold",
      maxWidth: "0px",
    },
  };
  const tableObjecDescStyle = {
    td: {
      padding: "6px 0px",
      maxWidth: "0px",
    },
  };
  const handleClose = () => {
    setOpen(false);
  };
  const handleFocus = (event) => event.target.select();
  const handleValidation = () => {
    let isValid = true;
    if (propertyName == "") {
      errorNotification("Please enter property name");
      isValid = false;
    }
    if (description == "") {
      errorNotification("Please enter description");
      isValid = false;
    }
    return isValid;
  };
  const getFilledObjectData = () => {
    let obj = {
      property: propertyName,
      type: propertyType.text,
      isRequired: isRequired,
      description: description,
    };
    return obj;
  };
  const getFilteredObjectByName = () => {
    let existObj = format.find((row) => row.property == propertyName);
    return existObj;
  };
  const resetForm = () => {
    setPropertyName("");
    setIsRequired(false);
    setDescription("");
    setPropertyType({
      id: 1,
      text: "string",
    });
  };
  const handleAdd = () => {
    let isValid = handleValidation();
    if (!isValid) {
      return false;
    }

    let obj = getFilledObjectData();

    let exist = getFilteredObjectByName();
    if (exist) {
      errorNotification(`Property with name:${propertyName} already exist`);
    } else {
      resetForm();
      setFormat((oldArray) => [...oldArray, obj]);
    }
  };
  const handleDeleteRow = (data) => {
    const updatedRows = format.filter((row) => row.property != data.property);
    setFormat(updatedRows);
  };
  const handleUpdateRecord = () => {
    let isValid = handleValidation();
    if (!isValid) {
      return false;
    } else {
      let data = getFilledObjectData();

      const updatedData = format?.map((obj) => {
        if (obj.property == recordForEdit.property) {
          // Update the object with the specified id
          return data;
        }
        return obj;
      });
      setFormat(updatedData);
      resetForm();
      setIsUpdateRecord(false);
      setRecordForEdit();
    }
  };
  const handleEditRow = (data) => {
    setPropertyName(data.property);
    setIsRequired(data.isRequired);
    setDescription(data.description);
    let type = allPropertyType.find((x) => x.text == data.type);
    setPropertyType(type);

    setRecordForEdit(data);
    setIsUpdateRecord(true);
  };
  return (
    <ModalComponent
      open={open}
      onClose={handleClose}
      maxWidth="md"
      fullWidth
      title={"Add Format"}
    >
      <Grid container spacing={2} md={12} sm={12}>
        <Grid item md={5} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>Property Name</InputLabel>
          <TextField
            placeholder={"Property Name"}
            size="small"
            fullWidth
            variant="outlined"
            value={propertyName}
            onChange={(e) => setPropertyName(e.target.value)}
          />
        </Grid>
        <Grid item md={5} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>Property Type</InputLabel>
          <SelectComponent
            name="pp"
            options={allPropertyType}
            value={propertyType}
            optionLabel={EnumOptions.PROPERTY_TYPE.LABEL}
            optionValue={EnumOptions.PROPERTY_TYPE.VALUE}
            onChange={(e, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setPropertyType(resolvedId);
            }}
            size={"md"}
          />
        </Grid>
        <Grid item md={2} sm={12} sx={{ textAlign: "right" }} mt={2}>
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
                checked={isRequired}
                onChange={(e) => setIsRequired(e.target.checked)}
                edge="start"
                disableRipple
              />
            }
            label={"Required"}
          />
        </Grid>
        <Grid item md={12} sm={12}>
          <InputLabel sx={styleSheet.inputLabel}>Description</InputLabel>
          <TextField
            placeholder={"Description"}
            size="small"
            fullWidth
            multiline
            rows={2}
            variant="outlined"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </Grid>
        <Grid
          item
          md={12}
          sm={12}
          sx={{ display: "flex", justifyContent: "end" }}
        >
          <ActionButtonCustom
            label={isUpdateRecord ? "Update" : "Add"}
            onClick={() => {
              isUpdateRecord ? handleUpdateRecord() : handleAdd();
            }}
          />
        </Grid>
      </Grid>
      {format.length > 0 && (
        <Grid container spacing={2} mt={1}>
          <Grid item xs={12}>
            <Stack spacing={1}>
              <Typography variant="h4" gutterBottom>
                {"Format"}
              </Typography>
              <Table aria-label="simple table">
                <TableHead>
                  <TableRow>
                    <TableCell sx={tableObjecDescStyle.td}>Name</TableCell>
                    <TableCell sx={tableObjecDescStyle.td}>Type</TableCell>
                    <TableCell sx={tableObjecDescStyle.td}>
                      Description
                    </TableCell>
                    <TableCell sx={tableObjecDescStyle.td}>Action</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody
                  sx={{
                    "&.MuiTableBody-root": {
                      // maxWidth: "550px",
                    },
                  }}
                >
                  {format?.map((row) => (
                    <TableRow
                      key={row.property}
                      sx={{
                        "&:last-child td, &:last-child th": { border: 0 },
                      }}
                    >
                      <TableCell sx={tableObjecDescStyle.td}>
                        {row.property}
                      </TableCell>
                      <TableCell sx={tableObjecDescStyle.td}>
                        {row.type}
                      </TableCell>
                      <TableCell sx={tableObjecDescStyle.td}>
                        {row.isRequired && (
                          <Box>
                            <Chip
                              sx={{
                                "&.MuiChip-root": {
                                  size: "5px",
                                  borderRadius: "8px",
                                  height: "15px",
                                  fontSize: "10px",
                                },
                              }}
                              label="REQUIRED"
                              color="warning"
                              size="small"
                            />
                          </Box>
                        )}
                        {row.description}
                      </TableCell>
                      <TableCell sx={tableObjecDescStyle.td}>
                        <ActionButtonEdit
                          label=""
                          onClick={(e) => handleEditRow(row)}
                        />
                        <ActionButtonDelete
                          label=""
                          onClick={(e) => handleDeleteRow(row)}
                        />
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>

              <Grid sx={{ display: "flex", justifyContent: "end" }}>
                <ClipboardIcon size={18} text={JSON.stringify(format)} />
              </Grid>
            </Stack>
          </Grid>
        </Grid>
      )}
    </ModalComponent>
  );
}
export default AddFormatModal;
