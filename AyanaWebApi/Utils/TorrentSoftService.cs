using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using AyanaWebApi.Models;
using AyanaWebApi.ApiEntities;

namespace AyanaWebApi.Utils
{
    public class TorrentSoftService : ITorrentSoftService
    {
        public TorrentSoftService(AyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddPostTest(TorrentSoftAddPostTestInput inputParam)
        {
            TorrentSoftImgUploadResult imgUploadResult = await UploadPoster(inputParam,
                                                                            "qqfile",
                                                                            "img.jpg",
                                                                            "img.jpg");
            if (imgUploadResult.Success)
            {
                return await AddPost(inputParam, imgUploadResult);
            }
            return imgUploadResult.Success;
        }

        #region Private
        readonly AyDbContext _context;

        /// <summary>
        /// Загружает постер на сервер
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="httpFormFileHeader"></param>
        /// <param name="httpFormFileName"></param>
        /// <param name="fullPathFileOnServer"></param>
        /// <returns></returns>
        async Task<TorrentSoftImgUploadResult> UploadPoster(TorrentSoftAddPostTestInput inputParam,
                                                            string httpFormFileHeader,
                                                            string httpFormFileName,
                                                            string fullPathFileOnServer)
        {
            var baseAddress = new Uri(inputParam.BaseAddress);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer, })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var form = new MultipartFormDataContent();
                HttpContent content = new StringContent(httpFormFileHeader);
                form.Add(content, httpFormFileHeader);
                content = new StreamContent(System.IO.File.OpenRead(fullPathFileOnServer));
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = httpFormFileHeader,
                    FileName = httpFormFileName,
                };
                form.Add(content);

                foreach (KeyValuePair<string, string> item in inputParam.Cookies)
                {
                    cookieContainer.Add(baseAddress, new Cookie(item.Key, item.Value));
                }

                var query = HttpUtility.ParseQueryString("");
                foreach (var item in inputParam.PosterUploadQueryString)
                {
                    query[item.Key] = item.Value;
                }
                query[httpFormFileHeader] = httpFormFileName;

                string fullAddrUploadImg = inputParam.UploadPosterAddress + query.ToString();
                var result = await client.PostAsync(fullAddrUploadImg, form);
                result.EnsureSuccessStatusCode();

                var contents = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TorrentSoftImgUploadResult>(contents);
            }
        }

        /// <summary>
        /// Добавляет новый пост на сервер
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="imgUploadResult"></param>
        /// <returns></returns>
        async Task<bool> AddPost(TorrentSoftAddPostTestInput inputParam,
                                 TorrentSoftImgUploadResult imgUploadResult)
        {
            var baseAddress = new Uri(inputParam.BaseAddress);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                IEnumerable<KeyValuePair<string, string>> formContent = inputParam.FormData;
                var manualContent = new Dictionary<string, string>
                {
                    {"xfield[poster2]", imgUploadResult.Xfvalue}
                };
                var content = new FormUrlEncodedContent(formContent.Union(manualContent));

                foreach (KeyValuePair<string, string> item in inputParam.Cookies)
                {
                    cookieContainer.Add(baseAddress, new Cookie(item.Key, item.Value));
                }
                var result = await client.PostAsync(inputParam.AddPostAddress, content);
                return result.IsSuccessStatusCode;
            }
        }
        #endregion
    }
}
