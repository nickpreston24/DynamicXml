using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DynamicXml
{
    /// <summary>
    /// <references>
    /// https://stackoverflow.com/questions/13171525/converting-xml-to-a-dynamic-c-sharp-object
    /// https://www.codeproject.com/Articles/461677/Creating-a-dynamic-object-from-XML-using-ExpandoOb
    /// </references>
    /// </summary>
    /// <param name="xDocument"></param>
    /// <returns></returns>
    public static partial class DynamicExtensions
    {
        private static StringComparison _comparison = StringComparison.OrdinalIgnoreCase;
        private static bool _throwErrors;

        //todo: put in your XmlMapper class
        public static T Extract<T>(string xml, bool errorOnMismatch = false) where T : class
        {
#if DEBUG
            var watch = new Stopwatch();
            watch.Start();
#endif
            _throwErrors = errorOnMismatch;

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

            xmlDocument = null;
            xmlDocument = XDocument.Parse(node);
            var dictionary = xmlDocument.Root.ToDynamic() as ExpandoObject;
            var result = dictionary.ToInstance<T>();

#if DEBUG
            watch.Stop();
            var elapsedTime = watch.Elapsed;
            Debug.WriteLine($"{ MethodBase.GetCurrentMethod().Name }() Time Elapsed: {elapsedTime.TotalMilliseconds} ms");
#endif

            return result as T;
        }

        public static T ToInstance<T>(this IDictionary<string, object> dictionary) where T : class
        {
#if DEBUG
            var watch = new Stopwatch();
            watch.Start();
#endif

            var type = typeof(T);
            var instance = (T)ToInstance(Activator.CreateInstance(type, true), dictionary, type);

#if DEBUG
            watch.Stop();
            var elapsedTime = watch.Elapsed;
            Debug.WriteLine($"{ MethodBase.GetCurrentMethod().Name }() Time Elapsed: {elapsedTime.TotalMilliseconds} ms");
#endif

            return instance;
        }

        public static object ToInstance(this IDictionary<string, object> dictionary, Type type)
        {
            //todo: check whether type is a class            
            object instance = ToInstance(Activator.CreateInstance(type, true), dictionary, type);
            return instance;
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
                    //Debug.WriteLine($"Class: {parentTypeName}\tElement: {pair.Key.ToString()} raw Value: {value.ToString()}\ttype: {valType.ToString()}");                    

                    if (!type.Name.Equals(nameof(ExpandoObject)))
                    {
                        var nextProperty = childProperties
                            .SingleOrDefault(childProperty => childProperty.Name.Equals(pair.Key, _comparison));

                        if (string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            value = GetDefault(nextProperty.PropertyType);
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
                            .SingleOrDefault(pi => pi.Name.Equals(propertyName, _comparison)
                                || pi.PropertyType.Name.Equals(propertyName, _comparison));

                        if (nextProperty == null)
                        {
                            Debug.WriteLine($"Info: Could not find property '{propertyName}'");
                            continue;
                        }

                        var nextChildType = nextProperty?.PropertyType;

                        object nextChildInstance = CreateChild(nextChildDictionary, nextChildType);
                        var childBaseType = nextChildInstance.GetType();

#if !(NET20 || NET35 || NET40 || NET45 || NET451 || NET452)
                        if (nextChildInstance is IEnumerable list)
                        {
                            nextProperty.SetValue(parent, list);
                        }
#else
                        if (nextChildInstance is IEnumerable)
                        {
                            var list = nextChildInstance as IEnumerable;
                            nextProperty.SetValue(parent, list);
                        }
#endif
                        else
                        {
                            nextProperty.SetValue(parent, nextChildInstance);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (_throwErrors)
                    {
                        string message = $"Encountered an error when converting to instance of '{parentTypeName}' from element '{pair.Key.ToString()}':\n {ex.Message}\nEnsure XML elements match the instance schema!";

                        Debug.WriteLine(message);
                        continue;
                    }
                }
            }

            return parent;
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

        private static object CreateChild(IDictionary<string, object> childDictionary, Type childType)
        {
            object child = null;

            if (childType.BaseType.Equals(typeof(Array)))
            {
                var elementType = childType.GetElementType();
                var list = new List<object>(childDictionary.Values.Count);

                foreach (IEnumerable expandos in childDictionary.Values)
                {
                    foreach (ExpandoObject expando in expandos)
                    {
                        object next = ToInstance(expando, elementType);
                        list.Add(next);
                    }
                }

                var childArray = Array.CreateInstance(elementType, list.Count);
                var source = list.Cast<object>().ToArray();
                Array.Copy(source, childArray, list.Count);

                child = childArray;
            }
            else
            {
                object parent = Activator.CreateInstance(childType, true);
                child = ToInstance(parent, childDictionary, childType) ?? Activator.CreateInstance(childType);
            }

            return child;
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<string, object>)[name] = value;
            }
        }

        private static object GetDefault(Type type)
        {
            return (type.IsValueType) ? Activator.CreateInstance(type) : null;
        }

        //#region Need Testing
        //public static dynamic ToDynamic(this IDictionary<string, object> dictionary)
        //{
        //    dynamic expando = new ExpandoObject();
        //    var expandoDictionary = (IDictionary<string, object>)expando;

        //    //todo: make recursive for objects in the dictionary with more levels.
        //    dictionary.ToList()
        //              .ForEach(keyValue => expandoDictionary.Add(keyValue.Key, keyValue.Value));

        //    return expando;
        //}

        //public static dynamic ToDynamic<T>(this T obj)
        //{
        //    IDictionary<string, object> expando = new ExpandoObject();
        //    var properties = typeof(T).GetProperties();

        //    foreach (var propertyInfo in properties ?? Enumerable.Empty<PropertyInfo>())
        //    {
        //        var propertyExpression = Expression.Property(Expression.Constant(obj), propertyInfo);
        //        string currentValue = Expression.Lambda<Func<string>>(propertyExpression).Compile().Invoke();
        //        expando.Add(propertyInfo.Name.ToLower(), currentValue);
        //    }
        //    return expando as ExpandoObject;
        //}

        //public static DataTable ToDataTable(this List<dynamic> list)
        //{
        //    DataTable table = new DataTable();
        //    var properties = list.GetType().GetProperties();
        //    properties = properties.ToList().GetRange(0, properties.Count() - 1).ToArray();
        //    properties.ToList().ForEach(p => table.Columns.Add(p.Name, typeof(string)));
        //    list.ForEach(x => table.Rows.Add(x.Name, x.Phone));
        //    return table;
        //}
        //#endregion Need Testing
    }

    public static partial class DynamicExtensions
    {
        /// <summary>
        /// Slower and misses chars.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        [Obsolete]
        public static T DeserializeXML<T>(this string xml)
          where T : class
        {
#if DEBUG
            var watch = new Stopwatch();
            watch.Start();
#endif
            T value;
            using (TextReader reader = new StringReader(xml))
            {
                value = new XmlSerializer(typeof(T)).Deserialize(reader) as T;
            }

#if DEBUG
            watch.Stop();
            var elapsedTime = watch.Elapsed;
            Debug.WriteLine("{0}() Time Elapsed: {1} ", System.Reflection.MethodBase.GetCurrentMethod().Name, elapsedTime);
#endif

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
