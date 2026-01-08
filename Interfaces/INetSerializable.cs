using Core.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces;

public interface INetSerializable
{
    public abstract void Serialize(in NetDataWriter writer = default);
    public abstract void Deserialize(in NetDataReader reader = default);
}
