import integrationBackIcon from "../images/integrationBackIcon.png";
import modalBack from "../images/modalBack.png";
export const styleSheet = {
  "@global": {
    ".auth-app-bar .MuiToolbar-root": {
      backgroundColor: "white",
    },

    ".method-main-loader": {
      height: "600px",
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
    },
    ".method-main-loader .blob": {
      background: "#E5EAEE",
      borderRadius: "50%",
      boxShadow: "0 0 0 0 black",
      margin: "10px",
      height: "116px",
      width: "116px",
      animation: "pulse 2s infinite",
    },
    "@keyframes pulse": {
      "0%": {
        boxShadow: "0 0 0 0 #181C32",
      },
      "70%": {
        boxShadow: "0 0 0 100px rgba(0, 0, 0, 0)",
      },
      "100%": {
        boxShadow: "0 0 0 0 rgba(0, 0, 0, 0)",
      },
    },
    ".method-main-loader .setImage-dfd": {
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
    },
    ".method-main-loader .setImage-dfd img": {
      width: "80px",
    },
    ".Unauthorized-page": {
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      height: "90vh",
      lineHeight: "30px",
    },
    ".Unauthorized-page h1": {
      fontSize: "40px",
      fontWeight: "600",
      margin: "0",
      textAlign: "center",
      lineHeight: "60px",
    },
    ".Unauthorized-page p": {
      margin: "0",
      textAlign: "center",
    },
    ".Unauthorized-page .MuiSvgIcon-root": {
      fontSize: "120px",
    },
    ".Unauthorized-page .form-group-element": {
      marginTop: "30px",
      fontSize: "14px",
    },
    ".Unauthorized-page .form-group-element b": {
      cursor: "pointer",
      marginLeft: "7px",
    },
    ".Unauthorized-page .form-group-element button": {
      width: "100%",
      height: "35px",
      cursor: "pointer",
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      backgroundColor: "black",
      color: "white",
      borderRadius: "25px",
      maxWidth: "220px",
      textAlign: "center",
    },
    ".global-data-not-found": {
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      height: "100%",
      width: "100%",
    },
    ".global-data-not-found div": {
      textAlign: "center",
    },
  },
  pageRoot: {
    marginTop: "0px",
  },
  allOrderTable: {
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    height: "70vh",

    width: "100%",
    "& .MuiCheckbox-root": {
      fontSize: 18,
    },
    "& .Mui-checked": {
      color: "#1976D2 !important",
    },
    "& .MuiDataGrid-root": {
      borderRadius: "0px 0px 8px 8px",
    },
    "& .MuiDataGrid-main": {
      background: "#F8F8F8 !important",
    },
    "& .Mui-selected": {
      background: "#E6E1FD !important",
    },
    "& .MuiDataGrid-columnHeaders": {
      background: "#fff !important",
    },
  },
  simpleTable: {
    "& .MuiTableCell-root": {
      fontSize: "14px",
    },
    "& .MuiSvgIcon-root": {
      fontSize: "18px",
    },
  },
  simpleTable: {
    "& .MuiFormLabel-root": {
      // marginBottom: "1px",
    },
  },
  emailTextCell: {
    fontSize: "14px  !important",
    fontWeight: "500  !important",
    fontFamily: "Inter Medium !important",
    color: "var(--primary-color)  !important",
    textDecoration: "underline !important",
  },
  modelMainClassWithImage: {
    "& .MuiDialog-paper": {
      borderRadius: "20px !important",
      // backgroundImage: `url(${modalBack})`,
    },
    "& .MuiDialogActions-root": {
      padding: "0px  !important",
    },
    "& .MuiButtonBase-root": {
      marginLeft: "0px !important",
    },
  },
  modelMainClassWithImageWithImage: {
    "& .MuiDialog-paper": {
      borderRadius: "20px !important",
    },
    "& .MuiDialogActions-root": {
      padding: "0px  !important",
    },
    "& .MuiButtonBase-root": {
      marginLeft: "0px !important",
    },
  },
  modelMainClass: {
    "& .MuiDialog-paper": {
      borderRadius: "20px !important",
    },
    "& .MuiDialogActions-root": {
      padding: "0px  !important",
    },
    "& .MuiButtonBase-root": {
      marginLeft: "0px !important",
    },
  },
  cardMainClass: {
    borderRadius: "20px !important",
    "& .MuiDialogActions-root": {
      padding: "0px  !important",
    },
    "& .MuiButtonBase-root": {
      marginLeft: "0px !important",
    },
  },
  cardContentArea: {
    width: "500px",
    "& .MuiOutlinedInput-notchedOutline": {
      borderRadius: "8px !important",
    },
    "& .MuiSelect-select": {
      display: "flex !important",
    },
  },
  outScanHeading: {
    textAlign: "center  !important",
    fontSize: "25px  !important",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    fontWeight: "600 !important",
    marginTop: "15px !important",
  },
  modalSubmitButton: {
    background:
      "var(--primary-gradient) !important",
    height: "50px  !important",
    fontSize: "15px",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    textTransform: "capitalize  !important",
    borderRadius: "0px 0px 20px 20px !important",
  },

  modalDismissButton: {
    background: "#FF9A23 !important",
    height: "40px  !important",
    fontSize: "14px",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    textTransform: "capitalize  !important",
    borderRadius: "0px !important",
  },
  modalCarrierSubmitButton: {
    background:
      "var(--primary-color)",
    height: "40px  !important",
    fontSize: "14px",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    textTransform: "capitalize  !important",
    borderRadius: "0px !important",
  },
  modelContentArea: {
    "& .MuiOutlinedInput-notchedOutline": {
      borderRadius: "8px !important",
    },
    "& .MuiSelect-select": {
      display: "flex !important",
    },
    // "& .MuiButtonBase-root": {
    //     color: "var(--primary-color) !important"
    // }
  },

  tagsCard: {
    borderRadius: "10px !important",
    padding: "5px 10px",
  },
  tagsCardHeading: {
    fontSize: "20px  !important",
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    fontWeight: "600 !important",
    textAlign: "center !important",
    padding: "3px 0px !important",
  },
  ModalLogoArea: {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    marginLeft: "-15px",
    "& img": {
      width: "120px",
      marginBottom: "20px",
    },
  },
  dateInputStyle: { padding: "4px 5px" },

  inputLabel: {
    fontSize: "12px  !important",
    fontWeight: "400  !important",
    color: "#000000  !important",
    fontFamily: "'Lato', 'Inter', 'Arial' !important",
    textTransform: "capitalize  !important",
    marginTop: "4px",
    marginBottom: "4px",
    " .MuiFormLabel-asterisk": {
      color: "red",
      fontSize: "1.5rem",
      position: "relative",
      top: 8,
      right: 3,
      lineHeight: 0,
    },
  },
  inputLabelCity: {
    display: "flex!important",
    alignItems: "center",
    justifyContent: "space-between!important",
    fontSize: "12px  !important",
    fontWeight: "400  !important",
    color: "#000000  !important",
    fontFamily: "'Lato', 'Inter', 'Arial' !important",
    textTransform: "capitalize  !important",
    marginTop: "4px",
    marginBottom: "4px",
    " .MuiFormLabel-asterisk": {
      color: "red",
      fontSize: "1.5rem",
      position: "relative",
      top: 8,
      right: 3,
      lineHeight: 0,
    },
  },
  inputLabelRequired: {
    fontSize: "12px  !important",
    fontWeight: "400  !important",
    color: "#red  !important",
    fontFamily: "'Lato', 'Inter', 'Arial' !important",
    textTransform: "capitalize  !important",
    marginTop: "4px",
    marginBottom: "4px",
    " .MuiFormLabel-asterisk": {
      color: "red",
      fontSize: "1.5rem",
      position: "relative",
      top: 8,
      right: 3,
      lineHeight: 0,
    },
  },
  inputLabelAddProduct: {
    fontSize: "12px  !important",
    fontWeight: "400  !important",
    color: "#000000  !important",
    fontFamily: "'Lato', 'Inter', 'Arial' !important",
    textTransform: "capitalize  !important",
    marginTop: "4px",
    marginBottom: "4px",
    " .MuiFormLabel-asterisk": {
      color: "red",
      fontSize: "1.5rem",
      position: "relative",
      top: 8,
      right: 3,
      lineHeight: 0,
    },
  },
  addButton: {
    background:
      "var(--primary-gradient)",
    height: "40px  !important",
    fontSize: "18px",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    textTransform: "capitalize  !important",
  },
  addCarrierButton: {
    background:
      "var(--primary-gradient)",
    fontSize: "14px",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
    marginLeft: "5px",
  },
  addDriverHeadingAndUpload: {
    display: "flex  !important",
    justifyContent: "space-between  !important",
    alignItems: "center  !important",
  },
  addDriverHeading: {
    fontSize: "25px  !important",
    fontFamily: "'Lato Bold', 'Inter Bold', 'Arial' !important",
    fontWeight: "600 !important",
    color: "#1E1E1E !important",
  },
  editProductsHeadingAndUpload: {
    // display: "flex  !important",
    justifyContent: "space-between  !important",
    alignItems: "center  !important",
  },
  editProductsHeading: {
    textAlign: "center  !important",
    fontSize: "25px  !important",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    fontWeight: "600 !important",
  },
  addInventoryButton: {
    color: "white !important",
    background:
      "var(--primary-gradient)",
    height: "35px  !important",
    fontSize: "14px",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
  },
  addNewItemsButton: {
    color: "white !important",
    background:
      "var(--primary-gradient)",
    height: "33px  !important",
    fontSize: "13px",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
  },
  addNewItemsButtonOutlined: {
    background: "#F8F8F8 !important",
    border: "1px solid var(--primary-color) !important",
    color: "var(--primary-color) !important",
    height: "33px  !important",
    fontSize: "13px  !important",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
  },

  inventoryMethodHeading: {
    fontSize: "16px !important",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    fontWeight: "200 !important",
  },
  productsHeading: {
    fontSize: "20px !important",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    fontWeight: "700 !important",
  },
  uploadIconArea: {
    width: "90px  !important",
    height: "90px  !important",
    borderRadius: "50%",
    // border:"1px solid lightgrey",
    position: "relative",
  },
  uploadStoreIconArea: {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    marginLeft: "-15px",
  },
  collectPaymentButton: {
    background: "var(--primary-color) !important",
    border: "1px solid var(--primary-color)  !important",
    borderRadius: "10px  !important",
    color: "white !important",
    fontWeight: "500  !important",
    fontSize: "18px  !important",
    textTransform: "capitalize   !important",
    minWidth: "150px  !important",
  },
  placeOrderButton: {
    background: "var(--primary-color) !important",
    border: "1px solid var(--primary-color)  !important",
    borderRadius: "10px  !important",
    color: "white !important",
    fontWeight: "500  !important",
    fontSize: "14px  !important",
    textTransform: "capitalize   !important",
    padding: "3px 8px",
    // minWidth: "150px  !important",
  },
  sendNotButton: {
    background: "var(--primary-color) !important",
    color: "white !important",
    width: "30px !important",
    minWidth: "40px !important",
    height: "30px",
  },
  currentLocationButton: {
    background: "#F8F8F8 !important",
    border: "1px solid var(--primary-color)  !important",
    borderRadius: "10px  !important",
    color: "var(--primary-color)  !important",
    fontWeight: "500  !important",
    fontSize: "15px  !important",
    textTransform: "capitalize   !important",
    marginTop: "20px",
  },
  fromMapButton: {
    height: "37px",
  },
  sendInvoiceButton: {
    background: "#F8F8F8 !important",
    border: "1px solid rgba(0, 0, 0, 0.2)  !important",
    borderRadius: "10px  !important",
    color: "var(--primary-color)  !important",
    fontWeight: "500  !important",
    fontSize: "14px  !important",
    textTransform: "capitalize   !important",
    // minWidth: "150px  !important",
    padding: "3px 8px",
  },
  editProfileIcon: {
    background: "var(--primary-color) !important",
    border: "1px solid white  !important",
    position: "absolute !important",
    right: "10px !important",
    bottom: "0px !important",
    color: "white !important",
    padding: "4px !important",
  },
  driverLoginHeading: {
    fontSize: "20px  !important",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    fontWeight: "500 !important",
    color: "#1E1E1E !important",
  },

  profileSettingArea: {
    background: "rgba(238, 238, 238,.4) !important",
    borderRadius: "8px",
    padding: "15px 15px 25px 15px",
    maxWidth: "386px",
  },
  paymentSettingArea: {
    background: "rgba(238, 238, 238,.4) !important",
    borderRadius: "8px",
    padding: "15px 15px 25px 15px",
    maxWidth: "586px",
  },
  profileSettingHeading: {
    fontSize: "24px !important",
    fontFamily: "Inter Medium !important",
    fontWeight: "500 !important",
    marginBottom: "15px !important",
  },
  paymentMethodText: {
    fontSize: "13px !important",
    fontFamily: "Inter Medium !important",
    fontWeight: "500 !important",
    marginBottom: "15px !important",
    color: "rgba(0, 0, 0, 0.4)  !important",
  },
  paymentMethodHeading: {
    fontSize: "22px !important",
    fontFamily: "Inter Medium !important",
    fontWeight: "500 !important",
  },

  inputLabelBottom: {
    fontSize: "12px  !important",
    color: "rgba(0, 0, 0, 0.4) !important",
    fontFamily: "Inter Medium !important",
    textAlign: "right  !important",
    fontWeight: "500 !important",
    cursor: "pointer",
  },
  saveButtonUI: {
    background: "var(--primary-color) !important",
    borderRadius: "8px !important",
    height: "40px !important",
    fontSize: "18px !important",
    fontFamily: "Inter Medium !important",
    fontWeight: "500 !important",
    textTransform: "capitalize  !important",
  },
  paymentMethodCardArea: {
    padding: "7px  !important",
    boxShadow: "0px 14px 14px -10px rgba(0, 0, 0, 0.25)  !important",
    border: "1px solid rgba(30, 30, 30, 0.2)  !important",
    borderRadius: "8px  !important",
    marginBottom: "15px  !important",
    display: "flex",
    justifyContent: "space-between",
  },
  addAccountButton: {
    background: "var(--primary-color) !important",
    color: "white !important",
    borderRadius: "8px  !important",
    width: "40px  !important",
    minWidth: "40px  !important",
    height: "40px",
  },
  autocompleteFontSize: {
    fontSize: "14px !important",
  },
  addProductButton: {
    background:
      "var(--primary-color)",
    height: "40px  !important",
    fontSize: "15px",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
  },
  forgotPasswordText: {
    fontSize: "14px !important",
    textAlign: "right !important",
    fontWeight: "400 !important",
    color: "black !important",
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    marginTop: "7px !important",
  },

  inputLabelSmallText: {
    fontSize: "13px  !important",
    fontWeight: "400 !important",
    color: "rgba(30, 30, 30, 0.3); !important",
  },
  addProductOrderButton: {
    background: "var(--primary-color) !important",
    color: "white !important",
    borderRadius: "8px  !important",
    width: "45px  !important",
    minWidth: "45px  !important",
    height: "45px",
  },
  searchFieldInput: {
    outline: "none",
    border: "none",
    fontSize: "16px",
    background: "#F8F8F8 !important",
  },
  searchFieldBox: {
    padding: "5px",
    height: "47px",
    width: "92%",
    background: "#F8F8F8 !important",
    borderRadius: "8px  !important",
  },
  searchFieldBoxContent: {
    display: "flex  !important",
    justifyContent: "space-between  !important",
    alignItems: "center  !important",
  },
  productItemBox: {
    display: "flex",
    justifyContent: "flex-start",
    "& img": {
      width: "50px",
      marginRight: "12px",
    },
  },
  productItemBoxHeading: {
    fontFamily: "'Lato', 'Inter', 'Arial' !important",
    fontSize: "16px  !important",
    fontWeight: "600 !important",
  },
  productItemBoxPrice: {
    width: "max-content",
    background: "#00AC6E",
    color: "white",
    borderRadius: "4px",
    fontSize: "12px  !important",
    fontWeight: "400 !important",
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    padding: "1px 5px",
  },
  editProductButton: {
    minWidth: "30px !important",
    width: "30px !important",
    height: 30,
    background: "rgba(0, 123, 255, 0.1) !important",
    border: "1px solid rgba(0, 123, 255, 0.1) !important",
    borderRadius: "4px !important",
    "& .MuiSvgIcon-root": {
      color: "#007bff !important",
    },
  },
  deleteProductButton: {
    minWidth: "30px  !important",
    width: "30px !important",
    height: 30,
    background: "rgba(250, 0, 90, 0.1) !important",
    border: "1px solid rgba(250, 0, 90, 0.1) !important",
    borderRadius: "4px !important",
    "& .MuiSvgIcon-root": {
      color: "#FA005A !important",
    },
  },
  downloadButton: {
    minWidth: "30px !important",
    width: "30px !important",
    height: 30,
    background: "rgba(80, 160, 120, 0.15) !important",
    border: "1px solid rgba(80, 160, 120, 0.15) !important",
    borderRadius: "4px !important",
    "& .MuiSvgIcon-root": {
      color: "#52d479ff  !important",
    },
  },
  restoreButton: {
    minWidth: "30px !important",
    width: "30px !important",
    height: 30,
    background: "rgba(80, 120, 200, 0.15) !important",
    border: "1px solid rgba(80, 120, 200, 0.15) !important",
    borderRadius: "4px !important",
    "& .MuiSvgIcon-root": {
      color: "#4F7CFF !important",
    },
  },

  browseProductButton: {
    background: "rgba(0, 0, 0, 0.1)  !important",
    borderRadius: "6px !important",
    fontSize: "16px !important",
    textTransform: "capitalize  !important",
  },
  createOrderCard: {
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    padding: "15px !important",
    background: "#F8F8F8 !important",
    borderRadius: "px !important",
    marginBottom: "15px",
    "& .MuiSelect-select": {
      display: "flex !important",
    },
  },
  createCarrierReturns: {
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    padding: "15px !important",
    background: "#F8F8F8 !important",
    borderBottom: "none!important",
    borderRadius: "8px 8px 0px 0px !important",
    "& .MuiSelect-select": {
      display: "flex !important",
    },
  },
  uploadOrderCard: {
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    padding: "12px !important",
    background: "#F8F8F8 !important",
    borderRadius: "4px !important",
    "& .MuiSelect-select": {
      display: "flex !important",
    },
  },
  createOrderCardRight: {
    padding: "15px !important",
    background: "#F8F8F8 !important",
    borderRadius: "20px !important",
    marginBottom: "15px",
    "& .MuiSelect-select": {
      display: "flex !important",
    },
    "& .MuiListItemText-primary": {
      fontSize: "18px  !important",
      fontWeight: "500  !important",
      fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    },
  },
  createOrderCardNoteArea: {
    padding: "15px !important",
    borderRadius: "20px !important",
    marginBottom: "15px",
  },
  addStoreButton: {
    background:
      "var(--primary-color)",
    height: "35px  !important",
    fontSize: "15px",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
  },

  deleteStoreButton: {
    background:
      "radial-gradient(100% 5102.04% at 100% 100%, rgba(250, 0, 90) 0%, rgba(250, 0, 90, 0.9) 100%)",
    height: "40px  !important",
    fontSize: "14px",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    textTransform: "capitalize  !important",
    borderRadius: "0px !important",
  },
  modelCancelButton: {
    background: "transparent  !important",
    color: "#1E1E1E !important",
    height: "40px !important",
    fontSize: "15px",
    fontFamily: "'Lato Medium', 'Inter Medium', 'Arial' !important",
    textTransform: "capitalize  !important",
  },
  exportButton: {
    background: "#7BC043",
    height: "50px  !important",
    fontSize: "15px",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
  },

  submitButton: {
    background: "#fb9700",
    height: "50px  !important",
    fontSize: "20px",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
  },
  filterButton: {
    width: "40px !important",
    minWidth: "40px !important",
    height: "40px",
    background: "white  !important ",
    borderRadius: "6px !important",
    border: "1px solid rgba(30, 30, 30, 0.2)  !important",
  },
  importButton: {
    background: "#F8F8F8 !important",
    border: "1px solid var(--primary-color)  !important",
    borderRadius: "10px  !important",
    color: "var(--primary-color)  !important",
    fontWeight: "500  !important",
    fontSize: "14px  !important",
    textTransform: "capitalize   !important",
  },
  orderButton: {
    background:
      "var(--primary-color)",
    borderRadius: "4px  !important",
    fontWeight: "500  !important",
    fontSize: "14px  !important",
    textTransform: "capitalize   !important",
    color: "white  !important",
  },
  disabledButton: {
    background:
      "radial-gradient(100% 5102.04% at 100% 100%, #A6A6A6 0%, #808080 100%)",
    borderRadius: "10px  !important",
    fontWeight: "500  !important",
    fontSize: "14px  !important",
    textTransform: "capitalize   !important",
    color: "white  !important",
  },
  modelOrderDetail: {
    "& .MuiDialog-paper": {
      borderRadius: "20px !important",
    },
    "& .MuiDialogActions-root": {
      padding: "0px  !important",
    },
    "& .MuiButtonBase-root": {
      marginLeft: "0px !important",
    },
  },
  allProductionHeading: {
    textAlign: "center  !important",
    fontSize: "25px  !important",
    fontFamily: "Inter Medium !important",
    fontWeight: "600 !important",
  },
  cardTitleOrder: {
    fontFamily: "Inter Medium !important",
    fontSize: "20px !important",
    fontWeight: "700 !important",
    marginBottom: "-6px !important",
  },
  cardDesOrder: {
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    fontSize: "13px !important",
    fontWeight: "bold !important",
    marginBottom: "0px !important",
    color: "rgba(0, 0, 0, 0.4)  !important",
  },
  NotFoundRoot: {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    minHeight: "300px",
    width: " 100%",
  },
  borderError: {
    borderColor: "#d32f2f",
  },
  NotFoundContent: {
    textAlign: "center",
  },
  generalTabBarArea: {
    background: "#F8F8F8",
    borderRadius: "8px 8px 0px 0px",
    border: "1px solid rgba(224, 224, 224, 1)",
    borderBottom: "none",
    display: "flex",
    justifyContent: "flex-start",
    alignItems: "center",
    padding: " 7px",
    "& .MuiTabs-root": {
      minHeight: "26px !important",
    },
  },
  generalFilterArea: {
    background: "white",
    borderRadius: "8px 8px 0px 0px",
    border: "1px solid rgba(224, 224, 224, 1)",
  },
  TabBarArea: {
    width: "85%",
  },
  circularProgress: {
    "&.MuiCircularProgress-root": {
      width: "15px !important", // Adjust the width as needed
      height: "15px  !important", // Adjust the height as needed
      color: "white",
    },
  },
  circularProgressForTableRow: {
    "&.MuiCircularProgress-root": {
      width: "15px !important", // Adjust the width as needed
      height: "15px  !important", // Adjust the height as needed
    },
  },
  otherIconForTableRow: {
    fontSize: "20px",
  },
  gridActionButtonIcon: {
    fontSize: "12px",
  },
  circularProgress: {
    "&.MuiCircularProgress-root": {
      width: "15px !important", // Adjust the width as needed
      height: "15px  !important", // Adjust the height as needed
      color: "white",
    },
  },
  customTabsUI: {
    height: "26px  !important",

    "& .Mui-selected": {
      color: "var(--primary-color) !important",
      background: "rgb(86, 58, 213, .2)",
      borderRadius: "4px",
      minHeight: "24px !important",
      height: "26px  !important",
    },
    "& .MuiButtonBase-root": {
      textTransform: "capitalize !important",
      minHeight: "24px !important",
      height: "26px  !important",
    },
    "& .MuiTabs-scroller": {
      height: "26px  !important",
    },
    "& .MuiTabs-indicator": {
      display: "none !important",
    },
    "&.MuiTabs-scrollButtons.Mui-disabled": {
      opacity: 0.3,
    },
  },
  filterIcon: {
    border: " 1px solid rgba(30, 30, 30, 0.2)   !important",
    textTransform: "capitalize  !important",
    height: "29px",
    fontSize: "12px",
    padding: "5px 5px",
  },
  // filterButtonMargin: {
  //   marginTop: "24px !important",
  // },
  filterIconColord: {
    color: "#fff",
    border: "1px solid rgba(30, 30, 30, 0.2) !important",
    textTransform: "capitalize !important",
    height: "29px",
    fontSize: "12px",
    background:
      "var(--primary-color)",
    "&:disabled": {
      color: "gray", // Change the color for disabled state
      background: "rgba(30, 30, 30, 0.2) !important", // Change the background for disabled state
      border: "1px solid rgba(30, 30, 30, 0.2) !important",
    },
  },
  filterIconDeleteColord: {
    color: "#fff",
    border: " 1px solid rgba(30, 30, 30, 0.2)   !important",
    textTransform: "capitalize  !important",
    height: "29px",
    fontSize: "12px",
    background:
      "radial-gradient(100% 5102.04% at 100% 100%, #dc3545 0%, #dc3545 100%)",
    "&:disabled": {
      color: "gray", // Change the color for disabled state
      background: "rgba(30, 30, 30, 0.2) !important", // Change the background for disabled state
      border: "1px solid rgba(30, 30, 30, 0.2) !important",
    },
  },
  LoadingButtonDelete: {
    color: "#fff !important",
    border: " 1px solid rgba(30, 30, 30, 0.2)   !important",
    textTransform: "capitalize  !important",
    height: "29px",
    width: "15px !important",
    fontSize: "12px",
    background:
      "radial-gradient(100% 5102.04% at 100% 100%, #dc3545 0%, #dc3545 100%)",
    "&:disabled": {
      color: "gray", // Change the color for disabled state
      background: "rgba(30, 30, 30, 0.2) !important",
      border: "1px solid rgba(30, 30, 30, 0.2) !important",
    },
  },
  filterAndSearchArea: {
    width: "15%",
  },
  sideBarRoot: {
    screenY: "auto",
    height: "100vh",
    background: "#EEEEEE",
  },
  sideBarListText: {
    "& .MuiListItemText-primary": {
      fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
      fontSize: "14px  !important",
      fontWeight: "600  !important",
    },
    fontSize: "14px  !important",
    fontWeight: "500  !important",
  },
  sideBarListTextSub: {
    fontSize: "14px  !important",
    fontWeight: "500  !important",
    "& .MuiListItemText-primary": {
      fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
      fontSize: "14px  !important",
      fontWeight: "600  !important",
    },
  },
  userArea: {
    position: "relative",
  },
  userMenuArea: {
    width: "100%",
    cursor: "pointer",
    backgroundColor: "Transparent  !important",
  },
  userMenuAreaPcClose: {
    width: "23%",
    backgroundColor: "Transparent  !important",
    border: "0px !important",
  },
  userMenuHeading: {
    width: "10%",
    "& .MuiCardHeader-title": {
      fontWeight: "600",
      fontSize: "14px  !important",
      fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    },
    "& .MuiCardHeader-subheader": {
      fontWeight: "00",
      fontSize: "14px !important",
      fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    },
  },
  navCollapseItems: {},
  sideBarRootList: {
    "&::-webkit-scrollbar-thumb": {
      background: "var(--primary-color)",
    },
    "&::-webkit-scrollbar-track": {
      background: "#ccc",
    },
    "&::-webkit-scrollbar": {
      width: "6px",
    },
    "& .Mui-selected": {
      backgroundColor: "rgba(86, 58, 213, 0.08) !important",
    },
  },
  topNavBar: {
    display: "flex",
    justifyContent: "space-between !important",
    alignItems: "center",
    padding: "0px",
    width: "100%",
  },
  topNavBarLeft: {
    fontSize: "18px !important",
    fontWeight: "600 !important",
    textTransform: "capitalize",
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
  },
  headingLeft: {
    fontSize: "16px !important",
    marginBottom: "16px !important",
    fontWeight: "600 !important",
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    textTransform: "capitalize",
  },
  orderProductHeading: {
    fontSize: "12px !important",
  },
  selectionCheckBoxSize: {
    fontSize: "18px !important",
  },

  languageDropdownTitle: {
    fontSize: "14px",
    fontFamily: "Inter Medium !important",
  },
  languageDropdownBox: {
    padding: "5px",
    border: "1px solid lightgrey",
    borderRadius: "5px",
    overflow: "hidden !important",
  },
  UnauthorizedPage: {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    height: "90vh",
  },
  UnauthorizedPageHeading: {
    fontSize: "20px",
    fontWeight: "600",
    margin: 0,
    textAlign: "center",
  },
  pageHeading: {
    fontSize: "28px  !important",
    fontWeight: "600  !important",
    fontFamily: "'Lato', 'Inter', 'Arial' !important",
  },
  pageHeadingRight: {
    fontSize: "28px  !important",
    fontWeight: "600  !important",
    fontFamily: "'Lato', 'Inter', 'Arial' !important",
    textAlign: "center !important",
  },
  pageSubHeading: {
    fontSize: "18px  !important",
    fontWeight: "600  !important",
    fontFamily: "'Lato', 'Inter', 'Arial' !important",
  },
  UnauthorizedPageContent: {
    margin: 0,
    textAlign: "center",
  },
  integrationListArea: {},
  integrationCard: {
    borderRadius: "18px !important",
    backgroundPosition: "left bottom",
    backgroundRepeat: "no-repeat",
    backgroundColor: "#fff !important",
    padding: "5px 20px 20px 20px",
  },
  integrationLogoArea: {
    display: "flex",
    justifyContent: "center",
    marginTop: "25px",
    "& img": {
      width: "130px",
      marginBottom: "15px",
    },
  },
  integrationContent: {
    maxWidth: "28ch",
  },
  integrationActiveUser: {
    color: "#402c99",
  },
  integrationCardHeading: {
    fontSize: "20px !important",
    textAlign: "center !important",
    fontWeight: "700 !important",
    fontFamily: "Inter Medium !important",
  },
  integrationCardDes: {
    fontSize: "14px !important",
    textAlign: "center !important",
    fontWeight: "400 !important",
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    marginTop: "7px !important",
  },
  integrationCardText: {
    fontSize: "14px !important",
    fontWeight: "400 !important",
    textAlign: "center",
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    margin: "0px 14px",
  },
  integrationCardDesRight: {
    fontSize: "14px !important",
    textAlign: "right !important",
    fontWeight: "400 !important",
    fontFamily: "'Lato Regular', 'Inter Regular', 'Arial' !important",
    marginTop: "7px !important",
  },

  tagsChipStyle: {
    background: "var(--primary-color) !important",
    color: "white !important",
  },
  integrationCardAction: {
    display: "flex",
    justifyContent: "center",
  },
  integrationactivatedButton: {
    background: "var(--primary-color) !important",
    borderRadius: " 6px !important",
    color: "white !important",
    width: "100%",
    fontSize: "12px !important",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
  },

  integrationDeactiveButton: {
    background: "#dc3545 !important",
    borderRadius: " 6px !important",
    color: "white !important",
    fontSize: "12px !important",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
    padding: "5px",
  },
  qualityIncrementBox: {
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
    width: 80,
    height: 30,
    border: "1px solid rgba(0, 0, 0, 0.4) !important",
    borderRadius: "4px !important",
    background: "#F8F8F8 !important",
    "& .MuiSvgIcon-root": {
      cursor: "poLato",
    },
  },
  inOperationButton: {
    background: "#6DC2FF !important",
    textTransform: "capitalize !important",
  },
  dangerButton: {
    background: "#dc3545 !important",
    textTransform: "capitalize !important",
  },
  tableHeading: {
    fontSize: "16px  !important",
    fontFamily: "Inter Medium !important",
    fontWeight: "500 !important",
    color: "rgba(0, 0, 0, 0.5)!important",
  },
  linkColor: {
    color: "#0070E0 !important",
  },
  orderInfoModelButtn: {
    height: "35px",
  },
  orderInfoModelLoadingText: {
    color: "var(--primary-color)",
  },
  tabInputStyle: {
    "& .MuiInputBase-input": {
      padding: "2px 14px !important",
    },
    "& .MuiOutlinedInput-notchedOutline": {
      border: "unset !important",
    },
  },
  timelineDot: {
    backgroundColor: "var(--primary-color)",
  },

  viewBox: {
    width: "250px",
    height: "250px",
    alignItems: "center",
    justifyContent: "center",
    border: "1px solid #d3d3d391",
    borderRadius: "15px",
  },
  viewMianBox: {
    display: "flex",
    flexDirection: "column",
    gap: "15px",
    paddingLeft: "30px",
  },
  viewCarrierServices: {
    paddingLeft: "20px",
    display: "flex",
    marginTop: "20px",
    gap: "20px",
  },
  viewCarrierServicesimg: {
    width: "50px",
    height: "50px",
  },
  filterBtn: {
    flexBasis: "49.5%!important",
    maxWidth: "48.5%!important",
  },
  searchIcon: {
    cursor: "pointer",
    transition: "color 0.3s ease",
    "&:hover": {
      color: "blue",
    },
  },
  permissionAccordion: {
    margin: "0",
    padding: "0",
    minHeight: "30px !important",
    "& .MuiAccordionSummary-content": {
      padding: "0",
      margin: "10px",
    },
    "& .MuiAccordionSummary-root.Mui-expande": {
      padding: "0",
      margin: "0",
      minHeight: "30px",
    },
    "& .MuiAccordionSummary-content.Mui-expanded": {
      padding: "0",
      margin: "10px",
    },
  },
  permissionAccordionDetail: {
    padding: "0px 35px 10px!important",
    "& .MuiFormControlLabel-root": {
      marginBottom: "8px",
    },
  },
  statusHeading: {
    fontWeight: "400 !important",
    textTransform: "uppercase !important",
    padding: "3px 12px",
  },
  podFilesImgBox: {
    maxHeight: "250px",
    height: "250px",
    boxShadow: "0px 0px 1px #1b1f21",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    padding: "30px",
    flexDirection: "column",
    textAlign: "center",
    "&:hover": {
      boxShadow: "0px 0px 5px #a4a5a5",
      cursor: "pointer",
    },
  },
  podFilesImg: {
    width: "100%",
    height: "100%",
    objectFit: "contain",
  },
  podFilesIcon: {
    fontSize: "75px",
    color: "indianred",
  },
  SearchBtn: {
    border: " 1px solid rgba(30, 30, 30, 0.2)   !important",
    textTransform: "capitalize  !important",
    height: "38px",
    fontSize: "12px",
  },
  integrationConfigBtn: {
    borderRadius: " 6px !important",
    color: "white !important",
    fontSize: "12px !important",
    fontFamily: "Inter Medium !important",
    textTransform: "capitalize  !important",
    padding: "5px",
  },
  notificationGreyBox: {
    borderRadius: "5px !important",
    backgroundPosition: "left bottom",
    backgroundRepeat: "no-repeat",
    padding: "12px",
  },
};
