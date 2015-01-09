using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAP.Middleware.Connector;

namespace ConsoleApplication1
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.Write("Username: ");
            string _username = Console.ReadLine();
            Console.Write("Password: ");
            string _password = Console.ReadLine();


            
            SAPSystemConnect sapcfg = new SAPSystemConnect();
            sapcfg.ApplicationServer = "rb3pa786.server.bosch.com";
            sapcfg.Username = _username;
            sapcfg.Password = _password;

            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(sapcfg);
                RfcDestination rfcDest = RfcDestinationManager.GetDestination("P78");
                RfcRepository repo = rfcDest.Repository;
                Console.WriteLine("Connected!");
            }
            catch (RfcLogonException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
