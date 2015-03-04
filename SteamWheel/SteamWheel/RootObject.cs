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
        public Result result { get; set; }
        public List<Result> results { get; set; }
    }
}
