using Protocol;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            PacketTypesRegistrator.RegisterTypes();
            var server = new ServerObject();
            await server.ListenAsync();
        }
    }
}