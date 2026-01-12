using Core.Extensions;
using Core.Services;
using Core.Structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Core.Handlers;

public abstract class Server : UdpClient
{
    // Properties //
    /// <summary>
    /// The network port this <see cref="UdpClient"/> uses.
    /// </summary>
    public required int Port = NetworkService.GetAvailablePort();
    /// <summary>
    /// A list of all of the clients this <see cref="Server"/> holds.
    /// </summary>
    public readonly SortedSet<IPEndPoint> Clients = [];
    // Settings //
    /// <summary>
    /// Gets or sets the maximum number of clients that can be connected simultaneously.
    /// </summary>
    /// <remarks>
    /// A value of 0 allows infinite.
    /// </remarks>
    public int MaxClients = 0;
    /// <summary>
    /// A boolean representing whether connections are currently being accepted.
    /// </summary>
    public bool AcceptingConnections = true;
    /// <summary>
    /// Contains the endpoint of which the <see cref="Server"/> receives data from.
    /// </summary>
    public required IPEndPoint RemoteEndPoint;
    // Events //
    /// <summary>
    /// Occurs when data is received from the network.
    /// </summary>
    /// <remarks>The event provides a <see cref="NetDataReader"/> instance containing the received data.
    /// Subscribers can use this event to process incoming network messages as they arrive.</remarks>
    public event EventHandler<NetDataReader>? NetworkReceived;
    /// <summary>
    /// Occurs when a new network connection is accepted and data is available for processing.
    /// </summary>
    /// <remarks>The event provides a <see cref="NetDataReader"/> instance containing the received data for
    /// the accepted connection. Subscribers can use this event to handle incoming connections and process their data as
    /// needed. This event is raised by the server when a client successfully connects.</remarks>
    public event EventHandler<NetDataReader>? ConnectionAccepted;
    // Indexers //
    public IPEndPoint? this[EndPoint endPoint] => Clients
        .FirstOrDefault(client => client == endPoint);
    public IPEndPoint? this[IPEndPoint ipEndPoint] => Clients
        .FirstOrDefault(client => client == ipEndPoint);
    // Constructors //
    [SetsRequiredMembers]
    public Server(IPEndPoint RemoteEndPoint) : base(RemoteEndPoint)
    {
        // Settings //
        this.RemoteEndPoint = RemoteEndPoint;
        this.Port = RemoteEndPoint.Port;
        // Connections //
        NetworkReceived += OnNetworkReceived;
    }
    [SetsRequiredMembers]
    public Server(IPAddress address, int Port) : this(new IPEndPoint(address, Port)) {}
    // Listen Methods //
    public async Task ListenAsync()
    {
        while (true)
        {
            Console.WriteLine("Awaiting Data from Client");
            // Await Receive of Data //
            UdpReceiveResult result = await ReceiveAsync();
            // Sender //
            IPEndPoint? peer = Clients.FirstOrDefault(peer => peer.Equals(result.RemoteEndPoint));
            // Conditions //
            if (peer is null
                && AcceptingConnections
                && (MaxClients <= 0 || Clients.Count < MaxClients))
                // If New Connection //
                OnConnectionReceived(result.RemoteEndPoint, new NetDataReader(result.Buffer));

            // If Existing Connection //
            else NetworkReceived?.Invoke(peer, new NetDataReader(result.Buffer));
        }
    }
    // Public Methods //
    public void Send(in byte[] bytes, IPEndPoint client) => base.Send(bytes, client);
    public void Send(in byte[] bytes, params IEnumerable<IPEndPoint> clients)
    {
        foreach (IPEndPoint client in clients)
            Send(bytes, client);
    }
    public void Send(in byte[] bytes) => Send(bytes, Clients);
    // Event Methods //
    private void OnConnectionReceived(IPEndPoint endPoint, in NetDataReader reader)
    {
        // Get Type //
        ConnectionRequest.ConnectionType type = reader.ReadConnectionType();
        ConnectionRequest request = new(this, endPoint, type);
        // Fire //
        OnConnectionReceived(endPoint, request, reader);
    }
    protected abstract void OnConnectionReceived(IPEndPoint client, ConnectionRequest request, in NetDataReader reader);

    private void OnNetworkReceived(object? _endPoint, NetDataReader reader)
    {
        // Conversions //
        IPEndPoint? endPoint = (IPEndPoint?)_endPoint;
        // Conditions //
        if (endPoint is null)
            return;
        // Pass on //
        OnNetworkReceived(endPoint, reader);
    }
    protected abstract void OnNetworkReceived(in IPEndPoint endPoint, NetDataReader reader);
}

[method: SetsRequiredMembers]
public class ConnectionRequest(Server Parent, IPEndPoint endPoint, ConnectionRequest.ConnectionType Type)
{
    // Data //
    public required Server Parent = Parent;
    public required IPEndPoint Client = endPoint;
    public required ConnectionType Type = Type;
    public bool Accepted = false;
    // Enumerations //
    public enum ConnectionType : byte
    {
        Admin,
        SignUp,
        LogIn,
    }
    // Methods //
    public void Accept()
    {
        // Conditions //
        if (Accepted)
            return;
        // Add to Parent's Clients //
        Parent.Clients.Add(Client);
        // Acknowledge //
        Accepted = true;
        // Debug //
        //Console.WriteLine("Accepted!");
    }
}