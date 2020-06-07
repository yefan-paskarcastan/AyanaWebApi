using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class ImghostGetOriginalsInput
    {
        public List<ImghostParsingInput> ParsingParams { get; set; }

        public List<string> ImgsUri { get; set; }
    }
}
