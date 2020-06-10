using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class TorrentSoftAddPostInput
    {
        /// <summary>
        /// Id готового для выкладывания поста
        /// </summary>
        public int TorrentSoftPostId { get; set; }

        /// <summary>
        /// Основной адрес сайта
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Адрес для запроса добавления поста
        /// </summary>
        public string AddPostAddress { get; set; }

        /// <summary>
        /// Адрес для загрузки файла на сайт
        /// </summary>
        public string UploadFileAddress { get; set; }

        /// <summary>
        /// Название заголовка для пользовательского хэша
        /// </summary>
        public string UserHashHttpHeaderName { get; set; }

        /// <summary>
        /// Название переменной на странице сайта, в которой храниться user hash
        /// По этой значению этой переменной вынимается user hash
        /// </summary>
        public string UserHashFindVarName { get; set; }

        /// <summary>
        /// Количество символов между названием перменной и самим токеном
        /// </summary>
        public int UserHashExStringCount { get; set; }

        /// <summary>
        /// Длина хэша
        /// </summary>
        public int UserHashLength { get; set; }

        /// <summary>
        /// Элемент формы для добавления поста, в котором будет прикреплен постер
        /// </summary>
        public string AddPostFormPosterHeader { get; set; }

        /// <summary>
        /// Элемент формы для добавления поста, в котором будет имя поста
        /// </summary>
        public string AddPostFormNameHeader { get; set; }
        
        /// <summary>
        /// Элемент формы для добавления поста, в котором будет основная информация о посте
        /// </summary>
        public string AddPostFormDescriptionHeader { get; set; }

        /// <summary>
        /// Шаблон элемента формы для добавления поста, основная часть
        /// </summary>
        public string AddPostFormScreenshotTemplateStartHeader { get; set; }

        /// <summary>
        /// Шаблон элемента формы для добавления поста, конец строки
        /// </summary>
        public string AddPostFormScreenshotTemplateEndHeader { get; set; }

        /// <summary>
        /// Максимальное количество скриншотов, которые можно прикерпить
        /// </summary>
        public int AddPostFormMaxCountScreenshots { get; set; }

        /// <summary>
        /// Заголовок для запроса, в котором будут содержаться двоичные данные файла
        /// </summary>
        public string AddPostFormFileHeader { get; set; }

        /// <summary>
        /// Параметры формы для добавления поста
        /// </summary>
        public Dictionary<string, string> FormData { get; set; }

        /// <summary>
        /// Квери стринг для загрукзи постера
        /// </summary>
        public Dictionary<string, string> PosterUploadQueryString { get; set; }

        /// <summary>
        /// Квери стринг для загрузки торрент файла
        /// </summary>
        public Dictionary<string, string> TorrentUploadQueryString { get; set; }

        /// <summary>
        /// Данные для авторизации на сайте
        /// </summary>
        public Dictionary<string, string> AuthData { get; set; }
    }
}
