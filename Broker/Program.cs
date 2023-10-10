// See https://aka.ms/new-console-template for more information
using Broker;
using Common;

Console.WriteLine("Broker");

BrokerSocket socket = new BrokerSocket();
socket.Start(Settings.BROKER_IP, Settings.BROKER_PORT);

var worker = new Worker();
Task.Factory.StartNew(worker.DoSendMessageWork, TaskCreationOptions.LongRunning);


Console.ReadLine();
