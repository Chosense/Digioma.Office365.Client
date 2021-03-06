﻿using Microsoft.Azure.ActiveDirectory.GraphClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Claims
{
    public static class ClaimsExtensions
    {
        public static string Authority(this IIdentity identity)
        {
            return string.Format("https://login.windows.net/{0}/", identity.TenantId());
        }

        public static string Authority(this IPrincipal user)
        {
            if(null != user && null != user.Identity)
            {
                return user.Identity.Authority();
            }
            return null;
        }

        public static void AddClaim(this IIdentity identity, string claimType, string claimValue)
        {
            var claimId = identity as ClaimsIdentity;
            if(null != claimId)
            {
                claimId.AddClaim(new Claim(claimType, claimValue));
            }
        }

        public static void AddClaim(this IPrincipal user, string claimType, string claimValue)
        {
            if (null != user) user.Identity.AddClaim(claimType, claimValue);
        }

        public static string ClaimValue(this IIdentity identity, string claimType)
        {
            return identity.GetFirstClaimValue(claimType);
        }

        public static string ClaimValue(this IPrincipal user, string claimType)
        {
            if(null != user && null != user.Identity)
            {
                return user.Identity.ClaimValue(claimType);
            }

            return null;
        }

        public static string Domain(this IPrincipal user)
        {
            if(null != user && null != user.Identity)
            {
                return user.Identity.Domain();
            }

            return null;
        }
        public static string Domain(this IIdentity identity)
        {
            return identity.ClaimValue(AdditionalClaimTypes.Domain);
        }

        public static string FirstName (this IPrincipal user)
        {
            return user.Identity.FirstName();
        }

        public static string FirstName(this IIdentity identity)
        {
            return identity.ClaimValue(ClaimTypes.GivenName);
        }

        public static string LastName(this IPrincipal user)
        {
            return user.Identity.LastName();
        }

        public static string LastName(this IIdentity identity)
        {
            return identity.ClaimValue(ClaimTypes.Surname);
        }

        public static string NameIdentifier(this IIdentity identity)
        {
            return identity.ClaimValue(ClaimTypes.NameIdentifier);
        }

        public static string NameIdentifier(this IPrincipal user)
        {
            return user.ClaimValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Returns true if the current user is a member of at least one of the specified roles.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static bool IsInAnyRole(this IPrincipal user, params string[] roles)
        {
            if(null != user && null != roles && roles.Length > 0)
            {
                var inRoles = from x in roles where user.IsInRole(x) select x;
                return inRoles.Count() > 0;
            }
            return false;
        }

        public static string ObjectIdentifier(this IIdentity identity)
        {
            return identity.ClaimValue(AdditionalClaimTypes.ObjectIdentifier);
        }

        public static string ObjectIdentifier(this IPrincipal user)
        {
            return user.ClaimValue(AdditionalClaimTypes.ObjectIdentifier);
        }

        public static string PreferredLanguage(this IIdentity identity)
        {
            return identity.ClaimValue(AdditionalClaimTypes.PreferredLanguage);
        }

        public static string PreferredLanguage(this IPrincipal user)
        {
            return user.ClaimValue(AdditionalClaimTypes.PreferredLanguage);
        }

        public static string TenantId(this IIdentity identity)
        {
            return identity.ClaimValue(AdditionalClaimTypes.TenantId);
        }

        public static string TenantId(this IPrincipal user)
        {
            return user.ClaimValue(AdditionalClaimTypes.TenantId);
        }


        private static string GetFirstClaimValue(this IIdentity identity, string claimType)
        {
            var id = identity as ClaimsIdentity;
            if(null != id)
            {
                var claim = id.FindFirst(claimType);
                if(null != claim && !string.IsNullOrEmpty(claim.Value))
                {
                    return claim.Value;
                }
            }

            return null;
        }
    }
}
