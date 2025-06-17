# Deploying this sample

- [Deploying Resource Group, Managed Identity, and Azure Container Registry](#deploying-resource-group-managed-identity-and-azure-container-registry)
- [Issues and workarounds](#issues-and-workarounds)

## Deploying Resource Group, Managed Identity, and Azure Container Registry.

The ```deployAcr.bicep``` template is deployed at the subscription level. To deploy the template, run the following AZ CLI command:

```bash
az deployment sub create --location <location> --template-file .\deployAcr.bicep --parameters .\parameters.deployAcr.bicepparam
```

## Issues and workarounds

The following issues and workarounds were discovered when deploying this sample:

- [Concurrent Federated Identity Credentials writes under the same managed identity are not supported.](#concurrent-federated-identity-credentials-writes-under-the-same-managed-identity-are-not-supported)

### Concurrent Federated Identity Credentials writes under the same managed identity are not supported.

When attempting the deploy the User-Assigned Managed Identity with Federated Credentials, the following error was thrown:

```bash
"code":"ConcurrentFederatedIdentityCredentialsWritesForSingleManagedIdentity","message":"Too many Federated Identity Credentials are written concurrently for the managed identity '{MANAGED_IDENTITY_RESOURCE_ID}'. Concurrent Federated Identity Credentials writes under the same managed identity are not supported.
```

To overcome this, I used the `dependsOn` property to enforce a explicit dependency:

```bicep
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
```

More information:

- [Resource dependencies in Bicep](http://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/resource-dependencies)