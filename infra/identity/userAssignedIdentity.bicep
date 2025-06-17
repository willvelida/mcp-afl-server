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

@description('The GitHub Organization for the federated credentials')
param githubOrganization string

@description('The GitHub Repository for the federated credentials')
param githubRepositoryName string

var uaiName = 'uai-${baseName}-${environmentName}'
var issuer = 'https://token/actions.githubusercontent.com'
var azureADaudience = 'api://AzureADTokenExchange'

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2025-01-31-preview' = {
  name: uaiName
  location: location
  tags: tags
}

resource gitHubEnvCreds 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2024-11-30' = {
  name: '${githubOrganization}-${githubRepositoryName}-${environmentName}'
  parent: uai
  properties: {
    audiences: [
      azureADaudience
    ]
    issuer: issuer
    subject: 'repo:${githubOrganization}/${githubRepositoryName}:environment:${environmentName}'
  }
}

resource gitHubPrCreds 'Microsoft.ManagedIdentity/userAssignedIdentities/federatedIdentityCredentials@2024-11-30' = {
  name: '${githubOrganization}-${githubRepositoryName}-pr'
  parent: uai
  properties: {
    audiences: [
      azureADaudience
    ]
    issuer: issuer
    subject: 'repo:${githubOrganization}/${githubRepositoryName}:pull_request'
  }
  dependsOn: [
    gitHubEnvCreds // Concurrent Federated Identity Credential writes under the same managed identity are not supported.s
  ]
}

@description('The resource ID of the deployed user-assigned identity')
output id string = uai.id

@description('The name of the deployed user-assigned identity')
output name string = uai.name
