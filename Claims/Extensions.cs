﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Claims
{
    public static class Extensions
    {

        public static string NameIdentifier(this IIdentity identity)
        {
            return identity.GetFirstClaimValue(ClaimTypes.NameIdentifier);
        }

        public static string NameIdentifier(this IPrincipal user)
        {
            if(null != user)
            {
                return user.Identity.NameIdentifier();
            }

            return null;
        }

        public static string ObjectIdentifier(this IIdentity identity)
        {
            return identity.GetFirstClaimValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
        }

        public static string ObjectIdentifier(this IPrincipal user)
        {
            if(null != user)
            {
                return user.Identity.ObjectIdentifier();
            }
            return null;
        }

        private static string GetFirstClaimValue(this IIdentity identity, string claimType)
        {
            var id = identity as ClaimsIdentity;
            if(null != id)
            {
                var claim = id.FindFirst(claimType);
                if(null != claim)
                {
                    return claim.Value;
                }
            }

            return null;
        }
    }
}
