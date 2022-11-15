using System;
using Serilog;

namespace Server.Tools
{
    public static class NameAndVersionReporter
    {
        public static void Print()
        {
            var message = System.Reflection.Assembly.GetEntryAssembly().GetName().Name + " " + Version;
            Log.Information(message);
        }

        public static string Version => System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
    }
}