using System.ServiceModel;
using System.ServiceModel.Web;
using System;
using System.Web;
using System.Net;
using Rockey4NDControl;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Net.Sockets;

namespace ConsoleApp4
{
    class Program
    {
        static string IPAddress;

        static void Main(string[] args)
        {
            //Class1 dc = new Class1();
            //Console.WriteLine(dc.EncryptPassword("WashPlan,GlobalDashBoard,LineDashBoard,TNA,Production,LinePlan,", true));
            IPAddress = GetIPAddress();
            Uri baseAddress = new Uri("http://0.0.0.0:8091/");
            WebServiceHost svcHost = new WebServiceHost(new APICommandManager("Test"), baseAddress);
            ((ServiceBehaviorAttribute)svcHost.Description.Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode = InstanceContextMode.Single;

            try
            {
                //start api
                Console.WriteLine("API Starting at : " + baseAddress.ToString());
                svcHost.Open();

                Console.WriteLine("API Started at : " + baseAddress.ToString());
                string param1 = HttpUtility.ParseQueryString(baseAddress.Query).Get("param1");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            while (true) ;
        }

        static public string GetIPAddress()
        {
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;
        }        
    }
}
