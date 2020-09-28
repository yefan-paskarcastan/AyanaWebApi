using System;

namespace AyanaWebApi.Models
{
    public class NnmclubItemImg
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string ImgUri { get; set; }

        public int NnmclubItemId { get; set; }

        public NnmclubItem NnmclubItem { get; set; }
    }
}
