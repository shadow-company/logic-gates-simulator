using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BinarySerializer;

internal static class GenerateClassModelFromSyntaxContext
{
    internal static ClassModel? Run(GeneratorSyntaxContext generatorSyntaxContext)
    {
        ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)generatorSyntaxContext.Node;
        if (generatorSyntaxContext.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol symbol)
        {
            return null;
        }

        AttributeData? attributeData = symbol.GetAttributes().FirstOrDefault(attributeData =>
        {
            return attributeData.AttributeClass?.Name is "BinarySerializable" or "BinarySerializableAttribute";
        });

        if (attributeData is null)
        {
            return null;
        }

        List<IPropertySymbol> properties = [.. GetSerializableProperties.Run(symbol)];
        return new ClassModel(symbol.ContainingNamespace.ToDisplayString(), symbol.Name, properties);
    }
}
