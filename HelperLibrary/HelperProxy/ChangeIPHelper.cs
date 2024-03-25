using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetCookieFBTool.HelperLibrary.HelperProxy
{
    public class ChangeIPHelper
    {
        public static string GetIPTMProxy(string apikey)
        {
            try
            {
                string sign = CreateMD5(apikey);

                var client = new RestClient("https://tmproxy.com/api/proxy/get-new-proxy");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                var body = "";
                if(ProxyInstances.location.locationTM == -1)
                    body = @"{""api_key"":""" + apikey + @""",""sign"":""" + sign + @""",""id_location"":1}";
                else
                {
                    body = @"{""api_key"":""" + apikey + @""",""sign"":""" + sign + @""",""id_location"":" + ProxyInstances.location.locationTM.ToString() + "}";
                }
                request.AddParameter("application /json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                var jsonResult = (JObject)JsonConvert.DeserializeObject(response.Content);
                string new_proxy = jsonResult["data"]["https"].ToString();

                if (new_proxy.Equals(""))
                    return GetCurrentIPTMProxy(apikey);
                else
                    return new_proxy;

            }
            catch { }

            return "";
        }

        public static string GetCurrentIPTMProxy(string apikey)
        {
            try
            {
                var client = new RestClient("https://tmproxy.com/api/proxy/get-current-proxy");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                var body = @"{""api_key"":""" + apikey + @"""}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                var jsonResult = (JObject)JsonConvert.DeserializeObject(response.Content);
                string new_proxy = jsonResult["data"]["https"].ToString();

                if (new_proxy.Equals(""))
                    return "";
                else
                    return new_proxy;

            }
            catch { }

            return "";
        }

        public static string GetIPMinProxy(string apikey)
        {
            try
            {
                var client = new RestClient("https://dash.minproxy.vn/api/rotating/v1/proxy/get-new-proxy?api_key=" + apikey);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                JObject jObject = JObject.Parse(response.Content);

                if (jObject["code"] == null || !jObject["code"].ToString().Contains("2"))
                {
                    return jObject.ToString();
                }


                return jObject["data"]["http_proxy"].ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Get Proxy Tinsoft error: " + ex.Message, "Error");
            }

            return "";
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string GetProxyTinsoft(string tinsoftKey)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    RestClient client = null;
                    if (ProxyInstances.location.locationTinsoft == -1)
                        client = new RestClient("http://proxy.tinsoftsv.com/api/changeProxy.php?key=" + tinsoftKey + "&location=0"); //TL32ysqjlmjBXSj9cB3DO1DF66QYQkDhY6NyLM
                    else
                    {
                        client = new RestClient("http://proxy.tinsoftsv.com/api/changeProxy.php?key=" + tinsoftKey + "&location=" + ProxyInstances.location.locationTinsoft.ToString());
                    }
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);

                    JObject jObject = JObject.Parse(response.Content);

                    if (jObject["success"].ToString().Contains("alse"))
                    {
                        if (jObject["success"].ToString().Contains("proxy not found!"))
                        {
                            Thread.Sleep(10000);
                            continue;
                        }
                        return GetCurrentProxyTinsoft(tinsoftKey);
                    }


                    return jObject["proxy"].ToString();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Get Proxy Tinsoft error: " + ex.Message, "Error");
                }
                return "";
            }
            return "";
        }


        public static string GetCurrentProxyTinsoft(string tinsoftKey)
        {
            try
            {
                var client = new RestClient("http://proxy.tinsoftsv.com/api/getProxy.php?key=" + tinsoftKey);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                JObject jObject = JObject.Parse(response.Content);

                if (jObject["success"].ToString().Contains("alse"))
                {
                    return "";
                }


                return jObject["proxy"].ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Get Proxy Tinsoft error: " + ex.Message, "Error");
            }

            return "";
        }


        public static void ChangeIPHMA()
        {
            try
            {
                // [1] Turn Off HMA
                System.Diagnostics.Process pProcess_turnOff = new System.Diagnostics.Process();
                pProcess_turnOff.StartInfo.FileName = "C:\\Windows\\System32\\netsh";
                pProcess_turnOff.StartInfo.Arguments = "interface set interface \"HMA! Pro VPN OpenVPN\" disable";
                pProcess_turnOff.StartInfo.UseShellExecute = false;
                pProcess_turnOff.StartInfo.CreateNoWindow = true;
                pProcess_turnOff.StartInfo.RedirectStandardOutput = true;
                pProcess_turnOff.Start();

                string strOutput_turn_on = pProcess_turnOff.StandardOutput.ReadToEnd();
                pProcess_turnOff.WaitForExit();
                Thread.Sleep(TimeSpan.FromSeconds(18));

                // [2] Turn On HMA
                System.Diagnostics.Process pProcess_turnOn = new System.Diagnostics.Process();
                pProcess_turnOn.StartInfo.FileName = "C:\\Windows\\System32\\netsh";
                pProcess_turnOn.StartInfo.Arguments = "interface set interface \"HMA! Pro VPN OpenVPN\" enable";
                pProcess_turnOn.StartInfo.UseShellExecute = false;
                pProcess_turnOn.StartInfo.CreateNoWindow = true;
                pProcess_turnOn.StartInfo.RedirectStandardOutput = true;
                pProcess_turnOn.Start();

                string strOutput_turnOn = pProcess_turnOn.StandardOutput.ReadToEnd();
                pProcess_turnOn.WaitForExit();
                Thread.Sleep(TimeSpan.FromSeconds(20));
            }
            catch (Exception ex) { }

        }

        public static void ChangeIPXProxy(string proxy)
        {
            try
            {
                string xproxy = File.ReadAllText(Directory.GetCurrentDirectory() + "\\xproxy").Trim();
                var client = new RestClient(xproxy + proxy.Trim());
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Cache-Control", "max-age=0");
                request.AddHeader("DNT", "1");
                request.AddHeader("Upgrade-Insecure-Requests", "1");
                //client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36";
                request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                request.AddHeader("Accept-Language", "en,en-US;q=0.9,vi;q=0.8");
                request.AddHeader("Cookie", "Admin-Token=admin-token; sidebarStatus=0");
                IRestResponse response = client.Execute(request);
            }
            catch { }

        }


        public static string getCurrentTinProxy(string key)
        {
            try
            {
                RestClient client = null;
                client = new RestClient("https://api.tinproxy.com/proxy/get-current-proxy?api_key=" + key + "&authen_ips=AUTHEN_IPS");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["status"].ToString().Contains("active"))
                {
                    return jObject["data"]["http_ipv4"].ToString();
                }
            }
            catch
            {

            }
            return "";
        }

        public static string ChangeTinProxy(string key)
        {
            try
            {
                RestClient client = null;
                if (ProxyInstances.location.locationTin == -1 || ProxyInstances.location.locationTin == 0)
                    client = new RestClient("https://api.tinproxy.com/proxy/get-new-proxy?api_key=" + key + "&authen_ips=AUTHEN_IPS" + "&location=random");
                else if (ProxyInstances.location.locationTin == 1)
                {
                    client = new RestClient("https://api.tinproxy.com/proxy/get-new-proxy?api_key=" + key + "&authen_ips=AUTHEN_IPS" + "&location=vn_hanoi");
                }
                else if (ProxyInstances.location.locationTin == 3)
                {
                    client = new RestClient("https://api.tinproxy.com/proxy/get-new-proxy?api_key=" + key + "&authen_ips=AUTHEN_IPS" + "&location=vn_hcm");
                }
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["status"].ToString().Contains("active") && jObject["data"]["http_ipv4"].ToString().Split('.').Length > 3)
                {
                    return jObject["data"]["http_ipv4"].ToString();
                }
                else
                {
                    return getCurrentTinProxy(key);
                }
            }
            catch
            {

            }
            return "";
        }


        public static string GetCurrentMinProxy(string key)
        {
            try
            {
                var url = "https://dash.minproxy.vn/api/rotating/v1/proxy/get-new-proxy?api_key=" + key;
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);

                return jObject["data"]["http_proxy"].ToString();

            }
            catch
            {

            }
            return "";
        }

        public static string ChangeMinProxy(string key)
        {
           
            try
            {
                var url = "https://dash.minproxy.vn/api/rotating/v1/proxy/get-new-proxy?api_key=" + key;
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);

                return jObject["data"]["http_proxy"].ToString() + ':' + jObject["data"]["username"].ToString() + ':' + jObject["data"]["password"].ToString();
                
            }
            catch
            {

            }
            return "";
        }

        public static string ChangeProxyMMONet(string key,  string ips)
        {
            try
            {
                var url = "https://proxymmo.net/api/proxy/get-new-proxy?api_key=" + key;
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if(jObject["status"].ToString().Contains("success"))
                    return jObject["data"]["public_ip"].ToString() + ':' + jObject["data"]["http_port"].ToString() + ':' + jObject["data"]["user"].ToString() + ':' + jObject["data"]["pass"].ToString();
                else
                {
                    return GetCurrentProxyMMONet(key, ips);
                }
            }
            catch
            {
                return "";
            }
            
        }
        public static string GetCurrentProxyMMONet(string key, string ips)
        {

            try
            {
                var url = $"https://proxymmo.net/api/proxy/get-current-proxy?api_key={key}&ips={ips}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);

                return jObject["data"]["public_ip"].ToString() + ':' + jObject["data"]["http_port"].ToString() + ':' + jObject["data"]["user"].ToString() + ':' + jObject["data"]["pass"].ToString();
            }
            catch
            {

            }
            return "";
        }


        public static string ChangeProxyMobie(string key)
        {
            try
            {
                var url = $"http://{key.Split(':')[0]}/proxy_recreat?proxy={key}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["result"].ToString().Contains("ok"))
                    return key;
                else
                {
                    return key;
                }
            }
            catch
            {

            }
            return null;
        }

        public static string ChangePOCProxy(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var url = $"http://{key.Split(':')[0]}:{key.Split(':')[1]}/reset?proxy={key.Split(':')[2]}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["status"].ToString().Contains("true"))
                    return $"{key.Split(':')[0]}:{key.Split(':')[2]}";
                else
                {
                    return $"{key.Split(':')[0]}:{key.Split(':')[2]}";
                }
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        //ChangePOCWanProxy
        public static string ChangePOCWanProxy(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var url = $"http://{key.Split(':')[0]}:{key.Split(':')[1]}/reset?proxy={key.Split(':')[0]}:{key.Split(':')[2]}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["status"].ToString().Contains("true"))
                    return $"{key.Split(':')[0]}:{key.Split(':')[2]}";
                else
                {
                    return $"{key.Split(':')[0]}:{key.Split(':')[2]}";
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static string ChangeZProxy(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var url = $"https://api.zproxy.me/key/get-new-ip?api_key_rotating={key}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["message"].ToString().Contains("SUCCESS"))
                    return $"{jObject["data"]["host"].ToString()}:{jObject["data"]["port"].ToString()}";
                else
                {
                    return CurentZProxy(key);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static string ChangeZProxy2(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var client = new RestClient($"http://51.79.161.132:8080/socks5?apikey={key}&ip={CoreProxy.ips}&changenow=1");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["code"].ToString() == "1")
                    return $"{jObject["message"]}";
                else
                {
                    return CurentZProxy2(key);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }


        public static string CurentZProxy(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var url = $"https://api.zproxy.me/key/get-current-ip?api_key_rotating={key}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["message"].ToString().Contains("SUCCESS"))
                    return $"{jObject["data"]["host"].ToString()}:{jObject["data"]["port"].ToString()}";
                else
                    return "";
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static string CurentZProxy2(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var client = new RestClient($"http://51.79.161.132:8080/socks5?apikey={key}ip={CoreProxy.ips}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["code"].ToString()=="1")
                    return $"{jObject["message"]}";
                else
                    return "";
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static string ChangeShopLikeProxy(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var url = $"http://proxy.shoplike.vn/Api/getNewProxy?access_token={key}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["status"].ToString().Contains("success"))
                    return $"{jObject["data"]["proxy"].ToString()}";
                else
                {
                    return CurentShopLikeProxy(key);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static string CurentShopLikeProxy(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var url = $"http://proxy.shoplike.vn/Api/getCurrentProxy?access_token={key}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if (jObject["status"].ToString().Contains("success"))
                    return $"{jObject["data"]["proxy"].ToString()}";
                else
                    return "";
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "";
        }

        public static string ChangeNo1Proxy(string key)
        {
            //MessageBox.Show("change POC proxy!", "check");
            try
            {
                var url = $"https://app.proxyno1.com/api/change-key-ip/{key}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
            }
            catch
            {

            }
            try
            {
                var url = $"https://app.proxyno1.com/api/key-status/{key}";
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("accept", "*/*");
                request.AddHeader("X-CSRF-TOKEN", "");
                IRestResponse response = client.Execute(request);
                JObject jObject = JObject.Parse(response.Content);
                if ((int)jObject["status"] == 0)
                    return $"{jObject["data"]["proxy"]["ip"].ToString()}:{jObject["data"]["proxy"]["HTTP_IPv4"].ToString()}:{jObject["data"]["authentication"].ToString()}";
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return "";
        }
    }
}
