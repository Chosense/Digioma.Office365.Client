using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Digioma.Office365.Client.Graph
{
    public static class GraphExtensions
    {

        public static HttpRequestMessage CreateGetRequestMessage(this AuthenticationResult token, Uri uri)
        {
            return token.CreateGetRequestMessage(uri.ToString());
        }

        public static HttpRequestMessage CreateGetRequestMessage(this AuthenticationResult token, string url)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("Authorization", string.Format("{0} {1}", token.AccessTokenType, token.AccessToken));
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return msg;
        }

        public static HttpRequestMessage CreatePostRequestMessage(this AuthenticationResult token, Uri uri)
        {
            return token.CreatePostRequestMessage(uri.ToString());
        }

        public static HttpRequestMessage CreatePostRequestMessage(this AuthenticationResult token, string url)
        {
            var msg = token.CreateGetRequestMessage(url);
            msg.Method = HttpMethod.Post;
            
            return msg;
        }

    }
}
