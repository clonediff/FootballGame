using FootballLogicLib;
using Protocol;
using Protocol.Packets;
using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace UIApplication.Connection
{
    public static class ConnectionManager
    {
        public const string HOST = "127.0.0.1";
        public static string Team { get; set; }

        public static TcpClient Client { get; private set; }

        public static NetworkStream Stream { get; private set; }

        public static event Action OnCantConnect;
        public static event Action<Player> OnConnect;
        public static event Action<Player[]> OnPlayersList;

        static ConnectionManager()
        {
            PacketTypesRegistrator.RegisterTypes();
        }

        public static async Task ConnectAndRunAsync()
        {
            if (Client?.Client is null)
                Client = new();
            Client.Connect(HOST, 8888);
            Stream = Client.GetStream();
            // запускаем ввод сообщений
            Task.Run(() => ReceiveMessageAsync());
            await SendConnectMessageAsync();
        }

        private static async Task SendConnectMessageAsync()
        {
            var connect = new ConnectPlayer
            {
                Team = Team
            };
            await Stream.WritePacketAsync(
                PacketConverter.Serialize(PacketType.Connect, connect));

            while (true)
            {

            }
        }

        private static async Task ReceiveMessageAsync()
        {
            while (true)
            {
                try
                {
                    var recievedPacket = await Stream.ReadPacketAsync();
                    ProccessIncomingPacket(recievedPacket);
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }

        private static async Task ProccessIncomingPacket(Packet packet)
        {
            var packetType = PacketTypeManager.GetTypeFromPacket(packet);
            switch (packetType)
            {
                case PacketType.CantConnect:
                    OnCantConnect();
                    break;
                case PacketType.Connect:
                    var connect = PacketConverter.Deserialize<ConnectPlayer>(packet);
                    OnConnect(new Player { Id = connect.Id, TeamName = connect.Team });
                    break;
                case PacketType.PlayersList:
                    var players = PacketConverter.Deserialize<PlayersList>(packet);
                    OnPlayersList(players.Players);
                    break;
            }
        }

        public static void Disconnect()
        {
            Stream?.Close();
            Client.Dispose();
        }
    }
}
