using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // string host = Dns.GetHostName();
            // IPHostEntry ipHost = Dns.GetHostEntry(host);
            // IPAddress ipAddr = ipHost.AddressList[0];
            IPAddress ipAddr = IPAddress.Parse("192.168.35.155");
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 5280);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 5);

            Thread.Sleep(2000);
            SessionManager.Instance.SendRegisterReq();

            Thread.Sleep(2000);
            SessionManager.Instance.SendLoginReq();

            while (true)
            {
                try
                {
                    // SessionManager.Instance.SendForEach();
                    // SessionManager.Instance.SendSyncFish();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(5000);
            }
        }
    }
}