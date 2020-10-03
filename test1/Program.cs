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
                //rio.LogOn();

                //if (rio.IsConnected)
                //{
                    Console.WriteLine("Conectado.");

                    Credential credential = new Credential("00000001", 37);
                    Door door = new Door("Demo", "Lucas", "R1");

                    var response = rio.ReportCardSwipe(door, credential);

                    Console.WriteLine(response);
                //}

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
