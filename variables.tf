variable "stage" {
  type        = string
  description = "Three characters long stage eg. dev, prd. Will be used for names"
}

variable "name" {
  type        = string
  description = "Name of the App"
  default     = "Icebreaker"
}

variable "suffix" {
  type        = string
  description = "Two character suffix to avoid name clashes"
  default     = "01"
}

variable "pairingWeekInterval" {
  type        = number
  default     = 1
  description = "The number of weeks between pairings."
}

variable "pairingDayOfWeek" {
  type        = string
  default     = "Monday"
  description = "The day of the week when pairings are created."
}

variable "pairingHour" {
  type        = number
  default     = 10
  description = "The hour at which pairings are created."
}

variable "pairingTimeZone" {
  type        = string
  default     = "W. Europe Standard Time"
  description = "The time zone for the hour at which pairings are created."
}

variable "sku" {
  type        = string
  default     = "Free"
  description = "The pricing tier for the hosting plan."
}

variable "defaultCulture" {
  type        = string
  default     = "de"
  description = "Default culture. You can select default culture from the following: en, de"
}


variable "appDisplayName" {
  type        = string
  default     = "Icebreaker"
  description = "The app (and bot) display name."
}

variable "description_short" {
  type        = string
  default     = "Bringt Menschen nach dem Zufallsprinzip zusammen."
  description = "The short description must not be longer than 80 characters"
}

variable "description_long" {
  type        = string
  default     = "Icebreaker ist ein niedlicher kleiner Bot, der jede Woche nach dem Zufallsprinzip Menschen zusammenbringt, um zu helfen, Vertrauen und persönliche Beziehungen aufzubauen."
  description = "The app (and bot) description."
}

variable "companyName" {
  type        = string
  default     = "Example Company"
  description = "The display name for the company."
}

variable "websiteUrl" {
  type        = string
  default     = "https://example.org"
  description = "The https:// URL to the company's website. This link should take users to your company or product-specific landing page."
}

variable "privacyUrl" {
  type        = string
  default     = "https://example.org/privacy"
  description = "The https:// URL to the company's privacy policy."
}

variable "termsOfUseUrl" {
  type        = string
  default     = "https://example.org/termsofuse"
  description = "The https:// URL to the company's terms of use."
}

variable "app_version" {
  type        = string
  default     = "1.2.0"
  description = "The version String of the App. Used in Manifest."
}

variable "cosmosDbEnableFreeTier" {
  type        = bool
  default     = false
  description = "Enable free Tier for CosmosDB. not compatible with Serverless."
}

variable "cosmosDbServerless" {
  type        = bool
  default     = true
  description = "Use serverless CosmosDB. Not compatible with Free Tier."
}

variable "icon_color_path" {
  type = string
  default = "./Manifest/color.png"
  description = "Path to the color png icon of the app."
}

variable "icon_outline_path" {
  type = string
  default = "./Manifest/outline.png"
  description = "Path to the outline png icon of the app."
}

variable "icon_url" {
  type = string
  default = "https://raw.githubusercontent.com/WhiteTomX/microsoft-teams-apps-icebreaker/main/Manifest/color.png"
  description = "The link to the icon for the bot. It must resolve to a PNG file."
}