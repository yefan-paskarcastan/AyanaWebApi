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

            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            _httpClient = new HttpClient(handler);
        }

        public async Task<bool> AddPostTest(TorrentSoftAddPostTestInput inputParam)
        {
            _httpClient.BaseAddress = new Uri(inputParam.BaseAddress);
            string userHash = await Authorize(inputParam);
            TorrentSoftImgUploadResult imgUploadResult = await UploadPoster(inputParam,
                                                                            "qqfile",
                                                                            "img.jpg",
                                                                            "img.jpg",
                                                                            userHash);
            if (imgUploadResult.Success)
            {
                return await AddPost(inputParam, imgUploadResult, userHash);
            }
            return imgUploadResult.Success;
        }

        #region Private
        readonly AyDbContext _context;

        readonly HttpClient _httpClient;

        /// <summary>
        /// Авторизоваться на сайте
        /// </summary>
        /// <param name="inputParam">Параметры доступа к сайту</param>
        /// <returns></returns>
        async Task<string> Authorize(TorrentSoftAddPostTestInput inputParam)
        {
            IEnumerable<KeyValuePair<string, string>> formContent = inputParam.AuthData;
            var content = new FormUrlEncodedContent(formContent);

            HttpResponseMessage result = await _httpClient.PostAsync(inputParam.AddPostAddress, content);
            string htmlPage = await result.Content.ReadAsStringAsync();
            int startHash = htmlPage.IndexOf(inputParam.UserHashFindVarName)
                                     + inputParam.UserHashFindVarName.Length
                                     + inputParam.UserHashExStringCount;
            return htmlPage.Substring(startHash, inputParam.UserHashLength);
        }

        /// <summary>
        /// Загрузить постер на cайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="httpFormFileHeader"></param>
        /// <param name="httpFormFileName"></param>
        /// <param name="fullPathFileOnServer"></param>
        /// <returns></returns>
        async Task<TorrentSoftImgUploadResult> UploadPoster(TorrentSoftAddPostTestInput inputParam,
                                                            string httpFormFileHeader,
                                                            string httpFormFileName,
                                                            string fullPathFileOnServer,
                                                            string userHash)
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

            var query = HttpUtility.ParseQueryString("");
            foreach (var item in inputParam.PosterUploadQueryString)
            {
                query[item.Key] = item.Value;
            }
            query[httpFormFileHeader] = httpFormFileName;
            query[inputParam.UserHashHttpHeaderName] = userHash;

            string fullAddrUploadImg = inputParam.UploadPosterAddress + query.ToString();
            var result = await _httpClient.PostAsync(fullAddrUploadImg, form);
            result.EnsureSuccessStatusCode();

            var contents = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TorrentSoftImgUploadResult>(contents);
        }

        /// <summary>
        /// Добавляет новый пост на сайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="imgUploadResult"></param>
        /// <returns></returns>
        async Task<bool> AddPost(TorrentSoftAddPostTestInput inputParam,
                                 TorrentSoftImgUploadResult imgUploadResult,
                                 string userHash)
        {
            IEnumerable<KeyValuePair<string, string>> formContent = inputParam.FormData;
            var manualContent = new Dictionary<string, string>
            {
                {inputParam.AddPostFormPosterHeader, imgUploadResult.Xfvalue},
                {inputParam.UserHashHttpHeaderName, userHash}
            };
            var content = new FormUrlEncodedContent(formContent.Union(manualContent));

            var result = await _httpClient.PostAsync(inputParam.AddPostAddress, content);
            return result.IsSuccessStatusCode;
        }
        #endregion
    }
}
