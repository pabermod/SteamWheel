using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace SteamWheel
{  
    public class Player
    {
        public string steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        public string personaname { get; set; }
        public int lastlogoff { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public int personastate { get; set; }
        public string realname { get; set; }
        public string primaryclanid { get; set; }
        public int timecreated { get; set; }
        public int personastateflags { get; set; }
        public string loccountrycode { get; set; }
    }
    public class Game
    {
        public int appid { get; set; }
        public string name { get; set; }
        public int playtime_forever { get; set; }
        public string img_icon_url { get; set; }
        public string img_logo_url { get; set; }
        public bool has_community_visible_stats { get; set; }
        public int? playtime_2weeks { get; set; }
    }
    public class Response
    {
        public List<Player> players { get; set; }
        public int game_count { get; set; }
        public List<Game> games { get; set; }
    }
    public class RootObject
    {
        public Response response { get; set; }
    }
     
    public class SteamUser
    {
        private string _steamId;
        private string APIKey = "453412963F6C3B0EBD4ED9C2C79822DD";
        private string webText = string.Empty;
        private Random R = new Random();

        public string steamId
        {
            get { return _steamId; }
            set { _steamId = value; }
        }

        public SteamUser(string steamId)
        {
            _steamId = steamId;
        }

        public async Task<string> getUserName()
        {
            var url = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + APIKey + "&steamids=" + _steamId;
            webText = await GetWebPageAsync(url); //async method, saves json_data in "content" string.
            var rootObject = _download_serialized_json_data<RootObject>(webText);
            return rootObject.response.players[0].personaname;
        }

        private async Task<string> GetWebPageAsync(string url)
        {
            // Start an async task. 
            Task<string> getStringTask = (new HttpClient()).GetStringAsync(url);
            webText = await getStringTask;
            return webText;
        }

        private static T _download_serialized_json_data<T>(string json_data) where T : new()
        {
            // if string with JSON data is not empty, deserialize it to class and return its instance 
            return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
        }

        public async Task<string> getGame()
        {
            var url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + APIKey + "&steamid=" + _steamId + "&format=json&include_appinfo=1";
            webText = await GetWebPageAsync(url); //async method, saves json_data in "content" string.
            var rootObject = _download_serialized_json_data<RootObject>(webText);
            int num_games = rootObject.response.game_count;
            int rand_game = R.Next(num_games);
            await getLogo(rand_game, rootObject.response.games[rand_game].img_logo_url);
            return rootObject.response.games[rand_game].name;
        }

        private async Task getLogo(int appid, string img_logo_url)
        {
            var url = "http://media.steampowered.com/steamcommunity/public/images/apps/" + appid + "/" + img_logo_url + ".jpg";
        }

    }
}

