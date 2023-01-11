using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.LogicLayer
{
    public interface IPacket
    {
        Type PacketType { get; }
        object Payload { get; }
    }

    public interface IPacketHandler
    {
        Type PacketType { get; }
        void Handle(IPacket packet);
    }

    public class PacketHandler
    {
        private readonly Dictionary<Type, IPacketHandler> _packetHandlerLookup;

        public PacketHandler(IPacketHandler[] handlerReferences)
        {
            _packetHandlerLookup = handlerReferences
                .ToDictionary(h => h.PacketType);
        }

        public void Handle(IPacket packet)
        {
            if (_packetHandlerLookup.TryGetValue(packet.PacketType, out var handler))
            {
                handler.Handle(packet);
            }
            else
            {
                throw new Exception(
                    $"Unknown packet handler for {packet.PacketType}");
            }
        }
    }
}