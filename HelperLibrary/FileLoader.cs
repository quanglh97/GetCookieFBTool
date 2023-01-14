using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetCookieFBTool.HelperLibrary
{
    internal class FileLoader
    {

        public static HashSet<Account> LoadBaseFromFile(string filePath)
        {
            HashSet<Account> Accounts = new HashSet<Account>();
            if (!File.Exists(filePath))
                return Accounts;
            try
            {
                foreach (string readLine in File.ReadLines(filePath))
                {
                    try
                    {
                        if (readLine != null)
                        {
                            Account account = new Account(readLine);
                            if (!string.IsNullOrEmpty(account.userName))
                            {
                                if (!string.IsNullOrEmpty(account.pass))
                                {
                                    Accounts.Add(account);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                //Accounts.ExceptBadDomains();
                return Accounts;
            }
            finally
            {
                HashSet<Account> accountSet = new HashSet<Account>();
            }
        }

        public static HashSet<Account> LoadBaseFromText(string sAccount)
        {
            HashSet<Account> Accounts = new HashSet<Account>();

            try
            {
                Account account = new Account(sAccount);
                if (!string.IsNullOrEmpty(account.userName))
                {
                    if (!string.IsNullOrEmpty(account.pass))
                    {
                        Accounts.Add(account);
                    }
                }
                return Accounts;
            }
            finally
            {
                HashSet<Account> accountSet = new HashSet<Account>();
            }

            return null;
        }

        public static HashSet<string> LoadStringsFromFile(string filePath)
        {
            HashSet<string> stringSet = new HashSet<string>();
            if (!File.Exists(filePath))
                return stringSet;
            foreach (string readLine in File.ReadLines(filePath))
            {
                try
                {
                    if (readLine != null)
                        stringSet.Add(readLine);
                }
                catch
                {
                }
            }
            return stringSet;
        }
    }
}
