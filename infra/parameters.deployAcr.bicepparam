using 'deployAcr.bicep'

param baseName = 'aflmcpserver'
param location = 'australiaeast'
param tags = {
  Owner: 'velidawill@microsoft.com'
  Environment: 'prod'
}
