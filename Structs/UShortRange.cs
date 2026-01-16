using System;
using System.Collections.Generic;
using System.Text;

namespace PonyNetwork.Structs;

internal struct UShortRange(ushort Begin, ushort End)
{
    // Properties //
    public ushort Begin = Begin;
    public ushort End = End;
    // Base Methods //
    public readonly IEnumerator<int> GetEnumerator()
    {
        return Enumerable.Range(Begin, End).GetEnumerator();
    }
}
