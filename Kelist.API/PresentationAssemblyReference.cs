using Application;
using System.Reflection;

namespace Kelist.API
{
    public class PresentationAssemblyReference
    {
        internal static readonly Assembly Assembly = typeof(ApplicationAssemblyReference).Assembly;
    }
}
