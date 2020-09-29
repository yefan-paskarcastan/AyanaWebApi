using System;

namespace AyanaWebApi.Models
{
    public class SoftPostImg
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public int SoftPostId { get; set; }

        public SoftPost SoftPost { get; set; }

        public string ImgUri { get; set; }
    }
}
