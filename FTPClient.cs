using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace k3rn3lpanicTools
{
    public class FTPClient
    {
        public bool IsUpload { get; set; }

        private string domain;
        private string username;
        private string password;
        public FTPClient(string _Domain , string _username, string _password , bool _IsUpload) {
            domain = _Domain;
            username = _username;
            password = _password;
            IsUpload = _IsUpload;
        }
        public bool DoWork(string filename) {
            try
            {
                bool _success = true;
                if (IsUpload)
                {
                    using (var client = new WebClient())
                    {
                        
                        client.Credentials = new NetworkCredential(username, password);
                        client.UploadFile(domain+"/K3rn3lPanic/"+"capt.panic", WebRequestMethods.Ftp.UploadFile,filename);
                        
                    }
                }
                else
                {
                    using (var client = new WebClient())
                    {

                        client.Credentials = new NetworkCredential(username, password);
                        
                        client.DownloadFile(domain + "/K3rn3lPanic/" + "capt.panic", filename);

                    }
                    
                }
                return _success;
            }
            catch {
                return false;
            }
        }


    }
}
