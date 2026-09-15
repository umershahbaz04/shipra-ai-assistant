import { Box } from "@mui/material";
import { purple } from "@mui/material/colors";
import React, { useEffect, useState } from "react";
import ModalButtonComponent from "../../../.reUseableComponents/Buttons/ModalButtonComponent";
import JsonViewViewer from "../../../.reUseableComponents/jsonViewer";
import ModalComponent from "../../../.reUseableComponents/Modal/ModalComponent";
import SelectComponent from "../../../.reUseableComponents/TextField/SelectComponent";
import TextFieldComponent from "../../../.reUseableComponents/TextField/TextFieldComponent";
import TextFieldLableComponent from "../../../.reUseableComponents/TextField/TextFieldLableComponent";
import {
  GetAllWebhookEventLookup,
  UpdateClientWebhookEvent,
} from "../../../api/AxiosInterceptors";
import { EnumOptions } from "../../../utilities/enum";
import {
  fetchMethod,
  GridContainer,
  GridItem,
} from "../../../utilities/helpers/Helpers";
import {
  errorNotification,
  successNotification,
} from "../../../utilities/toast";
import { useSelector } from "react-redux";
const initialSmsFieldsData = {
  url: "",
};
const UpdateWebhookUrlModal = (props) => {
  const { open, onClose, webhookUrl, getAllClientWebhookEvents } = props;
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [updateWebhook, setUpdateWebhook] = useState(initialSmsFieldsData);
  const [loading, setLoading] = useState(false);
  const [allWebhookEventLookup, setAllWebhookEventLookup] = useState();
  const [selectedWebhookEvent, setselectedWebhookEvent] = useState();
  const [webhookConfig, setwebhookConfig] = useState({});
  const handleChange = (e) => {
    const { name, value } = e.target;
    setUpdateWebhook((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const getAllWebhookEventLookup = async () => {
    try {
      const { response } = await fetchMethod(GetAllWebhookEventLookup);
      if (response?.result) {
        setAllWebhookEventLookup(response?.result);
      } else {
        console.log("No data found");
      }
    } catch (error) {
      console.error("Error fetching order returns:", error);
    }
  };

  const handleupdateWebhook = async () => {
    const body = {
      clientWebhookEventId: webhookUrl.clientWebhookEventId,
      webhookEventLookupId: selectedWebhookEvent.webhookEventLookupId,
      webhookUrl: updateWebhook.url,
    };
    try {
      const res = await UpdateClientWebhookEvent(body);
      if (!res?.data?.isSuccess) {
        errorNotification("Failed to update Webhook.");
      } else {
        successNotification("Webhook updated successfully.");
        getAllClientWebhookEvents();
      }
    } catch (e) {
      errorNotification("An error occurred while creating the Webhook.");
    } finally {
      setUpdateWebhook(initialSmsFieldsData);
      setselectedWebhookEvent();
      setLoading(false);
      onClose();
    }
  };

  useEffect(() => {
    getAllWebhookEventLookup();
  }, []);

  useEffect(() => {
    if (
      selectedWebhookEvent?.webhookEventLookupId > 0 &&
      selectedWebhookEvent.config
    ) {
      setwebhookConfig(JSON.parse(selectedWebhookEvent?.config));
    }
  }, [selectedWebhookEvent]);

  useEffect(() => {
    if (webhookUrl) {
      setUpdateWebhook({
        url: webhookUrl.webhookUrl,
      });
      const matchedWebhookEvent = allWebhookEventLookup?.find(
        (event) =>
          event.webhookEventLookupId === webhookUrl?.webhookEventLookupId
      );
      setselectedWebhookEvent(matchedWebhookEvent);
    }
  }, [webhookUrl, allWebhookEventLookup]);

  return (
    <>
      <ModalComponent
        open={open}
        onClose={onClose}
        maxWidth="sm"
        title={LanguageReducer?.languageType?.SETTING_WEBHOOK_UPDATE_WEBHOOK}
        actionBtn={
          <ModalButtonComponent
            title={
              LanguageReducer?.languageType?.SETTING_WEBHOOK_UPDATE_WEBHOOK
            }
            bg={purple}
            loading={loading}
            onClick={(e) => handleupdateWebhook()}
          />
        }
      >
        <GridContainer spacing={1}>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={LanguageReducer?.languageType?.SETTING_WEBHOOK_URL}
            />
            <TextFieldComponent
              sx={{
                fontsize: "14px",
                marginTop: "4px",
                "& .css-setb27-MuiInputBase-input-MuiOutlinedInput-input": {
                  padding: "8px!important",
                },
              }}
              type="text"
              placeholder={"Enter url"}
              value={updateWebhook.url}
              name="url"
              onChange={handleChange}
            />
          </GridItem>
          <GridItem sm={12} md={12} lg={12}>
            <TextFieldLableComponent
              title={
                LanguageReducer?.languageType?.SETTING_WEBHOOK_WEBHOOK_EVENT
              }
            />
            <SelectComponent
              name="webhookevent"
              options={allWebhookEventLookup}
              value={selectedWebhookEvent}
              optionLabel={EnumOptions.WEBHOOK_EVENTS.LABEL}
              optionValue={EnumOptions.WEBHOOK_EVENTS.VALUE}
              isRefesh={true}
              handleRefreshClick={getAllWebhookEventLookup}
              onChange={(e, val) => {
                setselectedWebhookEvent(val);
              }}
            />
          </GridItem>
          {selectedWebhookEvent?.webhookEventLookupId > 0 &&
            selectedWebhookEvent.config && (
              <Box sx={{ width: "80%", margin: "auto" }}>
                <JsonViewViewer title="Body" jsonData={webhookConfig} />
              </Box>
            )}
        </GridContainer>
      </ModalComponent>
    </>
  );
};

export default UpdateWebhookUrlModal;
