using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace StackWarden.Core.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<T> FromAssembly<T>(this T expectedType, Assembly assembly, bool shouldIncludeAbstract = false)
            where T: Type
        {
            var foundTypes = assembly.DefinedTypes
                                     .Where(x => x.IsPublic &&
                                                 (shouldIncludeAbstract || !x.IsAbstract) &&
                                                 expectedType.IsAssignableFrom(x))
                                     .Cast<T>();

            return foundTypes;
        }

        public static IEnumerable<T> FromAssembly<T>(this T expectedType, string assemblyPath, bool shouldIncludeAbstract = false)
            where T: Type
        {
            var assembly = Assembly.LoadFrom(assemblyPath);

            return expectedType.FromAssembly(assembly, shouldIncludeAbstract);
        }

        public static Assembly FromDirectory(this AssemblyName name, string path)
        {
            var foundAssembly = Directory.Exists(path)
                                    ? Directory.GetFiles(path, "*.dll")
                                               .Select(x => Assembly.LoadFile(path))
                                               .FirstOrDefault(x => x.FullName == name.FullName)
                                    : null;

            return foundAssembly;
        }

        public static Type FromDirectory(this string typeName, string path)
        {
            var foundType = Directory.Exists(path)
                                ? Directory.GetFiles(path, "*.dll")
                                           .Select(x => Assembly.LoadFile(x))
                                           .Select(x => x.GetType(typeName))
                                           .Where(x => x != null)
                                           .FirstOrDefault()
                                : null;

            return foundType;
        }
    }
}