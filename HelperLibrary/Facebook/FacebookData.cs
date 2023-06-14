using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GetCookieFBTool.HelperLibrary.Facebook
{
    public class FacebookData
    {
        public FieldsLogin GetFieldsMFB(Request request)
        {
           
            try
            {
                string resFields = request.RequestGet("https://m.facebook.com/");
                FieldsLogin fields = new FieldsLogin()
                {
                    fb_dtsg = Regex.Match(resFields, "name=\"fb_dtsg\" value=\"(.*?)\"").Groups[1].Value,
                    jazoest = Regex.Match(resFields, "name=\"jazoest\" value=\"(.*?)\"").Groups[1].Value,
                    lsd = Regex.Match(resFields, "name=\"lsd\" value=\"(.*?)\"").Groups[1].Value,
                    m_ts = Regex.Match(resFields, "name=\"m_ts\" value=\"(.*?)\"").Groups[1].Value,
                    li = Regex.Match(resFields, "name=\"li\" value=\"(.*?)\"").Groups[1].Value
                };
                return fields;
            }
            catch
            {

                return null;
            }


        }
        public FieldsLogin GetFieldsMBasicFB(Request request)
        {
            try
            {
                string resFields = request.RequestGet("https://mbasic.facebook.com/");
                FieldsLogin fields = new FieldsLogin()
                {
                    fb_dtsg = Regex.Match(resFields, "name=\"fb_dtsg\" value=\"(.*?)\"").Groups[1].Value,
                    jazoest = Regex.Match(resFields, "name=\"jazoest\" value=\"(.*?)\"").Groups[1].Value,
                    lsd = Regex.Match(resFields, "name=\"lsd\" value=\"(.*?)\"").Groups[1].Value,
                    m_ts = Regex.Match(resFields, "name=\"m_ts\" value=\"(.*?)\"").Groups[1].Value,
                    li = Regex.Match(resFields, "name=\"li\" value=\"(.*?)\"").Groups[1].Value
                };
                return fields;
            }
            catch
            {

                return null;
            }
        }

        public FieldsLogin GetFB_DTSG_MBasic(Request request, string uri = "https://mbasic.facebook.com/login/checkpoint/")
        {
            try
            {
                string resFields = request.RequestGet(uri);
                FieldsLogin fields = new FieldsLogin()
                {
                    fb_dtsg = Regex.Match(resFields, "name=\"fb_dtsg\" value=\"(.*?)\"").Groups[1].Value,
                    jazoest = Regex.Match(resFields, "name=\"jazoest\" value=\"(.*?)\"").Groups[1].Value,
                    nh = Regex.Match(resFields, "name=\"nh\" value=\"(.*?)\"").Groups[1].Value
                };
                return fields;
            }
            catch
            {

                return null;
            }
        }

        public FieldsLogin GetFB_DTSG_MFB(Request request, string uri = "https://m.facebook.com/")
        {
            try
            {
                string resFields = request.RequestGet(uri);
                FieldsLogin fields = new FieldsLogin()
                {
                    fb_dtsg = Regex.Match(resFields, "name=\"fb_dtsg\" value=\"(.*?)\"").Groups[1].Value,
                    jazoest = Regex.Match(resFields, "name=\"jazoest\" value=\"(.*?)\"").Groups[1].Value,
                    nh = Regex.Match(resFields, "name=\"nh\" value=\"(.*?)\"").Groups[1].Value
                };
                return fields;
            }
            catch
            {

                return null;
            }
        }

        public FieldsLogin GetFb_DTSG_BUSSINESS(Request rq)
        {
            //["DTSGInitialData",[],{"token":"
            try
            {
                string resFields = rq.RequestGet("https://business.facebook.com/content_management");
                var regexString = "\\[\"DTSGInitialData\",\\[\\],\\{\"token\":\"(.*?)\"";
                FieldsLogin fields = new FieldsLogin()
                {
                    fb_dtsg = Regex.Match(resFields, regexString).Groups[1].Value,
                    
                };
                return fields;
            }
            catch
            {

                return null;
            }
        }

        public class FieldsLogin
        {
            public string lsd { get; set; }
            public string jazoest { get; set; }
            public string m_ts { get; set; }
            public string li { get; set; }
            public string fb_dtsg { get; set; }
            public string nh { get; set; }
        }
    }
}
