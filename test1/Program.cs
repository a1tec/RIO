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
            Credential credential = new Credential("000000001", 37);
            Door door = new Door("Demo", "Lucas", "R1");

            try
            {
                Api rio = new Api("https://192.168.50.8", "admin", "Russ8000");

                bool success = rio.LogOn();

                if (success)
                {
                    Console.WriteLine("Connected.");
                }

                success = rio.ReportCardSwipe(door, credential);

                if (success)
                {
                    Console.WriteLine($"Card Swipe Reported");
                }

                success = rio.SetInterfaceOffline("Demo", "Lucas");

                if (success)
                {
                    Console.WriteLine($"Interface Offline Now");
                }

                success = rio.SetInterfaceOnline("Demo", "Lucas");

                if (success)
                {
                    Console.WriteLine($"Interface Online Now");
                }

                success = rio.ReportOfflineAccess(door, credential, DateTime.Now.AddDays(-1), true);

                if (success)
                {
                    Console.WriteLine($"Reported Offline Access");
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
