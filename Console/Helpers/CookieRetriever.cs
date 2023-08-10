using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Console.Helpers
{
    public static class CookieRetriever
    {
        private static string totalData = "";

        public static List<Cookie>? GetCookies()
        {
            try
            {
                StartProcessAndCaptureOutput();
                return JsonConvert.DeserializeObject<List<Cookie>>(totalData);
            }
            catch { return null; }
        }

        private static void StartProcessAndCaptureOutput()
        {
            var process = new ProcessStartInfo();
            process.RedirectStandardError = true;
            process.RedirectStandardOutput = true;
            process.FileName = "Login.exe";
            process.UseShellExecute = false;
            var p = new Process();
            p.StartInfo = process;
            p.Start();

            p.BeginOutputReadLine();
            p.OutputDataReceived += OnDataRecieved;
            p.WaitForExit();
        }


        private static void OnDataRecieved(object sender, DataReceivedEventArgs e)
        {
            totalData += e.Data;
        }
    }
}
