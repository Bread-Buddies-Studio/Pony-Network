using Core.Handlers;
using Core.Structs;
using System;
using System.Collections.Generic;
using System.Text;
using static Core.Handlers.ConnectionRequest;

namespace Core.Extensions;

public static class NetDataReaderExtensions
{
    extension(NetDataReader source)
    {
        /// <summary>
        /// Peeks the first byte in the <see cref="NetDataReader"/>'s data.
        /// </summary>
        /// <remarks>
        /// Doesn't advance the position of the <see cref="NetDataReader"/>.
        /// </remarks>
        /// <returns>The connection type.</returns>
        public ConnectionType PeekConnectionType() => (ConnectionType)source.Peek<byte>(0);

        /// <summary>
        /// Reads the first byte in the <see cref="NetDataReader"/>'s data.
        /// </summary>
        /// <returns>The connection type.</returns>
        public ConnectionType ReadConnectionType() => (ConnectionType)source.Read<byte>(0);
    }
}
