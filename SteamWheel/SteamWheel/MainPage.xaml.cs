using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Text.RegularExpressions;
using SteamWheel.Common;
using Windows.UI.ViewManagement;


// La plantilla de elemento Página en blanco está documentada en http://go.microsoft.com/fwlink/?LinkId=391641

namespace SteamWheel
{

    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private messagePop msgPop = new messagePop();
        private string _steamID64 = null;
        private SteamUser _user = new SteamUser();
        private Game _game;
        //Barra de progreso
        StatusBarProgressIndicator progressbar = StatusBar.GetForCurrentView().ProgressIndicator;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
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

        //Click on Spin it! button
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            progressbar.Text = "Loading...";
            await progressbar.ShowAsync();
            
            gameToPlay.Visibility = Visibility.Collapsed;
                        
            if (Spin_it.IsEnabled)
            {
                Spin_it.IsEnabled = false;
                if (steamIdTextBox.Text.Equals("") || steamIdTextBox.Text.Equals("enter your community name"))
                {
                    msgPop.Pop("Enter your community name / steamID64.", "Info");
                }
                else
                {
                    try
                    {
                        if (!_user.steamId.Equals(steamIdTextBox.Text))
                        {
                            
                            if (Regex.IsMatch(steamIdTextBox.Text, @"^\d+$") && steamIdTextBox.Text.Length == 17 )  //If is all number
                            {
                                _steamID64 = steamIdTextBox.Text;
                            }
                            else
                            {
                                _user.steamId = steamIdTextBox.Text;
                                _steamID64 = await _user.getSteamID64();
                            }                           
                        }
                        
                        List<object> resultados = await _user.getGame(_steamID64);
                        _game = (Game)resultados[0];

                        TextBlock gtp = new TextBlock();
                        gtp.Text = _game.name;
                        gtp.TextWrapping = TextWrapping.Wrap;                       
                        gameToPlay.Content = gtp;
                        
                        m_Image.Source = (ImageSource)resultados[1];
                        hyperLinkImg.NavigateUri = new Uri("http://store.steampowered.com/app/" + _game.appid);
                        string time = null;
                        if (_game.playtime_forever < 60)
                        {
                            time = _game.playtime_forever + " mins.";
                        }
                        else
                            time = String.Format("{0:0.##}", (double)_game.playtime_forever / 60) + " hours.";

                        gameInfo.Text = "Time played: " + time;
                    }

                    //Error on the httprequest
                    catch (NullReferenceException)
                    {
                        msgPop.Pop("Could not connect to Steam: An exception ocurred during a http request.", "Error");
                    }
                    //Any other exception
                    catch (Exception ex)
                    {
                        msgPop.Pop(ex.Message, "Error");
                    }

                }

                await progressbar.HideAsync();
                Spin_it.IsEnabled = true;
            }
        }

        //Click on the Help Button
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Help));
        }

        //Click on the Settings Button
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings));
        }

        // Handlers for Got/Lost Focus of the textBox steamID
        private void steamIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (steamIdTextBox.Text.Equals("enter your community name"))
            {
                steamIdTextBox.Text = "";
                steamIdTextBox.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);              
            }
        }

        private void steamIdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(steamIdTextBox.Text))
            {
                steamIdTextBox.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
                steamIdTextBox.Text = "enter your community name";
            }
        }

        //Click button on Enter key
        private void NumKeyUp(object sender, KeyRoutedEventArgs e)
        {

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Spin_it.Focus(FocusState.Programmatic);
                Button_Click(sender, e);
            }


        }


        //If there is no image (some steam games don't have a header)
        private void m_Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //If the new image is set there was a problem.
            if (m_Image.Source == new BitmapImage(new Uri("http://media.steampowered.com/steamcommunity/public/images/apps/" + _game.appid + "/" + _game.img_logo_url + ".jpg")))
            {
                gameToPlay.NavigateUri = new Uri("http://store.steampowered.com/app/" + _game.appid);
                gameToPlay.Visibility = Visibility.Visible;
                imageFailed.Visibility = Visibility.Visible;
            }
            //If there is no image
            else  
                m_Image.Source = new BitmapImage(new Uri("http://media.steampowered.com/steamcommunity/public/images/apps/" + _game.appid + "/" + _game.img_logo_url + ".jpg"));       
        }

    }
}
