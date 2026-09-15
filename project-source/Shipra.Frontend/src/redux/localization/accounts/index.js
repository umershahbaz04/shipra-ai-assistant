import {
  carrierSettlementArabic,
  carrierSettlementChines,
  carrierSettlementEnglish,
  carrierSettlementUrdu,
} from "./carrierSettlement/carrierSettlement";
import {
  codPendingArabic,
  codPendingChines,
  codPendingEnglish,
  codPendingUrdu,
} from "./codPending/codPending";

export const accountEnglish = {
  ...codPendingEnglish,
  ...carrierSettlementEnglish,
  ACCOUNTS: "Accounts",
};
export const accountArabic = {
  ...codPendingArabic,
  ...carrierSettlementArabic,
  ACCOUNTS: "حسابات",
};
export const accountChinese = {
  ...codPendingChines,
  ...carrierSettlementChines,
  ACCOUNTS: "账户",
};
export const accountUrdu = {
  ...codPendingUrdu,
  ...carrierSettlementUrdu,
  ACCOUNTS: "اکاؤنٹس",
};
