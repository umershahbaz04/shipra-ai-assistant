import FileDownloadOutlinedIcon from "@mui/icons-material/FileDownloadOutlined";
import { Avatar, Box, Stack } from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import DataGridComponent from "../../../../.reUseableComponents/DataGrid/DataGridComponent";
import DataGridHeader from "../../../../.reUseableComponents/DataGridHeader/DataGridHeader";
import {
  CheckUniqueProductStockSku,
  CreateProduct,
  GetProductSampleFile,
} from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import ImportProductModal from "../../../../components/modals/productModals/ImportProductModal";
import UploadProductModal from "../../../../components/modals/productModals/UploadProductModal";
import {
  ActionButtonCustom,
  ActionButtonEdit,
  amountFormat,
  centerColumn,
  ClipboardIcon,
  CodeBox,
  DescriptionBox,
  fetchMethod,
  handleDispatSuccessNFaildedData,
  rightColumn,
  StyledTooltip,
  useGetWindowHeight,
} from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
import { getAllStationLookupFunc } from "../../../../apiCallingFunction";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CancelIcon from "@mui/icons-material/Cancel";
import { useDispatch } from "react-redux";
import { SUCCESS_N_FAILED_REDUX_STATE } from "../../../../redux/constants";

const UploadProduct = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const { height: windowHeight } = useGetWindowHeight();
  const [selectionModel, setSelectionModel] = useState([]);
  const [errorsList, setErrorsList] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const resetRowRef = useRef(false);
  const [openImportProductModal, setOpenImportProductModal] = useState(false);
  const [productData, setProductData] = useState([]);
  const [openUploadProductModal, setOpenUploadProductModal] = useState(false);
  const [rowData, setrowData] = useState({});
  const [rowDataErrorMsg, setRowDataErrorMsg] = useState("");
  const [productStations, setProductStations] = useState([]);
  const initialSubmitData = {
    failedProductCount: 0,
    rowLoading: {},
    addedProductCount: 0,
    addedProductRowNum: {},
    failedProductRowNum: {},
    failedProductRowNumReason: {},
  };
  const [submitData, setSubmitData] = useState(initialSubmitData);

  let getAllStationLookup = async () => {
    let data = await getAllStationLookupFunc();
    if (data.length > 0) {
      if (data?.find((x) => x.productStationId == 0)) {
        data.shift();
      }
      setProductStations(data);
    }
  };

  const handleSelectedRow = (oNos) => {
    if (oNos.length !== productData.length) {
      const hasErrorRowSelect = oNos?.some((ind) => productData[ind]?.hasError);
      if (hasErrorRowSelect) {
        errorNotification("Please correct data before select");
        return;
      }
      const hasUploadedRowSelect = oNos?.some(
        (ind) => submitData.addedProductRowNum[ind]
      );
      if (hasUploadedRowSelect) {
        errorNotification("Already uploaded");
        return;
      }
    }
    const filteredRows = oNos?.filter(
      (ind) =>
        !productData[ind]?.hasError && !submitData.addedProductRowNum[ind]
    );
    setSelectionModel(filteredRows);
  };

  const handleUploadProduct = async () => {
    let updatedProductData = [];
    if (!productData.productStocks || productData.productStocks.length === 0) {
      updatedProductData = productData.map((product) => ({
        ...product,
        productName: product.productName || product.title,
        productStocks: productStations.map((station) => ({
          LowQuantityLimit: 0,
          ProductStationId: station.productStationId,
          QuantityAvailable: station.sname.includes("(Default)")
            ? product.quantityAvailable
            : 0,
          Sku: product.sku,
          VarientOption: "",
          price: product.price,
        })),
      }));
    }
    if (productData.length > 0) {
      setSubmitData((prev) => ({ ...prev, loading: true }));
      handleDispatSuccessNFaildedData(dispatch, {
        show: true,
        loading: true,
        msg: "",
      });
      let failed = 0;
      let success = 0;
      debugger;
      for (let params of updatedProductData) {
        if (
          !selectionModel.includes(params.rowNum) ||
          submitData.addedProductRowNum[params.rowNum]
        ) {
          continue;
        }
        setSubmitData((prev) => ({
          ...prev,
          rowLoading: { [params?.rowNum]: true },
        }));
        const { response: checkUniqueSKU_Response } = await fetchMethod(() =>
          CheckUniqueProductStockSku(params?.sku)
        );
        if (checkUniqueSKU_Response?.result?.isExist) {
          ++failed;
          setSubmitData((prev) => ({
            ...prev,
            failedProductCount: prev.failedProductCount + 1,
            failedProductRowNum: {
              ...prev.failedProductRowNum,
              [params.rowNum]: true,
            },
            failedProductRowNumReason: {
              ...prev.failedProductRowNumReason,
              [params.rowNum]: "Dulplicate sku",
            },
            rowLoading: { [params?.rowNum]: false },
          }));
          handleDispatSuccessNFaildedData(dispatch, {
            show: true,
            loading: true,
            msg: `Total products: ${selectionModel.length} , uploaded: ${success} , failed: ${failed}`,
          });
          continue;
        }

        const { response: createProductResponse } = await fetchMethod(() =>
          CreateProduct(params)
        );
        if (createProductResponse?.isSuccess) {
          ++success;
          setSubmitData((prev) => ({
            ...prev,
            addedProductCount: prev.addedProductCount + 1,
            addedProductRowNum: {
              ...prev.addedProductRowNum,
              [params.rowNum]: true,
            },
            failedProductRowNum: {
              ...prev.failedProductRowNum,
              [params.rowNum]: false,
            },
          }));
        } else {
          ++failed;
          setSubmitData((prev) => ({
            ...prev,
            failedProductCount: prev.failedProductCount + 1,
            failedProductRowNum: {
              ...prev.failedProductRowNum,
              [params.rowNum]: true,
            },
            failedProductRowNumReason: {
              ...prev.failedProductRowNumReason,
              [params.rowNum]: "Invalid data",
            },
          }));
        }
        handleDispatSuccessNFaildedData(dispatch, {
          show: true,
          loading: true,
          msg: `Total products: ${selectionModel.length} , uploaded: ${success} , failed: ${failed}`,
        });
        setSubmitData((prev) => ({
          ...prev,
          rowLoading: { [params?.rowNum]: false },
        }));
      }
      setSelectionModel([]);
      handleDispatSuccessNFaildedData(dispatch, {
        show: true,
        loading: false,
        msg: `Total products: ${selectionModel.length} , uploaded: ${success} , failed: ${failed}`,
      });
      // hide navbar after 30 sec
      setTimeout(() => {
        handleDispatSuccessNFaildedData(dispatch, SUCCESS_N_FAILED_REDUX_STATE);
      }, 30000);
    } else {
      errorNotification("Please Select Row(s)");
    }
  };

  const handleEditProduct = (data) => {
    setrowData(data);
    setRowDataErrorMsg(
      submitData.failedProductRowNum[data.rowNum] &&
        submitData.failedProductRowNumReason[data.rowNum]
    );
    setOpenUploadProductModal(true);
  };

  const downloadSample = async () => {
    try {
      const res = await GetProductSampleFile();
      console.log(res);
      if (res.data.isSuccess && res.data.result?.url) {
        const link = document.createElement("a");
        link.href = res.data.result.url;
        link.setAttribute("download", "sample-product.xlsx");
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        successNotification("Successfully Downloaded");
      } else {
        errorNotification("Unable to download");
      }
    } catch (e) {
      errorNotification("Unable to download");
    }
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
      minWidth: 140,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box>
            <Stack
              direction="row"
              justifyContent="center"
              alignItems="center"
              spacing={1}
            >
              <Avatar
                src={params.row.featureImage}
                sx={{ width: 30, height: 30, fontSize: "13px" }}
              >
                {params.row?.ProductName}
              </Avatar>
              <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                <CodeBox
                  title={
                    !submitData.rowLoading[params.row.rowNum]
                      ? params.row.title
                      : "Uploading Product..."
                  }
                />
                {submitData.addedProductRowNum[params.row.rowNum] && (
                  <CheckCircleIcon fontSize="small" color="success" />
                )}
                {submitData.failedProductRowNum[params.row.rowNum] && (
                  <CancelIcon fontSize="small" color="error" />
                )}
                <DescriptionBox description={params.row.description} />
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
      renderCell: ({ row }) => {
        return (
          <>
            <CodeBox title={row?.sku} />
            {<ClipboardIcon text={row.sku} />}
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
      renderCell: (params) => {
        return <Box>{params.row.weight}</Box>;
      },
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
        return <Box>{amountFormat(row.purchasePrice)} </Box>;
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
        return <Box>{amountFormat(row.price)} </Box>;
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
      minWidth: 80,
      flex: 1,
      renderCell: (params) => {
        return (
          <Box sx={{ position: "relative" }}>
            {!submitData.addedProductRowNum[params.row.rowNum] && (
              <ActionButtonEdit
                onClick={(e) => {
                  handleEditProduct(params.row);
                }}
              />
            )}
            {params.row?.hasError && (
              <StyledTooltip
                title={params.row?.errorMsg}
                sx={{ position: "absolute", bottom: -6 }}
              />
            )}
            {!params.row?.hasError &&
              submitData.failedProductRowNum[params.row?.rowNum] && (
                <StyledTooltip
                  title={
                    submitData.failedProductRowNumReason[params.row?.rowNum]
                  }
                  sx={{ position: "absolute", bottom: -6 }}
                />
              )}
          </Box>
        );
      },
    },
  ];

  const calculatedHeight = windowHeight - 129;

  const handleResetFields = () => {
    setSubmitData(initialSubmitData);
    setSelectionModel([]);
  };

  useEffect(() => {
    getAllStationLookup();
  }, []);

  return (
    <Box sx={styleSheet.pageRoot}>
      <Box sx={{ padding: "10px" }}>
        <DataGridHeader tabs={false}>
          <Box className={"flex_center"} gap={1}>
            {selectionModel?.length > 0 && (
              <ActionButtonCustom
                onClick={handleUploadProduct}
                label={"Upload Product"}
                loading={isLoading}
                width="120px"
              />
            )}
            <ActionButtonCustom
              onClick={(event) => {
                setOpenImportProductModal(true);
              }}
              label={"Import Product"}
            />
            <ActionButtonCustom
              label={"Sample Download"}
              onClick={downloadSample}
              startIcon={<FileDownloadOutlinedIcon />}
            />
            <ActionButtonCustom
              onClick={() => {
                navigate("/products");
              }}
              variant="contained"
              label={LanguageReducer?.languageType?.PRODUCTS_PRODUCT_DASHBOARD}
            />
          </Box>
        </DataGridHeader>
        <DataGridComponent
          columns={columns}
          rows={productData || []}
          getRowId={(rows) => rows.rowNum}
          resetRowRef={resetRowRef}
          getRowHeight={() => "auto"}
          checkboxSelection={true}
          selectionModel={selectionModel}
          onSelectionModelChange={(oNo) => handleSelectedRow(oNo)}
          getRowClassName={({ row }) => (row?.hasError ? "active-row" : "")}
          height={calculatedHeight}
        />
      </Box>
      {openImportProductModal && (
        <ImportProductModal
          open={openImportProductModal}
          onClose={() => setOpenImportProductModal(false)}
          setProductData={setProductData}
          handleResetFields={handleResetFields}
          setErrorsList={setErrorsList}
        />
      )}
      {openUploadProductModal && (
        <UploadProductModal
          open={openUploadProductModal}
          onClose={() => setOpenUploadProductModal(false)}
          rowData={rowData}
          setProductData={setProductData}
          productStations={productStations}
          errorMsg={rowDataErrorMsg}
        />
      )}
    </Box>
  );
};

export default UploadProduct;
