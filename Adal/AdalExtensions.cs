using Digioma.Office365.Client.Claims;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Adal
{
    public static class AdalExtensions
    {
        public static AuthenticationContext CreateAuthenticationContext(this IIdentity identity)
        {
            var cache = new AdalTokenCache(identity.NameIdentifier());
            AuthenticationContext authContext = new AuthenticationContext(identity.Authority(), cache);

            return authContext;
        }

        public static AuthenticationContext CreateAuthenticationContext(this IPrincipal user)
        {
            return user.Identity.CreateAuthenticationContext();
        }



        public static async Task<AuthenticationResult> AcquireTokenAsync(this AuthenticationContext authContext, string resourceId)
        {
            return await authContext.AcquireTokenAsync(
                resourceId,
                new ClientCredential(AppSettings.ClientId, AppSettings.AppKey)
            );
        }

        public static async Task<AuthenticationResult> AcquireTokenSilentAsync(this AuthenticationContext authContext, string resourceId)
        {
            return await authContext.AcquireTokenSilentAsync(
                resourceId, 
                new ClientCredential(AppSettings.ClientId, AppSettings.AppKey), 
                new UserIdentifier(ClaimsPrincipal.Current.ObjectIdentifier(), UserIdentifierType.UniqueId)
            );
        }

        public static async Task<AuthenticationResult> AcquireTokenSilentAsync(this IIdentity identity, string resourceId)
        {
            var authContext = identity.CreateAuthenticationContext();
            return await authContext.AcquireTokenSilentAsync(
                resourceId,
                new ClientCredential(AppSettings.ClientId, AppSettings.AppKey),
                new UserIdentifier(identity.ObjectIdentifier(), UserIdentifierType.UniqueId)
            );
        }



        public static async Task<AuthenticationResult> AcquireDiscoveryServiceTokenSilentAsync(this AuthenticationContext authContext)
        {
            return await authContext.AcquireTokenSilentAsync(AppSettings.DiscoveryServiceResourceId);
        }

        public static async Task<AuthenticationResult> AcquireDiscoveryServiceTokenSilentAsync(this IIdentity identity)
        {
            return await identity
                .CreateAuthenticationContext()
                .AcquireDiscoveryServiceTokenSilentAsync();
        }

        public static async Task<AuthenticationResult> AcquireDiscoveryServiceTokenSilentAsync(this IPrincipal user)
        {
            return await user.Identity.AcquireDiscoveryServiceTokenSilentAsync();
        }



        public static async Task<AuthenticationResult> AcquireGraphTokenSilentAsync(this AuthenticationContext authContext)
        {
            return await authContext.AcquireTokenSilentAsync(AppSettings.GraphResourceId);
        }

        public static async Task<AuthenticationResult> AcquireGraphTokenSilentAsync(this IIdentity identity)
        {
            return await identity.CreateAuthenticationContext().AcquireGraphTokenSilentAsync();
        }

        public static async Task<AuthenticationResult> AcquireGraphTokenSilentAsync(this IPrincipal user)
        {
            return await user.Identity.AcquireGraphTokenSilentAsync();
        }
    }
}
