using Digioma.Office365.Client.Adal;
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
        public static DiscoveryClient CreateDiscoveryClient(AuthenticationContext authContext)
        {
            return new DiscoveryClient(AppSettings.DiscoveryServiceEndpointUri,
                async () =>
                {
                    var authResult = await authContext.AcquireDiscoveryServiceTokenSilentAsync();
                    return authResult.AccessToken;
                });
        }

    }
}
