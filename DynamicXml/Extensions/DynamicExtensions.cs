using DynamicXml.Extensions;
using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using static DynamicXml.FunctionalExtensions;

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
        private static StringComparison _comparison = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Caches xml inside an XDocument, creating some overhead.
        /// todo: put in your XmlStreamer class
        /// </summary>
        public static T Extract<T>(string xml, bool errorOnMismatch = false) where T : class
        {
            T result = default(T);

            using (var timer = new TimeIt())
            {
                if (string.IsNullOrWhiteSpace(xml))
                {
                    return default(T);
                }

                string className = typeof(T).Name;
                var xmlDocument = XDocument.Parse(xml);
                string node = xmlDocument.GetFirstNode(className);

                if (node == null)
                {
                    throw new Exception($"Could not find element '{className}'");
                }

                //Todo: replace the following segment with code that goes from the first node.
                xmlDocument = null;
                xmlDocument = XDocument.Parse(node);

                var dictionary = xmlDocument.Root.ToDynamic() as ExpandoObject;
                result = dictionary.ToInstance<T>();

            }
            return result as T;
        }

        public static dynamic ToDynamic(this XDocument xDocument) => xDocument.Root.ToDynamic() as IDictionary<string, object>;

        public static dynamic ToDynamic(this XElement node)
        {
            var parent = new ExpandoObject();
            return node.ToDynamic(parent);
        }

        private static dynamic ToDynamic(this XElement node, dynamic parent)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
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
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }

            return parent;
        }


        public static T ToInstance<T>(this IDictionary<string, object> dictionary) where T : class
        {
            T instance = default(T);
            using (var timer = new TimeIt())
            {
                var type = typeof(T);
                instance = (T)ToInstance(Activator.CreateInstance(type, true), dictionary, type);
            }
            return instance;
        }

        public static object ToInstance(this IDictionary<string, object> dictionary, Type type)
        {
            return (type.IsClass) ? ToInstance(Activator.CreateInstance(type, true), dictionary, type) : new Dictionary<string, object>(0);
        }

        private static object ToInstance(object parent, IDictionary<string, object> dictionary, Type childType)
        {
            var parentType = parent.GetType();
            string parentTypeName = parentType.Name;

            var parentProprties = _properties[parentType];
            var childProperties = _properties[childType];

            foreach (var pair in dictionary ?? new Dictionary<string, object>(0))
            {
                object value = pair.Value;
                var type = value.GetType();

                try
                {
                    //Debug.WriteLine($"Class: {parentTypeName}\nElement: {pair.Key.ToString()} raw Value: {value.ToString()}\ntype: {type.Name}\n");

                    if (!type.Name.Equals(nameof(ExpandoObject)))
                    {
                        var nextProperty = childProperties
                            .SingleOrDefault(childProperty => childProperty.Name.Equals(pair.Key, _comparison));

                        if (string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            value = GetDefaultValue(nextProperty.PropertyType);
                        }

                        if (value == null)
                        {
                            Debug.WriteLine($"[Warning!]: Property '{nextProperty.Name}', of type '{nextProperty.PropertyType.Name}' is null and could not be assigned!");
                            continue;
                        }

                        nextProperty?.SetValue(parent, TypeDescriptor.GetConverter(nextProperty.PropertyType)
                                .ConvertFrom(value), null);

                        continue;
                    }

                    IDictionary<string, object> nextChildDictionary = value as ExpandoObject;
                    string propertyName = pair.Key.ToString();

                    if (propertyName.Equals(parentTypeName, _comparison))
                    {
                        object childTemplate = Activator.CreateInstance(childType, true);
                        object child = ToInstance(parent: childTemplate, dictionary: nextChildDictionary, childType: childType);

                        parent = child;
                    }
                    else
                    {
                        var nextProperty = childProperties
                            .SingleOrDefault(property => property.Name.Equals(propertyName, _comparison)
                                || property.PropertyType.Name.Equals(propertyName, _comparison));

                        if (nextProperty == null)
                        {
                            Debug.WriteLine($"Info: Could not find property '{propertyName}'");
                            continue;
                        }

                        var nextChildType = nextProperty?.PropertyType;

                        AssignChild(parent, nextProperty, nextChildDictionary);
                    }
                }
                catch (Exception ex)
                {
                    string message = new StringBuilder()
                        .AppendFormat($"Encountered an error when populating an instance of '{parentTypeName}'")
                        .AppendFormat($"with element '{pair.Key.ToString()}':\n {ex.ToString()}")
                        .AppendFormat($"\nEnsure XML elements match the instance schema!")
                        .ToString();

                    Debug.WriteLine(message);
                    continue;
                }
            }

            return parent;
        }

        private static object CreateChild(IDictionary<string, object> childDictionary, Type childType)
        {
            object child = new object();

            var baseType = childType.BaseType ?? childType;

            if (baseType.Equals(typeof(Array))
                || childType.IsIEnumerableOfT())
            {
                var elementType = baseType.Equals(typeof(Array))
                    ? childType.GetElementType()
                    : childType.GetGenericArguments().Single();

                var list = new List<object>(childDictionary.Values.Count);

                //1.
                foreach (IEnumerable expandos in childDictionary.Values)
                {
                    foreach (ExpandoObject expando in expandos) //todo: fix issue where a string is percieved as an expando of chars.
                    {
                        object next = ToInstance(expando, elementType);
                        list.Add(next);
                    }
                }
                //2.
                if (baseType.Equals(typeof(Array)))
                {
                    var childArray = Array.CreateInstance(elementType, list.Count);
                    object[] source = list.Cast<object>().ToArray();
                    Array.Copy(source, childArray, list.Count);
                    child = childArray;
                }
                else
                {
                    child = list;
                }
            }
            else
            {
                object parent = Activator.CreateInstance(childType, true);
                child = ToInstance(parent, childDictionary, childType) ?? Activator.CreateInstance(childType);
            }

            return child;
        }

        private static void AssignChild(object parent, PropertyInfo property, IDictionary<string, object> childDictionary)
        {
            var childType = property?.PropertyType;
            var baseType = childType.BaseType ?? childType;

            object nextChildInstance = CreateChild(childDictionary, childType);

            var childElementType = childType.GetGenericArguments().SingleOrDefault();

            if (baseType.Equals(typeof(Array)))
            {
                property.SetValue(parent, nextChildInstance);
            }
            else if (nextChildInstance is IList list)
            {
                var classList = childElementType.ToListType() ?? default(IList);

                foreach (object product in list)
                {
                    classList.Add(product);
                }

                property.SetValue(parent, classList);
            }
            else
            {
                property.SetValue(parent, nextChildInstance);
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic> list)
            {
                list.Add(value);
            }
            else
            {
                (parent as IDictionary<string, object>)[name] = value;
            }
        }

        private static object GetDefaultValue(Type type) => (type.IsValueType) ? Activator.CreateInstance(type) : null;

    }

    /// <summary>
    /// Extra extensions from the web which may or may not be useful
    /// </summary>
    public static partial class DynamicExtensions
    {
        #region Need Testing
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

            foreach (var propertyInfo in properties ?? Enumerable.Empty<PropertyInfo>())
            {
                var propertyExpression = Expression.Property(Expression.Constant(obj), propertyInfo);
                string currentValue = Expression.Lambda<Func<string>>(propertyExpression).Compile().Invoke();
                expando.Add(propertyInfo.Name.ToLower(), currentValue);
            }
            return expando as ExpandoObject;
        }
        public static DataTable ToDataTable(this List<dynamic> list)
        {
            var table = new DataTable();
            var properties = list.GetType().GetProperties();
            properties = properties.ToList().GetRange(0, properties.Count() - 1).ToArray();
            properties.ToList().ForEach(p => table.Columns.Add(p.Name, typeof(string)));
            list.ForEach(x => table.Rows.Add(x.Name, x.Phone));
            return table;
        }
        #endregion Need Testing
    }

    /// <summary>
    /// The old standard of Serializing and Deserializing POCO
    /// </summary>
    public static partial class DynamicExtensions
    {
        /// <summary>
        /// Slower and misses chars.
        /// </summary>
        [Obsolete]
        public static T DeserializeXML<T>(this string xml)
          where T : class
        {
            T value;
            using (var timer = new TimeIt())
            using (TextReader reader = new StringReader(xml))
            {
                value = new XmlSerializer(typeof(T)).Deserialize(reader) as T;
            }

            return value;
        }

        public static string SerializeAsXml<T>(this T @object)
            where T : class
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, @object);
                    return writer.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Probably garbage idea.  Safe to remove or ignore.
    /// </summary>
    public class DynamicAliasAttribute : Attribute
    {
        private string alias;
        public string Alias { get => alias; set => alias = value; }
        public DynamicAliasAttribute(string alias)
        {
            this.alias = alias;
        }
    }
}
