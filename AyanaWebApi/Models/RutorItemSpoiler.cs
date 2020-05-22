using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class RutorItemSpoiler
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public int RutorItemId { get; set; }

        public RutorItem RutorItem { get; set; }
    }
}
