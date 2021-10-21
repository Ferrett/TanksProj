using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public string Last;

        public Server(string ip, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            handler = new List<Client>();
            tank = new List<Tank>();
            actions = new List<Action<int>>();
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

            //this.Send(Server.FromStringToBytes("Connected"), handler.Count - 1);
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
       

        public List<byte> Get(int index)
        {
            List<byte> data = new List<byte>();
            int bytes = 0;
            byte[] array = new byte[255];

            do
            {
                
                    bytes = handler[index].socket.Receive(array, array.Length, 0);
                    for (int i = 0; i < bytes; i++)
                    {

                        data.Add(array[i]);
                    }

            } while (handler[index].socket.Available > 0);

            Console.WriteLine(handler[index].socket.RemoteEndPoint.ToString());
            return data;
        }
        public List<byte> Get()
        {
            List<byte> data = new List<byte>();
            int bytes = 0;
            byte[] array = new byte[255];

            for (int i = 0; i < tank.Count; i++)
            {
              do
              {
                   
                for (int j = 0; j < bytes; j++)
                {

                    data.Add(array[j]);
                }

              } while (handler[i].socket.Available > 0);
              Console.WriteLine(handler[i].socket.RemoteEndPoint.ToString());
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
