import AccountBalanceIcon from "@mui/icons-material/AccountBalance";
import { Box, ButtonGroup, Stack } from "@mui/material";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import DataGridTabs from "../../../.reUseableComponents/DataGridTabs/DataGridTabs";
import { styleSheet } from "../../../assets/styles/style";
import CreateBankModal from "../../../components/modals/settingsModals/CreateBankModal";
import {
  ActionButtonCustom,
  fetchMethod,
} from "../../../utilities/helpers/Helpers";
import PayoutBankList from "./list";
import { GetAllPayoutBanks } from "../../../api/AxiosInterceptors";

const PayoutBank = () => {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const [loading, setLoading] = useState(false);
  const [openBankModal, setOpenBankModal] = useState(false);
  const [allPayOutBank, setAllPayOutBank] = useState([]);

  const getAllPayoutBanks = async () => {
    setLoading(true);
    try {
      const { response } = await fetchMethod(() => GetAllPayoutBanks());
      if (response.isSuccess) {
        const updatedResult = response.result.map((item, index) => ({
          ...item,
          rowNum: index + 1,
        }));
        setAllPayOutBank(updatedResult);
      }
    } catch (error) {
      console.error("Error fetching client return reasons:", error);
    }
    setLoading(false);
  };

  useEffect(() => {
    getAllPayoutBanks();
  }, []);

  return (
    <>
      <Box sx={styleSheet.pageRoot}>
        <div style={{ padding: "10px" }}>
          <Box sx={styleSheet.topNavBar}>
            <Box
              sx={{ ...styleSheet.topNavBarLeft, fontWeight: "900 !important" }}
            ></Box>
            <Stack
              sx={styleSheet.topNavBarRight}
              direction="row"
              justifyContent="flex-end"
              alignItems="center"
              spacing={1}
            >
              <ButtonGroup
                variant="outlined"
                aria-label="split button"
              ></ButtonGroup>
            </Stack>
          </Box>
          <DataGridTabs
            tabsSmWidth="100px"
            tabsMdWidth="100px"
            tabData={[
              {
                label: LanguageReducer?.languageType?.SETTINGS_PAYOUT_BANK_ALL,
                route: "/payout-bank",
                children: (
                  <PayoutBankList
                    loading={loading}
                    allPayOutBank={allPayOutBank}
                  />
                ),
              },
            ]}
            otherBtns={
              <ActionButtonCustom
                label={
                  LanguageReducer?.languageType
                    ?.SETTINGS_PAYOUT_BANK_CREATE_BANK
                }
                startIcon={<AccountBalanceIcon fontSize="small" />}
                onClick={() => setOpenBankModal(true)}
              />
            }
          />
        </div>
        {openBankModal && (
          <CreateBankModal
            open={openBankModal}
            onClose={() => setOpenBankModal(false)}
            getAllPayoutBanks={getAllPayoutBanks}
          />
        )}
      </Box>
    </>
  );
};

export default PayoutBank;
