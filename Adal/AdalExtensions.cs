using Digioma.Office365.Client.Claims;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Digioma.Office365.Client.Adal
{
    public static class AdalExtensions
    {

        #region Active directory clients

        public static AuthenticationContext CreateAuthenticationContext(string tenantId)
        {
            var authority = string.Format("{0}/{1}", AppSettings.AuthorizationUri, tenantId);
            return new AuthenticationContext(authority);
        }

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
        /// This method uses the tenant ID configured in <see cref="AppSettings.TenantId"/>.
        /// To use the returned instance you must give the configured application the proper application permissions to to the
        /// <c>Windows Azure Active Directory</c> application, <c>Read directory data</c> at a minimum.
        /// </remarks>
        public static ActiveDirectoryClient CreateAppOnlyActiveDirectoryClient(this AuthenticationContext authContext)
        {
            return authContext.CreateAppOnlyActiveDirectoryClient(AppSettings.TenantId);
        }

        /// <summary>
        /// Creates an <see cref="ActiveDirectoryClient"/> instance in app-only mode from the current authentication context using the
        /// given tenant ID.
        /// </summary>
        public static ActiveDirectoryClient CreateAppOnlyActiveDirectoryClient(this AuthenticationContext authContext, string tenantId)
        {
            var root = new Uri(new Uri(AppSettings.GraphResourceId), tenantId);
            return new ActiveDirectoryClient(root, async () => (await authContext.AcquireAppOnlyGraphTokenAsync()).AccessToken);
        }

        public static ActiveDirectoryClient CreateAppOnlyActiveDirectoryClient(this AuthenticationContext authContext, Guid tenantId)
        {
            return authContext.CreateAppOnlyActiveDirectoryClient(tenantId.ToString());
        }

        public static ActiveDirectoryClient CreateAppOnlyActiveDirectoryClient(this AuthenticationContext authContext, string tenantId, string clientId, string clientSecret)
        {
            var root = new Uri(new Uri(AppSettings.GraphResourceId), tenantId);
            return new ActiveDirectoryClient(root, async () =>
            {
                var token = await authContext.AcquireAppOnlyGraphTokenAsync(clientId, clientSecret);
                return token.AccessToken;
            });
        }



        public static ActiveDirectoryClient CreateActiveDirectoryClient(this IIdentity identity)
        {
            var tenantId = identity.TenantId();
            var root = new Uri(new Uri(AppSettings.GraphResourceId), tenantId);

            return new ActiveDirectoryClient(root, async () =>
            {
                List<TokenCacheItem> items = null;

                await Task.Run(() =>
                {
                    var cache = new AdalTokenCache(identity.NameIdentifier());
                    items = cache.ReadItems().ToList();
                });

                if(items.Count > 0)
                {
                    return items.First().AccessToken;
                }

                return null;
            });
        }

        public static ActiveDirectoryClient CreateActiveDirectoryClient(this IPrincipal user)
        {
            if (null != user && null != user.Identity) return user.Identity.CreateActiveDirectoryClient();
            return null;
        }

        #endregion



        #region Token acquisition

        /// <summary>
        /// Returns an app-only token for the given resource.
        /// </summary>
        /// <param name="resourceId">The resource identifier for which to return the token.</param>
        /// <returns></returns>
        public static AuthenticationResult AcquireAppOnlyToken(this AuthenticationContext authContext, string resourceId)
        {
            return AsyncHelper.RunSync(async () => await authContext.AcquireAppOnlyTokenAsync(resourceId));
        }

        /// <summary>
        /// Returns an app-only token for the given resource.
        /// </summary>
        /// <param name="resourceId">The resource identifier for which to return the token.</param>
        /// <returns></returns>
        public static async Task<AuthenticationResult> AcquireAppOnlyTokenAsync(this AuthenticationContext authContext, string resourceId)
        {
            return await authContext.AcquireAppOnlyTokenAsync(resourceId, AppSettings.ClientId, AppSettings.ClientSecret);
        }

        /// <summary>
        /// Returns an app-only token for the resource specified in <paramref name="resourceId"/> using the given <paramref name="clientId"/> and <paramref name="clientSecret"/>.
        /// </summary>
        /// <param name="resourceId">The resource identifier for which to return the token.</param>
        /// <param name="clientId">The client ID to use, i.e. the ID (guid) of the application.</param>
        /// <param name="clientSecret">The client secret or key to authenticate with.</param>
        /// <returns></returns>
        public static async Task<AuthenticationResult> AcquireAppOnlyTokenAsync(this AuthenticationContext authContext, string resourceId, string clientId, string clientSecret)
        {
            var cred = new ClientCredential(clientId, clientSecret);
            return await authContext.AcquireTokenAsync(resourceId, cred);
        }

        /// <summary>
        /// Returns an app-only token using the current authentication context.
        /// </summary>
        public static AuthenticationResult AcquireAppOnlyGraphToken(this AuthenticationContext authContext)
        {
            return AsyncHelper.RunSync(() => authContext.AcquireAppOnlyGraphTokenAsync());
        }

        /// <summary>
        /// Returns an app-only token for Microsoft Graph using the current authentication context.
        /// </summary>
        public static async Task<AuthenticationResult> AcquireAppOnlyGraphTokenAsync(this AuthenticationContext authContext)
        {
            return await authContext.AcquireAppOnlyTokenAsync(AppSettings.GraphResourceId);
        }

        /// <summary>
        /// Returns an app-only token for Microsoft Graph using the current authentication context authenticating the application 
        /// using the given <paramref name="clientId"/> and <paramref name="clientSecret"/>.
        /// </summary>
        public static async Task<AuthenticationResult> AcquireAppOnlyGraphTokenAsync(this AuthenticationContext authContext, string clientId, string clientSecret)
        {
            return await authContext.AcquireAppOnlyTokenAsync(AppSettings.GraphResourceId, clientId, clientSecret);
        }



        /// <summary>
        /// Acquires a token for the given resource.
        /// </summary>
        public static async Task<AuthenticationResult> AcquireTokenAsync(this AuthenticationContext authContext, string resourceId)
        {
            return await authContext.AcquireTokenAsync(
                resourceId,
                new ClientCredential(AppSettings.ClientId, AppSettings.ClientSecret)
            );
        }

        /// <summary>
        /// Acquires a token for the given resource as the currently logged on user, meaning that <see cref="ClaimsPrincipal.Current"/> must
        /// return a value.
        /// </summary>
        public static async Task<AuthenticationResult> AcquireTokenSilentAsync(this AuthenticationContext authContext, string resourceId)
        {
            return await authContext.AcquireTokenSilentAsync(
                resourceId, 
                new ClientCredential(AppSettings.ClientId, AppSettings.ClientSecret), 
                new UserIdentifier(ClaimsPrincipal.Current.ObjectIdentifier(), UserIdentifierType.UniqueId)
            );
        }

        /// <summary>
        /// Acquires a token for the current user identity to the specified resource.
        /// </summary>
        public static async Task<AuthenticationResult> AcquireTokenSilentAsync(this IIdentity identity, string resourceId)
        {
            var authContext = identity.CreateAuthenticationContext();
            return await authContext.AcquireTokenSilentAsync(
                resourceId,
                new ClientCredential(AppSettings.ClientId, AppSettings.ClientSecret),
                new UserIdentifier(identity.ObjectIdentifier(), UserIdentifierType.UniqueId)
            );
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

        public static async Task<AuthenticationResult> AcquireGraphTokenByCodeAsync(this AuthenticationContext authContext, string code)
        {
            var clientCredential = new ClientCredential(AppSettings.ClientId, AppSettings.ClientSecret);
            return await authContext.AcquireGraphTokenByCodeAsync(code, clientCredential);
        }

        public static async Task<AuthenticationResult> AcquireGraphTokenByCodeAsync(this AuthenticationContext authContext, string code, ClientCredential clientCredential)
        {
            var token = await authContext.AcquireTokenByAuthorizationCodeAsync(
                code,
                new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)),
                clientCredential,
                AppSettings.GraphResourceId
            );

            return token;
        }

        #endregion



        public static IEnumerable<IGroup> ByDisplayNames(this IGroupCollection groups, params string[] displayNames)
        {
            return groups.OfType<IGroup>().ByDisplayNames(displayNames);
        }

        public static IEnumerable<IGroup> ByDisplayNames(this IReadOnlyQueryableSetBase<IGroup> groups, params string[] displayNames)
        {
            return AsyncHelper.RunSync(async () => await groups.ByDisplayNamesAsync(displayNames));
        }

        public static async Task<IEnumerable<IGroup>> ByDisplayNamesAsync(this IGroupCollection groups, params string[] displayNames)
        {
            return await groups.OfType<IGroup>().ByDisplayNamesAsync(displayNames).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<IGroup>> ByDisplayNamesAsync(this IReadOnlyQueryableSetBase<IGroup> groups, params string[] displayNames)
        {
            var result = await groups.ByPredicateAsync((g) => displayNames.Contains(g.DisplayName)).ConfigureAwait(false);
            return result;
        }

        public static IEnumerable<TSource> ByPredicate<TSource>(this IReadOnlyQueryableSetBase<TSource> set, Func<TSource, bool> predicate)
        {
            return AsyncHelper.RunSync(async () => await set.ByPredicateAsync(predicate));
        }

        public static async Task<IEnumerable<TSource>> ByPredicateAsync<TSource>(this IReadOnlyQueryableSetBase<TSource> set, Func<TSource, bool> predicate)
        {
            var list = new List<TSource>();
            var page = await set.OfType<TSource>().ExecuteAsync();
            while(null != page)
            {
                foreach(var item in from x in page.CurrentPage where predicate(x) select x)
                {
                    list.Add(item);
                }
                page = await page.GetNextPageAsync();
            }

            return list;
        }

        public static IEnumerable<string> CheckMemberGroups(this IUser user, ICollection<string> groupIds)
        {
            return AsyncHelper.RunSync(async () => await user.CheckMemberGroupsAsync(groupIds));
        }



        public static IPagedCollection<TSource> Execute<TSource>(this IReadOnlyQueryableSet<TSource> set)
        {
            return AsyncHelper.RunSync(async() => await set.ExecuteAsync());
        }

        public static IPagedCollection<IUser> Execute(this IUserCollection users)
        {
            return AsyncHelper.RunSync(async () => await users.ExecuteAsync());
        }

        public static IPagedCollection<IGroup> Execute(this IGroupCollection groups)
        {
            return AsyncHelper.RunSync(async () => await groups.ExecuteAsync());
        }

        public static IGroup Execute(this IGroupFetcher group)
        {
            return AsyncHelper.RunSync(async () => await group.ExecuteAsync());
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



        public static IReadOnlyQueryableSet<IUser> ByPrincipalName(this IUserCollection users, string principalName)
        {
            return users.Where(x => x.UserPrincipalName == principalName);
        }



        /// <summary>
        /// Returns the first tenant with the given verified domain from the current tenants collection.
        /// </summary>
        /// <param name="domain">The verified domain, e.g. <c>yourtenant.onmicrosoft.com</c></param>
        public static ITenantDetail ByDomain(this ITenantDetailCollection tenants, string domain)
        {
            return AsyncHelper.RunSync(async () => await tenants.ByDomainAsync(domain));
        }

        /// <summary>
        /// Returns the first tenant with the given verified domain from the current tenants collection.
        /// </summary>
        /// <param name="domain">The verified domain, e.g. <c>yourtenant.onmicrosoft.com</c></param>
        public static async Task<ITenantDetail> ByDomainAsync(this ITenantDetailCollection tenants, string domain)
        {
            var result = await tenants.ByPredicateAsync((t) =>
            {
                var dom = t.VerifiedDomains.FirstOrDefault(x => x.Name.Equals(domain, StringComparison.OrdinalIgnoreCase));
                return dom != null;
            });

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Returns the tenant from the current tenants collection with the given tenant id.
        /// </summary>
        /// <param name="tenantId">The object ID of the tenant to return.</param>
        public static ITenantDetail ByTenantId(this ITenantDetailCollection tenants, string tenantId)
        {
            return AsyncHelper.RunSync(async () => await tenants.ByTenantIdAsync(tenantId));
        }

        /// <summary>
        /// Returns the tenant from the current tenants collection with the given tenant id.
        /// </summary>
        /// <param name="tenantId">The object ID of the tenant to return.</param>
        public static async Task<ITenantDetail> ByTenantIdAsync(this ITenantDetailCollection tenants, string tenantId)
        {
            return await tenants.Where(x => x.ObjectId == tenantId).ExecuteSingleAsync();
        }

        public static async Task<ITenantDetail> ByTenantIdAsync(this ITenantDetailCollection tenants, Guid tenantId)
        {
            return await tenants.ByTenantIdAsync(tenantId.ToString());
        }



        public static async Task<IEnumerable<TSource>> GetAllAsync<TSource>(this IPagedCollection<TSource> source)
        {
            var list = new List<TSource>();
            if(null != source.CurrentPage) list.AddRange(source.CurrentPage);

            while(source.MorePagesAvailable)
            {
                source = await source.GetNextPageAsync();
                if(null != source.CurrentPage) list.AddRange(source.CurrentPage);
            }

            return list;
        }
    }
}
