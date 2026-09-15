import {
  ApiIntegrationArabic,
  ApiIntegrationChines,
  ApiIntegrationEnglish,
  ApiIntegrationUrdu,
} from "./apiIntegration/apiIntegration";
import {
  carrierArabic,
  carrierChines,
  carrierEnglish,
  carrierUrdu,
} from "./carriers/carrier";
import {
  paymentIntegrationArabic,
  paymentIntegrationChines,
  paymentIntegrationEnglish,
  paymentIntegrationUrdu,
} from "./paymentIntegration/paymentIntegration";
import {
  saleChannelArabic,
  saleChannelChinese,
  saleChannelEnglish,
  saleChannelUrdu,
} from "./saleChannels/saleChannel";
import {
  smsIntegrationArabic,
  smsIntegrationChines,
  smsIntegrationEnglish,
  smsIntegrationUrdu,
} from "./smsIntegration/smsIntegration";

export const integrationEnglish = {
  ...ApiIntegrationEnglish,
  ...carrierEnglish,
  ...paymentIntegrationEnglish,
  ...saleChannelEnglish,
  ...smsIntegrationEnglish,
  INTEGRATION: "Integration",
  START_DATE: "Start Date",
  END_DATE: "End Date",
  FILTER: "Filter",
  CLEAR_FILTER: "Clear FIlter",
};
export const integrationArabic = {
  ...ApiIntegrationArabic,
  ...carrierArabic,
  ...paymentIntegrationArabic,
  ...saleChannelArabic,
  ...smsIntegrationArabic,
  INTEGRATION: "اندماج",
  FILTER: "تصفية",
  CLEAR_FILTER: "مسح التصفية",
  START_DATE: "تاريخ البدء",
  END_DATE: "تاريخ الانتهاء",
};
export const integrationChinese = {
  ...ApiIntegrationChines,
  ...carrierChines,
  ...paymentIntegrationChines,
  ...saleChannelChinese,
  ...smsIntegrationChines,
  INTEGRATION: "一体化",
  FILTER: "过滤",
  CLEAR_FILTER: "清除过滤",
  START_DATE: "开始日期",
  END_DATE: "结束日期",
};
export const integrationUrdu = {
  ...ApiIntegrationUrdu,
  ...carrierUrdu,
  ...paymentIntegrationUrdu,
  ...saleChannelUrdu,
  ...smsIntegrationUrdu,
  INTEGRATION: "انضمام",
  START_DATE: "شروع ہونے کی تاریخ",
  END_DATE: "اختتام کی تاریخ",
  FILTER: "فلٹر",
  CLEAR_FILTER: "فلٹر صاف کریں",
};
