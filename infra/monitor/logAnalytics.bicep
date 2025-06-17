@description('Base name used for all resources')
param baseName string

@description('The Azure region where this Log Analytics resource will be deployed. Default is the resource group location')
param location string = resourceGroup().location

@description('The name of the environment that this Log Analytics workspace will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The tags that will be applied to the Log Analytics resource')
param tags object

var logAnalyticsName = 'law-${baseName}-${environmentName}'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2025-02-01' = {
  name: logAnalyticsName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30   
  }
}

@description('The resource ID of the deployed Log Analytics workspace')
output id string = logAnalytics.id

@description('The name of the deployed Log Analytics workspace')
output name string = logAnalytics.name
