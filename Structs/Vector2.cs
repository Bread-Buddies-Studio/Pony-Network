using PonyNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PonyNetwork.Structs;

/// <summary>
/// An example of an implementation of <see cref="INetSerializable{T}"/>
/// </summary>
public struct Vector2 : INetSerializable<Vector2>
{
    // Properties //
    public float X;
    public float Y;
    //
    #region Serializable Methods
    public readonly void Serialize(in NetDataWriter writer = default, bool allocate = true)
    {
        // Check if allocation is requested //
        if (allocate)
            writer.Allocate<float>(count: 2);
        // Insert Data //
        writer.Append(X);
        writer.Append(Y);
    }
    public Vector2 Deserialize(in NetDataReader reader = default, bool advancePosition = true)
    {
        // Create new Struct //
        return new()
        {
            // Set Properties //
            X = advancePosition ? reader.Read<float>() : reader.Peek<float>(),
            Y = advancePosition ? reader.Read<float>() : reader.Peek<float>()
        };
    }
    #endregion
}