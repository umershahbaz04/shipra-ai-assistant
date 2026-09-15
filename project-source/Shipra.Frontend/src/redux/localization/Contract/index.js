import {
  GenerateInvoiceArabic,
  GenerateInvoiceChinese,
  GenerateInvoiceEnglish,
  GenerateInvoiceUrdu,
} from "./GenerateInvoice";
import {
  InvoiceArabic,
  InvoiceChinese,
  InvoiceEnglish,
  InvoiceUrdu,
} from "./Invoice";
import {
  ServiceRateGroupArabic,
  ServiceRateGroupChinese,
  ServiceRateGroupEnglish,
  ServiceRateGroupUrdu,
} from "./ServiceRateGroup";
import {
  ShipperRatesArabic,
  ShipperRatesChinese,
  ShipperRatesEnglish,
  ShipperRatesUrdu,
} from "./ShipperRates";

export const ContractEnglish = {
  ...GenerateInvoiceEnglish,
  ...ServiceRateGroupEnglish,
  ...InvoiceEnglish,
  ...ShipperRatesEnglish,
  CONTRACT_CONTRACT_TEXT: "Contract",
};
export const ContractArabic = {
  ...GenerateInvoiceArabic,
  ...ServiceRateGroupArabic,
  ...InvoiceArabic,
  ...ShipperRatesArabic,
  CONTRACT_CONTRACT_TEXT: "عقد",
};
export const ContractChinese = {
  ...GenerateInvoiceChinese,
  ...ServiceRateGroupChinese,
  ...InvoiceChinese,
  ...ShipperRatesChinese,
  CONTRACT_CONTRACT_TEXT: "合同",
};
export const ContractUrdu = {
  ...GenerateInvoiceUrdu,
  ...ServiceRateGroupUrdu,
  ...InvoiceUrdu,
  ...ShipperRatesUrdu,
  CONTRACT_CONTRACT_TEXT: "معاہدہ",
};
