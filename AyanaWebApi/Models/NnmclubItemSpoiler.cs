using System;

namespace AyanaWebApi.Models
{
    public class NnmclubItemSpoiler
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public int NnmclubItemId { get; set; }

        public NnmclubItem NnmclubItem { get; set; }
    }
}
