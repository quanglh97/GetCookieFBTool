using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using xNet;

namespace GetCookieFBTool
{
    public class MyProxyChecker
    {
        public static bool PingHost(string proxy)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                HttpRequest httpRequest = new HttpRequest();
                httpRequest.Proxy = HttpProxyClient.Parse(proxy);
                httpRequest.ReadWriteTimeout = 15000;
                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";// queueHttp.ChromeUserAgent();
                httpRequest.IgnoreProtocolErrors = true;
                httpRequest.SslCertificateValidatorCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                var response = httpRequest.Get($"https://www.facebook.com/", null);
                if (response.IsOK)
                {
                    pingable = true;
                }
            }
            catch (Exception ex)
            {
                return pingable;
            }

            return pingable;
        }

        public static bool Socks5Checking(string _socks5)
        {
            if (string.IsNullOrEmpty(_socks5) || !_socks5.Contains(":"))
                return false;
            HttpRequest httpClient = new HttpRequest();
            httpClient.IgnoreProtocolErrors = true;
            httpClient.AllowAutoRedirect = true;
            httpClient.Cookies = new CookieDictionary();
            httpClient.KeepAlive = true;
            httpClient.UserAgent = Http.ChromeUserAgent();
            string SOURCE = "";
            try
            {
                httpClient.Proxy = Socks5ProxyClient.Parse(_socks5);
                SOURCE = httpClient.Get("http://ip-api.com/json").ToString();
            }
            catch { }
            if (string.IsNullOrEmpty(SOURCE))
            {
                try
                {
                    httpClient.Dispose();
                }
                catch { }
                return false;
            }
            try
            {
                httpClient.Dispose();
            }
            catch { }
            return true;
        }

        public static bool HttpChecking(string _http)
        {
            if (string.IsNullOrEmpty(_http) || !_http.Contains(":"))
                return false;
            HttpRequest httpClient = new HttpRequest();
            httpClient.IgnoreProtocolErrors = true;
            httpClient.AllowAutoRedirect = true;
            httpClient.Cookies = new CookieDictionary();
            httpClient.KeepAlive = true;
            httpClient.UserAgent = Http.ChromeUserAgent();
            httpClient.Proxy = ProxyClient.Parse(ProxyType.Http, _http);
            string SOURCE = "";
            try
            {
                SOURCE = httpClient.Get("http://ip-api.com/json").ToString();
            }
            catch { }
            if (string.IsNullOrEmpty(SOURCE))
            {
                try
                {
                    httpClient.Dispose();
                }
                catch { }
                return false;
            }
            try
            {
                httpClient.Dispose();
            }
            catch { }
            return true;
        }
    }
}
