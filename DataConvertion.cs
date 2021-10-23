using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k3rn3lpanicTools
{
    public class DataConvertion
    {
        public static bool ByteToFile(byte[] data , string filename) {
            if (!Tools.IsFileInUse(filename))
            {
                File.WriteAllBytes(filename,data);
                return true;
            }
            else {
                return false;
            }
        }
        public static byte[] FileToByte(string file) {
            if (!Tools.IsFileInUse(file))
                return File.ReadAllBytes(file);
            else
                return new byte[] { };
        }
        public static string ByteToStr(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
        public static byte[] StrToByte(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
    }
}
