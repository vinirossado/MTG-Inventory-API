param location string = resourceGroup().location
var uniqueId = uniqueString(resourceGroup().id)

@secure()
param pgSqlPassword string

var keyVaultName = 'kv-2g45mzh7gjumo'
var appServicePlanName = 'plan-api-2g45mzh7gjumo'

// Key Vault deployment
module keyVault 'modules/secrets/keyvault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    vaultName: keyVaultName
    location: location
  }
}

// Reference to existing App Service Plan in another resource group
resource existingAppServicePlan 'Microsoft.Web/serverfarms@2021-03-01' existing = {
  name: appServicePlanName
  scope: resourceGroup('urlshortener-dev')
}

// API Service deployment
module apiService 'modules/compute/appservice.bicep' = {
  name: 'apiDeployment'
  params: {
    appName: 'mtg-${uniqueId}'
    serverFarmId: existingAppServicePlan.id
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
