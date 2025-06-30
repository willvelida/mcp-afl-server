extension microsoftGraphV1

@description('Base name used for all resources')
param baseName string

@description('The name of the environment that this Entra App will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The Tenant ID where the application is registered')
param tenantId string = tenant().tenantId

@description('The name of the User-Assigned Identity that this Entra App will use')
param uaiName string

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2025-01-31-preview' existing = {
  name: uaiName
}

var appName = 'app-${baseName}-${environmentName}'
var loginEndpoint = environment().authentication.loginEndpoint
var issuer = '${loginEndpoint}${tenantId}/v2.0'

resource mcpEntraApp 'Microsoft.Graph/applications@v1.0' = {
  displayName: appName
  uniqueName: appName
  api: {
    oauth2PermissionScopes: [
      {
        id: guid(appName, 'user_impersonate')
        adminConsentDescription: 'Allows the App to access MCP resources on behalf of the signed-in user'
        adminConsentDisplayName: 'Access MCP resources'
        isEnabled: true
        type: 'User'
        userConsentDescription: 'Allows the app to acces MCP resources on your behalf'
        userConsentDisplayName: 'Access MCP resources'
        value: 'user_impersonate'
      }
    ]
    requestedAccessTokenVersion: 2
  }
  requiredResourceAccess: [
    {
      resourceAppId: '00000003-0000-0000-c000-000000000000' // Microsoft Graph
      resourceAccess: [
        {
          id: 'e1fe6dd8-ba31-4d61-89e7-88639da4683d' // User.Read
          type: 'Scope'
        }
      ]
    }
  ]

  resource fic 'federatedIdentityCredentials@v1.0' = {
    name: '${mcpEntraApp.uniqueName}/msiAsFic'
    description: 'Trust the user-assigned MI as a credential for the MCP app'
    audiences: [
      'api://AzureADTokenExchange'
    ]
    issuer: issuer
    subject: uai.properties.principalId
  }
}

resource microsoftGraphServicePrincipal 'Microsoft.Graph/servicePrincipals@v1.0' existing = {
  appId: '00000003-0000-0000-c000-000000000000' // Microsoft Graph
}

resource appRegistrationServicePrincipal 'Microsoft.Graph/servicePrincipals@v1.0' = {
  appId: mcpEntraApp.appId
}

resource grants 'Microsoft.Graph/oauth2PermissionGrants@v1.0' = {
  clientId: appRegistrationServicePrincipal.id
  consentType: 'AllPrincipals'
  resourceId: microsoftGraphServicePrincipal.id
  scope: 'User.Read'
}

@description('The resource ID of the deployed Entra App')
output mcpAppId string = mcpEntraApp.appId

@description('The Tenant ID of the deployed Entra App')
output mcpAppTenantId string = tenantId
