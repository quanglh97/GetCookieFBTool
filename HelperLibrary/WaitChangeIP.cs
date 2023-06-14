using GetCookieFBTool.HelperLibrary.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetCookieFBTool.HelperLibrary
{
    public class WaitChangeIP
    {
        public static void Wait2ChangeIP(int timeWait, bool stop)
        {
            while (true)
            {
                if (stop) break;
                FacebookInstances.waitChangeIP = true;
                MessageBox.Show("OK");
                FacebookInstances.waitChangeIP = false;
                Thread.Sleep(timeWait);

            }
        }
    }
}
