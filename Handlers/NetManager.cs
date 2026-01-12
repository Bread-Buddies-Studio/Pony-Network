using Core.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

namespace Core.Handlers;

/// <summary>
/// Exists solely as an example. Highly recommend making your own descendant of the <see cref="Server"/> class
/// </summary>
public class NetManager : Server
{
    #region Constructors
    [SetsRequiredMembers]
    public NetManager(IPEndPoint RemoteEndPoint) : base(RemoteEndPoint)
    {
        // Begin //
        Task.Run(ListenAsync);
    }
    [SetsRequiredMembers]
    public NetManager(IPAddress address, int Port) : this(new IPEndPoint(address, Port)) {}
    #endregion
    #region Event Methods
    protected override void OnConnectionReceived(IPEndPoint client, ConnectionRequest request, in NetDataReader reader)
    {
        // Debug //
        Console.WriteLine("Connection Received!");
        // Accept Request //
        request.Accept();
    }
    protected override void OnNetworkReceived(in IPEndPoint endPoint, NetDataReader reader)
    {
        // Conditions //
        if (endPoint is null)
            return;
        // Debug //
        Console.WriteLine($"Received: {reader.ReadString()}");
    }
    #endregion
}
