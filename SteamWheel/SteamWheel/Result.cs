using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWheel
{
    public class Result
    {
        public string name { get; set; } 
        public string score { get; set; }
        public List<string> genre { get; set; }
        public string thumbnail { get; set; }
        public double userscore { get; set; }
        public string summary { get; set; }
        public string platform { get; set; }
        public string publisher { get; set; }
        public string developer { get; set; }
        public string rating { get; set; }
        public string rlsdate { get; set; }
        public string url { get; set; }
    }
}
