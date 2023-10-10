// See https://aka.ms/new-console-template for more information
using Common;
using Subscriber;

Console.WriteLine("Subscriber!");

string topic;

Console.Write("Enter the topic: ");

topic = Console.ReadLine().ToLower();


var subscriberSocket = new SubscriberSocket(topic);

subscriberSocket.Connect(Settings.BROKER_IP,Settings.BROKER_PORT);

Console.WriteLine("Press any key to exit...");
Console.ReadLine();