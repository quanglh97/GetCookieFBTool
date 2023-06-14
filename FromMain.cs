using GetCookieFBTool.HelperLibrary;
using GetCookieFBTool.HelperLibrary.Facebook;
using GetCookieFBTool.HelperLibrary.HelperProxy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetCookieFBTool
{
    public partial class FromMain : Form
    {

        BackgroundWorker m_oWorker = null;
        BackgroundWorker m_oWorkerImportProxy = null;
        BackgroundWorker m_oWorkerImportAcc = null;
        BackgroundWorker statusWorker = null;
        public static bool stopCmd = false;


        List<dgvEventArgs> lsDataResult = new List<dgvEventArgs>();
        public static int nIndex = 0;
        public static int done = 0;
        public static int error = 0;
        public static int lostCookies = 0;
        public static int checkpoint = 0;
        public static int spam = 0;



        public static int doneInHour = 0;
        public static int errorInHour = 0;
        public static int lostCookiesInHour = 0;
        public static int checkpointInHour = 0;
        public static int spamInHour = 0;

        public static bool isNotifyKey = false;
        public static bool isAddLs = false;
        public static int timeWait2ChangeIP = 5000 * 60;


        //public static long beginTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        //public static long currentTimeStamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        public static List<object> argumentsTMProxy = new List<object>();
        public static List<object> argumentsTinsoftProxy = new List<object>();
        public static List<object> argumentsTinProxy = new List<object>();
        public static List<object> argumentsMinProxy = new List<object>();
        public static List<object> argumentsProxyMMONet = new List<object>();
        public static List<object> argumentsProxyMobie = new List<object>();
        public static List<object> argumentsPOCProxy = new List<object>();

        public static List<object> argumentsZProxy = new List<object>();
        public static List<object> argumentsZProxy2 = new List<object>();
        public static List<object> argumentsNo1Proxy = new List<object>();
        public static List<object> argumentsShopLikeProxy = new List<object>();
        public static List<object> argumentsPOCWanProxy = new List<object>();

        public static List<object> argumentsProxyRotatingV4 = new List<object>();
        public static List<object> argumentsProxyRotatingV6 = new List<object>();

        public static FileSaver FS = new FileSaver();
        int runTime = 0;
        public FromMain()
        {
            InitializeComponent();

            this.FormClosing += FormMain_FormClosing;

            m_oWorker = new BackgroundWorker();
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;

            m_oWorkerImportProxy = new BackgroundWorker();
            m_oWorkerImportProxy.DoWork += new DoWorkEventHandler(m_oWorker_Proxy);
            m_oWorkerImportProxy.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged_Proxy);
            m_oWorkerImportProxy.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted_Proxy);
            m_oWorkerImportProxy.WorkerReportsProgress = true;
            m_oWorkerImportProxy.WorkerSupportsCancellation = true;

            m_oWorkerImportAcc = new BackgroundWorker();
            m_oWorkerImportAcc.DoWork += new DoWorkEventHandler(m_oWorker_Main_Acc);
            m_oWorkerImportAcc.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged_Acc);
            m_oWorkerImportAcc.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted_Acc);
            m_oWorkerImportAcc.WorkerReportsProgress = true;
            m_oWorkerImportAcc.WorkerSupportsCancellation = true;

            statusWorker = new BackgroundWorker();
            statusWorker.DoWork += new DoWorkEventHandler(statusWorkerDowork);
            statusWorker.ProgressChanged += new ProgressChangedEventHandler(statusWorkerProgressChanged);
            statusWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(statusWorkerRunWorkerCompleted);
            statusWorker.WorkerReportsProgress = true;
            statusWorker.WorkerSupportsCancellation = true;

            FacebookInstances.NewAccountAdded += this.Statistic_newAccountAdded;

            new Thread(new ParameterizedThreadStart(UpdateChromeDriver.CheckAnDownloadChromeDriver))
            {
                IsBackground = true,
            }.Start();

            try
            {
                listAccCbBox.SelectedIndex = 0;
                defaultSetting();
                //IPSTbox.Text = AddIP();
            }
            catch
            {

            }
            SetTabHeader(tabControl1.TabPages[0], Color.FromArgb(255, 66, 66, 66));
            for (int i = 1; i < tabControl1.TabPages.Count; i++)
            {
                SetTabHeader(tabControl1.TabPages[i], Color.FromArgb(255, 171, 171, 171));
            }
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);



            List<LocationIp> listItem = new List<LocationIp>()
            {
                 new LocationIp(){locationTinsoft= 0,  locationTM= 1,  locationTin = 0,  locationMin= -1, Name = "Random"},
                 new LocationIp(){locationTinsoft= 1,  locationTM= 10, locationTin = 1,  locationMin= -1, Name = "Ha Noi"},
                 new LocationIp(){locationTinsoft= 2,  locationTM= -1, locationTin = -1, locationMin= -1, Name = "Phu Tho"},
                 new LocationIp(){locationTinsoft= 3,  locationTM= 9,  locationTin = 3,  locationMin= -1, Name = "Ho Chi Minh"},
                 new LocationIp(){locationTinsoft= 4,  locationTM= -1, locationTin = -1, locationMin= -1, Name = "Dak Lak"},
                 new LocationIp(){locationTinsoft= 5,  locationTM= -1, locationTin = -1, locationMin= -1, Name = "Hai Duong"},
                 new LocationIp(){locationTinsoft= 6,  locationTM= -1, locationTin = -1, locationMin= -1, Name = "Binh Dinh"},
                 new LocationIp(){locationTinsoft= 7,  locationTM= -1, locationTin = -1, locationMin= -1, Name = "Nghe An"},
                 new LocationIp(){locationTinsoft= 8,  locationTM= -1, locationTin = -1, locationMin= -1, Name = "Nam Dinh"},
                 new LocationIp(){locationTinsoft= 10, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Thai Binh"},
                 new LocationIp(){locationTinsoft= 11, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Ha Tinh"},
                 new LocationIp(){locationTinsoft= 14, locationTM= 2,  locationTin = -1, locationMin= -1, Name = "Bac Ninh"},
                 new LocationIp(){locationTinsoft= 12, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Gia Lai"},
                 new LocationIp(){locationTinsoft= 13, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Yen Bai"},
                 new LocationIp(){locationTinsoft= 15, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Ha Nam"},
                 new LocationIp(){locationTinsoft= 16, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Tra Vinh"},
                 new LocationIp(){locationTinsoft= 17, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Son La"},
                 new LocationIp(){locationTinsoft= 19, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Dong Nai"},
                 new LocationIp(){locationTinsoft= 20, locationTM= 8,  locationTin = -1, locationMin= -1, Name = "Ba Ria Vung Tau"},
                 new LocationIp(){locationTinsoft= 21, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Lao Cai"},
                 new LocationIp(){locationTinsoft= 22, locationTM= -1, locationTin = -1, locationMin= -1, Name = "Tuyen Quang"},
                 new LocationIp(){locationTinsoft= -1, locationTM= 5,  locationTin = -1, locationMin= -1, Name = "Da Nang"},
                 new LocationIp(){locationTinsoft= -1, locationTM= 7,  locationTin = -1, locationMin= -1, Name = "Can Tho"},
                 new LocationIp(){locationTinsoft= -1, locationTM= 11, locationTin = -1, locationMin= -1, Name = "Khanh Hoa"},

            };
            LocaltionIpCbBox.DataSource = listItem;
            LocaltionIpCbBox.DisplayMember = "Name";

            tabControl1.SelectedIndex = 1;
        }

        private void FormMain_FormClosing(Object sender, FormClosingEventArgs e)
        {
            var lineBackup = FacebookInstances.numRan.ToString();
            string listAccIndex = listAccCbBox.SelectedItem.ToString();
            var backupPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"backup_{listAccIndex}.txt";
            File.WriteAllText(backupPath, lineBackup);
            //In case windows is trying to shut down, don't hold the process up
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            else if (e.CloseReason == CloseReason.UserClosing)
            {
                // Assume that X has been clicked and act accordingly.
                // Confirm user wants to close
                switch (MessageBox.Show(this, "BẠN CÓ CHẮC MUỐN TẮT KHÔNG?", "BẠN VẪN MUỐN TĂT À?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    //Stay on this form
                    case DialogResult.No:
                        e.Cancel = true;
                        break;
                    default:
                        if (!FacebookInstances.MultiThrs.IsStopping)
                        {
                            FacebookInstances.MultiThrs.Cancel();
                            ProcessHelper.KillAllProcessTree("chromedriver");
                        }
                        break;
                }
            }
        }
        private Dictionary<TabPage, Color> TabColors = new Dictionary<TabPage, Color>();
        private void SetTabHeader(TabPage page, Color color)
        {
            TabColors[page] = color;
            tabControl1.Invalidate();
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var g = e.Graphics;
            var text = this.tabControl1.TabPages[e.Index].Text;
            var sizeText = g.MeasureString(text, this.tabControl1.Font);

            var x = e.Bounds.Left + 3;
            var y = e.Bounds.Top + (e.Bounds.Height - sizeText.Height) / 2;

            g.DrawString(text, this.tabControl1.Font, Brushes.White, x, y);
            e.DrawBackground();
            using (Brush br = new SolidBrush(TabColors[tabControl1.TabPages[e.Index]]))
            {
                e.Graphics.FillRectangle(br, e.Bounds);
                SizeF sz = e.Graphics.MeasureString(tabControl1.TabPages[e.Index].Text, e.Font);
                e.Graphics.DrawString(tabControl1.TabPages[e.Index].Text, e.Font, Brushes.White, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

                Rectangle rect = e.Bounds;
                rect.Offset(0, 1);
                rect.Inflate(0, -1);
                e.Graphics.DrawRectangle(Pens.DarkGray, rect);
                e.DrawFocusRectangle();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTabHeader(tabControl1.TabPages[tabControl1.SelectedIndex], Color.FromArgb(255, 66, 66, 66));
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                if (i != tabControl1.SelectedIndex)
                    SetTabHeader(tabControl1.TabPages[i], Color.FromArgb(255, 171, 171, 171));
            }
        }

        public void defaultSetting()
        {
            dtListResultOTP.RowHeadersVisible = false;
            var tinsoftKeyFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "TinsoftProxyKey.txt";
            var TMKeyFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "TMProxyKey.txt";
            var rotatingV4FilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "RotatingV4.txt";
            var rotatingV6FilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "RotatingV6.txt";
            var tinKeyFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "TinProxyKey.txt";
            var minKeyFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "MinProxyKey.txt";
            var KeyMMONetFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "ProxyMMONetKey.txt";
            var KeyMobieProxyFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "MobieProxy.txt";
            var POCProxyFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "POCProxy.txt";
            var POCWanProxyFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "POCWanProxy.txt";

            var ZProxyFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "ZProxy.txt";
            var ZProxy2FilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "ZProxy2.txt";
            var ProxyNo1FilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "ProxyNo1.txt";
            var ProxyShopLikeFilePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "ProxyShopLike.txt";


            var settingPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "Setting.txt";
            var CookiePreparePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "CookiesPrepare.txt";
            var AccPreparePath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "PreAcc.txt";
            var fakeUserAgentPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "fakeUserAgent.txt";
            var urlCustomPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "urlCustomPath.txt";

            if (File.Exists(fakeUserAgentPath))
            {
                string[] dataUserAgent = File.ReadAllText(fakeUserAgentPath).Trim().Split('\n');
                foreach (var userAgent in dataUserAgent)
                {
                    FacebookInstances.queueFakeUserAgent.Enqueue(userAgent);
                }

            }

            if (File.Exists(AccPreparePath))
            {
                string[] dataAccPre = File.ReadAllText(AccPreparePath).Trim().Split('\n');
                foreach (var acc in dataAccPre)
                {
                    FacebookInstances.queueAccPre.Enqueue(acc);
                }
                Random random = new Random();
                int a1 = random.Next(1, 5000);
                while(a1 > 0)
                {
                    string acc = "";
                    FacebookInstances.queueAccPre.TryDequeue(out acc);
                    if(acc != null && acc != "")
                        FacebookInstances.queueAccPre.Enqueue(acc);
                    a1--;
                }


            }

            if (File.Exists(urlCustomPath))
            {
                string[] dataUrlCustom = File.ReadAllText(urlCustomPath).Trim().Split('\n');
                foreach (var url in dataUrlCustom)
                {
                    FacebookInstances.lsUrlCustom.Add(url);
                }

            }

            if (File.Exists(tinsoftKeyFilePath))
            {
                argumentsTinsoftProxy.Add(tinsoftKeyFilePath);
            }
            if (File.Exists(TMKeyFilePath))
            {
                argumentsTMProxy.Add(TMKeyFilePath);
            }
            if (File.Exists(tinKeyFilePath))
            {
                argumentsTinProxy.Add(tinKeyFilePath);
            }
            if (File.Exists(minKeyFilePath))
            {
                argumentsMinProxy.Add(minKeyFilePath);
            }


            if (File.Exists(rotatingV4FilePath))
            {
                argumentsProxyRotatingV4.Add(rotatingV4FilePath);
            }
            if (File.Exists(rotatingV6FilePath))
            {
                argumentsProxyRotatingV6.Add(rotatingV6FilePath);
            }
            if (File.Exists(KeyMMONetFilePath))
            {
                argumentsProxyMMONet.Add(KeyMMONetFilePath);
            }
            if (File.Exists(KeyMobieProxyFilePath))
            {
                argumentsProxyMobie.Add(KeyMobieProxyFilePath);
            }
            if (File.Exists(POCProxyFilePath))
            {
                argumentsPOCProxy.Add(POCProxyFilePath);
            }

            if (File.Exists(POCWanProxyFilePath))
            {
                argumentsPOCWanProxy.Add(POCWanProxyFilePath);
            }

            if (File.Exists(ZProxyFilePath))
            {
                argumentsZProxy.Add(ZProxyFilePath);
            }
            if (File.Exists(ProxyNo1FilePath))
            {
                argumentsNo1Proxy.Add(ProxyNo1FilePath);
            }
            if (File.Exists(ProxyShopLikeFilePath))
            {
                argumentsShopLikeProxy.Add(ProxyShopLikeFilePath);
            }
            if (File.Exists(ZProxy2FilePath))
            {
                argumentsZProxy2.Add(ZProxy2FilePath);
            }

            if (File.Exists(CookiePreparePath))
            {
                HashSet<string> hashSet = new HashSet<string>();
                hashSet.UnionWith(FileLoader.LoadStringsFromFile(CookiePreparePath));
                FacebookInstances.cookiePrepare = new ConcurrentQueue<string>(hashSet);
                Random random = new Random();
                int a1 = random.Next(1, 5000);
                while (a1 > 0)
                {
                    string acc = "";
                    FacebookInstances.cookiePrepare.TryDequeue(out acc);
                    if (acc != null && acc != "")
                        FacebookInstances.cookiePrepare.Enqueue(acc);
                    a1--;
                }
            }

            if (File.Exists(settingPath))
            {
                var setting = File.ReadAllText(settingPath).Split('|');
                this.threadCountNumUD.Value = Int32.Parse(setting[0]);
                this.delayLevelUD.Value = Int32.Parse(setting[1]);
                this.typeAccTB.Text = setting[2];
                FacebookInstances.typeAcc = setting[2];


                switch (setting[3])
                {
                    case "0":
                        this.hideModeRB.Checked = true;
                        break;
                    case "1":
                        this.showAllModeRB.Checked = true;
                        break;
                    case "2":
                        this.showTaskbarModeRB.Checked = true;
                        break;
                }
                if (setting[4] == "0")
                {
                    this.CookieSpCB.Checked = false;
                }
                else
                {
                    this.CookieSpCB.Checked = true;
                }

                switch (setting[5])
                {
                    case "0":
                        this.TinsoftProxyMode.Checked = true;
                        break;

                    case "1":
                        this.TmProxyMode.Checked = true;
                        break;
                    case "2":
                        this.ProxyRoratingV4Mode.Checked = true;
                        break;
                    case "3":
                        this.ProxyRoratingV6Mode.Checked = true;
                        break;
                    case "4":
                        this.tinProxyMode.Checked = true;
                        break;
                    case "5":
                        this.minProxyMode.Checked = true;
                        break;
                    case "6":
                        this.NoneProxyMode.Checked = true;
                        break;
                    case "7":
                        this.ProxyMMONetMode.Checked = true;
                        break;
                    case "8":
                        this.ProxyMobieMode.Checked = true;
                        break;
                    case "9":
                        this.POCProxyMode.Checked = true;
                        break;
                    case "10":
                        this.ZProxyMode.Checked = true;
                        break;
                    case "11":
                        this.ProxyNo1Mode.Checked = true;
                        break;
                    case "12":
                        this.ProxyShopLikeMode.Checked = true;
                        break;
                    case "13":
                        this.POCWanProxyMode.Checked = true;
                        break;
                    case "14":
                        this.ZProxy2Mode.Checked = true;
                        break;
                }
                this.TimeUpdateProxyUD.Value = Int32.Parse(setting[6]);

                TimeRefreshUD.Value = Int32.Parse(setting[7]);

                if (setting[8] == "1")
                {
                    fakeUserAgentCB.Checked = true;
                }
                else
                {
                    fakeUserAgentCB.Checked = false;
                }

                linkFixCBox.Text = setting[9];

                string listAccIndex = setting[10];

                IPSTbox.Text = setting[11];

                pageLoadUD.Value = Int32.Parse(setting[12]);

                if (setting[13] == "1")
                {
                    checkIPCB.Checked = true;
                }
                else
                {
                    checkIPCB.Checked = false;
                }


                listAccCbBox.SelectedIndex = Int32.Parse(listAccIndex) - 1;
                var backupPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"backup_{listAccIndex}.txt";
                var accountPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"Account_{listAccIndex}.txt";

                if (File.Exists(accountPath))
                {
                    ProxyStateLb.Text = "Importing Accounts";
                    accTBox.Text = Path.GetFileName(accountPath);
                    if (!m_oWorkerImportAcc.IsBusy)
                        m_oWorkerImportAcc.RunWorkerAsync(accountPath);
                }

                if (File.Exists(backupPath))
                {
                    var backup = File.ReadAllText(backupPath);
                    FacebookInstances.numRan = Int32.Parse(backup);
                    this.numBackupTB.Text = FacebookInstances.numRan.ToString();
                }



                if (setting[14] == "1")
                {
                    addExtensionCBox.Checked = true;
                }
                else
                {
                    addExtensionCBox.Checked = false;
                }
                if (setting[15] == "0")
                {
                    urlRandom.Checked = true;
                }
                else if (setting[15] == "1")
                {
                    urlNomal.Checked = true;
                }
                else if (setting[15] == "2")
                {
                    urlFBCom.Checked = true;
                }
                else if (setting[15] == "3")
                {
                    urlMbasic.Checked = true;
                }
                else if (setting[15] == "4")
                {
                    urlMFB.Checked = true;
                }

                if (setting[16] == "0")
                {
                    this.accPreCBox.Checked = false;
                }
                else
                {
                    this.accPreCBox.Checked = true;
                }

                urlCustom.Text = "";

                if (setting[17] == "0")
                {
                    this.checkLocateCBox.Checked = false;
                }
                else
                {
                    this.checkLocateCBox.Checked = true;
                }
            }
        }


        private void AccBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Text|*.txt|All|*.*",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(ofd.FileName))
                    return;

                accTBox.Text = Path.GetFileName(ofd.FileName);
                this.ProxyStateLb.Text = "Importing Accounts";

                if (!m_oWorkerImportAcc.IsBusy)
                {
                    m_oWorkerImportAcc.RunWorkerAsync(ofd.FileName);
                }
            }
        }


        private delegate void InvokeDelegate(dgvEventArgs e);
        private void Statistic_newAccountAdded(object sender, dgvEventArgs e)
        {
            this.BeginInvoke(new InvokeDelegate(AddToGirdView), new object[] { e });
        }

        private void AddToGirdView(dgvEventArgs e)
        {
            if (e == null || e.InfoAccount == null)
            {
                return;
            }

            if (lsDataResult == null)
            {
                lsDataResult = new List<dgvEventArgs>();
            }

            lsDataResult.Add(e);

            if (dtListResultOTP.Rows.Count > 1000)
            {
                nIndex = 0;
                if (dtListResultOTP != null)
                {
                    dtListResultOTP.DataSource = null;
                    dtListResultOTP.Rows.Clear();
                }
            }

            dtListResultOTP.Rows.Insert(nIndex, nIndex + 1, e.status, e.InfoAccount.userName, e.InfoAccount.pass, e.cookies, e.token, e.userAgent, e.loacate, e.newLocate);
            dtListResultOTP.Rows[nIndex].DefaultCellStyle.ForeColor = Color.Red;
            nIndex++;
            if (e.status == "success" || e.status == "no-veri")
            {
                done += 1;
                this.doneTbox.Text = done.ToString();
                dtListResultOTP.Rows[nIndex].DefaultCellStyle.ForeColor = Color.Green;
                doneInHour++;


            }

            else if (e.status == "lostCookies")
            {
                lostCookies += 1;
                this.lostCookieTbox.Text = lostCookies.ToString();
                dtListResultOTP.Rows[nIndex].DefaultCellStyle.ForeColor = Color.Red;


                lostCookiesInHour++;


            }
            else if (e.status == "checkpoint")
            {
                checkpoint += 1;
                this.checkpointTBox.Text = checkpoint.ToString();
                dtListResultOTP.Rows[nIndex].DefaultCellStyle.ForeColor = Color.Yellow;

                checkpointInHour++;



            }
            else if (e.status == "spam")
            {
                spam += 1;
                this.spamTBox.Text = spam.ToString();
                dtListResultOTP.Rows[nIndex].DefaultCellStyle.ForeColor = Color.Yellow;

                spamInHour++;



            }
            else
            {
                error += 1;
                this.errorTbox.Text = error.ToString();
                dtListResultOTP.Rows[nIndex].DefaultCellStyle.ForeColor = Color.Red;

                errorInHour++;


            }
            //this.lbInfo.Visible = false;
            //this.btnFind.Visible = true;
        }

        // process Main Account
        void m_oWorker_Main_Acc(object sender, DoWorkEventArgs e)
        {
            ////NOTE : Never play with the UI thread here...
            //int indexItem = 0;

            if (e == null)
            {
                return;
            }

            string sFilePathAcc = (string)e.Argument;
            if (!File.Exists(sFilePathAcc))
            {
                return;
            }

            accTBox.Text = Path.GetFileName(sFilePathAcc);
            //HashSet<Account> hashSet = new HashSet<Account>();
            List<Account> hashSet = new List<Account>();
            try
            {
                //hashSet.UnionWith(FileLoader.LoadBaseFromFile(accountPath));
                string[] data = File.ReadAllText(sFilePathAcc).Trim().Split('\n');

                hashSet.Clear();
                for (int i = 0; i < data.Length; i++)
                {
                    Account acc = new Account(data[i]);
                    hashSet.Add(acc);
                }
                Statistic.AllAccounts = (long)hashSet.Count<Account>();
                FacebookInstances.MultiThrs.queueAccs = new ConcurrentQueue<Account>(hashSet);

                if (FacebookInstances.numRan < FacebookInstances.MultiThrs.Count && FacebookInstances.numRan > 0)
                {
                    for (int i = 0; i < FacebookInstances.numRan; i++)
                    {
                        Account freeAcc;
                        FacebookInstances.MultiThrs.queueAccs.TryDequeue(out freeAcc);
                    }
                }
                else
                {
                    FacebookInstances.numRan = 0;
                }
                //while(m_oWorkerImportAcc.IsBusy)
                //{
                //    m_oWorkerImportAcc.ReportProgress(0);
                //    Thread.Sleep(3000);
                //}

                //this.totalAccTBox.Text = FacebookInstances.MultiThrs.Count.ToString();
            }
            finally
            {
                hashSet = new List<Account>();
            }

            //HashSet<Account> hashSet = new HashSet<Account>();
            //try
            //{
            //    hashSet.UnionWith(FileLoader.LoadBaseFromFile(sFilePathAcc));
            //    FacebookInstances.MultiThrs.queueAccs = new ConcurrentQueue<Account>(hashSet);
            //    Statistic.AllAccounts = (long)FacebookInstances.MultiThrs.Count;
            //}
            //finally
            //{
            //    hashSet = new HashSet<Account>();
            //}
            //return;

        }

        void m_oWorker_ProgressChanged_Acc(object sender, ProgressChangedEventArgs e)
        {
            ProxyStateLb.Text = "Importing Accounts";

        }

        void m_oWorker_RunWorkerCompleted_Acc(object sender, RunWorkerCompletedEventArgs e)
        {
            totalAccTBox.Text = FacebookInstances.MultiThrs.Count.ToString();
            ProxyStateLb.Text = "Import Accounts Done";
        }


        void m_oWorker_Proxy(object sender, DoWorkEventArgs e)
        {
            //beginTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (e == null)
            {

                return;
            }

            List<object> para = e.Argument as List<object>;
            if (para == null || para.Count != 1)
            {

                return;
            }

            string sFileProxy = (string)para[0];


            HashSet<string> hashSet = new HashSet<string>();

            if (sFileProxy == null || sFileProxy.Length <= 0)
            {

                return;
            }

            try
            {

                hashSet.UnionWith(FileLoader.LoadStringsFromFile(sFileProxy));
                ProxyInstances.MultiThrs.queueAccs = new ConcurrentQueue<string>(hashSet);
                ProxyInstances.MultiThrs.ThreadCount = ProxyInstances.MultiThrs.Count;
                ProxyInstances.type = (int)para[0];
            }
            finally
            {

                hashSet = new HashSet<string>();
            }
        }

        void m_oWorker_ProgressChanged_Proxy(object sender, ProgressChangedEventArgs e)
        {
            // Here you play with the main UI thread
            ConcurrentQueue<string> queueHttp = (ConcurrentQueue<string>)e.UserState;
            ProxyStateLb.Text = "new proxy adding to list proxy";
            ProxyStateLb.ForeColor = Color.Red;
            int nCountProxy = queueHttp.Count;
            this.countProxyTBox.Text = nCountProxy.ToString();
            //this.countProxyTBox.ForeColor = Color.Brown;

            startBtn.Enabled = false;
            stopBtn.Enabled = false;

        }

        void m_oWorker_RunWorkerCompleted_Proxy(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (FacebookInstances.MultiThrs.IsPaused)
            //{
            //    FacebookInstances.MultiThrs.Resume();
            //}

            int nCountProxy = 0;
            if (ProxyInstances.Http != null)
            {
                ProxyStateLb.Text = "get list proxy complete";
                ProxyStateLb.ForeColor = Color.Green;
                //nCountProxy += FacebookInstances.Http.Count;
                nCountProxy += ProxyInstances.Http.Count;
            }

            //Statistic.AllProxy = nCountProxy;
            this.countProxyTBox.Text = nCountProxy.ToString();
            //this.countProxyTBox.ForeColor = Color.Brown;
            startBtn.Enabled = true;
            stopBtn.Enabled = true;
        }

        void getProxy()
        {
            ProcessHelper.KillAllProcessTree("chromedriver");
            object ObjectProxy = new List<object>();
            switch (ProxyInstances.type)
            {
                case 0:
                    ObjectProxy = argumentsTinsoftProxy;
                    break;
                case 1:
                    ObjectProxy = argumentsTMProxy;
                    break;
                case 2:
                    ObjectProxy = argumentsProxyRotatingV4;
                    break;
                case 3:
                    ObjectProxy = argumentsProxyRotatingV6;
                    break;
                case 4:
                    ObjectProxy = argumentsTinProxy;
                    break;
                case 5:
                    ObjectProxy = argumentsMinProxy;
                    break;
                case 7:
                    ObjectProxy = argumentsProxyMMONet;
                    break;
                case 8:
                    ObjectProxy = argumentsProxyMobie;
                    break;
                case 9:
                    ObjectProxy = argumentsPOCProxy;
                    break;
                case 10:
                    ObjectProxy = argumentsZProxy;
                    break;
                case 11:
                    ObjectProxy = argumentsNo1Proxy;
                    break;
                case 12:
                    ObjectProxy = argumentsShopLikeProxy;
                    break;
                case 13:
                    ObjectProxy = argumentsPOCWanProxy;
                    break;
                case 14:
                    ObjectProxy = argumentsZProxy2;
                    break;
            }
            if (ObjectProxy == null)
            {
                return;
            }

            List<object> para = ObjectProxy as List<object>;
            if (para == null || para.Count != 1)
            {
                return;
            }
            string sFileProxy = (string)para[0];



            if (sFileProxy == null || sFileProxy.Length <= 0)
            {

                return;
            }

            if (ProxyInstances.type == 0 || ProxyInstances.type == 1 || ProxyInstances.type == 4
                || ProxyInstances.type == 5 || ProxyInstances.type == 7 || ProxyInstances.type == 8
                || ProxyInstances.type == 9 || ProxyInstances.type == 10 || ProxyInstances.type == 11 || ProxyInstances.type == 12
                || ProxyInstances.type == 13 || ProxyInstances.type == 14)
            {
                HashSet<string> hashSet = new HashSet<string>();
                try
                {
                    hashSet.UnionWith(FileLoader.LoadStringsFromFile(sFileProxy));

                    ProxyInstances.MultiThrs.queueAccs = new ConcurrentQueue<string>(hashSet);
                    ProxyInstances.MultiThrs.ThreadCount = ProxyInstances.MultiThrs.Count;

                    ProxyInstances.beginTime = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                    if (ProxyInstances.type == 0)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_TinsoftProxy));
                    else if (ProxyInstances.type == 1)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_TMProxy));
                    else if (ProxyInstances.type == 4)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_TinProxy));
                    else if (ProxyInstances.type == 5)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_MinProxy));
                    else if (ProxyInstances.type == 7)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_ProxyMMONet));
                    else if (ProxyInstances.type == 8)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_ProxyMobie));
                    else if (ProxyInstances.type == 9)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_POCProxy));
                    else if (ProxyInstances.type == 10)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_ZProxy));
                    else if (ProxyInstances.type == 11)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_No1Proxy));
                    else if (ProxyInstances.type == 12)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_ShopLikeProxy));
                    else if (ProxyInstances.type == 13)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_POCWanProxy));
                    else if (ProxyInstances.type == 14)
                        ProxyInstances.MultiThrs.Run(new Action<string>(CoreProxy.Init_ZProxy2));

                    while (ProxyInstances.MultiThrs.IsWorking)
                    {
                        m_oWorker.ReportProgress(0, ProxyInstances.Http);
                        Thread.Sleep(1 * 1000);
                    }

                }
                finally
                {

                    hashSet = new HashSet<string>();
                }

            }
            else
            {
                if (ProxyInstances.type == 2 || ProxyInstances.type == 3)
                {
                    HashSet<string> hashSet = new HashSet<string>();
                    try
                    {
                        hashSet.UnionWith(FileLoader.LoadStringsFromFile(sFileProxy));
                        ProxyInstances.Http = new ConcurrentQueue<string>(hashSet);
                        m_oWorker.ReportProgress(0, ProxyInstances.Http);

                    }
                    finally
                    {

                        hashSet = new HashSet<string>();
                    }

                }
            }
        }

        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (NoneProxyMode.Checked)
                {
                    Thread thread = new Thread(() => WaitChangeIP.Wait2ChangeIP(timeWait2ChangeIP, stopCmd))
                    {
                        IsBackground = true,
                    };
                    thread.Start();
                }

                if (NoneProxyMode.Checked == false)
                {
                    while (true && stopCmd == false)
                    {
                        getProxy();
                        if (ProxyInstances.Http.Count == 0)
                        {
                            int time = 0;
                            while (time < 115)
                            {
                                if (stopCmd == true)
                                {
                                    break;
                                }
                                Thread.Sleep(1 * 1000);
                                time++;
                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    ProxyInstances.userProxy = false;
                }


                if (FacebookInstances.ThrsCount >= FacebookInstances.MultiThrs.Count)
                    FacebookInstances.MultiThrs.ThreadCount = FacebookInstances.MultiThrs.Count;
                else
                    FacebookInstances.MultiThrs.ThreadCount = FacebookInstances.ThrsCount;
                FacebookInstances.MultiThrs.delayTime = FacebookInstances.delayLevel;

                //if (FacebookInstances.requerstMBasic)
                //{
                //    FacebookInstances.MultiThrs.Run(new Action<Account>(Core.requestMbasicLogin));
                //}
                //else
                //{
                if (FacebookInstances.modeRun == 0)
                    FacebookInstances.MultiThrs.Run(new Action<Account>(Core.InitModeHide));
                else if (FacebookInstances.modeRun == 1)
                    FacebookInstances.MultiThrs.Run(new Action<Account>(Core.InitModeShowAll));
                else
                    FacebookInstances.MultiThrs.Run(new Action<Account>(Core.InitModeShowTaskbar));
                // }

                var beginTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                while (FacebookInstances.MultiThrs.IsWorking)
                {
                    while (FacebookInstances.dataSuccess.Count > 0)
                    {
                        string sLine = "";
                        FacebookInstances.dataSuccess.TryDequeue(out sLine);
                        File.AppendAllText(FS.sFileExportSuccess+ $"{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.txt", sLine + Environment.NewLine);
                    }
                    while (FacebookInstances.dataCookiePre.Count > 0)
                    {
                        string sLine = "";
                        FacebookInstances.dataCookiePre.TryDequeue(out sLine);
                        File.AppendAllText(FS.sFileCookiePre, sLine + Environment.NewLine);
                    }

                    while (FacebookInstances.dataNoveri.Count > 0)
                    {
                        string sLine = "";
                        FacebookInstances.dataNoveri.TryDequeue(out sLine);
                        File.AppendAllText(FS.sFileExportSuccess+ $"{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.txt", sLine + Environment.NewLine);
                    }
                    while (FacebookInstances.dataError.Count > 0)
                    {
                        string sLine = "";
                        FacebookInstances.dataError.TryDequeue(out sLine);
                        File.AppendAllText(FS.sFileExportError, sLine + Environment.NewLine);
                    }
                    while (FacebookInstances.dataCheckPoint.Count > 0)
                    {
                        string sLine = "";
                        FacebookInstances.dataCheckPoint.TryDequeue(out sLine);
                        File.AppendAllText(FS.sFileExportCheckPoint, sLine + Environment.NewLine);
                    }
                    while (FacebookInstances.dataLostCookies.Count > 0)
                    {
                        string sLine = "";
                        FacebookInstances.dataLostCookies.TryDequeue(out sLine);
                        File.AppendAllText(FS.sFileExportLostCookies, sLine + Environment.NewLine);
                    }
                    while (FacebookInstances.dataReCheck.Count > 0)
                    {
                        string sLine = "";
                        FacebookInstances.dataReCheck.TryDequeue(out sLine);
                        File.AppendAllText(FS.sFileExportRecheck, sLine + Environment.NewLine);
                    }
                    while (FacebookInstances.dataSpam.Count > 0)
                    {
                        string sLine = "";
                        FacebookInstances.dataSpam.TryDequeue(out sLine);
                        File.AppendAllText(FS.sFileExportRecheck, sLine + Environment.NewLine);
                    }


                    if (NoneProxyMode.Checked == false)
                    {
                        if (ProxyInstances.Http.Count == 0)
                        {
                            if (ProxyInstances.type == 0 || ProxyInstances.type == 1 || ProxyInstances.type == 4
                                || ProxyInstances.type == 5 || ProxyInstances.type == 7 || ProxyInstances.type == 8
                                || ProxyInstances.type == 9 || ProxyInstances.type == 10 || ProxyInstances.type == 11
                                || ProxyInstances.type == 12 || ProxyInstances.type == 13 || ProxyInstances.type == 14)
                            {
                                Thread.Sleep(60 * 1000);
                                if(stopCmd == false)
                                    getProxy();
                            }
                        }
                    }

                    m_oWorker.ReportProgress(1);
                    m_oWorker.ReportProgress(0, ProxyInstances.Http);
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Here you play with the main UI thread
            //string runningStatus = e.UserState.ToString();
            if (e.ProgressPercentage == 0)
            {
                ProxyStateLb.Text = "ĐANG LẤY PROXYS";
                ProxyStateLb.ForeColor = Color.Red;
                ConcurrentQueue<string> queueHttp = (ConcurrentQueue<string>)e.UserState;
                int nCountProxy = queueHttp.Count;
                this.countProxyTBox.Text = nCountProxy.ToString();
                //this.countProxyTBox.ForeColor = Color.Brown;
                this.numBackupTB.Text = FacebookInstances.numRan.ToString();

                var lineBackup = FacebookInstances.numRan.ToString();
                string listAccIndex = listAccCbBox.SelectedItem.ToString();
                var backupPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"backup_{listAccIndex}.txt";
                File.WriteAllText(backupPath, lineBackup);

            }
            else if (e.ProgressPercentage == 1)
            {
                ProxyStateLb.Text = "ĐANG CHẠY GIẢ LẬP";
                ProxyStateLb.ForeColor = Color.Red;

                this.totalAccTBox.Text = FacebookInstances.MultiThrs.Count.ToString();

            }
            startBtn.Enabled = false;
            stopBtn.Enabled = true;

        }

        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Stop();
            runTime = 0;
            ProxyStateLb.Text = "program is finish";
            ProxyStateLb.ForeColor = Color.Green;
            this.startBtn.Enabled = true;
            this.stopBtn.Enabled = true;
            this.clearBackupBtn.Enabled = true;
            this.numBackupTB.Text = FacebookInstances.numRan.ToString();

            var lineBackup = FacebookInstances.numRan.ToString();
            string listAccIndex = listAccCbBox.SelectedItem.ToString();
            var backupPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"backup_{listAccIndex}.txt";
            File.WriteAllText(backupPath, lineBackup);
            System.Windows.Forms.Application.Exit();
            //if (FacebookInstances.MultiThrs.queueAccs.Count == 0)
            //    MessageBox.Show(this, "CHẠY XONG !!!!");
        }
        private void startBtn_Click_1(object sender, EventArgs e)
        {
            IPSTbox.Text = AddIP();
            if (requestMBasicCBox.Checked == true)
            {
                FacebookInstances.requerstMBasic = true;
            }

            if(checkLocateCBox.Checked == true)
            {
                FacebookInstances.checkLocate = true;
            }

            timer1.Start();
            
            if (urlRandom.Checked)
            {
                FacebookInstances.urlType = 0;
            }
            else if (urlNomal.Checked)
            {
                FacebookInstances.urlType = 1;
            }
            else if (urlFBCom.Checked)
            {
                FacebookInstances.urlType = 2;
            }
            else if (urlMbasic.Checked)
            {
                FacebookInstances.urlType = 3;
            }
            else if (urlMFB.Checked)
            {
                FacebookInstances.urlType = 4;
            }

            FacebookInstances.urlCustom = urlCustom.Text;

            CoreProxy.ips = IPSTbox.Text;
            FacebookInstances.pageLoadTime = (int)pageLoadUD.Value;
            FacebookInstances.linkFix = linkFixCBox.Text;
            this.startBtn.Enabled = false;
            this.clearBackupBtn.Enabled = false;
            if (this.CookieSpCB.Checked)
                FacebookInstances.CookieSP = true;
            else
                FacebookInstances.CookieSP = false;

            if (this.accPreCBox.Checked)
                FacebookInstances.AccSP = true;
            else
                FacebookInstances.AccSP = false;

            if (this.getTokenCB.Checked)
                FacebookInstances.getToken = true;
            else
                FacebookInstances.getToken = false;

            if (this.checkIPCB.Checked)
                FacebookInstances.checkIP = true;
            else
                FacebookInstances.getToken = false;

            if (this.fakeUserAgentCB.Checked)
                FacebookInstances.fakeUserAgent = true;
            else
                FacebookInstances.fakeUserAgent = false;
            this.totalAccTBox.Text = FacebookInstances.MultiThrs.Count.ToString();
            if (this.hideModeRB.Checked)
                FacebookInstances.modeRun = 0;
            else if (this.showTaskbarModeRB.Checked)
                FacebookInstances.modeRun = 2;
            else
                FacebookInstances.modeRun = 1;

            if (this.TmProxyMode.Checked)
                ProxyInstances.type = 1;
            else if (this.TinsoftProxyMode.Checked)
                ProxyInstances.type = 0;
            else if (this.ProxyRoratingV4Mode.Checked)
                ProxyInstances.type = 2;
            else if (this.ProxyRoratingV6Mode.Checked)
                ProxyInstances.type = 3;
            else if (this.tinProxyMode.Checked)
                ProxyInstances.type = 4;
            else if (this.minProxyMode.Checked)
                ProxyInstances.type = 5;
            else if (this.ProxyMMONetMode.Checked)
                ProxyInstances.type = 7;
            else if (this.ProxyMobieMode.Checked)
                ProxyInstances.type = 8;
            else if (this.POCProxyMode.Checked)
                ProxyInstances.type = 9;
            else if (this.ZProxyMode.Checked)
                ProxyInstances.type = 10;
            else if (this.ProxyNo1Mode.Checked)
                ProxyInstances.type = 11;
            else if (this.ProxyShopLikeMode.Checked)
                ProxyInstances.type = 12;
            else if (this.POCWanProxyMode.Checked)
                ProxyInstances.type = 13;
            else if (this.ZProxy2Mode.Checked)
                ProxyInstances.type = 14;

            if (addExtensionCBox.Checked)
            {
                FacebookInstances.addEx = true;
            }

            timeWait2ChangeIP = (int)numericUpDownChangeIP.Value*60*1000;


            // add nThread
            if (!int.TryParse(this.threadCountNumUD.Text.ToString(), out FacebookInstances.ThrsCount))
            {
                MessageBox.Show("Select threads!", "Error");
                return;
            }

            //delayTypingLevel
            if (!int.TryParse(this.delayLevelUD.Text.ToString(), out FacebookInstances.delayLevel))
            {
                MessageBox.Show("Select threads!", "Error");
                return;
            }

            // reset data
            nIndex = 0;
            if (lsDataResult != null)
            {
                lsDataResult.Clear();
            }

            if (dtListResultOTP != null)
            {
                dtListResultOTP.DataSource = null;
                dtListResultOTP.Rows.Clear();
            }

            //int.TryParse(this.threadCountNumUD.Text.ToString(), out FacebookInstances.ThrsCount)
            ProxyInstances.timeUpdate = (int)this.TimeUpdateProxyUD.Value * 60 * 1000;
            if (!m_oWorker.IsBusy && !statusWorker.IsBusy)
            {
                stopCmd = false;
                m_oWorker.RunWorkerAsync();
                statusWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Chờ chút, chương trình chưa giải phóng xong");
            }
        }

        void statusWorkerDowork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (m_oWorker.IsBusy)
                {
                    if (runTime % 3600 == 0)
                    {
                        var value = doneInHour;
                        statusWorker.ReportProgress(1, value);
                        doneInHour = 0;
                        Thread.Sleep(200);
                        value = lostCookiesInHour;
                        statusWorker.ReportProgress(2, value);
                        lostCookiesInHour = 0;
                        Thread.Sleep(200);
                        value = checkpointInHour;
                        statusWorker.ReportProgress(3, value);
                        checkpointInHour = 0;
                        Thread.Sleep(200);
                        value = spamInHour;
                        statusWorker.ReportProgress(4, value);
                        spamInHour = 0;
                        Thread.Sleep(200);
                        value = errorInHour;
                        statusWorker.ReportProgress(5, value);
                        errorInHour = 0;
                    }

                    Thread.Sleep(200);
                }
            }
            catch { }
        }

        void statusWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage == 1)
                {
                    success1HourTBox.Text = e.UserState.ToString();
                }
                if (e.ProgressPercentage == 2)
                {
                    lost1HourTBox.Text = e.UserState.ToString();
                }
                if (e.ProgressPercentage == 3)
                {
                    cp1HourTBox.Text = e.UserState.ToString();
                }
                if (e.ProgressPercentage == 4)
                {
                    spam1HourTBox.Text = e.UserState.ToString();
                }
                if (e.ProgressPercentage == 5)
                {
                    error1HourTBox.Text = e.UserState.ToString();
                }
            }
            catch { }
        }
        void statusWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.startBtn.Enabled = true;
            this.stopBtn.Enabled = true;
        }


        private void stopBtn_Click_1(object sender, EventArgs e)
        {
            runTime = 0;
            this.clearBackupBtn.Enabled = true;
            try
            {
                stopCmd = true;
                Thread.Sleep(2 * 1000);
                FacebookInstances.MultiThrs.Cancel();
                this.totalAccTBox.Text = FacebookInstances.MultiThrs.Count.ToString();
                ProxyInstances.MultiThrs.Cancel();
                ProxyInstances.Clear(ProxyInstances.Http);
                while (FacebookInstances.MultiThrs.IsWorking)
                {
                    ProxyStateLb.Text = "program is canceling";
                    ProxyStateLb.ForeColor = Color.Red;
                    Thread.Sleep(2 * 1000);
                }
                m_oWorker.CancelAsync();
                m_oWorker.Dispose();
                ProcessHelper.KillAllProcessTree("chromedriver");
            }
            catch { }
            
        }

        private void showAllModeRB_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void SaveTypeAccBtn_Click(object sender, EventArgs e)
        {
            FacebookInstances.typeAcc = this.typeAccTB.Text;
        }

        private void resultBtn_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "Result//");
        }

        private void viewDataBtn_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "data//");
        }

        private void SaveSettingBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var threadCount = this.threadCountNumUD.Value.ToString();
                var delayTypingLevel = this.delayLevelUD.Value.ToString();
                var typeAcc = this.typeAccTB.Text;



                var modeRun = "0";
                if (this.hideModeRB.Checked)
                {
                    modeRun = "0";
                }
                else if (this.showAllModeRB.Checked)
                {
                    modeRun = "1";

                }
                else if (this.showTaskbarModeRB.Checked)
                {
                    modeRun = "2";
                }

                var ProxySP = this.CookieSpCB.Checked ? "1" : "0";

                var chooseModeProxy = "0";
                if (this.TmProxyMode.Checked)
                {
                    chooseModeProxy = "1";
                }
                else if (this.ProxyRoratingV4Mode.Checked)
                {
                    chooseModeProxy = "2";
                }
                else if (this.ProxyRoratingV6Mode.Checked)
                {
                    chooseModeProxy = "3";
                }
                else if (this.tinProxyMode.Checked)
                {
                    chooseModeProxy = "4";
                }
                else if (this.minProxyMode.Checked)
                {
                    chooseModeProxy = "5";
                }
                else if (this.NoneProxyMode.Checked)
                {
                    chooseModeProxy = "6";
                }
                else if (this.ProxyMMONetMode.Checked)
                {
                    chooseModeProxy = "7";
                }
                else if (this.ProxyMobieMode.Checked)
                {
                    chooseModeProxy = "8";
                }
                else if (this.POCProxyMode.Checked)
                {
                    chooseModeProxy = "9";
                }
                else if (this.ZProxyMode.Checked)
                {
                    chooseModeProxy = "10";
                }
                else if (this.ProxyNo1Mode.Checked)
                {
                    chooseModeProxy = "11";
                }
                else if (this.ProxyShopLikeMode.Checked)
                {
                    chooseModeProxy = "12";
                }
                else if (this.POCWanProxyMode.Checked)
                {
                    chooseModeProxy = "13";
                }
                else if (this.ZProxy2Mode.Checked)
                {
                    chooseModeProxy = "14";
                }

                var TimeUpdate = this.TimeUpdateProxyUD.Value.ToString();

                var TimeRefresh = TimeRefreshUD.Value.ToString();

                string fakeUserAgent = "0";
                if (fakeUserAgentCB.Checked)
                {
                    fakeUserAgent = "1";
                }

                string linkFix = linkFixCBox.Text;

                string accIndex = listAccCbBox.SelectedItem.ToString();

                string ips = IPSTbox.Text;

                string pageLoadTime = pageLoadUD.Value.ToString();

                //checkIPCB
                string checkIP = "0";
                if (checkIPCB.Checked)
                {
                    checkIP = "1";
                }
                string addEx = "0";
                if (addExtensionCBox.Checked)
                {
                    addEx = "1";
                }

                var urlMode = "0";
                if (urlRandom.Checked)
                {
                    urlMode = "0";
                }
                else if (urlNomal.Checked)
                {
                    urlMode = "1";
                }
                else if (urlFBCom.Checked)
                {
                    urlMode = "2";
                }
                else if (urlMbasic.Checked)
                {
                    urlMode = "3";
                }
                else if (urlMFB.Checked)
                {
                    urlMode = "4";
                }
                var accPre = "0";
                if (accPreCBox.Checked)
                {
                    accPre = "1";
                }
                var checkLocate = "0";
                if (checkLocateCBox.Checked)
                {
                    checkLocate = "1";
                }

                var url = urlCustom.Text == null ? "": urlCustom.Text;
                var lineSetting = threadCount + "|" + delayTypingLevel + "|" + typeAcc + "|" + modeRun + "|"
                    + ProxySP + "|" + chooseModeProxy + "|" + TimeUpdate + "|" + TimeRefresh + "|" + fakeUserAgent
                    + "|" + linkFix + "|" + accIndex + "|" + ips + "|" + pageLoadTime + "|" + checkIP + "|" + addEx
                    + "|" + urlMode + "|" + url + accPre + "|" + checkLocate;
                var settingPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "Setting.txt";
                File.WriteAllText(settingPath, lineSetting);
            }
            catch { }
        }

        private void clearSettingBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var settingPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + "Setting.txt";
                if (File.Exists(settingPath))
                {
                    File.Delete(settingPath);
                }
            }
            catch { }
        }

        private void clearBackupBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string listAccIndex = listAccCbBox.SelectedItem.ToString();
                var backupPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"backup_{listAccIndex}.txt";
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                    FacebookInstances.numRan = 0;
                    this.numBackupTB.Text = FacebookInstances.numRan.ToString();
                }

                var accountPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"Account_{listAccIndex}.txt";
                if (File.Exists(accountPath))
                {
                    m_oWorkerImportAcc.RunWorkerAsync(accountPath);
                }

            }
            catch { }
        }

        private void keyLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Clipboard.SetText(this.keyLinkLabel.Text);
            //MessageBox.Show("Đã copy key thành công, CTRL + V vào notepad để lấy key !", "Thông báo",
            //    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LocaltionIpCbBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.SelectedIndex != null)
            {
                LocationIp cl = cb.SelectedValue as LocationIp;
                ProxyInstances.location = cl;
            }
        }

        private void listAccCbBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.SelectedIndex != null)
            {
                string cl = cb.SelectedItem.ToString();
                //

                string fileAccPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"Account_{cl}.txt";
                if (!File.Exists(fileAccPath))
                {
                    MessageBox.Show("Hiện không có file account: " + $"Account_{cl}.txt");
                    cb.SelectedIndex = 0;
                    return;
                }


                accTBox.Text = Path.GetFileName(fileAccPath);
                this.ProxyStateLb.Text = "Importing: " + $"Account_{cl}.txt";

                if (!m_oWorkerImportAcc.IsBusy)
                {
                    m_oWorkerImportAcc.RunWorkerAsync(fileAccPath);
                }

                var backupPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"backup_{cl}.txt";
                if (File.Exists(backupPath))
                {
                    var backup = File.ReadAllText(backupPath);
                    FacebookInstances.numRan = Int32.Parse(backup);
                    this.numBackupTB.Text = FacebookInstances.numRan.ToString();
                }
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                string listAccIndex = listAccCbBox.SelectedItem.ToString();
                var backupPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"backup_{listAccIndex}.txt";
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                    FacebookInstances.numRan = 0;
                    this.numBackupTB.Text = FacebookInstances.numRan.ToString();
                }

                var accountPath = AppDomain.CurrentDomain.BaseDirectory + "data//" + $"Account_{listAccIndex}.txt";
                if (File.Exists(accountPath))
                {
                    m_oWorkerImportAcc.RunWorkerAsync(accountPath);
                }

            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            runTime += 1;
            timeLabel.Text = TimeSpan.FromSeconds(runTime).ToString("hh':'mm':'ss");
        }


        public static string AddIP()
        {
            try
            {
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
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
                driver = new ChromeDriver(service, options);
                driver.Navigate().GoToUrl("https://ipinfo.io/ip");
                Thread.Sleep(2 * 1000);
                var ip = driver.FindElement(By.TagName("body")).Text;
                driver.Close();
                driver.Quit();
                return ip;
            }
            catch
            {
                return "";
            }
        }
    }
}
