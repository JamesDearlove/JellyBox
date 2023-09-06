using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JellyBox.Services
{
    public class SettingService
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        ApplicationDataContainer userDataContainer;

        public SettingService()
        {
            userDataContainer = localSettings.CreateContainer("UserData", ApplicationDataCreateDisposition.Always);
        }

        public string ServerUri
        {
            get { return (string)userDataContainer.Values["ServerUri"]; }
            set { userDataContainer.Values["ServerUri"] = value; }
        }

        public string Username
        {
            get { return (string)userDataContainer.Values["Username"]; }
            set { userDataContainer.Values["Username"] = value; }
        }

        public string AccessToken
        {
            get { return (string)userDataContainer.Values["AccessToken"]; }
            set { userDataContainer.Values["AccessToken"] = value; }
        }
    }
}
