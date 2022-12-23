using Protocol.Packets;
using Protocol.Protocol;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Protocol
{
    internal class Program
    {
        static int MagicHandShake;

        static async Task Main(string[] args)
        {
            var rnd = new Random();

            MagicHandShake = rnd.Next();

            //var handshakePacket = PacketConverter.Serialize(
            //    PacketType.Handshake,
            //    new HandshakePacket
            //    {
            //        MagicHandshakeNumber = MagicHandShake
            //    });

            //var testPacket = PacketConverter.Serialize(
            //    PacketType.TestPacket,
            //    new TestPacket
            //    {
            //        TestNumber = 54,
            //        TestDouble = 4.56,
            //        TestBoolean = true
            //    });

            var connect = PacketConverter.Serialize(PacketType.Connect,
                new ConnectPlayer
                {
                    Id = Guid.NewGuid().ToString(),
                    Team = "JAP"
                });

            using (var client = new TcpClient())
            {
                client.Connect("localhost", 8888);


                var stream = client.GetStream();
                if (stream is null) return;

                await stream.WritePacketAsync(connect);

                var recievedPacket = await stream.ReadPacketAsync();
                ProcessIncomingPacket(recievedPacket);

                //var bytes = new ArraySegment<byte>();
                //await reader.Socket.ReceiveAsync(bytes);
            }
        }

        private static void ProcessIncomingPacket(Packet packet)
        {
            var type = PacketTypeManager.GetTypeFromPacket(packet);

            switch (type)
            {
                case PacketType.Connect:
                    ProcessHandshake(packet);
                    break;
            }
        }

        private static void ProcessHandshake(Packet packet)
        {
            var connect = PacketConverter.Deserialize<ConnectPlayer>(packet);
            Console.WriteLine(connect.Id + " " + connect.Team);
        }
    }
}