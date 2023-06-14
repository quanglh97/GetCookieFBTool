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
        public static string linkFix = null;
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

        public static bool waitChangeIP = false;
        public static bool checkLocate = false;

        public static int maxThread = 100;
        public static int urlType = 1;
        public static string urlCustom = "";

        public static ConcurrentQueue<string> cookiePrepare = new ConcurrentQueue<string>();
        public static ConcurrentQueue<string> queueFakeUserAgent = new ConcurrentQueue<string>();
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
                    obj.userAgent + "|" +
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                if (status == "success")
                    dataSuccess.Enqueue(sLine);
                //File.AppendAllText(FS.sFileExportSuccess, sLine + Environment.NewLine);
                else if (status == "checkpoint")
                {
                    dataCheckPoint.Enqueue(sLine);
                    //File.AppendAllText(FS.sFileExportCheckPoint, sLine + Environment.NewLine);
                }
                else if (status == "lostCookies")
                {
                    dataLostCookies.Enqueue(sLine);
                    //File.AppendAllText(FS.sFileLostCookies, sLine + Environment.NewLine);
                }
                else if (status.Contains("proxy") || status.Contains("name =\"login\"")|| status.Contains("timeout"))
                {
                    dataReCheck.Enqueue(sLine);
                    //File.AppendAllText(FS.sFileLostCookies, sLine + Environment.NewLine);
                }
                else if (status == "spam")
                {
                    dataSpam.Enqueue(sLine);
                    //File.AppendAllText(FS.sFileLostCookies, sLine + Environment.NewLine);
                }
                else if(status == "no-veri")
                {
                    dataNoveri.Enqueue(sLine + "|No-verify");
                }    
                else
                {
                    
                    dataError.Enqueue(sLine.Replace('\n', ' ') + "|" + status.Replace('\n', ' '));
                    //File.AppendAllText(FS.sFileExportError, sLine + Environment.NewLine);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
