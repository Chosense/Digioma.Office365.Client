using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Discovery
{
    public static class DiscoveryFactory
    {

        public static async Task<CapabilityDiscoveryResult>  DiscoverCapabilityAsync(Capability capability)
        {
            var authContext = Adal.ADALFactory.CreateAuthenticationContext();

            DiscoveryClient discClient = new DiscoveryClient(AppSettings.DiscoveryServiceEndpointUri,
                async () =>
                {
                    var authResult = await Adal.ADALFactory.AcquireTokenSilentAsync();
                    return authResult.AccessToken;
                });

            return await discClient.DiscoverCapabilityAsync(capability.ToString());
        }

    }
}
