using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Kelist.Tests.Unit")]

namespace Application
{
    public class ApplicationAssemblyReference
    {
        internal static readonly Assembly Assembly = typeof(ApplicationAssemblyReference).Assembly;
    }
}
