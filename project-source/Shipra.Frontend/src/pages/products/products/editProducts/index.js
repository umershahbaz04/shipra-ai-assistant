import { FormLabel } from "@material-ui/core";
import EditIcon from "@mui/icons-material/Edit";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import {
  Autocomplete,
  Avatar,
  Box,
  Button,
  CardHeader,
  Checkbox,
  Chip,
  CircularProgress,
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
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import ModalButtonComponent from "../../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../../.reUseableComponents/TextField/SelectComponent";
import {
  CreateImageGallery,
  CreateProductCategory,
  DeleteProductMediaById,
  GetAllImageGalleries,
  GetAllProductCategoryLookup,
  GetAllProductOptionLookup,
  GetProductById,
  UpdateProduct,
} from "../../../../api/AxiosInterceptors";
import { getAllStationLookupFunc } from "../../../../apiCallingFunction";
import ImageIcon from "../../../../assets/images/uploadImage.png";
import { styleSheet } from "../../../../assets/styles/style";
import DragDropFile from "../../../../components/dragAndDrop/featureImage";
import CreateVariantModal from "../../../../components/modals/myCarrierModals/CreateVariantModal";
import UtilityClass from "../../../../utilities/UtilityClass";
import { EnumOptions } from "../../../../utilities/enum";
import {
  ActionButtonCustom,
  BackdropCustom,
  CustomColorLabelledOutline,
  GridContainer,
  GridItem,
  UploadButton,
  amountFormat,
  placeholders,
  purple,
  useGetHeight,
} from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import RichTextEditor from "../../../../.reUseableComponents/RichTextEditor/RichTextEditor";
import ImageListComponent from "../../../../.reUseableComponents/ImageList/Image";
import ImageGalleryModal from "../../../../components/modals/productModals/ImageGalleryModal";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";

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
  let navigate = useNavigate();
  const location = useLocation();
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
  const [productFile, setProductFile] = useState();
  const [updateQtyShowRecord, setUpdateQtyShowRecord] = useState([]);
  const [editorValue, setEditorValue] = useState("");
  const [uploadedImages, setUploadedImages] = useState([]);
  const [openImageGalleryModal, setOpenImageGalleryModal] = useState({
    open: false,
  });
  const [variantIndexFlag, setVariantIndexFlag] = useState(false);
  const [qtySelectedStation, setQtySelectedStation] = useState([]);
  const [allGalleryImage, setallGalleryImage] = useState([]);
  const [openDeleteImageModel, setOpenDeleteImageModel] = useState({
    open: false,
    productMediaId: null,
  });
  const [deleteImageModelLoading, setDeleteImageModelLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    getValues,
    control,
  } = useForm();
  const descriptionWatch = useWatch({ name: "description", control });
  const { ref: productDetailRef, height: productDetailHeight } = useGetHeight([
    descriptionWatch,
  ]);

  const getAllImageGalleries = async () => {
    try {
      const response = await GetAllImageGalleries();
      if (response?.data?.isSuccess) {
        setallGalleryImage(response?.data.result);
      }
    } catch (e) {}
  };
  useEffect(() => {
    if (productStations.length > 0) {
      setQtySelectedStation(productStations[0]);
    }
  }, [productStations]);

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
    let targetedData = getGroupedDataFromTempArray();
    setUpdateQtyShowRecord(targetedData);
    setOpenPrices(true);
  };

  const handleCloseSKU = () => {
    setOpenSKU(false);
  };

  const handleCloseQuantities = () => {
    setOpenQuantities(false);
  };
  //#region update price
  function updatePriceBySKU(array, SKU, prices) {
    const updatedProducts = array?.map((product) => {
      if (product.SKU === SKU) {
        return {
          ...product,
          prices: prices,
        };
      }
      return product;
    });

    return updatedProducts;
  }
  const handlePriceChanged = (e, item) => {
    let prices = eval(e.target.value);
    if (!prices) {
      errorNotification("Please enter price");
      prices = item.prices;
      return;
    }
    //#region
    const showTempArr = updatePriceBySKU(tempCombinations, item?.SKU, prices);
    setTempCombinations(showTempArr);
    //#endregion
    //#region
    const showArr = updatePriceBySKU(updateQtyShowRecord, item?.SKU, prices);
    setUpdateQtyShowRecord(showArr);
    //#endregion
  };
  function updateCombinationArrayWithUpdatedPriceValues() {
    console.log("before com" + combinations.length, combinations);
    let updatedArray = [];
    updateQtyShowRecord.forEach((srecord) => {
      updatedArray = combinations.map((item) => {
        if (item.SKU === srecord.SKU) {
          // Modify the properties you want to update
          item.prices = srecord.prices; // For example, update prices to 10
        }
        return item;
      });
    });
    console.log("after com " + combinations.length, combinations);

    return updatedArray;
  }
  const handleUpdatePrice = () => {
    //set final array data
    let updatedProducts = updateCombinationArrayWithUpdatedPriceValues();
    setCombinations(updatedProducts);
    //updated group data
    var groupedData = groupData(updatedProducts);
    setGroupedCombination(groupedData);
    // setTempCombinations([]);
    // setUpdateQtyShowRecord([]);
  };
  //#endregion
  //#region update quantity
  function updateCombinationArrayWithUpdatedValues() {
    console.log("before com" + combinations.length, combinations);
    updateQtyShowRecord.forEach((srecord) => {
      const indexToUpdate = combinations.findIndex(
        (obj1) => obj1.uuid === srecord.uuid
      );
      if (indexToUpdate !== -1) {
        combinations[indexToUpdate].quantities = srecord.quantities;
        combinations[indexToUpdate].lowQuantityLimit = srecord.lowQuantityLimit;
        combinations[indexToUpdate].isAddedStationQty = true;
      }
    });
    console.log("after com " + combinations.length, combinations);

    return combinations;
  }
  const handleAddQty = () => {
    //set final array data
    let updatedProducts = updateCombinationArrayWithUpdatedValues();
    setCombinations(updatedProducts);
    //updated group data
    var groupedData = groupData(updatedProducts);
    setGroupedCombination(groupedData);
    setQtySelectedStation(productStations[0]);
    // setTempCombinations([]);
    // setUpdateQtyShowRecord([]);
  };
  //#endregion
  const handleOpenQuantities = () => {
    let targetedData = tempCombinations?.filter(
      (x) =>
        x.SKUChecked == true &&
        x.stationIds == qtySelectedStation?.productStationId
    );
    setUpdateQtyShowRecord(targetedData);
    setOpenQuantities(true);
  };
  const groupData = (data) => {
    const groupedData = {};

    data?.forEach((item) => {
      const {
        SKU,
        ORGSKU,
        quantities,
        lowQuantityLimit,
        stationIds,
        prices,
        variantOption,
        variants,
        SKUChecked,
        isAddedStationQty,
        imageUrl,
        imageGalleryId,
      } = item;

      const key = variantOption || SKU;

      if (groupedData[key]) {
        groupedData[key].quantities += (quantities || 0);
        groupedData[key].lowQuantityLimit += (lowQuantityLimit || 0);
        groupedData[key].prices = prices;
        groupedData[key].variantOption = variantOption;
        groupedData[key].SKUChecked = SKUChecked;
        groupedData[key].isAddedStationQty = isAddedStationQty;
        groupedData[key].ORGSKU = ORGSKU;
        groupedData[key].imageUrl = imageUrl || groupedData[key].imageUrl;
        groupedData[key].imageGalleryId =
          imageGalleryId || groupedData[key].imageGalleryId;
        if (variants) {
          groupedData[key].variants = variants;
        }
        if (!groupedData[key].stationIds.includes(stationIds)) {
          if (quantities > 0) {
            groupedData[key].stationIds.push(stationIds);
          }
        }
      } else {
        let stationId = quantities > 0 ? [stationIds] : [];
        groupedData[key] = {
          quantities: quantities || 0,
          lowQuantityLimit: lowQuantityLimit || 0,
          stationIds: stationId,
          prices,
          variantOption,
          variants,
          SKUChecked,
          isAddedStationQty,
          ORGSKU: ORGSKU,
          imageUrl,
          imageGalleryId,
        };
      }
    });

    const result = Object.keys(groupedData).map((key) => {
      const g = groupedData[key];
      return {
        SKU: g.ORGSKU || key,
        ORGSKU: g.ORGSKU || key,
        quantities: g.quantities,
        lowQuantityLimit: g.lowQuantityLimit,
        stationIds: g.stationIds || [],
        stationCount: (g.stationIds || []).length,
        prices: g.prices,
        variantOption: g.variantOption,
        variants: g.variants,
        SKUChecked: g.SKUChecked,
        isAddedStationQty: g.isAddedStationQty,
        imageUrl: g.imageUrl,
        imageGalleryId: g.imageGalleryId,
      };
    });

    return result;
  };
  const getGroupedDataFromTempArray = () => {
    let targetedData = tempCombinations?.filter((x) => x.SKUChecked == true);
    var gdata = groupData(targetedData);
    return gdata;
  };
  const [groupedCombination, setGroupedCombination] = useState([]);
  useEffect(() => {
    var groupedData = groupData(combinations);
    setGroupedCombination(groupedData);
  }, [combinations]);

  const handleSelectAllClick = (event) => {
    if (event.target.checked) {
      const updCombination = combinations.map((product, indx) => {
        return {
          ...product,
          SKUChecked: true,
        };
      });
      setCombinations(updCombination);
      var newSelected = groupData(updCombination);
      setGroupedCombination(newSelected);
      setSelected(newSelected);
      return;
    } else {
      const updCombination = combinations.map((product, indx) => {
        return {
          ...product,
          SKUChecked: false,
        };
      });
      setCombinations(updCombination);
      var newSelected = groupData(updCombination);
      setGroupedCombination(newSelected);
      setSelected([]);
    }
  };
  useEffect(() => {
    let targetedData = combinations?.filter((x) => x.SKUChecked == true);
    setTempCombinations(targetedData);
    //for price and sku model without filter
  }, [combinations]);
  useEffect(() => {
    //set combinations
    if (productStations.length > 0 && productData?.productStocks) {
      handleSetDefaultSkusWithStations(productData?.productStocks);
    }
  }, [productStations, productData]);
  const handleSetDefaultSkusWithStations = (stockData) => {
    let structureArray = [];
    
    // Get all distinct variants from stockData
    const uniqueVariants = [];
    const seen = new Set();
    stockData?.forEach((item) => {
      const key = item.varientOption || item.sku;
      if (!seen.has(key)) {
        seen.add(key);
        uniqueVariants.push(item);
      }
    });

    productStations.forEach((station) => {
      uniqueVariants.forEach((v) => {
        const existingStock = stockData?.find(
          (x) =>
            (x.varientOption === v.varientOption || x.sku === v.sku) &&
            x.productStationId == station.productStationId
        );

        let obj = {
          productStockId: existingStock ? (existingStock.inventoryBalanceId || existingStock.productStockId) : 0,
          uuid: UtilityClass.randomId(),
          SKU: v.sku,
          ORGSKU: v.sku,
          prices: existingStock ? existingStock.price : v.price,
          quantities: existingStock ? (existingStock.quantityAvailable || 0) : 0,
          lowQuantityLimit: existingStock ? (existingStock.lowQuantityLimit || 0) : 0,
          variants: v.varientOption ? v.varientOption.split(" / ") : [],
          stationIds: station.productStationId,
          SKUChecked: false,
          variantOption: v.varientOption,
          isAddedStationQty: false,
          imageUrl: v.imageUrl,
          imageGalleryId: v.imageGalleryId,
        };
        structureArray.push(obj);
      });
    });

    setCombinations(structureArray);
  };
  const updateProductStationQty = () => {
    //filled station value
    const updatedProductStations = productStations?.map((item) => {
      const updatedItem = productData?.productStocks?.find(
        (x) => x.productStationId == item?.productStationId
      );
      if (updatedItem) {
        return {
          ...item,
          quantityAvailable: updatedItem.quantityAvailable || 0,
          lowQuantityLimit: updatedItem.lowQuantityLimit || 0,
        };
      }
      return item;
    });
    setStationValues(updatedProductStations);
  };
  const getProductData = async () => {
    try {
      setLoad(true);
      const response = await GetProductById(data?.products?.ProductId);
      setProductData(response.data.result);
      await getAllProductCategoryLookup();
      await getAllStationLookup();
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
  const handleCombinationIsSkuCheckd = (event, name, item) => {
    let updatedCombinations = updateSKUCheckedByUuid(
      combinations,
      item?.variantOption
    );
    setCombinations(updatedCombinations);

    //update grouped combination

    var groupedData = groupData(updatedCombinations);
    let allChecked = groupedData?.filter((x) => x.SKUChecked == true);
    setSelected(allChecked);
  };
  const handleClick = (event, name, item) => {
    handleCombinationIsSkuCheckd(event, name, item);
  };

  function updateSKUCheckedByUuid(array, Sku) {
    const updatedProducts = array?.map((product) => {
      if (product.variantOption === Sku) {
        let check = product?.stationIds == 0 ? true : !product?.SKUChecked;
        return {
          ...product,
          SKUChecked: check,
        };
      }
      return product;
    });
    return updatedProducts;
  }
  const isSelected = (name) => {
    return selected.indexOf(selected.find((x) => x?.SKU == name?.SKU)) !== -1;
  };

  const headCells = [
    {
      id: "name",
      numeric: false,
      disablePadding: true,
      label: `${LanguageReducer?.languageType?.SHOWING_TEXT} ${groupedCombination.length} ${LanguageReducer?.languageType?.VARIANTS_TEXT} `,
    },
    {},
  ];

  // function generateCombinations(optionsArray) {
  //   const combinations = [[]]; // Start with an empty combination

  //   optionsArray.forEach((optionsObj) => {
  //     const newCombinations = [];

  //     optionsObj.options.forEach((option) => {
  //       combinations.forEach((combination) => {
  //         newCombinations.push([...combination, option]);
  //       });
  //     });
  //     combinations.push(...newCombinations);
  //   });

  //   return combinations.slice(1); // Exclude the empty combination
  // }
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
  // const [groupedFormatedData, setGroupedFormatedData] = useState([]);
  // const transformData = (data) => {
  //   const groupedData = {};

  //   // Group data by SKU
  //   data.forEach((item) => {
  //     const {
  //       sku,
  //       productStationId,
  //       quantityAvailable,
  //       lowQuantityLimit,
  //       varientOption,
  //     } = item;
  //     if (!groupedData[sku]) {
  //       groupedData[sku] = {
  //         sku,
  //         stationIds: [],
  //         varientOption,
  //       };
  //     }

  //     groupedData[sku].stationIds.push({
  //       productStationId,
  //       qty: quantityAvailable,
  //       lowQuantityLimit: lowQuantityLimit,
  //       varientOption,
  //     });
  //   });

  //   // Convert grouped data to the desired format
  //   const result = Object.values(groupedData).map((group) => ({
  //     sku: group.sku,
  //     stationIds: group.stationIds,
  //     varientOption: group?.varientOption,
  //   }));
  //   setGroupedFormatedData(result);
  //   return result;
  // };

  // const optionsCombinations = (optionsArray) => {
  //   // debugger;
  //   const filteredOptionsArray = optionsArray.filter((optionArray) => {
  //     return optionArray.options.length !== 0;
  //   });
  //   const combinations = generateCombinations(optionsArray);
  //   const filteredArray = combinations.filter((combination) => {
  //     return combination.length == filteredOptionsArray.length;
  //   });

  //   const groupdTransformData = productData
  //     ? transformData(productData?.productStocks)
  //     : [];

  //   let structureArray = [];
  //   for (let index = 0; index < filteredArray.length; index++) {
  //     let updatedStationValue = groupdTransformData.find(
  //       (x) => x.sku == productStocks[index]?.sku
  //     );
  //     // debugger;
  //     //debugger;
  //     structureArray[index] = {
  //       productStockId: productStocks[index]?.productStockId,
  //       SKU: productStocks[index]?.sku,
  //       prices: productStocks[index]?.price,
  //       quantities: productStocks[index]?.quantityAvailable,
  //       lowQuantityLimit: productStocks[index]?.lowQuantityLimit,
  //       variants: filteredArray[index],
  //       stationIds: updatedStationValue.stationIds,
  //       // stationIds: stockCounts[productStocks[index]?.sku],
  //       SKUChecked: false,
  //       variantOption: updatedStationValue?.varientOption,
  //     };
  //   }
  //   setTempCombinations(structureArray);
  //   setCombinations(structureArray);
  // };

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
    } else {
      updateProductStationQty();
      reset({
        description: productData.description,
      });
      if (
        productData?.productMedias?.length > 0 &&
        allGalleryImage?.length > 0
      ) {
        const matchedImages = allGalleryImage
          .map((image) => {
            const media = productData.productMedias.find(
              (pm) => pm.imageGalleryId === image.imageGalleryId
            );
            if (media) {
              return {
                ...image,
                productMediaId: media.productMediaId,
              };
            }
            return null;
          })
          .filter(Boolean);

        setUploadedImages(matchedImages);
      }
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
    // updateStockData();
  }, [combinations]);
  const updateProduct = async (formData) => {
    let productStocks = [];
    let count = 0;

    //remove 0 station value
    let filteredCombination = combinations?.filter((x) => x.stationIds != 0);
    let errForStations = [];

    if (!optionsCheckbox) {
      for (let index = 0; index < stationValues.length; index++) {
        if (stationValues[index]) {
          productStocks[count] = {
            ProductStockId: filteredCombination[index].productStockId,
            Sku: data?.products?.SKU,
            price: data?.products?.Price,
            productId: data?.products?.ProductId,
            QuantityAvailable: stationValues[index].quantityAvailable,
            LowQuantityLimit: stationValues[index].lowQuantityLimit,
            ProductStationId: stationValues[index].productStationId === 0 ? null : (stationValues[index].quantityAvailable > 0 ? stationValues[index].productStationId : null),
            VarientOption: "",
          };
          count++;
        }
      }
    } else {
      for (let index = 0; index < filteredCombination.length; index++) {
        if (filteredCombination[index]) {
          // if (filteredCombination[index].stationIds.length > 0) {
          // for (
          //   let j = 0;
          //   j < filteredCombination[index].stationIds.length;
          //   j++
          // ) {
          productStocks[count] = {
            ProductStockId: filteredCombination[index].productStockId,
            Sku:
              filteredCombination[index].SKU == ""
                ? data.SKU + "-" + index
                : filteredCombination[index].SKU,
            productId: data?.products?.ProductId,
            price: filteredCombination[index].prices,
            QuantityAvailable: filteredCombination[index].quantities,
            LowQuantityLimit: filteredCombination[index].lowQuantityLimit,
            ProductStationId: filteredCombination[index].stationIds === 0 ? null : (filteredCombination[index].quantities > 0 ? filteredCombination[index].stationIds : null),
            imageGalleryId: filteredCombination[index].imageGalleryId,
            VarientOption: filteredCombination[index].variantOption,
          };
          count++;
          //}
          // } else {
          //   errForStations.push({
          //     index: index,
          //     sku: filteredCombination[index].SKU,
          //   });
          // }
        }
      }
    }

    const imageList = uploadedImages.map((dt, index) => ({
      MediaTypeId: dt.mediaTypeId,
      ProductMediaId: dt.productMediaId || 0,
      ImageGalleryId: dt.imageGalleryId,
      IsFeatured: index === 0,
    }));
    //create variable copy
    let updatedData = productData;
    updatedData.description = formData?.description;
    updatedData.featureImage = uploadedImages[0].imageUrl;
    updatedData.productStocks = productStocks;
    updatedData.productMedias = imageList;
    //update variant count
    updatedData.varientCount = productStocks?.length;
    setIsLoading(true);
    try {
      const response = await UpdateProduct(updatedData);
      console.log("Update Product Option Response", response);
      successNotification(
        LanguageReducer?.languageType?.PRODUCT_UPDATED_SUCCESSFULLY_TOAST
      );
      getProductData();
      navigate("/products");
    } catch (error) {
      console.error("Error in updating this product", error.response);
      errorNotification(
        LanguageReducer?.languageType?.UNABLE_TO_UPDATE_THE_PRODUCT_TOAST
      );
    } finally {
      setIsLoading(false);
    }
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
  function updateQuantityByUuid(array, uuid, newQuantity) {
    const updatedProducts = array?.map((product) => {
      if (product.uuid === uuid) {
        let llQty = product.lowQuantityLimit;
        if (newQuantity == 0) {
          llQty = 0;
        }
        return {
          ...product,
          quantities: newQuantity,
          lowQuantityLimit: llQty,
        };
      }
      return product;
    });

    return updatedProducts;
  }
  const handleStationQuantity = (e, item) => {
    let qty = eval(e.target.value) ? eval(e.target.value) : 0;
    //#region
    const showTempArr = updateQuantityByUuid(tempCombinations, item?.uuid, qty);
    setTempCombinations(showTempArr);
    //#endregion
    //#region
    const showArr = updateQuantityByUuid(updateQtyShowRecord, item?.uuid, qty);
    setUpdateQtyShowRecord(showArr);
    //#endregion
  };
  function updateLowQuantityByUuid(array, uuid, newQuantity) {
    const updatedProducts = array?.map((product) => {
      if (product.uuid === uuid) {
        return {
          ...product,
          lowQuantityLimit: newQuantity,
        };
      }
      return product;
    });

    return updatedProducts;
  }
  const handleStationLowQuantity = (e, item) => {
    let qty = eval(e.target.value) ? eval(e.target.value) : 0;
    //#region
    const showTempArr = updateLowQuantityByUuid(
      tempCombinations,
      item?.uuid,
      qty
    );
    setTempCombinations(showTempArr);
    //#endregion
    //#region
    const showArr = updateLowQuantityByUuid(
      updateQtyShowRecord,
      item?.uuid,
      qty
    );
    setUpdateQtyShowRecord(showArr);
    //#endregion
  };

  const handleDeleteProductMediaById = async () => {
    if (!openDeleteImageModel.productMediaId) return;

    setDeleteImageModelLoading(true);
    try {
      const response = await DeleteProductMediaById(
        openDeleteImageModel.productMediaId
      );
      if (response.data.isSuccess) {
        successNotification("Image Delete Successfully");
        setUploadedImages((prev) =>
          prev.filter(
            (img) => img.productMediaId !== openDeleteImageModel.productMediaId
          )
        );
      }
    } catch (error) {
      console.error(error);
    } finally {
      setDeleteImageModelLoading(false);
      setOpenDeleteImageModel({ open: false, productMediaId: null });
    }
  };

  const handleRemoveImage = (index) => {
    const image = uploadedImages[index];

    if (image.productMediaId) {
      setOpenDeleteImageModel({
        open: true,
        productMediaId: image.productMediaId,
      });
    } else {
      const updatedImages = [...uploadedImages];
      updatedImages.splice(index, 1);
      setUploadedImages(updatedImages);
    }
  };

  const handleFileUpload = async (event) => {
    const files = event.target.files;
    const newFiles = [];

    for (let i = 0; i < files.length; i++) {
      newFiles.push({
        img: URL.createObjectURL(files[i]),
        title: files[i].name,
        file: files[i],
      });
    }

    const formData = new FormData();

    newFiles.forEach((item, index) => {
      formData.append(`List[${index}].File`, item.file);
      formData.append(`List[${index}].Description`, "");
    });

    if (newFiles.length > 0) {
      try {
        const response = await CreateImageGallery(formData);
        if (response.data.isSuccess) {
          setUploadedImages((prev) => [...prev, ...response.data.result]);
          successNotification("Image Upload Successfully");
        }
      } catch (e) {}
    }
  };

  const handleReorder = (newImageList) => {
    setUploadedImages(newImageList);
  };

  const getValueFromRow = (items, index) => {
    {
      let val = updateQtyShowRecord?.map((items, index) => {
        let disabled = qtySelectedStation.productStationId == 0;
        return (
          <>
            <StyledTableRow key={index}>
              <StyledTableCell
                width={"50%"}
                align="left"
                component="th"
                scope="row"
              >
                {items?.variantOption}
              </StyledTableCell>
              <StyledTableCell width={"15%"}>
                <TextField
                  fullWidth
                  placeholder={placeholders.quantity}
                  label="Quantity"
                  onChange={(e) => {
                    handleStationQuantity(e, items);
                  }}
                  value={items?.quantities}
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
                    handleStationLowQuantity(e, items, true);
                  }}
                  value={items?.lowQuantityLimit}
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
  useEffect(() => {
    // debugger;
    let targetedData = tempCombinations?.filter(
      (x) =>
        x.stationIds == qtySelectedStation?.productStationId &&
        x.SKUChecked == true
    );
    setUpdateQtyShowRecord(targetedData);
  }, [qtySelectedStation]);

  //#endregion
  const [isLowerPrice, setIsLowerPrice] = useState(false);

  useEffect(() => {
    let sp = productData?.price || 0;
    let pp = productData?.purchasePrice || 0;
    if (parseInt(sp) && parseInt(pp)) {
      if (parseInt(sp) < parseInt(pp)) {
        setIsLowerPrice(true);
      } else {
        setIsLowerPrice(false);
      }
    } else {
      setIsLowerPrice(false);
    }
  }, [productData?.price, productData?.purchasePrice]);

  useEffect(() => {
    getAllImageGalleries();
  }, []);
  console.log({ productDetailRef, productDetailHeight, descriptionWatch });
  //#endregion
  return (
    <Box sx={styleSheet.pageRoot}>
      {/* <Container maxWidth="xl" fixed sx={{ paddingLeft: "0px" }}> */}
      <div style={{ padding: "10px" }}>
        <form onSubmit={handleSubmit(updateProduct)}>
          <GridContainer>
            <GridItem xs={12} textAlign={"right"}>
              <ActionButtonCustom
                onClick={() => {
                  navigate("/products");
                }}
                variant="contained"
                label={
                  LanguageReducer?.languageType?.PRODUCTS_PRODUCT_DASHBOARD
                }
              />
            </GridItem>
            {/* product detail box */}
            <GridItem xs={12} sm={12} md={6}>
              <CustomColorLabelledOutline
                label={LanguageReducer?.languageType?.PRODUCT_DETAIL_TEXT}
                height={"100%"}
              >
                <div ref={productDetailRef}>
                  <GridContainer>
                    <GridItem xs={12} sm={12} md={12}>
                      <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                        {LanguageReducer?.languageType?.TITLE_TEXT}
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
                        placeholder={placeholders.category_name}
                      ></TextField>
                    </GridItem>
                    <GridItem xs={12} sm={12}>
                      <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                        {LanguageReducer?.languageType?.DESCRIPTION_TEXT}
                      </InputLabel>
                      <RichTextEditor
                        name="description"
                        control={control}
                        required={true}
                      />
                    </GridItem>
                    <Grid item md={6} xs={12} sm={6}>
                      <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                        {LanguageReducer?.languageType?.CATEGORY_TEXT}
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
                    <Grid item md={6} xs={12} sm={6}>
                      <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                        {LanguageReducer?.languageType?.WEIGHT_TEXT}
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
                    <Grid item md={6} xs={12} sm={6}>
                      <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                        {LanguageReducer?.languageType?.SALE_PRICE_TEXT}
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
                    <Grid item md={6} xs={12} sm={6}>
                      <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                        {LanguageReducer?.languageType?.PURCHASE_PRICE_TEXT}
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
                      {isLowerPrice && (
                        <FormLabel sx={{ fontSize: "10px" }}>
                          *Your Purchase price is greater than sale price
                        </FormLabel>
                      )}
                    </Grid>
                  </GridContainer>{" "}
                </div>
              </CustomColorLabelledOutline>
            </GridItem>
            {/* feature img box */}
            <GridItem xs={12} sm={12} md={6}>
              <CustomColorLabelledOutline
                label={"Feature Image"}
                height={"100%"}
              >
                <>
                  {/* Show uploaded images if available */}
                  {Object.keys(uploadedImages).length > 0 ? (
                    <ImageListComponent
                      multiple={true}
                      itemData={uploadedImages}
                      cols={4}
                      height={productDetailHeight}
                      rowHeight={160}
                      handleRemoveImage={handleRemoveImage}
                      onUploadClick={() =>
                        setOpenImageGalleryModal((prev) => ({
                          ...prev,
                          open: true,
                        }))
                      }
                      onReorder={handleReorder}
                    />
                  ) : (
                    <Box
                      sx={{
                        display: "flex",
                        gap: 0.5,
                        alignItems: "center",
                        justifyContent: "center",
                        flexDirection: "column",
                        height: "345px",
                        backgroundColor: "#F8F8F8",
                      }}
                    >
                      <Box>
                        <img
                          src={ImageIcon}
                          alt="Upload"
                          style={{ height: "60px" }}
                        />
                      </Box>
                      <Box>
                        <Typography
                          sx={{
                            color: "grey",
                            fontSize: "30px",
                            textAlign: "center",
                          }}
                          component="span"
                        >
                          Upload image
                        </Typography>
                      </Box>
                      <Box sx={{ display: "flex", gap: 0.5 }}>
                        <UploadButton
                          onChange={handleFileUpload}
                          label="Upload Image"
                          multiple={true}
                          accept="image/jpeg, image/png, image/gif, image/heic, image/webp"
                        />
                        <Button
                          variant="outlined"
                          sx={{
                            color: "#616161",
                            border: "1px solid #616161",
                            fontSize: "12px",
                            textTransform: "capitalize",
                            height: "29px",
                          }}
                          onClick={() =>
                            setOpenImageGalleryModal((prev) => ({
                              ...prev,
                              open: true,
                            }))
                          }
                        >
                          Select Existing
                        </Button>
                      </Box>
                    </Box>
                  )}
                </>
              </CustomColorLabelledOutline>
            </GridItem>
            {/* inventory box */}
            <GridItem xs={12} sm={12} md={!optionsCheckbox ? 12 : 6}>
              <CustomColorLabelledOutline
                label={LanguageReducer?.languageType?.INVENTORY_TEXT}
              >
                <GridContainer>
                  <Grid item xs={12}>
                    <Grid item xs={12}>
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
                          label={
                            LanguageReducer?.languageType?.TRACK_INVENTORY_TEXT
                          }
                        />
                      </FormGroup>
                    </Grid>
                    <Grid item xs={12}>
                      <InputLabel required sx={styleSheet.inputLabelAddProduct}>
                        {LanguageReducer?.languageType?.SKU_TEXT}
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
                                {stationValues.map((row, i) => {
                                  return (
                                    <>
                                      {row.productStationId > 0 && (
                                        <StyledTableRow
                                          key={row.productStationId}
                                        >
                                          <StyledTableCell
                                            align="center"
                                            component="th"
                                            scope="row"
                                          >
                                            {row.sname}
                                          </StyledTableCell>
                                          <StyledTableCell align="right">
                                            <Box display={"flex"} gap={1}>
                                              <TextField
                                                disabled
                                                onChange={(e) => {
                                                  let newArr = [
                                                    ...stationValues,
                                                  ];
                                                  newArr[i] = {
                                                    sname: newArr[i].sname,
                                                    productStationId:
                                                      row.productStationId,
                                                    quantityAvailable:
                                                      eval(e.target.value) || 0,
                                                    lowQuantityLimit: newArr[i]
                                                      ?.lowQuantityLimit
                                                      ? newArr[i]
                                                          .lowQuantityLimit
                                                      : 0,
                                                  };
                                                  setStationValues(newArr);
                                                }}
                                                defaulValue={
                                                  row.quantityAvailable || 0
                                                }
                                                value={
                                                  row.quantityAvailable || 0
                                                }
                                                min={0}
                                                type="number"
                                                size="small"
                                                onFocus={handleFocus}
                                                fullWidth
                                                variant="outlined"
                                                label="Quantity"
                                              ></TextField>
                                              <TextField
                                                disabled
                                                onChange={(e) => {
                                                  let newArr = [
                                                    ...stationValues,
                                                  ];
                                                  newArr[i] = {
                                                    sname: newArr[i].sname,
                                                    productStationId:
                                                      row.productStationId,
                                                    lowQuantityLimit:
                                                      eval(e.target.value) || 0,
                                                    quantityAvailable: newArr[i]
                                                      ?.quantityAvailable
                                                      ? newArr[i]
                                                          .quantityAvailable
                                                      : 0,
                                                  };

                                                  setStationValues(newArr);
                                                }}
                                                defaulValue={
                                                  row.lowQuantityLimit || 0
                                                }
                                                value={
                                                  row.lowQuantityLimit || 0
                                                }
                                                min={0}
                                                type="number"
                                                size="small"
                                                onFocus={handleFocus}
                                                fullWidth
                                                variant="outlined"
                                                label="Low Quantity Amount"
                                              ></TextField>
                                            </Box>
                                          </StyledTableCell>
                                        </StyledTableRow>
                                      )}
                                    </>
                                  );
                                })}
                              </TableBody>
                            </Table>
                          </TableContainer>
                        </Grid>
                      ) : null
                    ) : null}
                    {optionsCheckbox && (
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
                            LanguageReducer?.languageType
                              ?.THIS_PRODUCT_HAS_OPTIONS_TEXT
                          }
                        />
                      </FormGroup>
                    )}
                    {optionsCheckbox ? (
                      <>
                        {inputFields.map((selected, index) => {
                          return (
                            <Fragment key={index}>
                              <Stack direction="row">
                                <InputLabel
                                  sx={styleSheet.inputLabelAddProduct}
                                >
                                  {
                                    LanguageReducer?.languageType
                                      ?.OPTION_NAME_TEXT
                                  }
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
                                <MenuItem
                                  value={selected.selectedOption}
                                  disabled
                                >
                                  {getProductOptionNameById(
                                    selected.selectedOption
                                  )}
                                </MenuItem>
                              </TextField>

                              <InputLabel sx={styleSheet.inputLabelAddProduct}>
                                {"Option Value"}
                              </InputLabel>

                              <Autocomplete
                                multiple
                                variant="outlined"
                                size="small"
                                value={selected.options}
                                disabled
                                freeSolo
                                renderTags={(value, getTagProps) => {
                                  return selected.options.map(
                                    (option, index) => (
                                      <Chip
                                        key={index}
                                        variant="outlined"
                                        size="small"
                                        label={option}
                                        {...getTagProps({ index })}
                                      />
                                    )
                                  );
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
                  {!optionsCheckbox && (
                    <Grid item md={12} sm={12}>
                      <div
                        style={{
                          paddingTop: "20px",
                          textAlign: "Right",
                          alignItems: "Right",
                        }}
                      >
                        <ActionButtonCustom
                          sx={{
                            ...styleSheet.integrationactivatedButton,
                            width: "100%",
                            height: "31px",
                            borderRadius: "4px",
                          }}
                          type="submit"
                          variant="contained"
                          loading={isLoading}
                          label={
                            LanguageReducer?.languageType?.UPDATE_PRODUCT_TEXT
                          }
                        />
                      </div>
                    </Grid>
                  )}
                </GridContainer>
              </CustomColorLabelledOutline>
            </GridItem>
            {/* varients box */}
            <GridItem xs={12} sm={12} md={6}>
              {optionsCheckbox && (
                <CustomColorLabelledOutline
                  label={LanguageReducer?.languageType?.VARIANTS_TEXT}
                >
                  <GridContainer>
                    <Grid item xs={12}>
                      <Grid>
                        <Button
                          variant="contained"
                          sx={{
                            ...styleSheet.addCarrierButton,
                            float: "right",
                          }}
                          onClick={handleCreateVariant}
                        >
                          {"Add Variant"}
                        </Button>

                        <br />
                        <Typography
                          sx={styleSheet.inventoryMethodHeading}
                          variant="h6"
                        >
                          {
                            LanguageReducer?.languageType
                              ?.AVAILABLE_INVENTORY_AT_TEXT
                          }
                          {LanguageReducer?.languageType?.ALL_LOCATION_TEXT}
                        </Typography>
                        <hr />
                        <FormGroup>
                          <Box sx={{ width: "100%" }}>
                            <Paper
                              sx={{ width: "100%", mb: 2, overflow: "hidden" }}
                            >
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
                                            selected.length ===
                                            groupedCombination.length
                                          }
                                          checked={
                                            selected.length ===
                                            groupedCombination.length
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
                                          align={
                                            headCell.numeric ? "right" : "left"
                                          }
                                          padding={
                                            headCell.disablePadding
                                              ? "none"
                                              : "normal"
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
                                      {groupedCombination?.map(
                                        (combination, index) => {
                                          const isItemSelected =
                                            isSelected(combination); //combination.SKUChecked;
                                          const labelId = `enhanced-table-checkbox-${index}`;
                                          return (
                                            <TableRow
                                              hover
                                              onClick={(event) =>
                                                handleClick(
                                                  event,
                                                  index,
                                                  combination
                                                )
                                              }
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
                                                  sx={{ padding: "8px" }}
                                                  avatar={
                                                    <Avatar
                                                      variant="square"
                                                      src={
                                                        combination?.imageUrl ||
                                                        ImageIcon
                                                      }
                                                      aria-label="Image"
                                                    />
                                                  }
                                                  titleTypographyProps={{
                                                    sx: {
                                                      maxWidth: {
                                                        md: "500px",
                                                        sm: "500px",
                                                        xs: "200px",
                                                      },
                                                      wordBreak: "break-word",
                                                      whiteSpace: "normal",
                                                    },
                                                  }}
                                                  title={
                                                    combination.variantOption
                                                  }
                                                  subheader={
                                                    combination.SKU == ""
                                                      ? getValues("SKU") +
                                                        "-" +
                                                        index
                                                      : combination.SKU
                                                  }
                                                  subheaderTypographyProps={{
                                                    maxWidth: {
                                                      md: "500px",
                                                      sm: "500px",
                                                      xs: "65px",
                                                    },
                                                    wordBreak: "break-word",
                                                    whiteSpace: "normal",
                                                  }}
                                                />
                                              </TableCell>
                                              <TableCell
                                                align="left"
                                                sx={{
                                                  padding: "8px 6px !important",
                                                }}
                                              >
                                                <CardHeader
                                                  sx={{ padding: "0px" }}
                                                  d
                                                  avatar={<span></span>}
                                                  title={amountFormat(
                                                    combination.prices
                                                  )}
                                                  subheader={`${
                                                    combination.quantities
                                                  } Available at ${
                                                    combination.stationIds
                                                      ? combination.stationIds
                                                          .length
                                                      : 0
                                                  } locations`}
                                                />
                                              </TableCell>
                                            </TableRow>
                                          );
                                        }
                                      )}
                                    </TableBody>
                                  ) : null}
                                </Table>
                              </TableContainer>
                            </Paper>
                          </Box>
                        </FormGroup>
                      </Grid>
                    </Grid>
                    <Grid item md={12} sm={12}>
                      <div
                        style={{
                          paddingTop: "20px",
                          textAlign: "Right",
                          alignItems: "Right",
                        }}
                      >
                        <ActionButtonCustom
                          sx={{
                            ...styleSheet.integrationactivatedButton,
                            width: "100%",
                            height: "31px",
                            borderRadius: "4px",
                          }}
                          type="submit"
                          variant="contained"
                          loading={isLoading}
                          label={
                            LanguageReducer?.languageType?.UPDATE_PRODUCT_TEXT
                          }
                        />
                      </div>
                    </Grid>
                  </GridContainer>
                </CustomColorLabelledOutline>
              )}
            </GridItem>
          </GridContainer>
        </form>
      </div>
      <ModalComponent
        open={open}
        onClose={handleClose}
        maxWidth="md"
        title={LanguageReducer?.languageType?.ADD_CATEGORY_TEXT}
        actionBtn={
          <ModalButtonComponent
            title={LanguageReducer?.languageType?.CREATE_TEXT}
            // loading={isLoading}
            bg={purple}
            onClick={createCategory}
          />
        }
        style={{ overflow: "hidden", height: "unset" }}
      >
        <Box>
          <InputLabel sx={styleSheet.inputLabelAddProduct}>
            {LanguageReducer?.languageType?.CATEGORY_NAME_TEXT}
          </InputLabel>
          <TextField
            placeholder="Category Name"
            value={categoryName}
            onChange={(e) => {
              setCategoryName(e.target.value);
            }}
            fullWidth
            variant="outlined"
            onFocus={handleFocus}
            size="small"
          />
        </Box>
      </ModalComponent>
      <ModalComponent
        open={openSKU}
        onClose={handleCloseSKU}
        maxWidth="md"
        title={LanguageReducer?.languageType?.EDIT_SKU_TEXT}
        actionBtn={
          <ModalButtonComponent
            title={LanguageReducer?.languageType?.SAVE_TEXT}
            // loading={isLoading}
            bg={purple}
            onClick={() => {
              updateStockData();
              handleCloseSKU();
            }}
          />
        }
        style={{ overflow: "hidden", height: "unset" }}
      >
        <TableContainer component={Paper}>
          <Table fullWidth aria-label="customized table">
            <TableBody>
              {selected.map((items, index) => {
                return (
                  <StyledTableRow key={index}>
                    <StyledTableCell align="center" component="th" scope="row">
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
      </ModalComponent>
      <ModalComponent
        open={openPrices}
        onClose={handleClosePrices}
        maxWidth="md"
        title={LanguageReducer?.languageType?.EDIT_PRICES_TEXT}
        actionBtn={
          <ModalButtonComponent
            title={"Edit Price"}
            // loading={isLoading}
            bg={purple}
            onClick={() => {
              handleUpdatePrice();
              handleClosePrices();
            }}
          />
        }
        style={{ overflow: "hidden", height: "unset" }}
      >
        <TableContainer component={Paper}>
          <Table fullWidth aria-label="customized table">
            <TableBody>
              {updateQtyShowRecord.map((items, index) => {
                return (
                  <StyledTableRow key={index}>
                    <StyledTableCell align="center" component="th" scope="row">
                      {items?.variantOption}
                    </StyledTableCell>
                    <StyledTableCell align="right">
                      <TextField
                        placeholder={placeholders.price}
                        onChange={(e) => {
                          handlePriceChanged(e, items);
                        }}
                        value={items?.prices}
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
      </ModalComponent>
      <ModalComponent
        open={openQuantities}
        onClose={handleCloseQuantities}
        maxWidth="md"
        title={LanguageReducer?.languageType?.EDIT_QUNATITIES_TEXT}
        actionBtn={
          <ModalButtonComponent
            title={"Update Quantity"}
            // loading={isLoading}
            bg={purple}
            onClick={() => {
              handleAddQty();
              handleCloseQuantities();
            }}
          />
        }
        style={{ overflow: "hidden", height: "unset" }}
      >
        <Grid container spacing={2}>
          <Grid item sm={12} md={12} lg={12}>
            <SelectComponent
              name="reason"
              closeIcon={null}
              options={productStations}
              defaulValue={qtySelectedStation}
              value={qtySelectedStation}
              optionLabel={EnumOptions.SELECT_STATION.LABEL}
              optionValue={EnumOptions.SELECT_STATION.VALUE}
              onChange={(e, newValue) => {
                const resolvedId = newValue ? newValue : productStations[0];
                setQtySelectedStation(resolvedId);
              }}
              sx={{}}
            />
          </Grid>
          <Grid item xs={12}>
            <TableContainer component={Paper}>
              <Table fullWidth aria-label="customized table">
                <TableBody>{getValueFromRow()}</TableBody>
              </Table>
            </TableContainer>
          </Grid>
        </Grid>
      </ModalComponent>

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
        productStations={productStations}
      ></CreateVariantModal>
      {openImageGalleryModal.open && (
        <ImageGalleryModal
          open={openImageGalleryModal.open}
          onClose={() => {
            setOpenImageGalleryModal((prev) => ({
              ...prev,
              open: false,
            }));
            setVariantIndexFlag(false);
          }}
          uploadedImages={uploadedImages}
          setUploadedImages={setUploadedImages}
          allGalleryImage={allGalleryImage}
        />
      )}
      {openDeleteImageModel.open && (
        <DeleteConfirmationModal
          open={openDeleteImageModel.open}
          setOpen={() =>
            setOpenDeleteImageModel({ open: false, productMediaId: null })
          }
          handleDelete={handleDeleteProductMediaById}
          loading={deleteImageModelLoading}
          heading="Are you sure you want to delete this image"
          message="This image will be permanently removed from this product and cannot be recovered."
          buttonText="Delete"
        />
      )}
    </Box>
  );
}
export default EditProducts;
