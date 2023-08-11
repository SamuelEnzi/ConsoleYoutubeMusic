using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
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
            try
            {
                Debug.WriteLine($"found cookies: {cookies.Count}");

                List<Cookie> converted = new List<Cookie>();

                foreach (var cookie in cookies)
                    converted.Add(cookie.ToSystemNetCookie());

                var json = JsonConvert.SerializeObject(converted);

                popup?.Close();

                Console.WriteLine(json);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), ex.Message); }
            finally
            {
                Environment.Exit(0);

            }
        }
    }
}
