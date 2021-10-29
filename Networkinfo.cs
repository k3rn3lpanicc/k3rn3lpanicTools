using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace k3rn3lpanicTools
{
    public class Networkinfo
    {
        public static string PublicIPAddress_m1()
        {
            return new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim(); 
        }
        public static string PublicIPAddress_m2()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }
        public static IPAddress getIPByHostname(string hostname)
        {
            return Dns.GetHostAddresses(hostname)[0];
        }
        public static string GetRequest(string url)
        {
            return new WebClient().DownloadString(url).Trim();
        }

    }
}
