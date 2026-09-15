import FilterAltOutlinedIcon from "@mui/icons-material/FilterAltOutlined";
import { Box, Button, Grid, InputLabel, Typography } from "@mui/material";
import React, { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import DataGridComponent from "../../.reUseableComponents/DataGrid/DataGridComponent";
import {
  GetActiveCarriers,
  GetAllProductVariantsCount,
  GetCarrierActivityWithDetail,
  GetCarrierStats,
  GetDelieveredOrderCount,
  GetDeliveryRatioCount,
  GetFulFillableOrderCount,
  GetInProgressOrderCount,
  GetLowStockItemsCount,
  GetProductCounts,
  GetRegularOrderCount,
  GetReturnedOrderCount,
  GetToBePackedCount,
  GetToBeShippedCount,
  GetTopSellingItemsCount,
  GetTotalCarriers,
  GetTotalCollected,
  GetTotalCompletedOrderCountWithCarrier,
  GetTotalInprogressOrderCountWithCarrier,
  GetTotalNoofOrdersPlacedCounts,
  GetTotalPurchaseStockValue,
  GetTotalStockValue,
  GetTotalStoreCount,
  GetTotalUncollected,
} from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import {
  ClipboardIcon,
  CodeBox,
  DataGridHeaderBox,
  FilterIconButton,
  GridContainer,
  GridItem,
  MenuComponent,
  PageMainBox,
  ScreenToggleButton,
  amountFormat,
  checkObjHasSpecificProperty,
  decimalFormat,
  fetchMethod,
  getSpacesLabelFromCamelCase,
  graphColorsArr,
  navbarHeight,
  pageMainBoxPadding,
  textTrucate,
  useClientSubscriptionReducer,
  useGetBreakPoint,
  useGetWindowHeight,
  useMenu,
} from "../../utilities/helpers/Helpers";
// import { DatePicker } from "antd";
import CustomReactDatePickerInput from "../../.reUseableComponents/TextField/CustomReactDatePickerInput";
import activeCariiers from "../../assets/images/AlisImages/analytics/Carriers Activity/Active.svg";
import completedCarriers from "../../assets/images/AlisImages/analytics/Carriers Activity/Completed.svg";
import inProgressCarriers from "../../assets/images/AlisImages/analytics/Carriers Activity/In Progress.svg";
import totalCarriers from "../../assets/images/AlisImages/analytics/Carriers Activity/Total.svg";
import financeCOD from "../../assets/images/AlisImages/analytics/Finance/Frame 1000009461.svg";
import financePaid from "../../assets/images/AlisImages/analytics/Finance/Frame 1000009462.svg";
import financeStockValue from "../../assets/images/AlisImages/analytics/Finance/Frame 1000009463.svg";
import financeStockValue2 from "../../assets/images/AlisImages/analytics/Finance/Frame 1000009464.svg";
import ordersTotal from "../../assets/images/AlisImages/analytics/Orders Activity/Frame 1000009465.svg";
import ordersRegular from "../../assets/images/AlisImages/analytics/Orders Activity/Frame 1000009466.svg";
import ordersFulfillable from "../../assets/images/AlisImages/analytics/Orders Activity/Frame 1000009467.svg";
import ordersDelivered from "../../assets/images/AlisImages/analytics/Orders Activity/Frame 1000009468.svg";
import ordersToBeShipped from "../../assets/images/AlisImages/analytics/Orders Activity/Frame 1000009469.svg";
import ordersToBePacked from "../../assets/images/AlisImages/analytics/Orders Activity/Frame 1000009470.svg";
import storeInSales from "../../assets/images/AlisImages/analytics/Sales Activity/Frame 1000009471.svg";
import products from "../../assets/images/AlisImages/analytics/Sales Activity/Frame 1000009472.svg";
import totalProductsSKUs from "../../assets/images/AlisImages/analytics/Sales Activity/Frame 1000009473.svg";
import activeCarriersPartner from "../../assets/images/AlisImages/analytics/Sales Activity/Frame 1000009474.svg";
import "../../assets/styles/datePickerCustomStyles.css";
import UtilityClass from "../../utilities/UtilityClass";
import AnalyticsCard from "./cads/AnalyticsCard";
import AnalyticsMainCard from "./cads/AnalyticsMainCard";

import { startOfMonth } from "date-fns";
import ReactApexChart from "react-apexcharts";
import { DragDropContext, Draggable, Droppable } from "react-beautiful-dnd";
import { FullScreen, useFullScreenHandle } from "react-full-screen";
import ButtonComponent from "../../.reUseableComponents/Buttons/ButtonComponent";
import { getThisKeyCookie } from "../../utilities/cookies";
import GraphDateRange from "./GraphDateRange";
import { sampleCarrierStat } from "./SampleData";

const sampleCountData = {
  totalCarriers: { loading: false, count: 100 },
  totalProduct: { loading: false, count: 100 },
  totalProductVariants: { loading: false, count: 100 },
  totalNoofOrdersPlaced: { loading: false, count: 100 },
  totalToBeShipped: { loading: false, count: 10 },
  totalToBePacked: { loading: false, count: 10 },
  totalTopSellingItems: { loading: false, count: 100 },
  totalLowStockItems: { loading: false, count: 100 },
  totalDeliveryRatio: { loading: false, count: 100 },
  totalActiveCarriers: { loading: false, count: 50 },
  totalUncollectedFinance: { loading: false, count: 100 },
  totalStockValue: { loading: false, count: "1000.00" },
  totalPurchaseValue: { loading: false, count: "1000.00" },
  totalCollectedFinance: { loading: false, count: 100 },
  totalReturnedOrderCount: { loading: false, count: 100 },
  totalInProgressOrderCount: { loading: false, count: 100 },
  totalRegularOrderCount: { loading: false, count: 50 },
  totalFulFillableOrderCount: { loading: false, count: 50 },
  totalDelieveredOrderCount: { loading: false, count: 80 },
  totalStoreCount: { loading: false, count: 100 },
  totalCompletedOrderCountWithCarrier: { loading: false, count: 40 },
  totalInprogressOrderCountWithCarrier: { loading: false, count: 10 },
};

const barGraphData = {
  series: [],
  options: {
    chart: {
      type: "bar",
      stacked: true,
    },
    yaxis: {
      show: false,
    },
    xaxis: {
      show: false,
    },
    tooltip: {
      y: {
        formatter: function (val) {
          return val;
        },
      },
    },
    grid: {
      yaxis: {
        lines: {
          show: false,
        },
      },
      xaxis: {
        lines: {
          show: false,
        },
      },
    },
    plotOptions: {
      bar: {
        columnWidth: "90%",
      },
    },
    colors: graphColorsArr,
  },
};

const pieGrapgData = {
  series: [],
  options: {
    chart: {
      width: 380,
      type: "pie",
    },
    labels: [],
    responsive: [
      {
        breakpoint: 480,
        options: {
          chart: {
            width: 200,
          },
          legend: {
            position: "bottom",
            horizontalAlign: "center",
          },
        },
      },
    ],
    dataLabels: {
      enabled: true,
      formatter: function (percentage, val) {
        const statusesValueSum = val.w.globals.initialSeries.reduce(
          (acc, val) => acc + val,
          0
        );
        const valOfPercentage = (percentage * statusesValueSum) / 100;
        return `${decimalFormat(valOfPercentage)} (${percentage}%)`;
      },
    },
    plotOptions: {
      pie: {
        dataLabels: {
          offset: -20,
        },
      },
    },

    colors: graphColorsArr,
  },
};
function AnalyticsPage() {
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  const belowMdScreen = useGetBreakPoint();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const userName = getThisKeyCookie("user_name");
  const [filterModal, setFilterModal] = useState({
    createdFrom: null,
    createdTo: null,
  });
  const [graphFilterModal, setGraphFilterModal] = useState({
    createdFrom: startOfMonth(new Date()),
    createdTo: new Date(),
  });

  const [sampleLoaded, setSampleLoaded] = useState(false);

  const [count, setCount] = useState({
    totalCarriers: { loading: false, count: 0 },
    totalProduct: { loading: false, count: 0 },
    totalProductVariants: { loading: false, count: 0 },
    totalNoofOrdersPlaced: { loading: false, count: 0 },
    totalToBeShipped: { loading: false, count: 0 },
    totalToBePacked: { loading: false, count: 0 },
    totalTopSellingItems: { loading: false, count: 0 },
    totalLowStockItems: { loading: false, count: 0 },
    totalDeliveryRatio: { loading: false, count: 0 },
    totalActiveCarriers: { loading: false, count: 0 },
    totalUncollectedFinance: { loading: false, count: 0 },
    totalStockValue: { loading: false, count: 0 },
    totalPurchaseValue: { loading: false, count: 0 },
    totalCollectedFinance: { loading: false, count: 0 },
    totalCollectedFinance: { loading: false, count: 0 },
    totalCollectedFinance: { loading: false, count: 0 },
    totalReturnedOrderCount: { loading: false, count: 0 },
    totalInProgressOrderCount: { loading: false, count: 0 },
    totalRegularOrderCount: { loading: false, count: 0 },
    totalFulFillableOrderCount: { loading: false, count: 0 },
    totalDelieveredOrderCount: { loading: false, count: 0 },
    totalStoreCount: { loading: false, count: 0 },
    totalCompletedOrderCountWithCarrier: { loading: false, count: 0 },
    totalInprogressOrderCountWithCarrier: { loading: false, count: 0 },
  });
  const [dataTable, setDataTable] = useState({
    carrierActivityWithDetail: { loading: false, data: [] },
    salesActivityWithDetail: { loading: false, data: [] },
  });
  const [graphData, setGraphData] = useState({
    carriesStats: {
      series: [],
      dataset: [],
      rawData: [],
      uniqueDates: [],
    },
    carriesStatsNew: {
      series: [],
      companies: [],
      statuses: [],
      barType: true,
    },
  });
  const [cardData, setCardData] = useState([
    {
      id: LanguageReducer?.languageType?.ANALYTICS_CARRIERS_STATS,
      md: 7,
      height: "45%",
    },
    {
      id: LanguageReducer?.languageType?.ANALYTICS_CARRIERS_ACTIVITY,
      md: 5,
      height: "45%",
    },
    {
      id: LanguageReducer?.languageType?.ANALYTICS_SALES_ACTIVITY,
      md: 7,
      height: "55%",
    },
    {
      id: LanguageReducer?.languageType?.ANALYTICS_FINANCE,
      md: 5,
      height: "55%",
    },
  ]);
  const [isFilterClear, setIsFilterClear] = useState(false);
  const clientSubscriptionData = useClientSubscriptionReducer();
  const { height } = useGetWindowHeight(clientSubscriptionData);
  const { anchorEl, open, handleOpen, handleClose } = useMenu();
  const handle = useFullScreenHandle();
  const heightBelow650 = height <= 650;
  const heightBelow500 = height < 499;
  const gridContainerHeight = height - navbarHeight - pageMainBoxPadding - 54;

  // handleGetCarrierActivityWithDetail
  const handleGetCarrierActivityWithDetail = async () => {
    setDataTable((prev) => ({
      ...prev,
      carrierActivityWithDetail: {
        loading: true,
        data: prev.carrierActivityWithDetail.data,
      },
    }));
    await GetCarrierActivityWithDetail(filterModal)
      .then((res) => {
        const response = res.data;

        setDataTable((prev) => ({
          ...prev,
          carrierActivityWithDetail: {
            loading: prev.carrierActivityWithDetail.loading,
            data: response.result?.map((dt, index) => ({ id: index, ...dt })),
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setDataTable((prev) => ({
          ...prev,
          carrierActivityWithDetail: {
            loading: false,
            data: prev.carrierActivityWithDetail.data,
          },
        }));
      });
  };
  // handleGetTotalInprogressOrderCountWithCarrier
  const handleGetTotalInprogressOrderCountWithCarrier = async () => {
    setCount((prev) => ({
      ...prev,
      totalInprogressOrderCountWithCarrier: {
        loading: true,
        count: prev.totalInprogressOrderCountWithCarrier.TotalCount,
      },
    }));
    await GetTotalInprogressOrderCountWithCarrier(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalInprogressOrderCountWithCarrier: {
            loading: prev.totalInprogressOrderCountWithCarrier.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalInprogressOrderCountWithCarrier: {
            loading: false,
            count: prev.totalInprogressOrderCountWithCarrier.count,
          },
        }));
      });
  };
  // handleGetTotalCompletedOrderCountWithCarrier
  const handleGetTotalCompletedOrderCountWithCarrier = async () => {
    setCount((prev) => ({
      ...prev,
      totalCompletedOrderCountWithCarrier: {
        loading: true,
        count: prev.totalCompletedOrderCountWithCarrier.TotalCount,
      },
    }));
    await GetTotalCompletedOrderCountWithCarrier(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalCompletedOrderCountWithCarrier: {
            loading: prev.totalCompletedOrderCountWithCarrier.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalCompletedOrderCountWithCarrier: {
            loading: false,
            count: prev.totalCompletedOrderCountWithCarrier.count,
          },
        }));
      });
  };
  // handleGetTotalStoreCount
  const handleGetTotalStoreCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalStoreCount: {
        loading: true,
        count: prev.totalStoreCount.TotalCount,
      },
    }));
    await GetTotalStoreCount(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalStoreCount: {
            loading: prev.totalStoreCount.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalStoreCount: {
            loading: false,
            count: prev.totalStoreCount.count,
          },
        }));
      });
  };
  // handleGetDelieveredOrderCount
  const handleGetDelieveredOrderCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalDelieveredOrderCount: {
        loading: true,
        count: prev.totalDelieveredOrderCount.TotalCount,
      },
    }));
    await GetDelieveredOrderCount(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalDelieveredOrderCount: {
            loading: prev.totalDelieveredOrderCount.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalDelieveredOrderCount: {
            loading: false,
            count: prev.totalDelieveredOrderCount.count,
          },
        }));
      });
  };
  // handleGetFulFillableOrderCount
  const handleGetFulFillableOrderCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalFulFillableOrderCount: {
        loading: true,
        count: prev.totalFulFillableOrderCount.TotalCount,
      },
    }));
    await GetFulFillableOrderCount(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalFulFillableOrderCount: {
            loading: prev.totalFulFillableOrderCount.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalFulFillableOrderCount: {
            loading: false,
            count: prev.totalFulFillableOrderCount.count,
          },
        }));
      });
  };
  // handleGetRegularOrderCount
  const handleGetRegularOrderCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalRegularOrderCount: {
        loading: true,
        count: prev.totalRegularOrderCount.TotalCount,
      },
    }));
    await GetRegularOrderCount(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalRegularOrderCount: {
            loading: prev.totalRegularOrderCount.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalRegularOrderCount: {
            loading: false,
            count: prev.totalRegularOrderCount.count,
          },
        }));
      });
  };
  // handleGetInProgressOrderCount
  const handleGetInProgressOrderCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalInProgressOrderCount: {
        loading: true,
        count: prev.totalInProgressOrderCount.TotalCount,
      },
    }));
    await GetInProgressOrderCount(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalInProgressOrderCount: {
            loading: prev.totalInProgressOrderCount.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalInProgressOrderCount: {
            loading: false,
            count: prev.totalInProgressOrderCount.count,
          },
        }));
      });
  };
  // handleGetReturnedOrderCount
  const handleGetReturnedOrderCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalReturnedOrderCount: {
        loading: true,
        count: prev.totalReturnedOrderCount.TotalCount,
      },
    }));
    await GetReturnedOrderCount(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalReturnedOrderCount: {
            loading: prev.totalReturnedOrderCount.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalReturnedOrderCount: {
            loading: false,
            count: prev.totalReturnedOrderCount.count,
          },
        }));
      });
  };
  // handleGetTotalCollected
  const handleGetTotalCollected = async () => {
    setCount((prev) => ({
      ...prev,
      totalCollectedFinance: {
        loading: true,
        count: prev.totalCollectedFinance.TotalCount,
      },
    }));
    await GetTotalCollected(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalCollectedFinance: {
            loading: prev.totalCollectedFinance.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalCollectedFinance: {
            loading: false,
            count: prev.totalCollectedFinance.count,
          },
        }));
      });
  };
  // handleGetTotalStockValue
  const handleGetTotalStockValue = async () => {
    setCount((prev) => ({
      ...prev,
      totalStockValue: {
        loading: true,
        count: prev.totalStockValue.TotalCount,
      },
    }));
    await GetTotalStockValue(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalStockValue: {
            loading: prev.totalStockValue.loading,
            count: amountFormat(response.result?.TotalCount),
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalStockValue: {
            loading: false,
            count: prev.totalStockValue.count,
          },
        }));
      });
  };
  // handleGetTotalPurchaseStockValue
  const handleGetTotalPurchaseStockValue = async () => {
    setCount((prev) => ({
      ...prev,
      totalPurchaseValue: {
        loading: true,
        count: prev.totalPurchaseValue.TotalCount,
      },
    }));
    await GetTotalPurchaseStockValue(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalPurchaseValue: {
            loading: prev.totalPurchaseValue.loading,
            count: amountFormat(response.result?.TotalCount),
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalPurchaseValue: {
            loading: false,
            count: prev.totalPurchaseValue.count,
          },
        }));
      });
  };
  // handleGetTotalUncollected
  const handleGetTotalUncollected = async () => {
    setCount((prev) => ({
      ...prev,
      totalUncollectedFinance: {
        loading: true,
        count: prev.totalUncollectedFinance.TotalCount,
      },
    }));
    await GetTotalUncollected(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalUncollectedFinance: {
            loading: prev.totalUncollectedFinance.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalUncollectedFinance: {
            loading: false,
            count: prev.totalUncollectedFinance.count,
          },
        }));
      });
  };
  // handleGetTotalCarriers
  const handleGetTotalCarriers = async () => {
    setCount((prev) => ({
      ...prev,
      totalCarriers: {
        loading: true,
        count: prev.totalCarriers.count,
      },
    }));
    await GetTotalCarriers(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalCarriers: {
            loading: prev.totalCarriers.loading,
            count: response.result?.count,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalCarriers: {
            loading: false,
            count: prev.totalCarriers.count,
          },
        }));
      });
  };
  // handleGetActiveCarriers
  const handleGetActiveCarriers = async () => {
    setCount((prev) => ({
      ...prev,
      totalActiveCarriers: {
        loading: true,
        count: prev.totalCarriers.count,
      },
    }));
    await GetActiveCarriers(filterModal)
      .then((res) => {
        const response = res.data;

        setCount((prev) => ({
          ...prev,
          totalActiveCarriers: {
            loading: prev.totalActiveCarriers.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalActiveCarriers: {
            loading: false,
            count: prev.totalActiveCarriers.count,
          },
        }));
      });
  };
  // handleGetProductCounts
  const handleGetProductCounts = async () => {
    setCount((prev) => ({
      ...prev,
      totalProduct: {
        loading: true,
        count: prev.totalCarriers.count,
      },
    }));
    await GetProductCounts(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalProduct: {
            loading: prev.totalProduct.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalProduct: {
            loading: false,
            count: prev.totalProduct.count,
          },
        }));
      });
  };
  // handleGetAllProductVariantsCount
  const handleGetAllProductVariantsCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalProductVariants: {
        loading: true,
        count: prev.totalProductVariants.count,
      },
    }));
    await GetAllProductVariantsCount(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalProductVariants: {
            loading: prev.totalProductVariants.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalProductVariants: {
            loading: false,
            count: prev.totalProductVariants.count,
          },
        }));
      });
  };
  // handleGetTotalNoofOrdersPlacedCounts
  const handleGetTotalNoofOrdersPlacedCounts = async () => {
    setCount((prev) => ({
      ...prev,
      totalNoofOrdersPlaced: {
        loading: true,
        count: prev.totalNoofOrdersPlaced.count,
      },
    }));
    await GetTotalNoofOrdersPlacedCounts(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalNoofOrdersPlaced: {
            loading: prev.totalNoofOrdersPlaced.loading,
            count: response.result?.count,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalNoofOrdersPlaced: {
            loading: false,
            count: prev.totalNoofOrdersPlaced.count,
          },
        }));
      });
  };
  // handleGetToBeShippedCount
  const handleGetToBeShippedCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalToBeShipped: {
        loading: true,
        count: prev.totalToBeShipped.count,
      },
    }));
    await GetToBeShippedCount(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalToBeShipped: {
            loading: prev.totalToBeShipped.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalToBeShipped: {
            loading: false,
            count: prev.totalToBeShipped.count,
          },
        }));
      });
  };
  // handleGetToBePackedCount
  const handleGetToBePackedCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalToBePacked: {
        loading: true,
        count: prev.totalToBePacked.count,
      },
    }));
    await GetToBePackedCount(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalToBePacked: {
            loading: prev.totalToBePacked.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalToBePacked: {
            loading: false,
            count: prev.totalToBePacked.count,
          },
        }));
      });
  };
  // handleGetTopSellingItemsCount
  const handleGetTopSellingItemsCount = async () => {
    setDataTable((prev) => ({
      ...prev,
      salesActivityWithDetail: {
        loading: true,
        data: prev.salesActivityWithDetail.data,
      },
    }));
    await GetTopSellingItemsCount(filterModal)
      .then((res) => {
        const response = res.data;
        setDataTable((prev) => ({
          ...prev,
          salesActivityWithDetail: {
            loading: prev.salesActivityWithDetail.loading,
            data: response.result?.map((dt, index) => ({ id: index, ...dt })),
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setDataTable((prev) => ({
          ...prev,
          salesActivityWithDetail: {
            loading: false,
            data: prev.salesActivityWithDetail.data,
          },
        }));
      });
  };
  // handleGetLowStockItemsCount
  const handleGetLowStockItemsCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalLowStockItems: {
        loading: true,
        count: prev.totalLowStockItems.count,
      },
    }));
    await GetLowStockItemsCount(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalLowStockItems: {
            loading: prev.totalLowStockItems.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalLowStockItems: {
            loading: false,
            count: prev.totalLowStockItems.count,
          },
        }));
      });
  };
  // handleGetDeliveryRatioCount
  const handleGetDeliveryRatioCount = async () => {
    setCount((prev) => ({
      ...prev,
      totalDeliveryRatio: {
        loading: true,
        count: prev.totalDeliveryRatio.count,
      },
    }));
    await GetDeliveryRatioCount(filterModal)
      .then((res) => {
        const response = res.data;
        setCount((prev) => ({
          ...prev,
          totalDeliveryRatio: {
            loading: prev.totalDeliveryRatio.loading,
            count: response.result?.TotalCount,
          },
        }));
      })
      .catch((err) => console.log(err))
      .finally(() => {
        setCount((prev) => ({
          ...prev,
          totalDeliveryRatio: {
            loading: false,
            count: prev.totalDeliveryRatio.count,
          },
        }));
      });
  };
  const handleManuplateCarrierStat = (data = []) => {
    const responseData = data;
    // const responseData = result.slice(0, 1);
    const responseObj = responseData[0];
    const isBarType = responseData.length > 1;
    const companies = responseData
      .map((dt) => {
        return dt.carrierName;
      })
      .filter(Boolean);
    const statusesKeysForBarChart = Object.entries(responseObj)
      .map(([key, val]) => {
        if (
          key !== "carrierId" &&
          key !== "carrierName" &&
          key !== "TotalCount"
        ) {
          return key;
        }
      })
      .filter(Boolean);
    const statusesKeysForPieChart = Object.entries(responseObj)
      .map(([key, val]) => {
        if (
          key !== "carrierId" &&
          key !== "carrierName" &&
          key !== "TotalCount" &&
          responseObj[key]
        ) {
          return key;
        }
      })
      .filter(Boolean);

    let series;
    if (isBarType) {
      series = statusesKeysForBarChart.map((status) => {
        const _data = responseData.map((dt) => {
          if (checkObjHasSpecificProperty(dt, status)) {
            return dt[status];
          }
          return null;
        });

        return {
          name: getSpacesLabelFromCamelCase(status),
          data: _data,
        };
      });
    } else {
      series = statusesKeysForPieChart
        .map((status) => {
          const value = responseObj[status];
          return value;
        })
        .filter(Boolean);
    }
    if (isBarType) {
      series = series.filter((item) =>
        item.data.some((value) => value !== null)
      );
    }
    const statuses = (
      isBarType ? statusesKeysForBarChart : statusesKeysForPieChart
    ).map((status) => getSpacesLabelFromCamelCase(status));
    const carrierTotals = isBarType
      ? companies.map((_, index) => {
          return series.reduce((sum, s) => {
            const value = s.data[index];
            return sum + (value || 0);
          }, 0);
        })
      : [];
    setGraphData((prev) => ({
      ...prev,
      carriesStatsNew: {
        series,
        companies,
        statuses,
        barType: isBarType,
        carrierTotals,
      },
    }));
  };
  console.log(graphData);
  const handleGetCarrierStatsNew = async (sampleData) => {
    const { response } = await fetchMethod(() =>
      GetCarrierStats(graphFilterModal)
    );
    if (response.isSuccess && response.result?.length > 0) {
      handleManuplateCarrierStat(response.result);
    }
  };

  const productColumns = [
    {
      field: "Name",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.ANALYTICS_NAME}
        />
      ),
      minWidth: 200,
      flex: 1,
      renderCell: (params) => {
        return <Box>{params.row.ProductName || "-"}</Box>;
      },
    },
    {
      field: "SKU",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.ANALYTICS_SKU}
        />
      ),
      minWidth: 200,
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => {
        return (
          <>
            <CodeBox title={params.row.SKU} hasColor={false} />
            <ClipboardIcon text={params.row.SKU} />
          </>
        );
      },
    },
    {
      field: "Quantity",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.ANALYTICS_QUANTITY}
        />
      ),
      minWidth: 100,
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => {
        return <CodeBox title={params.row.Quantity} hasColor={false} />;
      },
    },
    {
      field: "Revenue",
      headerName: (
        <DataGridHeaderBox
          title={LanguageReducer?.languageType?.ANALYTICS_REVENUE}
        />
      ),
      minWidth: 150,
      flex: 1,
      align: "right",
      headerAlign: "right",
      renderCell: (params) => {
        return <Box>{amountFormat(params.row.Revenue) || "-"}</Box>;
      },
    },
  ];

  const handleDashboardFilter = async () => {
    getAnalyticsData();
  };

  const getAnalyticsData = async () => {
    handleGetCarrierStatsNew();
    handleGetTotalCarriers();
    handleGetActiveCarriers();
    handleGetProductCounts();
    handleGetAllProductVariantsCount();
    handleGetTotalNoofOrdersPlacedCounts();
    handleGetToBeShippedCount();
    handleGetToBePackedCount();
    handleGetTopSellingItemsCount();
    handleGetLowStockItemsCount();
    handleGetDeliveryRatioCount();
    handleGetTotalUncollected();
    handleGetTotalStockValue();
    handleGetTotalPurchaseStockValue();
    handleGetTotalCollected();
    handleGetReturnedOrderCount();
    handleGetInProgressOrderCount();
    handleGetRegularOrderCount();
    handleGetFulFillableOrderCount();
    handleGetDelieveredOrderCount();
    handleGetTotalStoreCount();
    handleGetTotalCompletedOrderCountWithCarrier();
    handleGetTotalInprogressOrderCountWithCarrier();
    handleGetCarrierActivityWithDetail();
    // handleGetCarrierStats();
  };

  const handleFilterRest = () => {
    setFilterModal({
      createdFrom: "",
      createdTo: "",
    });
    setIsFilterClear(true);
  };

  const getGraphHeight = () => {
    const height45Percent = (gridContainerHeight * 45) / 100;
    const _height = graphData.carriesStatsNew.barType
      ? height45Percent - 40 - 20 - 8 - 14
      : height45Percent - 40 - 20 - 10;
    return _height;
  };

  const CallBackNewGraph = useCallback(
    ({ active }) => {
      return (
        <>
          <FullScreen handle={handle}>
            <Box
              sx={{
                background: "#fff",
                borderRadius: "10px",
                position: "relative",
                display: { xs: "flex", sm: "block", md: "block" },
                justifyContent: { xs: "center" },
              }}
            >
              <ReactApexChart
                options={{
                  ...(graphData.carriesStatsNew.barType
                    ? {
                        ...barGraphData.options,
                        xaxis: {
                          categories: graphData.carriesStatsNew.companies,
                          labels: {
                            show: true,
                            formatter: function (val) {
                              const numericalIndex =
                                graphData?.carriesStatsNew?.companies?.indexOf(
                                  val
                                );

                              if (numericalIndex === -1) {
                                return "0";
                              }

                              const total =
                                graphData?.carriesStatsNew?.carrierTotals?.[
                                  numericalIndex
                                ] || 0;

                              return `${val}:${total}`;
                            },
                            style: {
                              fontSize: "9px",
                              fontWeight: 400,
                            },
                          },
                        },

                        dataLabels: {
                          enabled: false,
                        },
                        tooltip: {
                          enabled: true,
                          y: {
                            formatter: function (val, opts) {
                              return (
                                val +
                                " (" +
                                opts.w.globals.seriesPercent[opts.seriesIndex][
                                  opts.dataPointIndex
                                ].toFixed(2) +
                                "%)"
                              );
                            },
                          },
                        },
                      }
                    : {
                        ...pieGrapgData.options,
                        labels: graphData.carriesStatsNew.statuses,
                        dataLabels: {
                          enabled: true,
                          formatter: function (val, opts) {
                            return val.toFixed(2) + "%";
                          },
                        },
                      }),
                  legend: {
                    position: "right",
                    horizontalAlign: "center",
                    floating: false,
                  },
                }}
                series={graphData.carriesStatsNew.series}
                type={graphData.carriesStatsNew.barType ? "bar" : "pie"}
                height={
                  active
                    ? window.outerHeight
                    : getGraphHeight(heightBelow650, heightBelow500, height)
                }
              />
              <Box sx={{ position: "absolute", bottom: 0, right: 0 }}>
                <ScreenToggleButton
                  show={active}
                  onClick={active ? handle.exit : handle.enter}
                />
              </Box>
            </Box>
          </FullScreen>
        </>
      );
    },
    [graphData.carriesStatsNew.barType, graphData.carriesStatsNew.series]
  );

  const reorder = (list, startIndex, endIndex) => {
    const result = Array.from(list);
    const [removed] = result.splice(startIndex, 1);
    result.splice(endIndex, 0, removed);

    return result;
  };
  useEffect(() => {
    // if (isFilterClear) {
    getAnalyticsData();
    // setIsFilterClear(false);
    // }
  }, [isFilterClear]);
  const cardChildren = {
    [LanguageReducer?.languageType?.ANALYTICS_CARRIERS_STATS]: (
      <AnalyticsMainCard
        title={LanguageReducer?.languageType?.ANALYTICS_CARRIERS_STATS}
        bgColor={"#eeeeee6e"}
        mt={0}
        titleRightSide={
          <>
            <FilterIconButton onClick={handleOpen} />
            <MenuComponent
              anchorEl={anchorEl}
              open={open}
              onClose={handleClose}
            >
              <Box
                sx={{
                  display: "flex",
                }}
              >
                <GraphDateRange
                  graphFilterModal={graphFilterModal}
                  setGraphFilterModal={setGraphFilterModal}
                />
                <Box mx={1.5} mt={1.5}>
                  <ButtonComponent
                    title={LanguageReducer?.languageType?.ANALYTICS_FILTER}
                    onClick={() => {
                      // handleGetCarrierStats();
                      handleGetCarrierStatsNew();
                      handleClose();
                    }}
                  />
                </Box>
              </Box>
            </MenuComponent>
          </>
        }
      >
        {/* new chart */}
        <Box className={"w_100"}>
          <CallBackNewGraph active={handle.active} />
        </Box>
      </AnalyticsMainCard>
    ),
    [LanguageReducer?.languageType?.ANALYTICS_CARRIERS_ACTIVITY]: (
      <AnalyticsMainCard
        title={LanguageReducer?.languageType?.ANALYTICS_CARRIERS_ACTIVITY}
        bgColor={"#eeeeee6e"}
        height={"100%"}
      >
        <AnalyticsCard
          title={LanguageReducer?.languageType?.ANALYTICS_TOTAL}
          data={count.totalCarriers}
          Icon={totalCarriers}
          iconBgColor={"#F63AA0"}
          height={{ xs: 50, md: "25%" }}
        />

        <AnalyticsCard
          data={count.totalActiveCarriers}
          title={LanguageReducer?.languageType?.ANALYTICS_ACTIVE}
          Icon={activeCariiers}
          iconBgColor={"#F63AA0"}
          height={{ xs: 50, md: "25%" }}
        />

        <AnalyticsCard
          data={count.totalInprogressOrderCountWithCarrier}
          title={LanguageReducer?.languageType?.ANALYTICS_IN_PROGRESS}
          Icon={inProgressCarriers}
          iconBgColor={"#F63AA0"}
          height={{ xs: 50, md: "25%" }}
        />

        <AnalyticsCard
          data={count.totalCompletedOrderCountWithCarrier}
          title={LanguageReducer?.languageType?.ANALYTICS_COMPLETED}
          Icon={completedCarriers}
          iconBgColor={"#F63AA0"}
          height={{ xs: 50, md: "25%" }}
        />
      </AnalyticsMainCard>
    ),
    [LanguageReducer?.languageType?.ANALYTICS_SALES_ACTIVITY]: (
      <AnalyticsMainCard
        title={LanguageReducer?.languageType?.ANALYTICS_SALES_ACTIVITY}
        bgColor={"#eeeeee6e"}
        height={"100%"}
      >
        {/* sales activity */}
        <Box
          className={"flex_col h_33"}
          sx={{
            gap: { xs: "5px", md: "3%" },
          }}
        >
          <Box
            className={"flex_between"}
            gap={{ xs: "5px", md: 1 }}
            flexDirection={{ xs: "column", md: "row" }}
            height={"100%"}
          >
            <Box width={{ xs: "100%", md: "50%" }} className={"h_100"}>
              <AnalyticsCard
                data={count.totalStoreCount}
                title={LanguageReducer?.languageType?.ANALYTICS_STORES_CHANNELS}
                Icon={storeInSales}
                iconBgColor={"#474AD9"}
                height={{ xs: 50, md: "100%" }}
              />
            </Box>
            <Box width={{ xs: "100%", md: "50%" }} className={"h_100"}>
              <AnalyticsCard
                data={count.totalProduct}
                title={LanguageReducer?.languageType?.ANALYTICS_TOTAL_PRODUCTS}
                Icon={products}
                iconBgColor={"#474AD9"}
                height={{ xs: 50, md: "100%" }}
              />
            </Box>
          </Box>
          <Box
            className={"flex_between"}
            gap={{ xs: "5px", md: 1 }}
            flexDirection={{ xs: "column", md: "row" }}
            height={"100%"}
          >
            <Box width={{ xs: "100%", md: "50%" }} className={"h_100"}>
              <AnalyticsCard
                data={count.totalProductVariants}
                title={
                  LanguageReducer?.languageType?.ANALYTICS_TOTAL_PRODUCTS_SKUS
                }
                Icon={totalProductsSKUs}
                iconBgColor={"#474AD9"}
                height={{ xs: 50, md: "100%" }}
              />
            </Box>
            <Box width={{ xs: "100%", md: "50%" }} className={"h_100"}>
              <AnalyticsCard
                data={count.totalActiveCarriers}
                title={
                  LanguageReducer?.languageType
                    ?.ANALYTICS_ACTIVE_CARRIER_PARTNERS
                }
                Icon={activeCarriersPartner}
                iconBgColor={"#474AD9"}
                height={{ xs: 50, md: "100%" }}
              />
            </Box>
          </Box>
        </Box>
        {/* top products */}
        <Box
          sx={{
            p: { xs: 0, md: 1 },
            borderRadius: "20px",
            height: "100%",
            overflowY: { xs: "initial", md: "scroll" },
          }}
        >
          <Typography variant="h5" mb={"14px"}>
            {LanguageReducer?.languageType?.ANALYTICS_TOP_PRODUCTS}
          </Typography>
          <DataGridComponent
            columns={productColumns}
            rows={dataTable.salesActivityWithDetail.data}
            loading={dataTable.salesActivityWithDetail.loading}
            bgColor={"#fff"}
            height={belowMdScreen ? 200 : "100%"}
            footerHeight={28}
            rowHeight={30}
          />
        </Box>
      </AnalyticsMainCard>
    ),
    [LanguageReducer?.languageType?.ANALYTICS_FINANCE]: (
      <GridContainer
        sx={{ height: { xs: "100%", md: "calc(100% + 8px)" } }}
        spacing={1}
      >
        <GridItem xs={12} sx={{ height: { xs: "100%", md: "40%" } }}>
          <AnalyticsMainCard
            title={LanguageReducer?.languageType?.ANALYTICS_FINANCE}
            bgColor={"#eeeeee6e"}
            height={"100%"}
            width={"100%"}
          >
            <Box
              className={"flex_between h_100"}
              gap={1}
              flexDirection={{ xs: "column", md: "row" }}
            >
              <Box
                sx={{
                  width: { xs: "100%", md: "50%" },
                  height: "100%",
                }}
              >
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_COD}
                  data={count.totalUncollectedFinance}
                  Icon={financeCOD}
                  iconBgColor={"#FB4D31"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
              <Box
                width={{ xs: "100%", md: "50%" }}
                sx={{
                  height: "100%",
                }}
              >
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_PAID}
                  data={count.totalUncollectedFinance}
                  Icon={financePaid}
                  iconBgColor={"#FB4D31"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
            </Box>
            <Box
              className={"flex_between h_100"}
              gap={1}
              flexDirection={{ xs: "column", md: "row" }}
            >
              <Box width={{ xs: "100%", md: "50%" }} className={"h_100"}>
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_STOCK_VALUE}
                  data={count.totalStockValue}
                  Icon={financeStockValue}
                  iconBgColor={"#FB4D31"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
              <Box width={{ xs: "100%", md: "50%" }} className={"h_100"}>
                <AnalyticsCard
                  title={
                    LanguageReducer?.languageType?.ANALYTICS_PURCHASE_VALUE
                  }
                  data={count.totalPurchaseValue}
                  Icon={financeStockValue2}
                  iconBgColor={"#FB4D31"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
            </Box>
          </AnalyticsMainCard>
        </GridItem>

        {/* orders activity */}
        <GridItem xs={12} sx={{ height: { xs: "100%", md: "60%" } }}>
          <AnalyticsMainCard
            title={LanguageReducer?.languageType?.ANALYTICS_ORDERS_ACTIVITY}
            bgColor={"#eeeeee6e"}
            height={"100%"}
            width={"100%"}
          >
            <Box
              // className={{
              //   md: `flex_between  ${heightBelow650 ? "h_50" : "h_33"}`,
              // }}

              sx={{
                display: "flex",
                justifyContent: { md: "space-between" },
                flexDirection: { xs: "column", md: "row" },
                height: { xs: "16.5%", md: "33%" },
              }}
              gap={1}
            >
              <Box
                flexBasis={heightBelow650 ? "33%" : "50%"}
                className={"h_100"}
              >
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_TOTAL}
                  data={count.totalNoofOrdersPlaced}
                  Icon={ordersTotal}
                  iconBgColor={"#71C92C"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
              <Box
                flexBasis={heightBelow650 ? "33%" : "50%"}
                className={"h_100"}
              >
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_REGULAR}
                  data={count.totalRegularOrderCount}
                  Icon={ordersRegular}
                  iconBgColor={"#71C92C"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
              <Box
                flexBasis={heightBelow650 ? "33%" : "50%"}
                height={"100%"}
                display={heightBelow650 ? "block" : "none"}
              >
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_FULLFILLABLE}
                  data={count.totalFulFillableOrderCount}
                  Icon={ordersFulfillable}
                  iconBgColor={"#71C92C"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
            </Box>
            <Box
              // className={`flex_between  ${heightBelow650 ? "h_50" : "h_33"}`}
              gap={1}
              // display={heightBelow650 ? "none !important" : "flex"}
              sx={{
                display: "flex",
                justifyContent: { md: "space-between" },
                flexDirection: { xs: "column", md: "row" },
                height: { xs: "16.5%", md: "33%" },
              }}
            >
              <Box flexBasis={"50%"} height={"100%"}>
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_FULLFILLABLE}
                  data={count.totalFulFillableOrderCount}
                  Icon={ordersFulfillable}
                  iconBgColor={"#71C92C"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
              <Box flexBasis={"50%"} height={"100%"}>
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_DELIVERED}
                  data={count.totalDelieveredOrderCount}
                  Icon={ordersDelivered}
                  iconBgColor={"#71C92C"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>{" "}
            </Box>
            <Box
              // className={`flex_between  ${heightBelow650 ? "h_50" : "h_33"}`}
              sx={{
                display: "flex",
                justifyContent: { md: "space-between" },
                flexDirection: { xs: "column", md: "row" },
                height: { xs: "16.5%", md: "33%" },
              }}
              gap={1}
            >
              <Box
                flexBasis={heightBelow650 ? "33%" : "50%"}
                height={"100%"}
                display={heightBelow650 ? "block" : "none"}
              >
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_DELIVERED}
                  data={count.totalDelieveredOrderCount}
                  Icon={ordersDelivered}
                  iconBgColor={"#71C92C"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
              <Box
                flexBasis={heightBelow650 ? "33%" : "50%"}
                className={"h_100"}
              >
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_TO_BE_SHIPPED}
                  data={count.totalDelieveredOrderCount}
                  Icon={ordersToBeShipped}
                  iconBgColor={"#71C92C"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
              <Box
                flexBasis={heightBelow650 ? "33%" : "50%"}
                className={"h_100"}
              >
                <AnalyticsCard
                  title={LanguageReducer?.languageType?.ANALYTICS_To_BE_PACKED}
                  data={count.totalToBePacked}
                  Icon={ordersToBePacked}
                  iconBgColor={"#71C92C"}
                  height={{ xs: 50, md: "100%" }}
                />
              </Box>
            </Box>
          </AnalyticsMainCard>
        </GridItem>
      </GridContainer>
    ),
  };
  return (
    <PageMainBox>
      {/* filter box */}
      <Box
        className={"flex_between"}
        sx={{
          height: 54,
          mb: 1,
          alignItems: "end !important",
        }}
      >
        <Box width={{ xs: 100, md: 210, sm: 210 }}>
          <Typography variant="h2">
            <span style={{ fontWeight: 400 }}>Welcome Back,</span>
            <span style={{ fontWeight: 700, marginLeft: 10 }}>
              {textTrucate(userName, 100)}
            </span>
          </Typography>
        </Box>
        <Box sx={{ display: "flex", alignItems: "end", gap: 1 }}>
          <Box>
            <ButtonComponent
              title={
                sampleLoaded
                  ? LanguageReducer?.languageType?.ANALYTICS_LOAD_ORIGINAL
                  : LanguageReducer?.languageType?.ANALYTICS_LOAD_SAMPLE
              }
              onClick={() => {
                setSampleLoaded((prev) => !prev);
                if (sampleLoaded) {
                  getAnalyticsData();
                  handleGetCarrierStatsNew();
                } else {
                  setCount(sampleCountData);
                  handleManuplateCarrierStat(sampleCarrierStat);
                  setDataTable((prev) => ({
                    ...prev,
                    salesActivityWithDetail: {
                      loading: false,
                      data: [
                        {
                          id: 1,
                          ProductName: "Laptop",
                          SKU: "ZXCG212471",
                          Quantity: 8,
                          Revenue: 199997.0,
                        },
                      ],
                    },
                  }));
                }
              }}
            />
          </Box>
          {belowMdScreen ? (
            <Box>
              <Button
                sx={{ ...styleSheet.filterIconColord, minWidth: "90px" }}
                color="inherit"
                variant="outlined"
                onClick={() => setIsFilterOpen(!isFilterOpen)}
                startIcon={<FilterAltOutlinedIcon fontSize="small" />}
              >
                {LanguageReducer?.languageType?.FILTER}
              </Button>
            </Box>
          ) : (
            <Box>
              <Box className={"flex_center"} gap={1}>
                <Box width={"100%"}>
                  <InputLabel
                    sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                  >
                    {"Start Date"}
                  </InputLabel>
                  <CustomReactDatePickerInput
                    value={filterModal.createdFrom}
                    onClick={(date) =>
                      setFilterModal((prevState) => ({
                        ...prevState,
                        createdFrom: date,
                      }))
                    }
                    maxDate={UtilityClass.todayDate()}
                    size="small"
                    isClearable
                    inputProps={{
                      style: {
                        padding: "5.6px 5px",
                        width: "90px",
                        fontSize: 12,
                      },
                    }}
                  />
                </Box>
                <Box width={"100%"}>
                  <InputLabel
                    sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                  >
                    {"End Date"}
                  </InputLabel>
                  <CustomReactDatePickerInput
                    maxDate={UtilityClass.todayDate()}
                    value={filterModal.createdTo}
                    onClick={(date) =>
                      setFilterModal((prevState) => ({
                        ...prevState,
                        createdTo: date,
                      }))
                    }
                    size="small"
                    minDate={filterModal.createdFrom}
                    disabled={!filterModal.createdFrom ? true : false}
                    isClearable
                    inputProps={{
                      style: {
                        padding: "5.6px 5px",
                        width: "90px",
                        fontSize: 12,
                      },
                    }}
                  />
                </Box>
                <Box display={"flex"} gap={1} alignSelf={"end"}>
                  <Button
                    sx={{
                      ...styleSheet.filterIcon,
                      minWidth: "100px",
                    }}
                    color="inherit"
                    variant="outlined"
                    onClick={() => {
                      handleFilterRest();
                    }}
                  >
                    {LanguageReducer?.languageType?.CLEAR_FILTER_TEXT}
                  </Button>

                  <Button
                    sx={{ ...styleSheet.filterIconColord, minWidth: "100px" }}
                    color="inherit"
                    variant="outlined"
                    onClick={() => {
                      handleDashboardFilter();
                    }}
                    startIcon={<FilterAltOutlinedIcon fontSize="small" />}
                  >
                    {LanguageReducer?.languageType?.ANALYTICS_FILTER}
                  </Button>
                </Box>
              </Box>
            </Box>
          )}
        </Box>
      </Box>
      {isFilterOpen && belowMdScreen ? (
        <Box display={"flex"} px={0.5} py={2} gap={1} justifyContent={"end"}>
          <Box
            gap={1}
            flexDirection={{ xs: "column", md: "row", sm: "row" }}
            display={"flex"}
          >
            <Box
              width={"100%"}
              display={"flex"}
              flexDirection={{ xs: "column", md: "row", sm: "row" }}
              gap={{ md: "5px", sm: "5px", xs: "0px" }}
            >
              <Box>
                <InputLabel
                  sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                >
                  {"Start Date"}
                </InputLabel>
                <CustomReactDatePickerInput
                  value={filterModal.createdFrom}
                  onClick={(date) =>
                    setFilterModal((prevState) => ({
                      ...prevState,
                      createdFrom: date,
                    }))
                  }
                  maxDate={UtilityClass.todayDate()}
                  size="small"
                  isClearable
                  inputProps={{
                    style: {
                      padding: "5.6px 5px",
                      width: "90px",
                      fontSize: 12,
                    },
                  }}
                />
              </Box>
              <Box>
                <InputLabel
                  sx={{ ...styleSheet.inputLabel, overflow: "unset" }}
                >
                  {"End Date"}
                </InputLabel>
                <CustomReactDatePickerInput
                  maxDate={UtilityClass.todayDate()}
                  value={filterModal.createdTo}
                  onClick={(date) =>
                    setFilterModal((prevState) => ({
                      ...prevState,
                      createdTo: date,
                    }))
                  }
                  size="small"
                  minDate={filterModal.createdFrom}
                  disabled={!filterModal.createdFrom ? true : false}
                  isClearable
                  inputProps={{
                    style: {
                      padding: "5.6px 5px",
                      width: "90px",
                      fontSize: 12,
                    },
                  }}
                />
              </Box>
            </Box>
            <Box alignSelf={"end"}>
              <Box display={"flex"} gap={1} alignSelf={"end"}>
                <Button
                  sx={{
                    ...styleSheet.filterIcon,
                    minWidth: "100px",
                  }}
                  color="inherit"
                  variant="outlined"
                  onClick={() => {
                    handleFilterRest();
                  }}
                >
                  {LanguageReducer?.languageType?.CLEAR_FILTER_TEXT}
                </Button>

                <Button
                  sx={{
                    ...styleSheet.filterIcon,
                    minWidth: "100px",
                  }}
                  variant="contained"
                  onClick={() => {
                    handleDashboardFilter();
                  }}
                >
                  {LanguageReducer?.languageType?.ANALYTICS_FILTER}
                </Button>
              </Box>
            </Box>
          </Box>
        </Box>
      ) : null}
      <DragDropContext
        onDragEnd={(params) => {
          if (!params.destination) {
            return;
          }
          const sorceIndex = params.source.index;
          const destinationIndex = params.destination.index;
          const items = reorder(cardData, sorceIndex, destinationIndex);
          setCardData(items);
        }}
      >
        <Droppable droppableId="droppable-1">
          {(provided, snapshot) => (
            <div ref={provided.innerRef} {...provided.droppableProps}>
              <GridContainer
                spacing={1}
                sx={{ height: { xs: "100%", md: gridContainerHeight } }}
              >
                {cardData.map((card, index) => {
                  return (
                    <Draggable
                      key={card.id}
                      draggableId={`draggable-${card.id}`}
                      index={index}
                    >
                      {(provided, snapshot) => (
                        <Box
                          component={Grid}
                          id={"test"}
                          item
                          xs={12}
                          md={card.md}
                          sx={{ height: { xs: "100%", md: card.height } }}
                          ref={provided.innerRef}
                          {...provided.draggableProps}
                          {...provided.dragHandleProps}
                        >
                          {cardChildren[card.id]}
                        </Box>
                      )}
                    </Draggable>
                  );
                })}
                {provided.placeholder}
              </GridContainer>
            </div>
          )}
        </Droppable>
      </DragDropContext>
    </PageMainBox>
  );
}
export default AnalyticsPage;
