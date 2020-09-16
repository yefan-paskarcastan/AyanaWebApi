using System;
using System.Collections.Generic;

namespace AyanaWebApi.Models
{
    public class NnmclubItem
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<NnmclubItemSpoiler> Spoilers { get; set; }

        public List<RutorItemImg> Imgs { get; set; }

        public int NnmclubListItemId { get; set; }

        public NnmclubListItem NnmclubListItem { get; set; }
    }
}
