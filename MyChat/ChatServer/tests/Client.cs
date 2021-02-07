using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    class Client
    {
        public string Id { get; set; }
        public NetworkStream Stream { get; set; }
        string userName;
        TcpClient client;
        Server server;
        string color;

        public Client(TcpClient tcpclient,Server conserver) 
        {
            Id = Guid.NewGuid().ToString();
            client = tcpclient;
            server = conserver;
            server.AddConnection(this);

            color = (new Random().Next(0, 16)).ToString();

        }
        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                userName = message;
                if (userName == "Алина")
                    color = "13";
                Console.ForegroundColor = (ConsoleColor)Int32.Parse(color);
                Console.Write(userName);
                Console.ResetColor();
                Console.WriteLine(" вошел в чат");
                message = userName + ": вошел в чат";
                server.BroadcastMessage(message, this.Id,this.color);
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        Console.ForegroundColor = (ConsoleColor)Int32.Parse(color);
                        Console.Write(userName+" : ");
                        Console.ResetColor();
                        Console.WriteLine(message);
                        message = String.Format("{0}: {1}",userName, message);
                        server.BroadcastMessage(message, this.Id,this.color);
                    }
                    catch
                    {
                        Console.ForegroundColor = (ConsoleColor)Int32.Parse(color);
                        Console.Write(userName);
                        Console.ResetColor();
                        Console.WriteLine(" покинул чат");
                        message = String.Format("{0}: покинул чат", userName);
                        server.BroadcastMessage(message, this.Id,this.color);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);
            return builder.ToString();
        }
        protected internal void Close()
        {
            if (Stream !=null)
            {
                Stream.Close();
            }
            if (client !=null)
            {
                client.Close();
            }
        }
    }
}
