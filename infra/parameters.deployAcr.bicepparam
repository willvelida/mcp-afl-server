using 'deployAcr.bicep'

param baseName = 'aflmcpserver'
param location = 'australiaeast'
param githubOrganization = 'willvelida'
param githubRepositoryName = 'mcp-afl-server'
param tags = {
  Owner: 'velidawill@microsoft.com'
  Environment: 'prod'
}
