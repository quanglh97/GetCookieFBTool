using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetCookieFBTool.HelperLibrary
{
    public class dgvOutput : EventArgs
    {
        public int nID { get; set; }
        public string sOutput { get; set; }

        public dgvOutput(int nID, string sOutput)
        {
            this.nID = nID;
            this.sOutput = sOutput;
        }
    }

    public class dgvEventArgs : EventArgs
    {
        public Account InfoAccount { get; set; }

        public string status { get; set; }

        public string cookies { get; set; }

        public string token { get; set; }

        public string userAgent { get; set; }
        public string loacate { get; set; }
        public string newLocate { get; set; }


        public dgvEventArgs(Account newacc,string status, string cookies, string token, string userAgent)
        {
            this.InfoAccount = newacc;
            this.status = status == null ? "" : status;
            this.cookies = cookies == null ? "" : cookies;
            this.token = token == null? "": token;
            this.userAgent = userAgent == null ? "" : userAgent;
            this.loacate = newacc.locate == null ? "" : newacc.locate;
            this.newLocate = newacc.locateNew == null ? "" : newacc.locateNew;

        }
    }
}
