import {
  codPendingArabic,
  codPendingChinese,
  codPendingEnglish,
  codPendingUrdu,
} from "./codPending/codPending";
import {
  deliveryNotesArabic,
  deliveryNotesChines,
  deliveryNotesEnglish,
  deliveryNotesUrdu,
} from "./deliveryNotes/deliveryNotes";
import {
  deliveryTaskArabic,
  deliveryTaskChines,
  deliveryTaskEnglish,
  deliveryTaskUrdu,
} from "./deliveryTasks/deliveryTask";
import {
  driverRecievableArabic,
  driverRecievableChines,
  driverRecievableEnglish,
  driverRecievableUrdu,
} from "./driverRecievable/driverRecievable";
import {
  expenseArabic,
  expenseChines,
  expenseEnglish,
  expenseUrdu,
} from "./expense/expense";
import {
  pendingForReturnArabic,
  pendingForReturnChines,
  pendingForReturnEnglish,
  pendingForReturnUrdu,
} from "./pendingForReturn/pendingForReturn";
import {
  returnArabic,
  returnChinese,
  returnEnglish,
  returnUrdu,
} from "./returns/return";

export const myCarrierEnglish = {
  ...codPendingEnglish,
  ...deliveryNotesEnglish,
  ...deliveryTaskEnglish,
  ...driverRecievableEnglish,
  ...expenseEnglish,
  ...pendingForReturnEnglish,
  ...returnEnglish,
  My_CARRIER: "My Carrier",
};
export const myCarrierArabic = {
  ...codPendingArabic,
  ...deliveryNotesArabic,
  ...deliveryTaskArabic,
  ...driverRecievableArabic,
  ...expenseArabic,
  ...pendingForReturnArabic,
  ...returnArabic,
  My_CARRIER: "ناقلي",
};
export const myCarrierChinese = {
  ...codPendingChinese,
  ...deliveryNotesChines,
  ...deliveryTaskChines,
  ...driverRecievableChines,
  ...expenseChines,
  ...pendingForReturnChines,
  ...returnChinese,
  My_CARRIER: "我的承运人",
};
export const myCarrierUrdu = {
  My_CARRIER: "میرا کیریئر",
  ...codPendingUrdu,
  ...deliveryNotesUrdu,
  ...deliveryTaskUrdu,
  ...driverRecievableUrdu,
  ...expenseUrdu,
  ...pendingForReturnUrdu,
  ...returnUrdu,
};
