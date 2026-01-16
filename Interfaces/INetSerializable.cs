using PonyNetwork.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PonyNetwork.Interfaces;

/// <summary>
/// An interface representing an object which can be serialized into a <see cref="NetDataWriter"/>
/// and deserialized from a <see cref="NetDataReader"/>
/// </summary>
/// <typeparam name="T">The class in which to be serialized/deserialized</typeparam>
public interface INetSerializable<T>
{
    /// <summary>
    /// Converts <typeparamref name="T"/> into an array of bytes in order to be stored in a <see cref="NetDataWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="NetDataWriter"/> in which to store <typeparamref name="T"/>.</param>
    /// <param name="allocate">A boolean representing whether to automatically allocate in the method.</param>
    public abstract void Serialize(in NetDataWriter writer = default, bool allocate = true);
    /// <summary>
    /// Converts an array of bytes located in <paramref name="reader"/> into <typeparamref name="T"/>.
    /// </summary>
    /// <param name="reader">The <see cref="NetDataReader"/> in which to deserialize from.</param>
    /// <param name="advancePosition">A boolean representing whether to advance the position of the <see cref="NetDataReader"/></param>
    /// <returns>A new struct</returns>
    public abstract T Deserialize(in NetDataReader reader = default, bool advancePosition = true);
}
