namespace PonyNetwork.Extensions;

public static class RangeExtensions
{
    extension(Range source)
    {
        public IEnumerator<int> GetEnumerator()
        {
            return Enumerable.Range(source.Start.Value, source.End.Value).GetEnumerator();
        }
    }
}
