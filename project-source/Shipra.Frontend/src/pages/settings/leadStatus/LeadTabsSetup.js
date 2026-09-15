import { Box, CircularProgress } from "@mui/material";
import React, { useEffect, useState, forwardRef, useImperativeHandle } from "react";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import {
  GetAllClientLeadStatusForSelection,
  GetLeadTabsCountConfig,
  UpdateLeadTab,
} from "../../../api/AxiosInterceptors";
import AddLeadTabModal from "../../../components/modals/leadsModals/AddLeadTabModal";
import UtilityClass from "../../../utilities/UtilityClass";
import { PageMainBox } from "../../../utilities/helpers/Helpers";
import { errorNotification, successNotification, warningNotification } from "../../../utilities/toast";
import { ProfileDetailsBox } from "../../Profile/Profile/Profile";
import LeadTabList from "./leadStatusList";

const LeadTabsSetup = forwardRef(({ setIsTabUpdating }, ref) => {
  const [leadTabCount, setLeadTabCount] = useState([]);
  const [allLeadStatuses, setAllLeadStatuses] = useState([]);
  const [openAddTabModal, setOpenAddTabModal] = useState(false);
  const [statusMoved, setStatusMoved] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const [inputValues, setInputValues] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [pageLoading, setPageLoading] = useState(false);

  useEffect(() => {
    if (setIsTabUpdating) {
      setIsTabUpdating(isLoading);
    }
  }, [isLoading, setIsTabUpdating]);

  useImperativeHandle(ref, () => ({
    triggerAddTab() {
      setOpenAddTabModal(true);
    },
    triggerUpdateTab() {
      handleUpdateLeadTab();
    }
  }));

  // ─── Fetch tab config (mirrors getShipmentTabsCountConfig) ──────────────
  const getLeadTabsCountConfig = async () => {
    setPageLoading(true);
    try {
      const res = await GetLeadTabsCountConfig();
      if (res?.data?.result) {
        // Normalize PascalCase/camelCase API response fields
        const normalized = res.data.result.map((x) => ({
          leadGridColumnId: x.leadGridColumnId ?? x.LeadGridColumnId,
          dashboardStatusName: x.dashboardStatusName ?? x.DashboardStatusName,
          dashboardStatusNameForKey: x.dashboardStatusNameForKey ?? x.DashboardStatusNameForKey,
          dashboardStatusValue: x.dashboardStatusValue ?? x.DashboardStatusValue,
          isDefaultStatusTab: x.isDefaultStatusTab ?? x.IsDefaultStatusTab ?? false,
          displayOrder: x.displayOrder ?? x.DisplayOrder,
        }));
        // Exclude the default "All" tab from the settings list using the boolean flag
        const filtered = normalized.filter((x) => !x.isDefaultStatusTab);
        setLeadTabCount(filtered);
      }
    } catch (e) {
      console.error("Error fetching lead tabs:", e);
    } finally {
      setPageLoading(false);
    }
  };

  // ─── Fetch all client statuses for the multi-select dropdown ───────────
  const getAllClientLeadStatusForSelection = async () => {
    try {
      const res = await GetAllClientLeadStatusForSelection();
      if (res?.data?.result) {
        setAllLeadStatuses(res.data.result);
      }
    } catch (e) {
      console.error("Error fetching lead statuses:", e);
    }
  };

  // ─── Sync inputValues whenever leadTabCount changes ─────────────────────
  const defaultValues = () => {
    const list = leadTabCount.map((item) => ({
      leadGridColumnId: item.leadGridColumnId,
      columnName: item.dashboardStatusName,
    }));
    setInputValues(list);
  };

  useEffect(() => {
    defaultValues();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [leadTabCount]);

  // ─── Reload on statusMoved or delete ────────────────────────────────────
  useEffect(() => {
    getLeadTabsCountConfig();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [statusMoved, isDeletedConfirm]);

  useEffect(() => {
    getAllClientLeadStatusForSelection();
  }, []);

  // ─── Handle individual tab name change ──────────────────────────────────
  const handleInputChange = (id, value) => {
    setInputValues((prev) =>
      prev.map((item) =>
        item.leadGridColumnId === id
          ? { ...item, columnName: value }
          : item
      )
    );
  };

  // ─── "Update Lead Tab" button — saves all tab names + status values ─────
  const handleUpdateLeadTab = () => {
    // Validate: no tab (except the default "All" tab) should have empty status
    const invalidTabs = leadTabCount.filter(
      (item) =>
        !item.isDefaultStatusTab &&
        (!item.dashboardStatusValue || item.dashboardStatusValue.trim() === "")
    );
    if (invalidTabs.length > 0) {
      const names = invalidTabs.map((t) => `"${t.dashboardStatusName}"`).join(", ");
      warningNotification(`Please select at least one status for tab(s): ${names}`);
      return;
    }

    setIsLoading(true);
    const list = leadTabCount.map((item) => {
      const inputItem = inputValues.find(
        (v) => v.leadGridColumnId === item.leadGridColumnId
      );
      return {
        LeadGridColumnId: item.leadGridColumnId,
        DashboardStatusValue: item.dashboardStatusValue || "",
        ColumnName: inputItem ? inputItem.columnName : item.dashboardStatusName,
      };
    });

    UpdateLeadTab({ List: list })
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Lead tab updated successfully");
          // Refresh the tab list after successful update
          getLeadTabsCountConfig();
        }
      })
      .catch(() => errorNotification("Something went wrong"))
      .finally(() => setIsLoading(false));
  };

  return (
    <Box>
      {pageLoading ? (
        <Box display="flex" justifyContent="center" alignItems="center" height="300px">
          <CircularProgress />
        </Box>
      ) : (
        <Box>
          {leadTabCount.map((data, index) => (
            <LeadTabList
              key={data.leadGridColumnId ?? index}
              tabList={data}
              existingStatuses={data.dashboardStatusValue}
              setLeadTabCount={setLeadTabCount}
              allLeadStatuses={allLeadStatuses}
              getAllClientLeadStatusForSelection={getAllClientLeadStatusForSelection}
              statusMoved={statusMoved}
              isDeletedConfirm={isDeletedConfirm}
              setIsDeletedConfirm={setIsDeletedConfirm}
              leadTabCount={leadTabCount}
              inputValues={inputValues}
              setInputValues={setInputValues}
              handleInputChange={handleInputChange}
            />
          ))}
        </Box>
      )}

      {openAddTabModal && (
        <AddLeadTabModal
          open={openAddTabModal}
          setOpen={setOpenAddTabModal}
          setStatusMoved={setStatusMoved}
          onSuccess={() => {
            getLeadTabsCountConfig();
            getAllClientLeadStatusForSelection();
          }}
        />
      )}
    </Box>
  );
});

export default LeadTabsSetup;
