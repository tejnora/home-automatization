﻿using System;
using System.Reflection;

namespace Server.Core.MessageMapping
{
    public static class AssemblyScanEvil
    {
        public static bool IsProbablyUserAssembly(Assembly assembly)
        {
            if (string.IsNullOrEmpty(assembly.FullName))
                return false;

            if (assembly.IsDynamic)
                return false;

            var prefixes = new[]
                {
                    "System", "Microsoft", "nunit", "JetBrains", "Autofac", "mscorlib", "ProtoBuf",
                    "Disruptor", "log4net"
                };

            foreach (var prefix in prefixes)
            {
                if (assembly.FullName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}