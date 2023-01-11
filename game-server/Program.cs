using System;
using System.Net;
using System.Threading;
using ENet;

internal class Program
{
    private static void Main(string[] args)
    {
        Thread serverThread = new Thread(() =>
        {
            Server server = new Server();
        });
        serverThread.Start();

    }
}