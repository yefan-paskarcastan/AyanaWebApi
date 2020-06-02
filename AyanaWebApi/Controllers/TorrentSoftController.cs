using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AyanaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TorrentSoftController : ControllerBase
    {
        [HttpPost("AddPost")]
        public async void AddPost()
        {
            var baseAddress = new Uri("https://torrent-soft.net");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mod", "addnews"),
                    new KeyValuePair<string, string>("action", "doaddnews"),
                    new KeyValuePair<string, string>("user_hash", "a3dd97059fb33e491583e55d3c41747156a9e630"),
                    new KeyValuePair<string, string>("title", "value1"),
                    new KeyValuePair<string, string>("full_story", "value2"),
                    new KeyValuePair<string, string>("allow_main", "1"),
                    new KeyValuePair<string, string>("allow_rating", "1"),
                    new KeyValuePair<string, string>("allow_br", "1"),
                    new KeyValuePair<string, string>("allow_comm", "1"),
                    new KeyValuePair<string, string>("allow_rss", "1"),
                    new KeyValuePair<string, string>("allow_rss_turbo", "1"),
                    new KeyValuePair<string, string>("allow_rss_dzen", "1"),
                });
                cookieContainer.Add(baseAddress, new Cookie("PHPSESSID", "2f208b2aec5d996a00d72ed865a1c5cc"));
                var result = await client.PostAsync("/admin.php", content);
                result.EnsureSuccessStatusCode();
            }
        }
    }
}
