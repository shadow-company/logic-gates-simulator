using Microsoft.CodeAnalysis;

namespace BinarySerializer;

internal record ClassModel
{
    public readonly string NamespaceString;
    public readonly string Name;
    public bool IsInherited;
    public List<IPropertySymbol> Properties;

    public ClassModel(string namespaceString, string name, bool isInherited, List<IPropertySymbol> properties)
    {
        NamespaceString = namespaceString;
        Name = name;
        IsInherited = isInherited;
        Properties = properties;
    }
}
