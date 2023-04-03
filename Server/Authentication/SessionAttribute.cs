using System;

namespace Server.Authentication;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class SessionAttribute : Attribute
{
    
}