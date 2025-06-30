using 'deployMcpServer.bicep'

param baseName = 'aflmcpserver'
param containerRegistryName = 'arcaflmcpserverprod'
param uaiName = 'uai-aflmcpserver-prod'
param imageName = ''
param emailAddress = ''
param publisherName = 'Will Velida'
param tags = {
  Owner: 'velidawill@microsoft.com'
  Environment: 'prod'
}
