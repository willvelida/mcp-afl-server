@description('Base name used for all resources')
param baseName string

@description('The Azure region where this App Insights resource will be deployed. Default is the resource group location')
param location string = resourceGroup().location

@description('The name of the environment that this App Insights workspace will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The tags that will be applied to the App Insights resource')
param tags object

@description('The resource ID of the Log Analytics workspace that this App Insights workspace will be connected to')
param logAnalyticsWorkspaceId string

var appInsightsName = 'appins-${baseName}-${environmentName}'

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

@description('The resource ID of the deployed App Insights workspace')
output id string = appInsights.id

@description('The name of the deployed App Insights workspace')
output name string = appInsights.name

@description('The Connection String of the deployed App Insights workspace')
output connectionString string = appInsights.properties.ConnectionString
