using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClientServer
{
    public class Server
    {
        private Socket socket;
        private IPEndPoint ipPoint;
        public List<Client> handler { get; }
        public List<Tank> tank { get; }
        public List<Action<int>> actions { set; get; }

        public List<Task> tasks { get; set; }



        public Server(string ip, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            handler = new List<Client>();
            tank = new List<Tank>();
            tasks = new List<Task>();
            actions = new List<Action<int>>();

            TimerCallback tm = new TimerCallback(SendTanks);
            Timer timer = new Timer(tm, 0, 0, 100);
        }

        public void Start()
        {
            socket.Bind(ipPoint);
            socket.Listen(10);

        }
        public void AddClient(string ip, int port)
        {
            handler.Add(new Client(socket.Accept(), "127.0.0.1", 8000));
            tank.Add(new Tank());
            Console.WriteLine("NEW PLAYER");

            tasks.Add(new Task(() =>
            {
                int idx = handler.Count - 1;
                string ip = handler[idx].socket.RemoteEndPoint.ToString();

                while (true)
                {
                    Get(ip);
                    HandlerCheck();

                   


                    try
                    {
                        tank[idx] = JsonSerializer.Deserialize<Tank>(FromBytesToString(Get(ip)));


                    }
                    catch (Exception)
                    {

                            
                    }
                        

                    if (!handler.Any(x => x.socket.RemoteEndPoint.ToString() == ip))
                    {
                       
                        break;
                    }
                }

            }));
            tasks.Last().Start();

            //this.Send(Server.FromStringToBytes("Connected"), handler.Count - 1);
        }
        public void SendTanks(object obj)
        {
            Console.WriteLine("Call");
            string json = JsonSerializer.Serialize<List<Tank>>(tank);
            for (int i = 0; i < handler.Count; i++)
            {
                Send(FromStringToBytes(json), i);
            }
        }
        public bool HandlerCheck()
        {
            for (int i = 0; i < handler.Count; i++)
            {
                if (!handler[i].socket.Connected)
                {
                    handler.Remove(handler[i]);
                    tank.RemoveAt(i);
                    return false;
                }
               
            }
            return true;
        }
        public void Send(List<byte> data, int index)
        {
            handler[index].socket.Send(data.ToArray());
        }
        public void RemoveClient(int id)
        {
            //this.Send(Server.FromStringToBytes("Disconnected"), id);
            this.handler[id].socket.Close();
            this.handler.RemoveAt(id);
            tank.RemoveAt(id);
        }
        public void ShowAllUsers(Server server)
        {
            Console.WriteLine("\nUser List:");
            for (int i = 0; i < server.handler.Count; i++)
            {
                Console.WriteLine($"#{i + 1}: {server.handler[i].socket.RemoteEndPoint}");
            }
            Console.WriteLine();
        }
       

        public List<byte> Get(string ip)
        {
            List<byte> data = new List<byte>();
            int bytes = 0;
            byte[] array = new byte[255];
           
            try
            {


                do
                {
                    try
                    {
                        bytes = handler.Where(x=>x.socket.RemoteEndPoint.ToString() == ip).ToList()[0].socket.Receive(array, array.Length, 0);
                        for (int i = 0; i < bytes; i++)
                        {

                            data.Add(array[i]);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("DISCONNECT");
                        //tasks.RemoveAt(index);
                        // handler.RemoveAt(index);
                        data.Clear();

                  
                        break;
                    }

                } while (handler.Where(x => x.socket.RemoteEndPoint.ToString() == ip).ToList()[0].socket.Available > 0);

        
                    try
                    {
                        //Console.WriteLine(handler.Where(x => x.socket.RemoteEndPoint.ToString() == ip).ToList()[0].socket.RemoteEndPoint.ToString());
                        for (int i = 0; i < tank.Count; i++)
                        {

                        
                            Console.WriteLine(tank[i]);
                        //SendTanks();
                        }
                    }
                    catch (Exception)
                    {

                    }

            }
            catch (Exception)
            {


            }
            return data;
        }

       
        public void Close()
        {
            for (int i = 0; i < handler.Count; i++)
            {
                handler[i].socket.Shutdown(SocketShutdown.Both);
                handler[i].Close();
            }
        }
        public void ConnectionUpdate()
        {
            while (true)
            {
                this.AddClient("127.0.0.1", 8000);

            }
        }
        public static string FromBytesToString(List<byte> bytes)
        {
            return Encoding.Unicode.GetString(bytes.ToArray());
        }
        public static List<byte> FromStringToBytes(string str)
        {
            return Encoding.Unicode.GetBytes(str).ToList();
        }
    }
}
