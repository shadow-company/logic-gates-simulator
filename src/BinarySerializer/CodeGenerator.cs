using Microsoft.CodeAnalysis;

namespace BinarySerializer;

[Generator]
public class CodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext)
    {
        AddInterfaceSourceCode.Run(incrementalGeneratorInitializationContext);
        AddAttributeSourceCode.Run(incrementalGeneratorInitializationContext);
        IncrementalValuesProvider<ClassModel?> incrementalValuesProvider = CheckForAttributeUsages.Run(incrementalGeneratorInitializationContext);
        incrementalGeneratorInitializationContext.RegisterSourceOutput(incrementalValuesProvider, GeneratePartialClassSourceCode.Run);
    }
}
