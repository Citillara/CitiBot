using CitiBot.Plugins.Twitch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CitiBot.Helpers
{
    internal class ApiHelper
    {
        public static void CallBanApi(string botUsername, string broadcaster, long targetUserId, string reason, int duration = 0)
        {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://citillara.fr/citibot/api/moderation/ban");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                request.ServerCertificateValidationCallback += CheckCertificate;
            }

            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            TwitchUser bot = TwitchUser.GetOrCreateUser(null, botUsername.ToLower(), null);

            string postData = "secret=" + Uri.EscapeDataString(bot.ApiKey);
            postData += "&secret_name=" + Uri.EscapeDataString(bot.Name);
            postData += "&broadcaster=" + Uri.EscapeDataString(broadcaster);
            postData += "&moderator=" + Uri.EscapeDataString(bot.Name);
            postData += "&target_id=" + Uri.EscapeDataString(targetUserId.ToString());
            postData += "&duration=" + Uri.EscapeDataString(duration.ToString());
            postData += "&reason=" + Uri.EscapeDataString(reason);
            byte[] data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                Console.WriteLine(ex.Message);
            }
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseData = reader.ReadToEnd();
                    if (!responseData.StartsWith("OK"))
                    {
                        Console.WriteLine(responseData);
                    }
                }
            }
        }


        public static void CallWhisperApi(string botUsername, long targetUserId, string format, params object[] arg)
        {
            CallWhisperApi(botUsername, targetUserId, string.Format(Thread.CurrentThread.CurrentCulture, format, arg));
        }

        public static void CallWhisperApi(string botUsername, long targetUserId, string message)
        {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://citillara.fr/citibot/api/whisper");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                request.ServerCertificateValidationCallback += CheckCertificate;
            }

            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            TwitchUser bot = TwitchUser.GetOrCreateUser(null, botUsername.ToLower(), null);

            string postData = "secret=" + Uri.EscapeDataString(bot.ApiKey);
            postData += "&secret_name=" + Uri.EscapeDataString(bot.Name);
            postData += "&sender=" + Uri.EscapeDataString(bot.Name);
            postData += "&target_id=" + Uri.EscapeDataString(targetUserId.ToString());
            postData += "&message=" + Uri.EscapeDataString(message);
            byte[] data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                Console.WriteLine(ex.ToString());
            }
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseData = reader.ReadToEnd();
                    if (!responseData.StartsWith("OK"))
                    {
                        Console.WriteLine(responseData);
                    }
                }
            }
        }

        private static bool CheckCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Server hardcoded to loopback on the domain for now
            return true;
        }
    }
}
