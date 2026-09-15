import React, { useState, useEffect, useCallback } from "react";
import { Box } from "@mui/material";
import { Route, Routes } from "react-router-dom";
import { useSelector } from "react-redux";
import { styleSheet } from "../../../assets/styles/style";
import DataGridHeader from "../../../.reUseableComponents/DataGridHeader/DataGridHeader";
import { ActionButtonCustom } from "../../../utilities/helpers/Helpers";
import SyncPolicyList from "./list";
import CreateSyncPolicyDrawer from "./drawer/CreateSyncPolicyDrawer";
import { InventorySyncPolicy } from "./models/InventorySyncPolicy";
import {
  successNotification,
  errorNotification,
} from "../../../utilities/toast";
import {
  GetAllInventorySyncPolicies,
  DeleteInventorySyncPolicy,
  ToggleInventorySyncPolicyStatus,
  GetInventorySyncPolicyById,
} from "../../../api/AxiosInterceptors";

export default function SyncPoliciesPage() {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [policies, setPolicies] = useState([]);
  const [loading, setLoading] = useState(true);

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedPolicyForEdit, setSelectedPolicyForEdit] = useState(null);
  const [tabFilter, setTabFilter] = useState("all"); // "all", "active", "in-active"
  const [activeFilters, setActiveFilters] = useState({});

  const fetchPolicies = useCallback((filters = {}) => {
    setLoading(true);
    const params = {};
    if (filters.search && filters.search.trim()) {
      params.search = filters.search.trim();
    }
    if (filters.statusFilter === "active" || filters.tabFilter === "active") {
      params.active = true;
    } else if (filters.statusFilter === "paused" || filters.tabFilter === "in-active") {
      params.active = false;
    }
    if (filters.channelFilter && filters.channelFilter !== "all") {
      const num = Number(filters.channelFilter);
      if (!isNaN(num)) params.saleChannelConfigId = num;
    }

    GetAllInventorySyncPolicies(params)
      .then((res) => {
        if (res?.data?.isSuccess && Array.isArray(res?.data?.result)) {
          setPolicies(res.data.result.map((p) => new InventorySyncPolicy(p)));
        }
      })
      .catch((err) => {
        console.error("Error fetching sync policies:", err);
      })
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    fetchPolicies(activeFilters);
  }, [fetchPolicies, activeFilters]);

  const handleFilterChange = useCallback((newFilters) => {
    setActiveFilters(newFilters);
  }, []);

  // Open Drawer for creating new policy
  const handleOpenCreateDrawer = () => {
    setSelectedPolicyForEdit(null);
    setDrawerOpen(true);
  };

  // Open Drawer for editing existing policy
  const handleEditPolicy = (policy) => {
    if (!policy || !policy.InventorySyncPolicyId) return;
    GetInventorySyncPolicyById(policy.InventorySyncPolicyId)
      .then((res) => {
        if (res?.data?.isSuccess && res?.data?.result) {
          setSelectedPolicyForEdit(res.data.result);
        } else {
          setSelectedPolicyForEdit(policy);
        }
      })
      .catch((err) => {
        console.error("Error fetching policy details for edit", err);
        setSelectedPolicyForEdit(policy);
      })
      .finally(() => {
        setDrawerOpen(true);
      });
  };

  const handleCloseDrawer = () => {
    setDrawerOpen(false);
    setSelectedPolicyForEdit(null);
  };

  // Refresh policies list on save
  const handleSavePolicy = () => {
    fetchPolicies(activeFilters);
  };

  // Delete policy via API
  const handleDeletePolicy = (policyId) => {
    DeleteInventorySyncPolicy(policyId)
      .then((res) => {
        if (res?.data?.isSuccess) {
          successNotification("Sync policy deleted successfully");
          fetchPolicies(activeFilters);
        } else {
          errorNotification(res?.data?.message || "Failed to delete policy");
        }
      })
      .catch((err) => {
        console.error(err);
        errorNotification("Error deleting policy");
      });
  };

  // Toggle active/paused status via API
  const handleToggleStatus = (policyId) => {
    const target = policies.find((p) => p.InventorySyncPolicyId === policyId);
    if (!target) return;
    const newStatus = !target.Active;

    ToggleInventorySyncPolicyStatus(policyId, newStatus)
      .then((res) => {
        if (res?.data?.isSuccess) {
          successNotification(
            `Policy ${newStatus ? "resumed" : "paused"} successfully`
          );
          fetchPolicies(activeFilters);
        } else {
          errorNotification(res?.data?.message || "Failed to toggle status");
        }
      })
      .catch((err) => {
        console.error(err);
        errorNotification("Error toggling policy status");
      });
  };

  const counts = {
    all: policies.length,
    active: policies.filter((p) => p.Active).length,
    inActive: policies.filter((p) => !p.Active).length,
  };

  const tabsData = [
    {
      label: `All Policies (${counts.all})`,
      value: "all",
    },
    {
      label: `Active (${counts.active})`,
      value: "active",
    },
    {
      label: `Paused (${counts.inActive})`,
      value: "in-active",
    },
  ];

  const handleTabChange = (event, newValue) => {
    setTabFilter(newValue);
    setActiveFilters((prev) => ({ ...prev, tabFilter: newValue }));
  };

  return (
    <Box sx={styleSheet.mainContainer}>
      <DataGridHeader
        title={LanguageReducer?.languageType?.PRODUCT_SYNC_POLICIES_TITLE || "Sync Policies"}
        tabsData={tabsData}
        value={tabFilter}
        handleChange={handleTabChange}
      >
        <ActionButtonCustom
          onClick={handleOpenCreateDrawer}
          label="+ Create Sync Policy"
          width="165px"
        />
      </DataGridHeader>

      <Routes>
        <Route
          path="/"
          element={
            <SyncPolicyList
              policies={policies}
              loading={loading}
              tabFilter={tabFilter}
              onOpenCreateDrawer={handleOpenCreateDrawer}
              onEditPolicy={handleEditPolicy}
              onDeletePolicy={handleDeletePolicy}
              onToggleStatus={handleToggleStatus}
              onFilterChange={handleFilterChange}
            />
          }
        />
      </Routes>

      <CreateSyncPolicyDrawer
        open={drawerOpen}
        onClose={handleCloseDrawer}
        onSaveSuccess={handleSavePolicy}
        initialData={selectedPolicyForEdit}
      />
    </Box>
  );
}
