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


        /*
        * Async method to get the player info
        */
        public async Task<string> getUserName()
        {
            string webText = await GetWebPageAsync("http://steamidconverter.com/" + _steamId);

            HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(webText);
            HtmlNode divContainer = htmlDoc.GetElementbyId("steamID64");
            return divContainer.InnerText;
        }

        /*
        * Async method to get a random game from an user gamelist
        */
        public async Task<List<object>> getGame(string caca)
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
            var url = "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=" + APIKey + "&steamid=" + caca + "&format=json&include_appinfo=1";
            string webText = await GetWebPageAsync(url); //async method, saves json_data in "content" string.
            var rootObject = _download_serialized_json_data<RootObject>(webText);
            int num_games = rootObject.response.game_count;
            int rand_game = R.Next(num_games);
            Game game = rootObject.response.games[rand_game];
            Uri storelink = new Uri("http://store.steampowered.com/app/" + game.appid);
            ImageSource imgsrc;
            if (is_wifi_connected)
                imgsrc = new BitmapImage(new Uri("http://cdn.akamai.steamstatic.com/steam/apps/" + game.appid + "/header.jpg"));
            else
                imgsrc = new BitmapImage(new Uri("http://cdn.akamai.steamstatic.com/steam/apps/" + game.appid + "/header_292x136.jpg"));
            List<object> result = new List<object>();
            result.Add(game);
            result.Add(imgsrc);
            result.Add(storelink);         
            return result;
        }   

        /*
        * Async method to get the data from an url
        */
        private async Task<string> GetWebPageAsync(string url)
        {
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

