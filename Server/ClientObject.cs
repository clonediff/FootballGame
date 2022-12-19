using FootballLogicLib;
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
    public delegate Task ProccesPacket(Packet packet);

    internal class ClientObject
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();
        protected internal Player Player { get; private set; }

        protected internal NetworkStream Stream { get; }
        protected internal StreamReader Reader { get; }
        protected internal StreamWriter Writer { get; }

        TcpClient client;
        ServerObject server; // объект сервера

        internal Dictionary<PacketType, ProccesPacket> ProccesPackets { get; }

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            // получаем NetworkStream для взаимодействия с сервером
            Stream = client.GetStream();

            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);

            ProccesPackets = new()
            {
                [PacketType.Connect] = ProccessPlayerConnect
            };
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
                        // покинуть
                        Console.WriteLine("Пока");
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
                server.RemoveConnection(Id);
            }
        }

        private async Task ProccessIncomingPacketAsync(Packet packet)
        {
            var type = PacketTypeManager.GetTypeFromPacket(packet);

            if (ProccesPackets.TryGetValue(type, out var proccess))
                await proccess(packet);
            else
            {
                // Unknown Packet
            }
        }

        private async Task ProccessPlayerConnect(Packet packet)
        {
            var connect = PacketConverter.Deserialize<ConnectPlayer>(packet);
            Player = new Player { Id = Id, TeamName = connect.Team };
            server.SendPlayersList(Id);
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
