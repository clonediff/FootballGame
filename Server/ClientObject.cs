using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ClientObject
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();

        protected internal NetworkStream Stream { get; }
        protected internal StreamReader Reader { get; }
        protected internal StreamWriter Writer { get; }

        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            // получаем NetworkStream для взаимодействия с сервером
            Stream = client.GetStream();

            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                var t = await Stream.ReadPacketAsync();
                //await server.BroadcastMessageAsync(t, Id);
                Console.WriteLine(t);

                // получаем имя пользователя
                //string? userName = await Reader.ReadLineAsync();
                //string? message = $"{userName} вошел в чат";
                //// посылаем сообщение о входе в чат всем подключенным пользователям
                //await server.BroadcastMessageAsync(message, Id);
                //Console.WriteLine(message);
                //// в бесконечном цикле получаем сообщения от клиента
                string? message = "";
                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message == null) continue;
                        message = $": {message}";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                    }
                    catch
                    {
                        message = $"покинул чат";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
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
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(Id);
            }
        }
        // закрытие подключения
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
