using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FootballLogicLib;
using Protocol.Protocol;
using Protocol;
using Protocol.Packets;

namespace Server
{
    internal class ServerObject
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, Constants.PORT);
        Dictionary<string, ClientObject> clients = new();

        Dictionary<ClientObject, Game> games = new();

        protected internal void RemoveConnection(string id)
        {
            if (clients.TryGetValue(id, out var client))
                clients.Remove(id);
            client?.Close();
        }

        protected internal async Task ListenAsync()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    ClientObject clientObject = new ClientObject(tcpClient, this);

                    if (clients.Count >= 2)
                    {
                        var cantConnect = new CantConnect
                        {
                            Id = clientObject.Id
                        };
                        await clientObject.Stream.WritePacketAsync(
                            PacketConverter.Serialize(PacketType.CantConnect, cantConnect));
                        continue;
                    }

                    clients.Add(clientObject.Id, clientObject);
                    Task.Run(clientObject.ProcessAsync);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal async Task BroadcastPacketAsync(Packet packet)
        {
            foreach (var (clientId, client) in clients)
            {
                await client.Stream.WritePacketAsync(packet);
            }
        }

        protected internal async Task SendPlayersList(string id)
        {
            if (clients.TryGetValue(id, out var reciever))
            {
                var players = clients
                    .Where(x => x.Key != id)
                    .Select(keyValue => keyValue.Value.Player)
                    .ToArray();
                await reciever.Stream.WritePacketAsync(
                    PacketConverter.Serialize(PacketType.PlayersList, 
                        new PlayersList
                        {
                            Players = players
                        }));
            }
            else
                throw new ArgumentException($"Client {id} isn't exists");
        }
        
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            foreach (var (id, client) in clients)
            {
                client.Close(); //отключение клиента
            }
            tcpListener.Stop(); //остановка сервера
        }
    }
}
