using System.Reflection;
using Share;


namespace ApiGenerator;

class AssemblyScanner
{
    public Dictionary<string, ServiceData> Services { get; } = new();
    readonly Dictionary<Type, Type> _responseTypes = new();
    Assembly _assembly;

    public void Init(string assemblyName)
    {
        _assembly = Assembly.LoadFrom(assemblyName);
        FillResponseTypeMap();
        Fill<WebGetAttribute>((s, what) => { s.GetFunction.Add(new Tuple<Type, Type>(what, _responseTypes[what])); });
        Fill<WebPostAttribute>((s, what) => { s.PostFunction.Add(new Tuple<Type, Type>(what, _responseTypes[what])); });
    }

    void Fill<T>(Action<ServiceData, Type> addAction)
    {
        var types= _assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(T), true).Length > 0);
        foreach (var t in types)
        {
            var serviceName = GetServiceName(t);
            if(!Services.ContainsKey(serviceName))
                Services.Add(serviceName,new ServiceData {Name = serviceName});
            var serviceData = Services[serviceName];
            addAction(serviceData, t);
        }
    }

    void FillResponseTypeMap()
    {
        var types = _assembly.ExportedTypes.ToList();
        var genericDef = new[]
        {
            _assembly.GetType("Server.Core.Define+IQuery`2"),
            _assembly.GetType("Server.Core.Define+IConsumer`1"),
            _assembly.GetType("Server.Core.Define+IConsumer`2"),
        };
        var consumerTypes = types
            .Where(t => t.InheritsOrImplements(genericDef))
            .Where(t => !t.IsGenericType)
            .ToArray();
        foreach (var consumerType in consumerTypes)
        {
            var ifaces=consumerType.GetInterfaces().Where(i => i.IsGenericType);
            foreach (var iface in ifaces)
            {
                var arguments = iface.GetGenericArguments();
                _responseTypes.Add(arguments[0], arguments[1]);
            }
        }
    }

    public string GetServiceName(Type aType)
    {
        var namespacePath = aType.Namespace.Split('.');
        return namespacePath[namespacePath.Length - 2];
    }
}
