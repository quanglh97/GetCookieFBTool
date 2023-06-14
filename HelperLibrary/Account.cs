using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetCookieFBTool.HelperLibrary
{
    public class Account
    {
        public string userName;
        public string pass;
        public string key2fa;
        public string locate;
        public string locateNew;

        public Account()
        {
        }

        public Account(string newAccount)
        {
            try
            {
                var elementAccount = newAccount.Split('|');
                this.userName = elementAccount[0].Trim();
                this.pass = elementAccount[1].Trim();
                if (elementAccount.Length > 2)
                    this.key2fa = elementAccount[2].Trim();
                if (elementAccount.Length > 3)
                    this.locate = elementAccount[3].Trim();
            }
            catch
            {
               
            }
        }

        public override bool Equals(object obj) => obj is Account account && account.userName == this.userName;
        public override int GetHashCode() => this.userName.GetHashCode();
    }
}
