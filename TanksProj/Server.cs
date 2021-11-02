﻿using System;
using ClientServer;
using TankLib;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 8000);
            List<Tank> tanks = new List<Tank>();
            List<Task> tankThreads = new List<Task>();
            server.Start();

            Task clientsUpdate = new Task(() => {
                while (true)
                {
                    server.ConnectionUpdate();
                    if (server.handler.Count < 5)
                        server.Send(Server.FromStringToBytes("nice"), server.handler.Count-1);
                    else
                        server.Send(Server.FromStringToBytes("full"), server.handler.Count - 1);

                    if (server.handler.Count < 5)
                    {
                        tankThreads.Add(new Task(() =>
                        {
                            string ip = server.handler.Last().socket.RemoteEndPoint.ToString();
                            int index = server.handler.IndexOf(server.handler.Where((item) => item.socket.RemoteEndPoint.ToString() == ip).First());
                            Console.WriteLine(ip);

                            tanks.Add(new Tank());

                            while (true)
                            {


                                if (!server.handler[index].socket.Connected)
                                {
                                    server.handler.RemoveAt(index);
                                    tanks.RemoveAt(index);
                                    break;
                                }

                                List<byte> data = server.Get(index);
                                if (data.Count != 0)
                                {


                                    try
                                    {
                                        Tank info = JsonSerializer.Deserialize<Tank>(Server.FromBytesToString(data));
                                        tanks[index] = info;
                                    }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("Lost Bytes");

                                    }

                                }
                                GC.Collect(GC.GetGeneration(data));



                            }


                        }));
                        tankThreads.Last().Start();
                    }
                    
                }
            });
            clientsUpdate.Start();


            try
            {
                while (true)
                {
                    try
                    {
                        for (int i = 0; i < tanks.Count; i++)
                        {
                            server.Send(Server.FromStringToBytes(JsonSerializer.Serialize(tanks)), i);
                        }
                        Thread.Sleep(5);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Send Error!");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            server.Close();
        }
    }
}
