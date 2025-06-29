@description('The name of the API Management Service')
param apimName string

@description('The name of the Container App hosting the MCP Endpoints')
param containerAppName string

@description('The resource ID of the MCP Entra App')
param mcpEntraAppId string

@description('The Tenant ID of the MCP Entra App')
param mcpEntraTenantId string

resource apim 'Microsoft.ApiManagement/service@2024-06-01-preview' existing = {
  name: apimName
}

resource mcpServer 'Microsoft.App/containerApps@2025-02-02-preview' existing = {
  name: containerAppName
}

resource mcpTenantIdNamedValue 'Microsoft.ApiManagement/service/namedValues@2024-06-01-preview' = {
  name: 'McpTenantId'
  parent: apim
  properties: {
    displayName: 'McpTenantId'
    value: mcpEntraTenantId
    secret: false
  }
}

resource mcpClientIdNamedValue 'Microsoft.ApiManagement/service/namedValues@2024-06-01-preview' = {
  name: 'McpClientId'
  parent: apim
  properties: {
    displayName: 'McpClientId'
    value: mcpEntraAppId
    secret: false
  }
}

resource apimGatewayUrlNamedValue 'Microsoft.ApiManagement/service/namedValues@2024-06-01-preview' = {
  name: 'APIMGatewayURL'
  parent: apim
  properties: {
    displayName: 'APIMGatewayURL'
    value: apim.properties.gatewayUrl
    secret: false
  }
}

resource mcpApi 'Microsoft.ApiManagement/service/apis@2024-06-01-preview' = {
  name: 'mcp'
  parent: apim
  properties: {
    path: 'mcp'
    displayName: 'AFL MCP API'
    description: 'MCP API Endpoints for the AFL MCP Server'
    subscriptionRequired: false
    protocols: [
      'https'
    ]
    serviceUrl: 'https://${mcpServer.properties.configuration.ingress.fqdn}'
  }
}

resource mcpApiPolicy 'Microsoft.ApiManagement/service/apis/policies@2024-06-01-preview' = {
  name: 'policy'
  parent: mcpApi
  properties: {
    value: loadTextContent('mcp-api.policy.xml')
    format: 'rawxml'
  }
  dependsOn: [
    apimGatewayUrlNamedValue
    mcpTenantIdNamedValue
    mcpClientIdNamedValue
  ]
}

resource mcpSseOperation 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  name: 'mcp-sse'
  parent: mcpApi
  properties: {
    displayName: 'MCP SSE Endpoint'
    method: 'GET'
    urlTemplate: '/sse'
    description: 'Server-Sent Events endpoint for AFL MCP Server' 
  }
}

resource mcpMessageOperation 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  name: 'mcp-message'
  parent: mcpApi
  properties: {
    displayName: 'MCP Message Endpoint'
    method: 'POST'
    urlTemplate: '/message'
    description: 'Message endpoint for AFL MCP Server'
  }
}

resource mcpHealthCheckOperation 'Microsoft.ApiManagement/service/apis/operations@2024-06-01-preview' = {
  name: 'mcp-health-check'
  parent: mcpApi
  properties: {
    displayName: 'MCP Health Check'
    method: 'GET'
    urlTemplate: '/api/healthz'
    description: 'Health Check endpoint for AFL MCP Server'
  }
}

resource mcpStreamableGetOperation 'Microsoft.ApiManagement/service/apis/operations@2023-05-01-preview' = {
  parent: mcpApi
  name: 'mcp-streamable-get'
  properties: {
    displayName: 'MCP Streamable GET Endpoint'
    method: 'GET'
    urlTemplate: '/'
    description: 'Streamable GET endpoint for AFL MCP Server'
  }
}

resource mcpStreamablePostOperation 'Microsoft.ApiManagement/service/apis/operations@2023-05-01-preview' = {
  parent: mcpApi
  name: 'mcp-streamable-post'
  properties: {
    displayName: 'MCP Streamable POST Endpoint'
    method: 'POST'
    urlTemplate: '/'
    description: 'Streamable POST endpoint for AFL MCP Server'
  }
}

// Create the PRM (Protected Resource Metadata) endpoint - RFC 9728
resource mcpPrmOperation 'Microsoft.ApiManagement/service/apis/operations@2023-05-01-preview' = {
  parent: mcpApi
  name: 'mcp-prm'
  properties: {
    displayName: 'Protected Resource Metadata'
    method: 'GET'
    urlTemplate: '/prm'
    description: 'Protected Resource Metadata endpoint (RFC 9728)'
  }
}

// Apply specific policy for the PRM endpoint (anonymous access)
resource mcpPrmPolicy 'Microsoft.ApiManagement/service/apis/operations/policies@2023-05-01-preview' = {
  parent: mcpPrmOperation
  name: 'policy'
  properties: {
    format: 'rawxml'
    value: loadTextContent('mcp-prm.policy.xml')
  }
  dependsOn: [
    apimGatewayUrlNamedValue
    mcpTenantIdNamedValue
    mcpClientIdNamedValue
  ]
}

@description('The resource ID of the deployed API in APIM')
output apiId string = mcpApi.id

@description('The App ID of the MCP API')
output mcpAppId string = mcpEntraAppId

@description('The Tenant ID of the MCP API')
output mcpAppTenantId string = mcpEntraTenantId
