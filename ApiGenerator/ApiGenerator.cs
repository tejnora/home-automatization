using System.Reflection;
using System.Text;

namespace ApiGenerator;

class ApiGenerator
{
    readonly string _rootPath;
    readonly ServiceData _service;
    StringBuilder _outputCode = new();
    List<StringBuilder> _outputCodeStack = new();
    StringBuilder _methodsCode = new();
    StringBuilder _IMethodCode = new();
    HashSet<Type> _interfaces = new();
    Dictionary<Type, string> _exportedObjects = new();

    public ApiGenerator(string rootPath, ServiceData service)
    {
        var name = service.Name.ToLowerInvariant();
        _rootPath = Path.Combine(rootPath, name, "services", $"{name}Api.ts");
        _service = service;
    }
    public void GenerateService()
    {
        PreprocessApiFunctions();

        EmitImports();
        EmitEmptyLine();
        EmitResponseInterfaces();
        EmitServiceInterface();
        EmitEmptyLine();
        EmitServiceBeginClass();
        EmitEmptyLine();
        _outputCode.Append(_methodsCode);
        EmitServiceEndClass();
        EmitEmptyLine();
        EmitServiceExport();
        Directory.CreateDirectory(Path.GetDirectoryName(_rootPath));
        File.WriteAllText(_rootPath, _outputCode.ToString());
    }
    void EmitImports()
    {
        _outputCode.AppendLine("//Whole file is generated");
        _outputCode.AppendLine("import { IApiClient, apiClient } from '../../core/api/apiClient';");
        _outputCode.AppendLine("import { IResponseBase } from \"../../core/api/responseBase\"");
    }

    void PushOutputCode()
    {
        _outputCodeStack.Add(_outputCode);
        _outputCode = new StringBuilder();
    }

    void PopOutputCode()
    {
        _outputCodeStack[0].Append(_outputCode);
        _outputCode = _outputCodeStack[^1];
        _outputCodeStack.RemoveAt(_outputCodeStack.Count-1);
    }
    void EmitResponseInterfaces()
    {
        foreach (var type in _interfaces)
        {
            PushOutputCode();
            _outputCode.AppendLine($"export interface {GetResponseInterface(type)} extends IResponseBase {{");
            foreach (var param in GetFunctionParams(type))
            {
                if (param.Name == "Result") continue;
                _outputCode.AppendLine($"   {param.Name}: {TypeToString(param.PropertyType)};");
            }
            _outputCode.AppendLine("}");
            EmitEmptyLine();
            PopOutputCode();
        }
    }

    string EmitResponseObject(Type type)
    {
        if (_exportedObjects.TryGetValue(type, out var typeName))
            return typeName;
        PushOutputCode();
        typeName = GetResponseInterface(type);
        _outputCode.AppendLine($"export interface {typeName}{{");
        foreach (var param in GetFunctionParams(type))
        {
            _outputCode.AppendLine($"   {param.Name}: {TypeToString(param.PropertyType)};");
        }
        _outputCode.AppendLine("}");
        EmitEmptyLine();
        PopOutputCode();
        _exportedObjects.Add(type, typeName);
        return typeName;
    }

    void EmitServiceBeginClass()
    {
        _outputCode.AppendLine($"class {_service.Name}Client implements I{_service.Name}Client {{");
        _outputCode.AppendLine("    private profileApiClient: IApiClient;");
        EmitEmptyLine();
        _outputCode.AppendLine("    constructor(profileApiClient: IApiClient) {");
        _outputCode.AppendLine("       this.profileApiClient = apiClient;");
        _outputCode.AppendLine("    }");
    }

    void EmitServiceInterface()
    {
        _outputCode.AppendLine($"export interface I{_service.Name}Client {{");
        _outputCode.Append(_IMethodCode);
        _outputCode.AppendLine("}");
    }
    void PreprocessApiFunctions()
    {
        foreach (var type in _service.PostFunction)
        {
            var inputType = type.Item1;
            var outputType = type.Item2;
            var fncParams = GetFunctionParams(inputType);
            var responseInterface = GetResponseInterface(outputType);
            var methodWithParamsDef = $"{GetFunctionName(inputType)}({GetFunctionArguments(fncParams)}): Promise<{responseInterface}>";
            if (responseInterface != "IResponseBase")
            {
                _interfaces.Add(outputType);
            }
            _methodsCode.AppendLine($"    async {methodWithParamsDef}{{");
            _methodsCode.AppendLine($"        return this.profileApiClient.post<{responseInterface}>(\"api/{_service.Name.ToLowerInvariant()}/{inputType.Name}\",{{{GetCallApiArguments(fncParams)}}});");
            _methodsCode.AppendLine("    }");
            _methodsCode.AppendLine("");

            _IMethodCode.AppendLine($"    {methodWithParamsDef};");
        }

        foreach (var type in _service.GetFunction)
        {
            var inputType = type.Item1;
            var outputType = type.Item2;
            var fncParams = GetFunctionParams(inputType);
            var responseInterface = GetResponseInterface(outputType);
            var methodWithParamsDef = $"{GetFunctionName(inputType)}({GetFunctionArguments(fncParams)}): Promise<{responseInterface}>";
            if (responseInterface != "IResponseBase")
            {
                _interfaces.Add(outputType);
            }
            _methodsCode.AppendLine($"    async {methodWithParamsDef}{{");
            _methodsCode.AppendLine($"        return this.profileApiClient.get<{responseInterface}>(\"api/{_service.Name.ToLowerInvariant()}/{inputType.Name}\");");
            _methodsCode.AppendLine("    }");
            _methodsCode.AppendLine("");

            _IMethodCode.AppendLine($"    {methodWithParamsDef};");
        }

    }

    void EmitServiceExport()
    {
        _outputCode.AppendLine($"export const {_service.Name.ToLowerInvariant()}Client = new {_service.Name}Client(apiClient);");
    }

    void EmitServiceEndClass()
    {
        _outputCode.AppendLine("}");
    }

    void EmitEmptyLine()
    {
        _outputCode.AppendLine();
    }

    static string GetFunctionName(MemberInfo type)
    {
        var name = ToLowerCase(type.Name);
        if (name.EndsWith("Command"))
        {
            return name[..^7];
        }
        return name.EndsWith("Query") ? name[..^5] : name;
    }

    static IEnumerable<PropertyInfo> GetFunctionParams(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    }

    string GetFunctionArguments(IEnumerable<PropertyInfo> values)
    {
        if (!values.Any()) return "";
        var result = new StringBuilder();
        foreach (var value in values)
        {
            result.Append($"{ToLowerCase(value.Name)}: {TypeToString(value.PropertyType)}, ");
        }
        return result.ToString(0, result.Length - 2);
    }

    string TypeToString(Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean:
                return "boolean";
            case TypeCode.Char:
                return "string";
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                return "number";
            case TypeCode.DateTime:
                return "Date";
            case TypeCode.String:
                return "string";
            case TypeCode.Object:
                {
                    if (type.IsConstructedGenericType)
                    {
                        if (type.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            return $"Array<{TypeToString(type.GenericTypeArguments[0])}>";
                        }
                    }
                    else
                    {
                        return EmitResponseObject(type);
                    }
                }
                break;
        }
        var code = Type.GetTypeCode(type);
        throw new ArgumentOutOfRangeException($"Type {code} is not supported.");
    }

    static string GetCallApiArguments(IEnumerable<PropertyInfo> values)
    {
        if (!values.Any()) return "";
        var result = new StringBuilder();
        foreach (var value in values)
        {
            result.Append($"{value.Name}:{ToLowerCase(value.Name)},");
        }
        return result.ToString(0, result.Length - 1);
    }

    static string ToLowerCase(string value)
    {
        return char.ToLower(value[0]) + value.Substring(1);
    }

    static string GetResponseInterface(Type requestType)
    {
        if (requestType.Name == "GeneralResponses")
            return "IResponseBase";
        return "I" + requestType.Name;
    }

}