using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class TorrentSoftPost
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> Screenshots { get; set; }

        public string Spoilers { get; set; }

        public string PosterImg { get; set; }

        public string TorrentFile { get; set; }
    }
}
