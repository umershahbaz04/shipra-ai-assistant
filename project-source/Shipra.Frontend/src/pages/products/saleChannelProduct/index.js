import { Avatar, Box, MenuItem, MenuList, Popover, Stack } from "@mui/material";
import React, { useEffect, useRef, useState } from "react";
import { useSelector } from "react-redux";
import DataGridComponent from "../../../.reUseableComponents/DataGrid/DataGridComponent.js";
import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader.js";
import {
  GetStoresForSelection,
  SaleChannelProductPostProcessor,
} from "../../../api/AxiosInterceptors.js";
import { styleSheet } from "../../../assets/styles/style.js";
import SaleChannelProductSyncModel from "../../../components/modals/productModals/SaleChannelProductSyncModel.js";
import {
  ActionButton,
  ActionButtonCustom,
  DataGridHeaderBox,
  amountFormat,
  centerColumn,
  navbarHeight,
  pagePadding,
  rightColumn,
  useClientSubscriptionReducer,
  useGetWindowHeight,
} from "../../../utilities/helpers/Helpers.js";
import {
  errorNotification,
  successNotification,
  warningNotification,
} from "../../../utilities/toast/index.js";
import UtilityClass from "../../../utilities/UtilityClass.js";

export default function SaleChannelProductPage() {
  const initialFilterFields = {
    loading: false,
    show: false,
    startDate: null,
    endDate: null,
  };
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height: windowHeight } = useGetWindowHeight(clientSubscriptionData);
  const [allProducts, setAllProducts] = useState([]);
  const [filterModal, setFilterModal] = useState(initialFilterFields);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [stores, setStores] = React.useState([]);
  const resetRowRef = useRef(false);
  const getOrdersRef = useRef([]);
  const handleClose = () => {
    setAnchorEl(null);
  };
  const open = Boolean(anchorEl);
  const id = open ? "simple-popover" : undefined;

  let getAllStores = async () => {
    let res = await GetStoresForSelection();
    if (res.data.result !== null) {
      setStores(res.data.result);
    }
  };

  // columns
  const columns = [
    {
      field: "title",
      headerName: (
        <Box sx={{ fontWeight: "600" }}>
          {" "}
          {LanguageReducer?.languageType?.PRODUCT_SALE_CHANNEL_PRODUCT_TITLE}
        </Box>
      ),
      minWidth: 120,
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
                src={params.row.imageSrc}
                sx={{ width: 30, height: 30, fontSize: "13px" }}
              >
                {params.row?.imageSrc}
              </Avatar>
              <Box>{params.row?.productName || ""}</Box>
            </Stack>
          </Box>
        );
      },
    },
    {
      field: "vendor",
      minWidth: 90,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.PRODUCT_SALE_CHANNEL_PRODUCT_VENDOR
          }
        />
      ),
      flex: 1,
    },
    {
      field: "productType",
      minWidth: 90,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType
              ?.PRODUCT_SALE_CHANNEL_PRODUCT_PRODUCT_TYPE
          }
        />
      ),
      flex: 1,
    },
    {
      field: "description",
      minWidth: 90,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType
              ?.PRODUCT_SALE_CHANNEL_PRODUCT_DESCRIPTION
          }
        />
      ),
      flex: 1,
    },
    {
      ...centerColumn,
      field: "variants",
      minWidth: 90,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType?.PRODUCT_SALE_CHANNEL_PRODUCT_VARIANTS
          }
        />
      ),
      flex: 1,
      renderCell: (params) => {
        return params.row?.variantCount;
      },
    },
    {
      ...centerColumn,
      field: "inventoryQuantity",
      minWidth: 90,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType
              ?.PRODUCT_SALE_CHANNEL_PRODUCT_INVENTORY_QTY
          }
        />
      ),
      flex: 1,
    },
    {
      ...rightColumn,
      field: "productPrice",
      minWidth: 90,
      headerName: (
        <DataGridHeaderBox
          title={
            LanguageReducer?.languageType
              ?.PRODUCT_SALE_CHANNEL_PRODUCT_PRODUCT_PRICE
          }
        />
      ),
      flex: 1,
      renderCell: ({ row }) => {
        return <Box>{amountFormat(row.productPrice)}</Box>;
      },
    },
  ];
  useEffect(() => {
    getAllStores();
  }, []);

  const handleCreateReturnReport = () => {
    let selectedTrNos = getOrdersRef.current;
    if (selectedTrNos.length > 0) {
      console.log(selectedTrNos);
      let dataForIds = allProducts.find((x) => x.productId == selectedTrNos[0]);
      let skus = allProducts
        .filter((x) => selectedTrNos.includes(x.productId))
        .map((x) => x.sku || x.productId)
        .join(",");

      let param = {
        productIds: selectedTrNos.toString(),
        saleChannelConfigId: !dataForIds?.saleChannelConfigId
          ? null
          : dataForIds?.saleChannelConfigId,
        saleChannelLookupId: dataForIds?.saleChannelLookupId,
        storeId: dataForIds?.storeId,
        skus: skus,
        fileReference: dataForIds?.fileReference,
      };

      SaleChannelProductPostProcessor(param)
        .then((res) => {
          if (!res.data.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
          } else {
            successNotification("Product created successfully");
            ////////////
            //remove created rows
            let filterdOrder = allProducts?.filter(
              (x) => !selectedTrNos.includes(x.productId)
            );
            setAllProducts(filterdOrder);
            ///////////

            resetRowRef.current = true;
          }
        })
        .catch((e) => {
          errorNotification(
            LanguageReducer?.languageType
              ?.SOMETHING_WENT_WRONG_PLEASE_TRY_AGAIN_TOAST
          );
          console.log("e", e);
        });
    } else {
      warningNotification(
        LanguageReducer?.languageType?.MUST_SELECT_SINGLE_ROW_WARNING_TOAST
      );
    }

    setAnchorEl(null);
  };

  const [openFetch, setOpenFetch] = useState();
  const calculatedHeight = windowHeight - navbarHeight - 66;

  return (
    <Box sx={styleSheet.pageRoot}>
      <Box sx={{ padding: pagePadding }}>
        <DataGridHeader tabs={false}>
          <Box className={"flex_center"} gap={1}>
            {allProducts.length > 0 && (
              <ActionButton
                onClick={(event) => {
                  setAnchorEl(event.currentTarget);
                }}
              />
            )}
            <ActionButtonCustom
              onClick={(event) => {
                setOpenFetch(true);
              }}
              label={
                LanguageReducer?.languageType
                  ?.PRODUCT_SALE_CHANNEL_PRODUCT_SYNC_PRODUCT
              }
              width="120px"
            />
          </Box>
        </DataGridHeader>
        <DataGridComponent
          columns={columns}
          rows={allProducts}
          loading={filterModal.loading}
          getRowId={(row) => row?.productId}
          getOrdersRef={getOrdersRef}
          resetRowRef={resetRowRef}
          getRowHeight={() => "auto"}
          checkboxSelection={true}
          rowPerPage={10}
          height={calculatedHeight}
        />
      </Box>
      <Popover
        id={id}
        open={open}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "left",
        }}
      >
        <MenuList
          sx={{
            minWidth: "162px !important",
          }}
          id={id}
          autoFocusItem
        >
          <MenuItem onClick={(event) => handleCreateReturnReport()}>
            Create Product
          </MenuItem>
        </MenuList>
      </Popover>

      {openFetch && (
        <SaleChannelProductSyncModel
          resetRowRef={resetRowRef}
          setAllProducts={setAllProducts}
          allStores={stores}
          getAllStores={getAllStores}
          orderNosData={getOrdersRef.current}
          open={openFetch}
          setOpen={setOpenFetch}
        />
      )}
    </Box>
  );
}
