using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class RutorItem
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<RutorItemSpoiler> Spoilers { get; set; }

        public List<RutorItemImg> Imgs { get; set; }
    }
}
