using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;

namespace Digioma.Office365.Client
{
    public class AppSettings
    {
        private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId"] ?? ConfigurationManager.AppSettings["ida:ClientID"];
        private static string _clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"] ?? ConfigurationManager.AppSettings["ida:AppKey"] ?? ConfigurationManager.AppSettings["ida:Password"];

        private static string _tenantId = ConfigurationManager.AppSettings["ida:TenantId"] ?? "common"; // Default to the fixed "tenant" that is used by multi-tenant applications.
        private static string _authorizationUri = ConfigurationManager.AppSettings["ida:AADInstance"] ?? "https://login.microsoftonline.com";
        //private static string _authority = "https://login.windows.net/{0}/";

        private static string _graphResourceId = "https://graph.windows.net";
        private static string _discoverySvcResourceId = "https://api.office.com/discovery/";
        private static string _discoverySvcEndpointUri = "https://api.office.com/discovery/v1.0/";

        private static string _postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string _digioma_TokenCacheConnectionString = ConfigurationManager.AppSettings["digioma:TokenCacheConnectionString"];

        public static string ClientId
        {
            get { return _clientId; }
        }

        public static string ClientSecret
        {
            get { return _clientSecret; }
        }

        public static string TenantId
        {
            get
            {
                if (string.IsNullOrEmpty(_tenantId)) throw new ConfigurationErrorsException("The 'ida:TenantId' app setting has not been configured.");
                return _tenantId;
            }
        }

        public static string PostLogoutRedirectUri
        {
            get { return _postLogoutRedirectUri; }
        }

        public static string AuthorizationUri
        {
            get { return _authorizationUri; }
        }

        public static string Authority
        {
            get { return String.Format("{0}/{1}", AuthorizationUri, TenantId); }
        }

        public static string GraphResourceId
        {
            get { return _graphResourceId; }
        }

        public static string DiscoveryServiceResourceId
        {
            get { return _discoverySvcResourceId; }
        }

        public static Uri DiscoveryServiceEndpointUri
        {
            get { return new Uri(_discoverySvcEndpointUri); }
        }


        public static string Digioma_TokenCacheConnectionString
        {
            get { return _digioma_TokenCacheConnectionString; }
        }


        public static ClientCredential CreateClientCredential()
        {
            return new ClientCredential(AppSettings.ClientId, AppSettings.ClientSecret);
        }

        public static AuthenticationContext CreateAuthorityAuthenticationContext()
        {
            return new AuthenticationContext(AppSettings.Authority, true);
        }
    }

}
