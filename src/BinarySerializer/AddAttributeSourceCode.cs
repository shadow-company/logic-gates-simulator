using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace BinarySerializer;

internal static class AddAttributeSourceCode
{
    internal static void Run(IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext)
    {
        string filename = "BinarySerializableAttribute.g.cs";
        string code =
@"namespace BinarySerializer;

[AttributeUsage(AttributeTargets.Class)]
internal class BinarySerializableAttribute : Attribute { }
";

        IncrementalValueProvider<bool> existsProvider = incrementalGeneratorInitializationContext.CompilationProvider.Select((compilation, _) =>
        {
            return compilation.GetTypeByMetadataName("BinarySerializer.BinarySerializableAttribute`1") is not null;
        });

        incrementalGeneratorInitializationContext.RegisterSourceOutput(existsProvider, (sourceProductionContext, exists) =>
        {
            if (!exists)
            {
                sourceProductionContext.AddSource(filename, SourceText.From(code, Encoding.UTF8));
            }
        });
    }
}
