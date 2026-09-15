import { Box } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import ButtonComponent from "../../../.reUseableComponents/Buttons/ButtonComponent";
import {
  GetAllCarrierTrackingStatusForSelection,
  GetShipmentTabsCountConfig,
  UpdateShipmentTab,
} from "../../../api/AxiosInterceptors";
import AddShipmentTabModal from "../../../components/modals/orderModals/AddShipmentTabModal";
import UtilityClass from "../../../utilities/UtilityClass";
import { PageMainBox } from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { ProfileDetailsBox } from "../../Profile/Profile/Profile";
import ShipmentTabList from "./shipmentTabList";
function ShipmentTabs() {
  const [shipmentTabCount, setShipmentTabCount] = useState([]);
  const [allCarrierTrackingStatus, setAllCarrierTrackingStatus] = useState([]);
  const [openAddTabModel, setOpenAddTabModel] = useState(false);
  const [statusMoved, setStatusMoved] = useState(false);
  const [isDeletedConfirm, setIsDeletedConfirm] = useState(false);
  const [inputValues, setInputValues] = useState([]);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const handleAddTab = (e) => {
    setOpenAddTabModel(true);
  };
  const [unassiignedValues, setUnassignedValues] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  let getShipmentTabsCountConfig = async () => {
    let res = await GetShipmentTabsCountConfig();
    if (res.data.result !== null) {
      res.data.result = res.data.result?.filter(
        (x, index) => x.dashboardStatusName.toLowerCase() != "all"
      );
    }
    setShipmentTabCount(res.data.result);
  };
  let getAllCarrierTrackingStatusForSelection = async () => {
    let res = await GetAllCarrierTrackingStatusForSelection();

    if (res.data.result !== null) {
      if (res.data.result.find((x) => x?.carrierTrackingStatusId == 0)) {
        res.data.result = res.data.result?.filter(
          (x) => x?.carrierTrackingStatusId != 0
        );
      }
      setAllCarrierTrackingStatus(res.data.result);
    }
  };
  const defaultValues = () => {
    const list = shipmentTabCount.map((item) => ({
      shipmentGridColumnId: item.shipmentGridColumnId,
      columnName: item.dashboardStatusName,
    }));
    setInputValues(list);
  };
  useEffect(() => {
    defaultValues();
  }, [shipmentTabCount]);
  useEffect(() => {
    getShipmentTabsCountConfig();
  }, [statusMoved, isDeletedConfirm]);
  useEffect(() => {
    getAllCarrierTrackingStatusForSelection();
  }, []);
  const getUnAssignedValues = () => {
    const uniqueValues = [
      ...new Set(shipmentTabCount.map((item) => item.dashboardStatusValue)),
    ];
    const resultString = uniqueValues.join(",");

    if (resultString && resultString.length > 0) {
      const stringArray = resultString.split(",");
      const numericIds = stringArray.map(Number);
      const newArray = allCarrierTrackingStatus
        .filter((item) => !numericIds.includes(item.carrierTrackingStatusId))
        .map((item) => ({
          carrierTrackingStatusId: item.carrierTrackingStatusId,
          trackingStatus: item.trackingStatus,
        }));
      setUnassignedValues(newArray);
    }
  };
  useEffect(() => {
    getUnAssignedValues();
  }, [shipmentTabCount, allCarrierTrackingStatus]);

  const handleInputChange = (id, value) => {
    setInputValues((prev) => {
      const updatedValues = prev.map((item) =>
        item.shipmentGridColumnId === id
          ? { ...item, columnName: value, shipmentGridColumnId: id }
          : item
      );
      return updatedValues;
    });
  };
  const handleShipmentTab = () => {
    setIsLoading(true);
    const list = shipmentTabCount.map((item) => {
      const obj = inputValues.find(
        (inputItem) =>
          inputItem.shipmentGridColumnId == item.shipmentGridColumnId
      );
      return {
        shipmentGridColumnId: item.shipmentGridColumnId,
        dashboardStatusValue: item.dashboardStatusValue,
        columnName: obj ? obj.columnName : item.dashboardStatusName, // Use a default value if obj is not found
      };
    });
    const requestData = {
      list,
    };
    UpdateShipmentTab(requestData)
      .then((res) => {
        if (!res?.data?.isSuccess) {
          UtilityClass.showErrorNotificationWithDictionary(res.data.errors);
        } else {
          successNotification("Shipment tab updated successfully");
        }
      })
      .catch((e) => {
        console.log("e", e);
        errorNotification("Something went wrong");
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  return (
    <PageMainBox>
      <ProfileDetailsBox
        rightBtn={
          <div style={{ display: "flex", gap: "10px" }}>
            <ButtonComponent
              style={{ marginRight: "10px" }}
              title={
                LanguageReducer?.languageType?.SETTING_SHIPMENT_TAB_ADD_NEW_TAB
              }
              // loading={isloading}
              onClick={handleAddTab}
            />
            <ButtonComponent
              style={{ marginRight: "10px" }}
              title={
                LanguageReducer?.languageType
                  ?.SETTING_SHIPMENT_TAB_UPDATE_SHIPMENT_TAB
              }
              loading={isLoading}
              onClick={handleShipmentTab}
            />
          </div>
        }
      >
        <Box>
          {shipmentTabCount.map((data, index) => (
            <ShipmentTabList
              key={index}
              tabList={data}
              exsitingStatues={data.dashboardStatusValue}
              setShipmentTabCount={setShipmentTabCount}
              allCarrierTrackingStatus={allCarrierTrackingStatus}
              getAllCarrierTrackingStatusForSelection={
                getAllCarrierTrackingStatusForSelection
              }
              statusMoved={statusMoved}
              isDeletedConfirm={isDeletedConfirm}
              setIsDeletedConfirm={setIsDeletedConfirm}
              shipmentTabCount={shipmentTabCount}
              inputValues={inputValues}
              setInputValues={setInputValues}
              handleInputChange={handleInputChange}
            />
          ))}
        </Box>

        {openAddTabModel && (
          <AddShipmentTabModal
            open={openAddTabModel}
            setOpen={setOpenAddTabModel}
            setStatusMoved={setStatusMoved}
          />
        )}
      </ProfileDetailsBox>
    </PageMainBox>
  );
}

export default ShipmentTabs;
