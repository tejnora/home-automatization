namespace ApiGenerator;

class Program
{
    static void Main(string[] args)
    {
        args = new[]
        {
            "", @"c:/_Data/Repos/home-automatization\Server/bin/Debug/net6.0-windows/Server.dll",
            "c:/_Data/Repos/home-automatization/frontend/src"
        };
        if (args.Length != 3)
        {
            Console.Write("Generator has invalid params.");
        }
        try
        {
            var scanner = new AssemblyScanner();
            scanner.Init(args[1]);
            foreach (var service in scanner.Services)
            {
                var generator = new ApiGenerator(args[2], service.Value);
                generator.GenerateService();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine($"Generator failed:{e}");
            Environment.Exit(1);
        }
        Environment.Exit(0);
    }
}