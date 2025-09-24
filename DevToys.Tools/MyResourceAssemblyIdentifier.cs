using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using DevToys.Api;

namespace NETWORG.DevToys.Tools
{

    [Export(typeof(IResourceAssemblyIdentifier))]
    [Name(nameof(MyResourceAssemblyIdentifier))]
    internal sealed class MyResourceAssemblyIdentifier : IResourceAssemblyIdentifier
    {
        public async ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
        {
            return Array.Empty<FontDefinition>();
        }
    }
}