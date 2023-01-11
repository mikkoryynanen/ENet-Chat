using System;
using System.Threading;

namespace client
{
    class Program
    {
        static void Main()
        {
            ThreadPool.QueueUserWorkItem(HandleClientConnection);

            while (true)
            {
                string line = Console.ReadLine();

                ClearLine();

                ChatClient.SendMessage(line);
            }
        }

        private static void HandleClientConnection(object state)
        {
            ChatClient client = new ChatClient();
        }

        static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}

