using System;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Резултат добавления поста на сайт
    /// </summary>
    public class SoftResult
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public int SoftPostId { get; set; }

        public SoftPost SoftPost { get; set; }

        /// <summary>
        /// Пост добавлен
        /// </summary>
        public bool SendPostIsSuccess { get; set; }

        /// <summary>
        /// Торрент файл прикреплен
        /// </summary>
        public bool TorrentFileIsSuccess { get; set; }

        /// <summary>
        /// Постер прикреплен
        /// </summary>
        public bool PosterIsSuccess { get; set; }
    }
}
