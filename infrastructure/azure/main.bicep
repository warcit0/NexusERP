@description('The location of all resources.')
param location string = resourceGroup().location

@description('The base name for resources.')
param baseName string = 'nexuserp'

@description('The environment name (e.g. dev, prod)')
param env string = 'dev'

var uniqueName = '${baseName}${env}${uniqueString(resourceGroup().id)}'

// SQL Server & Database
resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: '${uniqueName}-sql'
  location: location
  properties: {
    administratorLogin: 'sqladmin'
    administratorLoginPassword: 'Password12345!' // En producción, usar param + KeyVault
    version: '12.0'
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: 'NexusERP'
  location: location
  sku: {
    name: 'GP_S_Gen5_1'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
  }
}

// App Service Plan & App Service
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${uniqueName}-plan'
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
}

resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: '${uniqueName}-api'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v9.0' // O 'v10.0'
    }
  }
}

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: substring('${uniqueName}-kv', 0, 24)
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: []
  }
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${uniqueName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

// Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: substring(replace('${uniqueName}st', '-', ''), 0, 24)
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

// Redis Cache
resource redisCache 'Microsoft.Cache/redis@2022-06-01' = {
  name: '${uniqueName}-redis'
  location: location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
  }
}
