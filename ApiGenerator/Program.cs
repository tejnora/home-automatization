namespace ApiGenerator;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Write("Generator has invalid params.");
        }
        try
        {
            var scanner = new AssemblyScanner();
            scanner.Init(args[0]);
            foreach (var service in scanner.Services)
            {
                var generator = new ApiGenerator(args[1], service.Value);
                generator.GenerateService();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine($"Generator failed:{e}");
            Environment.Exit(1);
        }
        Console.WriteLine("The output was generated successfully.");
        Environment.Exit(0);
    }
}