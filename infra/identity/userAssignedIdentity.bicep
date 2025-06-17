@description('Base name used for all resources')
param baseName string

@description('The Azure region where this Managed Identity resource will be deployed. Default is the resource group location')
param location string = resourceGroup().location

@description('The name of the environment that this Managed Identity will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The tags that will be applied to the Managed Identity resource')
param tags object

var uaiName = 'uai-${baseName}-${environmentName}'

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2025-01-31-preview' = {
  name: uaiName
  location: location
  tags: tags
}

@description('The resource ID of the deployed user-assigned identity')
output id string = uai.id

@description('The name of the deployed user-assigned identity')
output name string = uai.name
