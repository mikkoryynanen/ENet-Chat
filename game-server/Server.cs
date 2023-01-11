using System;
using System.Text;
using System.Threading;
using ENet;
using game_server.Packets;
using game_server.Payloads;
using Shared;
using Shared.LogicLayer;


public class Server
{
    public Server()
    {
        Library.Initialize();
        using (Host server = new Host())
        {
            Address address = new Address();

            address.Port = 13000;
            server.Create(address, 20);

            Console.WriteLine($"Server running at {address.GetIP()}:{address.Port}");

            Event netEvent;

            PacketHandler packetHandler = new PacketHandler(new IPacketHandler[] { new MessagePacket() });

            while (true)
            {
                bool polled = false;

                while (!polled)
                {
                    if (server.CheckEvents(out netEvent) <= 0)
                    {
                        if (server.Service(15, out netEvent) <= 0)
                            break;

                        polled = true;
                    }

                    switch (netEvent.Type)
                    {
                        case EventType.None:
                            break;

                        case EventType.Connect:
                            Console.WriteLine("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                            break;

                        case EventType.Disconnect:
                            Console.WriteLine("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                            break;

                        case EventType.Timeout:
                            Console.WriteLine("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                            break;

                        case EventType.Receive:
                            // Console.WriteLine("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " +
                            //                   netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID +
                            //                   ", Data length: " + netEvent.Packet.Length);

                            byte[] buffer = new byte[netEvent.Packet.Length];
                            netEvent.Packet.CopyTo(buffer);

                            ThreadPool.QueueUserWorkItem(ThreadProcess, new ThreadMessagePayload { MessagePayload = new MessagePayload { Buffer = buffer, Host = server}, PacketHandler = packetHandler});

                            netEvent.Packet.Dispose();
                            break;
                    }
                }
            }

            server.Flush();
        }
        Library.Deinitialize();
    }

    private void ThreadProcess(object state)
    {
        ThreadMessagePayload threadPayload = (ThreadMessagePayload)state;
        threadPayload.PacketHandler.Handle(new MessagePacket { Payload = threadPayload.MessagePayload });
    }
}

