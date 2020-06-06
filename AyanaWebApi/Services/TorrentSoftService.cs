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
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services
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

        /// <summary>
        /// Добавляет тестовый пост на сайт. Проверка работоспособности
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns></returns>
        public async Task<bool> AddPostTest(TorrentSoftAddPostTestInput inputParam)
        {
            _httpClient.BaseAddress = new Uri(inputParam.BaseAddress);
            string userHash = await Authorize(inputParam);
            TorrentSoftFileUploadResult torrentUploadResult = await UploadFile(inputParam,
                                                                               "program.torrent",
                                                                               "program.torrent",
                                                                               userHash,
                                                                               inputParam.TorrentUploadQueryString);
            TorrentSoftFileUploadResult imgUploadResult = await UploadFile(inputParam,
                                                                           "img.jpg",
                                                                           "img.jpg",
                                                                           userHash,
                                                                           inputParam.PosterUploadQueryString);
            if (imgUploadResult.Success && torrentUploadResult.Success)
            {
                return await AddPost(inputParam, imgUploadResult, userHash);
            }
            return false;
        }

        #region Private
        readonly AyDbContext _context;

        readonly HttpClient _httpClient;

        /// <summary>
        /// Авторизоваться на сайте
        /// </summary>
        /// <param name="inputParam">Параметры доступа к сайту</param>
        /// <returns>Возвращает хеш пользователя, который нужно добавлять к некоторым запросам. Если авторизвация не успешна - null</returns>
        async Task<string> Authorize(TorrentSoftAddPostTestInput inputParam)
        {
            IEnumerable<KeyValuePair<string, string>> formContent = inputParam.AuthData;
            var content = new FormUrlEncodedContent(formContent);

            HttpResponseMessage result = await _httpClient.PostAsync(inputParam.AddPostAddress, content);
            string htmlPage = await result.Content.ReadAsStringAsync();
            int startHash = htmlPage.IndexOf(inputParam.UserHashFindVarName)
                                     + inputParam.UserHashFindVarName.Length
                                     + inputParam.UserHashExStringCount;
            if (startHash == 0 || startHash == -1) return null;
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
        async Task<TorrentSoftFileUploadResult> UploadFile(TorrentSoftAddPostTestInput inputParam,
                                                           string httpFormFileName,
                                                           string fullPathFileOnServer,
                                                           string userHash,
                                                           Dictionary<string, string> queryString)
        {
            var form = new MultipartFormDataContent();
            HttpContent content = new StringContent(inputParam.AddPostFormFileHeader);
            form.Add(content, inputParam.AddPostFormFileHeader);

            content = new StreamContent(System.IO.File.OpenRead(fullPathFileOnServer));
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = inputParam.AddPostFormFileHeader,
                FileName = httpFormFileName,
            };
            form.Add(content);

            var query = HttpUtility.ParseQueryString("");
            foreach (var item in queryString)
            {
                query[item.Key] = item.Value;
            }
            query[inputParam.AddPostFormFileHeader] = httpFormFileName;
            query[inputParam.UserHashHttpHeaderName] = userHash;

            string fullAddrUploadFile = inputParam.UploadFileAddress + query.ToString();
            var result = await _httpClient.PostAsync(fullAddrUploadFile, form);

            var contents = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TorrentSoftFileUploadResult>(contents);
        }

        /// <summary>
        /// Добавляет новый пост на сайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="imgUploadResult"></param>
        /// <returns></returns>
        async Task<bool> AddPost(TorrentSoftAddPostTestInput inputParam,
                                 TorrentSoftFileUploadResult imgUploadResult,
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
