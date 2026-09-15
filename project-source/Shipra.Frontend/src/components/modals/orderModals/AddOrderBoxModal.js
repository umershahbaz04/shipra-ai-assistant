import {
  Checkbox,
  InputAdornment,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SearchIcon from "../../../assets/images/topNav/Search.png";
import { GridContainer, GridItem } from "../../../utilities/helpers/Helpers";

function AddOrderBoxModal(props) {
  let {
    open,
    onClose,
    allClientOrderBox,
    selectedOrderBox,
    setSelectedOrderBox,
    setAddedOrderBoxes,
    addedOrderBoxes,
  } = props;
  const [search, setSearch] = useState("");
  const handleChange = (item) => {
    setSelectedOrderBox((prev) => {
      const isAlreadySelected = prev.some(
        (selectedItem) =>
          selectedItem.clientOrderBoxId === item.clientOrderBoxId
      );
      if (isAlreadySelected) {
        return prev.filter(
          (selectedItem) =>
            selectedItem.clientOrderBoxId !== item.clientOrderBoxId
        );
      } else {
        return [...prev, item];
      }
    });
  };
  const handleSelectAll = (event) => {
    if (event.target.checked) {
      setSelectedOrderBox(filteredOrderBoxes);
    } else {
      setSelectedOrderBox([]);
    }
  };
  const handleAddOrderBox = () => {
    setAddedOrderBoxes(selectedOrderBox);
    onClose();
  };
  const filteredOrderBoxes = allClientOrderBox.filter((item) =>
    item.boxName.toLowerCase().includes(search.toLowerCase())
  );

  useEffect(() => {
    if (addedOrderBoxes) {
      setSelectedOrderBox(addedOrderBoxes);
    }
  }, [open]);
  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="md"
      title={"Order Box"}
      actionBtn={
        <ModalButtonComponent
          title={"Add Order Box"}
          bg={purple}
          onClick={(e) => handleAddOrderBox()}
        />
      }
    >
      <GridContainer>
        <GridItem lg={12} md={12} sm={12} mb={2} mt={1}>
          <Stack direction={"row"}>
            <TextField
              size="small"
              placeholder={"Search Order Box"}
              fullWidth
              variant="outlined"
              sx={{ background: "white", mr: "10px" }}
              onChange={(e) => {
                setSearch(e.target.value);
              }}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <img
                      style={{ width: "18px", marginTop: "-4px" }}
                      src={SearchIcon}
                      alt="SearchIcon"
                    />
                  </InputAdornment>
                ),
              }}
            />
          </Stack>
        </GridItem>
        <GridItem lg={12} md={12} sm={12}>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell sx={{ padding: "6px !important" }}>
                    <Checkbox
                      checked={
                        selectedOrderBox.length === filteredOrderBoxes.length &&
                        filteredOrderBoxes.length > 0
                      }
                      indeterminate={
                        selectedOrderBox.length > 0 &&
                        selectedOrderBox.length < filteredOrderBoxes.length
                      }
                      onChange={handleSelectAll}
                    />
                  </TableCell>
                  <TableCell>Box</TableCell>
                  <TableCell>Length</TableCell>
                  <TableCell>Width</TableCell>
                  <TableCell>Height</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {filteredOrderBoxes.map((item, index) => (
                  <TableRow key={item.productId}>
                    {" "}
                    <TableCell sx={{ padding: "6px !important" }}>
                      <Checkbox
                        checked={selectedOrderBox.some(
                          (selectedItem) =>
                            selectedItem.clientOrderBoxId ===
                            item.clientOrderBoxId
                        )}
                        onChange={() => handleChange(item)}
                      />
                    </TableCell>
                    <TableCell>{item.boxName}</TableCell>
                    <TableCell>{item.length}</TableCell>
                    <TableCell>{item.width}</TableCell>
                    <TableCell>{item.height}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </GridItem>
      </GridContainer>
    </ModalComponent>
  );
}
export default AddOrderBoxModal;
