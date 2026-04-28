using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace BinarySerializer;

internal static class AddInterfaceSourceCode
{
    internal static void Run(IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext)
    {
        string filename = "IBinarySerializable.g.cs";
        string code =
@"namespace BinarySerializer;

public interface IBinarySerializable<T>
{
    MemoryStream Serialize();
    void Deserialize(MemoryStream memoryStream);
}
";

        IncrementalValueProvider<bool> existsProvider = incrementalGeneratorInitializationContext.CompilationProvider.Select((compilation, _) =>
        {
            return compilation.GetTypeByMetadataName("BinarySerializer.IBinarySerializable`1") is not null;
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
