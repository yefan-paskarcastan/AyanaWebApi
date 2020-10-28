using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.IO;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    public class SoftService : ISoftService
    {
        public SoftService(AyDbContext context,
                           IHttpClientFactory clientFactory,
                           ILogger<SoftService> logger)
        {
            _context = context;
            _httpClient = clientFactory.CreateClient("torrentSoft");
            _logger = logger;
        }

        readonly AyDbContext _context;
        readonly HttpClient _httpClient;
        readonly ILogger<SoftService> _logger;

        /// <summary>
        /// Добавляет пост на сайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns>Если отработал успешно, то true, инчае false</returns>
        public async Task<PublishResult> AddPost(SoftPostInput inputParam)
        {
            SoftPost post =
                _context.SoftPosts
                    .Include(el => el.Imgs)
                    .SingleOrDefault(el => el.Id == inputParam.SoftPostId);
            if (post == null)
            {
                _logger.LogError("Не удалось найти указанный подготовленный пост в базе. SoftPost.Id = " + inputParam.SoftPostId);
                return PublishResult.Error;
            }

            string userHash = await Authorize(inputParam);

            //Загружаем файл
            PublishResult torrentUploadResult = await UploadFile(inputParam,
                                                                 Path.GetFileName(post.TorrentFile),
                                                                 post.TorrentFile,
                                                                 userHash,
                                                                 inputParam.TorrentUploadQueryString);
            if (torrentUploadResult == PublishResult.Error)
            {
                _logger.LogError("Не удалось загрузить торрент файл на сайт");
                return PublishResult.Error;
            }
            if (torrentUploadResult == PublishResult.FileExist)
            {
                _logger.LogError("Такой торрент файл уже загружен");
                return PublishResult.FileExist;
            }

            //Загружаем постер
            SoftFileUploadResult imgUploadResult = await UploadPoster(inputParam,
                                                                      Path.GetFileName(post.PosterImg),
                                                                      post.PosterImg,
                                                                      userHash,
                                                                      inputParam.PosterUploadQueryString);
            if (imgUploadResult == null)
            {
                _logger.LogError("Не удалось загрузить постер на сайт");
                return PublishResult.Error;
            }
            
            //Выкладываем
            bool sendPostResult = await SendPost(inputParam, imgUploadResult, userHash, post);
            if(!sendPostResult)
            {
                _logger.LogError("Не удалось отправить пост на сайт");
                return PublishResult.Error;
            }
            return PublishResult.Success;
        }

        #region Private
        /// <summary>
        /// Авторизоваться на сайте
        /// </summary>
        /// <param name="inputParam">Параметры доступа к сайту</param>
        /// <returns>Возвращает хеш пользователя, который нужно добавлять к некоторым запросам. Если авторизвация не успешна - null</returns>
        async Task<string> Authorize(SoftPostInput inputParam)
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
        /// <returns>Возвращает null, если что-то пошло не так</returns>
        async Task<PublishResult> UploadFile(SoftPostInput inputParam,
                                             string httpFormFileName,
                                             string fullPathFileOnServer,
                                             string userHash,
                                             Dictionary<string, string> queryString)
        {
            var form = new MultipartFormDataContent();
            HttpContent content = new StringContent(inputParam.AddPostFormFileHeader);
            form.Add(content, inputParam.AddPostFormFileHeader);

            content = new StreamContent(File.OpenRead(fullPathFileOnServer));
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

            string contents = await result.Content.ReadAsStringAsync();
            SoftFileUploadResult uploadResult = JsonConvert.DeserializeObject<SoftFileUploadResult>(contents);

            if (!uploadResult.Success)
            {
                if (contents.Contains("\"error\":\"Такой файл уже загружен на сайт!"))
                {
                    _logger.LogError("Такой файл уже загружен на сайт!");
                    return PublishResult.FileExist;
                }
                _logger.LogError("Не удалось загрузить файл на сайт. Имя файла на сервере:" + fullPathFileOnServer);
                _logger.LogError("Returnbox = " + uploadResult.Returnbox);
                _logger.LogError("UploadResultContent = " + contents);
                return PublishResult.Error;
            }
            return PublishResult.Success;
        }

        /// <summary>
        /// Загрузить постер на cайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="httpFormFileHeader"></param>
        /// <param name="httpFormFileName"></param>
        /// <param name="fullPathFileOnServer"></param>
        /// <returns>Возвращает null, если что-то пошло не так</returns>
        async Task<SoftFileUploadResult> UploadPoster(SoftPostInput inputParam,
                                                      string httpFormFileName,
                                                      string fullPathFileOnServer,
                                                      string userHash,
                                                      Dictionary<string, string> queryString)
        {
            var form = new MultipartFormDataContent();
            HttpContent content = new StringContent(inputParam.AddPostFormFileHeader);
            form.Add(content, inputParam.AddPostFormFileHeader);

            content = new StreamContent(File.OpenRead(fullPathFileOnServer));
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

            string contents = await result.Content.ReadAsStringAsync();
            SoftFileUploadResult uploadResult = JsonConvert.DeserializeObject<SoftFileUploadResult>(contents);

            if (!uploadResult.Success)
            {
                _logger.LogError("Не удалось загрузить файл на сайт. Имя файла на сервере:" + fullPathFileOnServer);
                _logger.LogError("Returnbox = " + uploadResult.Returnbox);
                _logger.LogError("UploadResultContent = " + contents);
                return null;
            }
            return uploadResult;
        }

        /// <summary>
        /// Добавляет новый пост на сайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="imgUploadResult"></param>
        /// <returns>Если отработал успешно, то true, инчае false</returns>
        async Task<bool> SendPost(SoftPostInput inputParam,
                                  SoftFileUploadResult imgUploadResult,
                                  string userHash,
                                  SoftPost post)
        {
            IEnumerable<KeyValuePair<string, string>> formContent = inputParam.FormData;
            var manualContent = new Dictionary<string, string>
            {
                {inputParam.AddPostFormNameHeader, post.Name},
                {inputParam.AddPostFormDescriptionHeader, post.Description + post.Spoilers},
                {inputParam.AddPostFormPosterHeader, imgUploadResult.Xfvalue},
                {inputParam.UserHashHttpHeaderName, userHash}
            };

            string startKey = inputParam.AddPostFormScreenshotTemplateStartHeader;
            string endKey = inputParam.AddPostFormScreenshotTemplateEndHeader;
            for (int i = 1; i <= inputParam.AddPostFormMaxCountScreenshots
                            && i <= post.Imgs.Count; i++)
            {
                manualContent.Add(startKey + i + endKey, post.Imgs[i - 1].ImgUri);
            }
            //todo: Нужно добавить обработку исключения на случай если строка слишком длинная https://stackoverflow.com/questions/38440631
            var content = new FormUrlEncodedContent(formContent.Union(manualContent));

            var result = await _httpClient.PostAsync(inputParam.AddPostAddress, content);
            return result.IsSuccessStatusCode;
        }
        #endregion
    }
}