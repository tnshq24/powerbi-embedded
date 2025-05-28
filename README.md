# Power BI Embedded Web Application

A complete ASP.NET Core MVC application demonstrating how to embed Power BI reports using the "embed for your customers" (app owns data) scenario. This application allows customers to view Power BI reports without requiring Power BI licenses or user authentication.

## Features

- **Service Principal Authentication**: Uses Azure AD app registration for secure API access
- **Embed for Customers**: Implements the "app owns data" pattern
- **Responsive Design**: Modern Bootstrap-based UI
- **Real-time Embedding**: Dynamic report embedding with JavaScript API
- **Error Handling**: Comprehensive error handling and logging
- **Clean Architecture**: Well-structured MVC pattern with separation of concerns

## Prerequisites

Before running this application, ensure you have:

1. **.NET 9.0 SDK** installed
   - Download from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download)
   - Verify installation: `dotnet --version`

2. **Power BI Pro or Premium license** for the service account
3. **Azure AD tenant** with administrative access
4. **Power BI workspace** with reports to embed

## Quick Start

### 1. Clone the Repository

```bash
git clone <repository-url>
cd powerbi-embedded
```

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Configure Azure AD Application

#### Step 1: Register Azure AD Application
1. Go to [Azure Portal](https://portal.azure.com) → Azure Active Directory → App registrations
2. Click "New registration"
3. Enter application name (e.g., "PowerBI-Embedded-App")
4. Set redirect URI: `https://localhost:5001/signin-oidc`
5. Click "Register"

#### Step 2: Configure API Permissions
1. Go to "API permissions" in your app registration
2. Click "Add a permission" → "Power BI Service"
3. Select "Delegated permissions" and add:
   - `Report.Read.All`
   - `Dataset.Read.All`
   - `Workspace.Read.All`
4. Click "Grant admin consent"

#### Step 3: Create Client Secret
1. Go to "Certificates & secrets"
2. Click "New client secret"
3. Add description and set expiration
4. **Copy the secret value immediately** (you won't see it again)

#### Step 4: Note Required IDs
Copy these values from your app registration:
- **Application (client) ID**
- **Directory (tenant) ID**
- **Client secret value**

### 4. Configure Power BI

#### Step 1: Get Workspace and Report IDs
1. Go to [Power BI Service](https://app.powerbi.com)
2. Navigate to your workspace
3. Copy the Workspace ID from the URL: `https://app.powerbi.com/groups/{workspace-id}/`
4. Open a report and copy the Report ID from the URL: `https://app.powerbi.com/groups/{workspace-id}/reports/{report-id}/`

#### Step 2: Add Service Principal to Workspace
1. In Power BI Service, go to workspace settings
2. Go to "Access" tab
3. Add your Azure AD application as a "Member" or "Admin"

### 5. Update Configuration

Edit `appsettings.json` with your values:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-company-domain.com",
    "TenantId": "YOUR-TENANT-ID-HERE",
    "ClientId": "YOUR-CLIENT-ID-HERE",
    "ClientSecret": "YOUR-CLIENT-SECRET-HERE",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  },
  "PowerBi": {
    "ServiceRootUrl": "https://api.powerbi.com/",
    "WorkspaceId": "YOUR-WORKSPACE-ID-HERE",
    "ReportId": "YOUR-REPORT-ID-HERE"
  }
}
```

### 6. Install HTTPS Certificate (First Time Only)

```bash
dotnet dev-certs https --trust
```

### 7. Run the Application

```bash
dotnet run
```

The application will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## Project Structure

```
PowerBiEmbedded/
├── Controllers/
│   └── HomeController.cs          # Main controller with Index and Embed actions
├── Services/
│   └── PowerBiServiceApi.cs       # Power BI API service and view models
├── Models/
│   └── ErrorViewModel.cs          # Error handling model
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml           # Home page with setup instructions
│   │   └── Embed.cshtml           # Report embedding page
│   ├── Shared/
│   │   └── _Layout.cshtml         # Main layout template
│   ├── _ViewImports.cshtml        # Global view imports
│   └── _ViewStart.cshtml          # View start configuration
├── wwwroot/
│   ├── css/
│   │   └── site.css               # Custom styles
│   └── js/
│       ├── site.js                # General JavaScript
│       └── embed.js               # Power BI embedding logic
├── Properties/
│   └── launchSettings.json        # Development server settings
├── appsettings.json               # Application configuration
├── appsettings.Development.json   # Development-specific settings
├── Program.cs                     # Application entry point
├── Startup.cs                     # Service configuration and middleware
└── PowerBiEmbedded.csproj        # Project file with dependencies
```

## Key Components

### PowerBiServiceApi.cs
- Handles Azure AD authentication using service principal
- Manages Power BI API calls
- Generates embed tokens for reports
- Returns embedding data to controllers

### embed.js
- Implements Power BI JavaScript API
- Configures report embedding with proper token type
- Handles responsive resizing
- Manages report events (loaded, rendered, error)

### Startup.cs
- Configures Microsoft Identity Web for service principal auth
- Sets up dependency injection
- Configures middleware pipeline

## Troubleshooting

### Common Issues

#### 1. AADSTS7000215: Invalid client secret
- **Cause**: Using client secret ID instead of the actual secret value
- **Solution**: Use the secret **value** from Azure portal, not the ID

#### 2. Certificate Issues
- **Cause**: HTTPS development certificate not trusted
- **Solution**: Run `dotnet dev-certs https --trust`

#### 3. Port Already in Use
- **Cause**: Another instance is running
- **Solution**: Stop other instances or change ports in `launchSettings.json`

#### 4. Report Not Loading
- **Cause**: Service principal not added to workspace
- **Solution**: Add your Azure AD app to the Power BI workspace with appropriate permissions

#### 5. Token Acquisition Errors
- **Cause**: Incorrect API permissions or admin consent not granted
- **Solution**: Verify API permissions and ensure admin consent is granted

### Debug Logging

Enable detailed logging by updating `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Identity": "Debug"
    }
  }
}
```

## Security Considerations

1. **Never commit secrets**: Use environment variables or Azure Key Vault for production
2. **Rotate secrets regularly**: Update client secrets before expiration
3. **Principle of least privilege**: Grant only necessary Power BI permissions
4. **Monitor access**: Review Azure AD sign-in logs regularly
5. **Use HTTPS**: Always use HTTPS in production environments

## Deployment

### Azure App Service

1. Create an Azure App Service
2. Configure application settings (instead of appsettings.json):
   - `AzureAd:TenantId`
   - `AzureAd:ClientId`
   - `AzureAd:ClientSecret`
   - `PowerBi:WorkspaceId`
   - `PowerBi:ReportId`
3. Deploy using Visual Studio, Azure DevOps, or GitHub Actions

### Environment Variables

For production, use environment variables instead of appsettings.json:

```bash
export AzureAd__TenantId="your-tenant-id"
export AzureAd__ClientId="your-client-id"
export AzureAd__ClientSecret="your-client-secret"
export PowerBi__WorkspaceId="your-workspace-id"
export PowerBi__ReportId="your-report-id"
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Resources

- [Power BI Embedded Documentation](https://docs.microsoft.com/en-us/power-bi/developer/embedded/)
- [Power BI JavaScript API](https://github.com/Microsoft/PowerBI-JavaScript)
- [Microsoft Identity Web Documentation](https://docs.microsoft.com/en-us/azure/active-directory/develop/microsoft-identity-web)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)

## Support

For issues and questions:
1. Check the troubleshooting section above
2. Review Power BI Embedded documentation
3. Create an issue in this repository
4. Contact Microsoft Support for Power BI-specific issues 