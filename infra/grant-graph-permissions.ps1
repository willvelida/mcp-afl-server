# grant-graph-permissions.ps1
param(
    [Parameter(Mandatory = $true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory = $true)]
    [string]$UAIName
)

# Get the Principal ID
$uaiPrincipalId = (az identity show --resource-group $ResourceGroupName --name $UAIName --query principalId -o tsv)
Write-Host "User-Assigned Identity Principal ID: $uaiPrincipalId"

# Microsoft Graph App ID
$graphAppId = "00000003-0000-0000-c000-000000000000"

# Get Microsoft Graph Service Principal
$graphSp = (az ad sp list --filter "appId eq '$graphAppId'" --query "[0].id" -o tsv)
Write-Host "Microsoft Graph Service Principal ID: $graphSp"

# Required permissions
$permissions = @(
    @{
        Id = "1bfefb4e-e0b5-418b-a88f-73c46d2cc8e9"
        Value = "Application.ReadWrite.All"
    },
    @{
        Id = "8e8e4742-1d95-4f68-9d56-6ee75648c72a" 
        Value = "DelegatedPermissionGrant.ReadWrite.All"
    },
    @{
        Id = "06b708a9-e830-4db3-a914-8e69da51d44f"
        Value = "AppRoleAssignment.ReadWrite.All"
    }
)

# Grant each permission
foreach ($permission in $permissions) {
    Write-Host "Granting $($permission.Value) to managed identity..."
    
    $bodyObj = @{
        principalId = $uaiPrincipalId
        resourceId = $graphSp
        appRoleId = $permission.Id
    }
    
    # Convert to JSON and save to a temporary file to avoid escaping issues
    $bodyJson = $bodyObj | ConvertTo-Json -Compress
    $tempFile = [System.IO.Path]::GetTempFileName()
    Set-Content -Path $tempFile -Value $bodyJson -Encoding UTF8
    
    # Use the file as input to az rest
    try {
        $result = az rest --method POST `
            --uri "https://graph.microsoft.com/v1.0/servicePrincipals/$uaiPrincipalId/appRoleAssignments" `
            --headers "Content-Type=application/json" `
            --body "@$tempFile"
        
        Write-Host "Successfully granted $($permission.Value)" -ForegroundColor Green
        Write-Host $result
    }
    catch {
        Write-Host "Error granting $($permission.Value): $_" -ForegroundColor Red
    }
    finally {
        # Clean up the temp file
        Remove-Item -Path $tempFile -ErrorAction SilentlyContinue
    }
}

Write-Host "Graph permissions configuration completed." -ForegroundColor Green