using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    public class Packet
    {
        public byte PacketType { get; private set; }
        public byte PacketSubtype { get; private set; }
        public List<PacketField> Fields { get; set; } = new List<PacketField>();

        private Packet() { }

        public static Packet Create(byte type, byte subType)
        {
            return new Packet
            {
                PacketType = type,
                PacketSubtype = subType
            };
        }

        public byte[] ToPacket()
        {
            var packet = new MemoryStream();
            packet.Write(
                new byte[] { 0xAF, 0xAA, 0xAF, PacketType, PacketSubtype }, 0, 5);

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
            if (packet.Length < 7)
                return null;

            if (packet[0] != 0xAF ||
                packet[1] != 0xAA ||
                packet[2] != 0xAF)
                return null;

            if (packet[^2] != 0xFF ||
                packet[^1] != 0x00)
                return null;

            var packetType = packet[3];
            var packetSubType = packet[4];

            var myPacket = Create(packetType, packetSubType);

            var fields = packet.Skip(5).ToArray();

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
            var rawSize = Marshal.SizeOf(value);
            var rawData = new byte[rawSize];

            var handle = GCHandle.Alloc(rawData,
                GCHandleType.Pinned);

            Marshal.StructureToPtr(value,
                handle.AddrOfPinnedObject(),
                false);

            handle.Free();

            return rawData;
        }

        private T ByteArrayToFixedObject<T>(byte[] bytes)
            where T : struct
        {
            T structure;

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                structure = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }

            return structure;
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
            where T : struct
        {
            var field = GetField(id);

            if (field is null)
                throw new Exception($"Field with ID {id} wasn't found.");

            var neededSize = Marshal.SizeOf(typeof(T));

            if (neededSize != field.FieldSize)
                throw new Exception($"Can't convert field to type {typeof(T).FullName}.\n" + $"We have {field.FieldSize} bytes but we need exactly {neededSize}.");

            return ByteArrayToFixedObject<T>(field.Contents);
        }

        public void SetValue(byte id, object structure)
        {
            if (!structure.GetType().IsValueType)
                throw new Exception("Only value types are available.");

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
