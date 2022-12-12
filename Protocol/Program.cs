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

            PacketTypesRegistrator.RegisterTypes();

            MagicHandShake = rnd.Next();

            var handshakePacket = PacketConverter.Serialize(
                PacketType.Handshake,
                new HandshakePacket
                {
                    MagicHandshakeNumber = MagicHandShake
                });

            var testPacket = PacketConverter.Serialize(
                PacketType.TestPacket,
                new TestPacket
                {
                    TestNumber = 54,
                    TestDouble = 4.56,
                    TestBoolean = true
                });

            using (var client = new TcpClient())
            {
                client.Connect("localhost", 8888);


                var stream = client.GetStream();
                if (stream is null) return;

                await stream.WritePacketAsync(handshakePacket);

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
                case PacketType.Handshake:
                    ProcessHandshake(packet);
                    break;
            }
        }

        private static void ProcessHandshake(Packet packet)
        {
            var handshake = PacketConverter.Deserialize<HandshakePacket>(packet);
            if (MagicHandShake - handshake.MagicHandshakeNumber == 15)
                Console.WriteLine("Handshake successful!");
        }
    }
}