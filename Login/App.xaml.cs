using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace Login
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Popup? popup;
        protected override void OnStartup(StartupEventArgs e)
        {
            popup = new Popup(OnCookiesLoaded);
            popup.Show();
        }

        private void OnCookiesLoaded(List<CoreWebView2Cookie> cookies)
        {
            Debug.WriteLine($"found cookies: {cookies.Count}");

            List<Cookie> converted = new List<Cookie>();

            foreach (var cookie in cookies)
                converted.Add(cookie.ToSystemNetCookie());
            
            var json = JsonConvert.SerializeObject(converted);

            popup?.Close();
            
            Console.WriteLine(json);

            Environment.Exit(0);
        }
    }
}
