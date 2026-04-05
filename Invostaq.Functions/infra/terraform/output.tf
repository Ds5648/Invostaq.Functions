output "function_app_name" {
  description = "Name of the Function App"
  value       = azurerm_windows_function_app.func.name
}

output "function_app_hostname" {
  description = "Default hostname of the Function App"
  value       = azurerm_windows_function_app.func.default_hostname
}

output "sql_server_fqdn" {
  description = "Fully qualified domain name of SQL Server"
  value       = azurerm_mssql_server.sql.fully_qualified_domain_name
}

output "storage_account_name" {
  description = "Name of the Storage Account"
  value       = azurerm_storage_account.sa.name
}