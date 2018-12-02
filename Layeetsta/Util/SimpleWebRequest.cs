using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Layeetsta.Util
{
    public class SWRequest
    {
        public static string RequestJson(string url, string header, string value)
        {
            try
            {
                HttpWebRequest r = (HttpWebRequest)HttpWebRequest.Create(url);
                if (r != null)
                {
                    r.Method = "GET";
                    r.Timeout = 10000;
                    r.ContentType = "application/json";
                    r.Headers.Add(header, value);

                    using (System.IO.Stream s = r.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();
                            return jsonResponse;
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string RequestDL(string url, string agent, string filename)
        {
                HttpWebRequest r = (HttpWebRequest)HttpWebRequest.Create(url);
                if (r != null)
                {
                    r.Method = "GET";
                    r.Timeout = 10000;
                    r.UserAgent = agent;


                    var buff = new byte[1024];
                    int pos = 0;
                    int count;
                    using (Stream stream = r.GetResponse().GetResponseStream())
                    {
                        using (var fs = new FileStream(filename, FileMode.Create))
                        {
                            do
                            {
                                count = stream.Read(buff, pos, buff.Length);
                                fs.Write(buff, 0, count);
                            } while (count > 0);
                        return filename;
                        }
                    }
                }
                return null;
        }
    }
}
