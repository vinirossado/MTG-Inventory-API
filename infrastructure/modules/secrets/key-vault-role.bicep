param keyVaultName string
param principalType string = 'ServicePrincipal'
param roleDefinitionId string = '4633458b-17de-408a-b874-0445c86b69e6'
param principalIds array

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource keyVaultRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for principalId in principalIds: {
  name: guid(keyVault.id, principalId, roleDefinitionId)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleDefinitionId)
    principalId: principalId
    principalType: principalType
  }
}]

output roleAssignmentIds array = [for i in range(0, length(principalIds)): keyVaultRoleAssignment[i].id]

