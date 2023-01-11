using ENet;
using System;
using System.Collections.Concurrent;
using Shared;
using Shared.LogicLayer;

public class ChatClient
{
    static ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

    readonly Peer peer;
    
    public ChatClient()
    {
        Library.Initialize();
        using (Host client = new Host())
        {
            Console.WriteLine("Client connecting...");
            Address address = new Address();

            address.SetHost("127.0.0.1");
            address.Port = 13000;
            client.Create();

            peer = client.Connect(address);
            Event netEvent;

            while (true)
            {
                bool polled = false;

                while (!polled)
                {
                    if (client.CheckEvents(out netEvent) <= 0)
                    {
                        if (client.Service(15, out netEvent) <= 0)
                            break;

                        polled = true;
                    }

                    switch (netEvent.Type)
                    {
                        case EventType.None:
                            break;

                        case EventType.Connect:
                            Console.WriteLine("Client connected to server\nPlease enter message: ");
                            break;

                        case EventType.Disconnect:
                            Console.WriteLine("Client disconnected from server");
                            break;

                        case EventType.Timeout:
                            Console.WriteLine("Client connection timeout");
                            break;

                        case EventType.Receive:
                            //Console.WriteLine("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);

                            byte[] buffer = new byte[netEvent.Packet.Length];
                            netEvent.Packet.CopyTo(buffer);
                            Console.WriteLine(Serializer.Deserialize(buffer));

                            netEvent.Packet.Dispose();
                            break;
                    }
                }

                // TODO This could be on it's own thread, so it would not block enet things
                while(!_messageQueue.IsEmpty)
                {
                    if(_messageQueue.TryDequeue(out string message))
                    {
                        Packet packet = default(Packet);
                        byte[] data = Serializer.Serialize(message);

                        packet.Create(data, PacketFlags.Reliable);
                        peer.Send(0, ref packet);
                    }
                }
            }

            client.Flush();
        }
        Library.Deinitialize();
    }

    public static void SendMessage(string message)
    {
        //Packet packet = default(Packet);
        //byte[] data = Encoding.ASCII.GetBytes(message);

        //packet.Create(data, PacketFlags.Reliable);
        //peer.Send(0, ref packet);

        _messageQueue.Enqueue(message);
    }
}