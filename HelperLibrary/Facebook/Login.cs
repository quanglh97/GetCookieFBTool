using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using static GetCookieFBTool.HelperLibrary.Facebook.FacebookData;
using System.IO;
using System.Net;
using System.Web;
using RestSharp;
using System.Security.Policy;
using OpenQA.Selenium.DevTools;

namespace GetCookieFBTool.HelperLibrary.Facebook
{
    public static class Login
    {
        public struct HTTPHeader
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public class HTTPRequest
        {
            private CookieContainer cookieContainer;
            public CookieContainer Cookies { get { return cookieContainer; } }
            private List<HTTPHeader> headers = new List<HTTPHeader>();
            public string UserAgent { get; set; }
            public WebProxy Proxy { get; set; }
            public string Accept { get; set; }
            public string Referer { get; set; }
            public HTTPRequest(bool newCookie = false)
            {
                if (newCookie) cookieContainer = new CookieContainer();
            }
            public void AddRawHeaders(string raw)
            {
                try
                {
                    raw = raw.Replace("\r", "");
                    string[] headers = raw.Split('\n');
                    foreach (var header in headers)
                    {
                        if (header.Contains(":"))
                        {
                            string name = header.Split(':')[0];
                            string value = header.Split(':')[1];
                            if (value.StartsWith(" ")) value = value.Substring(1, value.Length - 1);
                            AddHeader(name, value);
                        }
                    }
                }
                catch { }
            }
            public void AddHeader(string name, string value)
            {
                headers.Add(new HTTPHeader() { Name = name, Value = value });
            }
            public void RemoveHeader(string name)
            {
                headers.Remove(headers.Where(i => i.Name == name).FirstOrDefault());
            }
            public void ClearHeader()
            {
                headers.Clear();
            }
            public string GetCookies(string uri)
            {
                string cookies = "";
                foreach (Cookie cok in cookieContainer.GetCookies(new Uri(uri)))
                {
                    cookies += $"{cok.Name}={cok.Value};";
                }
                return cookies;
            }
            public string Get(string uri)
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                if (!string.IsNullOrEmpty(UserAgent)) request.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(Referer)) request.Referer = Referer;
                if (!string.IsNullOrEmpty(Accept)) request.Accept = Accept;
                if (Proxy != null) request.Proxy = Proxy;
                if (cookieContainer != null) request.CookieContainer = cookieContainer;
                if (headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        request.Headers[header.Name] = header.Value;
                    }
                }
                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            public string Post(string uri, string dataPost, string contentType = "application/x-www-form-urlencoded")
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                var data = Encoding.UTF8.GetBytes(dataPost);
                if (!string.IsNullOrEmpty(UserAgent)) request.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(Referer)) request.Referer = Referer;
                if (!string.IsNullOrEmpty(Accept)) request.Accept = Accept;
                if (Proxy != null) request.Proxy = Proxy;
                if (cookieContainer != null) request.CookieContainer = cookieContainer;
                if (headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        request.Headers[header.Name] = header.Value;
                    }
                }
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var urlRsp = response.ResponseUri.OriginalString;
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            public string Post(string uri)
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                if (!string.IsNullOrEmpty(UserAgent)) request.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(Referer)) request.Referer = Referer;
                if (!string.IsNullOrEmpty(Accept)) request.Accept = Accept;
                if (Proxy != null) request.Proxy = Proxy;
                if (cookieContainer != null) request.CookieContainer = cookieContainer;
                if (headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        request.Headers[header.Name] = header.Value;
                    }
                }
                request.Method = "POST";
                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
        }
        public static string Func_2fa(string _2fa)
        {
            return Regex.Match(new HTTPRequest(false).Get("https://2fa.live/tok/" + _2fa.Replace(" ", "")), "\"token\":\"(.*?)\"").Groups[1].Value;
        }
        public static int LoginWWW_HTTPRequest(string UID, string Password, string _2fa, string userAgent, string proxy, out string cookies)
        {
            try
            {
                cookies = "";
                HTTPRequest http = new HTTPRequest(true);
                if (userAgent == null || userAgent == "")
                    http.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
                else
                    http.UserAgent = userAgent;
                if (proxy != null && proxy != "")
                    http.Proxy = new WebProxy(proxy);

                http.AddRawHeaders("sec-fetch-dest: document\r\nsec-fetch-mode: navigate\r\nsec-fetch-site: none");
                string getData = http.Get("https://www.facebook.com");
                string linkLogin = "https://www.facebook.com/login/?privacy_mutation_token" + Regex.Match(getData, "login/\\?privacy_mutation_token(.*?)\"").Groups[1].Value;
                string jaz = Regex.Match(getData, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
                string lsd = Regex.Match(getData, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
                string _js_datr = Regex.Match(getData, "\"_js_datr\",\"(.*?)\"").Groups[1].Value;
                http.Cookies.Add(new System.Net.Cookie() { Name = "_js_datr", Value = _js_datr, Domain = ".facebook.com" });
                string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string Data = $"jazoest={jaz}&lsd={lsd}&email={UID}&login_source=comet_headerless_login&next=&encpass=" + HttpUtility.UrlEncode($"#PWD_BROWSER:0:{timeStamp}:{Password}");
                http.ClearHeader();
                http.AddRawHeaders("sec-fetch-dest: document\r\nsec-fetch-mode: navigate\r\nsec-fetch-site: same-origin\r\nsec-fetch-user: ?1\r\nupgrade-insecure-requests: 1\r\nsec-ch-ua-mobile: ?0");
                string cookiess = http.GetCookies("https://www.facebook.com/");
                string PostData = http.Post(linkLogin, Data, "application/x-www-form-urlencoded");
                if (PostData.Contains("id=\"approvals_code\"") && PostData.Contains("name=\"approvals_code\""))
                {
                    string Maotp = Func_2fa(_2fa);
                    string fbdt = Regex.Match(PostData, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
                    string nh = Regex.Match(PostData, @"name=""nh"" value=""(.*?)""").Groups[1].ToString();
                    jaz = Regex.Match(getData, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
                    Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&no_fido=true&approvals_code={Maotp}&submit%5BContinue%5D=Continue";
                    PostData = http.Post("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                    while (true)
                    {
                        if (PostData.Contains("name=\"name_action_selected\""))
                        {
                            Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&name_action_selected=save_device&submit%5BContinue%5D=Continue";
                            PostData = http.Post("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                            if (PostData.Contains("id=\"checkpointSubmitButton\""))
                            {
                                Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BContinue%5D=Continue";
                                PostData = http.Post("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                                if (PostData.Contains("submit[This was me]"))
                                {
                                    Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BThis+was+me%5D=This+Was+Me";
                                    PostData = http.Post("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (PostData.Contains("success_status\":\"success") && PostData.Contains("<title>Facebook</title>"))
                {
                    if (PostData.Contains("EntryPointRenderer:DisabledAccountCometApp.entrypoint:comet.checkpoint.disabled"))
                    {
                        
                        return 0;
                    }
                    // Thanh cong. Get cookie.
                    cookies = http.GetCookies("https://www.facebook.com");
                    if (cookies.Contains("c_user") && !cookies.Contains("checkpoint"))
                        return 1;
                    else
                        return 0;
                }
                else
                {
                    return -1;
                    //That bai!
                }
            }
            catch (Exception e) { cookies = ""; return -1; }  
        }

        public static int LoginWWWLogin_HTTPRequest(string UID, string Password, string _2fa, string userAgent, string proxy, out string cookies)
        {
            //string UID = "";
            //string Email = "";
            //string Password = "";
            //string _2fa = "";
            //string cookies = "";

            try
            {
                cookies = "";

                HTTPRequest http = new HTTPRequest(true);
                if (userAgent == null || userAgent == "")
                    http.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";
                else
                    http.UserAgent = userAgent;
                if (proxy != null && proxy != "")
                    http.Proxy = new WebProxy(proxy);

                http.AddRawHeaders("sec-fetch-dest: document\r\nsec-fetch-mode: navigate\r\nsec-fetch-site: none");
                string getData = http.Get("https://www.facebook.com/login");
                string jaz = Regex.Match(getData, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
                string lsd = Regex.Match(getData, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
                string _js_datr = Regex.Match(getData, "\"_js_datr\",\"(.*?)\"").Groups[1].Value;
                http.Cookies.Add(new System.Net.Cookie() { Name = "_js_datr", Value = _js_datr, Domain = ".facebook.com" });
                string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string Data = $"jazoest={jaz}&lsd={lsd}&email={UID}&login_source=comet_headerless_login&next=&encpass=" + HttpUtility.UrlEncode($"#PWD_BROWSER:0:{timeStamp}:{Password}");
                http.ClearHeader();
                http.AddRawHeaders("sec-fetch-dest: document\r\nsec-fetch-mode: navigate\r\nsec-fetch-site: same-origin\r\nsec-fetch-user: ?1\r\nupgrade-insecure-requests: 1\r\nsec-ch-ua-mobile: ?0");
                string cookiess = http.GetCookies("https://www.facebook.com/");
                string PostData = http.Post("https://www.facebook.com/login/device-based/regular/login/?login_attempt=1&lwv=120&lwc=1348028", Data, "application/x-www-form-urlencoded");
                if (PostData.Contains("id=\"approvals_code\"") && PostData.Contains("name=\"approvals_code\""))
                {
                    string Maotp = Func_2fa(_2fa);
                    string fbdt = Regex.Match(PostData, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
                    string nh = Regex.Match(PostData, @"name=""nh"" value=""(.*?)""").Groups[1].ToString();
                    jaz = Regex.Match(getData, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
                    Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&no_fido=true&approvals_code={Maotp}&submit%5BContinue%5D=Continue";
                    PostData = http.Post("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                    while (true)
                    {
                        if (PostData.Contains("name=\"name_action_selected\""))
                        {
                            Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&name_action_selected=save_device&submit%5BContinue%5D=Continue";
                            PostData = http.Post("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                            if (PostData.Contains("id=\"checkpointSubmitButton\""))
                            {
                                Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BContinue%5D=Continue";
                                PostData = http.Post("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                                if (PostData.Contains("submit[This was me]"))
                                {
                                    Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BThis+was+me%5D=This+Was+Me";
                                    PostData = http.Post("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (PostData.Contains("success_status\":\"success") && PostData.Contains("<title>Facebook</title>"))
                {
                    if (PostData.Contains("EntryPointRenderer:DisabledAccountCometApp.entrypoint:comet.checkpoint.disabled"))
                    {
                        return 0;
                    }
                    // Thanh cong. Get cookie.
                    cookies = http.GetCookies("https://www.facebook.com");
                    if (cookies.Contains("c_user") && !cookies.Contains("checkpoint"))
                        return 1;
                    else
                        return 0;
                }
                else
                {
                    return -1;
                    //That bai!
                }
            }
            catch (Exception e) { cookies = ""; return -1; }
        }

        public static bool LoginMethodMBasic_XNET(Request request, string email, string password, string authenCode)
        {
            FacebookData data = new FacebookData();

            var fbField =  data.GetFieldsMBasicFB(request);

            if (fbField == null)

                return false;

            #region POST LOGIN

            request.AddHeader("referer", "https://mbasic.facebook.com/login");

            request.AddParam("lsd", fbField.lsd);

            request.AddParam("jazoest", fbField.jazoest);

            request.AddParam("m_ts", fbField.m_ts);

            request.AddParam("li", fbField.li);

            request.AddParam("try_number", "0");

            request.AddParam("unrecognized_tries", "0");

            request.AddParam("email", email);

            request.AddParam("pass", password);

            request.AddParam("login", "Log In");

            request.AddParam("bi_xrwh", "0");

            string b = request.RequestPost("https://mbasic.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100");

            #endregion

            var secretKey = Base32Encoding.ToBytes(authenCode);
            var totp = new Totp(secretKey);
            var code = totp.ComputeTotp();

            string uriCP = request.Uri();

            if (uriCP.Contains("/home.php"))

                return true;

            var fbCp = data.GetFB_DTSG_MBasic(request, uriCP);

            if (fbCp == null)

                return false;

            #region POST LOGIN 2FA

            request.AddParam("fb_dtsg", fbCp.fb_dtsg);

            request.AddParam("jazoest", fbCp.jazoest);

            request.AddParam("nh", fbCp.nh);

            request.AddParam("checkpoint_data", "");

            request.AddParam("approvals_code", code);

            request.AddParam("codes_submitted", "0");

            request.AddParam("submit[Submit Code]", "Submit Code");

            request.RequestPost("https://mbasic.facebook.com/login/checkpoint/");

            #endregion


            #region POST SAVE DEVICE

            request.AddParam("fb_dtsg", fbCp.fb_dtsg);

            request.AddParam("jazoest", fbCp.jazoest);

            request.AddParam("nh", fbCp.nh);

            request.AddParam("checkpoint_data", "");

            request.AddParam("name_action_selected", "save_device");

            request.AddParam("submit[Continue]", "Continue");

            string a = request.RequestPost("https://mbasic.facebook.com/login/checkpoint/");

            #endregion

            if (request.GetCookie().Contains("c_user"))

                return true;

            else

                return false;
        }

        public static bool LoginMethodMFB_XNET(Request request, string email, string password, string authenCode)
        {
            FacebookData data = new FacebookData();

            var fbField = data.GetFieldsMFB(request);

            if (fbField == null)

                return false;

            #region POST LOGIN

            //request.AddHeader("referer", "https://m.facebook.com/login");
            if(fbField.lsd !="")
                request.AddParam("lsd", fbField.lsd);
            if (fbField.jazoest != "")
                request.AddParam("jazoest", fbField.jazoest);
            if (fbField.m_ts != "")
                request.AddParam("m_ts", fbField.m_ts);
            if (fbField.li != "")
                request.AddParam("li", fbField.li);

            request.AddParam("try_number", "0");

            request.AddParam("unrecognized_tries", "0");

            request.AddParam("email", email);

            request.AddParam("pass", password);

            request.AddParam("login", "Log In");

            request.AddParam("bi_xrwh", "0");

            string b = request.RequestPost("https://m.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100");

            #endregion

            var secretKey = Base32Encoding.ToBytes(authenCode);
            var totp = new Totp(secretKey);
            var code = totp.ComputeTotp();

            string uriCP = request.Uri();

            if (uriCP.Contains("/home.php"))

                return true;

            var fbCp = data.GetFB_DTSG_MFB(request, uriCP);

            if (fbCp == null)

                return false;

            #region POST LOGIN 2FA

            request.AddParam("fb_dtsg", fbCp.fb_dtsg);

            request.AddParam("jazoest", fbCp.jazoest);

            request.AddParam("nh", fbCp.nh);

            request.AddParam("checkpoint_data", "");

            request.AddParam("approvals_code", code);

            request.AddParam("codes_submitted", "0");

            request.AddParam("submit[Submit Code]", "Submit Code");

            var a = request.RequestPost("https://m.facebook.com/login/checkpoint/");

            #endregion


            #region POST SAVE DEVICE

            request.AddParam("fb_dtsg", fbCp.fb_dtsg);

            request.AddParam("jazoest", fbCp.jazoest);

            request.AddParam("nh", fbCp.nh);

            request.AddParam("checkpoint_data", "");

            request.AddParam("name_action_selected", "save_device");

            request.AddParam("submit[Continue]", "Continue");

            string respone = request.RequestPost("https://m.facebook.com/login/checkpoint/");

            #endregion

            if (request.GetCookie().Contains("c_user"))

                return true;

            else

                return false;
        }

        public static bool LoginMethodMBasic_XNET(Request request, string email, string password, string authenCode, out string cookies)
        {
            cookies = "";
            FacebookData data = new FacebookData();

            var fbField = data.GetFieldsMBasicFB(request);

            if (fbField == null)

                return false;

            #region POST LOGIN

            request.AddHeader("referer", "https://mbasic.facebook.com/login");

            request.AddParam("lsd", fbField.lsd);

            request.AddParam("jazoest", fbField.jazoest);

            request.AddParam("m_ts", fbField.m_ts);

            request.AddParam("li", fbField.li);

            request.AddParam("try_number", "0");

            request.AddParam("unrecognized_tries", "0");

            request.AddParam("email", email);

            request.AddParam("pass", password);

            request.AddParam("login", "Log In");

            request.AddParam("bi_xrwh", "0");

            string b = request.RequestPost("https://mbasic.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100");

            #endregion

            string uriCP = request.Uri();

            if (uriCP.Contains("/home.php"))
            {
                cookies = request.GetCookie();
                return true;
            }

            var fbCp = data.GetFB_DTSG_MBasic(request, uriCP);

            if (fbCp == null)

                return false;
            if (authenCode != null && authenCode.Length > 7)
            {
                var code = "";
                try
                {
                    var secretKey = Base32Encoding.ToBytes(authenCode);
                    var totp = new Totp(secretKey);
                    code = totp.ComputeTotp();
                }
                catch { }

                #region POST LOGIN 2FA

                request.AddParam("fb_dtsg", fbCp.fb_dtsg);

                request.AddParam("jazoest", fbCp.jazoest);

                request.AddParam("nh", fbCp.nh);

                request.AddParam("checkpoint_data", "");

                request.AddParam("approvals_code", code);

                request.AddParam("codes_submitted", "0");

                request.AddParam("submit[Submit Code]", "Submit Code");

                request.RequestPost("https://mbasic.facebook.com/login/checkpoint/");

                #endregion
            }

            try
            {
                #region POST SAVE DEVICE

                request.AddParam("fb_dtsg", fbCp.fb_dtsg);

                request.AddParam("jazoest", fbCp.jazoest);

                request.AddParam("nh", fbCp.nh);

                request.AddParam("checkpoint_data", "");

                request.AddParam("name_action_selected", "save_device");

                request.AddParam("submit[Continue]", "Continue");

                string a = request.RequestPost("https://mbasic.facebook.com/login/checkpoint/");

                #endregion
            }
            catch { }

            if (request.GetCookie().Contains("c_user"))
            {
                cookies = request.GetCookie();
                return true;
            }

            else

                return false;
        }

        public static int LoginMethodWWW_XNET(string UID, string Password, string _2fa, string userAgent, string proxy, out string cookies)
        {
            cookies = "";
            Request request = new Request("", userAgent, proxy);
            var linkFix = FacebookInstances.lsLinkFix[0];
            try
            {
                request.RequestGet(linkFix);
            }
            catch { }
            //Request request = new Request();
            string email = UID;
            string password = Password;
            string authenCode = _2fa;

            request.AddHeader("sec-fetch-dest", "document");
            request.AddHeader("sec-fetch-mode", "navigate");
            request.AddHeader("sec-fetch-site", "none");
            var response = request.RequestGet("https://www.facebook.com");
            string linkLogin = "https://www.facebook.com/login/?privacy_mutation_token" + Regex.Match(response, "login/\\?privacy_mutation_token(.*?)\"").Groups[1].Value;
            string jaz = Regex.Match(response, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
            string lsd = Regex.Match(response, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
            string _js_datr = Regex.Match(response, "\"_js_datr\",\"(.*?)\"").Groups[1].Value;

            #region POST LOGIN

            request.AddHeader("accept-language", "en-US,en;q=0.9");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-fetch-dest", "document");
            request.AddHeader("sec-fetch-mode", "navigate");
            request.AddHeader("sec-fetch-site", "same-origin");
            request.AddHeader("sec-fetch-user", "?1");
            request.AddHeader("upgrade-insecure-requests", "1");

            request.AddCookie("_js_datr=" + _js_datr);
            string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            var encpass = HttpUtility.UrlEncode($"#PWD_BROWSER:0:{timeStamp}:{password}");
            //request.AddParam("encpass", encpass);
            string Data = $"jazoest={jaz}&lsd={lsd}&email={email}&login_source=comet_headerless_login&next=&encpass=" + encpass;
            cookies = request.GetCookie();
            string PostData = request.RequestPost(linkLogin, Data, "application/x-www-form-urlencoded");
            #endregion
            if (PostData.Contains("id=\"approvals_code\"") && PostData.Contains("name=\"approvals_code\""))
            {
                var secretKey = Base32Encoding.ToBytes(authenCode);
                var totp = new Totp(secretKey);
                var code = totp.ComputeTotp();
                string fbdt = Regex.Match(PostData, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
                string nh = Regex.Match(PostData, @"name=""nh"" value=""(.*?)""").Groups[1].ToString();
                Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&no_fido=true&approvals_code={code}&submit%5BContinue%5D=Continue";
                PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                while (true)
                {
                    if (PostData.Contains("name=\"name_action_selected\""))
                    {
                        Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&name_action_selected=save_device&submit%5BContinue%5D=Continue";
                        PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                        if (PostData.Contains("id=\"checkpointSubmitButton\""))
                        {
                            Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BContinue%5D=Continue";
                            PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                            if (PostData.Contains("submit[This was me]"))
                            {
                                Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BThis+was+me%5D=This+Was+Me";
                                PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (PostData.Contains("success_status\":\"success") && PostData.Contains("<title>Facebook</title>"))
            {
                if (PostData.Contains("EntryPointRenderer:DisabledAccountCometApp.entrypoint:comet.checkpoint.disabled"))
                {
                    return 0;
                }
                // Thanh cong. Get cookie.
                cookies = request.GetCookie();
                if (cookies.Contains("c_user") && !cookies.Contains("checkpoint"))
                    return 1;
                else
                    return 0;
            }
            else
            {
                return -1;
                //That bai!
            }
        }

        public static int LoginMethodLoginWWW_XNET(string UID, string Password, string _2fa, string userAgent, string proxy, out string cookies)
        {
            cookies = "";
            Request request = new Request("", userAgent, proxy);
            var linkFix = FacebookInstances.lsLinkFix[0];
            try
            {
                request.RequestGet(linkFix);
            }
            catch { }
            //Request request = new Request();
            string email = UID;
            string password = Password;
            string authenCode = _2fa;

            request.AddHeader("sec-fetch-dest", "document");
            request.AddHeader("sec-fetch-mode", "navigate");
            request.AddHeader("sec-fetch-site", "none");
            var response = request.RequestGet("https://www.facebook.com/login");
            //string linkLogin = "https://www.facebook.com/login/?privacy_mutation_token" + Regex.Match(response, "login/\\?privacy_mutation_token(.*?)\"").Groups[1].Value;
            string jaz = Regex.Match(response, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
            string lsd = Regex.Match(response, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
            string _js_datr = Regex.Match(response, "\"_js_datr\",\"(.*?)\"").Groups[1].Value;

            #region POST LOGIN

            request.AddHeader("accept-language", "en-US,en;q=0.9");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-fetch-dest", "document");
            request.AddHeader("sec-fetch-mode", "navigate");
            request.AddHeader("sec-fetch-site", "same-origin");
            request.AddHeader("sec-fetch-user", "?1");
            request.AddHeader("upgrade-insecure-requests", "1");

            request.AddCookie("_js_datr=" + _js_datr);
            string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            var encpass = HttpUtility.UrlEncode($"#PWD_BROWSER:0:{timeStamp}:{password}");
            //request.AddParam("encpass", encpass);
            string Data = $"jazoest={jaz}&lsd={lsd}&email={email}&login_source=comet_headerless_login&next=&encpass=" + encpass;
            cookies = request.GetCookie();
            string PostData = request.RequestPost("https://www.facebook.com/login/device-based/regular/login/?login_attempt=1&lwv=120&lwc=1348028", Data, "application/x-www-form-urlencoded");
            #endregion
            if (PostData.Contains("id=\"approvals_code\"") && PostData.Contains("name=\"approvals_code\""))
            {
                var secretKey = Base32Encoding.ToBytes(authenCode);
                var totp = new Totp(secretKey);
                var code = totp.ComputeTotp();
                string fbdt = Regex.Match(PostData, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
                string nh = Regex.Match(PostData, @"name=""nh"" value=""(.*?)""").Groups[1].ToString();
                Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&no_fido=true&approvals_code={code}&submit%5BContinue%5D=Continue";
                PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                while (true)
                {
                    if (PostData.Contains("name=\"name_action_selected\""))
                    {
                        Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&name_action_selected=save_device&submit%5BContinue%5D=Continue";
                        PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                        if (PostData.Contains("id=\"checkpointSubmitButton\""))
                        {
                            Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BContinue%5D=Continue";
                            PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                            if (PostData.Contains("submit[This was me]"))
                            {
                                Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BThis+was+me%5D=This+Was+Me";
                                PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (PostData.Contains("success_status\":\"success") && PostData.Contains("<title>Facebook</title>"))
            {
                if (PostData.Contains("EntryPointRenderer:DisabledAccountCometApp.entrypoint:comet.checkpoint.disabled"))
                {
                    return 0;
                }
                // Thanh cong. Get cookie.
                cookies = request.GetCookie();
                if (cookies.Contains("c_user") && !cookies.Contains("checkpoint"))
                    return 1;
                else
                    return 0;
            }
            else
            {
                return -1;
                //That bai!
            }
        }




        //public static int LoginMethodWWW_XNET(string UID, string Password, string _2fa, string userAgent, string proxy, out string cookies)
        //{
        //    cookies = "";
        //    Request request = new Request();
        //    string email = "61554208091910";
        //    string password = "gbPJPJ8OgV4P";
        //    string authenCode = "AVZF3B4CI7BOSSEJHWRT3V2XYC5PMKFX";
        //    request.AddHeader("sec-fetch-dest", "document");
        //    request.AddHeader("sec-fetch-mode", "navigate");
        //    request.AddHeader("sec-fetch-site", "none");
        //    var response = request.RequestGet("https://www.facebook.com");
        //    string linkLogin = "https://www.facebook.com/login/?privacy_mutation_token" + Regex.Match(response, "login/\\?privacy_mutation_token(.*?)\"").Groups[1].Value;
        //    string jaz = Regex.Match(response, @"name=""jazoest"" value=""(.*?)""").Groups[1].ToString();
        //    string lsd = Regex.Match(response, @"name=""lsd"" value=""(.*?)""").Groups[1].ToString();
        //    string _js_datr = Regex.Match(response, "\"_js_datr\",\"(.*?)\"").Groups[1].Value;
        //    #region POST LOGIN
        //    request.AddHeader("accept-language", "en-US,en;q=0.9");
        //    request.AddHeader("sec-ch-ua-mobile", "?0");
        //    request.AddHeader("sec-fetch-dest", "document");
        //    request.AddHeader("sec-fetch-mode", "navigate");
        //    request.AddHeader("sec-fetch-site", "same-origin");
        //    request.AddHeader("sec-fetch-user", "?1");
        //    request.AddHeader("upgrade-insecure-requests", "1");
        //    request.AddCookie("_js_datr=" + _js_datr);
        //    string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        //    var encpass = HttpUtility.UrlEncode($"#PWD_BROWSER:0:{timeStamp}:{password}");
        //    //request.AddParam("encpass", encpass);
        //    string Data = $"jazoest={jaz}&lsd={lsd}&email={email}&login_source=comet_headerless_login&next=&encpass=" + encpass;
        //     cookies = request.GetCookie();
        //    string PostData = request.RequestPost(linkLogin, Data, "application/x-www-form-urlencoded");
        //    #endregion
        //    if (PostData.Contains("id=\"approvals_code\"") && PostData.Contains("name=\"approvals_code\""))
        //    {
        //        var secretKey = Base32Encoding.ToBytes(authenCode);
        //        var totp = new Totp(secretKey);
        //        var code = totp.ComputeTotp();
        //        string fbdt = Regex.Match(PostData, @"name=""fb_dtsg"" value=""(.*?)""").Groups[1].ToString();
        //        string nh = Regex.Match(PostData, @"name=""nh"" value=""(.*?)""").Groups[1].ToString();
        //        Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&no_fido=true&approvals_code={code}&submit%5BContinue%5D=Continue";
        //        PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
        //        while (true)
        //        {
        //            if (PostData.Contains("name=\"name_action_selected\""))
        //            {
        //                Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&name_action_selected=save_device&submit%5BContinue%5D=Continue";
        //                PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
        //                if (PostData.Contains("id=\"checkpointSubmitButton\""))
        //                {
        //                    Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BContinue%5D=Continue";
        //                    PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
        //                    if (PostData.Contains("submit[This was me]"))
        //                    {
        //                        Data = $"jazoest={jaz}&fb_dtsg={fbdt}&nh={nh}&submit%5BThis+was+me%5D=This+Was+Me";
        //                        PostData = request.RequestPost("https://www.facebook.com/checkpoint/?next", Data, "application/x-www-form-urlencoded");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    if (PostData.Contains("success_status\":\"success"))
        //    {
        //        // Thanh cong. Get cookie.
        //        cookies = request.GetCookie();
        //        return 1;
        //    }
        //    else
        //    {
        //        return 0;
        //        //That bai!
        //    }
        //}

        public static string getTokenRequest(Request request, string uid, string authenCode)
        {

            var secretKey = Base32Encoding.ToBytes(authenCode);
            var totp = new Totp(secretKey);
            var code = totp.ComputeTotp();
            string resFields = request.RequestGet("https://business.facebook.com/content_management");
            var regexString = "\\[\"DTSGInitialData\",\\[\\],\\{\"token\":\"(.*?)\"";
            var tokenEAAG = "";
            var fb_dtsg = Regex.Match(resFields, regexString).Groups[1].Value;
            if (request.Uri().Contains("security/twofactor/reauth/?"))
            {
                var response = request.RequestPost("https://business.facebook.com/security/twofactor/reauth/enter/", string.Concat(new string[]
                        {
                        "approvals_code=",
                        code,
                        "&save_device=false&__user=",
                        uid,
                        "&__a=1&__dyn=7xeUmFoO3-SudwCwRyU8EKnFwLBwCwXCwAxu13wqovzEdEnxy7Eiwzwq8S2S4okwAwSz82EyEqx60DU4m0_87Oq1eK1VgC11x-9xm1WxO4Uowuo9oeUa85vzo1eE4aUS1vwnEfU7e2l2UtggzE2DwiUmwnGwWx2365Ey19zUuxe0y83mwkE5G4E7K1uDwau58Gm0hi4Ejyo-3qazo8U3ywbS1bwzw&__csr=&__req=9&__hs=19108.BP:DEFAULT.2.0.0.0.&dpr=1.5&__ccg=EXCELLENT&__rev=1005404255&__s=n7y7to:i7topm:vo61pg&__hsi=7090761015361603355-0&__comet_req=0&fb_dtsg=",
                        fb_dtsg,
                        "&jazoest=21954&lsd=Qf7z54nRePaFgnLxvbGQyN&__spin_r=1005404255&__spin_b=trunk&__spin_t=1650946451&__jssesw=1"
                        }), "application/x-www-form-urlencoded").ToString();
                if (response.Contains("confirmed")){
                    string result = request.RequestGet("https://business.facebook.com/content_management");
                    tokenEAAG = "EAAG" + Regex.Match(result, "\"accessToken\":\"EAAG(.*?)\"").Groups[1].Value;
                }
            }
            else
            {
                tokenEAAG = "EAAG" + Regex.Match(resFields, "\"accessToken\":\"EAAG(.*?)\"").Groups[1].Value;
            }
            if (tokenEAAG.Length < 6)
                return null;
            return tokenEAAG;
        }
        public static string GetSig(Dictionary<string, string> formData)
        {
            string sig = "";
            foreach (string key in formData.Keys)
            {
                sig = sig + key + "=" + formData[key];
            }
            sig = HashCalculate(sig + "62f8ce9f74b12f84c123cc23437a4a32");
            return sig;
        }
        public static int EToInt(string eregAPIValue)
        {
            eregAPIValue = eregAPIValue.ToLower();
            if (eregAPIValue.Contains("e"))
            {
                string[] numAndTenPowByPair = eregAPIValue.Split(new char[]
                {
                    'e'
                }, StringSplitOptions.RemoveEmptyEntries);
                if (numAndTenPowByPair.Length == 2)
                {
                    return (int)(int.Parse(numAndTenPowByPair[0]) * Math.Pow(10.0, int.Parse(numAndTenPowByPair[1])));
                }
            }
            return 0;
        }
        public static int NumberCalculate(int min, int max) => new Random().Next(min, max + 1);
        public static string HashCalculate(string str)
        {
            HashAlgorithm hashAlgorithm = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            byte[] hash = hashAlgorithm.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
        public static string StringCalculate(int limit)
        {
            if (limit <= 0)
            {
                limit = 10;
            }
            Random rd = new Random();
            string text = "abcdefghijklmnopqrstuvwxyz";
            char firstText = text[rd.Next(text.Length)];
            string possible = "abcdefghijklmnopqrstuvwxyz0123456789";
            string randomText = firstText.ToString();
            for (int i = 0; i < limit - 1; i++)
            {
                randomText += possible[rd.Next(possible.Length)].ToString();
            }
            return randomText;
        }

        public static Dictionary<string, string> Sort(Dictionary<string, string> obj)
        {
            IEnumerable<string> enumerable = from key in obj.Keys
                                             orderby key
                                             select key;
            Dictionary<string, string> sortedObj = new Dictionary<string, string>();
            foreach (string sortedKey in enumerable)
            {
                sortedObj.Add(sortedKey, obj[sortedKey]);
            }
            return sortedObj;
        }

        public static string Stringify(Dictionary<string, string> parameters)
        {
            return string.Join("&", from kvp in parameters
                                    select kvp.Key + "=" + kvp.Value);
        }

        public static string ResultData(string uid, string pass, string authenCode=null, string userAgent=null, string proxy=null)
        {
            Random rand2 = new Random();
            string sim = NumberCalculate(EToInt("2e4"), EToInt("4e4")).ToString();
            string deviceID = Guid.NewGuid().ToString();
            string adID = Guid.NewGuid().ToString();
            string sim_serials = $"[\"00840075{rand2.Next(100000, 999999)}{rand2.Next(100000, 999999)}\"]";
            string machineId = StringCalculate(24);

            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                {
                    "adid",
                    adID
                },
                {
                    "format",
                    "json"
                },
                {
                    "device_id",
                    deviceID
                },
                {
                    "email",
                    uid
                },
                {
                    "password",
                     pass
                },
                {
                    "generate_analytics_claim",
                    "1"
                },
                {
                    "community_id",
                    ""
                },
                {
                    "cpl",
                    "true"
                },
                {
                    "try_num",
                    "1"
                },
                {
                    "family_device_id",
                    deviceID
                },
                {
                    "sim_serials",
                    sim_serials
                },
                {
                    "credentials_type",
                    "device_based_login_password"
                },
                {
                    "generate_session_cookies",
                    "1"
                },
                {
                    "error_detail_type",
                    "button_with_disabled"
                },
                {
                    "source",
                    "login"
                },
                {
                    "machine_id",
                    machineId
                },
                {
                    "jazoest",
                    rand2.Next(10000,99999).ToString()
                },

                {
                    "meta_inf_fbmeta",
                    ""
                },
                {
                    "advertiser_id",
                    adID
                },
                {
                    "encrypted_msisdn",
                    ""
                },
                {
                    "currently_logged_in_userid",
                    "0"
                },
                {
                    "locale",
                    "vi_VN"
                },
                {
                    "client_country_code",
                    "VN"
                },
                {
                    "method",
                    "auth.login"
                },
                {
                    "fb_api_req_friendly_name",
                    "authenticate"
                },
                {
                    "fb_api_caller_class",
                    "com.facebook.account.login.protocol.Fb4aAuthHandler"
                },
                {
                    "api_key",
                    "882a8490361da98702bf97a021ddc14d"
                }
            };

            string sig = GetSig(Sort(formData));

            formData.Add("sig", sig);

            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
            {"X-Fb-Connection-Quality","EXCELLENT"},
            {"X-Fb-Friendly-Name","authenticate"},
            {"X-Tigon-Is-Retry","False"},
            {"X-Fb-Connection-Type","unknown"},
            {"x-fb-sim-hni",sim},
            {"x-fb-net-hni",sim},
            {"X-FB-Socket-Option","TCP_CONGESTION=bbr"},
            {"content-type","application/x-www-form-urlencoded"}
            };

            string dataPost = Stringify(formData);


            var restClient = new RestClient();
            restClient.Timeout = -1;
            restClient.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";
            if (!string.IsNullOrEmpty(proxy))
            {
                switch (proxy.Split(':').Count())
                {
                    case 2:
                        {
                            restClient.Proxy = new WebProxy(proxy.Split(':')[0], int.Parse(proxy.Split(':')[1]));
                            break;
                        }
                    case 4:
                        {
                            WebProxy webProxy = new WebProxy(proxy.Split(':')[0], int.Parse(proxy.Split(':')[1]));
                            webProxy.Credentials = new NetworkCredential(proxy.Split(':')[2], proxy.Split(':')[3]);
                            restClient.Proxy = webProxy;
                            break;
                        }
                }
            }
            //if(!string.IsNullOrEmpty(userAgent))
            //    restClient.UserAgent = userAgent;
            var request = new RestRequest("https://b-api.facebook.com/method/auth.login", Method.POST);

            if (headers != null)
                foreach (var header in headers)
                    request.AddHeader(header.Key, header.Value);

            if (dataPost != null)
                request.AddParameter("application/x-www-form-urlencoded", dataPost, ParameterType.RequestBody);

            //var response = restClient.ExecuteAsync(request);
            //int i = 0;
            //do
            //{
            //    Task.Delay(1000);
            //    i++;
            //} while (string.IsNullOrEmpty(response.Result.Content) && i <= 20);

            //string resLog = response.Result.Content;

            var response = restClient.Execute(request);
            string resLog = response.Content;


            //resLog = _request.Send("https://b-api.facebook.com/method/auth.login", Method.Post, headers, dataPost);

            if (string.IsNullOrEmpty(resLog))
                return "ERROR";

            string firstFactor = Regex.Match(resLog.Replace("\\", ""), @"login_first_factor"":""(.*?)""").Groups[1].Value;

            string otp = "";

            if (!string.IsNullOrEmpty(authenCode)) {
                var secretKey = Base32Encoding.ToBytes(authenCode);
                var totp = new Totp(secretKey);
                otp = totp.ComputeTotp();
            }
                

            if (!resLog.Contains("two_factor"))
                return resLog;

            Dictionary<string, string> formData2 = new Dictionary<string, string>
                {
                { "adid", adID},
                {"format","json"},
                {"device_id",deviceID},
                {"email",uid},
                { "password",pass},
                { "generate_analytics_claim","1"},
                {"community_id", ""},
                {"cpl","true"},
                {"try_num", "1"},
                {"family_device_id", deviceID},
                {"sim_serials",sim_serials},
                { "credentials_type", "two_factor" },
                {"generate_session_cookies","1"},
                {"error_detail_type","button_with_disabled"},
                {"source","login"},
                {"machine_id",machineId},
                {"jazoest", rand2.Next(10000,99999).ToString()},
                {"meta_inf_fbmeta", ""},
                {"twofactor_code",otp },
                {"userid",uid},
                {"first_factor",firstFactor},
                {"advertiser_id",adID},
                {"encrypted_msisdn",""},
                {"currently_logged_in_userid", "0"},
                {"locale","en_US"},
                {"client_country_code","VN"},
                { "method", "auth.login" },
                { "fb_api_req_friendly_name","authenticate" },
                {"fb_api_caller_class","com.facebook.account.login.protocol.Fb4aAuthHandler"},
                { "api_key","882a8490361da98702bf97a021ddc14d"}
                };
            string sig2 = GetSig(Sort(formData2));
            formData2.Add("sig", sig2);

            Dictionary<string, string> headersTwo = new Dictionary<string, string>()
            {
            {"X-Fb-Connection-Quality","EXCELLENT"},
            {"X-Fb-Friendly-Name","authenticate"},
            {"X-Tigon-Is-Retry","False"},
            {"X-Fb-Connection-Type","unknown"},
            {"x-fb-sim-hni",sim},
            {"x-fb-net-hni",sim},
            {"X-FB-Socket-Option","TCP_CONGESTION=bbr"},
            {"content-type","application/x-www-form-urlencoded"}
            };

            string dataPostTwo = Stringify(formData2);

            request = new RestRequest("https://b-api.facebook.com/method/auth.login", Method.POST);

            if (headersTwo != null)
                foreach (var header in headersTwo)
                    request.AddHeader(header.Key, header.Value);

            if (dataPostTwo != null)
                request.AddParameter("application/x-www-form-urlencoded", dataPostTwo, ParameterType.RequestBody);

            var resLogTwo = restClient.Execute(request);

            //string resLogTwo = request.Send("https://b-api.facebook.com/method/auth.login", Method.Post, headersTwo, dataPostTwo);

            if (string.IsNullOrEmpty(resLogTwo.Content))
                return "ERROR";

            return resLogTwo.Content;
        }

    }
}
