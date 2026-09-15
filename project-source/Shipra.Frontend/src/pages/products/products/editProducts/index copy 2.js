import EditIcon from "@mui/icons-material/Edit";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import {
  Autocomplete,
  Avatar,
  Backdrop,
  Box,
  Button,
  CardHeader,
  Checkbox,
  Chip,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  FormControlLabel,
  FormGroup,
  Grid,
  InputLabel,
  MenuItem,
  Slide,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import Divider from "@mui/material/Divider";
import Menu from "@mui/material/Menu";
import Paper from "@mui/material/Paper";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell, { tableCellClasses } from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import TableSortLabel from "@mui/material/TableSortLabel";
import Toolbar from "@mui/material/Toolbar";
import { alpha, styled } from "@mui/material/styles";
import PropTypes from "prop-types";
import React, { Fragment, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useSelector } from "react-redux";
import { useLocation } from "react-router-dom";
import {
  CreateProductCategory,
  GetAllProductCategoryLookup,
  GetAllProductOptionLookup,
  GetAllStationLookup,
  GetProductById,
  UpdateProduct,
} from "../../../../api/AxiosInterceptors";
import ImageIcon from "../../../../assets/images/uploadImage.png";
import { styleSheet } from "../../../../assets/styles/style";
import DragDropAllFiles from "../../../../components/dragAndDrop/allImages";
import DragDropFile from "../../../../components/dragAndDrop/featureImage";
import CreateVariantModal from "../../../../components/modals/myCarrierModals/CreateVariantModal";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import {
  BackdropCustom,
  amountFormat,
  placeholders,
} from "../../../../utilities/helpers/Helpers";
import { getAllStationLookupFunc } from "../../../../apiCallingFunction";

const StyledTableCell = styled(TableCell)(({ theme }) => ({
  [`&.${tableCellClasses.head}`]: {
    backgroundColor: theme.palette.common.black,
    color: theme.palette.common.white,
  },
  [`&.${tableCellClasses.body}`]: {
    fontSize: 14,
  },
}));

const StyledTableRow = styled(TableRow)(({ theme }) => ({
  "&:nth-of-type(odd)": {
    backgroundColor: theme.palette.action.hover,
  },
  "&:last-child td, &:last-child th": {
    border: 0,
  },
}));

const StyledMenu = styled((props) => (
  <Menu
    elevation={0}
    anchorOrigin={{
      vertical: "bottom",
      horizontal: "right",
    }}
    transformOrigin={{
      vertical: "top",
      horizontal: "right",
    }}
    {...props}
  />
))(({ theme }) => ({
  "& .MuiPaper-root": {
    borderRadius: 6,
    marginTop: theme.spacing(1),
    minWidth: 180,
    color:
      theme.palette.mode === "light"
        ? "rgb(55, 65, 81)"
        : theme.palette.grey[300],
    boxShadow:
      "rgb(255, 255, 255) 0px 0px 0px 0px, rgba(0, 0, 0, 0.05) 0px 0px 0px 1px, rgba(0, 0, 0, 0.1) 0px 10px 15px -3px, rgba(0, 0, 0, 0.05) 0px 4px 6px -2px",
    "& .MuiMenu-list": {
      padding: "4px 0",
    },
    "& .MuiMenuItem-root": {
      "& .MuiSvgIcon-root": {
        fontSize: 18,
        color: theme.palette.text.secondary,
        marginRight: theme.spacing(1.5),
      },
      "&:active": {
        backgroundColor: alpha(
          theme.palette.primary.main,
          theme.palette.action.selectedOpacity
        ),
      },
    },
  },
}));

function EnhancedTableToolbar(props) {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const {
    numSelected,
    handleOpenPrices,
    handleOpenQuantities,
    handleCloseQuantities,
    handleCloseSKU,
    handleClosePrices,
  } = props;
  const [anchorEl, setAnchorEl] = React.useState(null);

  const open = Boolean(anchorEl);
  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <Toolbar
      sx={{
        pl: { sm: 2 },
        pr: { xs: 1, sm: 1 },
        ...(numSelected > 0 && {
          bgcolor: (theme) =>
            alpha(
              theme.palette.primary.main,
              theme.palette.action.activatedOpacity
            ),
        }),
      }}
    >
      {numSelected > 0 ? (
        <Typography
          sx={{ flex: "1 1 100%" }}
          color="inherit"
          variant="subtitle1"
          component="div"
        >
          {numSelected} {LanguageReducer?.languageType?.SELECTED_TEXT}
        </Typography>
      ) : (
        <Typography
          sx={{ flex: "1 1 100%" }}
          color="inherit"
          variant="subtitle1"
          component="div"
        >
          {0} {LanguageReducer?.languageType?.SELECTED_TEXT}
        </Typography>
      )}

      {numSelected > 0 ? (
        <div>
          <Button
            id="demo-customized-button"
            aria-controls={open ? "demo-customized-menu" : undefined}
            aria-haspopup="true"
            aria-expanded={open ? "true" : undefined}
            variant="contained"
            disableElevation
            onClick={handleClick}
            endIcon={<KeyboardArrowDownIcon />}
            sx={{
              background:
                "var(--primary-color)",
              fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
              textTransform: "capitalize  !important",
            }}
          >
            {LanguageReducer?.languageType?.EDIT_TEXT}
          </Button>
          <StyledMenu
            id="demo-customized-menu"
            MenuListProps={{
              "aria-labelledby": "demo-customized-button",
            }}
            anchorEl={anchorEl}
            open={open}
            onClose={handleClose}
          >
            <MenuItem
              onClick={() => {
                handleOpenPrices();
                handleCloseSKU();
                handleCloseQuantities();
                handleClose();
              }}
              disableRipple
            >
              <EditIcon />
              {LanguageReducer?.languageType?.EDIT_PRICES_TEXT}
            </MenuItem>
            <MenuItem
              onClick={() => {
                handleOpenQuantities();
                handleCloseSKU();
                handleClosePrices();
                handleClose();
              }}
              disableRipple
            >
              <EditIcon />
              {LanguageReducer?.languageType?.EDIT_QUNATITIES_TEXT}
            </MenuItem>
          </StyledMenu>
        </div>
      ) : null}
    </Toolbar>
  );
}
EnhancedTableToolbar.propTypes = {
  numSelected: PropTypes.number.isRequired,
};

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

function EditProducts() {
  const location = useLocation();
  //console.log("location data", location.state);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [productData, setProductData] = useState(null);
  const { data } = location.state || {};
  const [open, setOpen] = useState(false);
  const [openSKU, setOpenSKU] = useState(false);
  const [openPrices, setOpenPrices] = useState(false);
  const [openQuantities, setOpenQuantities] = useState(false);
  const [inputFields, setInputFields] = useState([]);
  const [categoryName, setCategoryName] = useState("");
  const [stationValues, setStationValues] = useState([]);
  const [productCategories, setProductCategories] = useState([]);
  const [productStations, setProductStations] = useState([]);
  const [allProductOptions, setAllProductOptions] = useState([]);
  const [optionsCheckbox, setOptionsCheckbox] = useState(false);
  const [trackInventoryCheckbox, setTrackInventoryCheckbox] = useState(true);
  const [selected, setSelected] = useState([]);
  const [load, setLoad] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [createVariantModal, setCreateVariantModal] = useState(false);
  const [combinations, setCombinations] = useState([]);
  const [tempCombinations, setTempCombinations] = useState([]);
  const [productStocks, setProductStocks] = useState([]);
  const [stockCounts, setStockCounts] = useState([]);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    getValues,
    control,
  } = useForm();

  const handleFocus = (event) => event.target.select();

  const handleClose = () => {
    setOpen(false);
  };

  const handleOpen = () => {
    setOpen(true);
  };

  const handleClosePrices = () => {
    setOpenPrices(false);
  };

  const handleOpenPrices = () => {
    setOpenPrices(true);
  };

  const handleCloseSKU = () => {
    setOpenSKU(false);
  };

  const handleCloseQuantities = () => {
    setOpenQuantities(false);
  };

  const handleOpenQuantities = () => {
    setOpenQuantities(true);
  };

  const handleSelectAllClick = (event) => {
    if (event.target.checked) {
      const newSelected = combinations.map((n, indx) => indx);
      console.log("i am here ", newSelected);
      console.log("combination in selected", combinations);
      setSelected(newSelected);
      return;
    }
    setSelected([]);
  };
  const getProductData = async () => {
    try {
      setLoad(true);
      const response = await GetProductById(data?.products?.ProductId);
      setProductData(response.data.result);
      await getAllProductCategoryLookup();
      await getAllStationLookup();
      // let CategoryId = response?.data?.result?.productCategoryId;
      // const categorySelected = productCategories.find(
      //   (category) => category.productCategoryId === CategoryId
      // );
      // console.log(categorySelected);
      // setSelectedCategory(categorySelected?.categoryName);
      const stockData = response.data.result.productStocks;
      const uniqueStocks = stockData.reduce((unique, stock) => {
        const existingStock = unique.find((s) => s.sku === stock.sku);
        if (!existingStock) {
          unique.push(stock);
        }
        return unique;
      }, []);
      setProductStocks(uniqueStocks);
      const skuCounts = stockData.reduce((counts, stock) => {
        if (counts[stock.sku]) {
          counts[stock.sku]++;
        } else {
          counts[stock.sku] = 1;
        }
        return counts;
      }, {});
      setStockCounts(skuCounts);
      setOptionsCheckbox(response.data.result.haveOptions);
      const options = [];
      response.data.result.productOptions.forEach((option) => {
        const existingField = options.find(
          (field) => field.selectedOption === option.optionId
        );

        if (existingField) {
          existingField.options.push(option.optionValue);
        } else {
          options.push({
            selectedOption: option.optionId,
            options: [option.optionValue],
          });
        }
      });
      options.sort((a, b) => a.selectedOption.localeCompare(b.selectedOption));
      setInputFields(options);
      setLoad(false);
    } catch (error) {
      console.error("Error in getting data of product:", error);
      setLoad(false);
    }
  };
  const handleClick = (event, name) => {
    const selectedIndex = selected.indexOf(name);
    let newSelected = [];

    if (selectedIndex === -1) {
      newSelected = newSelected.concat(selected, name);
    } else if (selectedIndex === 0) {
      newSelected = newSelected.concat(selected.slice(1));
    } else if (selectedIndex === selected.length - 1) {
      newSelected = newSelected.concat(selected.slice(0, -1));
    } else if (selectedIndex > 0) {
      newSelected = newSelected.concat(
        selected.slice(0, selectedIndex),
        selected.slice(selectedIndex + 1)
      );
    }
    setSelected(newSelected);
  };

  const isSelected = (name) => selected.indexOf(name) !== -1;
  const headCells = [
    {
      id: "name",
      numeric: false,
      disablePadding: true,
      label: `${LanguageReducer?.languageType?.SHOWING_TEXT} ${combinations.length} ${LanguageReducer?.languageType?.VARIANTS_TEXT} `,
    },
    {},
  ];

  function generateCombinations(optionsArray) {
    const combinations = [[]]; // Start with an empty combination

    optionsArray.forEach((optionsObj) => {
      const newCombinations = [];

      optionsObj.options.forEach((option) => {
        combinations.forEach((combination) => {
          newCombinations.push([...combination, option]);
        });
      });
      combinations.push(...newCombinations);
    });

    return combinations.slice(1); // Exclude the empty combination
  }
  // const updateExistingStationQty = (stationId, newQty, lowQuantityLimit) => {
  //   // Use map to create a new array with the updated qty for the specific station
  //   setProductStations((prevStations) =>
  //     prevStations.map((station) => {
  //       if (station.productStationId === stationId) {
  //         // Return a new object with updated qty
  //         return {
  //           ...station,
  //           qty: newQty,
  //           lowQuantityLimit: lowQuantityLimit,
  //         };
  //       }
  //       // For other stations, return the original object
  //       return station;
  //     })
  //   );
  // };
  const [groupedFormatedData, setGroupedFormatedData] = useState([]);
  const transformData = (data) => {
    const groupedData = {};

    // Group data by SKU
    data.forEach((item) => {
      const {
        sku,
        productStationId,
        quantityAvailable,
        lowQuantityLimit,
        varientOption,
      } = item;
      if (!groupedData[sku]) {
        groupedData[sku] = {
          sku,
          stationIds: [],
          varientOption,
        };
      }

      groupedData[sku].stationIds.push({
        productStationId,
        qty: quantityAvailable,
        lowQuantityLimit: lowQuantityLimit,
        varientOption,
      });
    });

    // Convert grouped data to the desired format
    const result = Object.values(groupedData).map((group) => ({
      sku: group.sku,
      stationIds: group.stationIds,
      varientOption: group?.varientOption,
    }));
    setGroupedFormatedData(result);
    return result;
  };

  const optionsCombinations = (optionsArray) => {
    // debugger;
    const filteredOptionsArray = optionsArray.filter((optionArray) => {
      return optionArray.options.length !== 0;
    });
    const combinations = generateCombinations(optionsArray);
    const filteredArray = combinations.filter((combination) => {
      return combination.length == filteredOptionsArray.length;
    });

    const groupdTransformData = productData
      ? transformData(productData?.productStocks)
      : [];

    let structureArray = [];
    for (let index = 0; index < filteredArray.length; index++) {
      let updatedStationValue = groupdTransformData.find(
        (x) => x.sku == productStocks[index]?.sku
      );
      // debugger;
      //debugger;
      structureArray[index] = {
        productStockId: productStocks[index]?.productStockId,
        SKU: productStocks[index]?.sku,
        prices: productStocks[index]?.price,
        quantities: productStocks[index]?.quantityAvailable,
        lowQuantityLimit: productStocks[index]?.lowQuantityLimit,
        variants: filteredArray[index],
        stationIds: updatedStationValue.stationIds,
        // stationIds: stockCounts[productStocks[index]?.sku],
        SKUChecked: false,
        variantOption: updatedStationValue?.varientOption,
      };
    }
    setTempCombinations(structureArray);
    setCombinations(structureArray);
  };

  useEffect(() => {
    optionsCombinations(inputFields);
  }, [inputFields]);

  let getAllProductCategoryLookup = async () => {
    let res = await GetAllProductCategoryLookup();
    if (res.data.result != null) {
      setProductCategories(res.data.result);
    }
  };
  let getAllStationLookup = async () => {
    let data = await getAllStationLookupFunc();
    if (data.length > 0) {
      if (data?.find((x) => x.productStationId == 0)) {
        data.shift();
      }
      const updatedData = data.map((item) => ({
        ...item,
        qty: 0, // You can set the initial value of qty as needed
        lowQuantityLimit: 0, // You can set the initial value of lowQuantityLimit as needed
      }));

      setProductStations(updatedData);
    }
  };
  let getAllProductOptionLookup = async () => {
    let res = await GetAllProductOptionLookup({});
    if (res.data.result != null) {
      setAllProductOptions(res.data.result);
    }
  };
  const handleCreateVariant = () => {
    setCreateVariantModal(true);
  };
  useEffect(() => {
    getAllStationLookup();
    getAllProductOptionLookup();
  }, [selectedCategory]);
  useEffect(() => {
    if (!productData) {
      getProductData();
    }
  }, [productData]);
  const createCategory = () => {
    if (categoryName == "") {
      errorNotification(
        LanguageReducer?.languageType?.CATEGORY_NAME_CANNOT_BE_EMPTY_TOAST
      );
      return;
    }
    CreateProductCategory({ categoryName: categoryName })
      .then((res) => {
        console.log("res:", res);
        if (!res?.data?.isSuccess) {
          errorNotification(
            LanguageReducer?.languageType?.UNABLE_TO_CREATE_CATEGORY_TOAST
          );
          errorNotification(res?.data?.customErrorMessage);
        } else {
          successNotification(
            LanguageReducer?.languageType?.CATEGORY_CREATED_SUCCESSFULLY_TOAST
          );
          handleClose();
        }
        getAllProductCategoryLookup();
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification(
          LanguageReducer?.languageType?.UNABLE_TO_CREATE_CATEGORY_TOAST
        );
      });
  };
  const handleCreateVariantClose = () => {
    setCreateVariantModal(false);
  };
  const updateStockData = () => {
    setCombinations(tempCombinations);
    let count = 0;
    for (let index = 0; index < combinations.length; index++) {
      productStocks[count] = {
        active: productStocks[count]?.active,
        price: parseFloat(combinations[index].prices),
        productId: productStocks[count]?.productId,
        productStationId: combinations[index].stationIds,
        productStockId: productStocks[count]?.productStockId,
        productStockStatusId: productStocks[count]?.productStockStatusId,
        quantityAvailable: combinations[index].quantities,
        LowQuantityLimit: combinations[index].lowQuantityLimit,
        sku:
          combinations[index].SKU == ""
            ? data.SKU + "-" + index
            : combinations[index].SKU,
        VarientOption: combinations[index].variants[0],
      };
      count++;
    }
    setProductData((prevData) => ({
      ...prevData,
      productStocks: productStocks,
    }));
  };
  useEffect(() => {
    updateStockData();
  }, [combinations]);
  const updateProduct = async () => {
    console.log("Product Data before Update", productData);

    let productStocks = [];
    let count = 0;

    for (let index = 0; index < combinations.length; index++) {
      if (combinations[index]) {
        if (combinations[index].stationIds.length > 0) {
          for (let j = 0; j < combinations[index].stationIds.length; j++) {
            productStocks[count] = {
              ProductStockId: combinations[index].productStockId,
              Sku:
                combinations[index].SKU == ""
                  ? data.SKU + "-" + index
                  : combinations[index].SKU,
              price: combinations[index].prices,
              QuantityAvailable: combinations[index]?.stationIds[j]?.qty,
              LowQuantityLimit:
                combinations[index]?.stationIds[j]?.lowQuantityLimit,
              ProductStationId:
                combinations[index]?.stationIds[j]?.productStationId,
              VarientOption: combinations[index]?.variants?.join("/"),
            };
            count++;
          }
        } else {
          // errForStations.push({ index: index, sku: combinations[index].SKU });
        }
      }
    }
    //create variable copy
    let updatedData = productData;
    updatedData.productStocks = productStocks;
    console.log(updatedData);
    // setIsLoading(true);
    // try {
    //   const response = await UpdateProduct(updatedData);
    //   console.log("Update Product Option Response", response);
    //   successNotification(
    //     LanguageReducer?.languageType?.PRODUCT_UPDATED_SUCCESSFULLY_TOAST
    //   );
    //   getProductData();
    // } catch (error) {
    //   console.error("Error in updating this product", error.response);
    //   errorNotification(
    //     LanguageReducer?.languageType?.UNABLE_TO_UPDATE_THE_PRODUCT_TOAST
    //   );
    // } finally {
    //   setIsLoading(false);
    // }
  };
  const handleChangeCategory = (event) => {
    const { id, name } = event.target.value;
    setSelectedCategory(name);
    setProductData((prevData) => ({
      ...prevData,
      ProductCategoryId: id,
    }));
  };
  const getProductOptionNameById = (selectedOption) => {
    let name = "";
    if (allProductOptions.length > 0) {
      let obj = allProductOptions?.find((x) => x.id == selectedOption);
      if (obj) {
        name = obj?.text;
      }
    }
    return name;
  };
  useEffect(() => {
    if (productCategories.length > 0) {
      let category = productCategories.find(
        (x) => x.productCategoryId == data.products.ProductCategoryId
      );
      const { productCategoryId, categoryName } = category;
      setSelectedCategory(categoryName);
      setProductData((prevData) => ({
        ...prevData,
        ProductCategoryId: productCategoryId,
      }));
    }
  }, [productCategories]);

  //#region
  //#region
  const handleStationQuantity = (e, items, stationId) => {
    let quantityArr = [...tempCombinations];
    const updatedStations = quantityArr[items]?.stationIds.map((station) => {
      if (station.productStationId == stationId) {
        let uData = {
          ...station,
          qty: eval(e.target.value) ? eval(e.target.value) : 0,
        };
        return uData;
      }
      return station;
    });
    //sum of all station qty
    const sumOfQty = updatedStations?.reduce((accumulator, currentItem) => {
      return accumulator + currentItem.qty;
    }, 0);
    quantityArr[items] = {
      ...quantityArr[items],
      quantities: sumOfQty,
      stationIds: updatedStations,
    };

    setTempCombinations(quantityArr);
  };
  const handleStationLowQuantity = (e, items, stationId) => {
    let quantityArr = [...tempCombinations];
    const updatedStations = quantityArr[items]?.stationIds.map((station) => {
      if (station.productStationId == stationId) {
        let uData = {
          ...station,
          lowQuantityLimit: eval(e.target.value) ? eval(e.target.value) : 0,
        };
        return uData;
      }
      return station;
    });
    //sum of all station qty
    const sumOfQty = updatedStations?.reduce((accumulator, currentItem) => {
      return accumulator + currentItem.lowQuantityLimit;
    }, 0);
    quantityArr[items] = {
      ...quantityArr[items],
      lowQuantityLimit: sumOfQty,
      stationIds: updatedStations,
    };

    setTempCombinations(quantityArr);
  };
  const getValueFromRow = (items, index) => {
    {
      let val = productStations.map((ps, index) => {
        // let stationArr = [...tempCombinations];
        // let stationIds = stationArr[items].stationIds;
        // debugger;
        // if (
        //   !stationArr[items].stationIds.find((id) => id == ps.productStationId)
        // )
        //   stationIds.push({ id: ps.productStationId, qty: 0 });
        // stationArr[items] = {
        //   ...stationArr[items],
        //   stationIds: stationIds,
        // };
        // setTempCombinations(stationArr);
        return (
          <>
            <StyledTableRow key={index}>
              <StyledTableCell
                width={"50%"}
                align="left"
                component="th"
                scope="row"
              >
                {combinations[items]?.variantOption}
              </StyledTableCell>
              <StyledTableCell
                width={"20%"}
                align="center"
                component="th"
                scope="row"
              >
                {ps.sname}
              </StyledTableCell>
              <StyledTableCell width={"15%"}>
                <TextField
                  fullWidth
                  placeholder={placeholders.quantity}
                  label="Quantity"
                  onChange={(e) => {
                    handleStationQuantity(e, items, ps?.productStationId);
                  }}
                  value={
                    tempCombinations[items]?.stationIds?.find(
                      (x) => x.productStationId == ps?.productStationId
                    )?.qty
                  }
                  sx={{
                    width: 150,
                  }}
                  onFocus={handleFocus}
                  variant="outlined"
                  size="small"
                ></TextField>
              </StyledTableCell>
              <StyledTableCell width={"15%"}>
                <TextField
                  placeholder={placeholders.quantity}
                  label="Low Quantity Limit"
                  onChange={(e) => {
                    handleStationLowQuantity(
                      e,
                      items,
                      ps?.productStationId,
                      true
                    );
                  }}
                  value={
                    tempCombinations[items]?.stationIds?.find(
                      (x) => x.productStationId == ps?.productStationId
                    )?.lowQuantityLimit
                  }
                  sx={{
                    width: 150,
                  }}
                  fullWidth
                  onFocus={handleFocus}
                  variant="outlined"
                  size="small"
                ></TextField>
              </StyledTableCell>
            </StyledTableRow>
          </>
        );
      });
      return val;
    }
  };

  //#endregion

  //#endregion
  return (
    <Box sx={styleSheet.pageRoot}>
      {/* <Container maxWidth="xl" fixed sx={{ paddingLeft: "0px" }}> */}
      <div style={{ padding: "10px" }}>
        {" "}
        <br />
        <form onSubmit={handleSubmit(updateProduct)}>
          <Grid container spacing={8}>
            <Grid item md={6} xs={12}>
              <Typography sx={styleSheet.productsHeading} variant="h5">
                {LanguageReducer?.languageType?.PRODUCT_DETAIL_TEXT}
              </Typography>
              <Grid item xs={12}>
                <InputLabel sx={styleSheet.inputLabelAddProduct}>
                  {LanguageReducer?.languageType?.SELECTED_STORE_TEXT}
                </InputLabel>
                <TextField
                  sx={{
                    "& .MuiInputLabel-root": {
                      color: "grey !important",
                    },
                  }}
                  select
                  size="small"
                  id="store"
                  disabled
                  name="store"
                  fullWidth
                  variant="outlined"
                  value={data?.products?.StoreName}
                  defaultValue=""
                >
                  <MenuItem value={data?.products?.StoreName} disabled>
                    {data?.products?.StoreName}
                  </MenuItem>
                </TextField>
              </Grid>{" "}
              <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                {LanguageReducer?.languageType?.ENTER_TITLE_TEXT}
              </InputLabel>
              <TextField
                type="text"
                size="small"
                id="productName"
                name="productName"
                onFocus={handleFocus}
                value={productData?.productName}
                onChange={(e) =>
                  setProductData({
                    ...productData,
                    productName: e.target.value,
                  })
                }
                required
                fullWidth
                variant="outlined"
                placeholder="Title"
              ></TextField>
              <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                {LanguageReducer?.languageType?.ENTER_DESCRIPTION_TEXT}
              </InputLabel>
              <TextField
                placeholder="Description"
                onFocus={handleFocus}
                size="small"
                multiline
                fullWidth
                rows={3}
                value={productData?.description}
                onChange={(e) =>
                  setProductData({
                    ...productData,
                    description: e.target.value,
                  })
                }
                variant="outlined"
                id="description"
                name="description"
                required
              />
              <br />
              <br />
            </Grid>
            <Grid item md={6} xs={12}>
              <Typography
                sx={{
                  ...styleSheet.productsHeading,
                  marginBottom: "60px",
                }}
                variant="h5"
              >
                {"Feature image"}
              </Typography>
              <DragDropFile
                imageURL={productData?.featureImage}
                disabled={true}
              />
              {/* <DragDropAllFiles disabled={true} /> */}
            </Grid>
            <Grid style={{ paddingTop: "0px" }} item md={6} xs={12}>
              <Grid container spacing={3}>
                <Grid item md={6} xs={12}>
                  <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                    {LanguageReducer?.languageType?.SELECT_CATEGORY_TEXT}
                  </InputLabel>
                  <TextField
                    sx={{
                      "& .MuiInputLabel-root": {
                        color: "grey !important",
                      },
                    }}
                    size="small"
                    select
                    id="productCategoryId"
                    name="productCategoryId"
                    fullWidth
                    variant="outlined"
                    value={selectedCategory}
                    onChange={(e) => handleChangeCategory(e)}
                  >
                    <MenuItem value={selectedCategory} disabled>
                      {selectedCategory}
                    </MenuItem>
                    {productCategories.map((productCategory) => (
                      <MenuItem
                        key={productCategory.productCategoryId}
                        value={{
                          id: productCategory.productCategoryId,
                          name: productCategory.categoryName,
                        }}
                      >
                        {productCategory.categoryName}
                      </MenuItem>
                    ))}
                  </TextField>
                </Grid>
                <Grid item md={6} xs={12}>
                  <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                    {LanguageReducer?.languageType?.ENTER_WEIGHT_TEXT}
                  </InputLabel>
                  <TextField
                    placeholder={placeholders.weight}
                    onFocus={handleFocus}
                    type="number"
                    size="small"
                    id="weight"
                    name="weight"
                    value={productData?.weight}
                    inputProps={{
                      step: "any",
                    }}
                    onChange={(e) =>
                      setProductData({
                        ...productData,
                        weight: e.target.value,
                      })
                    }
                    fullWidth
                    variant="outlined"
                  ></TextField>
                </Grid>
              </Grid>
              <Grid container spacing={2}>
                <Grid item md={6} xs={12}>
                  <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                    {LanguageReducer?.languageType?.ENTER_SALE_PRICE_TEXT}
                  </InputLabel>
                  <TextField
                    placeholder={placeholders.price}
                    type="number"
                    onFocus={handleFocus}
                    size="small"
                    id="price"
                    value={productData?.price}
                    inputProps={{
                      step: "any",
                    }}
                    onChange={(e) =>
                      setProductData({
                        ...productData,
                        price: e.target.value,
                      })
                    }
                    name="price"
                    fullWidth
                    variant="outlined"
                    error={Boolean(errors.price)} // set error prop
                    helperText={errors.price?.message}
                  ></TextField>
                </Grid>
                <Grid item md={6} xs={12}>
                  <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                    {LanguageReducer?.languageType?.ENTER_PURCHASE_PRICE_TEXT}
                  </InputLabel>
                  <TextField
                    placeholder={placeholders.price}
                    type="number"
                    size="small"
                    onFocus={handleFocus}
                    id="purchasePrice"
                    name="purchasePrice"
                    value={productData?.purchasePrice}
                    inputProps={{
                      step: "any",
                    }}
                    onChange={(e) =>
                      setProductData({
                        ...productData,
                        purchasePrice: e.target.value,
                      })
                    }
                    fullWidth
                    variant="outlined"
                    error={Boolean(errors.purchasePrice)} // set error prop
                    helperText={errors.purchasePrice?.message}
                  ></TextField>
                </Grid>
              </Grid>
            </Grid>
          </Grid>
          <br />
          <br />
          <br />
          <br />
          <Grid container spacing={8}>
            <Grid item md={6} xs={12}>
              <Typography sx={styleSheet.productsHeading} variant="h5">
                {LanguageReducer?.languageType?.INVENTORY_TEXT}
              </Typography>
              <Grid item XS={12}>
                <FormGroup>
                  <FormControlLabel
                    disabled
                    onChange={(e) => {
                      setTrackInventoryCheckbox(!trackInventoryCheckbox);
                    }}
                    value={trackInventoryCheckbox}
                    control={
                      <Checkbox
                        sx={{
                          color: "var(--primary-color)",
                          "&.Mui-checked": {
                            color: "var(--primary-color)",
                          },
                        }}
                        checked={trackInventoryCheckbox}
                      />
                    }
                    label={LanguageReducer?.languageType?.TRACK_INVENTORY_TEXT}
                  />
                </FormGroup>
              </Grid>
              <Grid item xs={12}>
                <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                  {LanguageReducer?.languageType?.ENTER_SKU_TEXT}
                </InputLabel>
                <TextField
                  placeholder={placeholders.sku}
                  size="small"
                  onFocus={handleFocus}
                  id="SKU"
                  name="SKU"
                  value={productData?.sku}
                  disabled
                  fullWidth
                  variant="outlined"
                  error={Boolean(errors.SKU)}
                  helperText={errors.SKU?.message}
                ></TextField>
              </Grid>
              <br />
              {!optionsCheckbox ? (
                trackInventoryCheckbox ? (
                  <Grid item xs={12}>
                    <TableContainer component={Paper}>
                      <Table fullWidth aria-label="customized table">
                        <TableBody>
                          {productStations.map((row, i) => (
                            <StyledTableRow key={row.productStationId}>
                              <StyledTableCell
                                align="center"
                                component="th"
                                scope="row"
                              >
                                {row.sname}
                              </StyledTableCell>
                              <StyledTableCell align="right">
                                <TextField
                                  onChange={(e) => {
                                    let newArr = [...stationValues];
                                    newArr[i] = {
                                      productStationId: row.productStationId,
                                      quantityAvailable: e.target.value,
                                    };
                                    setStationValues(newArr);
                                  }}
                                  defaulValue="0"
                                  min={0}
                                  type="number"
                                  size="small"
                                  onFocus={handleFocus}
                                  fullWidth
                                  variant="outlined"
                                ></TextField>
                              </StyledTableCell>
                            </StyledTableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  </Grid>
                ) : null
              ) : null}
              <FormGroup>
                <FormControlLabel
                  disabled
                  onChange={(e) => {
                    setOptionsCheckbox(!optionsCheckbox);
                    setCombinations([]);
                    setTempCombinations([]);
                  }}
                  value={optionsCheckbox}
                  control={
                    <Checkbox
                      sx={{
                        color: "var(--primary-color)",
                        "&.Mui-checked": {
                          color: "var(--primary-color)",
                        },
                      }}
                      checked={optionsCheckbox}
                    />
                  }
                  label={
                    LanguageReducer?.languageType?.THIS_PRODUCT_HAS_OPTIONS_TEXT
                  }
                />
              </FormGroup>
              {optionsCheckbox ? (
                <>
                  {inputFields.map((selected, index) => {
                    return (
                      <Fragment key={index}>
                        <Stack direction="row">
                          <InputLabel sx={styleSheet.inputLabelAddProduct}>
                            {LanguageReducer?.languageType?.OPTION_NAME_TEXT}
                          </InputLabel>
                        </Stack>

                        <TextField
                          id="edit"
                          name="edit"
                          defaultValue={selected.selectedOption}
                          select
                          size="small"
                          fullWidth
                          variant="outlined"
                          value={selected.selectedOption}
                          disabled
                        >
                          <MenuItem value={selected.selectedOption} disabled>
                            {getProductOptionNameById(selected.selectedOption)}
                          </MenuItem>
                        </TextField>

                        <InputLabel sx={styleSheet.inputLabelAddProduct}>
                          {LanguageReducer?.languageType?.OPTION_NAME_TEXT}
                        </InputLabel>

                        <Autocomplete
                          multiple
                          variant="outlined"
                          size="small"
                          value={selected.options}
                          disabled
                          freeSolo
                          renderTags={(value, getTagProps) => {
                            return selected.options.map((option, index) => (
                              <Chip
                                key={index}
                                variant="outlined"
                                size="small"
                                label={option}
                                {...getTagProps({ index })}
                              />
                            ));
                          }}
                          renderInput={(params) => (
                            <TextField
                              {...params}
                              id="editTag"
                              variant="outlined"
                              size="small"
                            />
                          )}
                        />
                      </Fragment>
                    );
                  })}
                </>
              ) : null}
            </Grid>
            <Grid item md={6} xs={12}>
              <Button
                variant="contained"
                sx={{ ...styleSheet.addCarrierButton, float: "right" }}
                onClick={handleCreateVariant}
              >
                {"Add Variant"}
              </Button>
              <br></br>
              <br></br>
              <Typography
                sx={{ ...styleSheet.productsHeading, marginTop: "25px" }}
                variant="h5"
              >
                {LanguageReducer?.languageType?.VARIANTS_TEXT}
              </Typography>
              <br />
              <Typography sx={styleSheet.inventoryMethodHeading} variant="h6">
                {LanguageReducer?.languageType?.AVAILABLE_INVENTORY_AT_TEXT}
                {LanguageReducer?.languageType?.ALL_LOCATION_TEXT}
              </Typography>
              <hr />
              <FormGroup>
                <Box sx={{ width: "100%" }}>
                  <Paper sx={{ width: "100%", mb: 2 }}>
                    {optionsCheckbox ? (
                      <EnhancedTableToolbar
                        numSelected={selected.length}
                        handleOpenPrices={handleOpenPrices}
                        handleOpenQuantities={handleOpenQuantities}
                        handleCloseQuantities={handleCloseQuantities}
                        handleCloseSKU={handleCloseSKU}
                        handleClosePrices={handleClosePrices}
                      />
                    ) : null}
                    <TableContainer>
                      <Table
                        sx={{ width: "100%" }}
                        aria-labelledby="tableTitle"
                        size={"small"}
                      >
                        <TableHead>
                          <TableRow>
                            <TableCell padding="checkbox">
                              <Checkbox
                                sx={{
                                  color: "var(--primary-color)",
                                  "&.Mui-checked": {
                                    color: "var(--primary-color)",
                                  },
                                }}
                                indeterminate={
                                  selected.length > 0 &&
                                  selected.length < combinations.length
                                }
                                checked={
                                  combinations.length > 0 &&
                                  selected.length === combinations.length
                                }
                                onChange={handleSelectAllClick}
                                inputProps={{
                                  "aria-label": "select all desserts",
                                }}
                              />
                            </TableCell>
                            {headCells.map((headCell) => (
                              <TableCell
                                key={headCell.id}
                                align={headCell.numeric ? "right" : "left"}
                                padding={
                                  headCell.disablePadding ? "none" : "normal"
                                }
                              >
                                <TableSortLabel>
                                  {headCell.label}
                                </TableSortLabel>
                              </TableCell>
                            ))}
                          </TableRow>
                        </TableHead>
                        {optionsCheckbox ? (
                          <TableBody>
                            {combinations.map((combination, index) => {
                              const isItemSelected = isSelected(index);
                              const labelId = `enhanced-table-checkbox-${index}`;
                              return (
                                <TableRow
                                  hover
                                  onClick={(event) => handleClick(event, index)}
                                  role="checkbox"
                                  aria-checked={isItemSelected}
                                  tabIndex={-1}
                                  key={index}
                                  selected={isItemSelected}
                                  sx={{ cursor: "pointer" }}
                                >
                                  <TableCell padding="checkbox">
                                    <Checkbox
                                      sx={{
                                        color: "var(--primary-color)",
                                        "&.Mui-checked": {
                                          color: "var(--primary-color)",
                                        },
                                      }}
                                      checked={isItemSelected}
                                      inputProps={{
                                        "aria-labelledby": labelId,
                                      }}
                                    />
                                  </TableCell>
                                  <TableCell
                                    component="th"
                                    id={labelId}
                                    scope="row"
                                    padding="none"
                                  >
                                    <CardHeader
                                      avatar={
                                        <Avatar
                                          variant="square"
                                          src={ImageIcon}
                                          aria-label="Image"
                                        />
                                      }
                                      title={combination.variantOption}
                                      subheader={
                                        combination.SKU == ""
                                          ? getValues("SKU") + "-" + index
                                          : combination.SKU
                                      }
                                    />
                                  </TableCell>
                                  <TableCell align="left">
                                    <CardHeader
                                      avatar={<span></span>}
                                      title={amountFormat(combination.prices)}
                                      subheader={`${
                                        combination.quantities
                                      } Available at ${
                                        combination.stationIds
                                          ? combination.stationIds.length
                                          : 0
                                      } locations`}
                                    />
                                  </TableCell>
                                </TableRow>
                              );
                            })}
                          </TableBody>
                        ) : null}
                      </Table>
                    </TableContainer>
                  </Paper>
                </Box>
              </FormGroup>
              <div
                style={{
                  paddingTop: "20px",
                  textAlign: "Right",
                  alignItems: "Right",
                }}
              >
                {isLoading ? (
                  <Button
                    fullWidth
                    variant="contained"
                    sx={styleSheet.addStoreButton}
                    type="submit"
                  >
                    <CircularProgress sx={{ color: "white" }} />
                  </Button>
                ) : (
                  <Button
                    fullWidth
                    variant="contained"
                    sx={styleSheet.addStoreButton}
                    type="submit"
                  >
                    {LanguageReducer?.languageType?.UPDATE_PRODUCT_TEXT}
                  </Button>
                )}
              </div>
            </Grid>
            <Grid style={{ paddingTop: "0px" }} item md={6} xs={12}></Grid>
          </Grid>
        </form>
      </div>
      <br />
      <br />
      <br />
      <br />
      <Dialog
        open={open}
        TransitionComponent={Transition}
        keepMounted
        onClose={handleClose}
        maxWidth="md"
        sx={styleSheet.modelMainClassWithImage}
        aria-describedby="alert-dialog-slide-description"
      >
        <DialogContent sx={{ ...styleSheet.modelContentArea, width: "600px" }}>
          <DialogContentText id="alert-dialog-slide-description">
            <Box sx={styleSheet.editProductsHeadingAndUpload}>
              <Typography sx={styleSheet.editProductsHeading} variant="h4">
                {LanguageReducer?.languageType?.ADD_CATEGORY_TEXT}
              </Typography>
            </Box>
            <Grid container spacing={2} sx={{ mt: "5px" }}>
              <Grid item sm={12} sx={{ mt: "10px" }}>
                <Divider />
              </Grid>
              <Grid item sm={12}>
                <InputLabel sx={styleSheet.inputLabelAddProduct}>
                  {LanguageReducer?.languageType?.CATEGORY_NAME_TEXT}
                </InputLabel>
                <TextField
                  placeholder={placeholders.category_name}
                  value={categoryName}
                  onChange={(e) => {
                    setCategoryName(e.target.value);
                  }}
                  fullWidth
                  variant="outlined"
                  onFocus={handleFocus}
                  size="small"
                />
              </Grid>
            </Grid>
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button
            fullWidth
            variant="contained"
            sx={styleSheet.modelCancelButton}
            onClick={handleClose}
          >
            {"Cancel"}
          </Button>
          <Button
            fullWidth
            variant="contained"
            sx={styleSheet.addStoreButton}
            onClick={createCategory}
          >
            {LanguageReducer?.languageType?.CREATE_TEXT}
          </Button>
        </DialogActions>
      </Dialog>
      <Dialog
        open={openSKU}
        TransitionComponent={Transition}
        keepMounted
        onClose={handleCloseSKU}
        maxWidth="md"
        sx={styleSheet.modelMainClassWithImage}
        aria-describedby="alert-dialog-slide-description"
      >
        <DialogContent sx={{ ...styleSheet.modelContentArea, width: "600px" }}>
          <DialogContentText id="alert-dialog-slide-description">
            <Box sx={styleSheet.editProductsHeadingAndUpload}>
              <Typography sx={styleSheet.editProductsHeading} variant="h4">
                {LanguageReducer?.languageType?.EDIT_SKU_TEXT}
              </Typography>
            </Box>
            <Grid container spacing={2} sx={{ mt: "5px" }}>
              <Grid item sm={12} sx={{ mt: "10px" }}>
                <Divider />
              </Grid>
              <Grid item xs={12}>
                <TableContainer component={Paper}>
                  <Table fullWidth aria-label="customized table">
                    <TableBody>
                      {selected.map((items, index) => {
                        return (
                          <StyledTableRow key={index}>
                            <StyledTableCell
                              align="center"
                              component="th"
                              scope="row"
                            >
                              {combinations[items]?.variants?.map(
                                (option, optionIndex) =>
                                  optionIndex ==
                                  combinations[items]?.variants?.length - 1
                                    ? `${option}`
                                    : `${option}/`
                              )}
                            </StyledTableCell>
                            <StyledTableCell align="right">
                              <TextField
                                placeholder={placeholders.sku}
                                onChange={(e) => {
                                  let SKUArr = [...combinations];
                                  SKUArr[items] = {
                                    ...SKUArr[items],
                                    SKU: e.target.value,
                                  };
                                  setCombinations(SKUArr);
                                }}
                                value={combinations[items]?.SKU}
                                onFocus={handleFocus}
                                fullWidth
                                variant="outlined"
                                size="small"
                              ></TextField>
                            </StyledTableCell>
                          </StyledTableRow>
                        );
                      })}
                    </TableBody>
                  </Table>
                </TableContainer>
              </Grid>
            </Grid>
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button
            fullWidth
            variant="contained"
            sx={styleSheet.modelCancelButton}
            onClick={() => {
              handleCloseSKU();
            }}
          >
            {"Close"}
          </Button>
          <Button
            fullWidth
            variant="contained"
            sx={styleSheet.addStoreButton}
            onClick={() => {
              updateStockData();
              handleCloseSKU();
            }}
          >
            {LanguageReducer?.languageType?.SAVE_TEXT}
          </Button>
        </DialogActions>
      </Dialog>
      <Dialog
        fullWidth
        open={openPrices}
        TransitionComponent={Transition}
        keepMounted
        onClose={handleClosePrices}
        maxWidth="md"
        sx={styleSheet.modelMainClassWithImage}
        aria-describedby="alert-dialog-slide-description"
      >
        <DialogContent sx={{ ...styleSheet.modelContentArea }}>
          <DialogContentText id="alert-dialog-slide-description">
            <Box sx={styleSheet.editProductsHeadingAndUpload}>
              <Typography sx={styleSheet.editProductsHeading} variant="h4">
                {LanguageReducer?.languageType?.EDIT_PRICES_TEXT}
              </Typography>
            </Box>
            <Grid container spacing={2} sx={{ mt: "5px" }}>
              <Grid item sm={12} sx={{ mt: "10px" }}>
                <Divider />
              </Grid>
              <Grid item xs={12}>
                <TableContainer component={Paper}>
                  <Table fullWidth aria-label="customized table">
                    <TableBody>
                      {selected.map((items, index) => {
                        return (
                          <StyledTableRow key={index}>
                            <StyledTableCell
                              align="center"
                              component="th"
                              scope="row"
                            >
                              {combinations[items]?.variants?.map(
                                (option, optionIndex) =>
                                  optionIndex ==
                                  combinations[items]?.variants?.length - 1
                                    ? `${option}`
                                    : `${option}/`
                              )}
                            </StyledTableCell>
                            <StyledTableCell align="right">
                              <TextField
                                placeholder={placeholders.price}
                                onChange={(e) => {
                                  let priceArr = [...tempCombinations];
                                  priceArr[items] = {
                                    ...priceArr[items],
                                    prices: e.target.value,
                                  };
                                  setTempCombinations(priceArr);
                                }}
                                value={tempCombinations[items]?.prices}
                                fullWidth
                                variant="outlined"
                                size="small"
                                onFocus={handleFocus}
                              ></TextField>
                            </StyledTableCell>
                          </StyledTableRow>
                        );
                      })}
                    </TableBody>
                  </Table>
                </TableContainer>
              </Grid>
            </Grid>
          </DialogContentText>
        </DialogContent>

        <DialogActions>
          <Button
            fullWidth
            variant="contained"
            sx={styleSheet.modalDismissButton}
            onClick={handleClosePrices}
          >
            {LanguageReducer?.languageType?.DISMISS_TEXT}
          </Button>
          <Button
            fullWidth
            variant="contained"
            sx={styleSheet.modalCarrierSubmitButton}
            onClick={() => {
              updateStockData();
              handleClosePrices();
            }}
          >
            {"Edit Price"}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={openQuantities}
        TransitionComponent={Transition}
        keepMounted
        onClose={handleCloseQuantities}
        maxWidth="md"
        fullWidth
        sx={styleSheet.modelMainClassWithImage}
        aria-describedby="alert-dialog-slide-description"
      >
        <DialogContent sx={{ ...styleSheet.modelContentArea }}>
          <DialogContentText id="alert-dialog-slide-description">
            <Box sx={styleSheet.editProductsHeadingAndUpload}>
              <Typography sx={styleSheet.editProductsHeading} variant="h4">
                {LanguageReducer?.languageType?.EDIT_QUNATITIES_TEXT}
              </Typography>
            </Box>
            <Grid container spacing={2} sx={{ mt: "5px" }}>
              <Grid item sm={12} sx={{ mt: "10px" }}>
                <Divider />
              </Grid>
              <Grid item xs={12}>
                <TableContainer component={Paper}>
                  <Table fullWidth aria-label="customized table">
                    <TableBody>
                      {selected.map((items, index) => {
                        return <>{getValueFromRow(items, index)}</>;
                      })}
                    </TableBody>
                  </Table>
                </TableContainer>
              </Grid>
            </Grid>
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button
            fullWidth
            variant="contained"
            sx={styleSheet.modalDismissButton}
            onClick={handleCloseQuantities}
          >
            {LanguageReducer?.languageType?.DISMISS_TEXT}
          </Button>
          <Button
            fullWidth
            variant="contained"
            sx={styleSheet.modalCarrierSubmitButton}
            onClick={() => {
              updateStockData();
              handleCloseQuantities();
            }}
          >
            {"Update Quantity"}
          </Button>
        </DialogActions>
      </Dialog>
      <BackdropCustom open={load} />
      {/* <Backdrop
        sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
        open={load}
      >
        <CircularProgress color="inherit" />
      </Backdrop> */}
      <CreateVariantModal
        open={createVariantModal}
        handleClose={handleCreateVariantClose}
        inputFields={inputFields}
        allProductOptions={allProductOptions}
        productId={data?.products?.ProductId}
        getProductData={getProductData}
        getProductOptionNameById={getProductOptionNameById}
      ></CreateVariantModal>
    </Box>
  );
}
export default EditProducts;
