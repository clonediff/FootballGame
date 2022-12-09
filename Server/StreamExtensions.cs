using Protocol.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class StreamExtensions
    {
        public static async Task<Packet> ReadPacketAsync(this NetworkStream stream)
        {
            var result = new List<byte>();
            int curByte = stream.ReadByte();
            while (curByte != -1)
            {
                result.Add((byte)curByte);
                curByte = stream.ReadByte();
            }
            return Packet.Parse(result.ToArray());
        }
    }
}
