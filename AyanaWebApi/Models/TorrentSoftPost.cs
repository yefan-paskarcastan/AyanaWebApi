using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class TorrentSoftPost
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<TorrentSoftPostScreenshot> Screenshots { get; set; }

        public string Spoilers { get; set; }

        public string PosterImg { get; set; }

        public string TorrentFile { get; set; }
    }
}
