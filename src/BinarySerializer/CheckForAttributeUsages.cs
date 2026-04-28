using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BinarySerializer;

internal static class CheckForAttributeUsages
{
    internal static IncrementalValuesProvider<ClassModel?> Run(IncrementalGeneratorInitializationContext incrementalGeneratorInitializationContext)
    {
        IncrementalValuesProvider<ClassModel?> incrementalValuesProvider = incrementalGeneratorInitializationContext.SyntaxProvider.CreateSyntaxProvider((syntaxNode, _) =>
        {
            if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
            {
                return false;
            }

            return classDeclarationSyntax.IsKind(SyntaxKind.ClassDeclaration);
        }, (generatorSyntaxContext, _) =>
        {
            return GenerateClassModelFromSyntaxContext.Run(generatorSyntaxContext);
        }).Where(classModel => classModel is not null);

        return incrementalValuesProvider;
    }
}
