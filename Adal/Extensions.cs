using Digioma.Office365.Client.Claims;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Adal
{
    public static class Extensions
    {
        public static async Task<AuthenticationResult> AcquireTokenSilentAsync(this AuthenticationContext authContext, string resourceId)
        {
            return await authContext.AcquireTokenSilentAsync(resourceId, new ClientCredential(AppSettings.ClientId, AppSettings.AppKey), new UserIdentifier(ClaimsFactory.ObjectIdentifier(), UserIdentifierType.UniqueId));
        }

        public static async Task<AuthenticationResult> AcquireDiscoveryServiceTokenSilentAsync(this AuthenticationContext authContext)
        {
            return await authContext.AcquireTokenSilentAsync(AppSettings.DiscoveryServiceResourceId);
        }
    }
}
