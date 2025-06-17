targetScope='subscription'

@description('The base name of the Application')
param baseName string

@description('The Tags applied to all resources')
param tags object

@description('Location to deploy Azure resources')
@allowed([
  'australiaeast'
  'australiasoutheast'
  'newzealandnorth'
])
param location string

var rgName = 'rg-${baseName}'

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
  }
}
