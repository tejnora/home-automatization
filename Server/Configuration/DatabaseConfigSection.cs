using System.Configuration;

namespace Server.Configuration;

public class DatabaseConfigSection : ConfigSection
{
    public static DatabaseConfigSection GetConfiguration()
    {
        var configuration = GetDefaultConfiguration().GetSection("databaseStorage") as DatabaseConfigSection;
        return configuration ?? new DatabaseConfigSection();
    }

    [ConfigurationProperty("DiskFileCollection", DefaultValue = "FileCollection", IsRequired = false)]
    public string DiskFileCollection => (string)this["DiskFileCollection"];
}