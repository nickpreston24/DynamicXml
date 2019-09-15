using DynamicXml.Extensions;
using Shared;
using Shared.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace DynamicXml
{
    /// <summary>
    /// <references>
    /// Convert XML text directly into dynamic objects or defined POCO classes.
    /// https://stackoverflow.com/questions/13171525/converting-xml-to-a-dynamic-c-sharp-object
    /// https://www.codeproject.com/Articles/461677/Creating-a-dynamic-object-from-XML-using-ExpandoOb
    /// https://codereview.stackexchange.com/questions/1002/mapping-expandoobject-to-another-object-type Todo: read the class comments in the 1st response example.  If true, don't need to create Dictionary<string, PropertyInfo>
    /// </references>
    /// </summary>
    public static partial class DynamicExtensions
    {
        private static StringComparison comparison = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Caches xml inside an XDocument, creating some overhead.
        /// todo: put in your XmlStreamer class
        /// </summary>
        public static T Extract<T>(string xml) where T : class
        {
            T result;

            using (TimeIt.GetTimer())
            {
                if (string.IsNullOrWhiteSpace(xml))
                    return default;

                string className = typeof(T).Name;
                var xmlDocument = XDocument.Parse(xml);
                string node = xmlDocument.GetFirstNode(className);

                if (node == null)
                    throw new Exception($"Could not find element '{className}' in xml");

                xmlDocument = XDocument.Parse(node);

                var dictionary = xmlDocument.Root.ToDynamic() as ExpandoObject;
                result = dictionary.ToInstance<T>();
            }

            return result;
        }

        public static dynamic ToDynamic(this XDocument xDocument) =>
            xDocument.Root.ToDynamic() as IDictionary<string, object>;

        public static dynamic ToDynamic(this XElement node)
        {
            var parent = new ExpandoObject();
            return node.ToDynamic(parent);
        }

        public static T ToInstance<T>(this IDictionary<string, object> dictionary) where T : class
        {
            T instance;
            using (TimeIt.GetTimer())
            {
                var type = typeof(T);
                instance = (T)ToInstance(Activator.CreateInstance(type, true), dictionary, type);
            }

            return instance;
        }

        public static object ToInstance(this IDictionary<string, object> dictionary, Type type)
        {
            return (type.IsClass)
                ? ToInstance(Activator.CreateInstance(type, true), dictionary, type)
                : new Dictionary<string, object>(0);
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            switch (parent)
            {
                case ICollection<dynamic> list:
                    list.Add(value);
                    break;

                default:
                    ((IDictionary<string, object>)parent)[name] = value;
                    break;
            }
        }

        private static void AssignChild(object parent, PropertyInfo property,
            IDictionary<string, object> childDictionary)
        {
            var childType = property?.PropertyType;
            var baseType = childType?.BaseType ?? childType;

            object nextChildInstance = CreateChild(childDictionary, childType);

            var childElementType = childType?.GetGenericArguments().SingleOrDefault();

            if (baseType == typeof(Array))
            {
                property.SetValue(parent, nextChildInstance);
            }
            else switch (nextChildInstance)
                {
                    case IList list:
                        {
                            var classList = childElementType.ToListType() ?? default(IList);

                            foreach (object product in list)
                            {
                                classList?.Add(product);
                            }

                            property?.SetValue(parent, classList);
                            break;
                        }
                    default:
                        property?.SetValue(parent, nextChildInstance);
                        break;
                }
        }

        private static object CreateChild(IDictionary<string, object> childDictionary, Type childType)
        {
            var baseType = childType.BaseType ?? childType;
            var parent = Activator.CreateInstance(childType, true);

            object child;
            if (baseType == typeof(Array) || childType.IsIEnumerable())
            {
                var elementType = baseType == typeof(Array)
                    ? childType.GetElementType()
                    : childType.GetGenericArguments().Single();

                var list = new List<object>(childDictionary.Values.Count);

                list.AddRange(from IEnumerable expandoCollection in childDictionary.Values
                              from ExpandoObject dictionary in expandoCollection
                              let next = ToInstance(dictionary, elementType)
                              select next);

                if (baseType != typeof(Array))
                {
                    child = list;
                }
                else
                {
                    var childArray = Array.CreateInstance(elementType, list.Count);
                    var source = list.Cast<object>().ToArray();
                    Array.Copy(source, childArray, list.Count);
                    child = childArray;
                }
            }
            else
            {
                child = ToInstance(parent, childDictionary, childType) ?? Activator.CreateInstance(childType);
            }

            return child;
        }

        private static object GetDefaultValue(Type type) => (type.IsValueType) ? Activator.CreateInstance(type) : null;

        private static dynamic ToDynamic(this XElement node, dynamic parent)
        {
            if (!node.HasElements)
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
            else if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
            {
                var item = new ExpandoObject();
                var list = new List<dynamic>();
                foreach (var element in node.Elements())
                {
                    ToDynamic(element, list);
                }

                AddProperty(item, node.Elements().First().Name.LocalName, list);
                AddProperty(parent, node.Name.ToString(), item);
            }
            else
            {
                var item = new ExpandoObject();

                foreach (var attribute in node.Attributes())
                {
                    AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                }

                foreach (var element in node.Elements())
                {
                    ToDynamic(element, item);
                }

                AddProperty(parent, node.Name.ToString(), item);
            }

            return parent;
        }

        private static object ToInstance(object parent, IDictionary<string, object> dictionary, Type childType)
        {
            var parentType = parent.GetType();
            string parentTypeName = parentType.Name;

            var childProperties = _properties[childType];
            StringBuilder errorMessages = new StringBuilder();

            foreach (var pair in dictionary ?? new Dictionary<string, object>(0))
            {
                var value = pair.Value;
                var type = value.GetType();

                try
                {
                    if (!type.Name.Equals(nameof(ExpandoObject)))
                    {
                        var nextProperty = childProperties
                            .SingleOrDefault(childProperty => childProperty.Name.Equals(pair.Key, comparison));

                        if (string.IsNullOrWhiteSpace(value.ToString()))
                            value = GetDefaultValue(nextProperty.PropertyType);

                        if (value == null)
                        {
                            Debug.WriteLine($"[Warning!]: Property '{nextProperty.Name}', " +
                                                     $"of type '{nextProperty.PropertyType.Name}' is null and could not be assigned!");
                            continue;
                        }

                        nextProperty?.SetValue(parent, TypeDescriptor.GetConverter(nextProperty.PropertyType)
                            .ConvertFrom(value), null);

                        continue;
                    }

                    IDictionary<string, object> nextChildDictionary = value as ExpandoObject;
                    string propertyName = pair.Key;

                    if (propertyName.Equals(parentTypeName, comparison))
                    {
                        var nextParent = Activator.CreateInstance(childType, true);
                        var child = ToInstance(nextParent, nextChildDictionary,
                            childType);

                        parent = child;
                    }
                    else
                    {
                        var nextProperty = childProperties
                            .SingleOrDefault(property =>
                                property.Name.Equals(propertyName, comparison)
                                || property.PropertyType.Name.Equals(propertyName, comparison));

                        if (nextProperty == null)
                        {
                            Debug.WriteLine($"Info: Could not find property '{propertyName}' in class {parentTypeName}");
                            continue;
                        }

                        AssignChild(parent, nextProperty, nextChildDictionary);
                    }
                }
                catch (Exception ex)
                {
                    string message = new StringBuilder()
                        .AppendFormat($"Encountered an error when populating an instance of '{parentTypeName}' ")
                        .AppendFormat($"with element '{pair.Key}':\n {ex}")
                        .AppendFormat("\nEnsure XML elements match the instance schema!")
                        .ToString();

                    Debug.WriteLine(message);
                }
            }

            return parent;
        }
    }

    /// <summary>
    /// Extra extensions from the web which may or may not be useful
    /// </summary>
    public static partial class DynamicExtensions
    {
        public static DataTable ToDataTable(this List<dynamic> list)
        {
            var table = new DataTable();
            var properties = list.GetType().GetProperties();
            properties = properties.ToList().GetRange(0, properties.Count() - 1).ToArray();
            properties.ToList().ForEach(p => table.Columns.Add(p.Name, typeof(string)));
            list.ForEach(x => table.Rows.Add(x.Name, x.Phone));
            return table;
        }

        public static dynamic ToDynamic(this IDictionary<string, object> dictionary)
        {
            dynamic expando = new ExpandoObject();
            var expandoDictionary = (IDictionary<string, object>)expando;

            //todo: make recursive for objects in the dictionary with more levels.
            dictionary.ToList()
                .ForEach(keyValue => expandoDictionary.Add(keyValue.Key, keyValue.Value));

            return expando;
        }

        public static dynamic ToDynamic<T>(this T obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            var properties = typeof(T).GetProperties();

            foreach (var propertyInfo in properties)
            {
                var propertyExpression = Expression.Property(Expression.Constant(obj), propertyInfo);
                string currentValue = Expression.Lambda<Func<string>>(propertyExpression).Compile().Invoke();
                expando.Add(propertyInfo.Name.ToLower(), currentValue);
            }

            return (ExpandoObject)expando;
        }
    }
}