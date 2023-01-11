using ENet;
using System;
using System.Text;
using System.Threading.Channels;

public class Client
{
    public Client()
    {
        Library.Initialize();
        using (Host client = new Host())
        {
            Console.WriteLine("Client connecting...");
            Address address = new Address();

            address.SetHost("127.0.0.1");
            address.Port = 13000;
            client.Create();

            Peer peer = client.Connect(address);


            Event netEvent;

            while (!Console.KeyAvailable)
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
                            Console.WriteLine("Client connected to server");

                            Packet packet = default(Packet);
                            byte[] data = Encoding.ASCII.GetBytes("Hello world");

                            packet.Create(data, PacketFlags.Reliable);
                            peer.Send(0, ref packet);
                            break;

                        case EventType.Disconnect:
                            Console.WriteLine("Client disconnected from server");
                            break;

                        case EventType.Timeout:
                            Console.WriteLine("Client connection timeout");
                            break;

                        case EventType.Receive:
                            Console.WriteLine("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);

                            byte[] buffer = new byte[64];
                            netEvent.Packet.CopyTo(buffer);
                            Console.WriteLine($"{Encoding.ASCII.GetString(buffer)}");

                            netEvent.Packet.Dispose();
                            break;
                    }
                }
            }

            client.Flush();
        }
        Library.Deinitialize();
    }
}