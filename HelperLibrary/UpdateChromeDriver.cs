using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetCookieFBTool.HelperLibrary
{
    public class UpdateChromeDriver
    {
        public static bool isLoadServer = false;
        public static void CheckAnDownloadChromeDriver(object param)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/auto_config.bin"))
            {
                if (File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/auto_config.bin") != null && File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/auto_config.bin").Contains('0'))
                {
                    isLoadServer = true;
                    return;
                }
            }

            // get current version chrome
            string path;
            path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null).ToString();
            if (path != null)
            {
                System.Diagnostics.Debug.WriteLine("Chrome version: " + FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion);
            }
            else
            {
                path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null).ToString();

                if (path == null || path.Length <= 0)
                {
                    isLoadServer = false;
                    return;
                }
            }

            // check version at current folder and chrome
            string version = "";
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/chromedriver.exe"))
            {
                // Start the child process.
                Process p = new Process();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "/chromedriver.exe";
                p.StartInfo.Arguments = "-v";
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                version = output.Split(' ')[1].Split('.')[0];
            }

            if (version != null & version.Length > 0 && version == FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion.Split('.')[0])
            {
                isLoadServer = true;
            }
            else
            {
                // download to current folder
                DownloadLatestVersionOfChromeDriver();

                try
                {
                    // get version again
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/chromedriver.exe"))
                    {
                        // Start the child process.
                        Process p = new Process();
                        // Redirect the output stream of the child process.
                        p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "/chromedriver.exe";
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.Arguments = "-v";
                        p.StartInfo.CreateNoWindow = true;
                        p.Start();

                        string output = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();

                        version = output.Split(' ')[1].Split('.')[0];

                        if (version != null & version.Length > 0 && version == FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion.Split('.')[0])
                        {
                            isLoadServer = true;
                        }
                    }
                }
                catch (Exception ex)
                { }
            }

            if (!isLoadServer)
            {
                MessageBox.Show("File setup tự động lỗi, vui lòng liên hệ admin để admin setup", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
        }

        public static void DownloadLatestVersionOfChromeDriver()
        {
            try
            {
                string path = DownloadLatestVersionOfChromeDriverGetVersionPath();
                var version = DownloadLatestVersionOfChromeDriverGetChromeVersion(path);
                var urlToDownload = DownloadLatestVersionOfChromeDriverGetURLToDownload(version);
                DownloadLatestVersionOfChromeDriverKillAllChromeDriverProcesses();
                DownloadLatestVersionOfChromeDriverDownloadNewVersionOfChrome(urlToDownload);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Auto config lastest lỗi, vui lòng liên hệ admin để setup", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }

        }

        public static string DownloadLatestVersionOfChromeDriverGetVersionPath()
        {
            //Path originates from here: https://chromedriver.chromium.org/downloads/version-selection            
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe"))
            {
                if (key != null)
                {
                    Object o = key.GetValue("");
                    if (!String.IsNullOrEmpty(o.ToString()))
                    {
                        return o.ToString();
                    }
                    else
                    {
                        throw new ArgumentException("Unable to get version because chrome registry value was null");
                    }
                }
                else
                {
                    throw new ArgumentException("Unable to get version because chrome registry path was null");
                }
            }
        }

        public static string DownloadLatestVersionOfChromeDriverGetChromeVersion(string productVersionPath)
        {
            if (String.IsNullOrEmpty(productVersionPath))
            {
                throw new ArgumentException("Unable to get version because path is empty");
            }

            if (!File.Exists(productVersionPath))
            {
                throw new FileNotFoundException("Unable to get version because path specifies a file that does not exists");
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(productVersionPath);
            if (versionInfo != null && !String.IsNullOrEmpty(versionInfo.FileVersion))
            {
                return versionInfo.FileVersion;
            }
            else
            {
                throw new ArgumentException("Unable to get version from path because the version is either null or empty: " + productVersionPath);
            }
        }

        public static string DownloadLatestVersionOfChromeDriverGetURLToDownload(string version)
        {
            if (String.IsNullOrEmpty(version))
            {
                throw new ArgumentException("Unable to get url because version is empty");
            }

            //URL's originates from here: https://chromedriver.chromium.org/downloads/version-selection
            string html = string.Empty;
            string urlToPathLocation = @"https://chromedriver.storage.googleapis.com/LATEST_RELEASE_" + String.Join(".", version.Split('.').Take(3));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToPathLocation);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            if (String.IsNullOrEmpty(html))
            {
                throw new WebException("Unable to get version path from website");
            }

            return "https://chromedriver.storage.googleapis.com/" + html + "/chromedriver_win32.zip";
        }

        public static void DownloadLatestVersionOfChromeDriverKillAllChromeDriverProcesses()
        {
            //It's important to kill all processes before attempting to replace the chrome driver, because if you do not you may still have file locks left over
            var processes = Process.GetProcessesByName("chromedriver");
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                    //We do our best here but if another user account is running the chrome driver we may not be able to kill it unless we run from a elevated user account + various other reasons we don't care about
                }
            }
        }

        public static void DownloadLatestVersionOfChromeDriverDownloadNewVersionOfChrome(string urlToDownload)
        {
            if (String.IsNullOrEmpty(urlToDownload))
            {
                throw new ArgumentException("Unable to get url because urlToDownload is empty");
            }

            //Downloaded files always come as a zip, we need to do a bit of switching around to get everything in the right place
            using (var client = new WebClient())
            {
                if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip"))
                {
                    File.Delete(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip");
                }

                client.DownloadFile(urlToDownload, "chromedriver.zip");

                if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip") && File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.exe"))
                {
                    File.Delete(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.exe");
                }

                if (File.Exists(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip"))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\chromedriver.zip", System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

                }
            }
        }
    }
}
