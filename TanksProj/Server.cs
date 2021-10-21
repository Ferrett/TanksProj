using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ClientServer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BroadcastMessengerConsole
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 8000);
            server.Start();
            Task task = new Task(() => server.ConnectionUpdate());
            task.Start();
            Tank tmp = new Tank();
            while (true)
            {

                if (server.handler.Count != 0)
                {


                    tmp = JsonSerializer.Deserialize<Tank>(Server.FromBytesToString(server.Get()));
                  
                    
            

                    //Server.FromBytesToString(server.GetAll());

                    //server.Send(Server.FromStringToBytes(str), index);
                }
            }

            server.Close();
        }
    }
}
