const initialStateFilter = {
  orderType: {
    orderTypeId: 0,
    orderTypeName: "Select Please",
  },
  carrier: {
    activeCarrierId: 0,
    name: "Select Please",
  },
  store: {
    storeId: 0,
    storeName: "Select Please",
  },
  paymentMethod: {
    paymentMethodId: 0,
    pmName: "Select Please",
  },
  station: {
    productStationId: 0,
    sname: "Select Please",
  },
  stores: [],
  multiple: [],
  common: {
    id: 0,
    text: "Select Please",
  },
  fulfillment: {
    id: 0,
    text: "Select Please",
  },
  paymentStatus: {
    id: 0,
    text: "Select Please",
  },
};

export default initialStateFilter;
