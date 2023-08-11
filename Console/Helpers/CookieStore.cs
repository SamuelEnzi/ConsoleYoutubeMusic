using Newtonsoft.Json;
using System.Net;
using YoutubeExplode;

namespace Console.Helpers
{
    public static class CookieStore
    {
        private static string filename = "cookies.json"; 

        static CookieStore()
        {
            //set working directory to the same directory as the executable
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
        }

        public static void SaveCookies(List<Cookie> cookies)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(cookies));
        }

        public static List<Cookie> GetCookies()
        {        
            try
            {
                return JsonConvert.DeserializeObject<List<Cookie>>(File.ReadAllText(filename))!;
            }
            catch { return new List<Cookie>(); }
        }

        public static bool ValidateCookies(List<Cookie> cookies)
        {
            try
            {
                var youtube = new YoutubeClient(cookies);
                var playlist = youtube.Playlists.GetVideosAsync("LM").ToBlockingEnumerable().First();
                return true;
            }
            catch { return false; }
        }
    }
}
