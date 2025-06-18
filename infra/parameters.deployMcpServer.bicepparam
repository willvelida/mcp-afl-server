using 'deployMcpServer.bicep'

param baseName = 'aflmcpserver'
param containerRegistryName = ''
param uaiName = 'uai-aflmcpserver-prod'
param imageName = ''
param tags = {
  Owner: 'velidawill@microsoft.com'
  Environment: 'prod'
}
