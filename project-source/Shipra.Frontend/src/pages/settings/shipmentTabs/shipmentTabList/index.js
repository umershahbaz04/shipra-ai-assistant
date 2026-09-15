import { Box, ListItem, TextField, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import DeleteConfirmationModal from "../../../../.reUseableComponents/Modal/DeleteConfirmationModal";
import SelectComponent from "../../../../.reUseableComponents/TextField/SelectComponent";
import { DeleteShipmentTab } from "../../../../api/AxiosInterceptors";
import { styleSheet } from "../../../../assets/styles/style";
import UtilityClass from "../../../../utilities/UtilityClass";
import { EnumOptions } from "../../../../utilities/enum";
import { ActionButtonDelete } from "../../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../../utilities/toast";
const ShipmentTabList = (props) => {
  const {
    tabList,
    exsitingStatues,
    setShipmentTabCount,
    allCarrierTrackingStatus,
    setIsDeletedConfirm,
    isDeletedConfirm,
    shipmentTabCount,
    inputValues,
    setInputValues,
    handleInputChange,
    getAllCarrierTrackingStatusForSelection,
  } = props;
  const [selectedStatusIds, setSelectedStatusIds] = useState([]);
  const [openDeleteRecord, setOpenDeleteRecord] = useState(false);
  const [selectedTabId, setSelectedTabId] = useState();
  const [isChanges, setIschanges] = useState(false);
  const [recentUpdatedId, setRecentUpdatedId] = useState(null);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  useEffect(() => {
    if (exsitingStatues && exsitingStatues.length > 0) {
      const stringArray = exsitingStatues.split(",");
      const numericIds = stringArray.map(Number);
      const newArray = allCarrierTrackingStatus
        .filter((item) => numericIds.includes(item.carrierTrackingStatusId))
        .map((item) => ({
          carrierTrackingStatusId: item.carrierTrackingStatusId,
          trackingStatus: item.trackingStatus,
        }));
      setSelectedStatusIds(newArray);
    }
  }, [exsitingStatues, allCarrierTrackingStatus]);

  const handleChangeStatus = (id) => {
    const dashboardStatusIdValues = selectedStatusIds.map(
      (status) => status.carrierTrackingStatusId
    );

    setShipmentTabCount((prevData) =>
      prevData.map((item) => {
        if (item.shipmentGridColumnId === id) {
          return {
            ...item,
            dashboardStatusValue: dashboardStatusIdValues.join(","),
          };
        } else {
          const currentValues2 = item.dashboardStatusValue
            .split(",")
            .map((val) => val.trim());
          const currentValues = currentValues2.map(Number);
          const updatedValues = currentValues.filter(
            (val) => !dashboardStatusIdValues.includes(val)
          );
          const updatedString = updatedValues.join(",");
          return { ...item, dashboardStatusValue: updatedString };
        }
      })
    );
    setIschanges(false);
  };
  useEffect(() => {
    handleChangeStatus(recentUpdatedId);
  }, [isChanges]);
  const handleDeleteConfirmation = (id) => {
    setSelectedTabId(id);
    setOpenDeleteRecord(true);
  };
  const handleDelete = () => {
    if (selectedTabId) {
      let param = {
        ShipmentGridColumnId: selectedTabId,
      };
      DeleteShipmentTab(param)
        .then((res) => {
          if (!res?.data?.isSuccess) {
            UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
          } else {
            successNotification("Shipment tab deleted successfully");
          }
        })
        .catch((e) => {
          errorNotification("Something went wrong");
        })
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
          {/* <Typography variant="caption">{tabList.dashboardStatusName}</Typography> */}
          <TextField
            type="text"
            size="small"
            defaultValue={tabList.dashboardStatusName || ""}
            onChange={(e) =>
              handleInputChange(tabList.shipmentGridColumnId, e.target.value)
            }
            disabled={tabList.isDefaultStatusTab}
            sx={styleSheet.tabInputStyle}
          />
          {!tabList.isDefaultStatusTab == true ? (
            <ActionButtonDelete
              label=""
              onClick={(e) =>
                handleDeleteConfirmation(tabList.shipmentGridColumnId)
              }
            />
          ) : null}
        </Box>
        <Box>
          <Typography variant="h5">
            {LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_SELECT_STATUS}
          </Typography>
          <SelectComponent
            multiple={true}
            name={tabList.dashboardStatusName}
            options={allCarrierTrackingStatus}
            value={selectedStatusIds}
            optionLabel={EnumOptions.CHOOSE_STATUS.LABEL}
            optionValue={EnumOptions.CHOOSE_STATUS.VALUE}
            isRefesh={true}
            handleRefreshClick={getAllCarrierTrackingStatusForSelection}
            onChange={(e, val) => {
              setSelectedStatusIds(val);
              setRecentUpdatedId(tabList.shipmentGridColumnId);
              setIschanges(true);
            }}
            renderOption={(props, option, { selected }) => (
              <ListItem
                {...props}
                style={{ backgroundColor: selected ? "#edeaff" : "inherit" }}
              >
                {option.trackingStatus}
              </ListItem>
            )}
            isOptionEqualToValue={(option, value) =>
              option.carrierTrackingStatusId === value.carrierTrackingStatusId
            }
            padding={"5px "}
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

export default ShipmentTabList;
