using System;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Резултат добавления поста на сайт
    /// </summary>
    public class TorrentSoftResult
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public int TorrentSoftPostId { get; set; }

        public TorrentSoftPost TorrentSoftPost { get; set; }

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
