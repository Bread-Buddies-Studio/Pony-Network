using System.Net;
using System.Net.NetworkInformation;
using PonyNetwork.Extensions;

namespace PonyNetwork.Services;

/// <summary>
/// Contains all the necessary static network methods.
/// </summary>
public static class NetworkService
{
    // Properties //
    /// <summary>
    /// Represents the range of valid TCP/UDP ports.
    /// </summary>
    public static readonly Range ValidPorts = new(49_152, 65_535);
    // Exceptions //
    /// <summary>
    /// The exception that is thrown when no valid network ports are available for use.
    /// </summary>
    private class NoAvailablePortsException() : Exception("No Valid Ports Found!");
    // Base Methods //
    /// <summary>
    /// Gets whether a certain port is in use.
    /// </summary>
    /// <param name="port">The port to check.</param>
    /// <param name="listeners">A span of listeners to check.</param>
    /// <returns>Whether the port is available.</returns>
    public static bool IsPortAvailable(int port, in ReadOnlySpan<IPEndPoint> listeners)
    {
        // Is Port Used? //
        foreach (IPEndPoint listener in listeners)
        {
            // Conditions //
            if (listener.Port != port)
                continue;
            // Same Port so Fail //
            return false;
        }
        // Success //
        return true;
    }
    /// <summary>
    /// Gets the first available port between 49,152 and 65,535.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NoAvailablePortsException"></exception>
    public static int GetAvailablePort()
    {
        // Network Properties //
        var globalProperties = IPGlobalProperties.GetIPGlobalProperties();
        // Get Listeners //
        ReadOnlySpan<IPEndPoint> listeners = globalProperties.GetActiveUdpListeners().AsSpan();
        // Ports //
        foreach (int port in ValidPorts)
        {
            // Is Port Used? //
            if (!IsPortAvailable(port, listeners))
                continue;
            // Success //
            return port;
        }
        // Fail; No Valid Ports; Should Be Impossible. //
        throw new NoAvailablePortsException();
    }
}
