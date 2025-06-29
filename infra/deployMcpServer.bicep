targetScope='subscription'

@description('The base name of the Application')
param baseName string

@description('The Tags applied to all resources')
param tags object

@description('The name of the Container Registry that this template will use')
param containerRegistryName string

@description('The name of the user-assigned managed identity that these resources will use')
param uaiName string

@description('The container image that the MCP server will use')
param imageName string

@description('The email address of the publisher')
param emailAddress string

@description('The name of the publisher')
param publisherName string

var rgName = 'rg-${baseName}'

resource resourceGroup 'Microsoft.Resources/resourceGroups@2025-04-01' existing = {
  name: rgName
}

module logAnalytics 'monitor/logAnalytics.bicep' = {
  scope: resourceGroup
  name: 'logAnalytics'
  params: {
    tags: tags
    baseName: baseName
  }
}

module appInsights 'monitor/appInsights.bicep' = {
  scope: resourceGroup
  name: 'appInsights'
  params: {
    tags: tags
    baseName: baseName
    logAnalyticsWorkspaceId: logAnalytics.outputs.id 
  }
}

module containerAppEnvironment 'host/containerAppEnvironment.bicep' = {
  scope: resourceGroup
  name: 'Env'
  params: {
    tags: tags
    appInsightsName: appInsights.outputs.name 
    baseName: baseName
    lawName: logAnalytics.outputs.name
  }
}

module apim 'apim/apim.bicep' = {
  scope: resourceGroup
  name: 'apim'
  params: {
    tags: tags
    appInsightsName: appInsights.outputs.name
    baseName: baseName
    emailAddress: emailAddress
    publisherName: publisherName
  }
}

module mcpServer 'host/mcpServer.bicep' = {
  scope: resourceGroup
  params: {
    tags: tags
    appInsightsName: appInsights.outputs.name 
    baseName: baseName
    containerAppEnvironmentId: containerAppEnvironment.outputs.id 
    containerRegistryName: containerRegistryName
    uaiName: uaiName
    imageName: imageName
  }
}
