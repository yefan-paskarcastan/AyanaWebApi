using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SoftService : ISoftService
    {
        public SoftService(AyDbContext context,
                                  IHttpClientFactory clientFactory,
                                  ILogService logService)
        {
            _context = context;
            _httpClient = clientFactory.CreateClient("torrentSoft");
            _logService = logService;
        }

        /// <summary>
        /// Добавляет пост на сайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns></returns>
        public async Task<ServiceResult<SoftResult>> AddPost(SoftPostInput inputParam)
        {
            var result = new SoftResult
            {
                Created = DateTime.Now,
                SoftPostId = inputParam.SoftPostId,
                SoftPost = null,
                PosterIsSuccess = false,
                TorrentFileIsSuccess = false,
                SendPostIsSuccess = false
            };

            var serviceResult = new ServiceResult<SoftResult>();
            serviceResult.ServiceName = nameof(SoftService);
            serviceResult.Location = "Публикация поста";
            serviceResult.ResultObj = result;

            SoftPost post =
                _context.SoftPosts
                        .Include(el => el.Imgs)
                        .SingleOrDefault(el => el.Id == inputParam.SoftPostId);
            if (post == null)
            {
                serviceResult.Comment = "Не удалось найти указанный подготовленный пост в базе";
                serviceResult.ErrorContent = "TorrentSoftPostId = " + inputParam.SoftPostId;
                _logService.Write(serviceResult);
                return serviceResult;
            }
            result.SoftPost = post;

            string userHash = await Authorize(inputParam);
            SoftFileUploadResult torrentUploadResult = await UploadFile(inputParam,
                                                                        Path.GetFileName(post.TorrentFile),
                                                                        post.TorrentFile,
                                                                        userHash,
                                                                        inputParam.TorrentUploadQueryString);
            if (!torrentUploadResult.Success)
            {
                serviceResult.Comment = "Не удалось загрузить торрент файл на сайт";
                serviceResult.ErrorContent = torrentUploadResult.Returnbox;
                _logService.Write(serviceResult);
                return serviceResult;
            }
            result.TorrentFileIsSuccess = true;

            SoftFileUploadResult imgUploadResult = await UploadFile(inputParam,
                                                                    Path.GetFileName(post.PosterImg),
                                                                    post.PosterImg,
                                                                    userHash,
                                                                    inputParam.PosterUploadQueryString);
            if (!imgUploadResult.Success)
            {
                serviceResult.Comment = "Не удалось загрузить постер на сайт";
                serviceResult.ErrorContent = imgUploadResult.Returnbox;
                _logService.Write(serviceResult);
                return serviceResult;
            }
            result.PosterIsSuccess = true;
            
            result.SendPostIsSuccess = await SendPost(inputParam, imgUploadResult, userHash, post);
            if(!result.SendPostIsSuccess)
            {
                serviceResult.Comment = "Не удалось отправить пост на сайт";
                serviceResult.ErrorContent = imgUploadResult.Returnbox;
                _logService.Write(serviceResult);
            }
            return serviceResult;
        }

        #region Private
        readonly AyDbContext _context;

        readonly HttpClient _httpClient;

        readonly ILogService _logService;

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
        /// <returns></returns>
        async Task<SoftFileUploadResult> UploadFile(SoftPostInput inputParam,
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

            var contents = await result.Content.ReadAsStringAsync();
            SoftFileUploadResult uploadResult = JsonConvert.DeserializeObject<SoftFileUploadResult>(contents);

            if (!uploadResult.Success)
            {
                var serviceResult = new ServiceResult<string>();
                serviceResult.ServiceName = nameof(SoftService);
                serviceResult.Location = "Загрузка файла";
                serviceResult.Comment = "Не удалось загрузить файл на сайт";
                serviceResult.ErrorContent = "Имя файла на сервере: " + fullPathFileOnServer;
                serviceResult.ExceptionMessage = contents;
                _logService.Write(serviceResult);
            }
            return uploadResult;
        }

        /// <summary>
        /// Добавляет новый пост на сайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="imgUploadResult"></param>
        /// <returns></returns>
        async Task<bool> SendPost(SoftPostInput inputParam,
                                  SoftFileUploadResult imgUploadResult,
                                  string userHash,
                                  SoftPost post)
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
