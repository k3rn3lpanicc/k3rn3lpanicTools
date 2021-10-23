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
    public class Client
    {
        private static IPAddress mServerIPAddress;
        private static int mServerPort;
        private static TcpClient mClient;

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

        public static async void ConnectToServer(RichTextBox l=null)
        {
            if (l == null)
                l = new RichTextBox();

             mClient = new TcpClient();
            try
            {
                await mClient.ConnectAsync(mServerIPAddress, mServerPort);
                l.Text += "\n" + string.Format("Connected to server {0}:{1}", mServerIPAddress, mServerPort);
                Console.WriteLine(string.Format("Connected to server {0}:{1}", mServerIPAddress, mServerPort));
                SendDataAsync(Encoding.UTF8.GetBytes(SystemInfo.GetInfo(SystemInfo.InfoType.Machinename)));
                ReadDataAsync(mClient,l);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        private static async void ReadDataAsync(TcpClient mClient,RichTextBox l)
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
                    byte[] buff = new byte[64];
                    readBytesCount =  await stream.ReadAsync(buff, 0, buff.Length);
                    if (readBytesCount <= 0)
                    {
                        Console.WriteLine("Disconnected");
                        l.Text += "\nDisconnected";

                        mClient.Close();
                        break;
                    }
                    Console.WriteLine("***Recived : " + Encoding.UTF8.GetString(buff));
                       l.Text += "\n"+ "@[#$]>Recived : " + Encoding.UTF8.GetString(buff);
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
                mClient.Close();
                Console.WriteLine(excp.Message);
                return;
                //throw;
            }

        }
        public static async void SendDataAsync(byte[] buff)
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
        public static void sendStr(string data)
        {
            byte[] _data = Encoding.UTF8.GetBytes(data);
            byte[] tosend = _data;//AddByteToArray(_data,0);
            SendDataAsync(tosend);
        }
        private static byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }
        public static void SendFile(string filename)
        {
            byte[] file = File.ReadAllBytes(filename);
            byte[] senddata = AddByteToArray(file, 1);
            SendDataAsync(senddata);
        }
    }
}
