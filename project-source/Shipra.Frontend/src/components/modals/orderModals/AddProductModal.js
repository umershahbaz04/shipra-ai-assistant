import {
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Grid,
  InputAdornment,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import Slide from "@mui/material/Slide";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import SearchIcon from "../../../assets/images/topNav/Search.png";
import { styleSheet } from "../../../assets/styles/style";
import { DataGridRenderGreyBox } from "../../../utilities/helpers/Helpers";

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});
function AddProductModal(props) {
  let {
    open,
    setOpen,
    setSelectedProducts,
    selectedProducts,
    isEditOrder = false,
    orderData,
    productStocksForSelection = [],
    setProductStocksForSelection,
  } = props;

  const [search, setSearch] = useState("");
  const [currentSelectedProducts, setCurrentSelectedProducts] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleClose = () => {
    setOpen(false);
  };

  // let getProductStocksForSelection = async () => {
  //   if (isEditOrder) {
  //     //let do somework regarding edit order
  //   } else {
  //     setSelectedProducts([]);
  //   }
  //   let stationId = qtySelectedStation?.productStationId || 0;
  //   let res = await GetProductStocksForSelection(stationId);
  //   console.log("data:::", res.data);
  //   if (res.data.result != null) {
  //     for (let index = 0; index < res.data?.result.length; index++) {
  //       //get selected object against station
  //       let obj = selectedProducts.find(
  //         (x) => x.ProductStockId == res.data.result[index].ProductStockId
  //       );
  //       //orderItemId

  //       res.data.result[index].checked = obj ? true : false;
  //       res.data.result[index].isRemove =
  //         obj && obj?.orderItemId ? true : false; //if edit case then remove row
  //       res.data.result[index].discount = 0;
  //       res.data.result[index].newPrice = res.data.result[index].Price;
  //       res.data.result[index].newQuantity = 1;
  //       res.data.result[index].stationId = qtySelectedStation?.productStationId;
  //       // res.data.result[index].QuantityAvailable;
  //     }
  //     setProductStocksForSelection(res.data.result);
  //   }
  // };

  const [qtySelectedStation, setQtySelectedStation] = useState([]);
  // useEffect(() => {
  //   if (productStations.length > 0) {
  //     let selectedStation;
  //     if (isEditOrder) {
  //       if (orderData) {
  //         selectedStation = productStations?.find(
  //           (x) => x.productStationId == orderData?.order?.stationId
  //         );
  //         selectedStation = selectedStation;
  //       }
  //     } else {
  //       selectedStation = productStations[0];
  //     }
  //     setQtySelectedStation(selectedStation);
  //   }
  // }, [productStations]);

  useEffect(() => {
    setCurrentSelectedProducts(selectedProducts);
    const updatedProductStocksForSelection = productStocksForSelection.map(
      (itemOne) => {
        const selectedCurrentSelectedProduct = selectedProducts.find(
          (itemTwo) => itemTwo.SKU === itemOne.SKU
        );
        if (selectedCurrentSelectedProduct) {
          itemOne.checked = true;
        } else {
          itemOne.checked = false;
        }
        return itemOne;
      }
    );
    setProductStocksForSelection([...updatedProductStocksForSelection]);
  }, [open]);
  const allProdHeading = {
    fontSize: "12px",
    fontWeight: "bold",
    padding: "5px",
  };
  return (
    <Dialog
      fullWidth
      open={open}
      TransitionComponent={Transition}
      keepMounted
      scroll={"paper"}
      onClose={handleClose}
      maxWidth="lg"
      sx={styleSheet.modelMainClassWithImage}
      aria-describedby="alert-dialog-slide-description"
    >
      <DialogTitle sx={styleSheet.allProductionHeading}>
        {LanguageReducer?.languageType?.PRODUCT_ALL}{" "}
        {LanguageReducer?.languageType?.PRODUCTS_TEXT}
      </DialogTitle>
      <DialogContent sx={{ ...styleSheet.modelContentArea, fontSize: "12px" }}>
        <DialogContentText id="alert-dialog-slide-description">
          <Grid container spacing={1}>
            {/* <Grid item sm={12} md={6} lg={6} mb={2}>
              <InputLabel required sx={styleSheet.inputLabel}>
                {"Select Station"}
              </InputLabel>
              <SelectComponent
                disabled={orderData?.order}
                name="reason"
                options={productStations}
                defaulValue={qtySelectedStation}
                value={qtySelectedStation}
                optionLabel={EnumOptions.SELECTED_REASON.LABEL}
                optionValue={EnumOptions.SELECTED_REASON.VALUE}
                onChange={(e, newValue) => {
                  const resolvedId = newValue ? newValue : null;
                  setQtySelectedStation(resolvedId);
                }}
                sx={{}}
              />
            </Grid> */}
            <Grid item sm={12} md={12} lg={12} mb={2} mt={2}>
              <Stack direction={"row"}>
                <TextField
                  size="small"
                  placeholder={
                    LanguageReducer?.languageType?.SEARCH_TEXT +
                    " " +
                    LanguageReducer?.languageType?.PRODUCTS_TEXT
                  }
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
                {/* <Button
                  sx={styleSheet.filterButton}
                  variant="outlined"
                  color="inherit"
                >
                  <img
                    src={filterIcon}
                    style={{ width: "12px" }}
                    alt="SearchIcon"
                  />
                </Button> */}
              </Stack>
            </Grid>
          </Grid>
          <Grid>
            <TableContainer>
              <Table
                sx={{ ...styleSheet.simpleTable, minWidth: 275 }}
                aria-label="simple table"
              >
                <TableHead>
                  <TableRow>
                    <TableCell
                      sx={{
                        fontWeight: "bold",
                        padding: "5px",
                      }}
                      align="left"
                    >
                      <Checkbox
                        sx={{
                          ...styleSheet.selectionCheckBoxSize,
                          color: "var(--primary-color)",
                          fontSize: "18px",
                          "&.Mui-checked": {
                            color: "var(--primary-color)",
                          },
                        }}
                        checked={
                          currentSelectedProducts.length ===
                          productStocksForSelection.length
                        }
                        value={
                          currentSelectedProducts.length ===
                          productStocksForSelection.length
                        }
                        onChange={(e) => {
                          setCurrentSelectedProducts([]);
                          const _productStocksForSelection = [
                            ...productStocksForSelection.filter((value) => {
                              return (
                                value.ProductName?.toLocaleLowerCase().includes(
                                  search?.toLocaleLowerCase()
                                ) == true ||
                                value.SKU?.toLowerCase().includes(
                                  search?.toLowerCase()
                                ) == true ||
                                value.VarientOption?.toLowerCase().includes(
                                  search?.toLowerCase()
                                ) == true ||
                                value.Price?.toString().includes(search) ==
                                  true ||
                                value.Station?.trim()
                                  .toLocaleLowerCase()
                                  .includes(
                                    search.trim().toLocaleLowerCase()
                                  ) == true
                              );
                            }),
                          ];
                          if (e.target.checked) {
                            setCurrentSelectedProducts(
                              _productStocksForSelection.map((value, index) => {
                                return { ...value, checked: true };
                              })
                            );
                            setProductStocksForSelection(
                              _productStocksForSelection.map((value, index) => {
                                return { ...value, checked: true };
                              })
                            );
                          } else {
                            setCurrentSelectedProducts([]);
                            setProductStocksForSelection(
                              _productStocksForSelection.map((value, index) => {
                                return { ...value, checked: false };
                              })
                            );
                          }
                        }}
                        edge="start"
                        disableRipple
                      />
                    </TableCell>
                    <TableCell
                      sx={{
                        ...allProdHeading,
                      }}
                    >
                      Title
                    </TableCell>
                    <TableCell
                      sx={{
                        ...allProdHeading,
                      }}
                    >
                      SKU
                    </TableCell>
                    <TableCell
                      sx={{
                        ...allProdHeading,
                      }}
                    >
                      Station
                    </TableCell>
                    <TableCell
                      sx={{
                        ...allProdHeading,
                        textAlign: "center",
                      }}
                    >
                      Commited
                    </TableCell>
                    <TableCell
                      sx={{
                        ...allProdHeading,
                        textAlign: "center",
                      }}
                    >
                      Quantity
                    </TableCell>
                    <TableCell
                      sx={{
                        ...allProdHeading,
                        textAlign: "center",
                      }}
                    >
                      Price
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {productStocksForSelection
                    .filter((value) => {
                      // console.log(value);
                      return (
                        value.ProductName?.toLocaleLowerCase().includes(
                          search?.toLocaleLowerCase()
                        ) == true ||
                        value.SKU?.toLowerCase().includes(
                          search?.toLowerCase()
                        ) == true ||
                        value.VarientOption?.toLowerCase().includes(
                          search?.toLowerCase()
                        ) == true ||
                        value.Price?.toString().includes(search) == true ||
                        value.Station?.trim()
                          .toLocaleLowerCase()
                          .includes(search.trim().toLocaleLowerCase()) == true
                      );
                    })
                    .map((value, index) => {
                      const labelId = `checkbox-list-label-${value.ProductStockId}`;
                      return (
                        <TableRow
                          key={index}
                          sx={{
                            "&:last-child td, &:last-child th": { border: 0 },
                            background:
                              value?.QuantityAvailable === 0 &&
                              value?.TrackInventory
                                ? "#f5b3b3"
                                : value?.checked
                                ? "rgba(86, 58, 213, 0.2)"
                                : "",
                          }}
                        >
                          <TableCell
                            sx={{
                              padding: "5px",
                            }}
                            align="left"
                            size="small"
                          >
                            <Checkbox
                              sx={{
                                ...styleSheet.selectionCheckBoxSize,
                                color: "var(--primary-color)",
                                "&.Mui-checked": {
                                  color: "var(--primary-color)",
                                },
                              }}
                              checked={value.checked}
                              onChange={(e) => {
                                if (value.checked === false) {
                                  value.checked = true;
                                  setCurrentSelectedProducts([
                                    ...currentSelectedProducts,
                                    value,
                                  ]);
                                } else {
                                  value.checked = false;
                                  setCurrentSelectedProducts(
                                    currentSelectedProducts.filter(
                                      (item) =>
                                        item.ProductStockId !==
                                        value.ProductStockId
                                    )
                                  );
                                }
                              }}
                              edge="start"
                              disableRipple
                              inputProps={{ "aria-labelledby": labelId }}
                            />
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "5px",
                            }}
                            align="left"
                          >
                            <Stack
                              direction={"row"}
                              spacing={2}
                              alignItems={"center"}
                            >
                              <img
                                src={value.FeatureImage}
                                height={30}
                                width={30}
                                alt=""
                              />{" "}
                              <Box>
                                {value.ProductName}
                                <DataGridRenderGreyBox
                                  title={value?.VarientOption}
                                />
                              </Box>
                            </Stack>
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "5px",
                            }}
                            align="left"
                          >
                            {value?.SKU}
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "5px",
                            }}
                            align="left"
                          >
                            {value?.Station || qtySelectedStation?.sname}
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "5px",
                            }}
                            align="center"
                          >
                            {value.QuantityCommited}
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "5px",
                            }}
                            align="center"
                          >
                            {value.QuantityAvailable}
                          </TableCell>
                          <TableCell
                            sx={{
                              padding: "5px",
                            }}
                            align="center"
                          >
                            <b>{value.Price}</b>
                          </TableCell>
                        </TableRow>
                      );
                    })}
                </TableBody>
              </Table>
            </TableContainer>
          </Grid>
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button
          fullWidth
          variant="contained"
          sx={styleSheet.modalDismissButton}
          onClick={() => {
            for (let i = 0; i < productStocksForSelection.length; i++) {
              for (let j = 0; j < selectedProducts.length; j++) {
                if (
                  productStocksForSelection[i].ProductStockId ===
                  selectedProducts[j].ProductStockId
                ) {
                  productStocksForSelection[i].checked = true;
                  break;
                } else {
                  productStocksForSelection[i].checked = false;
                }
              }
            }
            setCurrentSelectedProducts(selectedProducts);
            handleClose();
          }}
        >
          {LanguageReducer?.languageType?.DISMISS_TEXT}
        </Button>
        <Button
          disabled={false}
          fullWidth
          variant="contained"
          sx={styleSheet.modalCarrierSubmitButton}
          onClick={() => {
            for (let i = 0; i < productStocksForSelection.length; i++) {
              for (let j = 0; j < currentSelectedProducts.length; j++) {
                if (
                  productStocksForSelection[i].ProductStockId ===
                  currentSelectedProducts[j].ProductStockId
                ) {
                  productStocksForSelection[i].checked = true;
                  break;
                } else productStocksForSelection[i].checked = false;
              }
            }
            setSelectedProducts(currentSelectedProducts);
            // setCurrentSelectedProducts([])
            handleClose();
          }}
        >
          {"Add Product"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
export default AddProductModal;
