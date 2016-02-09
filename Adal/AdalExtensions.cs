using Digioma.Office365.Client.Claims;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
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
        /// <summary>
        /// Returns an authentication context using a token cache created from the current identity. Assumes that the identity
        /// is a <see cref="ClaimsIdentity"/> identity instance.
        /// </summary>
        public static AuthenticationContext CreateAuthenticationContext(this IIdentity identity)
        {
            var cache = new AdalTokenCache(identity.NameIdentifier());
            AuthenticationContext authContext = new AuthenticationContext(identity.Authority(), cache);

            return authContext;
        }

        /// <summary>
        /// Returns an authentication context using a token cache created from the current user. Assumes that the user instance
        /// is a <see cref="ClaimsPrincipal"/> instance.
        /// </summary>
        public static AuthenticationContext CreateAuthenticationContext(this IPrincipal user)
        {
            return user.Identity.CreateAuthenticationContext();
        }



        /// <summary>
        /// Creates an <see cref="ActiveDirectoryClient"/> instance in app-only mode from the current authentication context.
        /// </summary>
        /// <remarks>
        /// To use the returned instance you must give the configured application the proper application permissions to to the
        /// <c>Windows Azure Active Directory</c> application, <c>Read directory data</c> at a minimum.
        /// </remarks>
        public static ActiveDirectoryClient CreateAppOnlyActiveDirectoryClient(this AuthenticationContext authContext)
        {
            var root = new Uri(new Uri(AppSettings.GraphResourceId), AppSettings.TenantId);
            return new ActiveDirectoryClient(root, async () => (await authContext.AcquireAppOnlyTokenAsync()).AccessToken);
        }



        public static ITenantDetail CurrentTenantDetails(this ActiveDirectoryClient adClient)
        {
            return AsyncHelper.RunSync(() => adClient.CurrentTenantDetailsAsync());
        }

        public static async Task<ITenantDetail> CurrentTenantDetailsAsync(this ActiveDirectoryClient adClient)
        {
            return await adClient.TenantDetails.Where(x => x.ObjectId == AppSettings.TenantId).ExecuteSingleAsync();
        }



        public static AuthenticationResult AcquireAppOnlyToken(this AuthenticationContext authContext)
        {
            return AsyncHelper.RunSync(() => authContext.AcquireAppOnlyTokenAsync());
        }

        public static async Task<AuthenticationResult> AcquireAppOnlyTokenAsync(this AuthenticationContext authContext)
        {
            var cred = new ClientCredential(AppSettings.ClientId, AppSettings.AppKey);
            return await authContext.AcquireTokenAsync(AppSettings.GraphResourceId, cred);
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



        public static IPagedCollection<TSource> Execute<TSource>(this IReadOnlyQueryableSet<TSource> set)
        {
            return AsyncHelper.RunSync(async() => await set.ExecuteAsync());
        }

        public static TSource ExecuteSingle<TSource>(this IReadOnlyQueryableSet<TSource> set)
        {
            return AsyncHelper.RunSync(async () => await set.ExecuteSingleAsync());
        }

        public static IEnumerable<string> GetMemberGroups(this IUser user, bool? securityEnabledOnly)
        {
            return AsyncHelper.RunSync(async () => await user.GetMemberGroupsAsync(securityEnabledOnly));
        }

        public static IEnumerable<string> GetMemberObjects(this IUser user, bool? securityEnabledOnly)
        {
            return AsyncHelper.RunSync(async () => await user.GetMemberObjectsAsync(securityEnabledOnly));
        }
    }
}
