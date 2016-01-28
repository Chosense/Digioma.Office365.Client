using Digioma.Office365.Client.Claims;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Adal
{
    public static class ADALFactory
    {

        public static AuthenticationContext CreateAuthenticationContext()
        {
            var cache = new ADALTokenCache(ClaimsFactory.NameIdentifier());
            AuthenticationContext authContext = new AuthenticationContext(AppSettings.Authority, cache);

            return authContext;
        }

    }
}
