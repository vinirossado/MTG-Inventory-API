param name string
param location string
param keyVaultName string
param administratorLogin string
@secure()
param administratorPassword string

resource postgresqlServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-12-01-preview' = {
  name: name
  location: location
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    version: '16'
    storage: {
      storageSizeGB: 32
    }
    backup: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    network: {
      publicNetworkAccess: 'Enabled'
    }
  }
}

// Define the database as a separate resource under the PostgreSQL server
resource database 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-12-01-preview' = {
  parent: postgresqlServer
  name: 'inventory'
}

// Firewall rule to allow Azure services
resource firewallRule 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-12-01-preview' = {
  parent: postgresqlServer
  name: 'allow-azure-services'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Reference existing Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

// Store PostgreSQL connection string in Key Vault
resource postgresDbConnectionString 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: '${keyVaultName}/Postgres-ConnectionString'
  properties: {
    value: 'Server=${postgresqlServer.name}.postgres.database.azure.com;Database=inventory;Port=5432;User Id=${administratorLogin};Password=${administratorPassword};Ssl Mode=Require;'
  }
  dependsOn: [
    keyVault
  ]
}

output serverId string = postgresqlServer.id
