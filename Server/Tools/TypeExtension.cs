using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Tools;

internal static class TypeExtension
{
    public static bool InheritsOrImplements(this Type child, Type[] parents)
    {
        parents = ResolveGenericTypeDefinition(parents);

        var currentChild = child.IsGenericType
            ? child.GetGenericTypeDefinition()
            : child;

        while (currentChild != typeof(object))
        {
            foreach (var parent in parents)
            {
                if (parent == currentChild || HasAnyInterfaces(parent, currentChild))
                    return true;

            }
            currentChild = currentChild.BaseType != null
                           && currentChild.BaseType.IsGenericType
                ? currentChild.BaseType.GetGenericTypeDefinition()
                : currentChild.BaseType;

            if (currentChild == null)
                return false;
        }
        return false;
    }

    public static IEnumerable<Type> ListMessagesConsumedByInterfaces(Type consumerType, Type[] consumerTypeDefinition)
    {
        IEnumerable<Type> interfaces = consumerType
            .GetInterfaces()
            .Where(i => i.IsGenericType)
            .Where(t => consumerTypeDefinition.Any(n => t.GetGenericTypeDefinition() == n));

        foreach (var consumerInterface in interfaces)
        {
            var argument = consumerInterface.GetGenericArguments()[0];
            yield return argument;
        }
    }

    static bool HasAnyInterfaces(Type parent, Type child)
    {
        return child.GetInterfaces()
            .Any(childInterface =>
            {
                Type currentInterface = childInterface.IsGenericType
                    ? childInterface.GetGenericTypeDefinition()
                    : childInterface;

                return currentInterface == parent;
            });
    }

    static Type[] ResolveGenericTypeDefinition(Type[] parents)
    {
        for (var i = 0; i < parents.Length; i++)
        {
            var shouldUseGenericType = true;
            if (parents[i].IsGenericType && parents[i].GetGenericTypeDefinition() != parents[i])
                shouldUseGenericType = false;

            if (parents[i].IsGenericType && shouldUseGenericType)
                parents[i] = parents[i].GetGenericTypeDefinition();
        }
        return parents;
    }
}