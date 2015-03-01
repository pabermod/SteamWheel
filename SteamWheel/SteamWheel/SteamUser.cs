using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Networking.Connectivity;

namespace SteamWheel
{
    /*
    * Structure containing extensive player info.
    */
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

    /*
    * Structure containing game info.
    */
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

    /*
    * Structure the possible responses of the WebAPI call.
    */
    public class Response
    {
        public List<Player> players { get; set; }
        public int game_count { get; set; }
        public List<Game> games { get; set; }
    }

    /*
    * The root object of the WebAPI call.
    */
    public class RootObject
    {
        public Response response { get; set; }
    }

    /*
    * Main Class.
    */
    public class SteamUser
    {
        // Private variables
        private string _steamId;
        private string APIKey = "453412963F6C3B0EBD4ED9C2C79822DD";
        private Random R = new Random();

        // get-set for steamId
        public string steamId
        {
            get { return _steamId; }
            set { _steamId = value; }
        }

        // Default method
        public SteamUser(string steamId)
        {
            _steamId = steamId;
        }


        /*
        * Async method to get the player info
        */
        public async Task<string> getUserName()
        {
            
            var url = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + APIKey + "&steamids=" + _steamId;
            string webText = await GetWebPageAsync(url); //async method, saves json_data in "content" string.
            var rootObject = _download_serialized_json_data<RootObject>(webText);
            return rootObject.response.players[0].personaname;
        }

        /*
        * Async method to get a random game from an user gamelist
        */
        public async Task<List<object>> getGame()
        {
            bool is_wifi_connected = false;
            ConnectionProfile current_connection_for_internet = NetworkInformation.GetInternetConnectionProfile();
            if (current_connection_for_internet.IsWlanConnectionProfile)
            {
                if (current_connection_for_internet.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                {
                    is_wifi_connected = true;
                }
            }

            var url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + APIKey + "&steamid=" + _steamId + "&format=json&include_appinfo=1";
            string webText = await GetWebPageAsync(url); //async method, saves json_data in "content" string.
            var rootObject = _download_serialized_json_data<RootObject>(webText);
            int num_games = rootObject.response.game_count;
            int rand_game = R.Next(num_games);
            ImageSource imgsrc;
            if (is_wifi_connected)
                imgsrc = new BitmapImage(new Uri("http://cdn.akamai.steamstatic.com/steam/apps/" + rootObject.response.games[rand_game].appid + "/header.jpg"));
            else
                imgsrc = new BitmapImage(new Uri("http://cdn.akamai.steamstatic.com/steam/apps/" + rootObject.response.games[rand_game].appid + "/header_292x136.jpg"));
            List<object> result = new List<object>();
            result.Add(rootObject.response.games[rand_game].name);
            result.Add(imgsrc);
            return result;
        }   

        /*
        * Async method to get the data (json formatted) from an url
        */
        private async Task<string> GetWebPageAsync(string url)
        {
            // Start an async task. 
            Task<string> getStringTask = (new HttpClient()).GetStringAsync(url);
            string webText = await getStringTask;
            return webText;
        }

        /*
        * Method to deserialize json data
        */
        private static T _download_serialized_json_data<T>(string json_data) where T : new()
        {
            // if string with JSON data is not empty, deserialize it to class and return its instance 
            return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
        }

    }
}

