using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RIO;
using RIO.Models;

namespace test1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Api rio = new Api("https://192.168.50.8", "admin", "Russ8000");
                rio.LogOn();

                if (rio.IsConnected)
                {
                    Console.WriteLine("Conectado.");

                    //Credential credential = new Credential("000000001", 37);
                    //Door door = new Door("Demo", "Lucas", "R1");
                    //bool response = rio.ReportCardSwipe(door, credential);


                    //var response = rio.SetInterfaceOffline("Demo", "Lucas");

                    var response = rio.SetInterfaceOnline("Demo", "Lucas");

                    if (response)
                    {
                        Console.WriteLine($"Interface Offline");
                    }

                    Console.WriteLine(response);
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }
    }
}
