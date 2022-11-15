using System;

namespace Server.Core.Session
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SessionAttribute : Attribute
    {
    }
}