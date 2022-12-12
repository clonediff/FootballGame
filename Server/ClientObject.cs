using Protocol;
using Protocol.Packets;
using Protocol.Protocol;
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
                while (true)
                {
                    try
                    {
                        var t = await Stream.ReadPacketAsync();
                        await ProccessIncomingPacketAsync(t);
                    } catch
                    {
                        break;
                    }
                }
                //await server.BroadcastMessageAsync(t, Id);

                // получаем имя пользователя
                //string? userName = await Reader.ReadLineAsync();
                //string? message = $"{userName} вошел в чат";
                //// посылаем сообщение о входе в чат всем подключенным пользователям
                //await server.BroadcastMessageAsync(message, Id);
                //Console.WriteLine(message);
                //// в бесконечном цикле получаем сообщения от клиента
                //string? message = "";
                //while (true)
                //{
                //    try
                //    {
                //        message = await Reader.ReadLineAsync();
                //        if (message == null) continue;
                //        message = $": {message}";
                //        Console.WriteLine(message);
                //        await server.BroadcastMessageAsync(message, Id);
                //    }
                //    catch
                //    {
                //        message = $"покинул чат";
                //        Console.WriteLine(message);
                //        await server.BroadcastMessageAsync(message, Id);
                //        break;
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(Id);
            }
        }

        private async Task ProccessIncomingPacketAsync(Packet packet)
        {
            var type = PacketTypeManager.GetTypeFromPacket(packet);

            switch (type)
            {
                case PacketType.Handshake:
                    await ProccessHandshakeAsync(packet);
                    break;
                case PacketType.TestPacket:
                    ProccessTestPacket(packet);
                    break;
            }
        }

        private void ProccessTestPacket(Packet packet)
        {
            var testPacket = PacketConverter.Deserialize<TestPacket>(packet);
            Console.WriteLine(testPacket);
        }

        private async Task ProccessHandshakeAsync(Packet packet)
        {
            Console.WriteLine("Handshake recieved!");

            var handshake = PacketConverter.Deserialize<HandshakePacket>(packet);
            handshake.MagicHandshakeNumber -= 15;

            Console.WriteLine("Answering...");
            var newPacket = PacketConverter.Serialize(PacketType.Handshake, handshake);
            await Stream.WritePacketAsync(newPacket);
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
