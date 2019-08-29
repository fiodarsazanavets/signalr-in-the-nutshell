﻿using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please specify the URL of SignalR Hub with WS/WSS protocol");

            var url = Console.ReadLine();

            try
            {
                var ws = new ClientWebSocket();

                ws.ConnectAsync(new Uri(url), CancellationToken.None).Wait();

                var handshake = new List<byte>(Encoding.UTF8.GetBytes(@"{""protocol"":""json"", ""version"":1}"))
                {
                    0x1e
                };

                ws.SendAsync(new ArraySegment<byte>(handshake.ToArray()), WebSocketMessageType.Text, true, CancellationToken.None);

                Console.WriteLine("WebSockets connection established");
                ReceiveAsync(ws).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
        }

        private static async Task ReceiveAsync(ClientWebSocket ws)
        {
            var buffer = new byte[4096];

            try
            {
                while (true)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        break;
                    }
                    else
                    {
                        Console.WriteLine(Encoding.Default.GetString(Decode(buffer)));
                        buffer = new byte[4096];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
        }

        private static byte[] Decode(byte[] packet)
        {
            var i = packet.Length - 1;
            while (i >= 0 && packet[i] == 0)
            {
                --i;
            }

            var temp = new byte[i + 1];
            Array.Copy(packet, temp, i + 1);
            return temp;
        }
    }
}