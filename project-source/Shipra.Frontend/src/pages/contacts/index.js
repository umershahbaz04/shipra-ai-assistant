import {
  Box,
} from "@mui/material";

import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Route, Routes } from "react-router-dom";

import { GetLeadContacts } from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import { warningNotification } from "../../utilities/toast";

import SearchInputAutoCompleteMultiple from "../../.reUseableComponents/TextField/SearchInputAutoCompleteMultiple";
import ContactOrdersModal from "../../components/modals/leadsModals/ContactOrdersModal";

import ContactsList from "./contactsList";
import DataGridTabs from "../../.reUseableComponents/DataGridTabs/DataGridTabs";

const MAX_TAGS = 200;

function Contacts(props) {
  const [inputFields, setInputFields] = useState([]);
  const [allContacts, setAllContacts] = useState([]);
  const [isContactsLoading, setIsContactsLoading] = useState(false);
  const [openContactModal, setOpenContactModal] = useState(false);
  const [selectedContactMobile, setSelectedContactMobile] = useState(null);

  const LanguageReducer = useSelector((state) => state.LanguageReducer);

  const getAllContacts = async () => {
    setIsContactsLoading(true);
    try {
      const params = {
        Start: 0,
        Length: 1000,
        Search: inputFields.join(),
      };
      const res = await GetLeadContacts(params);
      if (res?.data?.result) {
        setAllContacts(res.data.result);
      }
    } catch (e) {
      console.error("Error fetching contacts:", e);
    } finally {
      setIsContactsLoading(false);
    }
  };

  useEffect(() => {
    getAllContacts();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    let timeoutId;
    if (inputFields.length > 0) {
      timeoutId = setTimeout(() => {
        getAllContacts();
      }, 800);
    } else {
      getAllContacts();
    }
    return () => clearTimeout(timeoutId);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [inputFields]);

  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        <Box sx={{ mb: 1 }}>
          <Box
            sx={{ display: "flex", alignItems: "center", gap: 1 }}
          >
            <Box flexGrow={1}>
              <SearchInputAutoCompleteMultiple
                onChange={(e, value) => {
                  if (value.length <= MAX_TAGS) {
                    setInputFields(value.slice(0, MAX_TAGS));
                  } else {
                    warningNotification(
                      LanguageReducer?.languageType?.MAXMIUM_NUMBER_REACHED ||
                        "Maximum tag limit reached"
                    );
                  }
                }}
                inputFields={inputFields}
                MAX_TAGS={MAX_TAGS}
              />
            </Box>
          </Box>
        </Box>

        <DataGridTabs
          handleTabChange={() => {}}
          tabData={[
            {
              label: "Contacts",
              route: "/contacts",
            },
          ]}
          actionBtnMenuData={null}
          otherBtns={null}
          filterBtn={null}
        />

        <Routes>
          <Route 
            path="/" 
            element={
              <ContactsList 
                loading={isContactsLoading} 
                allContacts={allContacts} 
                onContactClick={(mobile) => {
                  setSelectedContactMobile(mobile);
                  setOpenContactModal(true);
                }} 
              />
            } 
          />
        </Routes>
      </div>

      <ContactOrdersModal 
        open={openContactModal} 
        onClose={() => setOpenContactModal(false)} 
        mobileNumber={selectedContactMobile} 
      />
    </Box>
  );
}

export default Contacts;
