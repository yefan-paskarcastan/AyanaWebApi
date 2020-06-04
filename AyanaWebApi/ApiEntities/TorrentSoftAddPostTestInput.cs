using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.ApiEntities
{
    public class TorrentSoftAddPostTestInput
    {
        public string BaseAddress { get; set; }

        public string AddPostAddress { get; set; }

        public string UploadPosterAddress { get; set; }

        public Dictionary<string, string> Cookies { get; set; }

        public Dictionary<string, string> FormData { get; set; }

        public Dictionary<string, string> PosterUploadQueryString { get; set; }
    }
}
