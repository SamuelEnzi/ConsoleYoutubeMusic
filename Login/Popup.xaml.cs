using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Login
{
    /// <summary>
    /// Interaktionslogik für Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        private Action<List<CoreWebView2Cookie>> cookiesCallback;
        public Popup(Action<List<CoreWebView2Cookie>> cookies)
        {
            InitializeComponent();
            cookiesCallback = cookies;
            LoadUrl("https://www.youtube.com/account");
        }

        private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // if logged in
            if(e.Uri == "https://www.youtube.com/account")
            {
                GetCookies();
            }
        }

        private async Task<bool> CheckCookies()
        {
            var cookies = await UI_WebView.CoreWebView2.CookieManager.GetCookiesAsync("https://www.youtube.com/");
            if (cookies.Count > 0)
                return true;
            return false;
        }

        private async void GetCookies()
        {
            var cookies = await UI_WebView.CoreWebView2.CookieManager.GetCookiesAsync("https://www.youtube.com/");
            cookiesCallback.Invoke(cookies);
        }

        public async void LoadUrl(string url)
        {
            await UI_WebView.EnsureCoreWebView2Async();

            var alreadyLoggedIn = await CheckCookies();
            if (alreadyLoggedIn)
            {
                GetCookies();
                return;
            }

            UI_WebView.CoreWebView2.Navigate(url);
            UI_WebView.NavigationStarting += OnNavigationStarting;
            this.WindowState = WindowState.Normal;
            this.Visibility = Visibility.Visible;
        }
    }
}
