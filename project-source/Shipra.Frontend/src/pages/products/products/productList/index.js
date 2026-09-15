import {
  Avatar,
  Box,
  CircularProgress,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
  Menu,
  MenuItem,
  IconButton,
} from "@mui/material";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import { forwardRef, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import ButtonComponent from "../../../../.reUseableComponents/Buttons/ButtonComponent";
import DataGridProComponent from "../../../../.reUseableComponents/DataGrid/DataGridProComponent";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import {
  DeleteProductById,
  EnableProductById,
  GetAllStoreWithTokenByProductId,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import GenerateProductLinkModal from "../../../../components/modals/productModals/GenerateProductLinkModal";
import StatusBadge from "../../../../components/shared/statudBadge";
import {
  EnumChangeFilterModelApiUrls,
  viewTypesEnum,
} from "../../../../utilities/enum";
import Colors from "../../../../utilities/helpers/Colors";
import {
  ActionButtonCustom,
  ClipboardIcon,
  CodeBox,
  DescriptionBox,
  DescriptionBoxWithChild,
  DisableButton,
  EnableButton,
  GridContainer,
  GridItem,
  ReusableCardComponent,
  amountFormat,
  centerColumn,
  navbarHeight,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import { PaginationComponent } from "../../../../utilities/helpers/paginationSchema";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import UtilityClass from "../../../../utilities/UtilityClass";

function TasksList(props, ref) {
  const {
    getAllProducts,
    isFilterOpen,
    viewMode,
    productsCount,
    setIsGridLoading,
    isGridLoading,
    products,
  } = props;
  const navigate = useNavigate();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [infoModal, setInfoModal] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [openDelete, setOpenDelete] = useState(false);
  const [deleteItemObject, setDeleteItemObject] = useState({});
  const [loadingStates, setLoadingStates] = useState(false);
  const [loadingEnableStates, setLoadingEnableStates] = useState({
    open: false,
    loading: {},
    data: [],
  });
  const [openGenerateProductLinkModal, setOpenGenerateProductLinkModal] =
    useState({
      open: false,
      loading: {},
      data: [],
      productId: null,
    });
  const [menuAnchorEl, setMenuAnchorEl] = useState(null);
  const [selectedProductRow, setSelectedProductRow] = useState(null);

  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);

  const handleDeleteConfirmation = (data) => {
    setDeleteItemObject(data);
    setOpenDelete(true);
  };
  const handleDelete = async () => {
    setLoadingStates((prevStates) => ({
      ...prevStates,
      [deleteItemObject.ProductId]: true,
    }));
    try {
      let param = {
        ProductId: deleteItemObject.ProductId,
      };

      const response = await DeleteProductById(param);
      if (response.data?.isSuccess) {
        getAllProducts();
        successNotification("product deactivated successfully");
      } else {
        errorNotification("product deactivated unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      if (error?.response.data.errors) {
        UtilityClass.showErrorNotificationWithDictionary(
          error?.response?.data?.errors
        );
      }
    } finally {
      setLoadingStates((prevStates) => ({
        ...prevStates,
        [deleteItemObject.ProductId]: false,
      }));
      setOpenDelete(false);
      setDeleteItemObject({});
      setLoadingStates(false);
    }
  };
  const handleEnableProduct = async (data) => {
    try {
      let param = {
        ProductId: data.ProductId,
      };
      setLoadingEnableStates((prev) => ({
        ...prev,
        loading: { [data.ProductId]: true },
      }));
      const response = await EnableProductById(param);
      if (response.data?.isSuccess) {
        getAllProducts();
        successNotification("product activated successfully");
      } else {
        errorNotification("product activated unsuccessfull");
      }
    } catch (error) {
      console.error("Something went wrong", error.response);
      if (error?.response.data.errors) {
        UtilityClass.showErrorNotificationWithDictionary(
          error?.response?.data?.errors
        );
      }
    } finally {
      setLoadingEnableStates((prev) => ({
        ...prev,
        loading: { [data.ProductId]: false },
      }));
    }
  };

  const handleGenerateProductLinkToken = async (data) => {
    const productId = data?.ProductId;
    setOpenGenerateProductLinkModal((prev) => ({
      ...prev,
      loading: {
        ...prev.loading,
        [productId]: true,
      },
    }));
    try {
      const response = await GetAllStoreWithTokenByProductId(productId);
      if (response?.data?.isSuccess) {
        setOpenGenerateProductLinkModal((prev) => ({
          ...prev,
          open: true,
          data: response?.data?.result?.list,
          productId: productId,
        }));
      } else {
        UtilityClass.showErrorNotificationWithDictionary(
          response?.data?.errors
        );
      }
    } catch (e) {
      console.error(e);
    } finally {
      setOpenGenerateProductLinkModal((prev) => ({
        ...prev,
        loading: {
          ...prev.loading,
          [productId]: false,
        },
      }));
    }
  };

  const customRowStyle = {
    height: "10px",
  };
  const getRowClassName = (params) => {
    for (let i = 0; i < products?.length; i++) {
      if (params.row.Active != true) return "active-row";
    }
    return "";
  };
  const handleEditClick = (productData) => {
    setInfoModal((prev) => ({
      ...prev,
      loading: { [productData.ProductId]: true },
    }));

    const data = {
      products: productData,
    };

    navigate("/edit-products", { state: { data } });
    setInfoModal((prev) => ({
      ...prev,
      loading: { [productData.ProductId]: false },
    }));
  };

  const columns = [
    {
      field: "ProductName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_TITLE}
        </Box>
      ),
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        const fullTitle = params.row.ProductName || "";
        const maxLength = 20;

        const displayTitle =
          fullTitle.length > maxLength
            ? `${fullTitle.slice(0, maxLength)}...`
            : fullTitle;

        const codeBox = (
          <CodeBox
            title={displayTitle}
            onClick={() => handleEditClick(params.row)}
          />
        );
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              <Avatar
                variant="rounded"
                src={params.row.FeatureImage}
                sx={{ width: 30, height: 30, fontSize: "13px", padding: "1px" }}
              >
                {params.row.ProductName[0]}
              </Avatar>
              <Box sx={{ display: "flex", alignItems: "center" }}>
                {fullTitle.length > maxLength ? (
                  <Tooltip title={fullTitle} arrow placement="top">
                    <span>{codeBox}</span>
                  </Tooltip>
                ) : (
                  codeBox
                )}
                <DescriptionBox
                  description={
                    <span
                      dangerouslySetInnerHTML={{
                        __html: params.row.Description,
                      }}
                    />
                  }
                />
              </Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      field: "SKU",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_SKU}
        </Box>
      ),
      // ...centerColumn,
      minWidth: 150,
      flex: 1,
      renderCell: (params) => {
        return (
          <>
            {infoModal.loading[params.row.ProductId] ? (
              <CircularProgress size={20} />
            ) : (
              <>
                <CodeBox
                  onClick={(e) => {
                    handleEditClick(params.row);
                  }}
                  title={params.row.SKU}
                />
                {params.row.SKU && <ClipboardIcon text={params.row.SKU} />}
              </>
            )}
          </>
        );
      },
    },
    {
      field: "ProductCategoryName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_CATEGORY_NAME}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
    },
    {
      field: "StoreName",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_STORE_NAME}
        </Box>
      ),
      minWidth: 160,
      flex: 1,
      renderCell: (params) => {
        const storeNames = params.row.StoreName ? params.row.StoreName.split(', ') : [];
        const storeImages = params.row.StoreImages ? params.row.StoreImages.split(', ') : [];
        
        return (
          <Stack direction="row" spacing={0.5} sx={{ display: "flex", flexWrap: "wrap", alignItems: "center" }}>
            {storeNames.map((name, idx) => {
              const img = storeImages[idx] || "";
              const firstLetter = name ? name.trim()[0].toUpperCase() : "";
              return (
                <Tooltip key={idx} title={name} arrow placement="top">
                  <Avatar
                    variant="rounded"
                    src={img}
                    sx={{ width: 30, height: 30, fontSize: "13px", padding: "1px" }}
                  >
                    {firstLetter}
                  </Avatar>
                </Tooltip>
              );
            })}
          </Stack>
        );
      },
    },
    {
      field: "QuantityAvailable",
      ...centerColumn,
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_QTY_AVAILABLE}
        </Box>
      ),
      width: 160,
      renderCell: ({ row }) => {
        return (
          <>
            <Box sx={{ fontWeight: "bold" }} mr={1}>
              {row.QuantityAvailable}{" "}
            </Box>
            {`in ${row.VarientCount} varient(s)`}
            <DescriptionBoxWithChild>
              <TableContainer>
                <Table sx={{ minWidth: 275 }} aria-label="simple table">
                  <TableHead>
                    <TableRow>
                      <TableCell
                        sx={{
                          fontWeight: "bold",
                          fontSize: "11px",
                          padding: "5px",
                          width: "110px",
                        }}
                        align="left"
                      >
                        SKU
                      </TableCell>
                      <TableCell
                        sx={{
                          fontWeight: "bold",
                          fontSize: "11px",
                          padding: "5px",
                          width: "110px",
                        }}
                        align="left"
                      >
                        Station
                      </TableCell>

                      <TableCell
                        sx={{
                          fontWeight: "bold",
                          fontSize: "11px",
                          padding: "5px",
                        }}
                        align="right"
                      >
                        Price
                      </TableCell>
                      <TableCell
                        sx={{
                          fontWeight: "bold",
                          fontSize: "11px",
                          padding: "5px",
                        }}
                        align="center"
                      >
                        Qty
                      </TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {row?.ProductStocks?.map((row) => (
                      <TableRow
                        key={row.name}
                        sx={{
                          "&:last-child td, &:last-child th": { border: 0 },
                        }}
                        style={customRowStyle}
                      >
                        <TableCell
                          sx={{
                            padding: "7px",
                            fontSize: "11px",
                            width: "110px",
                          }}
                          align="left"
                        >
                          {/* <CodeBox title={row?.SKU} /> */}
                          <Box>
                            <Stack direction={"column"}>
                              <Box>{row?.SKU}</Box>
                              <Box>{row?.VarientOption}</Box>
                            </Stack>
                          </Box>
                        </TableCell>
                        <TableCell
                          sx={{
                            padding: "7px",
                            fontSize: "11px",
                            width: "90px",
                          }}
                          align="left"
                        >
                          {row?.StationName}
                        </TableCell>

                        <TableCell
                          sx={{
                            padding: "7px",
                            width: "50px",
                            fontSize: "11px",
                          }}
                          align="right"
                        >
                          {amountFormat(row?.Price)}
                        </TableCell>
                        <TableCell
                          sx={{
                            padding: "7px",
                            width: "50px",
                            fontSize: "11px",
                          }}
                          align="center"
                        >
                          <CodeBox title={row?.QuantityAvailable || "0"} />
                        </TableCell>
                      </TableRow>
                    )).slice(0, 5)}
                  </TableBody>
                </Table>
              </TableContainer>
              {row?.ProductStocks?.length > 5 && (
                <ButtonComponent
                  title="Show More"
                  sx={{ display: "flex", margin: "5px auto" }}
                  onClick={() => {
                    navigate("/inventory");
                  }}
                />
              )}
            </DescriptionBoxWithChild>
          </>
        );
      },
    },
    {
      field: "Weight",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {LanguageReducer?.languageType?.PRODUCTS_WEIGHT}
        </Box>
      ),
      minWidth: 60,
      flex: 1,
      ...centerColumn,
    },
    {
      field: "PurchasePrice",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_PURCHASE_PRICE}
        </Box>
      ),
      minWidth: 100,
      flex: 1,
      ...rightColumn,
      renderCell: ({ row }) => {
        return <Box>{amountFormat(row.PurchasePrice)} </Box>;
      },
    },
    {
      field: "Sale Price",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_SALE_PRICE}
        </Box>
      ),
      minWidth: 80,
      flex: 1,
      ...rightColumn,
      renderCell: ({ row }) => {
        return <Box>{amountFormat(row.Price)} </Box>;
      },
    },
    {
      ...centerColumn,
      field: "Status",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_STATUS}
        </Box>
      ),
      width: 80,

      renderCell: (params) => {
        let isActive = params.row.Active;
        let title = isActive ? "Active" : "InActive";
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              <Box>
                <StatusBadge
                  title={title}
                  color={isActive == false ? "#fff;" : "#fff;"}
                  bgColor={isActive === false ? Colors.danger : Colors.succes}
                />

                {/* <Typography>{params.row.ProductStockStatus || ""}</Typography> */}
              </Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      ...centerColumn,
      field: "Action",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCTS_ACTION}
        </Box>
      ),
      minWidth: 120,
      flex: 1,
      renderCell: ({ row }) => {
        return (
          <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
            {row?.Active ? (
              <DisableButton onClick={() => handleDeleteConfirmation(row)} />
            ) : (
              <EnableButton
                loading={loadingEnableStates.loading[row.ProductId]}
                onClick={() => handleEnableProduct(row)}
              />
            )}
            <IconButton
              onClick={(e) => {
                e.stopPropagation();
                setMenuAnchorEl(e.currentTarget);
                setSelectedProductRow(row);
              }}
            >
              <MoreVertIcon />
            </IconButton>
          </Box>
        );
      },
    },
  ];

  const calculatedHeight = isFilterOpen
    ? windowHeight - navbarHeight - 160 - 86
    : windowHeight - navbarHeight - 160;

  const calculatedCardHeight = isFilterOpen
    ? windowHeight - navbarHeight - 137 - 86
    : windowHeight - navbarHeight - 125;

  return (
    <>
      {viewMode === viewTypesEnum.GRID ? (
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "space-between",
            overflowX: "auto",
            bgcolor: "#F8F8F8",
            height: calculatedCardHeight,
            borderBottomLeftRadius: "8px",
            borderBottomRightRadius: "8px",
            padding: "8px",
          }}
        >
          {isGridLoading ? (
            <Box
              sx={{
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                height: "100%",
              }}
            >
              <CircularProgress />
            </Box>
          ) : (
            <GridContainer spacing={2}>
              {products.map((dt, index) => (
                <GridItem xs={12} sm={6} md={4} lg={3} key={index}>
                  <ReusableCardComponent
                    image={dt.FeatureImage}
                    title={dt.ProductName}
                    children={
                      <>
                        <Box sx={{ width: "100%" }}>
                          <Typography variant="body2">{`Price: ${dt.Price}`}</Typography>
                        </Box>
                      </>
                    }
                    description={
                      <>
                        <Typography
                          variant="body2"
                          display="flex"
                          alignItems="center"
                          justifyContent={"center"}
                        >
                          {dt.SKU}
                          <ClipboardIcon text={dt.SKU} />
                        </Typography>
                        <Box sx={{ display: "flex", gap: 0.5, justifyContent: "center", alignItems: "center", pb: 0.5 }}>
                          <Typography variant="body2">Store: </Typography>
                          {(dt.StoreName ? dt.StoreName.split(', ') : []).map((name, idx) => {
                            const storeImages = dt.StoreImages ? dt.StoreImages.split(', ') : [];
                            const img = storeImages[idx] || "";
                            const firstLetter = name ? name.trim()[0].toUpperCase() : "";
                            return (
                              <Tooltip key={idx} title={name} arrow placement="top">
                                <Avatar
                                  variant="rounded"
                                  src={img}
                                  sx={{ width: 20, height: 20, fontSize: "10px", padding: "1px" }}
                                >
                                  {firstLetter}
                                </Avatar>
                              </Tooltip>
                            );
                          })}
                        </Box>
                      </>
                    }
                    cardActionsChildren={
                      <>
                        {dt?.Active ? (
                          <ActionButtonCustom
                            sx={{
                              ...styleSheet.integrationactivatedButton,
                              width: "100%",
                              height: "30px",
                              borderRadius: "4px",
                              background: "#dc3545 !important",
                            }}
                            variant="contained"
                            loading={loadingEnableStates.loading[dt.ProductId]}
                            onClick={() => handleDeleteConfirmation(dt)}
                            label={"Deactivate"}
                          />
                        ) : (
                          <ActionButtonCustom
                            sx={{
                              ...styleSheet.integrationactivatedButton,
                              width: "100%",
                              height: "30px",
                              borderRadius: "4px",
                              background: Colors.succes,
                            }}
                            variant="contained"
                            loading={loadingEnableStates.loading[dt.ProductId]}
                            onClick={() => handleEnableProduct(dt)}
                            label={"Activate"}
                          />
                        )}

                        <ActionButtonCustom
                          sx={{
                            ...styleSheet.integrationactivatedButton,
                            width: "100%",
                            height: "30px",
                            borderRadius: "4px",
                          }}
                          variant="contained"
                          loading={
                            openGenerateProductLinkModal.loading?.[dt.ProductId]
                          }
                          onClick={() => handleGenerateProductLinkToken(dt)}
                          label={"Generate Link"}
                        />
                      </>
                    }
                    onCardActionClick={(e) => {
                      handleEditClick(dt);
                    }}
                    status={{
                      label: dt.Active
                        ? LanguageReducer?.languageType?.ACTIVE
                        : LanguageReducer?.languageType?.IN_ACTIVE,
                      color:
                        dt.Active === false ? Colors.danger : Colors.succes,
                    }}
                  />
                </GridItem>
              ))}
            </GridContainer>
          )}
          <PaginationComponent
            name={"Products"}
            dataCount={productsCount}
            paginationChangeMethod={getAllProducts}
            paginationMethodUrl={
              EnumChangeFilterModelApiUrls.GET_ALL_PRODUCTS.url
            }
            defaultRowsPerPage={
              EnumChangeFilterModelApiUrls.GET_ALL_PRODUCTS.length
            }
            setLoading={setIsGridLoading}
            color="primary"
          />
        </Box>
      ) : (
        <Box
          sx={{
            ...styleSheet.allOrderTable,
            height: calculatedHeight,
          }}
        >
          <DataGridProComponent
            rowPadding={3}
            loading={isGridLoading}
            sx={{
              fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
              fontSize: "12px",
              fontWeight: "500",
            }}
            getRowId={(row) => row.ProductId}
            rows={products}
            columns={columns}
            disableSelectionOnClick
            paginationChangeMethod={getAllProducts}
            paginationMethodUrl={
              EnumChangeFilterModelApiUrls.GET_ALL_PRODUCTS.url
            }
            defaultRowsPerPage={
              EnumChangeFilterModelApiUrls.GET_ALL_PRODUCTS.length
            }
            rowsCount={productsCount}
            getRowClassName={getRowClassName}
            height={calculatedHeight}
          />
        </Box>
      )}
      <DeleteConfirmationModal
        open={openDelete}
        setOpen={setOpenDelete}
        loading={false}
        handleDelete={handleDelete}
        heading={"Are you sure to disable this item?"}
        message={
          "After this action the item will be disabled immediately but you can undo this action anytime."
        }
        buttonText={"Disable"}
      />
      {openGenerateProductLinkModal.open && (
        <GenerateProductLinkModal
          open={openGenerateProductLinkModal.open}
          onClose={() =>
            setOpenGenerateProductLinkModal((prev) => ({
              ...prev,
              open: false,
            }))
          }
          GenerateProductLinkData={openGenerateProductLinkModal}
          setOpenGenerateProductLinkModal={setOpenGenerateProductLinkModal}
        />
      )}
      <Menu
        anchorEl={menuAnchorEl}
        open={Boolean(menuAnchorEl)}
        onClose={() => setMenuAnchorEl(null)}
      >
        <MenuItem
          onClick={() => {
            setMenuAnchorEl(null);
            handleGenerateProductLinkToken(selectedProductRow);
          }}
          disabled={openGenerateProductLinkModal.loading?.[selectedProductRow?.ProductId]}
        >
          Generate Link
        </MenuItem>
      </Menu>
    </>
  );
}

export default forwardRef(TasksList);
