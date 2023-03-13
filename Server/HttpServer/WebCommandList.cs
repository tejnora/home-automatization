using System;
using System.Collections.Generic;
using System.Linq;
using Share;

namespace Server.HttpServer;

public class WebCommandsList
{
    public IEnumerable<Type> ListGetCommands()
    {
        var types = System.Reflection.Assembly.GetExecutingAssembly().GetExportedTypes();
        return types.Where(t => Attribute.IsDefined(t, typeof(WebGetAttribute))).ToList();
    }

    public IEnumerable<Type> ListPostCommands()
    {
        var types = System.Reflection.Assembly.GetExecutingAssembly().GetExportedTypes();
        return types.Where(t => Attribute.IsDefined(t, typeof(WebPostAttribute))).ToList();
    }
}