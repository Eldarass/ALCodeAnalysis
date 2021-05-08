using Microsoft.Dynamics.Nav.CodeAnalysis;

namespace ALCodeAnalysis.Utilities
{
    internal static class PooledHashSetExtensions
    {
        public static void ConcurrentAdd<T>(this PooledHashSet<T> pooledHashSet, T value)
        {
            lock (pooledHashSet)
                pooledHashSet.Add(value);
        }
    }
}
