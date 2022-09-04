# Cost Estimate

## Original Assumptions and recommendations

The original cost estimate assumed that users where matched once a week and 400 RUs where needed across all Azure Cosmos DB collections. This is probably also true for this version, but as we storing and reading mor stuff from the database the RU assumption may be wrong.

The original recommendation was to use 400 provisioned RUs for the Cosmos DB and S1 for the Azure App Service. As Azure now offers a serverless Cosmos DB this App supports that and it is the default. I think the S1 App Service plan is quite expensive so I'm testing F1/D1 right now, but I don't know if it works yet. But as the bot usually only got load once a week i suppose it should be fine. (I think these kind of bots would be great for Azure Functions, if anyone want to give it a try ;)).

## [](/wiki/costestimate#estimated-load)Estimated load

**Data storage**: 1 GB max

**Logic Apps**: 1 action executions per week

## [](/wiki/costestimate#estimated-cost)Estimated cost

**IMPORTANT:**  This is only an estimate, based on the assumptions above. Your actual costs may vary.

Prices were taken from the  [Pricin Calculator](https://azure.microsoft.com/en-us/pricing/)  on 04.09.2020 the West Europe region.

Use the  [Azure Pricing Calculator](https://azure.com/e/ecf1f0efa694499cb0b6b8ac2b466b5a)  to model different service tiers and usage patterns.

|  Resource |  Tier |  Load |  Monthly price |
|---|---|---|---|
| Azure Cosmos DB| serverless |< 1GB storage, 1.000.000 RUs| $0,55 |
|  Azure Bot Service | S1  |  N/A | $0  |
|  App Service Plan | S1  | 730 hours  | $73.00  |
|  Logic Apps| -|1 action execution / 7 day(s)  | $0.01 |
|  Azure Monitor (Application Insights) | -  |  < 1GB data | (free up to 5 GB)|
|**Total**|||**$73.56**|

