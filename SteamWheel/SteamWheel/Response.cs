using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWheel
{
    public class Response
    {
        public List<Player> players { get; set; }
        public int game_count { get; set; }
        public List<Game> games { get; set; }
        public string steamid { get; set; }
        public int success { get; set; }
        public string message { get; set; }
    }
}
