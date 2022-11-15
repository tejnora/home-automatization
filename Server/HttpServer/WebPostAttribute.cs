using System;

namespace Server.HttpServer;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class WebPostAttribute : Attribute
{
}