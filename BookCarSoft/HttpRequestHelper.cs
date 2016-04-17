using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace BookCarSoft
{
    class HttpRequestHelper
    {

        public static string url = "http://www.51purse.com/aboutCar!isExistUserName.action";
        public static string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        public static string userAgent = "ios_xuechebu;V3.00";
        public static string contentType = "";
        public static string referer = "";
        public static CookieContainer cookies = new CookieContainer();

        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        //获取cookie数据
        public static bool getUserLimitData(string userName)
        {
            //generate http request
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            //add follow code to handle cookies
            req.CookieContainer = new CookieContainer();
            //req.CookieContainer.Add(curCookies);
            req.KeepAlive = true;
            req.Accept = "text/html, application/xhtml+xml, */*";
            req.Method = "POST";
            req.KeepAlive = true;
            req.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            string postData = "{\"userName\":\"" + userName + "\"}";
            byte[] postdatabyte = Encoding.UTF8.GetBytes(postData);
            req.ContentLength = postdatabyte.Length;

            using (Stream stream = req.GetRequestStream())
            {
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();
            }
            try
            {
                //use request to get response
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string html = string.Empty;
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    html = reader.ReadToEnd();

                    reader.Close();
                }

                if (html.IndexOf("\"isAuth\":\"true\"") > -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {

                return false;
            }
        }
        //替换\r\n和空格
        public static string replaceComma(string html)
        {
            if (html != null && !"".Equals(html))
            {
                html = html.Replace("\\r\\n", "").Replace("\r\n", "");
                html = html.Replace("\\\"", "\"");
                html = html.Replace(": ", ":").Replace(" ", "");
                return html;
            }
            else
            {
                return "";
            }
        }
        //替换\r\n和空格
        public static string getXnsdName(string xnsd)
        {
            if (xnsd.Equals("711"))
            {
                return "上午";
            }
            else if (xnsd.Equals("1216"))
            {
                return "下午";
            }
            else
            {
                return "晚上";
            }
        }
        //替换\r\n和空格
        public static string getXnsd(string name)
        {
            if (name.Equals("上午"))
            {
                return "711";
            }
            else if (name.Equals("下午"))
            {
                return "1216";
            }
            else
            {
                return "1720";
            }
        }
        /** 
         * splitAry方法<br> 
         * 2014-8-4 上午10:45:36 
         * @param ary 要分割的数组 
         * @param subSize 分割的块大小 
         * @return 
         * 
         */
        public static Object[] splitAry(string[] ary, int subSize)
        {
            int count = ary.Length % subSize == 0 ? ary.Length / subSize : ary.Length / subSize + 1;

            List<List<string>> subAryList = new List<List<string>>();

            for (int i = 0; i < subSize; i++)
            {
                int index = i * count;
                List<string> list = new List<string>();
                int j = 0;
                while (j < count && index < ary.Length)
                {
                    list.Add("" + ary[index++]);
                    j++;
                }
                subAryList.Add(list);
            }

            Object[] subAry = new Object[subAryList.Count];

            for (int i = 0; i < subAryList.Count; i++)
            {
                List<string> subList = subAryList[i];
                string[] subAryItem = new string[subList.Count];
                for (int j = 0; j < subList.Count; j++)
                {
                    subAryItem[j] = subList[j];
                }
                subAry[i] = subAryItem;
            }

            return subAry;
        }

        //Post方式请求 
        public static string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.Accept = accept;
            request.KeepAlive = true;
            request.UserAgent = userAgent;
            request.ContentType = contentType;
            request.AllowAutoRedirect = true;
            request.Referer = referer;
            request.CookieContainer = cookies;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream);
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.Cookies = cookies.GetCookies(response.ResponseUri);
            foreach (Cookie ck in response.Cookies)
            {
                cookies.Add(ck);
            }
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        //Post方式请求 ，发送JSON格式数据,查询科目三的时候，必须发送JSON数据
        public static string HttpPostByJson(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.Accept = "*/*";
            request.KeepAlive = true;
            request.UserAgent = userAgent;
            request.ContentType = contentType;
            request.AllowAutoRedirect = true;
            request.Referer = "";
            request.ContentType = "application/json; charset=UTF-8";
            request.CookieContainer = cookies;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream);
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.Cookies = cookies.GetCookies(response.ResponseUri);
            foreach (Cookie ck in response.Cookies)
            {
                cookies.Add(ck);
            }
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        //Get方式请求
        public static string HttpGet(string Url, string postDataStr, bool isReadHtml)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.CookieContainer = cookies;
            request.Method = "GET";
            request.Accept = accept;
            request.KeepAlive = true;
            request.UserAgent = userAgent;
            request.ContentType = contentType;
            request.AllowAutoRedirect = true;
            request.Referer = referer;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            foreach (Cookie ck in response.Cookies)
            {
                if (ck != null) 
                {
                    if (ck.Name.Equals("Webapi_LoginOn")) 
                    {
                        ck.Name = "JX_LoginOn";
                        cookies.Add(ck);
                    }else
                    {
                        cookies.Add(ck);
                    }
                }
            }
            string htmlString = "";
            if (isReadHtml)
            {
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                htmlString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            response.Close();
            request.Abort();
            return htmlString;
        }

    }
}
