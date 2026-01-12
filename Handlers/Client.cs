using Core.Services;
using Core.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Core.Handlers;
/// <summary>
/// Represents a <see cref="UdpClient"/> to connect to a <see cref="Server"/>
/// </summary>
public class Client : UdpClient, IComparable<Client>
{
    // Constructor //
    [SetsRequiredMembers]
    public Client(string hostname, int Port) : base(hostname, Port)
    {
        this.Port = Port;
        // Run Detection //
        Task.Run(ListenAsync);
    }
    [SetsRequiredMembers]
    public Client(IPEndPoint localEndPoint) : base(localEndPoint) 
    {
        Port = NetworkService.GetAvailablePort();
    }
    // Properties //
    /// <summary>
    /// Gets the network port number used in the connection.
    /// </summary>
    /// <remarks>An integer between 0 and 65,535 </remarks>
    public required int Port { get; init; }
    /// <summary>
    /// Returns a boolean representing whether the <see cref="Client"/> is currently connected.
    /// </summary>
    public bool IsConnected => Active;
    /// <summary>
    /// Gets the remote network endpoint to which the client is connected.
    /// </summary>
    public EndPoint? RemoteEndPoint => Client.RemoteEndPoint;
    // Conversions //
    public static implicit operator IPEndPoint?(Client client) => (IPEndPoint?)client?.RemoteEndPoint;
    // Events //
    /// <summary>
    /// Occurs when data is received from the network.
    /// </summary>
    /// <remarks>The event provides a <see cref="NetDataReader"/> instance containing the received data.
    /// Subscribers can use this event to process incoming packets as they arrive.</remarks>
    public event EventHandler<NetDataReader>? OnNetworkReceive;
    // Private Methods //
    private async Task ListenAsync()
    {
        while (true)
        {
            // Wait for Receive //
            UdpReceiveResult result = await ReceiveAsync();
            // Create Reader //
            NetDataReader reader = new(result.Buffer);
            // Tell //
            OnNetworkReceive?.Invoke(this, reader);
        }
    }
    // Overloads //
    /// <inheritdoc cref="UdpClient.SendAsync(byte[], int, IPEndPoint?)"/>
    /// <param name="server">The receiver of the <paramref name="datagram"/></param>
    public async Task<int> SendAsync(byte[] datagram, Server server) => await SendAsync(datagram, datagram.Length, server.RemoteEndPoint);
    // Overrides //
    public int CompareTo(Client? other)
    {
        // Conditions //
        if (other is null)
            return 1;
        if (other.RemoteEndPoint is null)
            return 1;
        if (RemoteEndPoint is null)
            return -1;
        // 
        return RemoteEndPoint.AddressFamily.CompareTo(other.RemoteEndPoint.AddressFamily);
    }
}
