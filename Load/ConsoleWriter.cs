﻿using System.Collections.Concurrent;

namespace Load;
internal static class ConsoleWriter
{
    private static readonly ConcurrentQueue<string> _messages = new();
    private static readonly System.Timers.Timer _timer = new()
    {
        Interval = 100,
        AutoReset = true,
        Enabled = true,
    };

    static ConsoleWriter() 
    {
        _timer.Elapsed += (sender, args) =>
        {
            string? message;
            while (_messages.TryDequeue(out message))
            {
                if (message != null)
                {
                    Console.WriteLine(message);
                }
            }
        };
    }

    public static void WriteLine(string message)
    {
        _messages.Enqueue(message);
    }
}