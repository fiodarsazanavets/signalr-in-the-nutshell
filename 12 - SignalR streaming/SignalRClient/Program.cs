using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Please specify the URL of SignalR Hub");

            var url = Console.ReadLine();

            Console.WriteLine("Please specify transport type:");
            Console.WriteLine("0 - default");
            Console.WriteLine("1 - WebSockets");
            Console.WriteLine("2 - Server-sent events");
            Console.WriteLine("3 - Long polling");

            var transportTypeNumber = Console.ReadLine();

            HttpTransportType transportType;

            switch (transportTypeNumber)
            {
                case "0":
                    transportType = HttpTransportType.None;
                    break;
                case "1":
                    transportType = HttpTransportType.WebSockets;
                    break;
                case "2":
                    transportType = HttpTransportType.ServerSentEvents;
                    break;
                case "3":
                    transportType = HttpTransportType.LongPolling;
                    break;
                default:
                    Console.WriteLine("Invalid transport type specified");
                    Console.ReadKey();
                    return;
            }

            var hubConnection = transportType == HttpTransportType.None ?
                new HubConnectionBuilder()
                .WithUrl(url)
                .Build() :
                new HubConnectionBuilder()
                .WithUrl(url, transportType)
                .Build();

            hubConnection.On<string>("ReceiveMessage", message => ReceiveMessage(message));
            hubConnection.On<MessagePayload>("ReceiveObject", payload => ReceiveObject(payload));

            try
            {
                hubConnection.StartAsync().Wait();

                var running = true;

                while (running)
                {
                    var message = string.Empty;
                    var groupName = string.Empty;
                    var counter = 0;
                    var delayMilliseconds = 0;


                    Console.WriteLine("Please specify the action:");
                    Console.WriteLine("0 - broadcast to all");
                    Console.WriteLine("1 - send to itself");
                    Console.WriteLine("2 - send to others");
                    Console.WriteLine("3 - send to a group");
                    Console.WriteLine("4 - add user to a group");
                    Console.WriteLine("5 - remove user from a group");
                    Console.WriteLine("6 - stream data");
                    Console.WriteLine("exit - Exit the program");

                    var action = Console.ReadLine();

                    if (action == "0" || action == "1" || action == "2" || action == "3")
                    {
                        Console.WriteLine("Please specify the message:");
                        message = Console.ReadLine();
                    }

                    if (action == "3" || action == "4" || action == "5")
                    {
                        Console.WriteLine("Please specify the group name:");
                        groupName = Console.ReadLine();
                    }

                    if (action == "6")
                    {
                        Console.WriteLine("Please specify counter for streaming:");
                        counter = int.Parse(Console.ReadLine());

                        Console.WriteLine("Please specify delay in milliseconds between iterations:");
                        delayMilliseconds = int.Parse(Console.ReadLine());
                    }

                    switch (action)
                    {
                        case "0":
                            hubConnection.SendAsync("BroadcastMessage", message).Wait();
                            break;
                        case "1":
                            hubConnection.SendAsync("SendToCaller", message).Wait();
                            break;
                        case "2":
                            hubConnection.SendAsync("SendToOthers", message).Wait();
                            break;
                        case "3":
                            hubConnection.SendAsync("SendToGroup", groupName, message).Wait();
                            break;
                        case "4":
                            hubConnection.SendAsync("AddUserToGroup", groupName).Wait();
                            break;
                        case "5":
                            hubConnection.SendAsync("RemoveUserFromGroup", groupName).Wait();
                            break;
                        case "6":
                            await Stream(counter, delayMilliseconds);
                            break;
                        case "exit":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Invalid action specified");
                            break;
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

            void ReceiveMessage(string message)
            {
                Console.WriteLine($"SignalR Hub Message: {message}");
            }

            void ReceiveObject(MessagePayload payload)
            {
                Console.WriteLine(JsonConvert.SerializeObject(payload));
            }

            async Task Stream(int counter, int delayMilliseconds)
            {
                var channel = await hubConnection.StreamAsChannelAsync<int>("Counter", counter, delayMilliseconds, CancellationToken.None);

                while (await channel.WaitToReadAsync())
                {
                    while (channel.TryRead(out var count))
                    {
                        Console.WriteLine($"{count}");
                    }
                }

                Console.WriteLine("Streaming completed");
            }
        }
    }
}
