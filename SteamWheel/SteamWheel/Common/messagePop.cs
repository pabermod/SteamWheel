using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SteamWheel.Common
{
    class messagePop
    {
        /*
        * Dialog box with a close button.
        */
        public async void Pop(string msg, string title)
        {
            try
            {
                MessageDialog msgDlg = new MessageDialog(msg, title);
                msgDlg.DefaultCommandIndex = 1;
                await msgDlg.ShowAsync();
            }
            catch (Exception)
            {
                return;
            }

        }
    }
}
