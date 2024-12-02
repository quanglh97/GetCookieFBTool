using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetCookieFBTool.HelperLibrary.HelperProxy
{
    public class FileSaver
    {
        private readonly string mainFolderPath = "";// AppDomain.CurrentDomain.BaseDirectory + "\\Result [" + DateTime.Now.ToString("dd MMMM", (IFormatProvider)CultureInfo.CreateSpecificCulture("en-US")) + "]";
        public string subFolderPath = string.Empty;
        public string sFileExportSuccess = string.Empty;
        public string sFileCookiePre = string.Empty;
        public string sFileExportCheckPoint = string.Empty;
        public string sFileExportLostCookies = string.Empty;
        public string sFileExportRecheck = string.Empty;
        public string sFileExportError = string.Empty;
        public string sFileExportErrorNoReason = string.Empty;
        public string LoggerFolderPath = string.Empty;
        public object LockReplace = new object();
        public object LockMsg = new object();
        public static event EventHandler<dgvOutput> evtPutToView;

        public FileSaver(string baseFileName)
        {
            string sTimeMark = DateTime.Now.ToString("HH.mm.ss", (IFormatProvider)CultureInfo.CreateSpecificCulture("vi-VN"));
            sTimeMark += "_" + DateTime.Now.ToString("dd.MM.yyyy", (IFormatProvider)CultureInfo.CreateSpecificCulture("en-US"));

         
            if (baseFileName.Contains(':'))
            {
                baseFileName = baseFileName.Split(':')[0];
            }
            this.subFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Result\\";
            this.mainFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Result\\" + baseFileName;

            sFileExportSuccess += this.subFolderPath + "\\SAVE_OUT_PUT_SUCCESS";
            sFileExportError += this.subFolderPath + "\\SAVE_OUT_PUT_ERROR.txt";
            sFileExportErrorNoReason += this.subFolderPath + "\\SAVE_OUT_PUT_ERROR_NO_REASON.txt";
            sFileExportCheckPoint += this.subFolderPath + "\\SAVE_OUT_PUT_CHECKPOINT.txt";
            sFileExportLostCookies += this.subFolderPath + "\\SAVE_OUT_PUT_LOSTCOOKIES.txt";

            Directory.CreateDirectory(this.mainFolderPath);

        }
        public FileSaver()
        {

            this.subFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Result\\";

            sFileExportSuccess += this.subFolderPath + $"\\SAVE_OUT_PUT_SUCCESS";
            sFileExportError += this.subFolderPath + "\\SAVE_OUT_PUT_ERROR.txt";
            sFileExportErrorNoReason += this.subFolderPath + "\\SAVE_OUT_PUT_ERROR_NO_REASON.txt";
            sFileExportCheckPoint += this.subFolderPath + "\\SAVE_OUT_PUT_CHECKPOINT.txt";
            sFileExportLostCookies += this.subFolderPath + "\\SAVE_OUT_PUT_LOSTCOOKIES.txt";
            sFileExportRecheck += AppDomain.CurrentDomain.BaseDirectory + "\\data" + "\\SAVE_OUT_PUT_RECHECK.txt";
            sFileCookiePre += this.subFolderPath + "\\SAVE_OUT_COOKIE_PRE.txt";
            Directory.CreateDirectory(this.subFolderPath);

        }

        public void SaveAccount(string text, string fileName)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(this.subFolderPath + "\\" + fileName + ".txt", true))
                    streamWriter.WriteLine(text);
            }
            catch (IOException ex)
            {
                Thread.Sleep(100);
                this.SaveAccount(text, fileName);
            }
        }

        public void Base64ToImage(string base64String, string fileName)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64String);
                using (var imageFile = new FileStream(fileName, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }
            }
            catch
            {
                return;
            }
        }

        public void CreateOstatok(HashSet<string> tmpArray)
        {
            try
            {
                if (File.Exists(this.subFolderPath + "\\Remains.txt"))
                    File.Move(this.subFolderPath + "\\Remains.txt", this.subFolderPath + "\\Remains_backup.txt");
                File.WriteAllLines(this.subFolderPath + "\\Remains.txt", tmpArray.ToArray<string>());
                File.Delete(this.subFolderPath + "\\Remains_backup.txt");
            }
            catch (IOException ex)
            {
                Thread.Sleep(100);
                this.CreateOstatok(tmpArray);
            }
            catch (InvalidOperationException ex)
            {
                Thread.Sleep(100);
                this.CreateOstatok(tmpArray);
            }
        }

        public void DeleteEmptyFolder()
        {
            if (Directory.Exists(this.subFolderPath) && Directory.GetFiles(this.subFolderPath, "*.txt", SearchOption.AllDirectories).Length == 0)
                Directory.Delete(this.subFolderPath, true);
            if (!Directory.Exists(this.mainFolderPath) || Directory.GetFiles(this.mainFolderPath, "*.txt", SearchOption.AllDirectories).Length != 0)
                return;
            Directory.Delete(this.mainFolderPath, true);
        }

        public static void PutToView(int nID, string sOutput)
        {
            EventHandler<dgvOutput> newToView = FileSaver.evtPutToView;
            if (newToView == null)
                return;

            newToView((object)null, new dgvOutput(nID, sOutput));
        }
    }
}
