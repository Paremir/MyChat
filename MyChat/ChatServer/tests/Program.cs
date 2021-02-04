using Chat;
using System;
using System.Threading;
namespace tests
{

    class Program
    {
        static Server server;
        static Thread listernTread;
        static void Main(string[] args)
        {
            try
            {
                server = new Server();
                listernTread = new Thread(new ThreadStart(server.Listen));
                listernTread.Start();
            }
            catch (Exception e)
            {
                server.Disconnect();
                Console.WriteLine(e.Message);
            }

        }

    }
}
