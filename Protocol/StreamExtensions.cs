using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public static class StreamExtensions
    {
        public static async Task<Packet> ReadPacketAsync(this Stream stream)
        {
            var buffer = new byte[4096];
            var bytesCount = await stream.ReadAsync(buffer);
            var asd = buffer[0..bytesCount];
            var packet = Packet.Parse(asd);
            return packet;
            //var result = new List<byte>();
            //byte prevByte = 0;
            //int curByte = stream.ReadByte();
            //while (curByte != -1 && (prevByte != 0xFF || curByte != 0x00))
            //{
            //    prevByte = (byte)curByte;
            //    result.Add(prevByte);
            //    curByte = stream.ReadByte();
            //}
            //result.Add((byte)curByte);
            //return Packet.Parse(result.ToArray());
        }

        public static async Task WritePacketAsync(this Stream stream, Packet packet)
        {
            var bytes = packet.ToPacket();
            await stream.WriteAsync(bytes, 0, bytes.Length);
            await stream.FlushAsync();
        }
    }
}
