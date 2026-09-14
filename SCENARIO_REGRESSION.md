# Shipra Assistant – Client/User Regression Matrix

This matrix is designed to prevent the failure patterns already seen:
semantic-neighbor mixing, parent/child entity confusion, incorrect intent
classification, invented UI steps, and proposed code appearing in normal
feature-usage answers.

## Existing feature / client-user scenarios

| Scenario | Expected routing / behavior |
|---|---|
| how to create dashboard? | Shipra existing-feature pipeline; verify dashboard entity + create operation |
| how to create dashboard in Shipra? | Shipra existing-feature pipeline |
| show carrier dashboard | Verify carrier dashboard page/route before describing UI |
| open sale dashboard | Verify sale dashboard route/page |
| return order kesy banau Shipra mai? | Verify return-order page and actual create/return flow |
| how to create Shipra stores? | Store != store channel; do not substitute channel creation |
| how to create store channel? | Verify store-channel flow specifically |
| connect Shopify sale channel | Verify connection flow; do not substitute update-config flow |
| sync sale channel | Verify sync action specifically |
| create order | Verify create-order screen/handler/API chain |
| upload orders | Verify upload/import flow, not manual create-order flow |
| import orders | Same as upload/import, with exact file/form evidence |
| return order | Return-order entity must remain distinct from regular order |
| update order amount | Verify amount-update flow only |
| update order status | Verify status-update flow only |
| assign carrier | Verify carrier assignment, not carrier dashboard/config |
| assign order label | Verify assignment flow, not reusable label creation |
| create order label | Verify reusable label creation, not assignment |
| delete order label | Verify delete flow specifically |
| validate order label fields | Frontend validation first; backend validation second |
| export order labels CSV | Verify actual export/download behavior |
| filter price calculator | Verify filter event and rate retrieval |
| track shipment | Verify tracking flow |
| show inventory | Verify inventory screen/data flow |
| update inventory | Verify update operation, not inventory listing |
| add product | Verify product-create flow |
| create customer | Verify customer-create flow |
| create lead | Verify lead-create flow |
| delete contact | Verify delete operation only |
| assign station | Verify station assignment only |
| show COD wallet | Verify wallet screen/route |
| carrier settlement | Verify settlement entity/flow |
| delivery tasks | Verify task screen/flow |
| analytics page | Verify analytics route/page |

## Mock/test data lookup scenarios

| Scenario | Expected behavior |
|---|---|
| Which orders have Priority or VIP label? | Answer directly from verified mock-data tool |
| Which COD orders are not delivered yet? | Apply payment + exclude-status filters |
| Which prepaid orders are assigned? | Apply payment + status filters |
| Show ORD-1001 | Exact record lookup |
| Which orders use TCS? | Carrier filter |
| Which delivered orders are VIP? | Combine verified filters |

## Project-change scenarios

These MUST be treated as code changes, not existing usage:
- change app.py to add delete chat
- modify dashboard UI
- implement a new bulk export feature
- create a new backend API endpoint
- refactor MCP search
- add a new validation rule

A Proposed implementation section is allowed only for this category.

## Prompt-generation scenarios

- generate coding prompt for return order feature
- make an AI prompt for dashboard redesign
- prompt bana do Shopify connection feature ka

These route to project_prompt.

## General questions that must stay general

- what is a dashboard?
- explain Python decorators
- how to build a dashboard in React?
- create dashboard in Excel
- how to build dashboard in Power BI
- what is inventory?
- what is a carrier?

Explicit generic technology context must not be hijacked by Shipra routing.

## Evidence rules

1. MCP-read raw source is the source of truth.
2. RAG/index output is discovery-only.
3. Entity AND action must both match.
4. Store != Store Channel.
5. Order != Return Order.
6. Create Label != Assign Label.
7. Create != Update != Delete != Filter != Export.
8. Backend-only evidence cannot prove a UI click path.
9. No direct code connection means no joined workflow.
10. Missing evidence means an evidence gap, not invented steps.
11. Existing-feature usage questions never receive proposed implementation code.
12. If raw source is missing from deployment, report source-health diagnostics instead of pretending the feature is absent.
