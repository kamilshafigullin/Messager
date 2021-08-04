using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static TcpListener listener = new TcpListener(IPAddress.Any, 5050);
        static List<ConnectedClient> clients = new List<ConnectedClient>();

        static void Main(string[] args)
        {
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();

                Task.Factory.StartNew(() =>
                {
                    var sr = new StreamReader(client.GetStream());
                    while (client.Connected)
                    {
                        var line = sr.ReadLine();

                        if (line.Contains("Login: "))
                        {
                            var nick = line.Replace("Login: ", "");

                            if (clients.FirstOrDefault(s => s.Name == nick) == null)
                            {
                                clients.Add(new ConnectedClient(client, nick));
                                Console.WriteLine($"New connection: {nick}");

                                SendToAllClients($"{nick} joined");

                                break;
                            }
                            else
                            {
                                var sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;

                                sw.WriteLine("Пользователь с таким ником уже есть в чате");
                                client.Client.Disconnect(false);
                            }
                        }
                    }

                    while (client.Connected)
                    {
                        try
                        {
                            var line = sr.ReadLine();
                            SendToAllClients(line);
                        }
                        catch { }
                    }
                });
            }
        }

        static async void SendToAllClients(string message)
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    try
                    {
                        var sw = new StreamWriter(clients[i].Client.GetStream());
                        sw.AutoFlush = true;

                        sw.WriteLine(message);
                    }
                    catch { }
                }
            });
        }
    }
}
