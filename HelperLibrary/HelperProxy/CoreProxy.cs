using GetCookieFBTool.HelperLibrary.Facebook;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;

namespace GetCookieFBTool.HelperLibrary.HelperProxy
{
    public class CoreProxy
    {
        public static string ips = "";
        public static bool PingHost(string proxy)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                HttpRequest httpRequest = new HttpRequest();
                httpRequest.Proxy = HttpProxyClient.Parse(proxy);
                httpRequest.ReadWriteTimeout = 15000;
                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";// Http.ChromeUserAgent();
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
        public static void Init_TMProxy(string key)
        {
            // dont use brute force so set this equal 0: wweb api, 1 is pop3/imap; 2 is webapi/pop3/imap

            if (key != null)
            {

                string new_proxy = ChangeIPHelper.GetIPTMProxy(key);
                if (new_proxy.Contains(":") && new_proxy.Split('.').Length >= 4)
                {
                    if (FacebookInstances.checkIP)
                    {
                        if (!PingHost(new_proxy))
                        {
                            return;
                        }
                    }
                    ProxyInstances.Http.Enqueue(new_proxy);
                    return;
                }
            }

        }

        public static void Init_TinsoftProxy(string key)
        {
            // dont use brute force so set this equal 0: wweb api, 1 is pop3/imap; 2 is webapi/pop3/imap

            if (key != null)
            {

                string new_proxy = ChangeIPHelper.GetProxyTinsoft(key);
                if (FacebookInstances.checkIP)
                {
                    if (!PingHost(new_proxy))
                    {
                        return;
                    }
                }
                if (new_proxy.Contains(":") && new_proxy.Split('.').Length >= 4)
                {
                    ProxyInstances.Http.Enqueue(new_proxy);
                    return;
                }
            }
        }

        public static void Init_TinProxy(string key)
        {
            if (key != null)
            {
                string new_proxy = ChangeIPHelper.ChangeTinProxy(key);
                if (FacebookInstances.checkIP)
                {
                    if (!PingHost(new_proxy))
                    {
                        return;
                    }
                }
                if (new_proxy.Contains(":") && new_proxy.Split('.').Length >= 4)
                {
                    ProxyInstances.Http.Enqueue(new_proxy);
                    return;
                }
            }
        }

        public static void Init_MinProxy(string key)
        {
            if (key != null)
            {
                string new_proxy = ChangeIPHelper.ChangeMinProxy(key);
                //if (FacebookInstances.checkIP)
                //{
                //    if (!PingHost(new_proxy))
                //    {
                //        return;
                //    }
                //}
                if (new_proxy.Contains(":") && new_proxy.Split('.').Length >= 4)
                {
                    ProxyInstances.Http.Enqueue(new_proxy);
                    return;
                }
            }
        }

        public static void Init_ProxyMMONet(string key)
        {
            if (key != null)
            {
                string new_proxy = ChangeIPHelper.ChangeProxyMMONet(key, ips);
                //if (FacebookInstances.checkIP)
                //{
                //    if (!PingHost(new_proxy))
                //    {
                //        return;
                //    }
                //}
                if (new_proxy.Contains(":") && new_proxy.Split('.').Length >= 4)
                {
                    ProxyInstances.Http.Enqueue(new_proxy);
                    return;
                }
            }

        }

        public static void Init_ProxyMobie(string key)
        {
            if (key != null)
            {
                var newProxy = ChangeIPHelper.ChangeProxyMobie(key);
                //if (FacebookInstances.checkIP)
                //{
                ////if (!PingHost(newProxy))
                ////{
                ////    return;
                ////}
                
                //    //try
                //    //{
                //    //    var url = $"http://tet.hoanggiaminh.shop/proxy_check?proxy={newProxy}";
                //    //    var client = new RestClient(url);
                //    //    client.Timeout = -1;
                //    //    var request = new RestRequest(Method.GET);
                //    //    request.AddHeader("accept", "*/*");
                //    //    request.AddHeader("X-CSRF-TOKEN", "");
                //    //    IRestResponse response = client.Execute(request);
                //    //    JObject jObject = JObject.Parse(response.Content);
                //    //    if (!jObject["result"].ToString().Contains("ok"))
                //    //        return;
                //    //}
                //    //catch
                //    //{
                //    //}

                //}
                if (newProxy.Contains(":") && newProxy.Split('.').Length >= 3)
                {
                    ProxyInstances.Http.Enqueue(newProxy);
                }
            }
        }

        public static void Init_POCProxy(string key)
        {
            if (key != null)
            {
                var newProxy = ChangeIPHelper.ChangePOCProxy(key);
                if (FacebookInstances.checkIP)
                {
                    //if (!PingHost(newProxy))
                    //{
                    //    return;
                    //}
                    Thread.Sleep(10000);
                    try
                    {
                        var url = $"http://{key.Split(':')[0]}:{key.Split(':')[1]}/api/v1/proxy/public_ip?proxy={key.Split(':')[2]}";
                        var client = new RestClient(url);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("accept", "*/*");
                        request.AddHeader("X-CSRF-TOKEN", "");
                        IRestResponse response = client.Execute(request);
                        JObject jObject = JObject.Parse(response.Content);
                        if (!jObject.ToString().Contains('.'))
                        {
                            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Result" + "\\obcProxy.txt", key + Environment.NewLine);
                            return;
                        }
                            

                    }
                    catch
                    {
                    }

                }
                if (newProxy.Contains(":") && newProxy.Split('.').Length >= 3)
                {
                    ProxyInstances.Http.Enqueue(newProxy);
                }
            }
        }

        //Init_POCWanProxy
        public static void Init_POCWanProxy(string key)
        {
            if (key != null)
            {
                var newProxy = ChangeIPHelper.ChangePOCWanProxy(key);
                if (newProxy != null)
                {
                    if (FacebookInstances.checkIP)
                    {
                        //if (!PingHost(newProxy))
                        //{
                        //    return;
                        //}
                        Thread.Sleep(5000);
                        try
                        {
                            var url = $"http://{key.Split(':')[0]}:{key.Split(':')[1]}/status?proxy={key.Split(':')[0]}:{key.Split(':')[2]}";
                            var client = new RestClient(url);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.GET);
                            request.AddHeader("accept", "*/*");
                            request.AddHeader("X-CSRF-TOKEN", "");
                            IRestResponse response = client.Execute(request);
                            JObject jObject = JObject.Parse(response.Content);
                            if (!jObject["status"].ToString().Contains("True"))
                                return;
                        }
                        catch
                        {
                        }

                    }
                    if (newProxy.Contains(":") && newProxy.Split('.').Length >= 3)
                    {
                        ProxyInstances.Http.Enqueue(newProxy);
                    }
                }
            }
        }

        public static void Init_ZProxy(string key)
        {
            if (key != null)
            {
                var newProxy = ChangeIPHelper.ChangeZProxy(key);
                //if (FacebookInstances.checkIP)
                //{
                //    if (!PingHost(newProxy))
                //    {
                //        return;
                //    }
                //}
                if (newProxy != null)
                {
                    if (newProxy.Contains(":") && newProxy.Split('.').Length >= 3)
                    {
                        ProxyInstances.Http.Enqueue(newProxy);
                    }
                }
            }
        }

        public static void Init_ShopLikeProxy(string key)
        {
            if (key != null)
            {
                var newProxy = ChangeIPHelper.ChangeShopLikeProxy(key);
                if (newProxy != null)
                {
                    if (FacebookInstances.checkIP)
                    {

                        if (!PingHost(newProxy))
                        {
                            return;
                        }

                    }
                    if (newProxy.Contains(":") && newProxy.Split('.').Length >= 3)
                    {
                        ProxyInstances.Http.Enqueue(newProxy);
                    }
                }
            }
        }

        public static void Init_No1Proxy(string key)
        {
            if (key != null)
            {
                var newProxy = ChangeIPHelper.ChangeNo1Proxy(key);
                if (newProxy.Contains(":") && newProxy.Split('.').Length >= 3)
                {
                    ProxyInstances.Http.Enqueue(newProxy);
                }
            }
        }


        public static void InitProxyRotatingV4()
        {

        }

        public static void InitProxyRotatingV6()
        {

        }

    }
}
