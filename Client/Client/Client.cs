using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace ClientProject
{
    public class Client
    {
        public int ID;
        public string ipAddr;
        public int port;
        public IPEndPoint iPEndPoint;
        public Socket socket;
        public Client()
        {
            this.ID++;

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public Client(Socket socket)
        {

            this.socket = socket;

        }

        public void Connect()
        {
            int exit = 0;
            bool trycon = false;
            do
            {
                try
                {
                    socket.Connect(iPEndPoint);
                    trycon = true;
                    Console.WriteLine("Connect success!");
                }
                catch (Exception)
                {

                    Console.WriteLine("Try to connect...");
                    exit++;
                    Thread.Sleep(700);

                }
                if (exit == 5)
                {
                    Console.WriteLine("Try to Connect Failed");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            } while (!trycon);
            Console.Clear();
        }
        public void SendMsg(string sms)
        {
            byte[] data = new byte[256];
            data = Encoding.Unicode.GetBytes(sms);
            socket.Send(data);
        }
        public void SendMsg(string[] sms)
        {
            string outstr = String.Empty;
            byte[] data = new byte[256];
            foreach (var item in sms)
            {
                data = Encoding.Unicode.GetBytes(outstr += $"{Path.GetFileName(item)}\n");
            }


            socket.Send(data);
        }
        public StringBuilder GetMsg()
        {
            int bytes = 0;
            byte[] data = new byte[256];
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                bytes = socket.Receive(data);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (socket.Available > 0);

            if (stringBuilder.ToString().ToLower() == "exit")
            {
                Environment.Exit(0);
            }
            return stringBuilder;
        }
        public void GetServerCommand(StringBuilder command)
        {
            string[] arr_command = command.ToString().Split("\n");
            string[] tmp = { };
            if (arr_command[0].ToLower() == "search")
            {
                try
                {
                    tmp = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop", "*", SearchOption.AllDirectories);
                    SendMsg(tmp);
                }
                catch (Exception ex)
                {

                    SendMsg(ex.Message);
                }
            }
            if (arr_command[0].ToLower() == "start")
            {
                string[] arg = { };
                try
                {
                    tmp = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop", "*", SearchOption.AllDirectories);

                    foreach (var item in tmp)
                    {
                        if (item.ToLower().EndsWith(arr_command[1].ToLower()))
                        {
                            arg = item.Split(".");
                            Process.Start(new ProcessStartInfo(item, arg[1]) { UseShellExecute = true });
                            SendMsg("Success start!");
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    SendMsg(ex.Message);

                }
            }
            if (arr_command[0].ToLower() == "changesize")
            {
                ChangeSize(arr_command);
            }
            

        }
        public void Search()
        {
            try
            {
                SendMsg(Directory.GetFiles(@$"C:\Users\" + $"{Environment.UserName}" + @"\Desktop", "*", SearchOption.AllDirectories));
            }
            catch (Exception ex)
            {
                SendMsg(ex.Message);
            }
        }
        public void ReadServerConfig(RegistryKey key)
        {
            if (key.OpenSubKey("ServerConfig") == null)
            {
                RegistryKey servKey = key.CreateSubKey("ServerConfig");
                servKey.SetValue("IpServer", "127.0.0.1");
                servKey.SetValue("PortServer", "8000");
            }
            else
            {
                RegistryKey servKey = key.OpenSubKey("ServerConfig");
                this.iPEndPoint = new IPEndPoint(IPAddress.Parse(servKey.GetValue("IpServer").ToString()), int.Parse(servKey.GetValue("PortServer").ToString()));
            }
        }
        public bool ExistRegisteryDir(RegistryKey key)
        {

            bool check = true;
            foreach (var item in key.GetSubKeyNames())
            {
                if (item.Contains("ConsoleSize"))
                {

                    check = false;
                }
            }

            return check;
        }

        public void ChangeSize(string[] sizes)
        {
            RegistryKey key = Registry.CurrentUser;

            RegistryKey newKey = key.OpenSubKey("ConsoleSize", true);


            newKey.SetValue("WIDTH", sizes[1]);
            newKey.SetValue("HEIGHT", sizes[2]);


            Console.BufferWidth = int.Parse(newKey.GetValue("WIDTH").ToString());
            Console.BufferHeight = int.Parse(newKey.GetValue("HEIGHT").ToString());

        }

    }
}