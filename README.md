# Icebreaker App

Many good things in life come about through happenstance. Icebreaker enables happenstance for your team. Icebreaker is a Teams bot that helps the whole team get closer by pairing up members every week at random to meet for coffee, burgers, pizza, or a walk around the block. The bot does the heavy lifting of finding a new person to meet with each week, and aids in scheduling the meetup.

Icebreaker works really well for cultivating small, interest-based communities within your organization. For example, an organization may leverage this bot for a DevOps interest group to help facilitate organic cross-pollination of ideas and best practices. Another common use case is new employees’ cohorts to help employees learn more about each other and assimilate better in the new environment.

This is a fork of the [icebreaker app from microsoft](https://github.com/OfficeDev/microsoft-teams-icebreaker-app). This version of the Icebreaker uses Opt-In instead of OptOut, offers Icebreaker-questions from database and allows to change the translation of texts dynamically from the database. On the technical side is uses terraform to deploy and allows the use of serverless and free CosmosDB.

# Icebreaker App Template Docs (for reference)

| [Documentation](https://github.com/OfficeDev/microsoft-teams-icebreaker-app/wiki) | [Deployment guide](https://github.com/OfficeDev/microsoft-teams-icebreaker-app/wiki/Deployment-guide) | [Architecture](https://github.com/OfficeDev/microsoft-teams-icebreaker-app/wiki/Solution-overview) |
| --------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------- |

![Icebreaker in action](wiki/images/IcebreakerScheduling.gif)

## Legal notice

This app template is provided under the [MIT License](LICENSE) terms.  In addition to these terms, by using this app template you agree to the following:

- You, not Microsoft, will license the use of your app to users or organization.

- This app template is not intended to substitute your own regulatory due diligence or make you or your app compliant with respect to any applicable regulations, including but not limited to privacy, healthcare, employment, or financial regulations.

- You are responsible for complying with all applicable privacy and security regulations including those related to use, collection and handling of any personal data by your app. This includes complying with all internal privacy and security policies of your organization if your app is developed to be sideloaded internally within your organization. Where applicable, you may be responsible for data related incidents or data subject requests for data collected through your app.

- Any trademarks or registered trademarks of Microsoft in the United States and/or other countries and logos included in this repository are the property of Microsoft, and the license for this project does not grant you rights to use any Microsoft names, logos or trademarks outside of this repository. Microsoft’s general trademark guidelines can be found [here](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general.aspx).

- If the app template enables access to any Microsoft Internet-based services (e.g., Office365), use of those services will be subject to the separately-provided terms of use. In such cases, Microsoft may collect telemetry data related to app template usage and operation. Use and handling of telemetry data will be performed in accordance with such terms of use.

- Use of this template does not guarantee acceptance of your app to the Teams app store. To make this app available in the Teams app store, you will have to comply with the [submission and validation process](https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/deploy-and-publish/appsource/publish), and all associated requirements such as including your own privacy statement and terms of use for your app.

## Get started

Begin with the [Solution overview](wiki/Solution-overview) to read about what the app does and how it works.

When you're ready to try out Icebreaker, or to use it in your own organization, follow the steps in the [Deployment guide](wiki/Deployment-guide).

## Feedback

Please report bugs and other code issues [here](https://github.com/WhiteTomX/microsoft-teams-icebreaker-app/issues/new).

## Contributing

This project welcomes contributions and suggestions.  

<!--
Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

!-->

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
