using Microsoft.Office365.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Discovery
{
    public static class Extensions
    {
        public static async Task<CapabilityDiscoveryResult> DiscoverCapabilityAsync(this DiscoveryClient discoveryClient, Capability capability)
        {
            return await discoveryClient.DiscoverCapabilityAsync(capability.ToString());
        }

        public static async Task<CapabilityDiscoveryResult> DiscoverContactsCapabilityAsync(this DiscoveryClient discoveryClient)
        {
            return await discoveryClient.DiscoverCapabilityAsync(Capability.Contacts);
        }
    }
}
