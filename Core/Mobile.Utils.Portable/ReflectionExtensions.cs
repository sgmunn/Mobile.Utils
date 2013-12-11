// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Globalization;

namespace Mobile.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods for reflection
    /// </summary>
    public static class ReflectionExtensions
    {
//        /// <summary>
//        /// Gets all Types that have the given attribute defined
//        /// </summary>        
//        public static IQueryable<Type> GetTypesWith<TAttribute>(Assembly[] assemblies, bool inherit) where TAttribute: System.Attribute
//        {
//            // assemblies = AppDomain.CurrentDomain.GetAssemblies()
//            return from a in assemblies.AsQueryable()
//                from t in a.GetTypes()
//                    where t.IsDefined(typeof(TAttribute),inherit)
//                    select t;
//        }
//        
//        /// <summary>
//        /// Gets the attributes for a given Type as a Queryable
//        /// </summary>
//        public static IQueryable<Attribute> GetAttributes(Type objectType)
//        {
//            return System.Attribute.GetCustomAttributes(objectType).AsQueryable();
//        }
//
//        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit)
//        {
//            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
//        }
//        

        public static PropertyInfo GetPropertyInfo(this object instance, string propertyPath)
        {
            return instance.GetType().GetProperty(propertyPath);
        }

        /// <summary>
        /// Provides a GetMethod compatible version that recurses up to base types 
        /// </summary>
        public static MethodInfo GetMethod(this Type type, string name)
        {
            var typeInfo = type.GetTypeInfo();
            var info = typeInfo.GetDeclaredMethod(name);

            if (info == null)
            {
                if (typeInfo.BaseType == null)
                {
                    return null;
                }

                return GetMethod(typeInfo.BaseType, name);
            }

            return info;
        }

        /// <summary>
        /// Provides a GetProperty compatible version that recurses up to base types 
        /// </summary>
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            var typeInfo = type.GetTypeInfo();
            var info = typeInfo.GetDeclaredProperty(name);

            if (info == null)
            {
                if (typeInfo.BaseType == null)
                {
                    return null;
                }

                return GetProperty(typeInfo.BaseType, name);
            }

            return info;
        }

        public static EventInfo GetEvent(this Type type, string name)
        {
            var typeInfo = type.GetTypeInfo();
            var info = typeInfo.GetDeclaredEvent(name);

            if (info == null)
            {
                type = typeInfo.BaseType;
                if (type == null)
                {
                    return null;
                }

                return GetEvent(type, name);
            }

            return info;
        }

        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsAssignableFrom(otherType.GetTypeInfo());
        }

        public static object GetPropertyPathValue(this object obj, string propertyPath)
        {
            if (obj != null)
            {
                object targetObject;
                var propertyInfo = obj.FindProperty(propertyPath, out targetObject);
                if (propertyInfo != null && propertyInfo.CanRead)
                {
                    var path = GetLastPropertyPathComponent(propertyPath);
                    if (IsIndexedPath(path))
                    {
                        var index = GetPropertyPathIndexParts(path);
                        return propertyInfo.GetValue(targetObject, GetIndexers(index, propertyInfo));
                    }

                    return propertyInfo.GetValue(targetObject, null);
                }
            }

            return null;
        }

        public static void SetPropertyPathValue(this object obj, string propertyPath, object value)
        {
            if (obj != null)
            {
                object targetObject;
                var propertyInfo = obj.FindProperty(propertyPath, out targetObject);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    var path = GetLastPropertyPathComponent(propertyPath);
                    if (IsIndexedPath(path))
                    {
                        var index = GetPropertyPathIndexParts(path);
                        propertyInfo.SetValue(targetObject, value, GetIndexers(index, propertyInfo));
                    }
                    else
                    {
                        propertyInfo.SetValue(targetObject, value, null);
                    }
                }
            }
        }

        public static PropertyInfo FindProperty(this object instance, string propertyPath)
        {
            object inspectedObject;
            return instance.FindProperty(propertyPath, out inspectedObject);
        }

        public static PropertyInfo FindProperty(this object instance, string propertyPath, out object inspectedObject)
        {
            inspectedObject = instance;
            if (propertyPath.Contains("."))
            {
                var pathComponents = propertyPath.Split(new [] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (pathComponents.Length > 1)
                {
                    // recurse into each level, except the last one                    
                    foreach (var path in pathComponents.Take(pathComponents.Length - 1))
                    {
                        var info = GetProperty(inspectedObject, path, out inspectedObject);
                        if (info == null)
                        {
                            return null;
                        }

                        if (IsIndexedPath(path))
                        {
                            var index = GetPropertyPathIndexParts(path);

                            inspectedObject = info.GetValue(inspectedObject, GetIndexers(index, info));
                        }
                        else
                        {
                            inspectedObject = info.GetValue(inspectedObject);
                        }
                    }

                    var lastPath = pathComponents.Last();
                    return GetProperty(inspectedObject, lastPath, out inspectedObject);
                }
            }

            return GetProperty(inspectedObject, propertyPath, out inspectedObject);
        }

        public static PropertyInfo GetProperty(object instance, string propertyName, out object inspectedObject)
        {
            inspectedObject = instance;

            if (IsIndexedPath(propertyName))
            {
                var indexer = GetPropertyPathIndexedPropertyName(propertyName);
                inspectedObject = inspectedObject.GetType().GetProperty(indexer).GetValue(inspectedObject);

                // only support default indexer
                return inspectedObject.GetType().GetProperty("Item");
            }

            return inspectedObject.GetType().GetProperty(propertyName);
        }

        public static string GetLastPropertyPathComponent(string propertyPath)
        {
            var pathComponents = propertyPath.Split(new [] { "." }, StringSplitOptions.RemoveEmptyEntries);
            return pathComponents.Last();
        }

        public static object[] GetIndexers(string indexParts, PropertyInfo indexer)
        {
            var indexValues = indexParts.Split(',');
            var result = new List<object>();

            ParameterInfo[] indexParameters = indexer.GetIndexParameters();

            for (int i = 0; i < indexParameters.Length; i++)
                result.Add(Convert.ChangeType(indexValues[i], indexParameters[i].ParameterType, CultureInfo.InvariantCulture));

            return result.ToArray();
        }

        public static bool IsIndexedPath(string path)
        {
            // IMPROVE: handling of how property paths are indexed - must be <indexedPropertyName> [ <index> , <index> ]  with no spaces
            return path.Contains("[") && path.Contains("]");
        }

        public static string GetPropertyPathIndexParts(string path)
        {
            // IMPROVE: handling of how property paths are indexed - must be <indexedPropertyName> [ <index> , <index> ]  with no spaces
            var pathComponents = path.Split(new [] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
            if (pathComponents.Length == 2)
            {
                return pathComponents[1];
            }

            // or throw
            return string.Empty;
        }

        public static string GetPropertyPathIndexedPropertyName(string path)
        {
            // IMPROVE: handling of how property paths are indexed - must be <indexedPropertyName> [ <index> , <index> ]  with no spaces
            var pathComponents = path.Split(new [] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
            if (pathComponents.Length == 2)
            {
                return pathComponents[0];
            }

            return path;
        }

    }
}