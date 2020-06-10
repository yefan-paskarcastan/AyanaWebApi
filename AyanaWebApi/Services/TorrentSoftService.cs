using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;
using AyanaWebApi.Services.Interfaces;

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
        public async Task<bool> AddPostTest(TorrentSoftAddPostInput inputParam)
        {
            TorrentSoftPost post =
                _context.TorrentSoftPosts
                        .Include(el => el.Screenshots)
                        .SingleOrDefault(el => el.Id == inputParam.TorrentSoftPostId);
            if (post == null)
            {
                _context.Logs.Add(new Log
                {
                    Created = DateTime.Now,
                    Location = "TorrentSoftService / AddPostTest / Выкладывание поста",
                    Message = $"Не удалось найти указанный обработанный пост в базе.",
                });
                _context.SaveChanges();
                return false;
            }

            _httpClient.BaseAddress = new Uri(inputParam.BaseAddress);
            string userHash = await Authorize(inputParam);
            TorrentSoftFileUploadResult torrentUploadResult = await UploadFile(inputParam,
                                                                               Path.GetFileName(post.TorrentFile),
                                                                               post.TorrentFile,
                                                                               userHash,
                                                                               inputParam.TorrentUploadQueryString);
            if (!torrentUploadResult.Success)
            {
                _context.Logs.Add(new Log
                {
                    Created = DateTime.Now,
                    Location = "TorrentSoftService / AddPostTest / Выкладывание поста",
                    Message = $"Не удалось загрузить торрент файл на сайт",
                    StackTrace = torrentUploadResult.Returnbox,
                });
                _context.SaveChanges();
                return false;
            }

            TorrentSoftFileUploadResult imgUploadResult = await UploadFile(inputParam,
                                                                           Path.GetFileName(post.PosterImg),
                                                                           post.PosterImg,
                                                                           userHash,
                                                                           inputParam.PosterUploadQueryString);

            if (!imgUploadResult.Success)
            {
                _context.Logs.Add(new Log
                {
                    Created = DateTime.Now,
                    Location = "TorrentSoftService / AddPostTest / Выкладывание поста",
                    Message = $"Не удалось загрузить постер на сайт",
                    StackTrace = imgUploadResult.Returnbox,
                });
                _context.SaveChanges();
                return false;
            }
            
            return await AddPost(inputParam, imgUploadResult, userHash, post);
        }

        #region Private
        readonly AyDbContext _context;

        readonly HttpClient _httpClient;

        /// <summary>
        /// Авторизоваться на сайте
        /// </summary>
        /// <param name="inputParam">Параметры доступа к сайту</param>
        /// <returns>Возвращает хеш пользователя, который нужно добавлять к некоторым запросам. Если авторизвация не успешна - null</returns>
        async Task<string> Authorize(TorrentSoftAddPostInput inputParam)
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
        /// Загрузить файл на cайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="httpFormFileHeader"></param>
        /// <param name="httpFormFileName"></param>
        /// <param name="fullPathFileOnServer"></param>
        /// <returns></returns>
        async Task<TorrentSoftFileUploadResult> UploadFile(TorrentSoftAddPostInput inputParam,
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
            TorrentSoftFileUploadResult uploadResult = JsonConvert.DeserializeObject<TorrentSoftFileUploadResult>(contents);

            if (!uploadResult.Success)
            {
                _context.Logs.Add(new Log
                {
                    Created = DateTime.Now,
                    Location = "TorrentSoftService / UploadFile / Загрузка файла",
                    Message = "Не удалось загрузить файл на сайт. Имя файла на сервере: " + fullPathFileOnServer,
                    StackTrace = contents,
                });
                _context.SaveChanges();
            }
            return uploadResult;
        }

        /// <summary>
        /// Добавляет новый пост на сайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="imgUploadResult"></param>
        /// <returns></returns>
        async Task<bool> AddPost(TorrentSoftAddPostInput inputParam,
                                 TorrentSoftFileUploadResult imgUploadResult,
                                 string userHash,
                                 TorrentSoftPost post)
        {
            IEnumerable<KeyValuePair<string, string>> formContent = inputParam.FormData;
            var manualContent = new Dictionary<string, string>
            {
                {inputParam.AddPostFormNameHeader, post.Name },
                {inputParam.AddPostFormDescriptionHeader, post.Description + post.Spoilers },
                {inputParam.AddPostFormPosterHeader, imgUploadResult.Xfvalue},
                {inputParam.UserHashHttpHeaderName, userHash}
            };

            string startKey = inputParam.AddPostFormScreenshotTemplateStartHeader;
            string endKey = inputParam.AddPostFormScreenshotTemplateEndHeader;
            for (int i = 1; i < inputParam.AddPostFormMaxCountScreenshots
                            && i < post.Screenshots.Count; i++)
            {
                manualContent.Add(startKey + i + endKey, post.Screenshots[i - 1].ScreenUri);
            }
            var content = new FormUrlEncodedContent(formContent.Union(manualContent));

            var result = await _httpClient.PostAsync(inputParam.AddPostAddress, content);
            return result.IsSuccessStatusCode;
        }
        #endregion
    }
}
