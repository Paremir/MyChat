using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Net;
using System.Threading;

namespace Chat
{
    class Server
    {
        static TcpListener tcpListener;
        List<Client> clients = new List<Client>();

        protected internal void AddConnection(Client clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            Client client = clients.FirstOrDefault(c => c.Id == id);
            if (client !=null)
            {
                clients.Remove(client);
            }
        }
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    Client client = new Client(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(client.Process));
                    clientThread.Start();
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                Disconnect();
            }
        }
        protected internal void BroadcastMessage(string message, string id, string color)
        
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            byte[] colordata = Encoding.Unicode.GetBytes(color);

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id !=id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                    clients[i].Stream.Write(colordata, 0, colordata.Length);

                }
            }

        }
        protected internal void Disconnect()
        {
            tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}
