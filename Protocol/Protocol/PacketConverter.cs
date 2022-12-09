using System.Reflection;

namespace Protocol.Protocol
{
    internal static class PacketConverter
    {
        private static List<(FieldInfo field, byte id)> GetField(Type t)
            => t.GetFields(BindingFlags.Instance |
                                    BindingFlags.NonPublic |
                                    BindingFlags.Public)
            .Where(x => x.GetCustomAttribute<FieldAttribute>() != null)
            .Select(x => (x, x.GetCustomAttribute<FieldAttribute>()!.FieldId))
            .ToList();

        public static Packet Serialize(PacketType packetType, object obj, bool strict = false)
        {
            (var type, var subType) = PacketTypeManager.GetType(packetType);
            return Serialize(type, subType, obj, strict);
        }

        public static Packet Serialize(byte type, byte subType, object obj, bool strict = false)
        {
            var fields = GetField(obj.GetType());

            if (strict)
            {
                var usedUp = new HashSet<byte>();

                foreach (var (field, id) in fields)
                {
                    if (usedUp.Contains(id))
                        throw new Exception("One field used two times.");

                    usedUp.Add(id);
                }
            }

            var packet = Packet.Create(type, subType);

            foreach (var (field, id) in fields)
                packet.SetValue(id, field.GetValue(obj)!);

            return packet;
        }

        public static T Deserialize<T>(Packet packet, bool strict = false)
        {
            var fields = GetField(typeof(T));

            var inst = Activator.CreateInstance<T>();

            if (fields.Count == 0)
                return inst;

            foreach (var (field, id) in fields)
            {
                if (!packet.HasField(id))
                {
                    if (strict)
                        throw new Exception($"Couldn't get field[{id}] for {field.Name}");

                    continue;
                }

                // packet.GetValue<field.FieldType>(id)
                var value = typeof(Packet)
                    .GetMethod("GetValue")?
                    .MakeGenericMethod(field.FieldType)
                    .Invoke(packet, new object[] { id });

                if (value is null)
                {
                    if (strict)
                        throw new Exception($"Couldn't get value for field[{id}] for {field.Name}");

                    continue;
                }

                // inst.field = value;
                field.SetValue(inst, value);
            }

            return inst;
        }
    }
}
