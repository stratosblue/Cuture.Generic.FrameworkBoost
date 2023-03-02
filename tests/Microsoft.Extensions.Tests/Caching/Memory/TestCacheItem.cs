using System.Diagnostics;

namespace Microsoft.Extensions.Tests.Caching.Memory
{
    public record TestCacheItem(Guid Id)
    {
        [DebuggerStepThrough]
        public TestCacheItem() : this(Guid.NewGuid())
        {
        }

        public override string ToString() => Id.ToString();
    }
}

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}