using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using static GetCookieFBTool.HelperLibrary.Facebook.FacebookData;

namespace GetCookieFBTool.HelperLibrary.Facebook
{
    public class Login
    {

        public static bool LoginMethodMBasic(Request request, string email, string password, string authenCode)
        {
            FacebookData data = new FacebookData();

            var fbField =  data.GetFieldsMBasicFB(request);

            if (fbField == null)

                return false;

            #region POST LOGIN

            request.AddHeader("referer", "https://mbasic.facebook.com/login");

            request.AddParam("lsd", fbField.lsd);

            request.AddParam("jazoest", fbField.jazoest);

            request.AddParam("m_ts", fbField.m_ts);

            request.AddParam("li", fbField.li);

            request.AddParam("try_number", "0");

            request.AddParam("unrecognized_tries", "0");

            request.AddParam("email", email);

            request.AddParam("pass", password);

            request.AddParam("login", "Log In");

            request.AddParam("bi_xrwh", "0");

            string b = request.RequestPost("https://mbasic.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100");

            #endregion

            var secretKey = Base32Encoding.ToBytes(authenCode);
            var totp = new Totp(secretKey);
            var code = totp.ComputeTotp();

            string uriCP = request.Uri();

            if (uriCP.Contains("/home.php"))

                return true;

            var fbCp = data.GetFB_DTSG_MBasic(request, uriCP);

            if (fbCp == null)

                return false;

            #region POST LOGIN 2FA

            request.AddParam("fb_dtsg", fbCp.fb_dtsg);

            request.AddParam("jazoest", fbCp.jazoest);

            request.AddParam("nh", fbCp.nh);

            request.AddParam("checkpoint_data", "");

            request.AddParam("approvals_code", code);

            request.AddParam("codes_submitted", "0");

            request.AddParam("submit[Submit Code]", "Submit Code");

            request.RequestPost("https://mbasic.facebook.com/login/checkpoint/");

            #endregion


            #region POST SAVE DEVICE

            request.AddParam("fb_dtsg", fbCp.fb_dtsg);

            request.AddParam("jazoest", fbCp.jazoest);

            request.AddParam("nh", fbCp.nh);

            request.AddParam("checkpoint_data", "");

            request.AddParam("name_action_selected", "save_device");

            request.AddParam("submit[Continue]", "Continue");

            string a = request.RequestPost("https://mbasic.facebook.com/login/checkpoint/");

            #endregion

            if (request.GetCookie().Contains("c_user"))

                return true;

            else

                return false;
        }

        public static bool LoginMethodMFB(Request request, string email, string password, string authenCode)
        {
            FacebookData data = new FacebookData();

            var fbField = data.GetFieldsMFB(request);

            if (fbField == null)

                return false;

            #region POST LOGIN

            //request.AddHeader("referer", "https://m.facebook.com/login");
            if(fbField.lsd !="")
                request.AddParam("lsd", fbField.lsd);
            if (fbField.jazoest != "")
                request.AddParam("jazoest", fbField.jazoest);
            if (fbField.m_ts != "")
                request.AddParam("m_ts", fbField.m_ts);
            if (fbField.li != "")
                request.AddParam("li", fbField.li);

            request.AddParam("try_number", "0");

            request.AddParam("unrecognized_tries", "0");

            request.AddParam("email", email);

            request.AddParam("pass", password);

            request.AddParam("login", "Log In");

            request.AddParam("bi_xrwh", "0");

            string b = request.RequestPost("https://m.facebook.com/login/device-based/regular/login/?refsrc=deprecated&lwv=100");

            #endregion

            var secretKey = Base32Encoding.ToBytes(authenCode);
            var totp = new Totp(secretKey);
            var code = totp.ComputeTotp();

            string uriCP = request.Uri();

            if (uriCP.Contains("/home.php"))

                return true;

            var fbCp = data.GetFB_DTSG_MFB(request, uriCP);

            if (fbCp == null)

                return false;

            #region POST LOGIN 2FA

            request.AddParam("fb_dtsg", fbCp.fb_dtsg);

            request.AddParam("jazoest", fbCp.jazoest);

            request.AddParam("nh", fbCp.nh);

            request.AddParam("checkpoint_data", "");

            request.AddParam("approvals_code", code);

            request.AddParam("codes_submitted", "0");

            request.AddParam("submit[Submit Code]", "Submit Code");

            var a = request.RequestPost("https://m.facebook.com/login/checkpoint/");

            #endregion


            #region POST SAVE DEVICE

            request.AddParam("fb_dtsg", fbCp.fb_dtsg);

            request.AddParam("jazoest", fbCp.jazoest);

            request.AddParam("nh", fbCp.nh);

            request.AddParam("checkpoint_data", "");

            request.AddParam("name_action_selected", "save_device");

            request.AddParam("submit[Continue]", "Continue");

            string respone = request.RequestPost("https://m.facebook.com/login/checkpoint/");

            #endregion

            if (request.GetCookie().Contains("c_user"))

                return true;

            else

                return false;
        }

        public static string getTokenRequest(Request request, string uid, string authenCode)
        {

            var secretKey = Base32Encoding.ToBytes(authenCode);
            var totp = new Totp(secretKey);
            var code = totp.ComputeTotp();
            string resFields = request.RequestGet("https://business.facebook.com/content_management");
            var regexString = "\\[\"DTSGInitialData\",\\[\\],\\{\"token\":\"(.*?)\"";
            var tokenEAAG = "";
            var fb_dtsg = Regex.Match(resFields, regexString).Groups[1].Value;
            if (request.Uri().Contains("security/twofactor/reauth/?"))
            {
                var response = request.RequestPost("https://business.facebook.com/security/twofactor/reauth/enter/", string.Concat(new string[]
                        {
                        "approvals_code=",
                        code,
                        "&save_device=false&__user=",
                        uid,
                        "&__a=1&__dyn=7xeUmFoO3-SudwCwRyU8EKnFwLBwCwXCwAxu13wqovzEdEnxy7Eiwzwq8S2S4okwAwSz82EyEqx60DU4m0_87Oq1eK1VgC11x-9xm1WxO4Uowuo9oeUa85vzo1eE4aUS1vwnEfU7e2l2UtggzE2DwiUmwnGwWx2365Ey19zUuxe0y83mwkE5G4E7K1uDwau58Gm0hi4Ejyo-3qazo8U3ywbS1bwzw&__csr=&__req=9&__hs=19108.BP:DEFAULT.2.0.0.0.&dpr=1.5&__ccg=EXCELLENT&__rev=1005404255&__s=n7y7to:i7topm:vo61pg&__hsi=7090761015361603355-0&__comet_req=0&fb_dtsg=",
                        fb_dtsg,
                        "&jazoest=21954&lsd=Qf7z54nRePaFgnLxvbGQyN&__spin_r=1005404255&__spin_b=trunk&__spin_t=1650946451&__jssesw=1"
                        }), "application/x-www-form-urlencoded").ToString();
                if (response.Contains("confirmed")){
                    string result = request.RequestGet("https://business.facebook.com/content_management");
                    tokenEAAG = "EAAG" + Regex.Match(result, "\"accessToken\":\"EAAG(.*?)\"").Groups[1].Value;
                }
            }
            else
            {
                tokenEAAG = "EAAG" + Regex.Match(resFields, "\"accessToken\":\"EAAG(.*?)\"").Groups[1].Value;
            }
            if (tokenEAAG.Length < 6)
                return null;
            return tokenEAAG;
        }
    }
}
