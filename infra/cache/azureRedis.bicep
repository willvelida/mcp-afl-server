@description('Base name used for all resources')
param baseName string

@description('The Azure region where this Azure Redis Cache resource will be deployed. Default is the resource group location')
param location string = resourceGroup().location

@description('The name of the environment that this Azure Redis Cache will be deployed to. Default value is "prod"')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string = 'prod'

@description('The tags that will be applied to the Azure Redis Cache resource')
param tags object

var redisCacheName string = 'cache-${baseName}-${environmentName}'

resource redisCache 'Microsoft.Cache/redis@2024-11-01' = {
  name: redisCacheName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'Basic'
      capacity: 0
      family: 'C'
    }
  }
}

@description('The resource ID of the deployed Azure Redis Cache')
output id string = redisCache.id

@description('The name of the deployed Azure Redis Cache')
output name string = redisCache.name
