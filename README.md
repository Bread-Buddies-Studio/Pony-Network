Pony Network.

Inspired by LiteNetLib: https://github.com/RevenantX/LiteNetLib

A wrapper for UDP networking based off of LiteNetLib.

Short Example:
```csharp
// Peers //
NetManager server = new(IPAddress.Loopback, NetworkService.GetAvailablePort());
Client client = new(new IPEndPoint(IPAddress.Any, 0));

// Data //
NetDataWriter writer = new();
string message = "Hello, World!";

// Prepare Connection Request //
writer.Allocate<byte>(1);
writer.Append(ConnectionRequest.ConnectionType.Admin);

// Connection Request //
await client.SendAsync(writer.Data, server.RemoteEndPoint);

writer.Clear(); // Clear for Reuse.
writer.Allocate<char>(message.Length + 1); // Allocate Memory
writer.Append(message); // Append Message
// Send //
while (true)
{
    // Debug Message //
    Console.WriteLine("Sent!");

    // Wait for Client to Receive //
    await client.SendAsync(writer.Data, server.RemoteEndPoint);
    
    // Wait for resend. //
    await Task.Delay(500);
}
```