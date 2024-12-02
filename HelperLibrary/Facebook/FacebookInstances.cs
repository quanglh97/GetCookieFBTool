using GetCookieFBTool.HelperLibrary.HelperProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetCookieFBTool.HelperLibrary.Facebook
{
    internal class FacebookInstances
    {
        public static List<string> lsLinkFix = new List<string>();
        public static List<string> lsUA = new List<string>();
        public static int numRan = 0;
        public static int pageLoadTime = 10;
        public static string typeAcc = "";

        public static bool CookieSP = false;
        public static bool AccSP = false;
        public static bool getToken = false;
        public static bool checkIP = false;
        public static bool fakeUserAgent = false;
        public static bool addEx = false;
        public static bool requerstMBasic = false;
        public static bool randomWWW = false;
        public static bool requerstWWW = false;
        public static bool requerstWWWLogin = false;
        public static bool requerstLogin2 = false;
        public static bool relogin = false;
        public static int numRelogin = 1;
        public static bool NotifyNoProxy = false;

        public static bool waitChangeIP = false;
        public static bool checkLocate = false;
        public static bool timeUa = false;
        public static bool timeRealUa = false;
        public static bool noTimeUa = false;

        public static int maxThread = 100;
        public static int urlType = 1;
        public static string urlCustom = "";

        public static bool waitClearChrome = false;

        public static ConcurrentQueue<string> cookiePrepare = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> queueFakeUserAgent = new ConcurrentQueue<string>();
        public static List<string> lsFakeUserAgent = new List<string>();
        public static ConcurrentQueue<string> queueAccPre = new ConcurrentQueue<string>();
        public static List<string> lsUrlCustom = new List<string>();

        public static MultiThreading<Account> MultiThrs = new MultiThreading<Account>();

        //public static string sFileExportSuccess = AppDomain.CurrentDomain.BaseDirectory + "Result\\" + "\\SAVE_OUT_PUT_SUCCESS.txt";
        //public static string sFileExportError = AppDomain.CurrentDomain.BaseDirectory + "Result\\" + "\\SAVE_OUT_PUT_ERROR.txt";
        public static FileSaver FS = new FileSaver();

        public static event EventHandler<dgvEventArgs> NewAccountAdded;

        public static int ThrsCount = 1;

        public static int delayLevel = 1;

        public static int modeRun = 0;

        public static ConcurrentQueue<string> dataSuccess = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> dataError = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> dataErrorNoReason = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> dataCheckPoint = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> dataLostCookies = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> dataReCheck = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> dataSpam = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> dataNoveri = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> dataCookiePre = new ConcurrentQueue<string>();

        public static void OnNewAccAdded(Account newacc, string status, string cookies, string token, string userAgent)
        {
            try
            {
                EventHandler<dgvEventArgs> newAccountAdded = FacebookInstances.NewAccountAdded;
                if (newAccountAdded == null)
                    return;
                dgvEventArgs obj = new dgvEventArgs(newacc, status, cookies, token, userAgent);

                newAccountAdded((object)null, new dgvEventArgs(newacc, status, cookies, token, userAgent));
                // save to file
                string sLine =  newacc.userName + "|" + newacc.pass + "|" + obj.cookies + "|" + obj.token + "|" + newacc.key2fa + "|" +
                    newacc.locate + "|" + newacc.locateNew + "|"+
                    obj.userAgent + "|" + status.Replace("\r","") + "|" +
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                if (status.Contains("success"))
                    dataSuccess.Enqueue(sLine);
                //File.AppendAllText(FS.sFileExportSuccess, sLine + Environment.NewLine);
                else if (status.Contains("checkpoint"))
                {
                    dataCheckPoint.Enqueue(sLine);
                    //File.AppendAllText(FS.sFileExportCheckPoint, sLine + Environment.NewLine);
                }
                else if (status.Contains("lostCookies"))
                {
                    dataLostCookies.Enqueue(sLine);
                    //File.AppendAllText(FS.sFileLostCookies, sLine + Environment.NewLine);
                }
                else if (status.Contains("proxy") || status.Contains("name =\"login\"")|| status.Contains("timeout"))
                {
                    dataReCheck.Enqueue(sLine);
                    //File.AppendAllText(FS.sFileLostCookies, sLine + Environment.NewLine);
                }
                else if (status.Contains("spam"))
                {
                    dataSpam.Enqueue(sLine);
                    //File.AppendAllText(FS.sFileLostCookies, sLine + Environment.NewLine);
                }
                else if(status.Contains("no-veri"))
                {
                    dataNoveri.Enqueue(sLine + "|No-verify");
                }    
                else if (status.Contains("error"))
                {
                    dataErrorNoReason.Enqueue(sLine.Replace('\n', ' ') + "|" + status.Replace('\n', ' '));
                }
                else{
                    
                    dataError.Enqueue(sLine.Replace('\n', ' ') + "|" + status.Replace('\n', ' '));
                    //File.AppendAllText(FS.sFileExportError, sLine + Environment.NewLine);
                }
            }
            catch
            {
                return;
            }
        }

        public static void GenUA()
        {
            var lsUA = new List<string>();
            Random rnd = new Random();
            List<string> lsModelUA = new List<string> { "zh-Hans-CN; SDY-AN00", "zh-CN; PFUM10", "zh-CN; V2244A", "zh-Hans-CN; JLH-AN00", "zh-Hans-CN; BRQ-AL00" };
            List<string> lsbuildUA = new List<string> { "HONORSDY-AN00", "SKQ1.211209.001", "TP1A.220624.014", "HONORJLH-AN00", "HUAWEIBRQ-AL00" };
            for (int i = 0; i < 1000; i++)
            {
                int r = rnd.Next(lsModelUA.Count);
                var model_ua = lsModelUA[r];
                r = rnd.Next(lsbuildUA.Count);
                var build_ua = lsbuildUA[r];
                r = rnd.Next(1, 31);
                var ua = $"Mozilla/5.0 (Linux; U; Android {r}; {model_ua} Build/{build_ua}; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/66.0.3359.126 MQQBrowser/6.2 TBS/044704 Mobile Safari/537.36";
                FacebookInstances.lsUA.Add(ua);
            }
        }
    }
}
