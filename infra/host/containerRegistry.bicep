@description('Base name used for all resources')
param baseName string

@description('The Azure region where this Container Registry resource will be deployed. Default is the resource group location')
param location string = resourceGroup().location

@description('The name of the environment that this Container Registry will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The tags that will be applied to the Container Registry resource')
param tags object

@description('The name of the user-assigned identity that this Container Registry will use')
param uaiName string

// VARIABLES
var acrName = 'arc${replace(baseName,'-','')}${environmentName}'
var acrPullRoleDefintionId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
var acrPushRoleDefinitionId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions','8311e382-0749-4cb8-b61a-304f252e45ec')

// EXISTING RESOURCES
resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' existing = {
  name: uaiName
}

// CONTAINER REGISTRY RESOURCES
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2025-04-01' = {
  name: acrName
  tags: tags
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource acrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, containerRegistry.id, acrPullRoleDefintionId)
  properties: {
    principalId: uai.properties.principalId
    roleDefinitionId: acrPullRoleDefintionId
    principalType: 'ServicePrincipal'
  }
}

resource acrPushRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, containerRegistry.id, acrPushRoleDefinitionId)
  properties: {
    principalId: uai.properties.principalId
    roleDefinitionId: acrPushRoleDefinitionId
    principalType: 'ServicePrincipal'
  }
}

@description('The resource ID of the deployed Container Registry')
output id string = containerRegistry.id

@description('The name of the deployed Container Registry')
output name string = containerRegistry.name
