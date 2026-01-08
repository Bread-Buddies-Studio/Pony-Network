using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Core.Structs;

public struct NetDataWriter(params byte[] Data)
{
    // Data //
    public byte[] Data { get; private set; } = Data;
    public uint Position { get; private set; } = 0;
    public readonly bool IsEmpty => Data.Length == 0;
    // Conversions //
    public static implicit operator NetDataWriter(in NetDataReader reader) => new(reader.Data);
    // Constructors //
    public NetDataWriter(int allocation) : this(new byte[allocation]) {}
    // Base Methods //
    public void Allocate(in int bytes) => Data = new byte[bytes + (Data?.Length ?? 0)];
    public void Allocate(in int itemSize, in int count) => Allocate(itemSize * count);
    public unsafe void Allocate<T>(in int count) where T : unmanaged => Allocate(sizeof(T), count);
    #region Append Methods
    public void Append(in byte value) => Data[Position++] = value;
    public void Append(in ReadOnlySpan<byte> value)
    {
        foreach (byte data in value)
            Data[Position++] = data;
    }
    public void Append(in string value)
    {
        // Add String Length //
        foreach (char letter in value)
            Append(letter);
        // Add Null Terminmator //
        Append('\0');
    }
    public readonly void Append(in INetSerializable value) => value.Serialize(this);
    public unsafe void Append<T>(T value) where T : unmanaged
    {
        // Get Bytes //
        Span<byte> bytes = stackalloc byte[sizeof(T)];
        // Get Memory Address //
        byte* pointer = (byte*)&value;
        // Add Bytes to Data //
        for (int index = 0; index < sizeof(T); index++)
            bytes[index] = *pointer++;
        // Append Bytes //
        Append(bytes);
    }
    #endregion
    #region Prepend Methods
    public void Prepend(in byte value) => Data = [value, ..Data];
    public void Prepend(in ReadOnlySpan<byte> value)
    {
        for (int index = value.Length - 1; index >= 0; index--)
            Prepend(value[index]);
    }
    public unsafe void Prepend<T>(T value) where T : unmanaged
    {
        // Get Bytes //
        Span<byte> bytes = stackalloc byte[sizeof(T)];
        // Get Memory Address //
        byte* pointer = (byte*)&value;
        // Add Bytes to Data //
        for (int index = 0; index < sizeof(T); index++)
            bytes[index] = *pointer++;
        // Append Bytes //
        Prepend(bytes);
    }
    public void Prepend(in string value)
    {
        // Add Null Terminmator //
        Prepend('\0');
        // Add String Length //
        foreach (char letter in value.Reverse())
            Prepend(letter);
    }
    public void Prepend(in INetSerializable value)
    {
        NetDataWriter writer = new();
        // Add Data //
        value.Serialize(writer);
        // Prepend //
        Prepend(writer.Data);
    }
    #endregion
    public void Clear(int newAllocation)
    {
        Position = 0;
        Data = new byte[newAllocation];
    }
    public void Clear() => Clear(0);
}