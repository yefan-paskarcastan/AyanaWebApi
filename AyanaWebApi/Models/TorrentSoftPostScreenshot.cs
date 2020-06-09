using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class TorrentSoftPostScreenshot
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public int TorrentSoftPostId { get; set; }

        public TorrentSoftPost TorrentSoftPost { get; set; }

        public string ScreenUri { get; set; }
    }
}
