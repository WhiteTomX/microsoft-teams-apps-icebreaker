resource "local_file" "icon_color" {
  content_base64 = filebase64(var.icon_color_path)
  filename       = "${path.module}/tmp/color.png"
}

resource "local_file" "icon_color" {
  content_base64 = filebase64(var.icon_outline_path)
  filename       = "${path.module}/tmp/outline.png"
}

resource "local_file" "manifest" {
  filename = "${path.module}/tmp/manifest.json"
  content = jsonencode({
    "$schema"         = "https://developer.microsoft.com/en-us/json-schemas/teams/v1.5/MicrosoftTeams.schema.json"
    "manifestVersion" = "1.5"
    "version"         = var.app_version
    "id"              = azuread_application.icebreaker.application_id
    "packageName"     = "de.whitetom.icebreaker${var.name}"
    "developer" = {
      "name"          = var.companyName
      "websiteUrl"    = var.websiteUrl
      "privacyUrl"    = var.privacyUrl
      "termsOfUseUrl" = var.termsOfUseUrl
    }

    "localizationInfo" = {
      "defaultLanguageTag" = "de"
    }

    "icons" = {
      "color"   = "color.png"
      "outline" = "outline.png"
    }

    "name" = {
      "short" = var.name
    }

    "description" = {
      "short" = var.description_short
      "full"  = var.description_long
    }

    "accentColor" = "#1037A6"

    "bots" = [{
      "botId"              = azuread_application.icebreaker.application_id
      "scopes"             = ["personal", "team"]
      "supportsFiles"      = false
      "isNotificationOnly" = true
    }]

    "permissions"  = ["identity", "messageTeamMembers"]
    "validDomains" = [jsondecode(azurerm_resource_group_template_deployment.icebreaker.output_content).appDomain.value]
  })
}

data "archive_file" "app_package" {
  type        = "zip"
  output_path = "${path.module}/manifest.zip"
  source_dir  = "${path.module}/tmp"
  depends_on = [
    local_file.logo, local_file.manifest
  ]
}
