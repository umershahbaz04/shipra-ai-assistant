import React from "react";
import {
  PageMainBox,
  useGetNavigateState,
} from "../../utilities/helpers/Helpers";
import { EnumRoutesUrls } from "../../utilities/enum";
import { useLocation } from "react-router-dom";

export default function PrincingPlan() {
  const { state } = useLocation();

  const {
    client_reference_id,
    customer_session_client_secret,
    pricing_table_id,
    publishable_key,
  } = state;
  console.log(state);
  return (
    <PageMainBox>
      <stripe-pricing-table
        pricing-table-id={pricing_table_id}
        publishable-key={publishable_key}
        client-reference-id={client_reference_id}
        customer-session-client-secret={customer_session_client_secret}
      ></stripe-pricing-table>
    </PageMainBox>
  );
}
