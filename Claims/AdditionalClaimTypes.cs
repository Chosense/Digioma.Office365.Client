using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Claims
{
    /// <summary>
    /// Declares constants for addtional claim types used in Digioma applications.
    /// </summary>
    public static class AdditionalClaimTypes
    {
        private const string Prefix = "http://schemas.digioma.co/2016/02/identity/claims/";

        /// <summary>
        /// The type for a claim that specifies the preferred language for a user. The value of the claim is the name 
        /// of a culture in format <c>languagecode2[-country/regioncode2]</c> where country / region is optional.
        /// </summary>
        public const string PreferredLanguage = Prefix + "preferredlanguage";

        /// <summary>
        /// The the for a claim that specifies the domain of the user's account.
        /// </summary>
        public const string Domain = Prefix + "domain";


        /// <summary>
        /// Defined by Microsoft. The URI for a claim that specifies the name of the identity provider.
        /// </summary>
        public const string IdentityProvider = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";

        /// <summary>
        /// Defined by Microsoft. The URI fot a claim that contains the directory object ID of a security principal.
        /// </summary>
        public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// Defined by Microsoft. The URI for a claim that contains the tenant ID of a security principal.
        /// </summary>
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";

    }
}
