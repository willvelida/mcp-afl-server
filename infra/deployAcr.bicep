targetScope='subscription'

@description('The base name of the Application')
param baseName string

@description('The Tags applied to all resources')
param tags object

@description('The GitHub Organization for the federated credentials')
param githubOrganization string

@description('The GitHub Repository for the federated credentials')
param githubRepositoryName string

@description('Location to deploy Azure resources')
@allowed([
  'australiaeast'
  'australiasoutheast'
  'newzealandnorth'
])
param location string

var rgName = 'rg-${baseName}'
var contributorRoleDefinitionId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions','b24988ac-6180-42a0-ab88-20f7382dd24c')

resource resourceGroup 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: rgName
  location: location
  tags: tags
}

module containerRegistry 'host/containerRegistry.bicep' = {
  name: 'acr'
  scope: resourceGroup
  params: {
    tags: tags
    baseName: baseName
    uaiName: uai.outputs.name
  }
}

module uai 'identity/userAssignedIdentity.bicep' = {
  name: 'uai'
  scope: resourceGroup
  params: {
    tags: tags
    baseName: baseName
    githubOrganization: githubOrganization
    githubRepositoryName: githubRepositoryName
  }
}

resource contributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(uai.name, contributorRoleDefinitionId, subscription().id)
  scope: subscription()
  properties: {
    principalId: uai.outputs.principalId
    roleDefinitionId: contributorRoleDefinitionId
    principalType: 'ServicePrincipal'
  }
}
