using Core.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

namespace Core.Handlers;

public class NetManager : Server
{
    [SetsRequiredMembers]
    public NetManager(IPEndPoint RemoteEndPoint) : base(RemoteEndPoint)
    {
        // Begin //
        Task.Run(ListenAsync);
    }
    [SetsRequiredMembers]
    public NetManager(IPAddress address, int Port) : this(new IPEndPoint(address, Port)) {}

    protected override void OnConnectionReceived(IPEndPoint client, ConnectionRequest request, in NetDataReader reader)
    {
        Console.WriteLine("Connection Received!");
        request.Accept();
    }
    protected override void OnNetworkReceived(in IPEndPoint endPoint, NetDataReader reader)
    {
        // Conditions //
        if (endPoint is null)
            return;
        Console.WriteLine($"Received: {reader.ReadString()}");
    }
}
