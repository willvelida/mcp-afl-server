@description('Base name used for all resources')
param baseName string

@description('The Azure region where this Container App Environment resource will be deployed. Default is the resource group location')
param location string = resourceGroup().location

@description('The name of the environment that this Container App Environment will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The tags that will be applied to the Container App Environment resource')
param tags object

@description('The name of the Log Analytics workspace that this Container App Environment will use')
param lawName string

@description('The name of the App Insights workspace that this Container App Environment will use')
param appInsightsName string

var envName = 'env-${baseName}-${environmentName}'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2025-02-01' existing = {
  name: lawName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource env 'Microsoft.App/managedEnvironments@2025-02-02-preview' = {
  name: envName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
    appInsightsConfiguration: {
      connectionString: appInsights.properties.ConnectionString
    }
    openTelemetryConfiguration: {
      logsConfiguration: {
        destinations: [
          'appInsights'
        ]
      }
      tracesConfiguration: {
        destinations: [
          'appInsights'
        ]
      }
    }
  }
}

@description('The resource ID of the deployed Container App Environment')
output id string = env.id
