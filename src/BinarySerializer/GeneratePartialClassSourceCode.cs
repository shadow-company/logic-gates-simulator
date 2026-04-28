using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace BinarySerializer;

internal static class GeneratePartialClassSourceCode
{
    internal static void Run(SourceProductionContext sourceProductionContext, ClassModel? classModel)
    {
        if (classModel is not { Properties.Count: > 0 })
        {
            return;
        }

        string sourceCode = GenerateSourceCode(classModel);
        sourceProductionContext.AddSource($"{classModel.Name}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private static string GenerateSourceCode(ClassModel classModel)
    {
        HashSet<string> namespaceStrings = [];

        foreach (IPropertySymbol propertySymbol in classModel.Properties)
        {
            if (propertySymbol.Type.TypeKind is TypeKind.Enum)
            {
                namespaceStrings.Add(propertySymbol.Type.ContainingNamespace.ToDisplayString());
            }
        }

        StringBuilder sourceCode = new();
        sourceCode.AppendLine($"#nullable enable");
        sourceCode.AppendLine($"using System.Text;");
        sourceCode.AppendLine($"using BinarySerializer;");
        foreach (string namespaceString in namespaceStrings)
        {
            sourceCode.AppendLine($"using {namespaceString};");
        }
        sourceCode.AppendLine();
        sourceCode.AppendLine($"namespace {classModel.NamespaceString};");
        sourceCode.AppendLine();
        sourceCode.AppendLine($"partial class {classModel.Name} : IBinarySerializable<{classModel.Name}>");
        sourceCode.AppendLine("{");
        sourceCode.AppendLine("    public MemoryStream Serialize()");
        sourceCode.AppendLine("    {");
        sourceCode.AppendLine("        MemoryStream memoryStream = new();");
        sourceCode.AppendLine("        using BinaryWriter binaryWriter = new(memoryStream, Encoding.UTF8, true);");
        GenerateSerializePropertiesSourceCode(sourceCode, classModel);
        sourceCode.AppendLine("        return memoryStream;");
        sourceCode.AppendLine("    }");
        sourceCode.AppendLine();
        sourceCode.AppendLine("    public void Deserialize(MemoryStream memoryStream)");
        sourceCode.AppendLine("    {");
        sourceCode.AppendLine("        using BinaryReader binaryReader = new(memoryStream, Encoding.UTF8, true);");
        GenerateDeserializePropertiesSourceCode(sourceCode, classModel);
        sourceCode.AppendLine("    }");
        sourceCode.AppendLine("}");
        return sourceCode.ToString();
    }

    private static void GenerateSerializePropertiesSourceCode(StringBuilder sourceCode, ClassModel recordModel)
    {
        foreach (IPropertySymbol propertySymbol in recordModel.Properties)
        {
            INamedTypeSymbol? namedTypeSymbol = propertySymbol.Type as INamedTypeSymbol;
            SpecialType specialType = propertySymbol.Type.SpecialType;
            TypeKind typeKind = propertySymbol.Type.TypeKind;
            string propertyName = propertySymbol.Name;
            string propertyType = propertySymbol.Type.Name;

            switch (specialType)
            {
                case SpecialType.System_String:
                case SpecialType.System_Int32:
                case SpecialType.System_Boolean:
                    sourceCode.AppendLine($"        {SerializeDefault(propertyName)}");
                    break;

                case SpecialType.None when namedTypeSymbol?.Name is "List" && namedTypeSymbol?.TypeArguments.Length is 1:
                    sourceCode.AppendLine($"        binaryWriter.Write({propertyName}.Count);");
                    sourceCode.AppendLine($"        foreach ({namedTypeSymbol.TypeArguments[0].Name} value in {propertyName})");
                    sourceCode.AppendLine($"        {{");
                    sourceCode.AppendLine($"            {Serialize("value", namedTypeSymbol.TypeArguments[0])}");
                    sourceCode.AppendLine($"        }}");
                    break;

                case SpecialType.None when namedTypeSymbol?.Name is "Dictionary" && namedTypeSymbol?.TypeArguments.Length is 2:
                    sourceCode.AppendLine($"        binaryWriter.Write({propertyName}.Count);");
                    sourceCode.AppendLine($"        foreach (({namedTypeSymbol.TypeArguments[0].Name} key, {namedTypeSymbol.TypeArguments[1].Name} value) in {propertyName})");
                    sourceCode.AppendLine($"        {{");
                    sourceCode.AppendLine($"            {Serialize("key", namedTypeSymbol.TypeArguments[0])}");
                    sourceCode.AppendLine($"            {Serialize("value", namedTypeSymbol.TypeArguments[1])}");
                    sourceCode.AppendLine($"        }}");
                    break;

                case SpecialType.None when typeKind is TypeKind.Enum:
                    sourceCode.AppendLine($"        {SerializeEnum(propertyName)}");
                    break;

                case SpecialType.None when namedTypeSymbol?.Name is "Guid":
                    sourceCode.AppendLine($"        {SerializeGuid(propertyName)}");
                    break;

                case SpecialType.None when propertyType is "Dictionary" && namedTypeSymbol?.TypeArguments.Length is 2:

                default:
                    sourceCode.AppendLine($"        // UNSUPPORTED: Name: '{propertyName}'. Type: '{propertyType}'. SpecialType: '{specialType}'. OriginalDefinition: '{propertySymbol.Type.OriginalDefinition}'. TypeKind: '{typeKind}'.");
                    break;
            }
        }
    }

    private static string Serialize(string propertyName, ITypeSymbol propertyType)
    {
        switch (propertyType.SpecialType)
        {
            case SpecialType.System_String:
            case SpecialType.System_Int32:
            case SpecialType.System_Boolean:
                return SerializeDefault(propertyName);

            case SpecialType.None when propertyType.TypeKind is TypeKind.Enum:
                return SerializeEnum(propertyName);

            case SpecialType.None when propertyType.Name is "Guid":
                return SerializeGuid(propertyName);

            default:
                return $"// UNSUPPORTED SUBTYPE: '{propertyType}'.";
        }
    }

    private static string SerializeDefault(string propertyName)
    {
        return $"binaryWriter.Write({propertyName});";
    }

    private static string SerializeEnum(string propertyName)
    {
        return $"binaryWriter.Write((int){propertyName});";
    }

    private static string SerializeGuid(string propertyName)
    {
        return $"binaryWriter.Write({propertyName}.ToByteArray());";
    }

    private static void GenerateDeserializePropertiesSourceCode(StringBuilder sourceCode, ClassModel recordModel)
    {
        foreach (IPropertySymbol propertySymbol in recordModel.Properties)
        {
            INamedTypeSymbol? namedTypeSymbol = propertySymbol.Type as INamedTypeSymbol;
            SpecialType specialType = propertySymbol.Type.SpecialType;
            TypeKind typeKind = propertySymbol.Type.TypeKind;
            string propertyName = propertySymbol.Name;
            string propertyType = propertySymbol.Type.Name;

            switch (specialType)
            {
                case SpecialType.System_String:
                    sourceCode.AppendLine($"        {DeserializeString(propertyName)}");
                    break;

                case SpecialType.System_Int32:
                    sourceCode.AppendLine($"        {DeserializeInt(propertyName)}");
                    break;

                case SpecialType.System_Boolean:
                    sourceCode.AppendLine($"        {DeserializeBoolean(propertyName)}");
                    break;

                case SpecialType.None when namedTypeSymbol?.Name is "List" && namedTypeSymbol.TypeArguments.Length == 1:
                    sourceCode.AppendLine($"        int {propertyName.ToLower()}Count = binaryReader.ReadInt32();");
                    sourceCode.AppendLine($"        {propertyName} = [];");
                    sourceCode.AppendLine($"        for (int i = 0; i < {propertyName.ToLower()}Count; i++)");
                    sourceCode.AppendLine($"        {{");
                    sourceCode.AppendLine($"            {namedTypeSymbol.TypeArguments[0].Name} {Deserialize("value", namedTypeSymbol.TypeArguments[0])}");
                    sourceCode.AppendLine($"            {propertyName}.Add(value);");
                    sourceCode.AppendLine($"        }}");
                    break;

                case SpecialType.None when namedTypeSymbol?.Name is "Dictionary" && namedTypeSymbol.TypeArguments.Length == 2:
                    sourceCode.AppendLine($"        int {propertyName.ToLower()}Count = binaryReader.ReadInt32();");
                    sourceCode.AppendLine($"        {propertyName} = [];");
                    sourceCode.AppendLine($"        for (int i = 0; i < {propertyName.ToLower()}Count; i++)");
                    sourceCode.AppendLine($"        {{");
                    sourceCode.AppendLine($"            {namedTypeSymbol.TypeArguments[0].Name} {Deserialize("key", namedTypeSymbol.TypeArguments[0])}");
                    sourceCode.AppendLine($"            {namedTypeSymbol.TypeArguments[1].Name} {Deserialize("value", namedTypeSymbol.TypeArguments[1])}");
                    sourceCode.AppendLine($"            {propertyName}.Add(key, value);");
                    sourceCode.AppendLine($"        }}");
                    break;

                case SpecialType.None when typeKind is TypeKind.Enum:
                    sourceCode.AppendLine($"        {DeserializeEnum(propertyName, propertyType)}");
                    break;

                case SpecialType.None when namedTypeSymbol?.Name is "Guid":
                    sourceCode.AppendLine($"        {DeserializeGuid(propertyName)}");
                    break;

                default:
                    sourceCode.AppendLine($"        // UNSUPPORTED: Name: '{propertyName}'. Type: '{propertyType}'. SpecialType: '{specialType}'. OriginalDefinition: '{propertySymbol.Type.OriginalDefinition}'. TypeKind: '{typeKind}'");
                    break;
            }
        }
    }

    private static string Deserialize(string propertyName, ITypeSymbol propertyType)
    {
        switch (propertyType.SpecialType)
        {
            case SpecialType.System_String:
                return DeserializeString(propertyName);

            case SpecialType.System_Int32:
                return DeserializeInt(propertyName);

            case SpecialType.System_Boolean:
                return DeserializeBoolean(propertyName);

            case SpecialType.None when propertyType.TypeKind is TypeKind.Enum:
                return DeserializeEnum(propertyName, propertyType.Name);

            case SpecialType.None when propertyType.Name is "Guid":
                return DeserializeGuid(propertyName);

            default:
                return $"// UNSUPPORTED SUBTYPE: '{propertyType}'.";
        }
    }

    private static string DeserializeString(string propertyName)
    {
        return $"{propertyName} = binaryReader.ReadString();";
    }

    private static string DeserializeInt(string propertyName)
    {
        return $"{propertyName} = binaryReader.ReadInt32();";
    }

    private static string DeserializeBoolean(string propertyName)
    {
        return $"{propertyName} = binaryReader.ReadBoolean();";
    }

    private static string DeserializeEnum(string propertyName, string typeName)
    {
        return $"{propertyName} = ({typeName})binaryReader.ReadInt32();";
    }

    private static string DeserializeGuid(string propertyName)
    {
        return $"{propertyName} = new(binaryReader.ReadBytes(16));";
    }
}
