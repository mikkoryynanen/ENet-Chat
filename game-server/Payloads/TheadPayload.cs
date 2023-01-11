using Shared.LogicLayer;

namespace game_server.Payloads
{
    public struct ThreadMessagePayload
    {
        public PacketHandler PacketHandler;
        public MessagePayload MessagePayload;
    }
}