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


// La plantilla de elemento Página en blanco está documentada en http://go.microsoft.com/fwlink/?LinkId=391641

namespace SteamWheel
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

        }

        /// <summary>
        /// Se invoca cuando esta página se va a mostrar en un objeto Frame.
        /// </summary>
        /// <param name="e">Datos de evento que describen cómo se llegó a esta página.
        /// Este parámetro se usa normalmente para configurar la página.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Preparar la página que se va a mostrar aquí.

            // TODO: Si la aplicación contiene varias páginas, asegúrese de
            // controlar el botón para retroceder del hardware registrándose en el
            // evento Windows.Phone.UI.Input.HardwareButtons.BackPressed.
            // Si usa NavigationHelper, que se proporciona en algunas plantillas,
            // el evento se controla automáticamente.
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (steamIdTextBox.Text.Equals("") || steamIdTextBox.Text.Equals("your steamID64"))
            {
                messagePop("Input your steamID64.");
            }
            else if (Regex.IsMatch(steamIdTextBox.Text, @"^\d+$"))  //If it's only numbers. (tryparse could overflow if big number)
            {
                SteamUser user = new SteamUser(steamIdTextBox.Text);

                try
                {
                    List<object> resultados = await user.getGame();

                    gameToPlay.Text = (string)resultados[0];
                    m_Image.Source = (ImageSource)resultados[1];
                    hyperLinkImg.NavigateUri = (Uri)resultados[2];
                }

                //If the http request fails, it means an user couldn't be found
                catch (System.Net.Http.HttpRequestException)
                {
                    messagePop("User not found. Make sure the steamID64 is correct.");
                }

                //If there isn't any internet connection.
                catch (System.NullReferenceException)
                {
                    messagePop("An active internet connection is needed.");
                }

                //Any other exception
                catch (Exception ex)
                {
                    messagePop("[ERROR] Exception:" + ex.GetType());
                }

            }
            else
            {
                messagePop("Input a correct steamID64.");
            }
       }


        /*
        * Handlers for the focus of the steam id box
        */
        private void steamIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (steamIdTextBox.Text.Equals("your steamID64"))
            {
                steamIdTextBox.Text = "";
                steamIdTextBox.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);  
            }                 
        }
        private void steamIdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (steamIdTextBox.Text.Equals(""))
            {
                steamIdTextBox.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray); 
                steamIdTextBox.Text = "your steamID64";               
            }
        }


        /*
        * Dialog box with a close button.
        */
        private async void messagePop(string msg)
        {
            var msgDlg = new Windows.UI.Popups.MessageDialog(msg);
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();          
        }

    }
}
