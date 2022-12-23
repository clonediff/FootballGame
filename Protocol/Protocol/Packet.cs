using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    public class Packet
    {
        public byte PacketType { get; private set; }
        public List<PacketField> Fields { get; set; } = new List<PacketField>();

        private Packet() { }

        public static Packet Create(byte type)
        {
            return new Packet
            {
                PacketType = type
            };
        }

        public byte[] ToPacket()
        {
            var packet = new MemoryStream();
            packet.Write(
                new byte[] { 0xAF, 0xAA, 0xAF, PacketType }, 0, 4);

            var fields = Fields.OrderBy(x => x.FieldID);

            foreach (var field in fields)
            {
                packet.Write(new byte[] { field.FieldID, field.FieldSize }, 0, 2);
                packet.Write(field.Contents, 0, field.Contents.Length);
            }

            packet.Write(new byte[] { 0xFF, 0x00 }, 0, 2);

            return packet.ToArray();
        }


        public static Packet Parse(byte[] packet)
        {
            if (packet.Length < 6)
                return null;

            if (packet[0] != 0xAF ||
                packet[1] != 0xAA ||
                packet[2] != 0xAF)
                return null;

            if (packet[^2] != 0xFF ||
                packet[^1] != 0x00)
                return null;

            var packetType = packet[3];

            var myPacket = Create(packetType);

            var fields = packet.Skip(4).ToArray();

            while (true)
            {
                if (fields.Length == 2)
                    return myPacket;

                var fieldId = fields[0];
                var fieldSize = fields[1];

                var contents = fieldSize != 0 ?
                    fields.Skip(2).Take(fieldSize).ToArray() :
                    null;

                myPacket.Fields.Add(
                    new PacketField
                    {
                        FieldID = fieldId,
                        FieldSize = fieldSize,
                        Contents = contents!
                    });

                fields = fields.Skip(2 + fieldSize).ToArray();
            }
        }
        public byte[] FixedObjectToByteArray(object value)
        {
            var json = JsonSerializer.Serialize(value);
            return Encoding.UTF8.GetBytes(json);
        }

        private T ByteArrayToFixedObject<T>(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json)!;
        }

        public PacketField GetField(byte id)
        {
            foreach (var field in Fields)
                if (field.FieldID == id)
                    return field;

            return null;
        }

        public bool HasField(byte id)
            => GetField(id) is not null;

        public T GetValue<T>(byte id)
        {
            var field = GetField(id);

            if (field is null)
                throw new Exception($"Field with ID {id} wasn't found.");

            return ByteArrayToFixedObject<T>(field.Contents);
        }

        public void SetValue(byte id, object structure)
        {
            var field = GetField(id);

            if (field == null)
            {
                field = new PacketField
                {
                    FieldID = id
                };

                Fields.Add(field);
            }

            var bytes = FixedObjectToByteArray(structure);

            if (bytes.Length > byte.MaxValue)
                throw new Exception("Object is too big. Max length is 255 bytes.");

            field.FieldSize = (byte)bytes.Length;
            field.Contents = bytes;
        }
    }
}
