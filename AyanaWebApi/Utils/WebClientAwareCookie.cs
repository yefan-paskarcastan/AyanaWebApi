using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AyanaWebApi.Utils
{
    public class WebClientAwareCookie : WebClient
    {
        public CookieContainer CookieContainer { get; set; }

        public WebClientAwareCookie() : base()
        {
            CookieContainer = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            HttpWebRequest webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = CookieContainer;
            }
            return request;
        }
    }
}
