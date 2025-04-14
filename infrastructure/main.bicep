param location string = resourceGroup().location
var uniqueId = uniqueString(resourceGroup().id)

@secure()
param pgSqlPassword string

var keyVaultName = 'kv-${uniqueId}'
var appServicePlanName = 'plan-api-${uniqueId}'

// Key Vault deployment
module keyVault 'modules/secrets/keyvault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    vaultName: keyVaultName
    location: location
  }
}

// App Service Plan deployment
module appServicePlan 'modules/compute/appserviceplan.bicep' = {
  name: 'appServicePlanDeployment'
  params: {
    appServicePlanName: appServicePlanName
    location: location
  }
}

// API Service deployment
module apiService 'modules/compute/appservice.bicep' = {
  name: 'apiDeployment'
  params: {
    appName: 'mtg-${uniqueId}'
    serverFarmId: appServicePlan.outputs.id
    location: location
    keyVaultName: keyVaultName
    linuxFxVersion: 'DOTNETCORE|9.0'
    appSettings: [
      {
        name: 'DatabaseName'
        value: 'inventory'
      }
    ]
  }
  dependsOn: [
    keyVault
  ]
}

// Key Vault Role Assignment
module keyVaultRoleAssignment 'modules/secrets/key-vault-role.bicep' = {
  name: 'keyVaultRoleAssignmentDeployment'
  params: {
    keyVaultName: keyVaultName
    principalIds: [
      apiService.outputs.principalId
    ]
    roleDefinitionId: '4633458b-17de-408a-b874-0445c86b69e6' // Key Vault Secrets User role
  }
  dependsOn: [
    keyVault
    apiService
  ]
}

// PostgreSQL deployment
module postgres 'modules/storage/postgresql.bicep' = {
  name: 'postgresDeployment'
  params: {
    name: 'postgresql-${uniqueId}'
    location: location
    administratorLogin: 'adminuser'
    administratorPassword: pgSqlPassword
    keyVaultName: keyVaultName
  }
  dependsOn: [
    keyVault
  ]
}
