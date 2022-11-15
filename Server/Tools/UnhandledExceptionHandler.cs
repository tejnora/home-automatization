using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Server.Tools
{
    public static class UnhandledExceptionHandler
    {
        static void TaskException(object test, UnobservedTaskExceptionEventArgs exception)
        {
            ProcessException("UnobservedTaskException", exception.Exception);
            exception.SetObserved();
        }

        static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ProcessException("UnhandledException", e.ExceptionObject);
        }

        static void ProcessException(string message, object e)
        {
            var exception = e as Exception;
            Log.Fatal(message, exception);
            CreateCrashLog(exception);
        }

        static void CreateCrashLog(Exception exception)
        {
            try
            {
                var file = $"{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.crash";
                using TextWriter fs = File.CreateText(file);
                var exceptionString = CreateExceptionString(exception);
                fs.Write(exceptionString);
            }
            catch (Exception ex)
            {
                Log.Fatal("Crash log could not be created.", ex);
            }
        }

        static string CreateExceptionString(Exception e)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Entry assembly: {0}\n", System.Reflection.Assembly.GetEntryAssembly().GetName().FullName);
            CreateExceptionString(sb, e, string.Empty);
            return sb.ToString();
        }

        private static void CreateExceptionString(StringBuilder sb, Exception e, string indent)
        {
            if (indent == null)
                indent = string.Empty;
            else if (indent.Length > 0)
                sb.Append($"{indent}Inner ");
            sb.Append($"Exception Found:\n{indent}Type: {e.GetType().FullName}");
            sb.Append($"\n{indent}Message: {e.Message}");
            sb.Append($"\n{indent}Source: {e.Source}");
            sb.Append($"\n{indent}Stacktrace: {e.StackTrace}");
            if (e.InnerException == null) return;
            sb.Append("\n");
            CreateExceptionString(sb, e.InnerException, indent + "  ");
        }

        public static void AttachHandlers()
        {
            TaskScheduler.UnobservedTaskException += TaskException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;
        }

        static void FirstChanceHandler(object sender, FirstChanceExceptionEventArgs e)
        {
            Log.Debug(e.ToString());
        }
    }
}
