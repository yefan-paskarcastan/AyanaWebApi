using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AyanaWebApi.ApiEntities;

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
            TorrentSoftImgUploadResult imgUploadResult = await AddPoster();

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
                    new KeyValuePair<string, string>("xfield[poster2]", imgUploadResult.Xfvalue),
                });
                cookieContainer.Add(baseAddress, new Cookie("PHPSESSID", "9a0c05959b722abad18da483a1a8ef6a"));
                var result = await client.PostAsync("/admin.php", content);
                result.EnsureSuccessStatusCode();
            }
        }

        async Task<TorrentSoftImgUploadResult> AddPoster()
        {
            var baseAddress = new Uri("https://torrent-soft.net");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer, })

            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var form = new MultipartFormDataContent();
                HttpContent content = new StringContent("qqfile");
                form.Add(content, "qqfile");
                content = new StreamContent(System.IO.File.OpenRead("img.jpg"));
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "qqfile",
                    FileName = "img.jpg",
                };
                form.Add(content);

                cookieContainer.Add(baseAddress, new Cookie("PHPSESSID", "9a0c05959b722abad18da483a1a8ef6a"));
                var query = HttpUtility.ParseQueryString("");
                query["mod"] = "upload";
                query["subaction"] = "upload";
                query["news_id"] = "0";
                query["area"] = "xfieldsimage";
                query["author"] = "Baguvix";
                query["xfname"] = "poster2";
                query["user_hash"] = "a3dd97059fb33e491583e55d3c41747156a9e630";
                query["qqfile"] = "img.jpg";
                string opt = "/engine/ajax/controller.php?" + query.ToString();
                var result = await client.PostAsync(opt, form);
                result.EnsureSuccessStatusCode();

                var contents = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TorrentSoftImgUploadResult>(contents);
            }
        }
    }
}
