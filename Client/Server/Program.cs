using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClientProject;
using System.Threading;

namespace ServerProject
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.StartServer();
            try
            {
                Task.Factory.StartNew(() => server.Connects());
                while (true)
                {

                    Console.WriteLine($"[1]Start Browser\n[2]Disconnect from server\n[3]Check download apps\n[4]Change a register data");
                    Console.WriteLine("Выберите дейстие:");
                    try
                    {
                        server.SendCommand(int.Parse(Console.ReadLine()));
                        Console.Clear();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Только числа![1][2][3]");
                        Console.Clear();
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}