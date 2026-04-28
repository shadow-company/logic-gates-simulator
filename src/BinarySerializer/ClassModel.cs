using Microsoft.CodeAnalysis;

namespace BinarySerializer;

internal record ClassModel
{
    public readonly string NamespaceString;
    public readonly string Name;
    public List<IPropertySymbol> Properties;

    public ClassModel(string namespaceString, string name, List<IPropertySymbol> properties)
    {
        NamespaceString = namespaceString;
        Name = name;
        Properties = properties;
    }
}
