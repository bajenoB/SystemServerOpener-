using Microsoft.Win32;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientProject
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            RegistryKey key = Registry.CurrentUser;
            Client client = new Client();
            client.ReadServerConfig(key);
            client.Connect();

            if (!client.ExistRegisteryDir(key))
            {

                RegistryKey newKey = key.CreateSubKey("ConsoleSize");
                newKey.SetValue("Height", "500");
                newKey.SetValue("Width", "800");

            }
            else
            {
                RegistryKey newKey = key.OpenSubKey("ConsoleSize");

                Console.BufferHeight = int.Parse(newKey.GetValue("Height").ToString());
                Console.BufferWidth = int.Parse(newKey.GetValue("Width").ToString());
            }




            while (true)
            {

                client.GetServerCommand(client.GetMsg());

            }


        }
    }
}