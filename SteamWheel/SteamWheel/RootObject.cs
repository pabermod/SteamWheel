using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWheel
{
    public class RootObject
    {
        public Response response { get; set; }
        public object result { get; set; } 
        public List<Result> results { get; set; }
        public Genres genres { get; set; }

    }
}
