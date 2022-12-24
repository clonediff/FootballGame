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

        private static TcpClient Client { get; set; } = new();

        private static NetworkStream Stream { get; set; }

        public static event Action<string> OnCantConnect;
        public static event Action<Player> OnConnect;
        public static event Action<PlayerIsReadyStruct[]> OnPlayersList;
        public static event Action<string> OnPlayerDisconnect;
        public static event Action<string, bool> OnReadyStateChanged;
        public static event Action<bool> OnGameReady;
        public static event Action<Player, Player> OnGameStart;

        public static string Id { get; set; }

        public static async Task ConnectAndRunAsync(string team)
        {
            Client.Connect(HOST, 8888);
            Stream = Client.GetStream();
            // запускаем ввод сообщений
            Task.Run(ReceiveMessageAsync);

            var connect = new ConnectPlayer { Team = team };
            await SendPacketAsync(PacketType.Connect, connect);
        }

        public static async Task SendPacketAsync(PacketType type, object packet)
        {
            await Stream.WritePacketAsync(PacketConverter.Serialize(type, packet));
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
                    Disconnect();
                    break;
                }
            }
        }

        private static async Task ProccessIncomingPacket(Packet packet)
        {
            var packetType = PacketTypeManager.GetTypeFromPacket(packet);
            switch (packetType)
            {
                case PacketType.SendId:
                    var id = PacketConverter.Deserialize<SendId>(packet);
                    Id = id.Id;
                    break;
                case PacketType.CantConnect:
                    var cantConnect = PacketConverter.Deserialize<CantConnect>(packet);
                    OnCantConnect(cantConnect.Id);
                    break;
                case PacketType.Connect:
                    var connect = PacketConverter.Deserialize<ConnectPlayer>(packet);
                    OnConnect(new Player { Id = connect.Id, TeamName = connect.Team });
                    break;
                case PacketType.PlayersList:
                    var players = PacketConverter.Deserialize<PlayersList>(packet);
                    OnPlayersList(players.Players);
                    break;
                case PacketType.Disconnect:
                    var disconnected = PacketConverter.Deserialize<DisconnectPlayer>(packet);
                    OnPlayerDisconnect(disconnected.Id);
                    break;
                case PacketType.ReadyState:
                    var state = PacketConverter.Deserialize<PlayerReadyState>(packet);
                    OnReadyStateChanged(state.Id, state.IsReady);
                    break;
                case PacketType.GameReady:
                    var gameReady = PacketConverter.Deserialize<GameReady>(packet);
                    OnGameReady(gameReady.IsReady);
                    break;
                case PacketType.StartGame:
                    var gameStart = PacketConverter.Deserialize<GameStart>(packet);
                    OnGameStart(gameStart.Player1, gameStart.Player2);
                    break;
            }
        }

        public static void Disconnect()
        {
            Stream?.Close();
            Client.Dispose();
            Client = new();
        }
    }
}
