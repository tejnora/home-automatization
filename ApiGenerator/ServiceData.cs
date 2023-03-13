namespace ApiGenerator;

class ServiceData
{
    public string Name { get; init; }
    public List<Tuple<Type, Type>> GetFunction { get; } = new();
    public List<Tuple<Type, Type>> PostFunction { get; } = new();
}