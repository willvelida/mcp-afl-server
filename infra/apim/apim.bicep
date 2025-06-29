@description('Base name used for all resources')
param baseName string

@description('The Azure region where this APIM resource will be deployed. Default is the resource group location')
param location string = resourceGroup().location

@description('The name of the environment that this APIM will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The tags that will be applied to the APIM resource')
param tags object

@description('The email address of the publisher')
param emailAddress string

@description('The name of the Publisher')
param publisherName string

@description('The name of the App Insights workspace that this APIM will use')
param appInsightsName string

var apimName = 'apim-${baseName}-${environmentName}'
var loggerName = '${apimName}-logger'
var loggerDescription = 'APIM Logger for MCP Servers'

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource apim 'Microsoft.ApiManagement/service@2024-06-01-preview' = {
  name: apimName
  location: location
  tags: tags
  sku: {
    name: 'Consumption'
    capacity: 0
  }
  properties: {
    publisherEmail: emailAddress
    publisherName: publisherName
  }
}

resource apimLogger 'Microsoft.ApiManagement/service/loggers@2024-06-01-preview' = {
  name: loggerName
  parent: apim
  properties: {
    loggerType: 'applicationInsights'
    credentials: {
      instrumentationKey: appInsights.properties.InstrumentationKey
    }
    description: loggerDescription
    isBuffered: false
    resourceId: appInsights.id
  }
}

@description('The resource ID of the deployed APIM Service')
output id string = apim.id

@description('The name of the deployed APIM service')
output name string = apim.name

@description('The Gateway URL of the deployed APIM service')
output gatewayUrl string = apim.properties.gatewayUrl
