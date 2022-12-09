using Protocol.Packets;
using Protocol.Protocol;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Protocol
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            PacketTypeManager.RegisterType(PacketType.Handshake, 0, 0);
            PacketTypeManager.RegisterType(PacketType.TestPacket, 0, 1);

            var handshakePacket = PacketConverter.Serialize(
                PacketType.Handshake,
                new HandshakePacket
                {
                    MagicHandshakeNumber = 42
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

                var reader = new StreamReader(client.GetStream());
                var writer = (client.GetStream());
                if (reader is null || writer is null) return;

                var packet = handshakePacket.ToPacket();
                await writer.WriteAsync(packet, 0, packet.Length);
                await writer.FlushAsync();

                var res = await reader.ReadLineAsync();

                //var bytes = new ArraySegment<byte>();
                //await reader.Socket.ReceiveAsync(bytes);
            }
        }
    }
}