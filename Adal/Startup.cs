using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using System.Configuration;
using System.Runtime.InteropServices;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;
using Digioma.Office365.Client.Claims;

namespace Digioma.Office365.Client.Adal
{
    public static class Startup
    {
        public static void ConfigureAuth(
            IAppBuilder app,
            [Optional] Func<AuthorizationCodeReceivedNotification, Task> authorizationCodeReceived,
            [Optional] Func<RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>, Task> redirectToIdentityProvider,
            [Optional] Func<AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>, Task> authenticationFailed
        )
        {
            var options = new OpenIdConnectAuthenticationOptions()
            {
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    AuthorizationCodeReceived = authorizationCodeReceived,
                    RedirectToIdentityProvider = redirectToIdentityProvider,
                    AuthenticationFailed = authenticationFailed
                }
            };

            if(null == options.Notifications.AuthorizationCodeReceived)
            {
                options.Notifications.AuthorizationCodeReceived = (context) =>
                {
                    CheckConfig();

                    var code = context.Code;
                    ClientCredential credential = new ClientCredential(AppSettings.ClientId, AppSettings.ClientSecret);
                    String signInUserId = context.AuthenticationTicket.Identity.NameIdentifier();//.FindFirst(ClaimTypes.NameIdentifier).Value;

                    AuthenticationContext authContext = new AuthenticationContext(AppSettings.Authority, new AdalTokenCache(signInUserId));
                    AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, AppSettings.GraphResourceId);

                    return Task.FromResult(0);
                };
            }

            ConfigureAuth(app, options);
        }

        public static void ConfigureAuth(IAppBuilder app)
        {
            ConfigureAuth(app, new OpenIdConnectAuthenticationOptions());
        }

        public static void ConfigureAuth(IAppBuilder app, OpenIdConnectAuthenticationOptions options)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            if(null == options)
            {
                options = new OpenIdConnectAuthenticationOptions();
            }

            if (string.IsNullOrEmpty(options.ClientId)) options.ClientId = AppSettings.ClientId;
            if (string.IsNullOrEmpty(options.Authority)) options.Authority = AppSettings.Authority;
            if (string.IsNullOrEmpty(options.PostLogoutRedirectUri)) options.PostLogoutRedirectUri = AppSettings.PostLogoutRedirectUri;
            if (null == options.Notifications) options.Notifications = new OpenIdConnectAuthenticationNotifications();

            if(null == options.Notifications.RedirectToIdentityProvider)
            {
                options.Notifications.RedirectToIdentityProvider = (context) =>
                {
                    // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                    // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                    // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                    string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                    context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                    context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;

                    return Task.FromResult(0);
                };
            }

            app.UseOpenIdConnectAuthentication(options);
        }

        private static void CheckConfig()
        {
            if(string.IsNullOrEmpty(AppSettings.Digioma_TokenCacheConnectionString))
            {
                throw new ConfigurationErrorsException("The 'digioma:TokenCacheConnectionString' app setting has not been defined in the appSettings configuration element. It must be specified as the name or connection string to the token cache database.");
            }
        }

    }
}
