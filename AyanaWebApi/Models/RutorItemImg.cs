using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class RutorItemImg
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string ParentUrl { get; set; }

        public string ChildUrl { get; set; }

        public int RutorItemId { get; set; }

        public RutorItem RutorItem { get; set; }
    }
}
