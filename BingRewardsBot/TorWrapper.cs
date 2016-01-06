using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace BingRewardsBot
{
    class TorWrapper
    {
        TcpClient sock;
        Process tor;

        public TorWrapper()
        {
            //Start tor
            ProcessStartInfo torInfo = new ProcessStartInfo();
            torInfo.WorkingDirectory = "C:/Users/mflduncan/Documents/visual studio 2012/Projects/BingBot 2.1/BingBot 2.1/tor/Tor/";
            torInfo.FileName = "C:/Users/mflduncan/Documents/visual studio 2012/Projects/BingBot 2.1/BingBot 2.1/tor/Tor/tor.exe";
            torInfo.Arguments = "-f torrc.txt";
            tor = Process.Start(torInfo);

            //Start socket
            sock = new TcpClient();
            sock.Connect("127.0.0.1", 9051);
            SendCommand("AUTHENTICATE\r\n");
        }

        public void SendCommand(string command)
        {

            NetworkStream stream = sock.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(command.ToCharArray());
            stream.Write(data, 0, data.Length);
            stream.Flush();


            byte[] inStream = new byte[10025];
            stream.Read(inStream, 0, 10025);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            //Console.WriteLine(returndata);
        }

        public string GetIp()
        { 

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://api.ipify.org");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            return result;
        }
        public void Quit()
        {
            tor.Close();
            sock.Close();
        }
    }
}
