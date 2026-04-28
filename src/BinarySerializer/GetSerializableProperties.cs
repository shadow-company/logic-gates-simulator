using Microsoft.CodeAnalysis;

namespace BinarySerializer;

internal static class GetSerializableProperties
{
    internal static (List<IPropertySymbol>, bool) Run(INamedTypeSymbol typeSymbol)
    {
        List<IPropertySymbol> propertySymbols = [];
        INamedTypeSymbol? currentSymbol = typeSymbol;
        int classesCount = 0;

        while (currentSymbol is { SpecialType: not SpecialType.System_Object })
        {
            classesCount++;
            List<IPropertySymbol> currentProperties = [.. currentSymbol.GetMembers().OfType<IPropertySymbol>().Where(property => property is { DeclaredAccessibility: Accessibility.Public, IsStatic: false, GetMethod: not null, SetMethod: not null })];
            propertySymbols.InsertRange(0, currentProperties);
            currentSymbol = currentSymbol.BaseType;
        }

        return (propertySymbols, classesCount > 1);
    }
}
