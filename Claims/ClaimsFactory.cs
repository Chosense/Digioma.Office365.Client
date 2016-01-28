using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Claims
{
    public static class ClaimsFactory
    {
        public static string NameIdentifier()
        {
            return ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public static string ObjectIdentifier()
        {
            return ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
        }
    }
}
