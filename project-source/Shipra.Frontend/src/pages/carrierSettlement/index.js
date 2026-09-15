import { Box } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { GetAllCarrierPaymentSettlements } from "../../api/AxiosInterceptors";
import { styleSheet } from "../../assets/styles/style";
import CarrierSettlementList from "./carrierSettlementList/index";

function CarrierSettlementPage(props) {
  const [allCarrierPaymentSettlements, setAllCarrierPaymentSettlements] =
    useState([]);
  const [isLoading, setIsLoading] = useState(false);

  let getAllCarrierPaymentSettlements = async () => {
    setIsLoading(true);
    let res = await GetAllCarrierPaymentSettlements({
      filterModel: {
        createdFrom: null,
        createdTo: null,
        start: 0,
        length: 10000,
        search: "",
        sortDir: "desc",
        sortCol: 0,
      },
    });
    setIsLoading(false);
    if (res.data.result !== null)
      setAllCarrierPaymentSettlements(res.data.result.list);
  };

  useEffect(() => {
    getAllCarrierPaymentSettlements();
  }, []);
  const LanguageReducer = useSelector((state) => state.LanguageReducer);
  return (
    <Box sx={styleSheet.pageRoot}>
      <div style={{ padding: "10px" }}>
        {" "}
        <CarrierSettlementList
          loading={isLoading}
          getAllCarrierPaymentSettlements={getAllCarrierPaymentSettlements}
          allCarrierPaymentSettlements={allCarrierPaymentSettlements}
        />
      </div>
    </Box>
  );
}
export default CarrierSettlementPage;
