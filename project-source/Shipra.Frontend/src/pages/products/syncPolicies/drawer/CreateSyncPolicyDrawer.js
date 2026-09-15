import React, { useState, useEffect } from "react";
import {
  Drawer,
  Box,
  Typography,
  IconButton,
  Button,
  Divider,
} from "@mui/material";
import LoadingButton from "@mui/lab/LoadingButton";
import CloseIcon from "@mui/icons-material/Close";
import CheckIcon from "@mui/icons-material/Check";
import Step1PolicyDetails from "./steps/Step1PolicyDetails";
import Step2InventorySource from "./steps/Step2InventorySource";
import Step3ChooseProducts from "./steps/Step3ChooseProducts";
import Step4SyncRules from "./steps/Step4SyncRules";
import Step5Review from "./steps/Step5Review";
import {
  InventorySyncPolicy,
  EnumStationSourceMode,
  defaultFormState,
} from "../models/InventorySyncPolicy";
import { successNotification, errorNotification } from "../../../../utilities/toast";
import {
  GetAllSaleChannelByLookupIdForSelection,
  GetSaleChannelProductStationsWithQtyForSelection,
  GetProductStations,
  GetProductsWithStationInventoryForSelection,
  SaveInventorySyncPolicy,
  GetInventorySyncModeLookups,
  GetInventorySyncDirectionLookups,
} from "../../../../api/AxiosInterceptors";

const STEP_LABELS = [
  "Policy",
  "Inventory Source",
  "Products",
  "Review",
];

export default function CreateSyncPolicyDrawer({
  open,
  onClose,
  onSavePolicy,
  initialData = null,
}) {
  const [currentStep, setCurrentStep] = useState(1);
  const [formData, setFormData] = useState(defaultFormState);
  const [salesChannels, setSalesChannels] = useState([]);
  const [isLoadingChannels, setIsLoadingChannels] = useState(false);
  const [isLoadingStations, setIsLoadingStations] = useState(false);
  const [isLoadingProducts, setIsLoadingProducts] = useState(false);
  const [syncModes, setSyncModes] = useState([]);
  const [syncDirections, setSyncDirections] = useState([]);

  const fetchProductsForInventorySource = (modeId, stations, currentInitialData = initialData) => {
    setIsLoadingProducts(true);
    const isAll = modeId === EnumStationSourceMode.AllStations;
    const selectedStationIds = isAll
      ? []
      : (stations || [])
          .filter((s) => s.Selected)
          .map((s) => s.ProductStationId);

    let savedProductGuids = [];
    let savedVariantIds = [];
    if (currentInitialData) {
      const rawProds =
        currentInitialData.Products || currentInitialData.products || [];
      const rawSelIds =
        currentInitialData.SelectedProductIds ||
        currentInitialData.selectedProductIds ||
        currentInitialData.ProductIds ||
        currentInitialData.productIds ||
        [];
      if (Array.isArray(rawProds) && rawProds.length > 0) {
        rawProds.forEach((p) => {
          if (p.ProductId || p.productId)
            savedProductGuids.push(String(p.ProductId || p.productId).toLowerCase());
          if (p.ProductVariantId || p.productVariantId)
            savedVariantIds.push(String(p.ProductVariantId || p.productVariantId));
        });
      } else if (Array.isArray(rawSelIds)) {
        rawSelIds.forEach((id) => {
          savedProductGuids.push(String(id).toLowerCase());
          savedVariantIds.push(String(id));
        });
      }
    }

    const modeSyncId =
      currentInitialData?.InventorySyncModeId ??
      currentInitialData?.inventorySyncModeId ??
      1;
    const isSelectedScope = modeSyncId === 2;

    GetProductsWithStationInventoryForSelection({
      isAllStations: isAll,
      productStationIds: selectedStationIds,
    })
      .then((res) => {
        const rawList =
          res?.data?.result ||
          res?.data?.data ||
          (Array.isArray(res?.data) ? res.data : []);

        if (Array.isArray(rawList)) {
          const mappedProducts = rawList.map((p) => {
            let isSelected = true;
            if (currentInitialData) {
              if (isSelectedScope) {
                if (savedProductGuids.length > 0 || savedVariantIds.length > 0) {
                  const matchGuid =
                    p.ProductId &&
                    savedProductGuids.includes(String(p.ProductId).toLowerCase());
                  const matchVar =
                    p.ProductVariantId &&
                    savedVariantIds.includes(String(p.ProductVariantId));
                  isSelected = Boolean(matchGuid || matchVar);
                } else {
                  isSelected = false;
                }
              } else {
                isSelected = true; // All products scope
              }
            }
            return {
              ProductId: p.ProductId,
              ProductVariantId: p.ProductVariantId,
              Title: p.Title || p.ProductName || "Product",
              VariantName: p.VariantName || "",
              SKU: p.SKU || "",
              Barcode: p.Barcode || "",
              Category: p.Category || "General",
              ProductCategoryId: p.ProductCategoryId,
              Store: p.Store || "Shipra Store",
              StoreId: p.StoreId,
              AvailableStock: p.AvailableStock ?? p.QuantityAvailable ?? p.TotalQty ?? 0,
              QuantityOnHand: p.QuantityOnHand ?? 0,
              StationBreakdown: p.StationBreakdown || "",
              StationNames: p.StationNames || "",
              PolicyTag: p.PolicyTag || "Default",
              Selected: isSelected,
            };
          });

          setFormData((prev) => ({
            ...prev,
            products: mappedProducts,
            selectedProductIds: mappedProducts
              .filter((p) => p && p.Selected)
              .map((p) => p.ProductId),
          }));
        }
      })
      .catch((err) =>
        console.error("Error fetching products with station inventory", err)
      )
      .finally(() => {
        setIsLoadingProducts(false);
      });
  };

  useEffect(() => {
    if (open) {
      const savedSources =
        initialData?.Sources ||
        initialData?.sources ||
        initialData?.ProductStationMappings ||
        initialData?.productStationMappings ||
        [];
      const savedStationIds = savedSources.map((s) =>
        String(s.ProductStationId || s.productStationId || s.id)
      );
      const hasSavedSources = savedSources.length > 0;

      if (initialData) {
        const firstProdRule =
          (initialData.Products && initialData.Products[0]) ||
          (initialData.products && initialData.products[0]) ||
          {};
        setFormData({
          ...defaultFormState,
          ...initialData,
          InventorySyncPolicyId:
            initialData.InventorySyncPolicyId ||
            initialData.inventorySyncPolicyId ||
            0,
          policyName: initialData.PolicyName || initialData.policyName || "",
          description: initialData.Description || initialData.description || "",
          saleChannelConfigId:
            initialData.SaleChannelConfigId ||
            initialData.saleChannelConfigId ||
            1,
          salesChannelName:
            initialData.SalesChannelName || initialData.salesChannelName || "",
          salesChannelCode:
            initialData.SalesChannelCode || initialData.salesChannelCode || "a",
          inventorySyncModeId:
            initialData.InventorySyncModeId ||
            initialData.inventorySyncModeId ||
            1,
          inventorySyncDirectionId:
            initialData.InventorySyncDirectionId ||
            initialData.inventorySyncDirectionId ||
            2,
          stationSourceModeId: hasSavedSources
            ? EnumStationSourceMode.SelectedStations
            : initialData.StationSourceModeId ||
              initialData.stationSourceModeId ||
              EnumStationSourceMode.AllStations,
          bufferQuantity:
            firstProdRule.BufferQuantity ??
            firstProdRule.bufferQuantity ??
            initialData.BufferQuantity ??
            initialData.bufferQuantity ??
            5,
          minPublishQuantity:
            firstProdRule.MinPublishQuantity ??
            firstProdRule.minPublishQuantity ??
            initialData.MinPublishQuantity ??
            initialData.minPublishQuantity ??
            0,
          maxPublishQuantity:
            firstProdRule.MaxPublishQuantity ??
            firstProdRule.maxPublishQuantity ??
            initialData.MaxPublishQuantity ??
            initialData.maxPublishQuantity ??
            null,
          allocationPercentage:
            firstProdRule.AllocationPercentage ??
            firstProdRule.allocationPercentage ??
            initialData.AllocationPercentage ??
            initialData.allocationPercentage ??
            5,
          syncInventory:
            firstProdRule.SyncInventory ??
            firstProdRule.syncInventory ??
            initialData.SyncInventory ??
            initialData.syncInventory ??
            true,
          setUnavailableToZero:
            firstProdRule.SetUnavailableToZero ??
            firstProdRule.setUnavailableToZero ??
            initialData.SetUnavailableToZero ??
            initialData.setUnavailableToZero ??
            true,
          syncPrice:
            firstProdRule.SyncPrice ??
            firstProdRule.syncPrice ??
            initialData.SyncPrice ??
            initialData.syncPrice ??
            false,
          syncProductInfo:
            firstProdRule.SyncProductInfo ??
            firstProdRule.syncProductInfo ??
            initialData.SyncProductInfo ??
            initialData.syncProductInfo ??
            false,
          syncMedia:
            firstProdRule.SyncMedia ??
            firstProdRule.syncMedia ??
            initialData.SyncMedia ??
            initialData.syncMedia ??
            false,
        });
      } else {
        setFormData(JSON.parse(JSON.stringify(defaultFormState)));
      }
      setCurrentStep(1);

      // Fetch Lookups
      GetInventorySyncModeLookups()
        .then((res) => {
          const fallbackModes = [
            { inventorySyncModeId: 3, name: "Percentage" },
            { inventorySyncModeId: 4, name: "FixedOrCapped" },
            { inventorySyncModeId: 5, name: "Dedicated" },
          ];
          if (res?.data?.isSuccess && Array.isArray(res?.data?.result)) {
            const allowedNames = ["percentage", "fixedorcapped", "dedicated"];
            const filtered = res.data.result.filter((m) => {
              const id = m.inventorySyncModeId ?? m.InventorySyncModeId;
              const name = (m.name ?? m.Name ?? "").toLowerCase();
              return id === 3 || id === 4 || id === 5 || allowedNames.includes(name);
            });
            setSyncModes(filtered.length > 0 ? filtered : fallbackModes);
          } else {
            setSyncModes(fallbackModes);
          }
        })
        .catch((e) => {
          console.error(e);
          setSyncModes([
            { inventorySyncModeId: 3, name: "Percentage" },
            { inventorySyncModeId: 4, name: "FixedOrCapped" },
            { inventorySyncModeId: 5, name: "Dedicated" },
          ]);
        });

      GetInventorySyncDirectionLookups()
        .then((res) => {
          if (res?.data?.isSuccess && Array.isArray(res?.data?.result)) {
            setSyncDirections(res.data.result);
          }
        })
        .catch((e) => console.error(e));

      // Fetch Live Sales Channels (strictly excluding Sale Person)
      setIsLoadingChannels(true);
      GetAllSaleChannelByLookupIdForSelection(0)
        .then((res) => {
          if (res?.data?.isSuccess && Array.isArray(res?.data?.result)) {
            const channels = res.data.result.filter((c) => {
              if (!c || c.id === -1 || c.id === 0) return false;
              const label = (c.text || c.SaleChannelName || "").toLowerCase();
              return (
                !label.includes("sale person") &&
                !label.includes("saleperson") &&
                c.SaleChannelLookupId !== 4 &&
                c.SaleChannelLookupId !== 100
              );
            });
            setSalesChannels(channels);
            if (!initialData && channels.length > 0) {
              setFormData((prev) => ({
                ...prev,
                saleChannelConfigId: channels[0].id,
                salesChannelName:
                  channels[0].text || channels[0].SaleChannelName,
              }));
            }
          }
        })
        .catch((err) => console.error("Error fetching sales channels", err))
        .finally(() => setIsLoadingChannels(false));

      // Fetch Live Sale Channel Product Stations with Station Type and Available Units
      setIsLoadingStations(true);
      GetSaleChannelProductStationsWithQtyForSelection()
        .then((res) => {
          if (res?.data?.isSuccess && Array.isArray(res?.data?.result)) {
            const mappedStations = res.data.result.map((st, index) => ({
              ProductStationId: st.ProductStationId || st.id,
              StationCode: st.StationCode || `PS${st.ProductStationId}`,
              StationName: st.StationName || st.Name,
              StationTypeName: st.StationTypeName || "Shipra Warehouse",
              AvailableUnits: st.AvailableUnits ?? st.QuantityAvailable ?? 0,
              QuantityOnHand: st.QuantityOnHand ?? 0,
              QuantityCommitted: st.QuantityCommitted ?? 0,
              QuantityIncoming: st.QuantityIncoming ?? 0,
              TotalSkuCount: st.TotalSkuCount ?? 0,
              Priority: 1,
              Selected: hasSavedSources
                ? savedStationIds.includes(String(st.ProductStationId || st.id))
                : true,
              IsDefault: st.IsDefault || false,
            }));

            setFormData((prev) => ({
              ...prev,
              stations: mappedStations,
            }));

            fetchProductsForInventorySource(
              hasSavedSources
                ? EnumStationSourceMode.SelectedStations
                : initialData?.StationSourceModeId ||
                    initialData?.stationSourceModeId ||
                    EnumStationSourceMode.AllStations,
              mappedStations,
              initialData
            );
          }
        })
        .catch((err) => {
          console.warn(
            "GetSaleChannelProductStationsWithQtyForSelection fallback to GetProductStations",
            err
          );
          GetProductStations({ start: 0, length: 100 })
            .then((res) => {
              const list = res?.data?.result?.data || res?.data?.result || [];
              if (Array.isArray(list)) {
                const mappedStations = list.map((st, index) => ({
                  ProductStationId: st.ProductStationId || st.id,
                  StationCode: st.StationCode || `PS${st.ProductStationId}`,
                  StationName: st.StationName || st.Name,
                  StationTypeName:
                    st.ProductStationTypeName || "Shipra Warehouse",
                  AvailableUnits: st.AvailableUnits ?? 0,
                  QuantityOnHand: st.QuantityOnHand ?? 0,
                  Priority: 1,
                  Selected: hasSavedSources
                    ? savedStationIds.includes(String(st.ProductStationId || st.id))
                    : true,
                  IsDefault: st.IsDefault || false,
                }));

                setFormData((prev) => ({
                  ...prev,
                  stations: mappedStations,
                }));

                fetchProductsForInventorySource(
                  hasSavedSources
                    ? EnumStationSourceMode.SelectedStations
                    : initialData?.StationSourceModeId ||
                        initialData?.stationSourceModeId ||
                        EnumStationSourceMode.AllStations,
                  mappedStations,
                  initialData
                );
              }
            })
            .catch((e) => console.error("Fallback stations fetch error", e));
        })
        .finally(() => setIsLoadingStations(false));
    }
  }, [open, initialData]);

  const handleNext = () => {
    if (currentStep === 1) {
      if (!formData.policyName || !formData.policyName.trim()) {
        errorNotification("Please enter a policy name");
        return;
      }
    }

    if (currentStep === 2) {
      // Only fetch if products are not loaded yet
      if (!formData.products || formData.products.length === 0) {
        fetchProductsForInventorySource(
          formData.stationSourceModeId,
          formData.stations
        );
      }
    }

    if (currentStep < 4) {
      setCurrentStep((prev) => prev + 1);
    } else {
      handleActivate();
    }
  };

  const handleBack = () => {
    if (currentStep > 1) {
      setCurrentStep((prev) => prev - 1);
    }
  };

  const [isSaving, setIsSaving] = useState(false);

  const handleActivate = () => {
    try {
      setIsSaving(true);
      const selectedStations = (formData.stations || []).filter((s) => s.Selected);
      const selectedProducts = (formData.products || []).filter((p) => p.Selected);

      const dto = {
        InventorySyncPolicyId: formData.InventorySyncPolicyId || formData.inventorySyncPolicyId || 0,
        PolicyName: formData.policyName || "New Sync Policy",
        Description: formData.description || "",
        StoreId: formData.storeId || null,
        SaleChannelConfigId: formData.saleChannelConfigId || null,
        InventorySyncModeId: formData.inventorySyncModeId || formData.stationSourceModeId || 1,
        InventorySyncDirectionId: formData.inventorySyncDirectionId || 2,
        BufferQuantity: Number(formData.bufferQuantity || 0),
        MaxPublishQuantity:
          formData.maxPublishQuantity === "" || formData.maxPublishQuantity === null
            ? null
            : Number(formData.maxPublishQuantity),
        MinPublishQuantity: Number(formData.minPublishQuantity || 0),
        IncludeIncoming: Boolean(formData.includeIncoming),
        IncludeCommitted: Boolean(formData.includeCommitted),
        ProductStationIds: selectedStations
          .map((s) => s.ProductStationId || s.productStationId || s.id)
          .filter(Boolean),
        ProductVariantIds: selectedProducts
          .map((p) => p.ProductVariantId || p.productVariantId)
          .filter(Boolean),
        ProductIds: selectedProducts
          .map((p) => p.ProductId || p.productId)
          .filter(Boolean),
      };

      SaveInventorySyncPolicy(dto)
        .then((res) => {
          if (res?.data?.isSuccess) {
            const savedId = res?.data?.result || dto.InventorySyncPolicyId;

            const newPolicyObject = new InventorySyncPolicy({
              ...formData,
              InventorySyncPolicyId: savedId,
              PolicyName: dto.PolicyName,
              Description: dto.Description,
              SalesChannelName: formData.salesChannelName || "Amazon UAE",
              SalesChannelCode: formData.salesChannelCode || "a",
              BufferQuantity: dto.BufferQuantity,
              MaxPublishQuantity: dto.MaxPublishQuantity,
              MinPublishQuantity: dto.MinPublishQuantity,
              Active: true,
              LastSync: "Just now",
              CreatedOn: new Date().toISOString(),
              ProductStationMappings: selectedStations,
              SelectedProductIds: selectedProducts.map((p) => p.ProductId),
              ProductsScopeSummary:
                formData.inventorySyncModeId === 1
                  ? "All products"
                  : `${selectedProducts.length} selected products`,
              InventoryRuleSummary: `Buffer ${dto.BufferQuantity} · ${
                dto.MaxPublishQuantity ? "Max " + dto.MaxPublishQuantity : "No max"
              }`,
            });

            if (onSavePolicy) {
              onSavePolicy(newPolicyObject);
            }

            successNotification(
              formData.InventorySyncPolicyId
                ? "Sync Policy updated successfully!"
                : "Sync Policy created and activated successfully!"
            );
            onClose();
          } else {
            errorNotification(res?.data?.message || "Failed to save sync policy");
          }
        })
        .catch((err) => {
          console.error(err);
          errorNotification("Error connecting to server. Please try again.");
        })
        .finally(() => {
          setIsSaving(false);
        });
    } catch (err) {
      console.error(err);
      errorNotification("Failed to prepare sync policy data");
      setIsSaving(false);
    }
  };

  const renderStepContent = () => {
    switch (currentStep) {
      case 1:
        return (
          <Step1PolicyDetails
            formData={formData}
            setFormData={setFormData}
            salesChannels={salesChannels}
            isLoadingChannels={isLoadingChannels}
          />
        );
      case 2:
        return (
          <Step2InventorySource
            formData={formData}
            setFormData={setFormData}
            isLoadingStations={isLoadingStations}
            syncModes={syncModes}
          />
        );
      case 3:
        return (
          <Step3ChooseProducts
            formData={formData}
            setFormData={setFormData}
            isLoadingProducts={isLoadingProducts}
            onRefreshProducts={() =>
              fetchProductsForInventorySource(
                formData.stationSourceModeId,
                formData.stations
              )
            }
          />
        );
      case 4:
        return <Step5Review formData={formData} />;
      default:
        return null;
    }
  };

  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={onClose}
      sx={{
        zIndex: 1300,
        "& .MuiDrawer-paper": {
          width: { xs: "100%", sm: "760px", md: "860px" },
          boxSizing: "border-box",
          boxShadow: "-10px 0 30px rgba(28,29,36,.15)",
          display: "flex",
          flexDirection: "column",
          overflow: "hidden",
        },
      }}
    >
      {/* Header */}
      <Box
        sx={{
          height: "64px",
          minHeight: "64px",
          borderBottom: "1px solid #dedee7",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          px: 3,
        }}
      >
        <Typography
          variant="h6"
          sx={{
            fontSize: "17px",
            fontWeight: 700,
            color: "#22242a",
          }}
        >
          {formData.InventorySyncPolicyId
            ? "Edit Sync Policy"
            : "Create Sync Policy"}
        </Typography>
        <IconButton
          onClick={onClose}
          size="small"
          sx={{
            width: "30px",
            height: "30px",
            backgroundColor: "#f5f5f8",
            color: "#62646d",
            "&:hover": { backgroundColor: "#eae9ef" },
          }}
        >
          <CloseIcon sx={{ fontSize: 18 }} />
        </IconButton>
      </Box>

      {/* Stepper Bar */}
      <Box
        sx={{
          height: "74px",
          minHeight: "74px",
          borderBottom: "1px solid #ececf2",
          backgroundColor: "#fcfcfd",
          display: "flex",
          alignItems: "center",
          px: 3,
          overflowX: "auto",
        }}
      >
        {STEP_LABELS.map((label, index) => {
          const stepNum = index + 1;
          const isDone = stepNum < currentStep;
          const isActive = stepNum === currentStep;

          return (
            <React.Fragment key={label}>
              <Box
                onClick={() => {
                  if (stepNum < currentStep) {
                    setCurrentStep(stepNum);
                  }
                }}
                sx={{
                  display: "flex",
                  alignItems: "center",
                  cursor: isDone ? "pointer" : "default",
                  color: isActive
                    ? "var(--primary-color)"
                    : isDone
                    ? "#4a4c53"
                    : "#999ba4",
                  fontSize: "12px",
                  fontWeight: 600,
                  whiteSpace: "nowrap",
                  userSelect: "none",
                }}
              >
                {/* Step Circle */}
                <Box
                  sx={{
                    width: "26px",
                    height: "26px",
                    borderRadius: "50%",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    mr: 1,
                    fontSize: "12px",
                    fontWeight: 700,
                    transition: "all 0.2s ease",
                    backgroundColor: isActive
                      ? "var(--primary-color)"
                      : isDone
                      ? "#fdf2f3"
                      : "#fff",
                    color: isActive ? "#fff" : isDone ? "var(--primary-color)" : "#999ba4",
                    border: "1px solid",
                    borderColor: isActive
                      ? "var(--primary-color)"
                      : isDone
                      ? "#f8d7da"
                      : "#cfcfd8",
                  }}
                >
                  {isDone ? <CheckIcon sx={{ fontSize: 14 }} /> : stepNum}
                </Box>
                {label}
              </Box>

              {/* Connector line */}
              {index < STEP_LABELS.length - 1 && (
                <Box
                  sx={{
                    height: "1px",
                    backgroundColor: "#dddde5",
                    width: "32px",
                    mx: 1.5,
                    flexShrink: 0,
                  }}
                />
              )}
            </React.Fragment>
          );
        })}
      </Box>

      {/* Body */}
      <Box
        sx={{
          flex: 1,
          overflowY: "auto",
          p: { xs: 2.5, sm: 3 },
          backgroundColor: "#fff",
        }}
      >
        {renderStepContent()}
      </Box>

      {/* Footer */}
      <Box
        sx={{
          height: "64px",
          minHeight: "64px",
          flexShrink: 0,
          borderTop: "1px solid #dedee7",
          display: "flex",
          alignItems: "center",
          justifyContent: "flex-end",
          gap: 1.25,
          px: 3,
          backgroundColor: "#fff",
          zIndex: 10,
        }}
      >
        {currentStep > 1 && (
          <Button
            variant="outlined"
            onClick={handleBack}
            sx={{
              height: "34px",
              borderRadius: "4px",
              border: "1px solid #cfcfd8",
              backgroundColor: "#fff",
              color: "#333",
              px: 2.25,
              fontWeight: 600,
              fontSize: "12px",
              textTransform: "none",
              "&:hover": {
                backgroundColor: "#f5f5f8",
                borderColor: "#b8b8c5",
              },
            }}
          >
            Back
          </Button>
        )}

        <LoadingButton
          variant="contained"
          onClick={handleNext}
          loading={isSaving}
          sx={{
            height: "34px",
            borderRadius: "4px",
            backgroundColor: "var(--primary-color)",
            color: "#fff",
            px: 2.5,
            fontWeight: 600,
            fontSize: "12px",
            textTransform: "none",
            boxShadow: "none",
            "&:hover": {
              backgroundColor: "var(--primary-color)",
              opacity: 0.9,
              boxShadow: "none",
            },
            "& .MuiLoadingButton-loadingIndicator": {
              color: "#fff",
            },
          }}
        >
          {currentStep === 4 ? "Activate Policy" : "Continue"}
        </LoadingButton>
      </Box>
    </Drawer>
  );
}
