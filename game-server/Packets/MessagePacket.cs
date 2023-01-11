using System;
using ENet;
using game_server.Payloads;
using Shared;
using Shared.LogicLayer;

namespace game_server.Packets
{
    public class MessagePacket : IPacket, IPacketHandler
    {
        public Type PacketType => GetType();
        public object Payload { get; set;  }
        
        public void Handle(IPacket packet)
        {
            // Logic that correctly shows the message on the SERVER side

            MessagePayload messagePacket = (MessagePayload)packet.Payload;

            string message = Serializer.Deserialize(messagePacket.Buffer);
            Console.WriteLine($"Received message: {message}");
            
            Packet enetPacket = default(Packet);
            byte[] data = Serializer.Serialize(message);
            enetPacket.Create(data, PacketFlags.Reliable);
            
            messagePacket.Host.Broadcast(0, ref enetPacket);
        }

    }
}