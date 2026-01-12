using Core.Handlers;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Core.Structs;

public struct NetDataReader(in byte[] Data)
{
    // Properties //
    public int Position { get; private set; }
    public readonly byte[] Data { get; private init; } = Data;
    // Conversions //
    public static implicit operator NetDataReader(in NetDataWriter writer) => new(writer.Data);
    // Skip Methods //
    public void Skip(int length = 1) => Position += length;

    #region Peek Methods
    public readonly string PeekString(int Position)
    {
        // Data //
        string message = string.Empty;
        int offset = 0;
        // Construct //
        while (true)
        {
            char letter = Peek<char>(Position + (sizeof(char) * offset++));
            // Conditions //
            if (letter == '\0' || Position >= Data.Length - 1)
                break;
            // Append Letter //
            message += letter;
        }
        // Decode //
        return message;
    }
    public readonly string PeekString() => PeekString(Position);

    public readonly ReadOnlySpan<byte> Peek(int Position, int length)
    {
        // Get Bytes //
        ReadOnlySpan<byte> bytes = Data.AsSpan();
        // Take out Data //
        return bytes.Slice(Position, length);
    }
    public readonly ReadOnlySpan<byte> Peek(int length) => Peek(Position, length);
    public unsafe readonly T Peek<T>(int Position) where T : unmanaged
    {
        // Get Bytes //
        ReadOnlySpan<byte> bytes = Peek(Position, sizeof(T));
        // Cast to Unmanaged //
        return MemoryMarshal.Cast<byte, T>(bytes)[0];
    }
    public readonly T Peek<T>() where T : unmanaged => Peek<T>(Position);
    #endregion

    #region Read Methods
    public unsafe T Read<T>(int Position) where T : unmanaged
    {
        // Get Data //
        ReadOnlySpan<byte> bytes = Peek(Position, sizeof(T));
        // Increment //
        this.Position += bytes.Length;
        // Convert Data //
        return MemoryMarshal.Cast<byte, T>(bytes)[0];
    }
    public T Read<T>() where T : unmanaged => Read<T>(Position);
    public string ReadString(int Position)
    {
        // Get Value //
        string str = PeekString(Position);
        // Increment //
        this.Position += str.Length + 1;
        // Return //
        return str;
    }
    public string ReadString() => ReadString(Position);
    #endregion
}