@description('Base name used for all resources')
param baseName string

@description('The Azure region where this Container App resource will be deployed. Default is the resource group location')
param location string = resourceGroup().location

@description('The name of the environment that this Container App will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The tags that will be applied to the Container App resource')
param tags object

@description('The resource ID of the Container App Environment that this Container App will be deployed to')
param containerAppEnvironmentId string

@description('The Container Registry that this Container App will use to pull images from')
param containerRegistryName string

@description('The name of the user-assigned managed identity that this Container App will use')
param uaiName string

@description('The name of the Application Insights workspace that this Container App will use')
param appInsightsName string

@description('The name of the container image that this Container App will use. Passed through in GitHub Action')
param imageName string

// EXISTING RESOURCES
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2025-04-01' existing = {
  name: containerRegistryName
}

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2025-01-31-preview' existing = {
  name: uaiName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

// VARIABLES
var containerAppName = 'app-${baseName}-${environmentName}'

// MCP SERVER INFRASTRUCTURE
resource mcpServer 'Microsoft.App/containerApps@2025-01-01' = {
  name: containerAppName
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${uai.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: [
        {
          server: containerRegistry.properties.loginServer
          identity: uai.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: baseName
          image: imageName
          env: [
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: appInsights.properties.ConnectionString
            }
            {
              name: 'managedidentityclientid'
              value: uai.properties.clientId
            }
          ]
          probes: [
            {
              type: 'Liveness'
              httpGet: {
                port: 8080
                path: '/api/healthz'
                scheme: 'HTTP'
              }
              initialDelaySeconds: 10
              periodSeconds: 5
              failureThreshold: 30
              timeoutSeconds: 2
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5i'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
        rules: [
          {
            name: 'http-scale-rule'
            http: {
              metadata: {
                concurrentRequests: '100'
              }
            }
          }
        ]
      }
    }
  }
}
