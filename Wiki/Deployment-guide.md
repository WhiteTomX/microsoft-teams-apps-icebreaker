# Deployment

## Prerequisites

Before you begin, please review the information in the [README file](../README.md#legal-notice). The default settings in the repo were selected to enable users to easily set up and try the application. You may need to modify these settings to meet your organization's specific requirements.

You will need:

* An Azure subscription where you can create the following kinds of resources:
  * Azure logic app
  * App service
  * App service plan
  * Bot channels registration
  * Azure Cosmos DB account
  * Application Insights
* A copy of the Icebreaker app GitHub repo
* Terraform installed
* AZ cli installed

The recommendation is to use CI/CD to deploy. If you want to deploy somehow manually see below. *Please note, that this is just a quick right up and may not be correct*.

## Create ResourceGroup

The terraform module expects a resourcegroup with the name `rg-${var.name}-${var.stage}-${var.suffix}`. PLease create it in Azure.

## Deploy via Terraform

Make sure that `az account show` shows the correct subscription.

Afterwards run `terraform init` and `terraform apply` as usual. You will be asked several inputs. There are more configurations available. Have a look at the [variables.tf](../variables.tf).

## Deploy code to webapp

Go to the created webapp and connect it to the repo or build and deploy manually.

## 4. Run the app in Microsoft Teams

The app package is created as `manifest.zip` in the root folder by terraform.

1. If your tenant has sideloading apps enabled, you can install your app to a team by following the instructions below.
    * Upload package to a team using the Apps tab: <https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/apps/apps-upload#upload-your-package-into-a-team-using-the-apps-tab>
    * Upload package to a team using the Store: <https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/apps/apps-upload#upload-your-package-into-a-team-or-conversation-using-the-store>

1. You can also upload it to your tenant's app catalog, so that it can be available for everyone in your tenant to install: <https://docs.microsoft.com/en-us/microsoftteams/tenant-apps-catalog-teams>

# Troubleshooting

Please see our [Troubleshooting](Troubleshooting) page.
