using Microsoft.CodeAnalysis;

namespace BinarySerializer;

internal static class GetSerializableProperties
{
    internal static IEnumerable<IPropertySymbol> Run(INamedTypeSymbol typeSymbol)
    {
        List<IPropertySymbol> propertySymbols = [];
        INamedTypeSymbol? currentSymbol = typeSymbol;

        while (currentSymbol is { SpecialType: not SpecialType.System_Object })
        {
            List<IPropertySymbol> currentProperties = [.. currentSymbol.GetMembers().OfType<IPropertySymbol>().Where(property => property is { DeclaredAccessibility: Accessibility.Public, IsStatic: false, GetMethod: not null, SetMethod: not null })];
            propertySymbols.InsertRange(0, currentProperties);
            currentSymbol = currentSymbol.BaseType;
        }

        return propertySymbols;
    }
}
