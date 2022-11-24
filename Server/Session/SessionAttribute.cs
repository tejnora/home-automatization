using System;

namespace Server.Session;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class SessionAttribute : Attribute
{
}