﻿using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Broker
{
     class BrokerSocket
     {
          private Socket _socket;
          private const int CONNECTIONS_LIMIT = 8;

          public BrokerSocket()
          {
               _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          }


          public void Start(string ip, int port)
          {
               _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
               _socket.Listen(CONNECTIONS_LIMIT);
               Accept();
          }

          private void Accept()
          {
               _socket.BeginAccept(AcceptedCallBack, null);
          }

          private void AcceptedCallBack(IAsyncResult asyncResult)
          {
               ConnectionInfo connection = new ConnectionInfo();

               try
               {
                    connection.Socket = _socket.EndAccept(asyncResult);
                    connection.Address = connection.Socket.RemoteEndPoint.ToString();
                    connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length,
                         SocketFlags.None, ReceiveCallBack, connection);
               }
               catch (Exception ex)
               {

                    Console.WriteLine($"Can't accept: {ex.Message}");
               }
               finally
               {
                    Accept();
               }
          }

          private void ReceiveCallBack(IAsyncResult asyncResult)
          {
               ConnectionInfo connection = asyncResult.AsyncState as ConnectionInfo;

               try
               {
                    Socket senderSocket = connection.Socket;
                    SocketError response;
                    int buffSize = senderSocket.EndReceive(asyncResult, out response);

                    if (response == SocketError.Success)
                    {
                         byte[] payload = new byte[buffSize];
                         Array.Copy(connection.Data, payload, payload.Length);

                         PayloadHandler.Handle(payload, connection);


                    }

               }
               catch (Exception ex)
               {

                    Console.WriteLine($"Can't receive data {ex.Message}");
               }
               finally
               {
                    try
                    {
                         connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length, SocketFlags.None,
                              ReceiveCallBack, connection);
                    }
                    catch (Exception ex)
                    {
                         Console.WriteLine($"{ex.Message}");
                         var address = connection.Socket.RemoteEndPoint.ToString();
                         ConnectionsStorage.Remove(address);
                         connection.Socket.Close();
                    }
               }
          }
     }
}
