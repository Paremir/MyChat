using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;
        static void Main(string[] args)
        {
            Console.WriteLine("Введите своё имя: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port);
                stream = client.GetStream();
                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0,data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                Console.WriteLine($"Добро пожаловать, {userName}");
                SendMessage();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        static void SendMessage()
        {
            while (true)
            {
                Console.WriteLine("Введите сообщение:");
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;

                    byte[] colordata = new byte[32];

                    StringBuilder colorbuilder = new StringBuilder();
                    int colorbytes = 0;

                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        colorbytes = stream.Read(colordata, 0, colordata.Length);
                        colorbuilder.Append(Encoding.Unicode.GetString(colordata, 0, colorbytes));
                    } while (stream.DataAvailable);

                    string message = builder.ToString();
                    string color = colorbuilder.ToString();
                    string[] words = message.Split(':');
                    Console.ForegroundColor = (ConsoleColor)Int32.Parse(color);
                    Console.Write(words[0]);
                    Console.ResetColor();
                    Console.Write(":" );
                    Console.WriteLine(words[1]+ "\nВведите сообщение:"); 
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine("Подключение прервано! " + ex.Message);
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }
        
        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}
