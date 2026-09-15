import {
  Avatar,
  Box,
  Checkbox,
  InputAdornment,
  InputLabel,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useForm, useWatch } from "react-hook-form";
import { useSelector } from "react-redux";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import {
  AssignStoreProduct,
  GetAllProducts,
  GetStoresForSelection,
} from "../../../api/AxiosInterceptors";
import { styleSheet } from "../../../assets/styles/style";
import { EnumOptions } from "../../../utilities/enum";
import SearchIcon from "../../../assets/images/topNav/Search.png";
import {
  fetchMethod,
  GridContainer,
  GridItem,
  purple,
} from "../../../utilities/helpers/Helpers";
import {
  successNotification,
  warningNotification,
} from "../../../utilities/toast";
import SearchInputAutoCompleteMultiple from "../../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";

const AssignStoreToProductModal = (props) => {
  const { open, onClose, getAllProductslist } = props;
  const {
    register,
    handleSubmit,
    formState: { errors },
    getValues,
    setValue,
    control,
  } = useForm();
  const selectedStore = useWatch({
    name: "store",
    control,
  });
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [storesForSelection, setStoresForSelection] = useState([]);
  const [allProduct, setAllProduct] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selectedProductIds, setSelectedProductIds] = useState([]);
  const [inputFields, setInputFields] = useState([]);
  const MAX_TAGS = 200;

  const handleFocus = (event) => event.target.select();

  const getStoresForSelection = async () => {
    try {
      const response = await GetStoresForSelection();
      if (response.data.isSuccess) {
        setStoresForSelection(response.data.result);
      }
    } catch {}
  };

  let getAllProducts = async () => {
    const { response } = await fetchMethod(() =>
      GetAllProducts(null, null, "", -1)
    );
    if (response.result !== null) {
      setAllProduct(response.result.list);
    }
  };

  // Toggle single product checkbox
  const handleCheckboxChange = (productId) => {
    const product = selectedProductIds.find((p) => p.ProductId === productId);
    setSelectedProductIds((prev) => {
      const existing = prev.find((item) => item.ProductId === productId);
      if (existing) {
        if (!product?.isExist) {
          return prev.filter((item) => item.ProductId !== productId);
        } else {
          return prev.map((item) =>
            item.ProductId === productId
              ? { ...item, active: !item.active }
              : item
          );
        }
      } else {
        if (product?.isExist) {
          return prev;
        } else {
          return [...prev, { ProductId: productId, active: true }];
        }
      }
    });
  };

  const handleSelectAllCheckboxChange = (e) => {
    const isChecked = e.target.checked;
    setSelectedProductIds((prev) => {
      const updated = [...prev];

      if (isChecked) {
        filteredProduct.forEach((product) => {
          const exists = updated.find((p) => p.ProductId === product.ProductId);
          if (!exists) {
            updated.push({ ProductId: product.ProductId, active: true });
          } else {
            exists.active = true;
          }
        });
      } else {
        return updated.reduce((acc, item) => {
          const isInFiltered = filteredProduct.some(
            (p) => p.ProductId === item.ProductId
          );
          if (isInFiltered) {
            if (item.isExist) {
              acc.push({ ...item, active: false });
            }
          } else {
            acc.push(item);
          }
          return acc;
        }, []);
      }

      return updated;
    });
  };

  const handleAssignStore = async (dt) => {
    const body = {
      storeId: dt?.store?.storeId,
      list: selectedProductIds,
    };
    setLoading(true);
    try {
      const response = await AssignStoreProduct(body);
      if (response.data.isSuccess) {
        successNotification("Product assigned to store successfully.");
        onClose();
        getAllProductslist();
      }
    } catch {
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (selectedStore?.storeId && allProduct.length > 0) {
      const matchingProductIds = allProduct
        .filter((item) =>
          item.StoreIds?.split(",")
            .map((id) => id.trim())
            .includes(String(selectedStore.storeId))
        )
        .map((item) => ({
          ProductId: item.ProductId,
          active: true,
          isExist: true,
        }));

      setSelectedProductIds(matchingProductIds);
    } else {
      setSelectedProductIds([]);
    }
  }, [selectedStore, allProduct]);

  useEffect(() => {
    getStoresForSelection();
  }, []);

  useEffect(() => {
    getAllProducts();
  }, []);

  const filteredProduct =
    inputFields.length === 0
      ? allProduct
      : allProduct.filter((item) =>
          inputFields.some((field) =>
            item.SKU.toLowerCase().includes(field.toLowerCase())
          )
        );

  return (
    <ModalComponent
      open={open}
      onClose={onClose}
      maxWidth="lg"
      title={"Assign products to Store"}
      paddingBottom={"0px !important"}
      paddingTop={"7px !important"}
      actionBtn={
        <ModalButtonComponent
          loading={loading}
          title={"Save"}
          bg={purple}
          onClick={handleSubmit(handleAssignStore)}
        />
      }
      component={"form"}
    >
      <GridContainer spacing={1}>
        <GridItem
          xs={12}
          sm={12}
          md={12}
          lg={12}
          paddingTop={
            selectedStore?.storeId > 0 ? "2px !important" : "8px !important"
          }
        >
          <InputLabel required sx={styleSheet.inputLabel}>
            {LanguageReducer?.languageType?.ORDERS_STORE}
          </InputLabel>
          <SelectComponent
            name="store"
            control={control}
            options={storesForSelection}
            isRHF={true}
            required={true}
            optionLabel={EnumOptions.STORE.LABEL}
            optionValue={EnumOptions.STORE.VALUE}
            isRefesh={true}
            handleRefreshClick={getStoresForSelection}
            {...register("store", {
              required: {
                value: true,
              },
            })}
            value={getValues("store")}
            onChange={(event, newValue) => {
              const resolvedId = newValue ? newValue : null;
              setValue("store", resolvedId);
            }}
            errors={errors}
          />
        </GridItem>
        {selectedStore?.storeId > 0 && (
          <>
            <GridItem
              xs={12}
              sm={12}
              md={12}
              lg={12}
              paddingTop={"6px !important"}
              paddingBottom={"2px !important"}
            >
              <SearchInputAutoCompleteMultiple
                padding={"0px !important"}
                handleFocus={handleFocus}
                placeholder="Enter Sku."
                onChange={(e, value) => {
                  if (value.length <= MAX_TAGS) {
                    setInputFields(value.slice(0, MAX_TAGS));
                  } else {
                    warningNotification(
                      LanguageReducer?.languageType?.MAXMIUM_NUMBER_REACHED
                    );
                  }
                }}
                inputFields={inputFields}
                MAX_TAGS={MAX_TAGS}
              />
            </GridItem>
            <GridItem
              xs={12}
              sm={12}
              md={12}
              lg={12}
              paddingTop={"2px !important"}
            >
              <TableContainer component={Paper}>
                <Table>
                  {/* real header: never moves */}
                  <TableHead
                    component="div"
                    sx={{
                      display: "table-header-group",
                      backgroundColor: "#f5f5f5",
                      borderBottom: "2px solid #ddd",
                    }}
                  >
                    <TableRow
                      component="div"
                      sx={{
                        display: "flex",
                        alignItems: "center",
                        height: "40px !important",
                      }}
                    >
                      <TableCell
                        component="div"
                        sx={{
                          flex: "0 0 64px",
                          fontWeight: "bold",
                          padding: "0px 7px !important",
                          borderBottom: "none !important",
                        }}
                      >
                        <Checkbox
                          checked={
                            filteredProduct.length > 0 &&
                            filteredProduct.every((item) => {
                              const found = selectedProductIds.find(
                                (p) => p.ProductId === item.ProductId
                              );
                              return found?.active || found?.isExist;
                            })
                          }
                          indeterminate={
                            filteredProduct.some(
                              (item) =>
                                selectedProductIds.find(
                                  (p) => p.ProductId === item.ProductId
                                )?.active
                            ) &&
                            !filteredProduct.every((item) => {
                              const found = selectedProductIds.find(
                                (p) => p.ProductId === item.ProductId
                              );
                              return found?.active || found?.isExist;
                            })
                          }
                          onChange={handleSelectAllCheckboxChange}
                        />
                      </TableCell>
                      <TableCell
                        component="div"
                        sx={{
                          flex: 0.5,
                          borderBottom: "none !important",
                          padding: "0px 7px !important",
                        }}
                      >
                        Product Name
                      </TableCell>
                      <TableCell
                        component="div"
                        sx={{
                          flex: 0.5,
                          borderBottom: "none !important",
                          padding: "0px 7px !important",
                        }}
                      >
                        SKU
                      </TableCell>
                      <TableCell
                        component="div"
                        sx={{
                          flex: 0.5,
                          borderBottom: "none !important",
                          padding: "0px 7px !important",
                        }}
                      >
                        Category Name
                      </TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody
                    component="div"
                    sx={{
                      display: "block",
                      maxHeight: "370px",
                      overflowY: "auto",
                      overflowX: "hidden",
                    }}
                  >
                    {filteredProduct.map((item) => (
                      <TableRow
                        key={item.productId}
                        component="div"
                        sx={{
                          display: "flex",
                          "&:last-child td, &:last-child th": { border: 0 },
                          background: selectedProductIds.find(
                            (p) => p.ProductId === item.ProductId && p.active
                          )
                            ? "rgba(86, 58, 213, .2)"
                            : "transparent",
                        }}
                      >
                        <TableCell
                          component="div"
                          sx={{
                            flex: "0 0 64px",
                            padding: "0px 7px !important",
                            display: "flex",
                            alignItems: "center",
                          }}
                        >
                          <Checkbox
                            checked={
                              selectedProductIds.find(
                                (p) => p.ProductId === item.ProductId
                              )?.active || false
                            }
                            onChange={() =>
                              handleCheckboxChange(item.ProductId)
                            }
                          />
                        </TableCell>
                        <TableCell
                          component="div"
                          sx={{
                            flex: 0.5,
                            display: "flex",
                            gap: 0.5,
                            alignItems: "center",
                            padding: "0px 7px !important",
                          }}
                        >
                          <Avatar
                            variant="rounded"
                            src={item?.FeatureImage}
                            alt={item?.ProductName}
                            sx={{
                              width: 35,
                              height: 35,
                              bgcolor: "#fff",
                              border: "2px solid #f8f8f8",
                              borderRadius: "6px",
                              padding: "2px",
                              "& img": {
                                objectFit: "cover  !important",
                              },
                            }}
                          />
                          {item.ProductName}
                        </TableCell>
                        <TableCell
                          component="div"
                          sx={{
                            flex: 0.5,
                            display: "flex",
                            alignItems: "center",
                            padding: "0px 7px !important",
                          }}
                        >
                          {item.SKU}
                        </TableCell>
                        <TableCell
                          component="div"
                          sx={{
                            flex: 0.5,
                            display: "flex",
                            alignItems: "center",
                            padding: "0px !important",
                            paddingLeft: "20px !important",
                          }}
                        >
                          {item.ProductCategoryName}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </GridItem>
          </>
        )}
      </GridContainer>
    </ModalComponent>
  );
};

export default AssignStoreToProductModal;
