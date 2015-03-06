using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using HtmlAgilityPack;

namespace SteamWheel
{

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

        // Default empty method
        public SteamUser()
        {
            _steamId = "";
        }
        
        // OLD -- Async method to convert any type of steamID to steamID64.
        public async Task<string> steamIDConverter()
        {
                     
            string webText = await GetWebPageAsync("http://steamidconverter.com/" + _steamId);
            HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(webText);
            try
            {            
            HtmlNode divContainer = htmlDoc.GetElementbyId("steamID64");
            return divContainer.InnerText; 
            }
            catch (Exception)
            {
                _steamId = "";
                throw new Exception("User not found.");
            }
        }

        //Async method to get the player steamID64
        public async Task<string> getSteamID64()
        {
            string url = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=" + APIKey + "&vanityurl=" + _steamId;
            string webText = await GetWebPageAsync(url); //async method, saves json_data in "content" string.
            RootObject rootObject = _download_serialized_json_data<RootObject>(webText);
            if (rootObject.response.success.Equals(1))
                return rootObject.response.steamid;
            else
            {
                _steamId = "";
                throw new Exception("User not found.");
            }                
        }

        // Async method to get a random game from an user gamelist
        public async Task<List<object>> getGame(string steamID64)
        {

            //If wifi is connected use higher quality logo
            bool is_wifi_connected = false;
            ConnectionProfile current_connection_for_internet = NetworkInformation.GetInternetConnectionProfile();
            if (current_connection_for_internet.IsWlanConnectionProfile)
            {
                if (current_connection_for_internet.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                {
                    is_wifi_connected = true;
                }
            }

            //URL for the petition
            var url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + APIKey + "&steamid=" + steamID64 + "&format=json&include_appinfo=1";
            string webText;
            try 
            {
            webText = await GetWebPageAsync(url); //async method, saves json_data in "content" string.   
            }
            catch
            {
            throw new Exception("User not found.");
            }                
            var rootObject = _download_serialized_json_data<RootObject>(webText);
            int num_games = rootObject.response.game_count;
            int rand_game = R.Next(num_games);
            Game game;
            try { game = rootObject.response.games[rand_game]; }
            catch (Exception) { throw new Exception("User not found."); }       
            ImageSource imgsrc;
            if (is_wifi_connected)
                imgsrc = new BitmapImage(new Uri("http://cdn.akamai.steamstatic.com/steam/apps/" + game.appid + "/header.jpg"));
            else
                imgsrc = new BitmapImage(new Uri("http://cdn.akamai.steamstatic.com/steam/apps/" + game.appid + "/header_292x136.jpg"));
            //236830
            List<object> result = new List<object>();
            result.Add(game);
            result.Add(imgsrc);      
            return result;
        }   

        // Async method to get data from an url
        private async Task<string> GetWebPageAsync(string url)
        {
            try
            {
                Task<string> getStringTask = (new HttpClient()).GetStringAsync(url);
                string webText = await getStringTask;
                return webText;
            }
            catch
            {
                throw new NullReferenceException();
            }
        }

        // Method to deserialize json data
        private static T _download_serialized_json_data<T>(string json_data) where T : new()
        {
            // if string with JSON data is not empty, deserialize it to class and return its instance 
            return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
        }

        //Test method for metacritic API
        public async Task<object> prueba()
        {
            try
            {
                //Create a client
                HttpClient httpClient = new HttpClient();

                //Headers for the metacritic API
                httpClient.DefaultRequestHeaders.Add("X-Mashape-Key", "LTIsnewGsImsh10Hh7mENQoaYhEOp10U1dtjsnKTzeLLD8o43c");

                //URL
                string url = "https://byroredux-metacritic.p.mashape.com/find/game";

                // This is the postdata
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("platform", "1"));
                postData.Add(new KeyValuePair<string, string>("retry", "4"));
                postData.Add(new KeyValuePair<string, string>("title", "Darksiders"));

                HttpContent content = new FormUrlEncodedContent(postData);
                //"Content-type" is automatically set to "application/x-www-form-urlencoded");

                var response = await httpClient.PostAsync(url, content);

                //will throw an exception if not successful
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();

                RootObject rootObject = null;
                if(!string.IsNullOrEmpty(result))
                    rootObject = JsonConvert.DeserializeObject<RootObject>(result);

                return rootObject;
            }
            catch (Exception e)
            {
                
                return e;
            }


        }

    }
}

