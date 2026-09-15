import { Box, ListItem, TextField, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import SelectComponent from "../../../../.reUseableComponents/TextField/SelectComponent";
import { DeleteLeadTab } from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UtilityClass from "../../../../utilities/UtilityClass";
import { EnumOptions } from "../../../../utilities/enum";
import { ActionButtonDelete } from "../../../../utilities/helpers/Helpers";
import { errorNotification, successNotification } from "../../../../utilities/toast";

const LeadTabList = (props) => {
  const {
    tabList,
    existingStatuses,         // comma-separated clientLeadStatusId values
    setLeadTabCount,
    allLeadStatuses,          // full ClientLeadStatusLookup array
    setIsDeletedConfirm,
    isDeletedConfirm,
    inputValues,
    setInputValues,
    handleInputChange,
    getAllClientLeadStatusForSelection,
  } = props;

  const [selectedStatusIds, setSelectedStatusIds] = useState([]);
  const [openDeleteRecord, setOpenDeleteRecord] = useState(false);
  const [selectedTabId, setSelectedTabId] = useState();

  // Parse existingStatuses string → array of objects
  useEffect(() => {
    if (existingStatuses && existingStatuses.length > 0) {
      const numericIds = existingStatuses.split(",").map(Number);
      const matched = allLeadStatuses
        .filter((s) => numericIds.includes(s.clientLeadStatusId))
        .map((s) => ({
          clientLeadStatusId: s.clientLeadStatusId,
          description: s.description,
        }));
      setSelectedStatusIds(matched);
    } else {
      setSelectedStatusIds([]);
    }
  }, [existingStatuses, allLeadStatuses]);

  // Called directly when user picks/removes statuses in the multi-select.
  // Receives the freshly-selected values (newVal) so we don't rely on stale state.
  const handleChangeStatus = (tabId, newVal) => {
    const selectedIds = newVal.map((s) => s.clientLeadStatusId);

    setLeadTabCount((prev) =>
      prev.map((item) => {
        if (item.leadGridColumnId === tabId) {
          // Assign the new selection to this tab
          return { ...item, dashboardStatusValue: selectedIds.join(",") };
        } else {
          // Remove from other tabs any status that was just claimed here
          const current = (item.dashboardStatusValue || "")
            .split(",")
            .map((v) => v.trim())
            .filter(Boolean)
            .map(Number);
          const updated = current.filter((v) => !selectedIds.includes(v));
          return { ...item, dashboardStatusValue: updated.join(",") };
        }
      })
    );
  };

  const handleDeleteConfirmation = (id) => {
    setSelectedTabId(id);
    setOpenDeleteRecord(true);
  };

  const handleDelete = () => {
    if (selectedTabId) {
      DeleteLeadTab({ LeadGridColumnId: selectedTabId })
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
          } else {
            successNotification("Lead tab deleted successfully");
          }
        })
        .catch(() => errorNotification("Something went wrong"))
        .finally(() => {
          setSelectedTabId(null);
          setIsDeletedConfirm(false);
          setOpenDeleteRecord(false);
        });
    }
  };

  useEffect(() => {
    if (isDeletedConfirm) {
      handleDelete();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isDeletedConfirm]);

  return (
    <>
      <Box
        sx={{
          marginBottom: "20px",
          boxShadow: "0px 0px 5px #c3c3c3",
          padding: "18px",
          borderRadius: "8px!important",
          backgroundColor: "#ffffff87",
        }}
      >
        {/* Tab Name row */}
        <Box
          sx={{
            border: "1px solid",
            padding: "5px",
            borderRadius: "4px",
            marginBottom: "10px",
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          <TextField
            fullWidth
            type="text"
            size="small"
            defaultValue={tabList.dashboardStatusName || ""}
            onChange={(e) =>
              handleInputChange(tabList.leadGridColumnId, e.target.value)
            }
            sx={styleSheet.tabInputStyle}
          />
          {!tabList.isDefaultStatusTab && (
            <ActionButtonDelete
              label=""
              onClick={() => handleDeleteConfirmation(tabList.leadGridColumnId)}
            />
          )}
        </Box>

        {/* Status multi-select */}
        <Box>
          <Typography variant="h5">Select Status</Typography>
          <SelectComponent
            multiple={true}
            name={tabList.dashboardStatusName}
            options={allLeadStatuses}
            value={selectedStatusIds}
            optionLabel={EnumOptions.CLIENT_LEAD_STATUS.LABEL}
            optionValue={EnumOptions.CLIENT_LEAD_STATUS.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllClientLeadStatusForSelection}
            onChange={(e, val) => {
              setSelectedStatusIds(val);
              // Pass val directly to avoid stale-state closure issues
              handleChangeStatus(tabList.leadGridColumnId, val);
            }}
            renderOption={(props, option, { selected }) => (
              <ListItem
                {...props}
                style={{ backgroundColor: selected ? "#edeaff" : "inherit" }}
              >
                {option.description}
              </ListItem>
            )}
            isOptionEqualToValue={(option, value) =>
              option.clientLeadStatusId === value.clientLeadStatusId
            }
            padding={"5px"}
          />
        </Box>
      </Box>

      <DeleteConfirmationModal
        open={openDeleteRecord}
        setOpen={setOpenDeleteRecord}
        setIsDeletedConfirm={setIsDeletedConfirm}
        loading={isDeletedConfirm}
        {...props}
      />
    </>
  );
};

export default LeadTabList;
