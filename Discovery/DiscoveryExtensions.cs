using Digioma.Office365.Client.Adal;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Discovery
{
    public static class DiscoveryExtensions
    {

        public static AuthenticationResult AcquireAppOnlyDiscoveryServiceToken(this AuthenticationContext authContext)
        {
            return AsyncHelper.RunSync(async () => await authContext.AcquireAppOnlyDiscoveryServiceTokenAsync());
        }

        public static async Task<AuthenticationResult> AcquireAppOnlyDiscoveryServiceTokenAsync(this AuthenticationContext authContext)
        {
            var cred = new ClientCredential(AppSettings.ClientId, AppSettings.ClientSecret);
            return await authContext.AcquireTokenAsync(AppSettings.DiscoveryServiceResourceId, cred);
        }


        #region DiscoveryClient extensions

        public static CapabilityDiscoveryResult DiscoverCapability(this DiscoveryClient discoverClient, Capability capability)
        {
            return AsyncHelper.RunSync<CapabilityDiscoveryResult>(async () =>
                {
                    return await discoverClient.DiscoverCapabilityAsync(capability);
                });
        }

        public static async Task<CapabilityDiscoveryResult> DiscoverCapabilityAsync(this DiscoveryClient discoveryClient, Capability capability)
        {
            return await discoveryClient.DiscoverCapabilityAsync(capability.ToString());
        }

        public static CapabilityDiscoveryResult DiscoverContactsCapability(this DiscoveryClient discoveryClient)
        {
            return discoveryClient.DiscoverCapability(Capability.Contacts);
        }

        public static async Task<CapabilityDiscoveryResult> DiscoverContactsCapabilityAsync(this DiscoveryClient discoveryClient)
        {
            return await discoveryClient.DiscoverCapabilityAsync(Capability.Contacts);
        }

        public static CapabilityDiscoveryResult DiscoverDirectoryCapability(this DiscoveryClient discoveryClient)
        {
            return discoveryClient.DiscoverCapability(Capability.Directory);
        }

        public static async Task<CapabilityDiscoveryResult> DiscoverDirectoryCapabilityAsync(this DiscoveryClient discoveryClient)
        {
            return await discoveryClient.DiscoverCapabilityAsync(Capability.Directory);
        }

        #endregion

    }
}
