using SteamWheel.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Connectivity;
using System.Xml.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.ComponentModel;

// La plantilla de elemento Página básica está documentada en http://go.microsoft.com/fwlink/?LinkID=390556

namespace SteamWheel
{
    
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        // Global variable
        private int imageCount = 0;
        private string[] imgarray;
        private messagePop msgPop = new messagePop();

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        public Settings()
        {
            this.InitializeComponent();
            
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Obtiene el <see cref="NavigationHelper"/> asociado a esta <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Obtiene el modelo de vista para esta <see cref="Page"/>.
        /// Este puede cambiarse a un modelo de vista fuertemente tipada.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Rellena la página con el contenido pasado durante la navegación.  Cualquier estado guardado se
        /// proporciona también al crear de nuevo una página a partir de una sesión anterior.
        /// </summary>
        /// <param name="sender">
        /// El origen del evento; suele ser <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Datos de evento que proporcionan tanto el parámetro de navegación pasado a
        /// <see cref="Frame.Navigate(Type, Object)"/> cuando se solicitó inicialmente esta página y
        /// un diccionario del estado mantenido por esta página durante una sesión
        /// anterior. El estado será null la primera vez que se visite una página.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Mantiene el estado asociado con esta página en caso de que se suspenda la aplicación o
        /// se descarte la página de la memoria caché de navegación.  Los valores deben cumplir los requisitos
        /// de serialización de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">El origen del evento; suele ser <see cref="NavigationHelper"/></param>
        /// <param name="e">Datos de evento que proporcionan un diccionario vacío para rellenar con
        /// un estado serializable.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Registro de NavigationHelper

        /// <summary>
        /// Los métodos proporcionados en esta sección se usan simplemente para permitir
        /// que NavigationHelper responda a los métodos de navegación de la página.
        /// <para>
        /// Debe incluirse lógica específica de página en los controladores de eventos para 
        /// <see cref="NavigationHelper.LoadState"/>
        /// y <see cref="NavigationHelper.SaveState"/>.
        /// El parámetro de navegación está disponible en el método LoadState 
        /// junto con el estado de página mantenido durante una sesión anterior.
        /// </para>
        /// </summary>
        /// <param name="e">Proporciona los datos para el evento y los métodos de navegación
        /// controladores que no pueden cancelar la solicitud de navegación.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        // pass above url in this method as input argument 
        public async void DownloadRSS(string rssURL)
        {
            string webText;
            try 
            { webText = await GetWebPageAsync(rssURL); }
            catch{ throw new Exception("User not found."); }

            myRSS_DownloadStringCompleted(webText);

        }

        //Method to get the image URL
        void myRSS_DownloadStringCompleted(string RSS)
        {
            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile(); 

            //Check if the Network is available
            if (InternetConnectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
            {
                // filter all images from rss located on media:content 
                XNamespace media = XNamespace.Get("http://search.yahoo.com/mrss/");
                //imgarray = XElement.Parse(RSS).Descendants(media + "content")
                 //   .Where(m => m.Attribute("type").Value == "image/jpeg")
                 //   .Select(m => m.Attribute("url").Value)
                 //   .ToArray();
                XName title = new XName();
                string imgarray = XElement.Parse(RSS).Element(title).ToString();

                // check that images are there in rss 
                if (imgarray.Length>0)
                {
                    imageCount = 0;
                    // download images 
                    //DownloadImagefromServer(Convert.ToString(imgarray[0]));
                    TextBlock gtp = new TextBlock();
                    //gtp.Text = Convert.ToString(imgarray[0]);
                    gtp.Text = imgarray;
                    gtp.TextWrapping = TextWrapping.Wrap;
                    SettingsPanel.Children.Add(gtp);
                }
                else
                {
                    msgPop.Pop("No image found in applied RSS link", "Error");
                    
                }

            }
            else
            {
                msgPop.Pop("No network is available.", "Error");
            }
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

        private void getRSS_Click(object sender, RoutedEventArgs e)
        {
            //DownloadRSS("http://www.degraeve.com/flickr-rss/rss.php?tags=nature&tagmode=all&sort=interestingness-desc&num=24");
            DownloadRSS("http://apod.nasa.gov/apod.rss");
        }
    }
}
