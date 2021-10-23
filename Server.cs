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
        private static IPAddress myIP;
        private static int port;
        private static TcpListener mTCPListener;
        public static List<TcpClient> mClients;
        public static IDictionary<string, TcpClient> TCPCLS;
        public static bool KeepRunning { get; set; }


        public static async void StartListeningForIncomingConnection(IPAddress _ip, int _port,RichTextBox l=null,ListBox ls=null, bool notfirst = false)
        {
           
            if(TCPCLS == null)
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
                    await returnedbyAccept.GetStream().WriteAsync(HostName,0,HostName.Length);
                    TakeCareOfTCPclient(returnedbyAccept,l,ls);
                    //what is here will be executed after the up line

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }



        }

        private async static void TakeCareOfTCPclient(TcpClient ParamClient,RichTextBox l,ListBox ls)
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
                ls.Items.Add(ParamClient.Client.RemoteEndPoint.ToString());
                TCPCLS.Add(ParamClient.Client.RemoteEndPoint.ToString(),ParamClient);

                char[] buff = new char[64];
                while (KeepRunning)
                {
                    int Nret = await reader.ReadAsync(buff, 0, buff.Length);

                    byte[] _buff = Encoding.UTF8.GetBytes(buff);
                    Console.WriteLine("Returned : " + Nret);
                    if (Nret == 0)
                    {
                        RemoveClient(ParamClient,l,ls);
                        Console.WriteLine("Socket Disconnected");
                        l.Text += "\nSocket Disconnected";
                        break;
                    }
                    string recivedtext = new string(buff);
                    Console.WriteLine("*** RECIVED : " + recivedtext + "\nFrom:" + ParamClient.Client.RemoteEndPoint.ToString());
                    l.Text += "\nFrom:"+ ParamClient.Client.RemoteEndPoint.ToString() + "@[#$]>Recived : " + recivedtext;
                    Array.Clear(buff, 0, buff.Length);

                }

            }
            catch (Exception ex)
            {
                RemoveClient(ParamClient,l,ls);
                Console.WriteLine(ex.Message);
            }
        }
        public static async void SendStrToSpecificClient(string ClientName,string Data)
        {
            if (TCPCLS.ContainsKey(ClientName))
            {
                byte[] tobesend = Encoding.UTF8.GetBytes(Data);
                await TCPCLS[ClientName].GetStream().WriteAsync(tobesend,0,tobesend.Length);
            }
        }
        private static string SearchByClient(TcpClient ParamClient) {
            
            for(int i = 0; i < TCPCLS.Count; i++)
            {
                if (TCPCLS[TCPCLS.ToList()[i].Key] == ParamClient)
                {
                    return TCPCLS.ToList()[i].Key;
                }
            }
            return "";
        }
        private static void RemoveClient(TcpClient paramClient,RichTextBox l,ListBox ls)
        {
            if (mClients.Contains(paramClient))
            {
                mClients.Remove(paramClient);
                Console.WriteLine(string.Format("Client Removed, Count : {0}", mClients.Count));
                l.Text += "\n" + string.Format("Client Removed, Count : {0}", mClients.Count);
                TCPCLS.Remove(SearchByClient(paramClient));
                ls.Items.Remove(paramClient.Client.RemoteEndPoint.ToString());
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
        public static async void Sendstr(string message , TcpClient cl)
        {

        }
        public static async void SendStrToAll(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            try
            {
                byte[] buffMessage = Encoding.UTF8.GetBytes(message);
                foreach (TcpClient c in mClients)
                {

                    c.GetStream().WriteAsync(buffMessage, 0, buffMessage.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
